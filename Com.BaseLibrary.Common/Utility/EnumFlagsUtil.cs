using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;


namespace Com.BaseLibrary.Utility
{
    //��ֵö����չ��
    /// <summary>
    /// ��ֵö����չ�ࡾdeanzhou 20131213��
    /// ö�ٱ������[Flags]����
    /// ö����������[Description]����
    /// ö�����ֵ������ 1,2,4,8 ������ ����ʽ���г�ʼֵ�趨
    /// </summary>
    public static class EnumFlagsUtil
    {
        public static bool HasTarget(this int currentValue, int targetValue)
        {
            return targetValue != 0 && (currentValue & targetValue) == targetValue;
        }

        //��֤�Ƿ����ָ��ö����
        /// <summary>
        /// ��֤�Ƿ����ָ��ö����
        /// </summary>
        /// <typeparam name="EnumType">ö������</typeparam>
        /// <param name="currentIntValue">��ǰ��ֵ</param>
        /// <param name="targetEnum">��Ҫ��֤��ö����</param>
        /// <returns>����ǰֵ���Ƿ��������Ҫ��֤��ö���</returns>
        public static bool HasTarget<EnumType>(this int currentIntValue, EnumType targetEnum) where EnumType : struct
        {
            return currentIntValue.HasTarget(targetEnum.ToInt());
        }

        public static string GetShowTextByValue<T>(this int currentIntValue, List<T> sourceList, string compPropName = "CodeValue", string showPropName = "CodeText", string split = "|") where T : class
        {
            StringBuilder sb = new StringBuilder();
            Type tType = typeof(T);
            if (sourceList != null)
            {
                foreach (var t in sourceList)
                {
                    var compObj = tType.GetProperty(compPropName).GetValue(t, null);
                    var compStr = compObj == null ? "0" : compObj.ToString();
                    int compInt;
                    if (int.TryParse(compStr, out compInt) && currentIntValue.HasTarget(compInt))
                    {
                        var showObj = tType.GetProperty(showPropName).GetValue(t, null);
                        sb.Append(showObj ?? "").Append(split);
                    }
                }
            }
            return sb.ToString().Trim(split.ToCharArray());
        }

        public static int? SumValue<T>(this List<T> sourceList, string valPropName = "CodeValue")
        {
            int? totalPlatforms;
            Type tType = typeof(T);
            if (sourceList != null && sourceList.Count > 0)
            {
                totalPlatforms = sourceList.Sum(c =>
                {
                    int ii;

                    var valObj = tType.GetProperty(valPropName).GetValue(c, null);
                    var valStr = valObj == null ? "0" : valObj.ToString();
                    int.TryParse(valStr, out ii);
                    return ii;
                });
            }
            else
            {
                totalPlatforms = null;
            }
            return totalPlatforms;
        }

        //��ȡ��ǰ��ֵ��Ӧ����ö���еĸ������Description��ƴ���ַ���
        /// <summary>
        /// ��ȡ��ǰ��ֵ��Ӧ����ö���еĸ������Description��ƴ���ַ���
        /// 1 ��ȡ��ǰֵ��ö�������е�����ö����
        /// 2 ��ȡ��Ӧö�����Description
        /// 3 ����ЩDecriptionʹ�÷ָ�������ƴ��
        /// 4 ����
        /// </summary>
        /// <typeparam name="EnumType">ö������</typeparam>
        /// <param name="currentIntValue">��ǰ��ֵ</param>
        /// <param name="split">�ַ����ָ���</param>
        /// <returns>��ǰ��ֵ��Ӧ����ö���еĸ������Description��ƴ���ַ���</returns>
        public static string GetShowTextByValue<EnumType>(this int currentIntValue, string split = "|") where EnumType : struct
        {
            StringBuilder sb = new StringBuilder();
            var allInfo = currentIntValue.GetTargetsByValue<EnumType>();

            foreach (EnumType platformsType in allInfo.Keys.Where(platformsType => currentIntValue.HasTarget(platformsType)))
            {
                sb.Append(allInfo[platformsType]).Append(split);
            }
            return sb.ToString().Trim(split.ToCharArray());
        }

        //��ȡ��ǰ��ֵ��Ӧ����ö���еĸ�����
        /// <summary>
        /// ��ȡ��ǰ��ֵ��Ӧ����ö���еĸ�����
        /// </summary>
        /// <typeparam name="EnumType">ö������</typeparam>
        /// <param name="currentIntValue">��ǰ��ֵ</param>
        /// <returns>��ǰ��ֵ��Ӧ����ö���еĸ�����</returns>
        public static Dictionary<EnumType, string> GetTargetsByValue<EnumType>(this int currentIntValue) where EnumType : struct
        {
            return GetEnumInfo<EnumType>().
                Where(item => currentIntValue.HasTarget(item.Key)).
                ToDictionary(item => item.Key, item => item.Value);
        }

        //��ö����ת��int
        /// <summary>
        /// ��ö����ת��int
        /// </summary>
        /// <param name="cEnum"></param>
        /// <returns></returns>
        public static int ToInt<EnumType>(this EnumType cEnum) where EnumType : struct
        {
            return Convert.ToInt32(cEnum);
        }

        //����ö�����ͣ���ȡ����ĸ���ö����Լ���Ӧ��Description��Ϣ
        /// <summary>
        /// ����ö�����ͣ���ȡ����ĸ���ö����Լ���Ӧ��Description��Ϣ
        /// </summary>
        /// <typeparam name="EnumType">ö������</typeparam>
        /// <returns>ö�ٸ���Լ���Ӧ��Description��Ϣ</returns>
        private static Dictionary<EnumType, string> GetEnumInfo<EnumType>() where EnumType : struct
        {
            Type type = typeof(EnumType);
            if (!type.IsEnum) throw new Exception("��Ҫ����ö��");

            FieldInfo[] fieldinfo = type.GetFields(BindingFlags.Static | BindingFlags.Public);

            var res = new Dictionary<EnumType, string>();
            foreach (FieldInfo item in fieldinfo)
            {
                Object[] obj = item.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (obj.Length != 0)
                {
                    var key = (EnumType)item.GetValue(null);
                    var value = ((DescriptionAttribute)obj[0]).Description;
                    if (!res.ContainsKey(key))
                    {
                        res.Add(key, value);
                    }
                }
            }
            return res;
        }

    }
}