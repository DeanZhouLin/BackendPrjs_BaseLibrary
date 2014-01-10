using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Com.BaseLibrary.Utility
{
    public static class TypeUtil
    {
        /// <summary>
        /// 获取属性类型的
        /// </summary>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        public static Type GetRealPropertyType(Type propertyType)
        {
            if (propertyType.IsGenericType)
            {
                return propertyType.GetGenericArguments()[0];
            }
            return propertyType;
        }

        public static List<PropertyInfo> GetValuePropertyList(Type type)
        {
            List<PropertyInfo> propertyList = type.GetProperties().ToList();
            propertyList = propertyList.FindAll(c => c.PropertyType.IsValueType || c.PropertyType == typeof(System.String));
            return propertyList;
        }
    }
}
