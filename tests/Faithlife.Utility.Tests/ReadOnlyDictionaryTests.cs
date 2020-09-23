using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class ReadOnlyDictionaryTests
	{
		[Test]
		public void ConstructorArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new ReadOnlyDictionary<int, int>(null));
		}

		[Test]
		public void CreateReadOnlyDictionary()
		{
			var dict = new Dictionary<string, bool>();
			var dictReadOnly = new ReadOnlyDictionary<string, bool>(dict);
			Assert.IsNotNull(dictReadOnly);
		}

		[Test]
		public void GetDictionaryCount()
		{
			var dict = new Dictionary<string, bool>();
			dict.Add("LLS:1.0.3", true);

			var dictReadOnly = new ReadOnlyDictionary<string, bool>(dict);
			Assert.AreEqual(1, dictReadOnly.Count);

			dict.Add("LLS:1.0.13", true);
			Assert.AreEqual(2, dictReadOnly.Count);
		}

		[Test]
		public void DictionaryContains()
		{
			var dict = new Dictionary<string, bool>();
			var keyvalue = new KeyValuePair<string, bool>("LLS:1.0.3", true);
			dict.Add(keyvalue.Key, keyvalue.Value);

			var dictReadOnly = new ReadOnlyDictionary<string, bool>(dict);
			Assert.IsTrue(((IDictionary<string, bool>) dictReadOnly).Contains(keyvalue));
		}

		[Test]
		public void DictionaryDoesNotContain()
		{
			var dictReadOnly = new ReadOnlyDictionary<string, bool>(new Dictionary<string, bool>());
			Assert.IsFalse(((IDictionary<string, bool>) dictReadOnly).Contains(new KeyValuePair<string, bool>("LLS:1.0.3", true)));
		}

		[Test]
		public void DictionaryTryGetSuccess()
		{
			var dict = new Dictionary<string, bool>();
			dict.Add("LLS:1.0.3", true);

			var dictReadOnly = new ReadOnlyDictionary<string, bool>(dict);

			var bGotValue = false;
			bGotValue = dictReadOnly.TryGetValue("LLS:1.0.3", out var bValue);
			Assert.IsTrue(bGotValue);
			Assert.IsTrue(bValue);
		}

		[Test]
		public void DictionaryTryGetFail()
		{
			var dictReadOnly = new ReadOnlyDictionary<string, bool>(new Dictionary<string, bool>());

			var bGotValue = false;
			bGotValue = dictReadOnly.TryGetValue("LLS:1.0.3", out var bValue);
			Assert.IsFalse(bGotValue);
			Assert.IsFalse(bValue);
		}

		[Test]
		public void ForEachDictionaryItem()
		{
			var dict = new Dictionary<string, bool>();
			dict.Add("LLS:1.0.3", true);
			dict.Add("LLS:1.0.4", true);
			dict.Add("LLS:1.0.5", true);

			var dictReadOnly = new ReadOnlyDictionary<string, bool>(dict);

			foreach (var pair in dictReadOnly)
				Assert.IsTrue(pair.Value);
		}
	}
}
