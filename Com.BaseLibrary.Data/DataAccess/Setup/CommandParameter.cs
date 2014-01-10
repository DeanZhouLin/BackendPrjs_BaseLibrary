using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Xml.Serialization;
using System.Data.OracleClient;
using Com.BaseLibrary.Collection;


namespace Com.BaseLibrary.DataAccess.Configuration
{
	public class CommandParameter : IKeyedItem<string>
	{
		/// <remarks/>
		[XmlAttribute("Name")]
		public string Name { get; set; }

		/// <remarks/>
		[XmlAttribute("DbType")]
		public DbType DbType { get; set; }

		/// <remarks/>
		[XmlAttribute("Direction")]
		public ParameterDirection Direction { get; set; }

		/// <remarks/>
		[XmlAttribute("Size")]
		public int Size { get; set; }


		public CommandParameter()
		{
		}

		public string Key
		{
			get { return this.Name; }
		}
	}

}
