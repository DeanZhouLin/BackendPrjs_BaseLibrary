using System;
using System.Collections.Generic;

namespace Com.BaseLibrary.Collection
{
	/// <summary>
	/// Represents a collection of IKeyedObject that can be accessed by index. 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IKeyedList<TKey, TItem> : ICollection<TItem> where TItem : IKeyedItem<TKey>
	{
		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		TItem this[int index] { get; }

		/// <summary>
		/// Gets or sets the element with the specified key value.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		TItem this[TKey key] { get; }

		/// <summary>
		/// Gets the item by key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		TItem GetItemByKey(TKey key);

		/// <summary>
		/// Indicates if the collection contains an object with the specified key value.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		bool Contains(TKey key);
	}
}
