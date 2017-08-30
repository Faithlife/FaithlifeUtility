using System;
using System.Collections.Generic;

namespace Faithlife.Utility
{
	/// <summary>
	/// Provides methods for manipulating arrays.
	/// </summary>
	public static class ArrayUtility
	{
		/// <summary>
		/// Creates a GenericComparer that uses ArrayUtility.Compare.
		/// </summary>
		/// <returns>A GenericComparer that uses ArrayUtility.Compare.</returns>
		public static IComparer<T[]> CreateComparer<T>()
		{
			// Mono breaks if you try to use ArrayUtility.Compare w/o explicitly constructing a delegate.- Novell/Mono Bug #523683
			return new GenericComparer<T[]>((a, b) => ArrayUtility.Compare(a, b));
		}

		/// <summary>
		/// Returns true if the arrays are equal.
		/// </summary>
		/// <param name="array1">The first array.</param>
		/// <param name="array2">The second array.</param>
		/// <returns>true if the arrays are both null, or if they both have equal length and the first
		/// item of the first array is equal to the first item of the second array, etc.</returns>
		public static bool AreEqual<T>(T[] array1, T[] array2)
		{
			return AreEqual(array1, array2, EqualityComparer<T>.Default);
		}

		/// <summary>
		/// Returns true if the arrays are equal.
		/// </summary>
		/// <param name="array1">The first array.</param>
		/// <param name="array2">The second array.</param>
		/// <param name="comparer">The equality comparer to use.</param>
		/// <returns>true if the arrays are both null, or if they both have equal length and the first
		/// item of the first array is equal to the first item of the second array, etc.</returns>
		public static bool AreEqual<T>(T[] array1, T[] array2, IEqualityComparer<T> comparer)
		{
			if (array1 == null)
				return array2 == null;
			if (array2 == null)
				return false;
			if (array1.Length != array2.Length)
				return false;
			for (int nIndex = 0; nIndex < array1.Length; nIndex++)
			{
				if (!comparer.Equals(array1[nIndex], array2[nIndex]))
					return false;
			}
			return true;
		}

		/// <summary>
		/// Clones the specified array.
		/// </summary>
		/// <param name="array">The array to clone.</param>
		/// <returns>A clone of the specified array.</returns>
		/// <remarks>This method is merely useful in avoiding the cast that is otherwise necessary
		/// when calling <see cref="System.Array.Clone" />.</remarks>
		public static T[] Clone<T>(T[] array)
		{
			return (T[]) array.Clone();
		}

		/// <summary>
		/// Compares the two arrays.
		/// </summary>
		/// <param name="array1">The first array.</param>
		/// <param name="array2">The second array.</param>
		/// <returns>The result of a lexicographical comparison of the array items.</returns>
		public static int Compare<T>(T[] array1, T[] array2)
		{
			return Compare(array1, array2, Comparer<T>.Default);
		}

		/// <summary>
		/// Compares the two arrays.
		/// </summary>
		/// <param name="array1">The first array.</param>
		/// <param name="array2">The second array.</param>
		/// <param name="comparer">Used to compare the two items.</param>
		/// <returns>The result of a lexicographical comparison of the array items.</returns>
		public static int Compare<T>(T[] array1, T[] array2, IComparer<T> comparer)
		{
			if (array1 == null)
				return array2 == null ? 0 : -1;
			if (array2 == null)
				return 1;
			int nLength1 = array1.Length;
			int nLength2 = array2.Length;
			int nIndex = 0;
			while (true)
			{
				if (nLength1 == nIndex)
					return nLength2 == nIndex ? 0 : -1;
				if (nLength2 == nIndex)
					return 1;
				T item1 = array1[nIndex];
				T item2 = array2[nIndex];
				int nCompare = comparer.Compare(item1, item2);
				if (nCompare != 0)
					return nCompare;
				nIndex++;
			}
		}

		/// <summary>
		/// Concatenates the specified arrays.
		/// </summary>
		/// <param name="arrays">The arrays to concatenate.</param>
		/// <returns>A new array with all of the elements of the specified arrays.</returns>
		public static T[] Concatenate<T>(params T[][] arrays)
		{
			if (arrays == null)
				throw new ArgumentNullException("arrays");
			int nTotalLength = 0;
			foreach (T[] array in arrays)
			{
				if (array == null)
					throw new ArgumentOutOfRangeException("arrays");
				nTotalLength += array.Length;
			}
			T[] arrayResult = new T[nTotalLength];
			nTotalLength = 0;
			foreach (T[] array in arrays)
			{
				array.CopyTo(arrayResult, nTotalLength);
				nTotalLength += array.Length;
			}
			return arrayResult;
		}
	}
}
