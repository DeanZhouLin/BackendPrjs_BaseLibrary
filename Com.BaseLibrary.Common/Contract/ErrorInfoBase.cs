using System;
using System.Collections.Generic;
using System.Text;

namespace Com.BaseLibrary.Contract
{

    /// <summary>
    /// 错误类型获取类型
    /// </summary>
    public enum GetErrorInfoType
    {
        /// <summary>
        /// //正常显示错误信息（格式化）
        /// </summary>
        Normal,
        /// <summary>
        /// 直接显示错误信息（无格式）
        /// </summary>
        Short,
        /// <summary>
        /// 正常显示前10条错误信息
        /// </summary>
        Partial
    }

    /// <summary>
    /// 错误类的基类
    /// </summary>
    [Serializable]
    public class ErrorInfoBase : DataContractBase
    {

        /// <summary>
        /// 错误信息缓存列表
        /// </summary>
        private List<string> errorInfo;
        private List<string> ErrorInfo
        {
            get { return errorInfo ?? (errorInfo = new List<string>()); }
        }

        /// <summary>
        /// 错误信息显示头
        /// </summary>
        private string headerStr = "操作完成";
        public string HeaderStr
        {
            get { return headerStr; }
            set { headerStr = value; }
        }

        /// <summary>
        /// 是否包含错误
        /// </summary>
        public bool HasError
        {
            get { return ErrorInfo.Count > 0; }
        }

        #region 公开方法（Public）

        /// <summary>
        /// 获取当前错误信息字符串
        /// </summary>
        /// <param name="getErrorInfoType">获取类型</param>
        /// <returns></returns>
        public string GetError(GetErrorInfoType getErrorInfoType = GetErrorInfoType.Normal)
        {
            switch (getErrorInfoType)
            {
                case GetErrorInfoType.Normal:
                    return GetErrorInfoStr();
                case GetErrorInfoType.Partial:
                    return GetPartialErrorInfoStr();
                case GetErrorInfoType.Short:
                    return GetErrorInfoShortStr();
                default:
                    return "";
            }
        }


        public string GetShowMessage(string successMsg, Action successAction = null, Action failedAction = null, GetErrorInfoType getErrorInfoType = GetErrorInfoType.Normal)
        {
            if (HasError)
            {
                if (failedAction != null)
                {
                    failedAction();
                }
                return GetError(getErrorInfoType);
            }
            if (successAction != null)
            {
                successAction();
            }
            return successMsg;
        }

        /// <summary>
        /// 清空错误信息缓存
        /// </summary>
        public void ClearErrors()
        {
            ErrorInfo.Clear();
        }

        public static string GetQuickError(Exception ex, string header = "操作完成", GetErrorInfoType getErrorInfoType = GetErrorInfoType.Normal)
        {
            if (ex == null)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder(ex.Message);

            if (ex.Message.StartsWith("系统发生错误") && ex.InnerException != null)
            {
                string str = ex.InnerException == null ? "" : ex.InnerException.ToString();
                string shortStr = string.IsNullOrEmpty(str) ? "" : str.Substring(0, Math.Min(100, str.Length));
                sb.Append("-----").Append(shortStr).Append("...");
            }

            ErrorInfoBase eb = new ErrorInfoBase { HeaderStr = header };
            eb.AddError(sb.ToString());
            return eb.GetError(getErrorInfoType);
        }

        public static string GetQuickError(string error, string header = "操作完成", GetErrorInfoType getErrorInfoType = GetErrorInfoType.Normal)
        {
            ErrorInfoBase eb = new ErrorInfoBase { HeaderStr = header };
            eb.AddError(error);
            return eb.GetError(getErrorInfoType);
        }

        public static string GetQuickError(List<string> errors, string header = "操作完成", GetErrorInfoType getErrorInfoType = GetErrorInfoType.Normal)
        {
            ErrorInfoBase eb = new ErrorInfoBase { HeaderStr = header };
            eb.AddRangeError(errors);
            return eb.GetError(getErrorInfoType);
        }

        /// <summary>
        /// 添加错误信息
        /// </summary>
        /// <param name="error">错误信息</param>
        public void AddError(string error)
        {
            ErrorInfo.Add(error);
        }

        public void AddRangeError(List<string> errors)
        {
            ErrorInfo.AddRange(errors);
        }

        public void AddError(ErrorInfoBase errorInfoBase)
        {
            foreach (var error in errorInfoBase.ErrorInfo)
            {
                ErrorInfo.Add(error);
            }
        }

        #endregion

        #region 获取错误信息方法（Private）

        private string GetErrorInfoStr()
        {
            if (ErrorInfo.Count == 0)
            {
                return "";
            }
            StringBuilder sb = new StringBuilder(string.Format("{0}，发生以下错误：\r\n———————————————————————\r\n", HeaderStr));
            int rowCount = 1;
            foreach (var str in ErrorInfo)
            {
                sb.Append(string.Format("{0:000}", rowCount)).Append("：").Append(str).Append("\r\n");
                rowCount++;
            }
            sb.Append("———————————————————————");
            return sb.ToString();

        }

        private string GetPartialErrorInfoStr()
        {
            if (ErrorInfo.Count == 0)
            {
                return "";
            }
            StringBuilder sb = new StringBuilder(string.Format("{0}，产生以下{1}条异常记录\r\n———————————————————————\r\n", HeaderStr, ErrorInfo.Count));
            int rowCount = 1;
            foreach (var str in ErrorInfo)
            {
                if (rowCount <= 10)
                {
                    sb.Append(string.Format("{0:000}", rowCount)).Append("：").Append(str).Append("\r\n");
                }
                else
                {
                    sb.Append(string.Format("{0:000}", rowCount)).Append("：").Append("【余下不显示】。。。").Append("\r\n");
                    break;
                }
                rowCount++;
            }
            sb.Append("———————————————————————");
            return sb.ToString();
        }

        public string GetErrorInfoShortStr()
        {
            if (ErrorInfo.Count == 0)
            {
                return "";
            }
            StringBuilder sb = new StringBuilder();
            foreach (var str in ErrorInfo)
            {
                sb.Append(str).Append("\r\n");
            }
            return sb.ToString();
        }


        #endregion

    }
}
