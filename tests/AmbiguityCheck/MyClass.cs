using System;
using System.Collections.Generic;
using System.Linq;
using Faithlife.Utility;

// This code checks that there are no ambiguous method compile errors between Faithlife.Utility extension methods and those built into the framework.
// This is not in the Faithlife.Utility namespace because namespace affects extension method resolution.
// The code is expected to compile on all target frameworks that Faithlife.Utility supports, but it does not run.
namespace AmbiguityCheck
{
	public static class MyClass
	{
		public static void Foo()
		{
			Dictionary<string, object> dictionary = default!;
			dictionary.GetValueOrDefault("key");
			dictionary.GetValueOrDefault("key", "default");
			dictionary.TryAdd("key", "value");

			IReadOnlyDictionary<string, object> readOnlyDictionary = default!;
			readOnlyDictionary.GetValueOrDefault("key");
			readOnlyDictionary.GetValueOrDefault("key", "default");

			IEnumerable<string> enumerable = default!;
			IEnumerable<string> enumerable2 = default!;
			enumerable.Zip(enumerable2);

			enumerable.TakeLast(1);

			enumerable.DistinctBy(x => x);
			enumerable.DistinctBy(x => x, StringComparer.OrdinalIgnoreCase);
		}
	}
}
