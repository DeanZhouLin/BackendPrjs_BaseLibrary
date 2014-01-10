/*****************************************************************
 * Copyright (C) DollMall Corporation. All rights reserved.
 * 
 * Author:   Dolphin Zhang (dolphin@dollmalll.com)
 * Create Date:  10/31/2006 15:24:29
 * Usage:
 *
 * RevisionHistory
 * Date         Author               Description
 * 
*****************************************************************/

using System;
using System.Xml.Serialization;

namespace Com.BaseLibrary.Collection.Sorting
{
	/// <summary>
	/// sort order for search result.
	/// </summary>
	public enum SortOrder
	{
		[XmlEnum("ASC")]
		Asc,
		[XmlEnum("DESC")]
		Desc,
	}
}
