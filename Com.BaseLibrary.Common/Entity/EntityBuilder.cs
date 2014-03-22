/*****************************************************************
 * Copyright (C) DollMall Corporation. All rights reserved.
 * 
 * Author:   Dolphin Zhang (dolphin@dollmalll.com)
 * Create Date:  08/29/2006 15:06:11
 * Usage:
 *
 * RevisionHistory
 * Date         Author               Description
 * 
*****************************************************************/

using System;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using Com.BaseLibrary.Utility;
using Com.BaseLibrary.Entity;

namespace Com.BaseLibrary.Entity
{

    /// <summary>
    /// Builds an entity.
    /// </summary>
    /// <remarks>
    /// Note to the extenders:
    ///		To enhance performance, many hashtables are used as caches to maintain relevant informantion.
    /// However, these hashtables do not limit the number of objects stored in them. This will occupy too 
    /// much memory if there is a large number of Entity Definitions. In that case, use cache objects with 
    /// expiration functionality. But keep in mind, performance is our top priority so far ^^;
    /// </remarks>
    public static class EntityBuilder
    {
        #region ReferencedTypeBindingInfo
        /// <summary>
        /// Contains data mapping info for a property that is referencing another type.
        /// </summary>
        private class ReferencedTypeBindingInfo
        {
            private ReferencedEntityAttribute m_ReferencedEntityAttribute;
            private PropertyInfo m_PropertyInfo;

            public ReferencedTypeBindingInfo(ReferencedEntityAttribute attri, PropertyInfo propertyInfo)
            {
                m_ReferencedEntityAttribute = attri;
                m_PropertyInfo = propertyInfo;
            }

            /// <summary>
            /// Get the type of the property
            /// </summary>
            public Type Type
            {
                get { return m_ReferencedEntityAttribute.Type; }
            }

            public string Prefix
            {
                get { return m_ReferencedEntityAttribute.Prefix; }
            }

            public string ConditionalProperty
            {
                get { return m_ReferencedEntityAttribute.ConditionalProperty; }
            }

            public PropertyInfo PropertyInfo
            {
                get { return m_PropertyInfo; }
            }
        }
        #endregion

        #region PropertyDataBindingInfo
        /// <summary>
        /// Contains data mapping info for a property in a type
        /// </summary>
        private class PropertyDataBindingInfo
        {
            public DataMappingAttribute DataMapping;
            public PropertyInfo PropertyInfo;
            public PropertyDataBindingInfo(DataMappingAttribute mapping, PropertyInfo propertyInfo)
            {
                DataMapping = mapping;
                PropertyInfo = propertyInfo;
            }
        }
        #endregion

        #region fields
        private static readonly Type s_RootType = typeof(Object);

        /// <summary>
        /// for each type, contains
        ///		string:							column name that could bound to a property
        ///		PropertyDataBindingInfo:		binding info
        /// </summary>
        private static Dictionary<Type, Dictionary<string, PropertyDataBindingInfo>> s_TypeMappingInfo =
            new Dictionary<Type, Dictionary<string, PropertyDataBindingInfo>>();

        /// <summary>
        /// for each type, contains:
        ///		a list of ReferencedTypeBindingInfo that the instance of this type refers to
        /// </summary>
        private static Dictionary<Type, List<ReferencedTypeBindingInfo>> s_TypeReferencedList =
            new Dictionary<Type, List<ReferencedTypeBindingInfo>>();

        /// <summary>
        /// for each type, contains
        ///		string:					property name
        ///		DataMappingAttribute:	data mapping attribute for this property
        /// </summary>
        private static Dictionary<Type, Dictionary<string, DataMappingAttribute>> s_TypePropertyInfo =
            new Dictionary<Type, Dictionary<string, DataMappingAttribute>>();
        private static object s_SyncMappingInfo = new object();
        #endregion

        #region public static functions for building entity
        /// <summary>
        /// Builds the entity.
        /// An exception will be thrown if failed to build the entity.
        /// </summary>
        /// <param name="dr">The dr.</param>
        /// <returns></returns>
        public static T BuildEntity<T>(IDataReader dr) where T : class, new()
        {
            return BuildEntity<T>(new DataReaderEntitySource(dr), string.Empty);
        }

