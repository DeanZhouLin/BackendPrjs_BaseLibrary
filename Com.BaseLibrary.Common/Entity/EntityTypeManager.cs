using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Com.BaseLibrary.Entity
{
    public static class EntityTypeManager
    {
        private static Dictionary<Type, List<PropertyInfo>> TypeDictionary = new Dictionary<Type, List<PropertyInfo>>();
        private static readonly object synObject = new object();
        public static List<PropertyInfo> GetPropertyList(Type type)
        {
            List<PropertyInfo> returnPropertyList = null;
            TypeDictionary.TryGetValue(type, out returnPropertyList);
            if (returnPropertyList == null)
            {
                lock (synObject)
                {
                    TypeDictionary.TryGetValue(type, out returnPropertyList);

                    if (returnPropertyList == null)
                    {
                        PropertyInfo[] propertyList = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        TypeDictionary.Add(type, propertyList.ToList());
                    }

                }
            }
            return TypeDictionary[type];
        }

        public static List<PropertyInfo> GetPropertyList<T>()
        {
            return GetPropertyList(typeof(T));
        }
    }
}
