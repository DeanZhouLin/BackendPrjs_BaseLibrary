using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Com.BaseLibrary.Utility;
using System.ServiceModel;
using System.Configuration;
using System.ServiceModel.Configuration;
using Com.BaseLibrary.Logging;

namespace Com.BaseLibrary.Contract
{
    public class ServiceFactory
    {
        static Dictionary<string, Object> ServiceImplInstanceDictionary = new Dictionary<string, object>();
        static Dictionary<string, Type> ServiceImplementTypeDictionary = new Dictionary<string, Type>();

        static ServiceFactory()
        {
            //InitServiceInstance();
        }

        public static T CreateService<T>()
            where T : class
        {
            if (ServiceImplInstanceDictionary.Count == 0)
            {
                InitServiceInstance();
            }
            return ServiceImplInstanceDictionary[typeof(T).FullName] as T;
        }

        public static Type GetServiceImplType(string typeName)
        {
            if (ServiceImplInstanceDictionary.Count == 0)
            {
                InitServiceInstance();
            }
            return ServiceImplementTypeDictionary[typeName];
        }

        private static void InitServiceInstance()
        {
            try
            {
                Dictionary<string, Object> tempServiceWarehouse = new Dictionary<string, object>();
                Dictionary<string, Type> tempServiceImplementTypeDictionary = new Dictionary<string, Type>();
                string assembly = ConfigurationHelper.GetAppSetting("ServiceAssemblyName");
                if (string.IsNullOrEmpty(assembly))
                {
                    ServiceImplInstanceDictionary = new Dictionary<string, object>();
                    ServiceImplementTypeDictionary = new Dictionary<string, Type>();
                    return;
                }
                string[] asseblyNames = assembly.Split(',');
                foreach (var asseblyName in asseblyNames)
                {
                    Assembly serviceAssebly = Assembly.Load(asseblyName);
                    Type[] types = serviceAssebly.GetTypes();
                    for (int i = 0; i < types.Length; i++)
                    {
                        Type type = types[i];
                        if (type.IsInterface)
                        {
                            continue;
                        }

                        Type[] interfaces = type.GetInterfaces();
                        if (interfaces.FirstOrDefault(c => c.FullName == "Com.BaseLibrary.Contract.IServiceBase") != null)
                        {
                            if (tempServiceWarehouse.ContainsKey(interfaces[0].FullName))
                            {
                                continue;
                            }
                            object obj = serviceAssebly.CreateInstance(type.FullName);
                            tempServiceWarehouse.Add(interfaces[0].FullName, obj);
                            tempServiceImplementTypeDictionary.Add(type.FullName, type);
                        }
                    }
                }
                ServiceImplementTypeDictionary = tempServiceImplementTypeDictionary;
                ServiceImplInstanceDictionary = tempServiceWarehouse;
            }
            catch (Exception ex)
            {
                Logger.CurrentLogger.DoWrite("", "初始化服务实现实例", "Error", ex.Message, ex.ToString());
            }
        }


        public static T CreateServiceClient<T>()
        where T : class
        {
            string url = ConfigurationHelper.GetAppSetting(typeof(T).FullName);

            return CreateServiceClient<T>(url);
        }

        public static T CreateServiceClient<T>(string serviceUrl)
            where T : class
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.ReaderQuotas.MaxStringContentLength = 81920000;

            EndpointAddress address = new EndpointAddress(serviceUrl);
            ChannelFactory<T> factory = new ChannelFactory<T>(binding, address);
            T t = factory.CreateChannel();
            return t;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceUrl"></param>
        /// <returns></returns>
        public static T CreateServiceClient1<T>(string endpointConfigurationName)
            where T : class
        {
            ChannelFactory<T> factory = new ChannelFactory<T>(endpointConfigurationName);
            T t = factory.CreateChannel();
            return t;
        }

		public static T CreateWCFServiceClient<T>(string endpointConfigurationName) where T : class
		{
			ChannelFactory<T> factory = new ChannelFactory<T>(endpointConfigurationName);
			T t = factory.CreateChannel();
			return t;
		}
	}

    public class ServiceHostGroup
    {
        static List<ServiceHost> _hosts = new List<ServiceHost>();
        private static void OpenHost(Type t)
        {
            ServiceHost hst = new ServiceHost(t);
            hst.Open();
            _hosts.Add(hst);
        }
        public static void StartAllConfiguredServices()
        {
			System.Configuration.Configuration conf = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            ServiceModelSectionGroup svcmod = (ServiceModelSectionGroup)conf.GetSectionGroup("system.serviceModel");
            foreach (ServiceElement el in svcmod.Services.Services)
            {
                Type svcType = ServiceFactory.GetServiceImplType(el.Name);
                if (svcType == null)
                    throw new Exception("Invalid Service Type " + el.Name + " in configuration file.");
                OpenHost(svcType);
            }

        }
        public static void CloseAllServices()
        {
            foreach (ServiceHost hst in _hosts)
            {
                hst.Close();
            }
        }
    }
}
