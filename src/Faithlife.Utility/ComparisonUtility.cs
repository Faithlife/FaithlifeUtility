using System;
using System.Collections.Generic;

namespace Faithlife.Utility
{
	/// <summary>
	/// Provides methods to simplify comparing instances.
	/// </summary>
	public static class ComparisonUtility
	{
		/// <summary>
		/// Compares the two instances and returns the greater of the two.
		/// </summary>
		/// <param name="first">The first instance to compare.</param>
		/// <param name="second">The second instance to compare.</param>
		/// <returns>The instance representing the greater value.</returns>
		public static T Max<T>(T first, T second)
		{
			return Comparer<T>.Default.Compare(first, second) < 0 ? second : first;
		}

		/// <summary>
		/// Compares the two instances and returns the lesser of the two.
		/// </summary>
		/// <param name="first">The first instance to compare.</param>
		/// <param name="second">The second instance to compare.</param>
		/// <returns>The instance representing the lesser value.</returns>
		public static T Min<T>(T first, T second)
		{
			return Comparer<T>.Default.Compare(first, second) <= 0 ? first : second;
		}

		/// <summary>
		/// Creates a comparer from a delegate.
		/// </summary>
		/// <typeparam name="T">The type to compare.</typeparam>
		/// <param name="comparer">The compare delegate.</param>
		/// <returns>The comparer.</returns>
		public static Comparer<T> CreateComparer<T>(Func<T, T, int> comparer)
		{
			return new GenericComparer<T>(comparer);
		}

		/// <summary>
		/// Creates a comparer from a delegate.
		/// </summary>
		/// <typeparam name="T">The type to compare.</typeparam>
		/// <param name="comparers">The compare delegates.</param>
		/// <returns>The comparer.</returns>
		public static Comparer<T> CreateComparer<T>(params Func<T, T, int>[] comparers)
		{
			return new GenericComparer<T>(CreateChainedComparison(comparers));
		}

		/// <summary>
		/// Executes a chained comparison between two objects.
		/// </summary>
		/// <typeparam name="T">The type of object.</typeparam>
		/// <param name="left">The left object.</param>
		/// <param name="right">The right object.</param>
		/// <param name="comparers">The comparers.</param>
		/// <returns>Less than zero: x is less than y. Zero: x equals y. Greater than zero: x is greater than y.</returns>
		/// <remarks>Each comparer is executed until one returns non-zero; that value is returned.</remarks>
		public static int ChainedCompare<T>(T left, T right, params Func<T, T, int>[] comparers)
		{
			VerifyComparers(comparers);

			return DoChainedCompare(left, right, comparers);
		}

		/// <summary>
		/// Creates a chained comparison.
		/// </summary>
		/// <typeparam name="T">The type of object</typeparam>
		/// <param name="comparers">The comparers.</param>
		/// <returns>A comparer that executes each comparer until one returns non-zero; that value is returned.</returns>
		public static Func<T, T, int> CreateChainedComparison<T>(params Func<T, T, int>[] comparers)
		{
			VerifyComparers(comparers);

			return (left, right) => DoChainedCompare(left, right, comparers);
		}

		private static void VerifyComparers<T>(Func<T, T, int>[] comparers)
		{
			if (comparers == null)
				throw new ArgumentNullException(nameof(comparers));
			if (comparers.Length == 0)
				throw new ArgumentException("Must supply at least one comparer; use Comparer<T>.Default for a default comparer.", nameof(comparers));
		}

		private static int DoChainedCompare<T>(T left, T right, Func<T, T, int>[] comparers)
		{
			foreach (var comparer in comparers)
			{
				int nCompare = comparer(left, right);
				if (nCompare != 0)
					return nCompare;
			}

			return 0;
		}
	}
}
