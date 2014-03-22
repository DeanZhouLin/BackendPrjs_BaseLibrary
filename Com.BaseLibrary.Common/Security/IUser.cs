using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.BaseLibrary.Common.Logging;
using Com.BaseLibrary.Contract;
using Com.BaseLibrary.Entity;

namespace Com.BaseLibrary.Common.Security
{

    public interface IUser
    {
        int UserId { get; }
        int MerchantID { get; }
        int CurrentMerchantID { get; set; }
        string CurrentMerchantName { get; }
        List<UserMerchant> MerchantList { get; }
        string MerchantName { get; }
        string UserName { get; set; }
        bool HasPermssion(string resourceName);
        void Logout();
        void Login(string userName, int userId);
        bool IsLogin { get; }
        bool IsAdmin { get; }
        List<Resource> GetUserResouceList();
        List<Resource> GetAllSubResouceList(int resourceId);
        List<Resource> GetPageControlResouceList(int pageResourceId);
        // List<UserMerchant> GetCurrentPageMerchantList(string rawUrl);
        List<UserMerchant> PageMerchantList { get; }

        #region AddByDean  For 多级审批

        /// <summary>
        /// 判断控件是否可见
        /// </summary>
        /// <param name="ctlResourceName">控件的ID</param>
        /// <param name="perAuditUserName">前审批人Name</param>
        /// <param name="crrentPageID">当前页面ID</param>
        /// <param name="pageControlResouceList"></param>
        /// <returns>用户审批权限资源</returns>
        UserAuditWorkFlowResource GetUserAuditWorkFlowResource(string ctlResourceName, string perAuditUserName, int crrentPageID, List<Resource> pageControlResouceList);

        #endregion

    }

    public interface IUserFactory
    {
        IUser CreateUser();
    }

    [Serializable]
    public class Resource
    {
        [DataMapping]
        public int ID { get; set; }
        [DataMapping]
        public int ParentID { get; set; }
        [DataMapping]
        public int MerchantID { get; set; }
        [DataMapping]
        public int Status { get; set; }
        [DataMapping]
        public int DisplayOrder { get; set; }
        [DataMapping]
        public string DisplayName { get; set; }
        [DataMapping]
        public string ResourceName { get; set; }
        [DataMapping]
        public string ResourceAddress { get; set; }
        [DataMapping]
        public int ShowInMenu { get; set; }
        [DataMapping]
        public int ResourceType { get; set; }

        public Resource ParentResource { get; set; }
        public List<Resource> SubResourceList { get; set; }

        private List<string> m_ControlNameList;
        public List<string> ControlIDList
        {
            get
            {
                return m_ControlNameList ?? (m_ControlNameList = ResourceAddress.Split('#').ToList());
            }
        }

    }

    public class UserMerchant
    {
        [DataMapping]
        public int MerchantID { get; set; }
        [DataMapping]
        public string MerchantName { get; set; }

        public string MechantIDAndName
        {
            get
            {
                return MerchantID + " - " + MerchantName;
            }
        }
    }

    #region AddByDean For 多级审批
    [Serializable]
    public sealed class UserAuditWorkFlowResource : ErrorInfoBase
    {
        /// <summary>
        /// 是否能够审批（控件是否显示）
        /// </summary>
        public bool HasAuth { get; set; }

        /// <summary>
        /// 是否是最后一步审批
        /// </summary>
        public bool IsFinalStep { get; set; }

        /// <summary>
        /// 当前记录等待审批的级数
        /// </summary>
        public int WaitAuditStep { get; set; }

        /// <summary>
        /// 总共审批级数
        /// </summary>
        public int TotalAuditStepCount
        {
            get
            {
                int totalStepCount = GetUpperStepCount(CurrentWorkFlowResource) + GetLowerStepCount(CurrentWorkFlowResource) + 1;
                return totalStepCount;
            }
        }

        /// <summary>
        /// 当前用户最高的审批级数
        /// </summary>
        public int CurrentAuditStep
        {
            get
            {
                int currentAuditStep = GetLowerStepCount(CurrentWorkFlowResource) + 1;
                return currentAuditStep;
            }
        }

        /// <summary>
        /// 控制资源
        /// </summary>
        public Resource ControlResouce { get; set; }

        /// <summary>
        /// 控制资源包含的控件ID列表
        /// </summary>
        public List<string> ControlIDList
        {
            get
            {
                return ControlResouce == null ? new List<string>() : ControlResouce.ControlIDList;
            }
        }

        /// <summary>
        /// 当前用户的审批流程资源
        /// </summary>
        public AuditWorkFlowResource CurrentWorkFlowResource
        {
            get
            {
                //用户若无授权流程，返回null
                if (AuthedAuditWorkFlowResources != null && AuthedAuditWorkFlowResources.Count > 0)
                {
                    return AuthedAuditWorkFlowResources.Values.OrderByDescending(GetLowerStepCount).First();
                }
                return null;
            }
        }

        public List<int> TotalAuditWorkFlowSteps
        {
            get
            {
                if (AuthedAuditWorkFlowResources != null && AuthedAuditWorkFlowResources.Count > 0)
                {
                    return AuthedAuditWorkFlowResources.Values.Select(c => GetLowerStepCount(c) + 1).OrderBy(c => c).ToList();
                }
                return new List<int>();
            }
        }

