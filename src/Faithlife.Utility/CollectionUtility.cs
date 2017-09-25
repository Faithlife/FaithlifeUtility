using System;
using System.Collections.Generic;

namespace Faithlife.Utility
{
	/// <summary>
	/// Methods for manipulating collections.
	/// </summary>
	public static class CollectionUtility
	{
		/// <summary>
		/// Adds the sequence of items to the collection.
		/// </summary>
		/// <typeparam name="T">The type of item in the collection.</typeparam>
		/// <param name="collection">The collection.</param>
		/// <param name="items">The sequence of items to add to the collection.</param>
		public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
		{
			if (collection == null)
				throw new ArgumentNullException(nameof(collection));
			if (collection.IsReadOnly)
				throw new ArgumentException(OurMessages.Argument_CollectionReadOnly, nameof(collection));
			if (items == null)
				throw new ArgumentNullException(nameof(items));

			// check for, and invoke, the most efficient method that is available
			if (collection is List<T> list)
			{
				list.AddRange(items);
			}
			else
			{
				foreach (T item in items)
					collection.Add(item);
			}
		}

		/// <summary>
		/// Adds the specified value to the collection if the value is not null.
		/// </summary>
		/// <typeparam name="T">The type of item in the collection.</typeparam>
		/// <param name="collection">The collection. Must not be null.</param>
		/// <param name="value">The value to add. May be null.</param>
		public static void AddIfNotNull<T>(this ICollection<T> collection, T value)
		{
			if (value != null)
				collection.Add(value);
		}

		/// <summary>
		/// Adds the specified value to the collection if the value is not null.
		/// </summary>
		/// <typeparam name="T">The type of item in the collection.</typeparam>
		/// <param name="collection">The collection. Must not be null.</param>
		/// <param name="value">The value to add. May be null.</param>
		public static void AddIfNotNull<T>(this ICollection<T> collection, T? value)
			where T : struct
		{
			if (value.HasValue)
				collection.Add(value.Value);
		}
	}
}
