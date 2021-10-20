using System.Collections.Generic;
using Faithlife.Utility;

namespace MyLib
{
	public static class MyClass
	{
		public static TValue? DoGetValueOrDefault<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> dict, TKey key)
			where TKey : notnull
			=> dict.GetValueOrDefault(key);

		public static bool DoTryAdd<TKey, TValue>(IDictionary<TKey, TValue> dict, TKey key, TValue value)
			where TKey : notnull
			=> dict.TryAdd(key, value);

		public static IReadOnlyList<T> DoTakeLast<T>(IEnumerable<T> source, int count) => source.TakeLast(count);
	}
}
