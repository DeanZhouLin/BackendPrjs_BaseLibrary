using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;


namespace Com.BaseLibrary.Utility
{
    //幂值枚举扩展类
    /// <summary>
    /// 幂值枚举扩展类【deanzhou 20131213】
    /// 枚举必须包含[Flags]特性
    /// 枚举项必须包含[Description]特性
    /// 枚举项的值必须以 1,2,4,8 。。。 的形式进行初始值设定
    /// </summary>
    public static class EnumFlagsUtil
    {
        public static bool HasTarget(this int currentValue, int targetValue)
        {
            return targetValue != 0 && (currentValue & targetValue) == targetValue;
        }

        //验证是否包含指定枚举项
        /// <summary>
        /// 验证是否包含指定枚举项
        /// </summary>
        /// <typeparam name="EnumType">枚举类型</typeparam>
        /// <param name="currentIntValue">当前的值</param>
        /// <param name="targetEnum">需要验证的枚举项</param>
        /// <returns>【当前值】是否包含【需要验证的枚举项】</returns>
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

        //获取当前数值对应的在枚举中的各个项的Description的拼接字符串
        /// <summary>
        /// 获取当前数值对应的在枚举中的各个项的Description的拼接字符串
        /// 1 获取当前值在枚举类型中的所有枚举项
        /// 2 获取对应枚举项的Description
        /// 3 将这些Decription使用分隔符进行拼接
        /// 4 返回
        /// </summary>
        /// <typeparam name="EnumType">枚举类型</typeparam>
        /// <param name="currentIntValue">当前的值</param>
        /// <param name="split">字符串分隔符</param>
        /// <returns>当前数值对应的在枚举中的各个项的Description的拼接字符串</returns>
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

        //获取当前数值对应的在枚举中的各个项
        /// <summary>
        /// 获取当前数值对应的在枚举中的各个项
        /// </summary>
        /// <typeparam name="EnumType">枚举类型</typeparam>
        /// <param name="currentIntValue">当前的值</param>
        /// <returns>当前数值对应的在枚举中的各个项</returns>
        public static Dictionary<EnumType, string> GetTargetsByValue<EnumType>(this int currentIntValue) where EnumType : struct
        {
            return GetEnumInfo<EnumType>().
                Where(item => currentIntValue.HasTarget(item.Key)).
                ToDictionary(item => item.Key, item => item.Value);
        }

        //将枚举项转成int
        /// <summary>
        /// 将枚举项转成int
        /// </summary>
        /// <param name="cEnum"></param>
        /// <returns></returns>
        public static int ToInt<EnumType>(this EnumType cEnum) where EnumType : struct
        {
            return Convert.ToInt32(cEnum);
        }

        //根据枚举类型，获取定义的各个枚举项，以及对应的Description信息
        /// <summary>
        /// 根据枚举类型，获取定义的各个枚举项，以及对应的Description信息
        /// </summary>
        /// <typeparam name="EnumType">枚举类型</typeparam>
        /// <returns>枚举各项，以及对应的Description信息</returns>
        private static Dictionary<EnumType, string> GetEnumInfo<EnumType>() where EnumType : struct
        {
            Type type = typeof(EnumType);
            if (!type.IsEnum) throw new Exception("需要传入枚举");

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