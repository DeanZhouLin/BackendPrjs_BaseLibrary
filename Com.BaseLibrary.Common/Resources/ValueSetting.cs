using System;
using System.Collections.Generic;
using System.Text;
using Com.BaseLibrary.Collection;
using Com.BaseLibrary.Utility;
using Com.BaseLibrary.Configuration;
using System.Xml.Serialization;
using System.Data.SqlClient;
using System.Data;
using Com.BaseLibrary.Data;

namespace Com.BaseLibrary.Resources
{

	[XmlRoot("resources")]
	public class ValueResource
	{
		private const string MESSAGERESOURCEFILE_CONFIG = "MessageResourceFile";

		static ValueResource()
		{
			InitResourceData();
		}

		private static void InitResourceData()
		{
			//instance = new ValueResource();
			//instance.MessageList = new KeyedList<string, Message>();
			//DataTable dt = DBHelper.GetTable("SELECT * FROM VALUERESOURCE");
			//foreach (DataRow row in dt.Rows)
			//{
			//    Message message = new Message();
			//    message.Name = row["Name"].ToString();
			//    message.Value = row["Value"].ToString();
			//    instance.MessageList.Add(message);
			//}
		}

		public class Message : IKeyedItem<string>
		{
			[XmlAttribute("name")]
			public string Name { get; set; }
			[XmlAttribute("value")]
			public string Value { get; set; }

			#region IKeyedItem<string> Members

			public string Key
			{
				get { return Name; }
			}

			#endregion
		}

		[XmlElement("message")]
		public KeyedList<string, Message> MessageList { get; set; }

		private static ValueResource instance;
		private static ValueResource Current
		{
			get
			{
				//string setting = ConfigurationHelper.GetAppSetting(MESSAGERESOURCEFILE_CONFIG);
				//return ConfigurationManager.LoadConfiguration<MessageResource>(MESSAGERESOURCEFILE_CONFIG, setting);
				return instance;
			}
		}

		public static string GetString(string name)
		{
			Message value = Current.MessageList.GetItemByKey(name);
			return value == null ? name : value.Value;
		}
	}
}
