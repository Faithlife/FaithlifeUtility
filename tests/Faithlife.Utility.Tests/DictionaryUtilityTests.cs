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
		public void DictionariesAreNotEqualWhenUsingDifferentKeyComparer()
		{
			Dictionary<string, int> dict1 = new Dictionary<string, int>(StringComparer.Ordinal) { { "one", 1 }, { "two", 2 }, { "three", 3 } };
			Dictionary<string, int> dict2 = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { { "ONE", 1 }, { "Two", 2 }, { "THREE", 3 } };

			Assert.IsFalse(DictionaryUtility.AreEqual(dict1, dict2));
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
		public void MergeWithWhenThisIsEmpty()
		{
			Dictionary<int, int> thisDictionary = new Dictionary<int, int>();
			Dictionary<int, int> otherDictionary = new Dictionary<int, int> { { 1, 1 }, { 2, 2 } };
			thisDictionary.MergeWith(otherDictionary, MergeWithStrategy.ThrowException);
			Assert.AreEqual(2, thisDictionary.Count);
			Assert.AreEqual(1, thisDictionary[1]);
			Assert.AreEqual(2, thisDictionary[2]);
		}

		[Test]
		public void MergeWithWhenOtherIsEmpty()
		{
			Dictionary<int, int> thisDictionary = new Dictionary<int, int> { { 1, 1 }, { 2, 2 } };
			Dictionary<int, int> otherDictionary = new Dictionary<int, int>();
			thisDictionary.MergeWith(otherDictionary, MergeWithStrategy.ThrowException);
			Assert.AreEqual(2, thisDictionary.Count);
			Assert.AreEqual(1, thisDictionary[1]);
			Assert.AreEqual(2, thisDictionary[2]);
		}

		[Test]
		public void MergeWithStrategyKeepOriginalValue()
		{
			Dictionary<int, int> thisDictionary = new Dictionary<int, int> { { 1, 1 }, { 2, 2 } };
			Dictionary<int, int> otherDictionary = new Dictionary<int, int> { { 2, -2 }, { 3, -3 } };
			thisDictionary.MergeWith(otherDictionary, MergeWithStrategy.KeepOriginalValue);
			Assert.AreEqual(3, thisDictionary.Count);
			Assert.AreEqual(1, thisDictionary[1]);
			Assert.AreEqual(2, thisDictionary[2]);
			Assert.AreEqual(-3, thisDictionary[3]);
		}

		[Test]
		public void MergeWithStrategyOverwriteValue()
		{
			Dictionary<int, int> thisDictionary = new Dictionary<int, int> { { 1, 1 }, { 2, 2 } };
			Dictionary<int, int> otherDictionary = new Dictionary<int, int> { { 2, -2 }, { 3, -3 } };
			thisDictionary.MergeWith(otherDictionary, MergeWithStrategy.OverwriteValue);
			Assert.AreEqual(3, thisDictionary.Count);
			Assert.AreEqual(1, thisDictionary[1]);
			Assert.AreEqual(-2, thisDictionary[2]);
			Assert.AreEqual(-3, thisDictionary[3]);
		}

		[Test]
		public void MergeWithStrategyThrowException()
		{
			Dictionary<int, int> thisDictionary = new Dictionary<int, int> { { 1, 1 }, { 2, 2 } };
			Dictionary<int, int> otherDictionary = new Dictionary<int, int> { { 2, -2 }, { 3, -3 } };
			Assert.Throws<InvalidOperationException>(() => thisDictionary.MergeWith(otherDictionary, MergeWithStrategy.ThrowException));
		}

		[Test]
		public void TryAdd()
		{
			Dictionary<int, int> dict = new Dictionary<int, int>();
			Assert.IsTrue(dict.TryAdd(1, 2));
			Assert.IsFalse(dict.TryAdd(1, 3));
			Assert.IsFalse(dict.TryAdd(1, 4));
			Assert.IsTrue(dict.TryAdd(2, 1));
		}

		[Test]
		public void TryGetValueWithConversion()
		{
			Dictionary<int, int> dict = new Dictionary<int, int> { { 2, 4 }, { 3, 6 } };

			int nResult;
			bool bExisted = DictionaryUtility.TryGetValueWithConversion(dict, 2, out nResult, n => n + 1);
			Assert.IsTrue(bExisted);
			Assert.AreEqual(5, nResult);

			bExisted = DictionaryUtility.TryGetValueWithConversion(dict, 1, out nResult, n => n + 1);
			Assert.IsFalse(bExisted);
			Assert.AreEqual(0, nResult);
		}

		[Test]
		public void ToDictionaries()
		{
			var kvps = new[] { DictionaryUtility.CreateKeyValuePair(2, "two"), DictionaryUtility.CreateKeyValuePair(1, "one"), DictionaryUtility.CreateKeyValuePair(3, "three") };
			Assert.AreEqual("one", kvps.ToDictionary()[1]);
			Assert.AreEqual("one", kvps.ToSortedDictionary()[1]);
			Assert.AreEqual("one", kvps.ToSortedList()[1]);
		}
	}
}
