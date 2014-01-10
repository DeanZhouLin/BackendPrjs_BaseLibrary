using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.BaseLibrary.Logging;
using System.Timers;
using System.ComponentModel;

namespace Com.BaseLibrary.Utility.WindowsService
{
    public abstract class ServiceWorker:MarshalByRefObject//:IDisposable
    {
        public string Arguments { get; set; }
        private bool IsWorking;
        public string ServiceName { get; set; }
        public string AssemblyName { get; set; }
        public string FilePath { get; set; }
        public string WorkTypeName { get; set; }
        public void Work(object sender, ElapsedEventArgs e)
        {
            if (IsWorking)
            {
                return;
            }
            IsWorking = true;
            try
            {
                if (WorkerConfiguration.Current.EnableMonitor)
                {
                    Logger.CurrentLogger.DoWrite(ConfigurationHelper.GetAppSetting("ServiceName"), ServiceName, "信息", "运行", "运行");
                }
                DoWork();
            }
            catch (Exception ex)
            {
                Logger.CurrentLogger.DoWrite("服务", ServiceName, "错误", ex.Message, ex.ToString());
            }
            finally
            {
                IsWorking = false;
            }
        }
        protected abstract void DoWork();
        
    }
}
