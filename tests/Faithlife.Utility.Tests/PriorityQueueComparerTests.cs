using System.Collections.Generic;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class PriorityQueueComparerTests
	{
		[SetUp]
		public void SetUp()
		{
			m_comparer = new StringLengthComparer();
		}

		[Test]
		public void EnqueueDequeueOrdered()
		{
			var pq = new PriorityQueue<string>(m_comparer);
			var startChar = "abcdefghijk".ToCharArray();
			for (var i = 1; i <= 100; ++i)
				pq.Enqueue(new string(startChar[i % 10], i));
			TestDequeueOrder(pq, 100);
		}

		[Test]
		public void EnqueueDequeueUnordered()
		{
			var pq = new PriorityQueue<string>(m_comparer);
			pq.Enqueue("a");
			pq.Enqueue("complete");
			pq.Enqueue("bunch");
			pq.Enqueue("of");
			pq.Enqueue("many");
			pq.Enqueue("and");
			pq.Enqueue("different");
			pq.Enqueue("length");
			pq.Enqueue("strings");
			TestDequeueOrder(pq, 9);
		}

		[Test]
		public void EnqueueDequeueReverse()
		{
			var pq = new PriorityQueue<string>(m_comparer);
			var startChar = "kjihgfedbca".ToCharArray();
			for (var i = 1; i <= 100; ++i)
				pq.Enqueue(new string(startChar[i % 10], 101 - i));
			TestDequeueOrder(pq, 100);
		}

		private static void TestDequeueOrder(PriorityQueue<string> pq, int nMaxItem)
		{
			// check that everything comes out in the correct order
			Assert.AreEqual(nMaxItem, pq.Count);
			for (var nItem = 1; nItem <= nMaxItem; ++nItem)
			{
				Assert.AreEqual(nItem, pq.Peek().Length);
				Assert.AreEqual(nItem, pq.Dequeue().Length);
				Assert.AreEqual(nMaxItem - nItem, pq.Count);
			}
		}
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
		private IComparer<string> m_comparer;
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
	}
}
