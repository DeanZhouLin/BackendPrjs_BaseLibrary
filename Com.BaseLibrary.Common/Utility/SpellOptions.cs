/*
* SunriseSpell - A Chinese pinyin library
*
* Copyright (C) 2004 mic <mic4free@hotmail.com>
*
* This program is free software; you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation; either version 2 of the License, or
* (at your option) any later version.
*
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with this program; if not, write to the Free Software
* Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*
* In case your copy of SunriseSpell does not include a copy of the license, you may find it online at 
* http://www.gnu.org/copyleft/gpl.html
*/

using System;

namespace Com.BaseLibrary.Utility
{
	/// <summary>
	/// 提供用于设置转换选项的枚举值。
	/// </summary>
	/// <value>
	/// <code>FirstLetterOnly</code>
	/// 只转换拼音首字母，默认转换全部
	/// </value>
	/// <value>
	/// <code>TranslateUnknowWordToInterrogation</code>
	/// 转换未知汉字为问号，默认不转换
	/// </value>
	/// <value>
	/// <code>EnableUnicodeLetter</code>
	/// 保留非字母、非数字字符，默认不保留
	/// </value>
	[System.FlagsAttribute]
	public enum SpellOptions
	{
		FirstLetterOnly = 1,													//只转换拼音首字母，默认转换全部
		TranslateUnknowWordToInterrogation = 1 << 1,		//转换未知汉字为问号，默认不转换
		EnableUnicodeLetter =  1 << 2,								//保留非字母、非数字字符，默认不保留
	}

}
