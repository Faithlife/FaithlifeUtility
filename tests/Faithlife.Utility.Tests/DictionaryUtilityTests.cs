using System.Collections.Generic;
using System.Collections.ObjectModel;
using MyLib;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class DictionaryUtilityTests
	{
		[Test]
		public void DictionariesAreEqual()
		{
			var dict1 = new Dictionary<int, int> { { 1, 1 }, { 2, 2 }, { 3, 3 } };
			var dict2 = new Dictionary<int, int> { { 1, 1 }, { 2, 2 }, { 3, 3 } };

			Assert.IsTrue(DictionaryUtility.AreEqual(dict1, dict2));
		}

		[Test]
		public void DictionariesAreEqualWhenItemsAreAddedOutOfOrder()
		{
			var dict1 = new Dictionary<int, int> { { 1, 1 }, { 2, 2 }, { 3, 3 } };
			var dict2 = new Dictionary<int, int> { { 1, 1 }, { 3, 3 }, { 2, 2 } };

			Assert.IsTrue(DictionaryUtility.AreEqual(dict1, dict2));
		}

		[Test]
		public void DictionariesArNotEqualWhenValueIsdifferent()
		{
			var dict1 = new Dictionary<int, int> { { 1, 1 }, { 2, 2 }, { 3, 3 } };
			var dict2 = new Dictionary<int, int> { { 1, 2 }, { 2, 2 }, { 3, 3 } };

			Assert.IsFalse(DictionaryUtility.AreEqual(dict1, dict2));
		}

		[Test]
		public void DictionariesAreNotEqualWhenSecondIsMissingItem()
		{
			var dict1 = new Dictionary<int, int> { { 1, 1 }, { 2, 2 }, { 3, 3 } };
			var dict2 = new Dictionary<int, int> { { 1, 1 }, { 2, 2 } };

			Assert.IsFalse(DictionaryUtility.AreEqual(dict1, dict2));
		}

		[Test]
		public void DictionariesAreNotEqualWhenFirstIsMissingItem()
		{
			var dict1 = new Dictionary<int, int> { { 1, 1 }, { 2, 2 } };
			var dict2 = new Dictionary<int, int> { { 1, 1 }, { 2, 2 }, { 3, 3 } };

			Assert.IsFalse(DictionaryUtility.AreEqual(dict1, dict2));
		}

		[Test]
		public void DictionariesAreEqualWhenBothAreEmpty()
		{
			var dict1 = new Dictionary<int, int>();
			var dict2 = new Dictionary<int, int>();

			Assert.IsTrue(DictionaryUtility.AreEqual(dict1, dict2));
		}

		[Test]
		public void DictionariesAreEqualWhenSameInstance()
		{
			var dict1 = new Dictionary<int, int> { { 1, 1 }, { 2, 2 }, { 3, 3 } };

			Assert.IsTrue(DictionaryUtility.AreEqual(dict1, dict1));
		}

		[Test]
		public void DictionariesAreEqualWhenUsingCustomComparer()
		{
			var dict1 = new Dictionary<int, int> { { 1, 1 }, { 2, 2 }, { 3, 3 } };
			var dict2 = new Dictionary<int, int> { { 1, 1 }, { 2, 2 }, { 3, 3 } };

			Assert.IsTrue(DictionaryUtility.AreEqual(dict1, dict2, new GenericEqualityComparer<int>((x, y) => x == y)));
		}

		[Test]
		public void DictionariesAreEqualWhenUsingCustomKeyComparer()
		{
			var comparer = new GenericEqualityComparer<int>((x, y) => x == y, HashCodeUtility.GetPersistentHashCode);
			var dict1 = new Dictionary<int, int>(comparer) { { 1, 1 }, { 2, 2 }, { 3, 3 } };
			var dict2 = new Dictionary<int, int>(comparer) { { 1, 1 }, { 2, 2 }, { 3, 3 } };

			Assert.IsTrue(DictionaryUtility.AreEqual(dict1, dict2));
		}

		[Test]
		public void ReadOnlyDictionariesAreEqual()
		{
			var dict1 = new Dictionary<int, int> { { 1, 1 }, { 2, 2 }, { 3, 3 } }.AsReadOnly();
			var dict2 = new Dictionary<int, int> { { 1, 1 }, { 2, 2 }, { 3, 3 } }.AsReadOnly();

			Assert.IsTrue(DictionaryUtility.AreEqual(dict1, dict2));
		}

		[Test]
		public void ReadOnlyDictionariesAreNotEqualWhenValueIsDifferent()
		{
			var dict1 = new Dictionary<int, int> { { 1, 1 }, { 2, 2 }, { 3, 3 } }.AsReadOnly();
			var dict2 = new Dictionary<int, int> { { 1, 2 }, { 2, 2 }, { 3, 3 } }.AsReadOnly();

			Assert.IsFalse(DictionaryUtility.AreEqual(dict1, dict2));
		}

		[Test]
		public void AsReadOnly()
		{
			var dict = new Dictionary<string, int>
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
			var dict = new Dictionary<int, List<int>>();
			Assert.Throws<KeyNotFoundException>(() =>
			{
				var value = dict[1];
			});
		}

		[Test]
		public void GetOrAddItemAdd()
		{
			var dict = new Dictionary<int, List<int>>();
			Assert.AreEqual(0, DictionaryUtility.GetOrAddValue(dict, 1).Count);
			Assert.AreEqual(0, dict[1].Count);
		}

		[Test]
		public void GetOrAddItemExists()
		{
			var dict = new Dictionary<int, List<int>>();
			var list = new List<int>();
			dict[2] = list;
			Assert.AreSame(list, DictionaryUtility.GetOrAddValue(dict, 2));
			Assert.AreSame(list, dict[2]);
		}

		[Test]
		public void GetOrAddItemAddValue()
		{
			var dict = new Dictionary<int, int>();
			Assert.AreEqual(0, DictionaryUtility.GetOrAddValue(dict, 0));
			Assert.AreEqual(0, dict[0]);
		}

		[Test]
		public void GetOrAddItemAddCustom()
		{
			var dict = new Dictionary<int, int>();
			Assert.AreEqual(3, DictionaryUtility.GetOrAddValue(dict, 2, () => 3));
			Assert.AreEqual(3, dict[2]);
		}

		[Test]
		public void GetOrAddItemExistingCustom()
		{
			var dict = new Dictionary<int, int>();
			dict[2] = 4;
			Assert.AreEqual(4, DictionaryUtility.GetOrAddValue(dict, 2, () => 3));
			Assert.AreEqual(4, dict[2]);
		}

		[Test]
		public void GetValueOrDefault()
		{
			var dict = new Dictionary<int, int> { { 2, 4 }, { 3, 6 } };
#pragma warning disable CS0618 // Type or member is obsolete
			Assert.AreEqual(4, DictionaryUtility.GetValueOrDefault(dict, 2));
			Assert.AreEqual(0, DictionaryUtility.GetValueOrDefault(dict, 1));
#pragma warning restore CS0618 // Type or member is obsolete
		}

		[Test]
		public void GetValueOrDefaultViaNetStandard()
		{
			var dict = new Dictionary<int, int> { { 2, 4 }, { 3, 6 } };
			Assert.AreEqual(4, MyClass.DoGetValueOrDefault(dict, 2));
			Assert.AreEqual(0, MyClass.DoGetValueOrDefault(dict, 1));
		}

		[Test]
		public void GetValueOrDefaultWithDefault()
		{
			var dict = new Dictionary<int, int> { { 2, 4 }, { 3, 6 } };
#pragma warning disable CS0618 // Type or member is obsolete
			Assert.AreEqual(4, DictionaryUtility.GetValueOrDefault(dict, 2, -1));
			Assert.AreEqual(-1, DictionaryUtility.GetValueOrDefault(dict, 1, -1));
#pragma warning restore CS0618 // Type or member is obsolete
		}

		[Test]
		public void GetValueOrDefaultWithFunc()
		{
			var dict = new Dictionary<int, int> { { 2, 4 }, { 3, 6 } };
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
			Assert.AreEqual(new Dictionary<int, int> { { 1, 2 }, { 2, 1 } }, dict);
		}

		[Test]
		public void TryAddViaNetStandard()
		{
			// use IDictionary<int, int> to force extension method
			IDictionary<int, int> dict = new Dictionary<int, int>();
			Assert.IsTrue(MyClass.DoTryAdd(dict, 1, 2));
			Assert.IsFalse(MyClass.DoTryAdd(dict, 1, 3));
			Assert.IsFalse(MyClass.DoTryAdd(dict, 1, 4));
			Assert.IsTrue(MyClass.DoTryAdd(dict, 2, 1));
			Assert.AreEqual(new Dictionary<int, int> { { 1, 2 }, { 2, 1 } }, dict);
		}
	}
}
