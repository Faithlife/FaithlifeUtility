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
		/// <param name="coll">The collection.</param>
		/// <param name="seqItems">The sequence of items to add to the collection.</param>
		public static void AddRange<T>(this ICollection<T> coll, IEnumerable<T> seqItems)
		{
			if (coll == null)
				throw new ArgumentNullException("coll");
			if (coll.IsReadOnly)
				throw new ArgumentException(OurMessages.Argument_CollectionReadOnly, "coll");
			if (seqItems == null)
				throw new ArgumentNullException("seqItems");

			// check for, and invoke, the most efficient method that is available
			List<T> list = coll as List<T>;
			if (list != null)
			{
				list.AddRange(seqItems);
			}
			else
			{
				foreach (T item in seqItems)
					coll.Add(item);
			}
		}

		/// <summary>
		/// Converts the specified collection to an array.
		/// </summary>
		/// <param name="coll">The collection to convert.</param>
		/// <param name="fnConvert">Converts from collection elements to array elements.</param>
		/// <returns>An array with the elements of the collection.</returns>
		public static TOutput[] ToArray<TInput, TOutput>(this ICollection<TInput> coll, Func<TInput, TOutput> fnConvert)
		{
			if (coll == null)
				throw new ArgumentNullException("coll");
			int nCount = coll.Count;
			TOutput[] aResult = new TOutput[nCount];
			int nIndex = 0;
			foreach (TInput input in coll)
				aResult[nIndex++] = fnConvert(input);
			return aResult;
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
