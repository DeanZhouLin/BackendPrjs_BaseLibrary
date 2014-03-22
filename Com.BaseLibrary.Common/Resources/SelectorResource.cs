using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.BaseLibrary.Collection;
using System.Xml.Serialization;
using Com.BaseLibrary.Utility;
using Com.BaseLibrary.Configuration;
using System.Data.SqlClient;
using System.Data;
using Com.BaseLibrary.Data;

namespace Com.BaseLibrary.Resources
{

	[XmlRoot("selectorSetting")]
	public class SelectorResource
	{
		private const string SELECTORRESOURCEFILE_CONFIG = "SelectorResourceFile";
		static SelectorResource()
		{
			//instance = new SelectorResource();
			//instance.SelectorList = new KeyedList<string, Selector>();
			//DataTable dt = DBHelper.GetTable( "SELECT * FROM SelectorResource");
			//foreach (DataRow row in dt.Rows)
			//{
			//    string name = row["Name"].ToString();
			//    Selector selector = instance.SelectorList.FirstOrDefault(c => c.Name == name);
			//    if (selector == null)
			//    {
			//        selector = new Selector();
			//        selector.Name = name;
			//        selector.ItemList = new KeyedList<string, Selector.SelectorItem>();
			//        instance.SelectorList.Add(selector);
			//    }
			//    Selector.SelectorItem selectorItem = new Selector.SelectorItem();
			//    selectorItem.Text = row["Text"].ToString();
			//    selectorItem.Value = row["Value"].ToString();
			//    selector.ItemList.Add(selectorItem);
			// }
		}

		public class Selector : IKeyedItem<string>
		{
			[XmlAttribute("name")]
			public string Name { get; set; }

			[XmlElement("item")]
			public KeyedList<string, SelectorItem> ItemList { get; set; }

			public string Key
			{
				get { return this.Name; }
			}

			public class SelectorItem : IKeyedItem<string>
			{
				[XmlAttribute("text")]
				public string Text { get; set; }
				[XmlAttribute("value")]
				public string Value { get; set; }

				#region IKeyedItem<string> Members

				public string Key
				{
					get { return this.Value; }
				}

				#endregion
			}
		}

		[XmlArray("selectors")]
		[XmlArrayItem("selector", typeof(Selector))]
		public KeyedList<string, Selector> SelectorList { get; set; }

		private static SelectorResource instance;
		private static SelectorResource Current
		{
			get
			{
				//string setting = ConfigurationHelper.GetAppSetting(SELECTORRESOURCEFILE_CONFIG);
				//return ConfigurationManager.LoadConfiguration<SelectorResource>(SELECTORRESOURCEFILE_CONFIG, setting);
				return instance;
			}
		}



		public static KeyedList<string, Selector.SelectorItem> GetItemList(string name)
		{
			Selector value = Current.SelectorList.GetItemByKey(name);
			return value == null ? null : value.ItemList;
		}
		public static Selector.SelectorItem GetItem(string name, string itemValue)
		{
			Selector value = Current.SelectorList.GetItemByKey(name);
			return value == null ? null : value.ItemList.GetItemByKey(itemValue);
		}

		public static string GetText(string name, string itemValue)
		{
			Selector.SelectorItem item = GetItem(name, itemValue);
			return item == null ? string.Empty : item.Text;
		}
	}
}
