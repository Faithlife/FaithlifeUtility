using System;
using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class PriorityQueueTests
	{
		[Test]
		public void Constructor2BadCapacity()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => new PriorityQueue<int>(-1));
		}

		[Test]
		public void Constructor4BadCapacity()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => new PriorityQueue<int>(-1, null));
		}

		[Test]
		public void NegativeCapacity()
		{
			PriorityQueue<int> pq = new PriorityQueue<int>();
			Assert.Throws<ArgumentOutOfRangeException>(() => pq.Capacity = -1);
		}

		[Test]
		public void SmallCapacity()
		{
			PriorityQueue<int> pq = new PriorityQueue<int>();
			pq.Enqueue(1);
			Assert.Greater(pq.Capacity, 0);
			Assert.Throws<ArgumentOutOfRangeException>(() => pq.Capacity = 0);
		}

		[Test]
		public void Capacity()
		{
			PriorityQueue<int> pq = new PriorityQueue<int>(3);
			Assert.AreEqual(3, pq.Capacity);
			Assert.AreEqual(0, pq.Count);
			pq.Enqueue(1);
			Assert.AreEqual(3, pq.Capacity);
			Assert.AreEqual(1, pq.Count);
			pq.Capacity = 2;
			Assert.AreEqual(2, pq.Capacity);
			Assert.AreEqual(1, pq.Count);
			pq.Capacity = 1;
			Assert.AreEqual(1, pq.Capacity);
			Assert.AreEqual(1, pq.Count);

			pq = new PriorityQueue<int>();
			Assert.AreEqual(0, pq.Capacity);
			pq.Capacity = 10;
			Assert.AreEqual(10, pq.Capacity);
			pq.Capacity = 0;
			Assert.AreEqual(0, pq.Capacity);
		}

		[Test]
		public void EnqueueDequeueOrdered()
		{
			// add numbers in order
			PriorityQueue<int> pq = new PriorityQueue<int>();
			for (int i = 1; i <= 100; ++i)
				pq.Enqueue(i);

			TestDequeueOrder(pq, 100);
		}

		[Test]
		public void EnqueueDequeueReverseOrder()
		{
			TestDequeueOrder(CreateReversed(100), 100);
		}

		[Test]
		public void EnqueueDequeueUnordered()
		{
			PriorityQueue<int> pq = new PriorityQueue<int>();

			// add numbers out of order
			pq.Enqueue(9);
			pq.Enqueue(3);
			pq.Enqueue(7);
			pq.Enqueue(4);
			pq.Enqueue(1);
			pq.Enqueue(5);
			pq.Enqueue(8);
			pq.Enqueue(6);
			pq.Enqueue(2);
			pq.Enqueue(10);

			TestDequeueOrder(pq, 10);
		}

		[Test]
		public void DequeueEmpty()
		{
			PriorityQueue<int> pq = new PriorityQueue<int>();
			Assert.Throws<InvalidOperationException>(() => pq.Dequeue());
		}

		[Test]
		public void PeekEmpty()
		{
			PriorityQueue<int> pq = new PriorityQueue<int>();
			Assert.Throws<InvalidOperationException>(() => pq.Peek());
		}

		[Test]
		public void RepositionHeadEmpty()
		{
			PriorityQueue<int> pq = new PriorityQueue<int>();
			Assert.Throws<InvalidOperationException>(() => pq.RepositionHead());
		}

		[Test]
		public void Enumerate()
		{
			PriorityQueue<int> pq = CreateReversed(100);

			// enumerate through items (default)
			int nExpected = 1;
			foreach (int item in pq)
				Assert.AreEqual(nExpected++, item);
			Assert.AreEqual(101, nExpected);

			// enumerate through items (IEnumerable<T>)
			nExpected = 1;
			IEnumerable<int> ie = pq;
			foreach (int item in ie)
				Assert.AreEqual(nExpected++, item);
			Assert.AreEqual(101, nExpected);

			// enumerate through items (IEnumerable)
			nExpected = 1;
#pragma warning disable CS8605 // Unboxing a possibly null value.
			foreach (int item in (IEnumerable) pq)
#pragma warning restore CS8605 // Unboxing a possibly null value.
				Assert.AreEqual(nExpected++, item);
			Assert.AreEqual(101, nExpected);

			// check that original queue is unmodified
			Assert.AreEqual(100, pq.Count);
			Assert.AreEqual(1, pq.Peek());
		}

		[Test]
		public void Collection()
		{
			ICollection c = new PriorityQueue<int>();
			Assert.IsFalse(c.IsSynchronized);

			object sr1 = c.SyncRoot;
			object sr2 = c.SyncRoot;
			Assert.AreEqual(sr1, sr2);

			Assert.AreEqual(0, c.Count);
		}

		[Test]
		public void CopyTo()
		{
			PriorityQueue<int> pq = CreateReversed(25);
			int[] aExpected = new int[25];
			for (int i = 0; i < aExpected.Length; ++i)
				aExpected[i] = i + 1;

			int[] aOutput1 = new int[25];
			pq.CopyTo(aOutput1, 0);
			CollectionAssert.AreEqual(aExpected, aOutput1);

			ICollection c = pq;
			Array aOutput2 = new int[25];
			c.CopyTo(aOutput2, 0);
			CollectionAssert.AreEqual(aExpected, (int[]) aOutput2);

			Array aOutput3 = new string[25];
			Assert.Throws<ArgumentException>(() => c.CopyTo(aOutput3, 0));
		}

		[Test]
		public void Remove()
		{
			PriorityQueue<int> queue = CreateReversed(10);
			Assert.AreEqual(1, queue.Peek());
			queue.Remove(1);
			Assert.AreEqual(2, queue.Peek());
			queue.Remove(3);
			Assert.AreEqual(2, queue.Dequeue());
			Assert.AreEqual(4, queue.Dequeue());
			queue.Remove(10);
			queue.Remove(11);
			queue.Remove(9);
			Assert.AreEqual(5, queue.Dequeue());
			Assert.AreEqual(6, queue.Dequeue());
			Assert.AreEqual(7, queue.Dequeue());
			Assert.AreEqual(8, queue.Dequeue());
			Assert.AreEqual(0, queue.Count);
		}

		[Test]
		public void RemoveAll()
		{
			PriorityQueue<int> queue = CreateReversed(5);
			Assert.AreEqual(5, queue.Count);
			Assert.AreEqual(1, queue.Peek());
			queue.Remove(1);
			Assert.AreEqual(2, queue.Peek());
			Assert.AreEqual(4, queue.Count);

			queue.Remove(5);
			Assert.AreEqual(3, queue.Count);

			queue.Remove(4);
			Assert.AreEqual(2, queue.Count);

			queue.Remove(3);
			Assert.AreEqual(1, queue.Count);

			queue.Remove(2);
			Assert.AreEqual(0, queue.Count);
		}

		private static PriorityQueue<int> CreateReversed(int nCount)
		{
			// add numbers in reverse order
			PriorityQueue<int> pq = new PriorityQueue<int>(null);
			for (int i = 0; i < nCount; ++i)
				pq.Enqueue(nCount - i);
			return pq;
		}

		private static void TestDequeueOrder(PriorityQueue<int> pq, int nMaxItem)
		{
			// check that everything comes out in the correct order
			Assert.AreEqual(nMaxItem, pq.Count);
			for (int nItem = 1; nItem <= nMaxItem; ++nItem)
			{
				Assert.AreEqual(nItem, pq.Peek());
				Assert.AreEqual(nItem, pq.Dequeue());
				Assert.AreEqual(nMaxItem - nItem, pq.Count);
			}
		}
	}

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
			PriorityQueue<string> pq = new PriorityQueue<string>(m_comparer);
			char[] startChar = "abcdefghijk".ToCharArray();
			for (int i = 1; i <= 100; ++i)
				pq.Enqueue(new string(startChar[i % 10], i));
			TestDequeueOrder(pq, 100);
		}

		[Test]
		public void EnqueueDequeueUnordered()
		{
			PriorityQueue<string> pq = new PriorityQueue<string>(m_comparer);
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
			PriorityQueue<string> pq = new PriorityQueue<string>(m_comparer);
			char[] startChar = "kjihgfedbca".ToCharArray();
			for (int i = 1; i <= 100; ++i)
				pq.Enqueue(new string(startChar[i % 10], 101 - i));
			TestDequeueOrder(pq, 100);
		}

		private static void TestDequeueOrder(PriorityQueue<string> pq, int nMaxItem)
		{
			// check that everything comes out in the correct order
			Assert.AreEqual(nMaxItem, pq.Count);
			for (int nItem = 1; nItem <= nMaxItem; ++nItem)
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

	// compare strings by length
	class StringLengthComparer : IComparer<string>
	{
		public int Compare(string x, string y)
		{
			return x.Length.CompareTo(y.Length);
		}
	}

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
			PriorityQueue<IntHolder> pq = new PriorityQueue<IntHolder>(m_comparer);
			for (int i = 1; i <= 100; ++i)
				pq.Enqueue(new IntHolder(i));
			TestDequeueOrder(pq, 1, 100);
		}

		[Test]
		public void EnqueueDequeueChange()
		{
			PriorityQueue<IntHolder> pq = new PriorityQueue<IntHolder>(m_comparer);
			for (int i = 100; i >= 1; --i)
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
			for (int nItem = nMinItem; nItem <= nMaxItem; ++nItem)
			{
				Assert.AreEqual(nItem, pq.Peek().Value);
				Assert.AreEqual(nItem, pq.Dequeue().Value);
				Assert.AreEqual(nMaxItem - nItem, pq.Count);
			}
		}

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
		private IComparer<IntHolder> m_comparer;
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
	}

	class IntHolder
	{
		public IntHolder(int i)
		{
			Value = i;
		}

		public int Value;
	}

	// compare objects by value
	class IntHolderComparer : IComparer<IntHolder>
	{
		public int Compare(IntHolder x, IntHolder y)
		{
			return x.Value.CompareTo(y.Value);
		}
	}
}
