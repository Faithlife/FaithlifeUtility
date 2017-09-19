using System;
using System.Collections.Generic;

namespace Faithlife.Utility
{
	/// <summary>
	/// An EqualityComparer which tests equality on a key type
	/// </summary>
	/// <typeparam name="TSource">The source object type</typeparam>
	/// <typeparam name="TKey">The key object type</typeparam>
	internal class KeyEqualityComparer<TSource, TKey> : EqualityComparer<TSource>
	{
		/// <summary>
		/// Initializes a new instance of the KeyEqualityComparer class.
		/// </summary>
		/// <param name="keySelector">The key selector delegate</param>
		public KeyEqualityComparer(Func<TSource, TKey> keySelector) :
			this(keySelector, EqualityComparer<TKey>.Default)
		{
		}

		/// <summary>
		/// Initializes a new instance of the KeyEqualityComparer class.
		/// </summary>
		/// <param name="keySelector">The key selector delegate</param>
		/// <param name="keyComparer">The IEqualityComparer used for comparison of keys</param>
		public KeyEqualityComparer(Func<TSource, TKey> keySelector, IEqualityComparer<TKey> keyComparer)
		{
			if (keySelector == null)
				throw new ArgumentNullException(nameof(keySelector));
			if (keyComparer == null)
				throw new ArgumentNullException(nameof(keyComparer));

			m_keySelector = keySelector;
			m_keyComparer = keyComparer;
		}

		/// <summary>
		/// Determines whether two objects are equal.
		/// </summary>
		/// <param name="x">The first object to compare.</param>
		/// <param name="y">The second object to compare.</param>
		/// <returns>
		/// true if the specified objects are equal; otherwise, false.
		/// </returns>
		public override bool Equals(TSource x, TSource y)
		{
			TKey k1 = m_keySelector(x);
			TKey k2 = m_keySelector(y);
			return m_keyComparer.Equals(k1, k2);
		}

		/// <summary>
		/// Serves as a hash function for the specified object for hashing algorithms and data structures, such as a hash table.
		/// </summary>
		/// <param name="obj">The object for which to get a hash code.</param>
		/// <returns>A hash code for the specified object.</returns>
		/// <exception cref="T:System.ArgumentNullException">The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.</exception>
		public override int GetHashCode(TSource obj)
		{
			return obj == null ? 0 : m_keyComparer.GetHashCode(m_keySelector(obj));
		}

		readonly Func<TSource, TKey> m_keySelector;
		readonly IEqualityComparer<TKey> m_keyComparer;
	}
}
