using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Com.BaseLibrary.Utility;

namespace Com.BaseLibrary.Collection
{
	public class KeyedList<TKey, TItem> : IKeyedList<TKey, TItem> where TItem : IKeyedItem<TKey>
	{
		private Dictionary<TKey, TItem> entriesTable;
		private List<TItem> entries;
		private IComparer comparer;

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="T"/> class.</para>
		/// </summary>
		public KeyedList()
			: this(null)
		{
		}

		public KeyedList(IEqualityComparer<TKey> equalityComaprer)
		{
			if (equalityComaprer == null)
			{
				if (typeof(TKey) == typeof(string))
				{
					entriesTable = new Dictionary<TKey, TItem>(new StringEqualityComparer() as IEqualityComparer<TKey>);
				}
				else
				{
					entriesTable = new Dictionary<TKey, TItem>();
				}
			}
			else
			{
				entriesTable = new Dictionary<TKey, TItem>(equalityComaprer);
			}
			entries = new List<TItem>();
			comparer = new CaseInsensitiveComparer(CultureInfo.InvariantCulture);
		}

		public TItem this[int index]
		{
			get { return (TItem)entries[index]; }
		}

		/// <summary>
		/// Returns null if specified key does not exist in the collection.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public TItem this[TKey key]
		{
			get
			{
				if (entriesTable.ContainsKey(key))
				{
					return entriesTable[key];
				}
				return default(TItem);
				//throw new Exception(string.Format("The key '{0}' doesn't exist in KeyedList", key));

			}
		}

		public TItem GetItemByKey(TKey key)
		{
			TItem val;
			entriesTable.TryGetValue(key, out val);

			return val;
		}

		public bool Contains(TKey key)
		{
			return entriesTable.ContainsKey(key);
		}

		#region ICollection<T> Members

		public void Add(TItem item)
		{
			entriesTable.Add(item.Key, item);
			entries.Add(item);
		}

		public void Clear()
		{
			entriesTable.Clear();
			entries.Clear();
		}

		public bool Contains(TItem item)
		{
			return entriesTable.ContainsKey(item.Key);
		}

		public void CopyTo(TItem[] array, int arrayIndex)
		{
			entriesTable.Values.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return entriesTable.Count; }
		}

		public bool IsReadOnly
		{
			get { return true; }
		}

		public bool Remove(TItem item)
		{
			bool removed = false;
			try
			{
				entriesTable.Remove(item.Key);

				// remove from array
				for (int i = entries.Count - 1; i >= 0; i--)
				{
					TItem entry = (TItem)entries[i];
					if (comparer.Compare(item.Key, entry.Key) == 0)
					{
						entries.RemoveAt(i);
					}
				}

				removed = true;
			}
			catch
			{
				removed = false;
			}

			return removed;
		}

		#endregion

		#region IEnumerable<T> Members

		public IEnumerator<TItem> GetEnumerator()
		{
			return entries.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return entries.GetEnumerator();
		}

		#endregion
        /// <summary>
        /// 获取实体类列表 added by candy 2013-10-10
        /// </summary>
        /// <returns></returns>
        public List<TItem> GetEntries() {

            return entries;
        }
	}
}
