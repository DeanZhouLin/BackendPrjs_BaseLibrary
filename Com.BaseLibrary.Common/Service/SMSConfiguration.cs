using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Com.BaseLibrary.Configuration;
using Com.BaseLibrary.Utility;
using Com.BaseLibrary.Caching;
using Com.BaseLibrary.Collection;

namespace Com.BaseLibrary.Service
{
    [XmlRoot("smsConfiguration")]
	public class SMSConfiguration
	{
        public static string GetTemplateContent(string name)
        {
            TemplateSetup template = SMSConfiguration.Current.TemplateList[name].Clone() as TemplateSetup;

            return template.Content;
        }

        /// <summary>
        /// 短信所属应用
        /// </summary>
        [XmlAttribute("application")]
        public string Application { get; set; }

        /// <summary>
        /// 短信模版列表
        /// </summary>
        [XmlArray("templates")]
        [XmlArrayItem("template", typeof(TemplateSetup))]
        public KeyedList<string, TemplateSetup> TemplateList { get; set; }

        /// <summary>
        /// App.config/web.config配置文件中指定异常信息配置文件的AppSetting的键值
        /// 例：<add key="SMSConfigFile" value="config/SMSConfiguration.config" />
        /// </summary>
        private const string SECTION_NAME_TEMPLATE_CONFIG = "SMSConfigFile";

        /// <summary>
        /// 获取当前应用程序的短信模版
        /// </summary>
        public static SMSConfiguration Current
        {
            get
            {
                string setting = ConfigurationHelper.GetAppSetting(SECTION_NAME_TEMPLATE_CONFIG);
                if (string.IsNullOrEmpty(setting))
                {
                    return null;
                }
                return ConfigurationManager.LoadConfiguration<SMSConfiguration>(SECTION_NAME_TEMPLATE_CONFIG, setting);
            }
        }
	}

    /// <summary>
    /// 短信定义
    /// </summary>
    public class TemplateSetup : IKeyedItem<string>, ICloneable
    {
        /// <summary>
        /// 异常信息的键
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

       
        /// <summary>
        /// 短信所属模块
        /// </summary>
        [XmlAttribute("module")]
        public string Module { get; set; }

        /// <summary>
        /// 短信发送方式（I：立即；B：缓存）
        /// </summary>
        [XmlAttribute("pattern")]
        public string Pattern { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        [XmlAttribute("priority")]
        public string Priority { get; set; }

        /// <summary>
        /// 发短信通道
        /// </summary>
        [XmlAttribute("channel")]
        public int  Channel { get; set; }

        /// <summary>
        /// 短信模版信息
        /// </summary>
        [XmlText]
        public string Content { get; set; }

        /// <summary>
        /// 辅助字段
        /// 异常信息的键
        /// </summary>
        [XmlIgnore]
        public string Key
        {
            get { return this.Name; }
        }

        #region ICloneable Members

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }
}
