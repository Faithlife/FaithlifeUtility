using System;
using System.Collections;
using System.Collections.Generic;

namespace Faithlife.Utility
{
	/// <summary>
	/// <see cref="ReadOnlyListAdapter{T}"/> adapts an <see cref="IList{T}"/> as an <see cref="IReadOnlyList{T}"/>.
	/// </summary>
	/// <typeparam name="T">The type of item in the collection.</typeparam>
	internal sealed class ReadOnlyListAdapter<T> : IReadOnlyList<T>
	{
		public ReadOnlyListAdapter(IList<T> list) => m_list = list ?? throw new ArgumentNullException(nameof(list));

		public int Count => m_list.Count;
		public T this[int index] => m_list[index];
		public IEnumerator<T> GetEnumerator() => m_list.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) m_list).GetEnumerator();

		private readonly IList<T> m_list;
	}
}
