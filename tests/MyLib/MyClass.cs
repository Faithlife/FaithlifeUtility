using System;
using System.Collections.Generic;
using Faithlife.Utility;

namespace MyLib
{
	public class MyClass
	{
		public static TValue DoGetValueOrDefault<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> dict, TKey key)
		{
			return dict.GetValueOrDefault(key);
		}
	}
}
