using System.Collections.Generic;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class MutablePriorityQueueTests
	{
		[SetUp]
		public void SetUp()
		{
			m_comparer = new IntHolderComparer();
		}

		[Test]
		public void EnqueueDequeueOrdered()
		{
			var pq = new PriorityQueue<IntHolder>(m_comparer);
			for (var i = 1; i <= 100; ++i)
				pq.Enqueue(new IntHolder(i));
			TestDequeueOrder(pq, 1, 100);
		}

		[Test]
		public void EnqueueDequeueChange()
		{
			var pq = new PriorityQueue<IntHolder>(m_comparer);
			for (var i = 100; i >= 1; --i)
				pq.Enqueue(new IntHolder(i));
			Assert.AreEqual(1, pq.Peek().Value);
			pq.RepositionHead();
			Assert.AreEqual(1, pq.Peek().Value);
			pq.Peek().Value = 102;
			Assert.AreEqual(102, pq.Peek().Value);
			pq.RepositionHead();
			Assert.AreEqual(2, pq.Peek().Value);
			pq.Peek().Value = 101;
			Assert.AreEqual(101, pq.Peek().Value);
			pq.RepositionHead();
			Assert.AreEqual(3, pq.Peek().Value);
			TestDequeueOrder(pq, 3, 102);
		}

		private static void TestDequeueOrder(PriorityQueue<IntHolder> pq, int nMinItem, int nMaxItem)
		{
			// check that everything comes out in the correct order
			Assert.AreEqual(nMaxItem - nMinItem + 1, pq.Count);
			for (var nItem = nMinItem; nItem <= nMaxItem; ++nItem)
			{
				Assert.AreEqual(nItem, pq.Peek().Value);
				Assert.AreEqual(nItem, pq.Dequeue().Value);
				Assert.AreEqual(nMaxItem - nItem, pq.Count);
			}
		}

		private class IntHolder
		{
			public IntHolder(int i)
			{
				Value = i;
			}

			public int Value { get; set; }
		}

		// compare objects by value
		private class IntHolderComparer : IComparer<IntHolder>
		{
			public int Compare(IntHolder? x, IntHolder? y)
			{
				return x!.Value.CompareTo(y!.Value);
			}
		}

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
		private IComparer<IntHolder> m_comparer;
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
	}
}
