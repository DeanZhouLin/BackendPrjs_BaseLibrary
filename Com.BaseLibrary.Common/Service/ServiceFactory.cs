using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.BaseLibrary.Utility;
using System.Reflection;
using System.IO;

namespace Com.BaseLibrary.Common.Service
{

    public static class ServiceFactory
    {
        private static Dictionary<string, object> ServiceInstanceList
                       = new Dictionary<string, object>();
        private static readonly object synObject = new object();
        public static T CreateServiceProxy<T>()
        where T : class
        {
            Type interfaceType = typeof(T);
            string interfaceTypeName = interfaceType.FullName;

            if (ServiceInstanceList.ContainsKey(interfaceTypeName))
            {
                return ServiceInstanceList[interfaceTypeName] as T;
            }
            else
            {
                lock (synObject)
                {
                    if (ServiceInstanceList.ContainsKey(interfaceTypeName))
                    {
                        return ServiceInstanceList[interfaceTypeName] as T;
                    }
                    else
                    {
                        string serviceInvokeMode = ConfigurationHelper.GetAppSetting("ServiceInvokeMode");
                        if (StringUtil.IsNullOrEmpty(serviceInvokeMode) || serviceInvokeMode.ToUpper() == "DLL")
                        {
                            //LoadServiceInstance(ConfigurationHelper.GetAppSetting("ServiceImplAssembles"));
                            LoadServiceInstance();
                        }
                        else
                        {
                            //WCF
                        }
                        if (ServiceInstanceList.ContainsKey(interfaceTypeName))
                        {
                            return ServiceInstanceList[interfaceTypeName] as T;
                        }
                        throw new Exception(string.Format("类型{0}所在的组件未引用或配置", interfaceTypeName));
                    }
                }
            }
        }

        private static void LoadServiceInstance(string assembleNames)
        {
            try
            {
                List<Assembly> assemblyList = AppDomain.CurrentDomain.GetAssemblies().ToList();

                string[] assembleNameArray = assembleNames.Split(',');

                foreach (var assembleName in assembleNameArray)
                {
                    if (StringUtil.IsNullOrEmpty(assembleName))
                    {
                        continue;
                    }
                    Assembly assembly = Assembly.Load(assembleName);
                    Type[] typeList = assembly.GetTypes();
                    foreach (Type type in typeList)
                    {
                        Type[] interTypeList = type.GetInterfaces();
                        if (interTypeList != null && interTypeList.Length > 0)
                        {
                            Type interType = interTypeList.FirstOrDefault(
                                c => c.GetInterfaces().FirstOrDefault(d => d.Name.StartsWith("IServiceContract")) != null);

                            if (interType != null)
                            {
                                if (ServiceInstanceList.ContainsKey(interType.FullName))
                                {
                                    continue;
                                }
                                ServiceInstanceList.Add(interType.FullName, assembly.CreateInstance(type.FullName));
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                throw;
            }

        }

        private static void LoadServiceInstance()
        {
            try
            {
                String[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "\\bin");

                foreach (String file in files)
                {
                    if (file.EndsWith(".Service.dll"))
                    {
                        Assembly assembly = Assembly.LoadFile(file);
                        Type[] typeList = assembly.GetTypes();
                        foreach (Type type in typeList)
                        {
                            Type[] interTypeList = type.GetInterfaces();
                            if (interTypeList != null && interTypeList.Length > 0)
                            {
                                Type interType = interTypeList.FirstOrDefault(
                                    c => c.GetInterfaces().FirstOrDefault(d => d.Name.StartsWith("IServiceContract")) != null);

                                if (interType != null)
                                {
                                    if (ServiceInstanceList.ContainsKey(interType.FullName))
                                    {
                                        continue;
                                    }
                                    ServiceInstanceList.Add(interType.FullName, assembly.CreateInstance(type.FullName));
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}
