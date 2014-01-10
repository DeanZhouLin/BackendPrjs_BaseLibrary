using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.BaseLibrary.Logging;
using System.Xml.Serialization;
using Com.BaseLibrary.Configuration;
using Com.BaseLibrary.Collection;
using System.Reflection;
using System.IO;
using System.ComponentModel;

namespace Com.BaseLibrary.Utility.WindowsService
{
    /// <summary>
    /// 异常信息配置
    /// </summary>
    [XmlRoot("workerConfiguration")]
    public class WorkerConfiguration
    {
        private WorkerConfiguration()
        { }

        public static WorkerConfiguration LoadWorkerConfiguration(string filePath)
        {
            return ConfigurationManager.LoadConfiguration<WorkerConfiguration>(filePath);
        }
        public static void SaveToConfiguration(string filePath, WorkerConfiguration workerConfiguration)
        {
            ConfigurationManager.SaveAs<WorkerConfiguration>(filePath, workerConfiguration);
        }
        /// <summary>
        /// App.config/web.config配置文件中指定异常信息配置文件的AppSetting的键值
        /// 例：<add key="ExceptionConfigFile" value="bin/Configuration/ExceptionMessage.config" />
        /// </summary>
        private const string SECTION_NAME_Error_CONFIG = "WorkerConfigFile";

        /// <summary>
        /// 获取当前应用程序的异常配置信息
        /// </summary>
        public static WorkerConfiguration Current
        {
            get
            {
                string setting = ConfigurationHelper.GetAppSetting(SECTION_NAME_Error_CONFIG);
                if (string.IsNullOrEmpty(setting))
                {
                    return null;
                }
                return ConfigurationManager.LoadConfiguration<WorkerConfiguration>(SECTION_NAME_Error_CONFIG, setting);
            }
        }

        public static void ClearCurrentCache()
        {
            ConfigurationManager.RemoveConfigurationCache(SECTION_NAME_Error_CONFIG);
        }

        [XmlAttribute("EnableMonitor")]
        public bool EnableMonitor { get; set; }
        /// <summary>
        /// 异常信息列表
        /// </summary>
        [XmlArray("workers")]
        [XmlArrayItem("worker", typeof(WorkerSetup))]
        public KeyedList<string, WorkerSetup> WorkerSetupList { get; set; }

    }

    /// <summary>
    /// 异常信息定义
    /// </summary>

