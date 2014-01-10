using System;
using System.Collections.Generic;

using System.Xml.Serialization;

using Com.BaseLibrary.Collection;
using Com.BaseLibrary.Configuration;
using Com.BaseLibrary.Utility;

namespace Com.BaseLibrary.Logging
{
	[XmlRoot("configuration")]
	public class LogConfiguration
	{
		[XmlArray("logSettings")]
		[XmlArrayItem("category", typeof(LogCategory))]
		public KeyedList<string, LogCategory> CategoryList { get; set; }

		private static readonly string ConfigFileKey = "LogConfigFile";

		internal static LogConfiguration Current
		{
			get
			{
				string configFile = ConfigurationHelper.GetConfigurationFile(ConfigFileKey);
				return ConfigurationManager.LoadConfiguration<LogConfiguration>(configFile);
			}
		}
	}

	public class LogCategory : IKeyedItem<string>
	{
		[XmlAttribute("name")]
		public string Name { get; set; }

		[XmlElement("log", typeof(LogSetup))]
		public KeyedList<string, LogSetup> LogList { get; set; }


		public string Key
		{
			get { return this.Name; }
		}

	}

	public class LogSetup : IKeyedItem<string>
	{
		[XmlAttribute("name")]
		public string Name { get; set; }

		[XmlAttribute("title")]
		public string Title { get; set; }

		[XmlAttribute("category")]
		public string Category { get; set; }

		[XmlAttribute("eventId")]
		public int EventID { get; set; }		

		[XmlText]
		public string MessageTemplate { get; set; }

		public string Key
		{
			get { return this.Name; }
		}


	}
}
