using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class DictionaryUtilityTests
	{
		[Test]
		public void DictionariesAreEqual()
		{
			Dictionary<int, int> dict1 = new Dictionary<int, int> { { 1, 1 }, { 2, 2 }, { 3, 3 } };
			Dictionary<int, int> dict2 = new Dictionary<int, int> { { 1, 1 }, { 2, 2 }, { 3, 3 } };

			Assert.IsTrue(DictionaryUtility.AreEqual(dict1, dict2));
		}

		[Test]
		public void DictionariesAreEqualWhenItemsAreAddedOutOfOrder()
		{
			Dictionary<int, int> dict1 = new Dictionary<int, int> { { 1, 1 }, { 2, 2 }, { 3, 3 } };
			Dictionary<int, int> dict2 = new Dictionary<int, int> { { 1, 1 }, { 3, 3 }, { 2, 2 } };

			Assert.IsTrue(DictionaryUtility.AreEqual(dict1, dict2));
		}

		[Test]
		public void DictionariesArNotEqualWhenValueIsdifferent()
		{
			Dictionary<int, int> dict1 = new Dictionary<int, int> { { 1, 1 }, { 2, 2 }, { 3, 3 } };
			Dictionary<int, int> dict2 = new Dictionary<int, int> { { 1, 2 }, { 2, 2 }, { 3, 3 } };

			Assert.IsFalse(DictionaryUtility.AreEqual(dict1, dict2));
		}

		[Test]
		public void DictionariesAreNotEqualWhenSecondIsMissingItem()
		{
			Dictionary<int, int> dict1 = new Dictionary<int, int> { { 1, 1 }, { 2, 2 }, { 3, 3 } };
			Dictionary<int, int> dict2 = new Dictionary<int, int> { { 1, 1 }, { 2, 2 } };

			Assert.IsFalse(DictionaryUtility.AreEqual(dict1, dict2));
		}

		[Test]
		public void DictionariesAreNotEqualWhenFirstIsMissingItem()
		{
			Dictionary<int, int> dict1 = new Dictionary<int, int> { { 1, 1 }, { 2, 2 } };
			Dictionary<int, int> dict2 = new Dictionary<int, int> { { 1, 1 }, { 2, 2 }, { 3, 3 } };

			Assert.IsFalse(DictionaryUtility.AreEqual(dict1, dict2));
		}

		[Test]
		public void DictionariesAreEqualWhenBothAreEmpty()
		{
			Dictionary<int, int> dict1 = new Dictionary<int, int>();
			Dictionary<int, int> dict2 = new Dictionary<int, int>();

			Assert.IsTrue(DictionaryUtility.AreEqual(dict1, dict2));
		}

		[Test]
		public void DictionariesAreEqualWhenSameInstance()
		{
			Dictionary<int, int> dict1 = new Dictionary<int, int> { { 1, 1 }, { 2, 2 }, { 3, 3 } };

			Assert.IsTrue(DictionaryUtility.AreEqual(dict1, dict1));
		}

		[Test]
		public void DictionariesAreEqualWhenUsingCustomComparer()
		{
			Dictionary<int, int> dict1 = new Dictionary<int, int> { { 1, 1 }, { 2, 2 }, { 3, 3 } };
			Dictionary<int, int> dict2 = new Dictionary<int, int> { { 1, 1 }, { 2, 2 }, { 3, 3 } };

			Assert.IsTrue(DictionaryUtility.AreEqual(dict1, dict2, new GenericEqualityComparer<int>((x, y) => x == y)));
		}

		[Test]
		public void DictionariesAreEqualWhenUsingCustomKeyComparer()
		{
			var comparer = new GenericEqualityComparer<int>((x, y) => x == y, HashCodeUtility.GetPersistentHashCode);
			Dictionary<int, int> dict1 = new Dictionary<int, int>(comparer) { { 1, 1 }, { 2, 2 }, { 3, 3 } };
			Dictionary<int, int> dict2 = new Dictionary<int, int>(comparer) { { 1, 1 }, { 2, 2 }, { 3, 3 } };

			Assert.IsTrue(DictionaryUtility.AreEqual(dict1, dict2));
		}

		[Test]
		public void ReadOnlyDictionariesAreEqual()
		{
			ReadOnlyDictionary<int, int> dict1 = new Dictionary<int, int> { { 1, 1 }, { 2, 2 }, { 3, 3 } }.AsReadOnly();
			ReadOnlyDictionary<int, int> dict2 = new Dictionary<int, int> { { 1, 1 }, { 2, 2 }, { 3, 3 } }.AsReadOnly();

			Assert.IsTrue(DictionaryUtility.AreEqual(dict1, dict2));
		}

		[Test]
		public void ReadOnlyDictionariesAreNotEqualWhenValueIsDifferent()
		{
			ReadOnlyDictionary<int, int> dict1 = new Dictionary<int, int> { { 1, 1 }, { 2, 2 }, { 3, 3 } }.AsReadOnly();
			ReadOnlyDictionary<int, int> dict2 = new Dictionary<int, int> { { 1, 2 }, { 2, 2 }, { 3, 3 } }.AsReadOnly();

			Assert.IsFalse(DictionaryUtility.AreEqual(dict1, dict2));
		}

		[Test]
		public void AsReadOnly()
		{
			ReadOnlyDictionary<string, int> dict = new Dictionary<string, int>
			{
				{ "one", 1 },
				{ "two", 2 },
				{ "three", 3 },
			}.AsReadOnly();

			Assert.AreEqual(3, dict.Count);
			Assert.AreEqual(1, dict["one"]);
			Assert.AreEqual(2, dict["two"]);
			Assert.AreEqual(3, dict["three"]);
			CollectionAssert.AreEqual(new[] { 1, 2, 3 }, dict.Values.Order());
		}

		[Test]
		public void DictionaryAsReadOnlyDictionary()
		{
			// AsReadOnlyDictionary should not rewrap a Dictionary
			var dictionary = new Dictionary<int, int> { [1] = 1, [2] = 4 };
			Assert.AreSame(dictionary, dictionary.AsReadOnlyDictionary());
		}

		[Test]
		public void ReadOnlyDictionaryAsReadOnlyDictionary()
		{
			// AsReadOnlyDictionary should not rewrap a ReadOnlyCollection
			var dictionary = new Dictionary<int, int> { [1] = 1, [2] = 4 };
			IEnumerable<KeyValuePair<int, int>> readOnlyDictionary = new ReadOnlyDictionary<int, int>(dictionary);
			Assert.AreSame(readOnlyDictionary, readOnlyDictionary.AsReadOnlyDictionary());
		}

		[Test]
		public void MutateDictionaryAsReadOnlyDictionary()
		{
			// AsReadOnlyDictionary does not guarantee that the collection can't be mutated by someone else
			var dictionary = new Dictionary<int, int> { [1] = 1, [2] = 4 };
			var readOnlyDictionary = dictionary.AsReadOnlyDictionary();
			dictionary.Add(3, 9);
			Assert.AreEqual(3, readOnlyDictionary.Count);
		}

		[Test]
		public void EnumerableAsReadOnlyDictionary()
		{
			// AsReadOnlyDictionary must duplicate a non-IDictionary
			var keyValuePairs = new[] { new KeyValuePair<int, int>(2, 4) };
			var readOnlyDictionary = keyValuePairs.AsReadOnlyDictionary();
			Assert.AreEqual(1, readOnlyDictionary.Count);
			Assert.AreEqual(4, readOnlyDictionary[2]);
		}

		[Test]
		public void GetOrAddItemProblem()
		{
			Dictionary<int, List<int>> dict = new Dictionary<int, List<int>>();
			Assert.Throws<KeyNotFoundException>(() => { var value = dict[1]; });
		}

		[Test]
		public void GetOrAddItemAdd()
		{
			Dictionary<int, List<int>> dict = new Dictionary<int, List<int>>();
			Assert.AreEqual(0, DictionaryUtility.GetOrAddValue(dict, 1).Count);
			Assert.AreEqual(0, dict[1].Count);
		}

		[Test]
		public void GetOrAddItemExists()
		{
			Dictionary<int, List<int>> dict = new Dictionary<int, List<int>>();
			List<int> list = new List<int>();
			dict[2] = list;
			Assert.AreSame(list, DictionaryUtility.GetOrAddValue(dict, 2));
			Assert.AreSame(list, dict[2]);
		}

		[Test]
		public void GetOrAddItemAddValue()
		{
			Dictionary<int, int> dict = new Dictionary<int, int>();
			Assert.AreEqual(0, DictionaryUtility.GetOrAddValue(dict, 0));
			Assert.AreEqual(0, dict[0]);
		}

		[Test]
		public void GetOrAddItemAddCustom()
		{
			Dictionary<int, int> dict = new Dictionary<int, int>();
			Assert.AreEqual(3, DictionaryUtility.GetOrAddValue(dict, 2, () => 3));
			Assert.AreEqual(3, dict[2]);
		}

		[Test]
		public void GetOrAddItemExistingCustom()
		{
			Dictionary<int, int> dict = new Dictionary<int, int>();
			dict[2] = 4;
			Assert.AreEqual(4, DictionaryUtility.GetOrAddValue(dict, 2, () => 3));
			Assert.AreEqual(4, dict[2]);
		}

		[Test]
		public void GetValueOrDefault()
		{
			Dictionary<int, int> dict = new Dictionary<int, int> { { 2, 4 }, { 3, 6 } };
			Assert.AreEqual(4, DictionaryUtility.GetValueOrDefault(dict, 2));
			Assert.AreEqual(0, DictionaryUtility.GetValueOrDefault(dict, 1));
		}

		[Test]
		public void GetValueOrDefaultViaNetStandard()
		{
			Dictionary<int, int> dict = new Dictionary<int, int> { { 2, 4 }, { 3, 6 } };
			Assert.AreEqual(4, MyLib.MyClass.DoGetValueOrDefault(dict, 2));
			Assert.AreEqual(0, MyLib.MyClass.DoGetValueOrDefault(dict, 1));
		}

		[Test]
		public void GetValueOrDefaultWithDefault()
		{
			Dictionary<int, int> dict = new Dictionary<int, int> { { 2, 4 }, { 3, 6 } };
			Assert.AreEqual(4, DictionaryUtility.GetValueOrDefault(dict, 2, -1));
			Assert.AreEqual(-1, DictionaryUtility.GetValueOrDefault(dict, 1, -1));
		}

		[Test]
		public void GetValueOrDefaultWithFunc()
		{
			Dictionary<int, int> dict = new Dictionary<int, int> { { 2, 4 }, { 3, 6 } };
			Assert.AreEqual(4, DictionaryUtility.GetValueOrDefault(dict, 2, () => -2));
			Assert.AreEqual(-2, DictionaryUtility.GetValueOrDefault(dict, 1, () => -2));
		}

		[Test]
		public void TryAdd()
		{
			// use IDictionary<int, int> to force extension method
			IDictionary<int, int> dict = new Dictionary<int, int>();
			Assert.IsTrue(dict.TryAdd(1, 2));
			Assert.IsFalse(dict.TryAdd(1, 3));
			Assert.IsFalse(dict.TryAdd(1, 4));
			Assert.IsTrue(dict.TryAdd(2, 1));
		}
	}
}
