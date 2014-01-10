using System;
using System.Xml.Serialization;

namespace Com.BaseLibrary.Collection.Sorting
{
	[Serializable]
	[XmlRoot("sortInfo")]
	public class SortInfo
	{

		public SortInfo(string sortBy, SortOrder order)
		{
			SortBy = sortBy;
			SortOrder = order;
		}

		public SortInfo()
		{
		}

		[XmlAttribute("sortBy")]
		public string SortBy { get; set; }

		[XmlAttribute("sortOrder")]
		public SortOrder SortOrder { get; set; }
	}
}
