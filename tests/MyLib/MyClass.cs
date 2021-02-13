using System.Collections.Generic;
using Faithlife.Utility;

namespace MyLib
{
	public class MyClass
	{
		public static TValue? DoGetValueOrDefault<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> dict, TKey key) => dict.GetValueOrDefault(key);

		public static bool DoTryAdd<TKey, TValue>(IDictionary<TKey, TValue> dict, TKey key, TValue value) => dict.TryAdd(key, value);
	}
}
