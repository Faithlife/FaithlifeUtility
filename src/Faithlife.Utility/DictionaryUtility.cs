using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Faithlife.Utility
{
	/// <summary>
	/// Provides methods for manipulating dictionaries.
	/// </summary>
	public static class DictionaryUtility
	{
		/// <summary>
		/// Returns true if there is a one-to-one relationship between every KeyValue pair.
		/// </summary>
		/// <remarks>Uses the default equality comparer for TValue if none is specified.</remarks>
		public static bool AreEqual<TKey, TValue>(IDictionary<TKey, TValue> left, IDictionary<TKey, TValue> right, IEqualityComparer<TValue> comparer = null)
		{
			comparer = comparer ?? EqualityComparer<TValue>.Default;

			if (left == right)
				return true;

			if (left == null || right == null || left.Count != right.Count)
				return false;

			var leftDictionary = left as Dictionary<TKey, TValue>;
			var rightDictionary = right as Dictionary<TKey, TValue>;

			if (leftDictionary != null && rightDictionary != null && !leftDictionary.Comparer.Equals(rightDictionary.Comparer))
				return false;

			foreach (KeyValuePair<TKey, TValue> pair in left)
			{
				TValue value;
				if (!right.TryGetValue(pair.Key, out value) || !comparer.Equals(pair.Value, value))
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
		public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
		{
			return new ReadOnlyDictionary<TKey, TValue>(dictionary);
		}

		/// <summary>
		/// Gets a value from the dictionary, adding and returning a new instance if it is missing.
		/// </summary>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="key">The key.</param>
		/// <returns>The new or existing value.</returns>
		public static TValue GetOrAddValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) where TValue : new()
		{
			TValue value;
			if (dictionary.TryGetValue(key, out value))
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
			TValue value;
			if (dictionary.TryGetValue(key, out value))
				return value;
			value = creator();
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
		/// <param name="creator">Used to create a new value if necessary; the function is called with specified key.</param>
		/// <returns>The new or existing value.</returns>
		public static TValue GetOrAddValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> creator)
		{
			TValue value;
			if (dictionary.TryGetValue(key, out value))
				return value;
			value = creator(key);
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
		public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
		{
			// specification for IDictionary<> requires that the returned value be the default if it fails
			TValue value;
			dictionary.TryGetValue(key, out value);
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
		public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
		{
			TValue value;
			return dictionary.TryGetValue(key, out value) ? value : defaultValue;
		}

		/// <summary>
		/// Gets a value from the dictionary, returning the generated default value if it is missing.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="key">The key.</param>
		/// <param name="getDefaultValue">The default value generator.</param>
		/// <returns>The value, or a default value.</returns>
		public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> getDefaultValue)
		{
			TValue value;
			return dictionary.TryGetValue(key, out value) ? value : getDefaultValue();
		}

		/// <summary>
		/// Gets a value from the dictionary, returning null if it is missing.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="key">The key.</param>
		/// <returns>The value, or null.</returns>
		public static TValue? GetNullableValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) where TValue : struct
		{
			TValue value;
			return dictionary.TryGetValue(key, out value) ? value : default(TValue?);
		}

		/// <summary>
		/// Merges <paramref name="otherDictionary"/> into <paramref name="thisDictionary"/>.
		/// The <paramref name="mergeWithStrategy"/> specifies how to resolve any key conflicts,
		/// either taking the new value, retaining the original value, or throwing an exception.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="thisDictionary">The dictionary to modify.</param>
		/// <param name="otherDictionary">The dictionary to merge from.</param>
		/// <param name="mergeWithStrategy">Specifies how to handle key collisions.</param>
		/// <exception cref="InvalidOperationException">
		/// If there is a key conflict and <paramref name="mergeWithStrategy"/> is MergeWithStrategy.ThrowException.
		/// Note that some KeyValuePairs from <paramref name="otherDictionary"/> may have already been added to <paramref name="thisDictionary"/>.
		/// </exception>
		/// <exception cref="ArgumentNullException">If either <paramref name="thisDictionary"/> or <paramref name="otherDictionary"/> is null.</exception>
		public static void MergeWith<TKey, TValue>(this IDictionary<TKey, TValue> thisDictionary, IDictionary<TKey, TValue> otherDictionary, MergeWithStrategy mergeWithStrategy)
		{
			if (otherDictionary == null)
				throw new ArgumentNullException(nameof(otherDictionary));

			if (thisDictionary == null)
				throw new ArgumentNullException(nameof(thisDictionary));

			foreach (KeyValuePair<TKey, TValue> keyValuePair in otherDictionary)
			{
				if (mergeWithStrategy == MergeWithStrategy.OverwriteValue || !thisDictionary.ContainsKey(keyValuePair.Key))
					thisDictionary[keyValuePair.Key] = keyValuePair.Value;
				else if (mergeWithStrategy == MergeWithStrategy.ThrowException)
					throw new InvalidOperationException("Key '{0}' already exists in the dictionary.".FormatInvariant(keyValuePair.Key));
			}
		}

		/// <summary>
		/// Creates a dictionary from key value pairs.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="keyValuePairs">The key value pairs.</param>
		/// <returns>The dictionary.</returns>
		public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> keyValuePairs)
		{
			var dict = new Dictionary<TKey, TValue>();
			foreach (var pair in keyValuePairs)
				dict.Add(pair.Key, pair.Value);
			return dict;
		}

		/// <summary>
		/// Creates a sorted dictionary from key value pairs.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="keyValuePairs">The key value pairs.</param>
		/// <returns>The sorted dictionary.</returns>
		public static SortedDictionary<TKey, TValue> ToSortedDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> keyValuePairs)
		{
			var dict = new SortedDictionary<TKey, TValue>();
			foreach (var pair in keyValuePairs)
				dict.Add(pair.Key, pair.Value);
			return dict;
		}

		/// <summary>
		/// Creates a sorted list from key value pairs.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="keyValuePairs">The key value pairs.</param>
		/// <returns>The sorted list.</returns>
		public static SortedList<TKey, TValue> ToSortedList<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> keyValuePairs)
		{
			var dict = new SortedList<TKey, TValue>();
			foreach (var pair in keyValuePairs)
				dict.Add(pair.Key, pair.Value);
			return dict;
		}

		/// <summary>
		/// Creates a key value pair.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <returns>The key value pair.</returns>
		public static KeyValuePair<TKey, TValue> CreateKeyValuePair<TKey, TValue>(TKey key, TValue value)
		{
			return new KeyValuePair<TKey, TValue>(key, value);
		}

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

		/// <summary>
		/// Tries to get the value and applies the specified conversion.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <typeparam name="TOut">The type of the out parameter.</typeparam>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="key">The key.</param>
		/// <param name="result">The converted result.</param>
		/// <param name="convert">The conversion.</param>
		/// <returns><c>True</c> if the specified key exists in the dictionary; otherwise, <c>false</c>.</returns>
		public static bool TryGetValueWithConversion<TKey, TValue, TOut>(this IDictionary<TKey, TValue> dictionary,
			TKey key, out TOut result, Func<TValue, TOut> convert)
		{
			TValue value;
			if (dictionary.TryGetValue(key, out value))
			{
				result = convert(value);
				return true;
			}

			result = default(TOut);
			return false;
		}

		/// <summary>
		/// Tries to get a value from the dictionary, executing the specified action if it exists.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="key">The key.</param>
		/// <param name="action">The action.</param>
		public static void WithValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Action<TValue> action)
		{
			TValue value;
			if (dictionary.TryGetValue(key, out value))
				action(value);
		}

		/// <summary>
		/// Tries to get a value from the dictionary, executing the specified action if it exists.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="key">The key.</param>
		/// <param name="convert">The action.</param>
		public static TResult WithValue<TKey, TValue, TResult>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue, TResult> convert)
		{
			TValue value;
			return dictionary.TryGetValue(key, out value) ? convert(value) : default(TResult);
		}
	}
}
