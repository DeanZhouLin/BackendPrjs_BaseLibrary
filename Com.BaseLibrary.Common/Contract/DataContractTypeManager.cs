using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Reflection;
using System.Data.Linq.Mapping;

namespace Com.BaseLibrary.Contract
{

	public class DataContractTypeManager
	{
		private static Dictionary<string, List<PropertyInfo>> TypePropertyDictionary = new Dictionary<string, List<PropertyInfo>>();

		private static Dictionary<string, List<PropertyInfo>> TypeAllPropertyDictionary = new Dictionary<string, List<PropertyInfo>>();

		private readonly static object synObj = new object();
		public static List<PropertyInfo> GetTypeProperies(Type type)
		{
			string typeName = type.FullName;

			if (TypePropertyDictionary.ContainsKey(typeName))
			{
				return TypePropertyDictionary[typeName];
			}
			lock (synObj)
			{
				if (TypePropertyDictionary.ContainsKey(typeName))
				{
					return TypePropertyDictionary[typeName];
				}
				else
				{
					TypePropertyDictionary.Add(typeName,
						type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
						.Where(c => c.GetCustomAttributes(typeof(ColumnAttribute), false).Length > 0)
						.ToList());

					return TypePropertyDictionary[typeName];
				}
			}
		}

		private readonly static object synAllObj = new object();
		public static List<PropertyInfo> GetTypeAllProperies(Type type)
		{
			string typeName = type.FullName;

			if (TypeAllPropertyDictionary.ContainsKey(typeName))
			{
				return TypeAllPropertyDictionary[typeName];
			}
			lock (synAllObj)
			{
				if (TypeAllPropertyDictionary.ContainsKey(typeName))
				{
					return TypeAllPropertyDictionary[typeName];
				}
				else
				{
					TypeAllPropertyDictionary.Add(typeName,
						type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
						.ToList());

					return TypeAllPropertyDictionary[typeName];
				}

			}
		}
	}
}
