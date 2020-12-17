using System;
using System.Collections;
using System.Collections.Generic;

namespace Faithlife.Utility
{
	/// <summary>
	/// Implements a read-only wrapper around a <see cref="ISet{T}"/>.
	/// </summary>
	/// <typeparam name="T">The type of item in the ReadOnlySet.</typeparam>
	public sealed class ReadOnlySet<T> : ISet<T>, IReadOnlyCollection<T>
#if NET5
		, IReadOnlySet<T>
#endif
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReadOnlySet{T}"/> class.
		/// </summary>
		/// <param name="set">The <see cref="ISet{T}"/> to wrap.</param>
		public ReadOnlySet(ISet<T> set)
		{
			m_set = set ?? throw new ArgumentNullException(nameof(set));
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		public IEnumerator<T> GetEnumerator() => m_set.GetEnumerator();

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		IEnumerator IEnumerable.GetEnumerator() => m_set.GetEnumerator();

		/// <summary>
		/// Adds an item to the collection. This method throws a <see cref="NotSupportedException"/>.
		/// </summary>
		void ICollection<T>.Add(T item) => throw CreateReadOnlyException();

		/// <summary>
		/// Adds an item to the collection. This method throws a <see cref="NotSupportedException"/>.
		/// </summary>
		bool ISet<T>.Add(T item) => throw CreateReadOnlyException();

		/// <summary>
		/// Modifies the current set so that it contains all elements that are present in the current set, in the specified collection, or in both.
		/// This method throws a <see cref="NotSupportedException"/>.
		/// </summary>
		void ISet<T>.UnionWith(IEnumerable<T> other) => throw CreateReadOnlyException();

		/// <summary>
		/// Modifies the current set so that it contains only elements that are also in a specified collection.
		/// This method throws a <see cref="NotSupportedException"/>.
		/// </summary>
		void ISet<T>.IntersectWith(IEnumerable<T> other) => throw CreateReadOnlyException();

		/// <summary>
		/// Removes all elements in the specified collection from the current set.
		/// This method throws a <see cref="NotSupportedException"/>.
		/// </summary>
		void ISet<T>.ExceptWith(IEnumerable<T> other) => throw CreateReadOnlyException();

		/// <summary>
		/// Modifies the current set so that it contains only elements that are present either in the current set or in the specified collection, but not both.
		/// This method throws a <see cref="NotSupportedException"/>.
		/// </summary>
		void ISet<T>.SymmetricExceptWith(IEnumerable<T> other) => throw CreateReadOnlyException();

		/// <summary>
		/// Determines whether a set is a subset of a specified collection.
		/// </summary>
		public bool IsSubsetOf(IEnumerable<T> other) => m_set.IsSubsetOf(other);

		/// <summary>
		/// Determines whether the current set is a proper (strict) subset of a specified collection.
		/// </summary>
		public bool IsProperSubsetOf(IEnumerable<T> other) => m_set.IsProperSubsetOf(other);

		/// <summary>
		/// Determines whether the current set is a superset of a specified collection.
		/// </summary>
		public bool IsSupersetOf(IEnumerable<T> other) => m_set.IsSupersetOf(other);

		/// <summary>
		/// Determines whether the current set is a proper (strict) superset of a specified collection.
		/// </summary>
		public bool IsProperSupersetOf(IEnumerable<T> other) => m_set.IsProperSupersetOf(other);

		/// <summary>
		/// Determines whether the current set overlaps with the specified collection.
		/// </summary>
		public bool Overlaps(IEnumerable<T> other) => m_set.Overlaps(other);

		/// <summary>
		/// Determines whether the current set and the specified collection contain the same elements.
		/// </summary>
		public bool SetEquals(IEnumerable<T> other) => m_set.SetEquals(other);

		/// <summary>
		/// Copies the elements of the ReadOnlySet to an Array, starting at a particular Array index.
		/// </summary>
		public void CopyTo(T[] array, int arrayIndex) => m_set.CopyTo(array, arrayIndex);

		/// <summary>
		/// Removes all items from the set. This method throws a <see cref="NotSupportedException"/>.
		/// </summary>
		void ICollection<T>.Clear() => throw CreateReadOnlyException();

		/// <summary>
		/// Determines whether the set contains a specific value.
		/// </summary>
		public bool Contains(T item) => m_set.Contains(item);

		/// <summary>
		/// Removes a value from the set. This method throws a <see cref="NotSupportedException"/>.
		/// </summary>
		bool ICollection<T>.Remove(T item) => throw CreateReadOnlyException();

		/// <summary>
		/// Gets the number of elements contained in the set.
		/// </summary>
		public int Count => m_set.Count;

		/// <summary>
		/// Returns <c>true</c>; this collection is read-only.
		/// </summary>
		public bool IsReadOnly => true;

		private static NotSupportedException CreateReadOnlyException() => new NotSupportedException("ReadOnlySet<T> may not be modified.");

		private readonly ISet<T> m_set;
	}
}