    public partial class WorkerSetup : IKeyedItem<string>, INotifyPropertyChanged
    {
        /// <summary>
        /// 异常信息的键
        /// </summary>
        private string name;
        [XmlAttribute("Name")]
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Name"));
            }
        }
        private string description;
        [XmlAttribute("Description")]
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Description"));
            }
        }
        private string filePath;
        [XmlAttribute("FilePath")]
        public string FilePath
        {
            get { return filePath; }
            set
            {
                filePath = value;
                OnPropertyChanged(new PropertyChangedEventArgs("FilePath"));
            }
        }
        private string assemblyName;
        [XmlAttribute("Assembly")]
        public string AssemblyName
        {
            get { return assemblyName; }
            set
            {
                assemblyName = value;
                OnPropertyChanged(new PropertyChangedEventArgs("AssemblyName"));
            }
        }
        private string workTypeName;
        [XmlAttribute("WorkTypeName")]
        public string WorkTypeName
        {
            get { return workTypeName; }
            set
            {
                workTypeName = value;
                OnPropertyChanged(new PropertyChangedEventArgs("WorkTypeName"));
            }
        }
        [XmlIgnore]
        public List<string> WorkTypeList { get; set; }
        [XmlAttribute("Interval")]
        public string TimeSet { get; set; }
        [XmlIgnore]
        public double Interval
        {
            get
            {
                if (string.IsNullOrEmpty(TimeSet))
                {
                    return 1000.0;
                }
                double result;
                if (double.TryParse(TimeSet, out result))
                {
                    if (result <= 0)
                    {
                        return 1000.0;
                    }
                    return result;
                }
                else
                {
                    return 1000.0;
                }
            }
            set
            {
                TimeSet = value.ToString();
                OnPropertyChanged(new PropertyChangedEventArgs("Interval"));
            }
        }
        private string arguments;
        [XmlAttribute("Arguments")]
        public string Arguments
        {
            get
            {
                return arguments;
            }
            set
            {
                arguments = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Arguments"));
            }
        }
        private string workStatus;
        [XmlAttribute("WorkStatus")]
        public string WorkStatus
        {
            get
            {
                return workStatus;
            }
            set
            {
                workStatus = value;
                OnPropertyChanged(new PropertyChangedEventArgs("WorkStatus"));
            }
        }
        private string runOnlyOnce;
        [XmlAttribute("RunOnlyOnce")]
        public string RunOnlyOnce
        {
            get
            {
                return runOnlyOnce;
            }
            set
            {
                runOnlyOnce = value;
                OnPropertyChanged(new PropertyChangedEventArgs("RunOnlyOnce"));
            }
        }
        /// <summary>
        /// 空窜会转为false
        /// </summary>
        [XmlAttribute("IsDeleted")]
        public bool IsDeleted { get; set; }
        
        public string Key
        {
            get { return this.Name; }
        }
        private string workStatusDescription;
        [XmlIgnore]
        public string WorkStatusDescription
        {
            get
            {
                string status,onlyOnce;
                if (WorkStatus!=null && WorkStatus.ToUpper() == "OFF")
                {
                    status = "停止";
                }
                else
                {
                    status = "运行";
                }
                if (RunOnlyOnce!=null && RunOnlyOnce == "1")
                {
                    onlyOnce = "单次";
                }
                else
                {
                    onlyOnce = "多次";
                }
                workStatusDescription = string.Format("{0}{1}", onlyOnce, status);
                return workStatusDescription;
            }
            set
            {
                //if (value.Contains("停止"))
                //{
                //    WorkStatus = "off";
                //}
                //else
                //{
                //    WorkStatus = "on";
                //}
                //if (value.Contains("单次"))
                //{
                //    RunOnlyOnce = "1";
                //}
                //else
                //{
                //    RunOnlyOnce = "0";
                //}
                workStatusDescription = value;
                OnPropertyChanged(new PropertyChangedEventArgs("WorkStatusDescription"));
            }
        }
        public ServiceWorker CreateServiceWorker()
        {
            return Assembly.Load(AssemblyName).CreateInstance(WorkTypeName) as ServiceWorker;
        }

        public ServiceWorker CreateServiceWorkerByFile()
        {
            string strServiceLibraryPath = PathUtil.GetFullFilePath(ConfigurationHelper.GetAppSetting("DLLPath"));
            if (System.IO.Directory.Exists(strServiceLibraryPath))
            {
                strServiceLibraryPath = Path.Combine(strServiceLibraryPath, AssemblyName);
                //using (FileStream stream = new FileStream(strServiceLibraryPath, FileMode.Open))
                //{
                //    using (MemoryStream memStream = new MemoryStream())
                //    {
                //        int res;
                //        byte[] b = new byte[4096];
                //        while ((res = stream.Read(b, 0, b.Length)) > 0)
                //        {
                //            memStream.Write(b, 0, b.Length);
                //        }
                //        Assembly asm = Assembly.Load(memStream.ToArray());
                //        ServiceWorker serviceWorker = asm.CreateInstance(WorkTypeName) as ServiceWorker;
                //        return serviceWorker;
                //    }
                //}
                return Assembly.LoadFile(strServiceLibraryPath).CreateInstance(WorkTypeName) as ServiceWorker;
                
            }

            return Assembly.LoadFile(strServiceLibraryPath).CreateInstance(WorkTypeName) as ServiceWorker;
        }



        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }
    }


}
