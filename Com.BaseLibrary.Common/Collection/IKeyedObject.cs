using System;
using System.Collections.Generic;

namespace Com.BaseLibrary.Collection
{
	/// <summary>
	/// Represents an object that owns a key and can be uniquely identified by that key in a collection.
	/// </summary>
	public interface IKeyedItem<T>
	{
		/// <summary>
		/// Gets the key that can uniquely identify the object.
		/// </summary>
		T Key { get; }
	}
}