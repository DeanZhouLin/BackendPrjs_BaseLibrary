/*****************************************************************
 * Copyright (C) DollMall Corporation. All rights reserved.
 * 
 * Author:   Dolphin Zhang (dolphin@dollmalll.com)
 * Create Date:  10/06/2006 17:18:36
 * Usage:
 *
 * RevisionHistory
 * Date         Author               Description
 * 
*****************************************************************/

using System;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Web;


using Com.BaseLibrary.Entity;
using Com.BaseLibrary.Utility;
using Com.BaseLibrary.XmlAccess.Configuration;

using Com.BaseLibrary.Caching;
using System.Web.Caching;

namespace Com.BaseLibrary.XmlAccess
{
    public static class XmlDataManager
    {

        private static string m_DefaultXmlDataFolder;
        static XmlDataManager()
        {
            Init();
        }

        private static void Init()
        {
            m_DefaultXmlDataFolder = (HttpContext.Current != null) ?
            HttpContext.Current.Server.MapPath(XmlAccessConfiguration.Current.DefaultXmlDataFolder) :
            Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, XmlAccessConfiguration.Current.DefaultXmlDataFolder.Replace("/", "\\").TrimStart('~').TrimStart('\\'));


        }





        /// <summary>
        /// Gets the data table contained in the specified xml file.
        /// </summary>
        /// <param name="xmlFileName">Filename with extension but no path.</param>
        /// <returns></returns>
        private static DataTable GetDataTable(string xmlFileName)
        {
            return XmlLoader.LoadDataTable(xmlFileName);
        }


        private static readonly object m_SynObject = new object();

        public static List<T> GetEntityList<T>(string xmlFileName) where T : class, new()
        {
            return GetEntityList<T>(xmlFileName, null);
        }

        public static List<T> GetEntityList<T>(string xmlFileName, CacheObjectRemovedCallBack callBack)
            where T : class, new()
        {
            ICacheManager cacheManager = CacheManagerFactory.CreateAspNetCacheManager();
            List<T> listT = null;
            if (cacheManager.Contains(xmlFileName))
            {
                listT = cacheManager[xmlFileName] as List<T>;
            }

            if (listT == null)
            {
                lock (m_SynObject)
                {
                    listT = cacheManager[xmlFileName] as List<T>;
                    if (listT == null)
                    {
                        DataTable dt = GetDataTable(xmlFileName);
                        if (dt == null)
                        {
                            listT = new List<T>(0);
                        }
                        listT = EntityBuilder.BuildEntityList<T>(dt);
                        string fullPath = Path.Combine(m_DefaultXmlDataFolder, xmlFileName);
                        cacheManager.Add(xmlFileName, listT, CacheObjectPriority.Normal, callBack, fullPath);
                    }
                }
            }
            return listT;
        }

    }
}