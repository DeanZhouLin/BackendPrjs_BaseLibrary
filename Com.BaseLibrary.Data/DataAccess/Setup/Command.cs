using System;
using System.Data.Common;
using System.Xml.Serialization;
using System.Data;
using System.ComponentModel;
using Com.BaseLibrary.Collection;
using System.Collections.Generic;

namespace Com.BaseLibrary.DataAccess.Configuration
{
	public class Command
	{


		/// <remarks/>
		[XmlAttribute("Name")]
		public string Name { get; set; }

		/// <remarks/>
		[XmlAttribute("Database")]
		public string Database { get; set; }


		/// <remarks/>
		[DefaultValueAttribute(CommandType.Text)]
		[XmlAttribute("CommandType")]
		public CommandType CommandType { get; set; }

		/// <remarks/>
		[DefaultValueAttribute(300)]
		[XmlAttribute("TimeOut")]
		public int TimeOut { get; set; }


		[XmlText]
		public string CommandText { get; set; }

		/// <remarks/>
		[XmlElement("parameter")]
		public List<CommandParameter> Parameters { get; set; }


		public Command()
		{
			CommandType = CommandType.Text;
			TimeOut = 300;
		}

		[XmlIgnore]
		public Dictionary<string, CommandParameter> ParameterDictionary { get; set; }

		internal void BuildParameterDictionary()
		{
			ParameterDictionary = new Dictionary<string, CommandParameter>();

			foreach (var item in Parameters)
			{
				if (!item.Name.StartsWith("@"))
				{
					item.Name = "@" + item.Name.Trim();
				}
				else
				{
					item.Name = item.Name.Trim();
				}
				ParameterDictionary.Add(item.Name, item);
			}
		}
	}


}
