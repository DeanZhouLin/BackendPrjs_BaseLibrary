using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Com.BaseLibrary.DataAccess.Configuration
{
	[XmlRoot("commandConfiguration")]
	public class CommandConfiguration
	{	
		[XmlElement("command")]
		public List<Command> CommandList { get; set; }
	}
}