        public string TotalAuditWorkFlowStepsStr
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                List<int> t = new List<int>();
                foreach (var item in TotalAuditWorkFlowSteps)
                {
                    if (!t.Contains(item))
                    {
                        t.Add(item);
                        sb.Append(item).Append(",");
                    }
                }
                return sb.ToString().Trim(',');
            }
        }

        /// <summary>
        /// key为ID
        /// 当前页面下 当前用户 所拥有的所有审批流程资源
        /// </summary>
        public Dictionary<int, AuditWorkFlowResource> AuthedAuditWorkFlowResources { get; private set; }

        /// <summary>
        /// 当前页面下所有的审批流程资源
        /// </summary>
        public List<AuditWorkFlowResource> TotalAuditWorkFlowResources { get; private set; }

        public UserAuditWorkFlowResource()
        {
            AuthedAuditWorkFlowResources = new Dictionary<int, AuditWorkFlowResource>();
            TotalAuditWorkFlowResources = new List<AuditWorkFlowResource>();
        }

        public bool ContainsStep(int step)
        {
            foreach (var authedAuditWorkFlowResource in AuthedAuditWorkFlowResources.Values)
            {
                if (GetLowerStepCount(authedAuditWorkFlowResource) + 1 == step)
                {
                    return true;
                }
            }
            return false;
        }

        private int GetUpperStepCount(AuditWorkFlowResource keyResource)
        {
            try
            {
                if (AuthedAuditWorkFlowResources == null || AuthedAuditWorkFlowResources.Count == 0 || keyResource == null)
                {
                    return -1;
                }
                if (TotalAuditWorkFlowResources == null || TotalAuditWorkFlowResources.Count == 0)
                {
                    return -1;
                }
                AuditWorkFlowResource tempResource = keyResource;
                int upperCount = 0;
                while (tempResource != null && tempResource.ParentGroupID != 0)
                {
                    upperCount++;
                    tempResource = TotalAuditWorkFlowResources.Find(c => c.GroupID == tempResource.ParentGroupID);
                }
                return upperCount;
            }
            catch (Exception ex)
            {
                var message = ControlResouce == null ? "ControlResouce为空" : ControlResouce.ID.ToString();
                LogHelper.CustomInfo(message + "|" + ex);
                return -1;
            }
        }

        private int GetLowerStepCount(AuditWorkFlowResource keyResource)
        {
            try
            {
                if (AuthedAuditWorkFlowResources == null || AuthedAuditWorkFlowResources.Count == 0 || keyResource == null)
                {
                    return -1;
                }
                if (TotalAuditWorkFlowResources == null || TotalAuditWorkFlowResources.Count == 0)
                {
                    return -1;
                }
                int lowerCount = 0;
                AuditWorkFlowResource tempResource = TotalAuditWorkFlowResources.Find(c => c.ParentGroupID == keyResource.GroupID);
                while (tempResource != null)
                {
                    lowerCount++;
                    tempResource = TotalAuditWorkFlowResources.Find(c => c.ParentGroupID == tempResource.GroupID);
                }
                return lowerCount;
            }
            catch (Exception ex)
            {
                var message = ControlResouce == null ? "ControlResouce为空" : ControlResouce.ID.ToString();
                LogHelper.CustomInfo(message + "|" + ex);
                return -1;
            }
        }

    }

    [Serializable]
    public sealed class AuditWorkFlowResource
    {

        [DataMapping]
        public int ID { get; set; }

        [DataMapping]
        public string DisplayName { get; set; }


        [DataMapping]
        public Int32 ParentGroupID { get; set; }

        [DataMapping]
        public Int32 GroupID { get; set; }

        [DataMapping]
        public Int32 RoleID { get; set; }

        [DataMapping]
        public Int32 UserID { get; set; }

        [DataMapping]
        public Int32 Status { get; set; }


        public bool IsFinalStep { get; set; }

        [DataMapping]
        public Int32 ResourceID { get; set; }


        //public Int32 ResourceID { get; set; }

        //[DataMapping]
        //public string ResourceIDs { get; set; }

        //private List<int> resourceIDList;
        //public List<int> ResourceIDList
        //{
        //    get
        //    {
        //        if (resourceIDList == null)
        //        {
        //            int i;
        //            resourceIDList = ResourceIDs.Split('#').Where(c => !string.IsNullOrEmpty(c) && int.TryParse(c, out i)).Select(int.Parse).ToList();
        //        }
        //        return resourceIDList;
        //    }
        //}

    }

    [Serializable]
    public sealed class UserRole
    {
        [DataMapping]
        public int UserID { get; set; }

        [DataMapping]
        public int RoleID { get; set; }

        [DataMapping]
        public int MerchantID { get; set; }
    }

    [Serializable]
    public sealed class UserInfo
    {
        [DataMapping]
        public Int32 ID { get; set; }

        [DataMapping]
        public string UserName { get; set; }

        [DataMapping]
        public Int32 IsAdmin { get; set; }
    }

    #endregion

}
