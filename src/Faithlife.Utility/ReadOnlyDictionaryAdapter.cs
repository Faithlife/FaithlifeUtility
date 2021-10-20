using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Faithlife.Utility
{
	/// <summary>
	/// <see cref="ReadOnlyDictionaryAdapter{TKey,TValue}"/> adapts an <see cref="IDictionary{TKey,TValue}"/> as an <see cref="IReadOnlyDictionary{TKey,TValue}"/>.
	/// </summary>
	/// <typeparam name="TKey">The type of key in the dictionary.</typeparam>
	/// <typeparam name="TValue">The type of value in the dictionary.</typeparam>
	internal sealed class ReadOnlyDictionaryAdapter<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
		where TKey : notnull
	{
		public ReadOnlyDictionaryAdapter(IDictionary<TKey, TValue> dictionary) => m_dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));

		public int Count => m_dictionary.Count;
		public TValue this[TKey key] => m_dictionary[key];
		public bool ContainsKey(TKey key) => m_dictionary.ContainsKey(key);
		public bool TryGetValue(TKey key,
#if !NETSTANDARD && !NETCOREAPP2_1
			[MaybeNullWhen(false)]
#endif
			out TValue value) => m_dictionary.TryGetValue(key, out value);
		public IEnumerable<TKey> Keys => m_dictionary.Keys;
		public IEnumerable<TValue> Values => m_dictionary.Values;
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => m_dictionary.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) m_dictionary).GetEnumerator();

		private readonly IDictionary<TKey, TValue> m_dictionary;
	}
}