        /// <summary>
        /// Builds the entity.
        /// An exception will be thrown if failed to build the entity.
        /// </summary>
        /// <param name="dr">The dr.</param>
        /// <returns></returns>
        public static T BuildEntity<T>(DataRow dr) where T : class, new()
        {
            return BuildEntity<T>(new DataRowEntitySource(dr), string.Empty);
        }

        /// <summary>
        /// Builds the entity.
        /// An exception will be thrown if failed to build the entity.
        /// </summary>
        /// <param name="dr">The dr.</param>
        /// <returns></returns>
        public static T BuildEntity1<T>(DataRow dr) where T : class, new()
        {
            return BuildEntity1<T>(new DataRowEntitySource(dr), string.Empty);
        }

        /// <summary>
        /// Builds the entity list.
        /// Returns an empty list if the rows contains no data.
        /// An exception will be thrown if failed to build the entity.
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <returns></returns>
        public static List<T> BuildEntityList<T>(DataRow[] rows) where T : class, new()
        {
            if (rows == null)
            {
                return new List<T>(0);
            }
            List<T> list = new List<T>(rows.Length);
            foreach (DataRow row in rows)
            {
                list.Add(BuildEntity<T>(row));
            }
            return list;
        }

        /// <summary>
        /// Builds the entity list.
        /// Returns an empty list if the rows contains no data.
        /// An exception will be thrown if failed to build the entity.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns></returns>
        public static List<T> BuildEntityList<T>(DataTable table) where T : class, new()
        {
            if (table == null)
            {
                return new List<T>(0);
            }
            List<T> list = new List<T>(table.Rows.Count);
            foreach (DataRow row in table.Rows)
            {
                list.Add(BuildEntity<T>(row));
            }
            return list;
        }

        /// <summary>
        /// Builds the entity list.
        /// Returns an empty list if the rows contains no data.
        /// An exception will be thrown if failed to build the entity.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns></returns>
        public static List<T> BuildEntityList1<T>(DataTable table) where T : class, new()
        {
            if (table == null)
            {
                return new List<T>(0);
            }
            if (table.Rows.Count < 2)
                return new List<T>(0);
            List<T> list = new List<T>(table.Rows.Count);
            int rowNO = 3; //记录报错行号

            int colNO = 0;
            try
            {

                colNO = 0;
                foreach (DataRow row in table.Rows)
                {
                    if (colNO == 0)
                    {
                        colNO++;
                        continue;
                    }
                    list.Add(BuildEntity1<T>(row));
                    rowNO++;
                }
            }
            catch (Exception ex)
            {
                //if (ex.Message.StartsWith("列名:"))
                //    throw ex;
                throw new Exception(string.Format("导入数据格式错误：请对照导入模板检查列（列数是否一致？）、数据（格式是否正确？）。\r\n\r\n /*******/ \r\n 开发人员参考：\r\n异常信息第{0}行,第{1}列：{2}", rowNO, colNO, ex.ToString()));
            }
            return list;
        }
        #endregion

        #region private function
        private static T BuildEntity<T>(IEntityDataSource ds, string prefix) where T : class, new()
        {
            T obj = new T();
            FillEntity(ds, obj, typeof(T), prefix);
            return obj;
        }

        private static T BuildEntity1<T>(IEntityDataSource ds, string prefix) where T : class, new()
        {
            T obj = new T();
            FillEntity1(ds, obj, typeof(T), prefix);
            return obj;
        }

        private static Object BuildEntity(IEntityDataSource ds, Type type, string prefix)
        {
            Object obj = Activator.CreateInstance(type);
            FillEntity(ds, obj, type, prefix);
            return obj;
        }


        private static void FillEntity(IEntityDataSource ds, Object obj, Type type, string prefix)
        {
            Type baseType = type.BaseType;
            if (!s_RootType.Equals(baseType))
            {
                FillEntity(ds, obj, baseType, prefix);
            }
            DoFillEntity(ds, obj, type, prefix);
        }
        private static void FillEntity1(IEntityDataSource ds, Object obj, Type type, string prefix)
        {
            Type baseType = type.BaseType;
            if (!s_RootType.Equals(baseType))
            {
                FillEntity1(ds, obj, baseType, prefix);
            }
            DoFillEntity1(ds, obj, type, prefix);
        }

