using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.BaseLibrary.Logging;
using System.Timers;
using System.ComponentModel;

namespace Com.BaseLibrary.Utility.WindowsService
{
    public abstract class ServiceWorker//:IDisposable
    {
        public string Arguments { get; set; }
        private bool IsWorking;
        public string ServiceName { get; set; }
        //public string AssemblyName { get; set; }
        //public string FilePath { get; set; }
        //public string WorkTypeName { get; set; }
        public void Work(object sender, ElapsedEventArgs e)
        {
            if (IsWorking)
            {
                return;
            }
            IsWorking = true;
            try
            {
                string isLoging=ConfigurationHelper.GetAppSetting("IsLoging");
                if (!string.IsNullOrEmpty(isLoging) && isLoging == "1")
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
        /// <summary>
        /// 此方法不能写在父类里否则反编译找不到该方法
        /// </summary>
        /// <param name="Arguments"></param>
        /// <param name="ServiceName"></param>
        public abstract void CommonWork(string Arguments,string ServiceName);
        
    }

    //public interface IServiceWorker
    //{
    //    string ServiceName{get;set;}

    //    bool IsWorking { get; set; }
    //    /// <summary>
    //    /// 子程序域中的exception无法被主程序域catch，所以需要子类自己去记录错误信息
    //    /// 找不到定义在父类里的Work所以要自己实现一个Work
    //    /// </summary>
    //    void Work(string ServiceName);

    //    void DoWork();
    //}

    //public class LogWorker
    //{

    //    public void Work(IServiceWorker seviceWoker)
    //    {
    //        if (seviceWoker.IsWorking)
    //        {
    //            return;
    //        }
    //        seviceWoker.IsWorking = true;
    //        try
    //        {
    //            if (WorkerConfiguration.Current.EnableMonitor)
    //            {
    //                Logger.CurrentLogger.DoWrite(ConfigurationHelper.GetAppSetting("ServiceName"), seviceWoker.ServiceName, "信息", "运行", "运行");
    //            }
    //            seviceWoker.DoWork();
    //        }
    //        catch (Exception ex)
    //        {
    //            Logger.CurrentLogger.DoWrite("服务", seviceWoker.ServiceName, "错误", ex.Message, ex.ToString());
    //        }
    //        finally
    //        {
    //            seviceWoker.IsWorking = false;
    //        }
    //    }
    //}


}
