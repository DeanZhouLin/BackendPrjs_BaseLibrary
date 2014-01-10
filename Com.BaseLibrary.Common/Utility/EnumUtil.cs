using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Com.BaseLibrary.Entity;


namespace Com.BaseLibrary.Utility
{
	/// <summary>
	//EnumUtil
	/// </summary>
	public static class EnumUtil
	{

		private static Dictionary<Type, List<EnumItemInfo>> DictEnumItems = new Dictionary<Type, List<EnumItemInfo>>();

		private static readonly object obj = new object();

		/// <summary>
		/// get the enum's all list
		/// </summary>
		/// <param name="enumType">the type of the enum</param>
		/// <param name="withAll">identicate whether the returned list should contain the all item</param>
		/// <returns></returns>
		public static List<EnumItemInfo> GetEnumItems(Type enumType)
		{
			if (DictEnumItems.ContainsKey(enumType))
			{
				return ListUtil.Clone<EnumItemInfo>(DictEnumItems[enumType]);
			}

			List<EnumItemInfo> list = new List<EnumItemInfo>();



			if (enumType.IsEnum != true)
			{
				// just whethe the type is enum type
				throw new InvalidOperationException();
			}



			// 获得特性Description的类型信息
			Type typeDescription = typeof(EnumFieldAttribute);

			// 获得枚举的字段信息（因为枚举的值实际上是一个static的字段的值）
			FieldInfo[] fields = enumType.GetFields();

			// 检索所有字段
			foreach (FieldInfo field in fields)
			{
				// 过滤掉一个不是枚举值的，记录的是枚举的源类型
				if (field.FieldType.IsEnum == false)
					continue;

				// 通过字段的名字得到枚举的值
				string value = string.Empty;
				string text = string.Empty;

				// 获得这个字段的所有自定义特性，这里只查找Description特性
				object[] arr = field.GetCustomAttributes(typeDescription, true);
				object itemObj = enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null);

				if (itemObj == null)
				{
					continue;
				}

				if (arr.Length > 0)
				{
					// 因为Description自定义特性不允许重复，所以只取第一个
					EnumFieldAttribute aa = (EnumFieldAttribute)arr[0];

					// 获得特性的描述值
					text = aa.Text;
					value = aa.Value;

				}
				else
				{
					// 如果没有特性描述，那么就显示英文的字段名
					text = field.Name;
					value = itemObj.ToString();
				}


				list.Add(new EnumItemInfo(itemObj, value, text));
			}

			if (!DictEnumItems.ContainsKey(enumType))
			{
				lock (obj)
				{
					DictEnumItems.Add(enumType, list);
				}
			}
			return ListUtil.Clone<EnumItemInfo>(list);
		}

		/// <summary>
		/// the the enum value's descrption attribute information
		/// </summary>
		/// <param name="enumType">the type of the enum</param>
		/// <param name="value">the enum value</param>
		/// <returns></returns>
		public static string GetEnumDescription<T>(T t)
		{
			Type enumType = typeof(T);
			List<EnumItemInfo> list = GetEnumItems(enumType);
			foreach (EnumItemInfo item in list)
			{
				if (item.EnumItem == null)
				{
					continue;
				}
				if (item.EnumItem.ToString() == t.ToString())
					return item.Description;
			}
			return string.Empty;
		}

		public static string GetEnumDescription(object t)
		{
			Type enumType = t.GetType();
			List<EnumItemInfo> list = GetEnumItems(enumType);
			foreach (EnumItemInfo item in list)
			{
				if (item.EnumItem.ToString() == t.ToString())
					return item.Description;
			}
			return string.Empty;
		}


		/// <summary>
		/// get the Enum value according to the its decription
		/// </summary>
		/// <param name="enumType">the type of the enum</param>
		/// <param name="value">the description of the EnumValue</param>
		/// <returns></returns>
		public static T GetEnumByDesc<T>(string description)
		{
			Type enumType = typeof(T);
			return (T)GetEnumByDesc(enumType, description);
		}
		public static object GetEnumByDesc(Type enumType, string description)
		{
			List<EnumItemInfo> list = GetEnumItems(enumType);
			foreach (EnumItemInfo item in list)
			{
				if (item.Description.ToLower() == description.Trim().ToLower())
					return item.EnumItem;
			}
			throw new Exception(string.Format("The enum {0} doesn't have item which has a attribute value of {1}", enumType.Name, description));
		}



		public static T GetEnumByValue<T>(string value)
		{
			if (value == null)
				return default(T);
			try
			{
				Type enumType = typeof(T);
				List<EnumItemInfo> list = GetEnumItems(enumType);
				foreach (EnumItemInfo item in list)
				{
					if (item.Value.ToLower() == value.Trim().ToLower())
						return (T)item.EnumItem;
				}
				return default(T);
			}
			catch
			{
				return default(T);
			}
		}

		public static object GetEnumByValue(Type enumType, string value)
		{
			List<EnumItemInfo> list = GetEnumItems(enumType);
			foreach (EnumItemInfo item in list)
			{
				if (item.Value == null)
				{
					continue;
				}
				if (item.Value.ToLower() == value.ToString().Trim().ToLower())
					return item.EnumItem;
			}
			foreach (EnumItemInfo item in list)
			{
				if ((int)item.EnumItem == int.Parse(value))
					return item.EnumItem;
			}
			return null;
		}

		public static string GetDescriptionByValue<T>(string value)
		{
			if (StringUtil.IsNullOrEmpty(value))
			{
				return string.Empty;
			}
			List<EnumItemInfo> list = GetEnumItems(typeof(T));
			foreach (EnumItemInfo item in list)
			{
				if (item.Value == null)
				{
					continue;
				}
				if (item.Value.ToLower() == value.ToString().Trim().ToLower())
					return item.Description;
			}
			return null;
		}

		public static string GetValueByEnum(Type enumType, object val)
		{
			if (val == null)
			{
				return string.Empty;
			}

			List<EnumItemInfo> list = GetEnumItems(enumType);
			foreach (EnumItemInfo item in list)
			{
				if (item.EnumItem != null && item.EnumItem.ToString() == val.ToString())
					return item.Value;
			}
			return null;
		}

		public static string GetValueByDescription<T>(string val)
		{
			if (StringUtil.IsNullOrEmpty(val))
			{
				return string.Empty;
			}

			Type enumType = typeof(T);

			List<EnumItemInfo> list = GetEnumItems(enumType);
			foreach (EnumItemInfo item in list)
			{
				if (item.Description == val)
					return item.Value;
			}
			return null;
		}

		public static string GetValueByEnum(object enumValue)
		{
			if (enumValue == null)
			{
				return null;
			}
			return GetValueByEnum(enumValue.GetType(), enumValue);
		}
	}

	public class EnumItemInfo
	{
		public EnumItemInfo()
		{ }
		public EnumItemInfo(object enumItem, string value, string description)
		{
			EnumItem = enumItem;
			Value = value;
			Description = description;
		}
		public object EnumItem { get; set; }
		public string Value { get; set; }
		public string Description { get; set; }
	}
}