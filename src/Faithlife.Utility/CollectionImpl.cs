using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Faithlife.Utility
{
	/// <summary>
	/// Provides implementations of common methods needed by an implementer of <see cref="ICollection"/>.
	/// </summary>
	public static class CollectionImpl
	{
		/// <summary>
		/// Performs standard validation of parameters passed to an implementation of <see cref="ICollection.CopyTo"/>.
		/// </summary>
		/// <param name="array">The one-dimensional <see cref="Array"></see> that is the destination of the elements
		/// copied from the <see cref="PriorityQueue{T}"/>. The <see cref="Array"></see> must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
		/// <param name="nCollectionSize">The size of the collection to be copied.</param>
		public static void CheckCopyToParameters(Array array, int index, int nCollectionSize)
		{
			// check for null array
			if (array == null)
				throw new ArgumentNullException("array", OurMessages.ArgumentNull_Array);

			// check for multi-dimensional array
			if (array.Rank != 1)
				throw new ArgumentException(OurMessages.Argument_ArrayMultiDimensional, "array");

			// check for array that isn't zero-based
			if (array.GetLowerBound(0) != 0)
				throw new ArgumentException(OurMessages.Argument_NonZeroLowerBound, "array");

			// check for negative index
			if (index < 0)
				throw new ArgumentOutOfRangeException("index", OurMessages.ArgumentOutOfRange_MustBeNonNegative);

			// check for more source elements than available destination space
			if (array.Length - index < nCollectionSize)
				throw new ArgumentException(OurMessages.Argument_ArrayIndexTooBig);
		}

		/// <summary>
		/// Provides a standard implementation of <see cref="ICollection{T}.CopyTo"/>.
		/// </summary>
		/// <typeparam name="T">The type of the collection.</typeparam>
		/// <param name="source">The source collection (i.e., the object on which CopyTo is being called).</param>
		/// <param name="array">The destination array.</param>
		/// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
		public static void CopyTo<T>(ICollection<T> source, T[] array, int arrayIndex)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			CheckCopyToParameters(array, arrayIndex, source.Count);

			foreach (T item in source)
			{
				array[arrayIndex] = item;
				arrayIndex++;
			}
		}

		/// <summary>
		/// Provides a standard implementation of <see cref="ICollection{T}.Contains"/>.
		/// </summary>
		/// <typeparam name="T">The type of the collection.</typeparam>
		/// <param name="source">The source collection (i.e., the object on which Contains is being called).</param>
		/// <param name="item">The object to locate.</param>
		/// <returns><c>true</c> if <paramref name="item"/> is found in <paramref name="source"/>; otherwise <c>false</c>.</returns>
		public static bool Contains<T>(ICollection<T> source, T item)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			foreach (T sourceItem in source)
			{
				if (sourceItem == null && item == null)
					return true;
				if (sourceItem != null && sourceItem.Equals(item))
					return true;
			}

			return false;
		}
	}
}
