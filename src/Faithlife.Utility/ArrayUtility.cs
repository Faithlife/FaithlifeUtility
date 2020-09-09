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
		/// Creates a comparer that uses ArrayUtility.Compare.
		/// </summary>
		public static IComparer<T[]> CreateComparer<T>() => new GenericComparer<T[]>(Compare);

		/// <summary>
		/// Returns true if the arrays are equal.
		/// </summary>
		/// <param name="array1">The first array.</param>
		/// <param name="array2">The second array.</param>
		/// <returns>true if the arrays are both null, or if they both have equal length and the first
		/// item of the first array is equal to the first item of the second array, etc.</returns>
		public static bool AreEqual<T>(T[]? array1, T[]? array2) => AreEqual(array1, array2, EqualityComparer<T>.Default);

		/// <summary>
		/// Returns true if the arrays are equal.
		/// </summary>
		/// <param name="array1">The first array.</param>
		/// <param name="array2">The second array.</param>
		/// <param name="comparer">The equality comparer to use.</param>
		/// <returns>true if the arrays are both null, or if they both have equal length and the first
		/// item of the first array is equal to the first item of the second array, etc.</returns>
		public static bool AreEqual<T>(T[]? array1, T[]? array2, IEqualityComparer<T> comparer)
		{
			if (array1 is null)
				return array2 is null;
			if (array2 is null)
				return false;
			if (array1.Length != array2.Length)
				return false;
			for (var nIndex = 0; nIndex < array1.Length; nIndex++)
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
		public static T[] Clone<T>(T[] array) => (T[]) array.Clone();

		/// <summary>
		/// Compares the two arrays.
		/// </summary>
		/// <param name="array1">The first array.</param>
		/// <param name="array2">The second array.</param>
		/// <returns>The result of a lexicographical comparison of the array items.</returns>
		public static int Compare<T>(T[]? array1, T[]? array2) => Compare(array1, array2, Comparer<T>.Default);

		/// <summary>
		/// Compares the two arrays.
		/// </summary>
		/// <param name="array1">The first array.</param>
		/// <param name="array2">The second array.</param>
		/// <param name="comparer">Used to compare the two items.</param>
		/// <returns>The result of a lexicographical comparison of the array items.</returns>
		public static int Compare<T>(T[]? array1, T[]? array2, IComparer<T> comparer)
		{
			if (array1 is null)
				return array2 is null ? 0 : -1;
			if (array2 is null)
				return 1;
			var length1 = array1.Length;
			var length2 = array2.Length;
			var index = 0;
			while (true)
			{
				if (length1 == index)
					return length2 == index ? 0 : -1;
				if (length2 == index)
					return 1;
				var item1 = array1[index];
				var item2 = array2[index];
				var compare = comparer.Compare(item1, item2);
				if (compare != 0)
					return compare;
				index++;
			}
		}

		/// <summary>
		/// Concatenates the specified arrays.
		/// </summary>
		/// <param name="arrays">The arrays to concatenate.</param>
		/// <returns>A new array with all of the elements of the specified arrays.</returns>
		public static T[] Concatenate<T>(params T[][] arrays)
		{
			if (arrays is null)
				throw new ArgumentNullException(nameof(arrays));
			var totalLength = 0;
			foreach (T[] array in arrays)
			{
				if (array is null)
					throw new ArgumentOutOfRangeException(nameof(arrays));
				totalLength += array.Length;
			}
			T[] arrayResult = new T[totalLength];
			totalLength = 0;
			foreach (T[] array in arrays)
			{
				array.CopyTo(arrayResult, totalLength);
				totalLength += array.Length;
			}
			return arrayResult;
		}
	}
}
