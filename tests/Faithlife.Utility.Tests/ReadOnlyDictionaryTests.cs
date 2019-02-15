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
			Dictionary<string, bool> dict = new Dictionary<string, bool>();
			ReadOnlyDictionary<string, bool> dictReadOnly = new ReadOnlyDictionary<string, bool>(dict);
			Assert.IsNotNull(dictReadOnly);
		}

		[Test]
		public void GetDictionaryCount()
		{
			Dictionary<string, bool> dict = new Dictionary<string, bool>();
			dict.Add("LLS:1.0.3", true);

			ReadOnlyDictionary<string, bool> dictReadOnly = new ReadOnlyDictionary<string, bool>(dict);
			Assert.AreEqual(1, dictReadOnly.Count);

			dict.Add("LLS:1.0.13", true);
			Assert.AreEqual(2, dictReadOnly.Count);
		}

		[Test]
		public void DictionaryContains()
		{
			Dictionary<string, bool> dict = new Dictionary<string, bool>();
			KeyValuePair<string, bool> keyvalue = new KeyValuePair<string, bool>("LLS:1.0.3", true);
			dict.Add(keyvalue.Key, keyvalue.Value);

			ReadOnlyDictionary<string, bool> dictReadOnly = new ReadOnlyDictionary<string, bool>(dict);
			Assert.IsTrue(((IDictionary<string, bool>) dictReadOnly).Contains(keyvalue));
		}

		[Test]
		public void DictionaryDoesNotContain()
		{
			ReadOnlyDictionary<string, bool> dictReadOnly = new ReadOnlyDictionary<string, bool>(new Dictionary<string, bool>());
			Assert.IsFalse(((IDictionary<string, bool>) dictReadOnly).Contains(new KeyValuePair<string, bool>("LLS:1.0.3", true)));
		}

		[Test]
		public void DictionaryTryGetSuccess()
		{
			Dictionary<string, bool> dict = new Dictionary<string, bool>();
			dict.Add("LLS:1.0.3", true);

			ReadOnlyDictionary<string, bool> dictReadOnly = new ReadOnlyDictionary<string, bool>(dict);

			bool bGotValue = false;
			bool bValue;
			bGotValue = dictReadOnly.TryGetValue("LLS:1.0.3", out bValue);
			Assert.IsTrue(bGotValue);
			Assert.IsTrue(bValue);
		}

		[Test]
		public void DictionaryTryGetFail()
		{
			ReadOnlyDictionary<string, bool> dictReadOnly = new ReadOnlyDictionary<string, bool>(new Dictionary<string, bool>());

			bool bGotValue = false;
			bool bValue = false;
			bGotValue = dictReadOnly.TryGetValue("LLS:1.0.3", out bValue);
			Assert.IsFalse(bGotValue);
			Assert.IsFalse(bValue);
		}

		[Test]
		public void ForEachDictionaryItem()
		{
			Dictionary<string, bool> dict = new Dictionary<string, bool>();
			dict.Add("LLS:1.0.3", true);
			dict.Add("LLS:1.0.4", true);
			dict.Add("LLS:1.0.5", true);

			ReadOnlyDictionary<string, bool> dictReadOnly = new ReadOnlyDictionary<string, bool>(dict);

			foreach (KeyValuePair<string, bool> pair in dictReadOnly)
				Assert.IsTrue(pair.Value);
		}
	}
}
