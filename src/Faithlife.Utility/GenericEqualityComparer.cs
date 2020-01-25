using System;
using System.Collections.Generic;

namespace Faithlife.Utility
{
	/// <summary>
	/// Implements IEqualityComparer interfaces with delegates.
	/// </summary>
	/// <typeparam name="T">Type of the object to compare.</typeparam>
	internal sealed class GenericEqualityComparer<T> : EqualityComparer<T>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GenericEqualityComparer&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="equals">The equals delegate.</param>
		/// <param name="getHashCode">The hash code delegate.</param>
		/// <remarks>If getHashCode is null and GetHashCode is called, it will throw a NotImplementedException.</remarks>
		public GenericEqualityComparer(Func<T, T, bool> equals, Func<T, int>? getHashCode = null)
		{
			m_equals = equals ?? throw new ArgumentNullException(nameof(equals));
			m_getHashCode = getHashCode ?? (_ => throw new NotImplementedException());
		}

		/// <summary>
		/// When overridden in a derived class, determines whether two objects are equal.
		/// </summary>
		/// <param name="x">The first object to compare.</param>
		/// <param name="y">The second object to compare.</param>
		/// <returns>
		/// true if the specified objects are equal; otherwise, false.
		/// </returns>
		public override bool Equals(T x, T y) => m_equals(x, y);

		/// <summary>
		/// When overridden in a derived class, serves as a hash function for the specified object for hashing algorithms and data structures, such as a hash table.
		/// </summary>
		/// <param name="obj">The object for which to get a hash code.</param>
		/// <returns>A hash code for the specified object.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.
		/// </exception>
		public override int GetHashCode(T obj) => m_getHashCode(obj);

		readonly Func<T, T, bool> m_equals;
		readonly Func<T, int> m_getHashCode;
	}
}