        private static void DoFillEntity1(IEntityDataSource ds, Object obj, Type type, string prefix)
        {
            // fill properties
            string columnError = "";
            int c = 0;
            foreach (string columnName in ds)
            {
                string mappingName;
                mappingName = columnName.ToUpper();
                if (!String.IsNullOrEmpty(prefix))
                {
                    if (mappingName.StartsWith(prefix.ToUpper()))
                    {
                        mappingName = mappingName.Substring(prefix.Length);
                    }
                }

                if (String.IsNullOrEmpty(prefix))
                {
                    prefix = string.Empty;
                }

                PropertyDataBindingInfo propertyBindingInfo = GetPropertyInfo(type, mappingName);
                if (propertyBindingInfo == null || columnName.ToUpper() != (prefix.ToUpper() + mappingName))
                {
                    if (c > 0)
                        columnError += columnName + ",";
                    continue;
                }
                string debug = string.Empty;
                try
                {

                    if (ds[columnName] != DBNull.Value && ValidateData(propertyBindingInfo, ds[columnName]))
                    {
                        Object val = ds[columnName];
                        debug = "列：" + columnName + " 异常|值:" + val.ToString();
                        //if (propertyBindingInfo.PropertyInfo.PropertyType == typeof(string))
                        //{
                            val = val.ToString().Trim();
                        //}
                        if (propertyBindingInfo.PropertyInfo.PropertyType.ToString().Contains("Int32"))
                        {
                            propertyBindingInfo.PropertyInfo.SetValue(obj, val == null || val.ToString()=="" ? 0 : Convert.ToInt32(val), null);
                        }
                        else if (propertyBindingInfo.PropertyInfo.PropertyType.ToString().Contains("Decimal"))
                        {
                            propertyBindingInfo.PropertyInfo.SetValue(obj, val == null || val.ToString() == "" ? 0 : Convert.ToDecimal(val), null);
                        }
                        else if (propertyBindingInfo.PropertyInfo.PropertyType.ToString().Contains("DateTime"))
                        {
                            propertyBindingInfo.PropertyInfo.SetValue(obj, val == null || val.ToString() == "" ? DateTime.Now : Convert.ToDateTime(val), null);
                        }
                        //if (propertyBindingInfo.PropertyInfo.PropertyType.IsEnum || propertyBindingInfo.PropertyInfo.PropertyType.IsValueType)
                        //{
                        //    object item = EnumUtil.GetEnumByValue(propertyBindingInfo.PropertyInfo.PropertyType, val.ToString().Trim());
                        //    propertyBindingInfo.PropertyInfo.SetValue(obj, item, null);

                        //}
                        else
                        {
                            propertyBindingInfo.PropertyInfo.SetValue(obj, val, null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logging.CustomLogger.CurrentLogger.DoWrite("backend", "ItemMgmt", "Exception", "商品导入", debug);
                    throw new Exception("\r\n" + debug + "\r\n___________\r\n以下供开发人员参考\r\n" + ex.ToString());
                }
                c++;
            }
            if (columnError != "")
            {
                throw new Exception("列名:" + columnError.Remove(columnError.Length - 1) + "有误。");
            }
            // fill referenced objects
            List<ReferencedTypeBindingInfo> refList = GetReferenceObjects(type);
            foreach (ReferencedTypeBindingInfo refObj in refList)
            {
                if (TryFill(ds, refObj))
                {
                    refObj.PropertyInfo.SetValue(obj, BuildEntity(ds, refObj.Type, refObj.Prefix), null);
                }
            }
        }

        private static void DoFillEntity(IEntityDataSource ds, Object obj, Type type, string prefix)
        {

            // fill properties
            foreach (string columnName in ds)
            {
                string mappingName;
                mappingName = columnName.ToUpper();
                if (!String.IsNullOrEmpty(prefix))
                {
                    if (mappingName.StartsWith(prefix.ToUpper()))
                    {
                        mappingName = mappingName.Substring(prefix.Length);
                    }
                }

                if (String.IsNullOrEmpty(prefix))
                {
                    prefix = string.Empty;
                }

                PropertyDataBindingInfo propertyBindingInfo = GetPropertyInfo(type, mappingName);
                if (propertyBindingInfo == null || columnName.ToUpper() != (prefix.ToUpper() + mappingName))
                    continue;

                try
                {

                    if (ds[columnName] != DBNull.Value && ValidateData(propertyBindingInfo, ds[columnName]))
                    {
                        Object val = ds[columnName];
                        if (propertyBindingInfo.PropertyInfo.PropertyType == typeof(string))
                        {
                            val = val.ToString().Trim();
                        }
                        if (propertyBindingInfo.PropertyInfo.PropertyType.IsEnum)
                        {
                            object item = EnumUtil.GetEnumByValue(propertyBindingInfo.PropertyInfo.PropertyType, val.ToString().Trim());
                            propertyBindingInfo.PropertyInfo.SetValue(obj, item, null);

                        }
                        else
                        {
                            propertyBindingInfo.PropertyInfo.SetValue(obj, val, null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            // fill referenced objects
            List<ReferencedTypeBindingInfo> refList = GetReferenceObjects(type);
            foreach (ReferencedTypeBindingInfo refObj in refList)
            {
                if (TryFill(ds, refObj))
                {
                    refObj.PropertyInfo.SetValue(obj, BuildEntity(ds, refObj.Type, refObj.Prefix), null);
                }
            }
        }

        private static bool TryFill(IEntityDataSource ds, ReferencedTypeBindingInfo refObj)
        {
            if (string.IsNullOrEmpty(refObj.ConditionalProperty))
            {
                return true;
            }
            string columnName = GetBindingColumnName(refObj.Type, refObj.ConditionalProperty, refObj.Prefix);
            if (columnName == null)
            {
                return false;
            }
            return ds.ContainsColumn(columnName);
        }

        private static string GetBindingColumnName(Type type, string propertyName, string prefix)
        {
            Dictionary<string, DataMappingAttribute> propertyInfos;
            string name = null;
            try
            {
                s_TypePropertyInfo.TryGetValue(type, out propertyInfos);
                if (propertyInfos == null)
                {
                    lock (s_SyncMappingInfo)
                    {
                        s_TypePropertyInfo.TryGetValue(type, out propertyInfos);
                        if (propertyInfos == null)
                        {
                            AddTypeInfo(type);
                            propertyInfos = s_TypePropertyInfo[type];
                        }
                    }
                }
                DataMappingAttribute mapping;
                propertyInfos.TryGetValue(propertyName, out mapping);
                if (mapping == null)
                {
                    name = null;
                }
                else
                {
                    name = mapping.ColumnName;
                }
            }
            catch
            {
                name = null;
            }

            if (name == null)
            {
                if (!s_RootType.Equals(type.BaseType) && !s_RootType.Equals(type))
                {
                    return GetBindingColumnName(type.BaseType, propertyName, prefix);
                }
                else
                {
                    return null;
                }
            }
            return prefix + name;
        }

        /// <summary>
        /// Validate data binding info.
        /// Note: type checking is skipped here.
        /// </summary>
        /// <param name="bindingInfo"></param>
        /// <param name="dbValue"></param>
        /// <returns></returns>
        private static bool ValidateData(PropertyDataBindingInfo bindingInfo, Object dbValue)
        {
            return true;
        }

        /// <summary>
        /// Get the property binding info.
        /// Returns null if no relevant binding info is found.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        private static PropertyDataBindingInfo GetPropertyInfo(Type type, string columnName)
        {
            Dictionary<string, PropertyDataBindingInfo> propertyInfoList;
            try
            {
                s_TypeMappingInfo.TryGetValue(type, out propertyInfoList);
                if (propertyInfoList == null)
                {
                    lock (s_SyncMappingInfo)
                    {
                        s_TypeMappingInfo.TryGetValue(type, out propertyInfoList);
                        if (propertyInfoList == null)
                        {
                            AddTypeInfo(type);
                            propertyInfoList = s_TypeMappingInfo[type];
                        }
                    }
                }
            }
            catch
            {
                // EntityBuilderLogger.LogGetPropertyBindingInfoException(type, columnName, e);
                return null;
            }

            PropertyDataBindingInfo info;
            propertyInfoList.TryGetValue(columnName, out info);
            return info;
        }

        private static List<ReferencedTypeBindingInfo> GetReferenceObjects(Type type)
        {
            List<ReferencedTypeBindingInfo> list;
            s_TypeReferencedList.TryGetValue(type, out list);
            if (list == null)
            {
                lock (s_SyncMappingInfo)
                {
                    s_TypeReferencedList.TryGetValue(type, out list);
                    if (list == null)
                    {
                        AddTypeInfo(type);
                        list = s_TypeReferencedList[type];
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// If no relevant properties exist, an empty hashtable and list is returned.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dataMappingInfos"></param>
        /// <param name="referObjs"></param>
        private static void GetTypeInfo(Type type, out Dictionary<string, PropertyDataBindingInfo> dataMappingInfos,
            out List<ReferencedTypeBindingInfo> referObjs,
            out Dictionary<string, DataMappingAttribute> propertyInfos)
        {
            PropertyInfo[] propertyList = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            dataMappingInfos = new Dictionary<string, PropertyDataBindingInfo>();
            referObjs = new List<ReferencedTypeBindingInfo>();
            propertyInfos = new Dictionary<string, DataMappingAttribute>(new StringEqualityComparer());

            foreach (PropertyInfo propertyInfo in propertyList)
            {
                Object[] attributes = propertyInfo.GetCustomAttributes(false);
                foreach (Object attribute in attributes)
                {
                    // properties binding to a data column
                    if (attribute is DataMappingAttribute)
                    {
                        DataMappingAttribute obj = attribute as DataMappingAttribute;
                        if (StringUtil.IsNullOrEmpty(obj.ColumnName))
                        {
                            obj.ColumnName = propertyInfo.Name;
                        }
                        dataMappingInfos[obj.ColumnName.ToUpper()] = new PropertyDataBindingInfo(obj, propertyInfo);
                        propertyInfos.Add(propertyInfo.Name, obj);
                        continue;
                    }

                    // properties that are referenced objects
                    if (attribute is ReferencedEntityAttribute)
                    {
                        ReferencedEntityAttribute obj = attribute as ReferencedEntityAttribute;
                        if (obj.Type == null)
                        {
                            obj.Type = propertyInfo.PropertyType;
                        }
                        referObjs.Add(new ReferencedTypeBindingInfo(obj, propertyInfo));
                    }
                }
            }
        }

        private static void AddTypeInfo(Type type)
        {
            // EntityBuilderLogger.LogAddTypeInfo(type);

            Dictionary<Type, Dictionary<string, PropertyDataBindingInfo>> newMappingList =
                new Dictionary<Type, Dictionary<string, PropertyDataBindingInfo>>(s_TypeMappingInfo);
            Dictionary<Type, List<ReferencedTypeBindingInfo>> newReferencedObjects =
                new Dictionary<Type, List<ReferencedTypeBindingInfo>>(s_TypeReferencedList);
            Dictionary<Type, Dictionary<string, DataMappingAttribute>> newPropertyList =
                new Dictionary<Type, Dictionary<string, DataMappingAttribute>>(s_TypePropertyInfo);

            Dictionary<string, PropertyDataBindingInfo> mappingInfos;
            List<ReferencedTypeBindingInfo> referObjs;
            Dictionary<string, DataMappingAttribute> propertyInfos;
            GetTypeInfo(type, out mappingInfos, out referObjs, out propertyInfos);

            newMappingList[type] = mappingInfos;
            newReferencedObjects[type] = referObjs;
            newPropertyList[type] = propertyInfos;

            s_TypeMappingInfo = newMappingList;
            s_TypeReferencedList = newReferencedObjects;
            s_TypePropertyInfo = newPropertyList;
        }
        #endregion


    }
}