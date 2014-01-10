using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Com.BaseLibrary.DataAccess;
using Com.BaseLibrary.Collection;
using Com.BaseLibrary.XmlAccess.Configuration;
using Com.BaseLibrary.Configuration;
using Com.BaseLibrary.Utility;
using Com.BaseLibrary.Caching;
using Com.BaseLibrary.Resources;
using Com.BaseLibrary.Common.Cryptography;

namespace Com.BaseLibrary.DataAccess.Configuration
{
    [XmlRoot("dataAccess")]
    public class DataAccessConfiguration
    {
        private const string DataAccessKey = "DataAccessConfiguration";

        [XmlAttribute("CommandFolder")]
        public string CommandFolder { get; set; }

        [XmlArray("databases")]
        [XmlArrayItem("database", typeof(DatabaseServer))]
        public KeyedList<string, DatabaseServer> DatabaseInstances { get; set; }

        [XmlArray("commandFiles")]
        [XmlArrayItem("file", typeof(DataCommandFile))]
        public List<DataCommandFile> CommandFileList { get; set; }

        internal static string DataAccessFile
        {
            get
            {
                return ConfigurationHelper.GetConfigurationFile(DataAccessKey);
            }
        }

        public static DataAccessConfiguration Current
        {
            get
            {
                //return GetFromDB();
                return GetFromConfigFile();
            }
        }
        private static DataAccessConfiguration setup;
        private static DataAccessConfiguration GetFromDB()
        {
            if (setup == null)
            {
                setup = new DataAccessConfiguration();
                setup.DatabaseInstances = new KeyedList<string, DatabaseServer>();
                foreach (var item in DBConnectionResource.Current.DataConnectionInfoList)
                {
                    DatabaseServer instance = new DatabaseServer();
                    instance.Name = item.ConnectionName;
                    instance.ConnectionString = item.ConnectionString;
                    instance.DataProvider = DataProvider.SqlServer;
                    setup.DatabaseInstances.Add(instance);
                }
            }
            return setup;
        }

        private static DataAccessConfiguration GetFromConfigFile()
        {
            setup = ConfigurationManager.GetFromCache<DataAccessConfiguration>(DataAccessKey);
            if (setup == null)
            {
                CacheObjectRemovedCallBack callBack = new CacheObjectRemovedCallBack(ObjectRemovedHandle);
                setup = ConfigurationManager.LoadConfiguration<DataAccessConfiguration>(DataAccessKey, DataAccessFile, callBack);
                setup.CheckConnection();
               
            }
            return setup;
        }

        private void CheckConnection()
        {
            var dbList = this.DatabaseInstances.ToList();
            foreach (var item in dbList)
            {
                if (item.ConnectionString.Contains(";"))
                {
                    continue;
                }
                else
                {
                    item.ConnectionString = Encryptor.Decrypt(item.ConnectionString);
                }
            }
        }

        private static void ObjectRemovedHandle(string key,
            object value,
            CacheObjectRemovedReason reason)
        {
            DataCommandFactory.ClearAllCommands();
            setup = null;
        }

    }
    public class DataCommandFile
    {
        [XmlAttribute("FileName")]
        public string FileName { get; set; }
    }
}
