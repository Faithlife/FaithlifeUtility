using System;
using System.Collections.Generic;

namespace Faithlife.Utility
{
	/// <summary>
	/// An EqualityComparer which tests equality on a key type
	/// </summary>
	/// <typeparam name="TSource">The source object type</typeparam>
	/// <typeparam name="TKey">The key object type</typeparam>
	public class KeyEqualityComparer<TSource, TKey> : EqualityComparer<TSource>
	{
		/// <summary>
		/// Initializes a new instance of the KeyEqualityComparer class.
		/// </summary>
		/// <param name="keySelector">The key selector delegate</param>
		/// <param name="keyComparer">The IEqualityComparer used for comparison of keys</param>
		public KeyEqualityComparer(Func<TSource, TKey> keySelector, IEqualityComparer<TKey> keyComparer = null)
		{
			m_keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
			m_keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
		}

		/// <summary>
		/// Determines whether two objects are equal.
		/// </summary>
		/// <param name="x">The first object to compare.</param>
		/// <param name="y">The second object to compare.</param>
		/// <returns>
		/// true if the specified objects are equal; otherwise, false.
		/// </returns>
		public override bool Equals(TSource x, TSource y) => m_keyComparer.Equals(m_keySelector(x), m_keySelector(y));

		/// <summary>
		/// Serves as a hash function for the specified object for hashing algorithms and data structures, such as a hash table.
		/// </summary>
		/// <param name="obj">The object for which to get a hash code.</param>
		/// <returns>A hash code for the specified object.</returns>
		/// <exception cref="T:System.ArgumentNullException">The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.</exception>
		public override int GetHashCode(TSource obj) => obj == null ? 0 : m_keyComparer.GetHashCode(m_keySelector(obj));

		readonly Func<TSource, TKey> m_keySelector;
		readonly IEqualityComparer<TKey> m_keyComparer;
	}
}
