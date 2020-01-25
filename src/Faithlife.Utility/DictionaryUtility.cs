using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Faithlife.Utility
{
	/// <summary>
	/// Provides methods for manipulating dictionaries.
	/// </summary>
	public static class DictionaryUtility
	{
		/// <summary>
		/// Returns true if there is a one-to-one relationship between every key-value pair.
		/// </summary>
		/// <remarks>Uses the default equality comparer for TValue if none is specified.</remarks>
		public static bool AreEqual<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> left, IReadOnlyDictionary<TKey, TValue> right, IEqualityComparer<TValue>? comparer = null)
		{
			comparer ??= EqualityComparer<TValue>.Default;

			if (left == right)
				return true;

			if (left == null || right == null || left.Count != right.Count)
				return false;

			foreach (KeyValuePair<TKey, TValue> pair in left)
			{
				if (!right.TryGetValue(pair.Key, out var value) || !comparer.Equals(pair.Value, value))
					return false;
			}

			return true;
		}

		/// <summary>
		/// Wraps the dictionary in a read-only dictionary.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="dictionary">The dictionary to wrap.</param>
		/// <returns>The read-only dictionary.</returns>
		public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dictionary) => new ReadOnlyDictionary<TKey, TValue>(dictionary);

		/// <summary>
		/// Represents the sequence of key-value pairs as a <see cref="IReadOnlyDictionary{TKey,TValue}"/>.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="keyValuePairs">The key-value pairs.</param>
		/// <returns>A <see cref="IReadOnlyDictionary{TKey,TValue}"/> containing the key-value pairs in the sequence.</returns>
		/// <remarks>If the sequence is an <see cref="IReadOnlyDictionary{TKey,TValue}"/>, it is returned directly.
		/// If it is an <see cref="IDictionary{TKey,TValue}"/>, an adapter is created to wrap it. Otherwise, the sequence
		/// is copied into a <see cref="Dictionary{TKey,TValue}"/> and then wrapped in a <see cref="ReadOnlyDictionary{TKey,TValue}"/>.
		/// This method is useful for forcing evaluation of a potentially lazy sequence while retaining reasonable
		/// performance for sequences that are already an <see cref="IReadOnlyDictionary{TKey,TValue}"/> or <see cref="IDictionary{TKey,TValue}"/>.</remarks>
		public static IReadOnlyDictionary<TKey, TValue> AsReadOnlyDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> keyValuePairs)
		{
			return keyValuePairs as IReadOnlyDictionary<TKey, TValue> ??
				(keyValuePairs is IDictionary<TKey, TValue> dictionary ?
					(IReadOnlyDictionary<TKey, TValue>) new ReadOnlyDictionaryAdapter<TKey, TValue>(dictionary) :
					keyValuePairs.ToDictionary(x => x.Key, x => x.Value).AsReadOnly());
		}

		/// <summary>
		/// Gets a value from the dictionary, adding and returning a new instance if it is missing.
		/// </summary>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="key">The key.</param>
		/// <returns>The new or existing value.</returns>
		public static TValue GetOrAddValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) where TValue : new()
		{
			if (dictionary.TryGetValue(key, out var value))
				return value;
			value = new TValue();
			dictionary.Add(key, value);
			return value;
		}

		/// <summary>
		/// Gets a value from the dictionary, adding and returning a new instance if it is missing.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="key">The key.</param>
		/// <param name="creator">Used to create a new value if necessary</param>
		/// <returns>The new or existing value.</returns>
		public static TValue GetOrAddValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> creator)
		{
			if (dictionary.TryGetValue(key, out var value))
				return value;
			value = creator();
			dictionary.Add(key, value);
			return value;
		}

		/// <summary>
		/// Gets a value from the dictionary, returning a default value if it is missing.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="key">The key.</param>
		/// <returns>The value, or a default value.</returns>
		public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key)
		{
			// specification for IDictionary<> requires that the returned value be the default if it fails
			dictionary.TryGetValue(key, out var value);
			return value;
		}

		/// <summary>
		/// Gets a value from the dictionary, returning the specified default value if it is missing.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="key">The key.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <returns>The value, or a default value.</returns>
		public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
			=> dictionary.TryGetValue(key, out var value) ? value : defaultValue;

		/// <summary>
		/// Gets a value from the dictionary, returning the generated default value if it is missing.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="key">The key.</param>
		/// <param name="getDefaultValue">The default value generator.</param>
		/// <returns>The value, or a default value.</returns>
		public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> getDefaultValue)
			=> dictionary.TryGetValue(key, out var value) ? value : getDefaultValue();

		/// <summary>
		/// Creates a key value pair.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <returns>The key value pair.</returns>
		public static KeyValuePair<TKey, TValue> CreateKeyValuePair<TKey, TValue>(TKey key, TValue value) => new KeyValuePair<TKey, TValue>(key, value);

		/// <summary>
		/// Tries to add a value to the dictionary.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <returns>True if successful, i.e. the key was not already in the dictionary.</returns>
		/// <remarks>Unfortunately, there is no more efficient way to do this on an IDictionary than to check
		/// ContainsKey before calling Add.</remarks>
		public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
		{
			if (dictionary.ContainsKey(key))
				return false;

			dictionary.Add(key, value);
			return true;
		}
	}
}
