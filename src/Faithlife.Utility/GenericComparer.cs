using System;
using System.Collections.Generic;

namespace Faithlife.Utility
{
	/// <summary>
	/// Implements IComparer interfaces with a delegate.
	/// </summary>
	/// <typeparam name="T">Type of the object to compare.</typeparam>
	internal sealed class GenericComparer<T> : Comparer<T>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GenericComparer{T}"/> class.
		/// </summary>
		/// <param name="comparer">The comparer delegate.</param>
		public GenericComparer(Func<T, T, int> comparer)
		{
			m_comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
		}

		/// <summary>
		/// Performs a comparison of two objects of the same type.
		/// </summary>
		/// <param name="x">The first object to compare.</param>
		/// <param name="y">The second object to compare.</param>
		/// <returns>Less than zero: x is less than y. Zero: x equals y. Greater than zero: x is greater than y.</returns>
		public override int Compare(T x, T y) => m_comparer(x, y);

		readonly Func<T, T, int> m_comparer;
	}
}
