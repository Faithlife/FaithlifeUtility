using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class ListUtilityTests
	{
		private delegate int SearchFunction(IList<SearchData> list, int key, Func<SearchData, int, int> fnCompare, out int nIndex);

		[Test]
		public void AsReadOnly()
		{
			var list = ((IList<string>) new[] { "hi", "ho" }).AsReadOnly();
			CollectionAssert.AreEqual(new[] { "hi", "ho" }, list);
			Assert.Throws<NotSupportedException>(() => { ((IList<string>) list)[0] = "hum"; });
		}

		[TestCase(0)]
		[TestCase(1)]
		public void MutateUnderlyingList(int initialCount)
		{
			List<int> original = new List<int>();
			for (var i = 0; i < initialCount; i++)
				original.Add(i);

			ReadOnlyCollection<int> wrapper = ListUtility.AsReadOnly(original);
			CollectionAssert.AreEqual(original, wrapper);

			original.Add(original.Count);
			CollectionAssert.AreEqual(original, wrapper);
		}

		[Test]
		public void BinarySearchNullList()
		{
			int nIndex;
			Assert.Throws<ArgumentNullException>(() => ListUtility.BinarySearchForKey<SearchData, int>(null!, 1, CompareItemToKey, out nIndex));
		}

		[Test]
		public void BinarySearchNullCompare()
		{
			int nIndex;
			List<SearchData> list = new List<SearchData>();
			Assert.Throws<ArgumentNullException>(() => ListUtility.BinarySearchForKey(list, 1, null!, out nIndex));
		}

		[Test]
		public void BinarySearchEmptyList()
		{
			// Can't use ListUtility.BinarySearchKey as the first parameter because of Mono bug #523683. Must use lambda expression.
			DoSearchEmptyList((IList<SearchData> list, int key, Func<SearchData, int, int> fnCompare, out int nIndex) => ListUtility.BinarySearchForKey(list.AsReadOnlyList(), key, fnCompare, out nIndex));
		}

		private static void DoSearchEmptyList(SearchFunction fnSearch)
		{
			int nIndex;
			List<SearchData> list = new List<SearchData>();
			var nCount = fnSearch(list, 10, CompareItemToKey, out nIndex);
			Assert.AreEqual(0, nCount);
			Assert.AreEqual(0, nIndex);
		}

		[TestCase(10, 1, 0)]
		[TestCase(13, 0, 1)]
		[TestCase(3, 0, 0)]
		public void BinarySearchOneItemList(int nKey, int nExpectedCount, int nExpectedIndex)
		{
			// Can't use ListUtility.BinarySearchKey as the first parameter because of Mono bug #523683. Must use lambda expression.
			DoSearchOneItemList((IList<SearchData> list, int key, Func<SearchData, int, int> fnCompare, out int nIndex) => ListUtility.BinarySearchForKey(list.AsReadOnlyList(), key, fnCompare, out nIndex), nKey, nExpectedCount, nExpectedIndex);
		}

		private static void DoSearchOneItemList(SearchFunction fnSearch, int nKey, int nExpectedCount, int nExpectedIndex)
		{
			int nIndex;
			List<SearchData> list = new List<SearchData>();
			list.Add(new SearchData(10, 2));
			var nCount = fnSearch(list, nKey, CompareItemToKey, out nIndex);
			Assert.AreEqual(nExpectedCount, nCount);
			Assert.AreEqual(nExpectedIndex, nIndex);
		}

		[Test]
		public void BinarySearchLargeListWithMultiple()
		{
			// Can't use ListUtility.BinarySearchKey as the first parameter because of Mono bug #523683. Must use lambda expression.
			DoSearchLargeListWithMultiple((IList<SearchData> list, int key, Func<SearchData, int, int> fnCompare, out int nIndex) => ListUtility.BinarySearchForKey(list.AsReadOnlyList(), key, fnCompare, out nIndex));
		}

		private static void DoSearchLargeListWithMultiple(SearchFunction fnSearch)
		{
			int nIndex;
			List<SearchData> list = new List<SearchData>();
			list.Add(new SearchData(3, 1));
			list.Add(new SearchData(6, 1));
			list.Add(new SearchData(10, 1));
			list.Add(new SearchData(10, 1));
			list.Add(new SearchData(10, 1));
			list.Add(new SearchData(10, 1));
			list.Add(new SearchData(14, 1));
			list.Add(new SearchData(17, 1));
			list.Add(new SearchData(21, 1));
			var nCount = fnSearch(list, 10, CompareItemToKey, out nIndex);
			Assert.AreEqual(4, nCount);
			Assert.AreEqual(2, nIndex);

			nCount = fnSearch(list, 7, CompareItemToKey, out nIndex);
			Assert.AreEqual(0, nCount);
			Assert.AreEqual(2, nIndex);

			nCount = fnSearch(list, 12, CompareItemToKey, out nIndex);
			Assert.AreEqual(0, nCount);
			Assert.AreEqual(6, nIndex);
		}

		[TestCase(0)]
		[TestCase(1)]
		[TestCase(2)]
		[TestCase(3)]
		[TestCase(4)]
		[TestCase(10)]
		[TestCase(100)]
		[TestCase(255)]
		[TestCase(256)]
		public void BinarySearchLargeList(int nItems)
		{
			// Can't use ListUtility.BinarySearchKey as the first parameter because of Mono bug #523683. Must use lambda expression.
			DoSearchLargeList((IList<SearchData> list, int key, Func<SearchData, int, int> fnCompare, out int nIndex) => ListUtility.BinarySearchForKey(list.AsReadOnlyList(), key, fnCompare, out nIndex), nItems);
		}

		private static void DoSearchLargeList(SearchFunction fnSearch, int nItems)
		{
			List<SearchData> list = new List<SearchData>();
			for (var nItem = 0; nItem < nItems; ++nItem)
			{
				list.Add(new SearchData(100 * nItem, 0));
			}

			for (var i = 0; i < list.Count; ++i)
			{
				int nIndex;
				var nKey = i * 100;
				var nCount = fnSearch(list, nKey, CompareItemToKey, out nIndex);
				Assert.AreEqual(1, nCount);
				Assert.AreEqual(i, nIndex);

				nCount = fnSearch(list, nKey - 20, CompareItemToKey, out nIndex);
				Assert.AreEqual(0, nCount);
				Assert.AreEqual(i, nIndex);

				nCount = fnSearch(list, nKey + 20, CompareItemToKey, out nIndex);
				Assert.AreEqual(0, nCount);
				Assert.AreEqual(i + 1, nIndex);
			}
		}

		[Test]
		public void RemoveWhere()
		{
			List<int> list = new List<int> { -2, 3, -5, 1, 5, -10 };
			list.RemoveWhere(x => x < 0);
			CollectionAssert.AreEqual(new[] { 3, 1, 5 }, list);
		}

		[Test]
		public void FindIndexBadArguments()
		{
			Assert.Throws<ArgumentNullException>(() => { ListUtility.FindIndex<object>(null!, n => true); });
			Assert.Throws<ArgumentNullException>(() => { ListUtility.FindIndex(new object[] { }, null!); });
		}

		[Test]
		public void FindIndex()
		{
			Assert.AreEqual(-1, ListUtility.FindIndex(new object[] { }, n => true));
			Assert.AreEqual(0, ListUtility.FindIndex(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, n => n < 5));
			Assert.AreEqual(5, ListUtility.FindIndex(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, n => n > 5));
			Assert.AreEqual(5, ListUtility.FindIndex(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 5, n => n > 5));
			Assert.AreEqual(6, ListUtility.FindIndex(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 6, n => n > 5));
			Assert.AreEqual(-1, ListUtility.FindIndex(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 6, n => n == 5));
			Assert.AreEqual(9, ListUtility.FindIndex(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, n => n == 10));
			Assert.AreEqual(-1, ListUtility.FindIndex(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, n => n == 11));
		}

		private static int CompareItemToKey(SearchData bsd, int nKey)
		{
			return bsd.nKey - nKey;
		}

		private struct SearchData
		{
			public SearchData(int k, int v)
			{
				nKey = k;
				nValue = v;
			}

			public readonly int nKey;
			public int nValue;
		}
	}
}
