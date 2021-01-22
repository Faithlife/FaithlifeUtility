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
			var pq = new PriorityQueue<int>();
			Assert.Throws<ArgumentOutOfRangeException>(() => pq.Capacity = -1);
		}

		[Test]
		public void SmallCapacity()
		{
			var pq = new PriorityQueue<int>();
			pq.Enqueue(1);
			Assert.Greater(pq.Capacity, 0);
			Assert.Throws<ArgumentOutOfRangeException>(() => pq.Capacity = 0);
		}

		[Test]
		public void Capacity()
		{
			var pq = new PriorityQueue<int>(3);
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
			var pq = new PriorityQueue<int>();
			for (var i = 1; i <= 100; ++i)
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
			var pq = new PriorityQueue<int>();

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
			var pq = new PriorityQueue<int>();
			Assert.Throws<InvalidOperationException>(() => pq.Dequeue());
		}

		[Test]
		public void PeekEmpty()
		{
			var pq = new PriorityQueue<int>();
			Assert.Throws<InvalidOperationException>(() => pq.Peek());
		}

		[Test]
		public void RepositionHeadEmpty()
		{
			var pq = new PriorityQueue<int>();
			Assert.Throws<InvalidOperationException>(() => pq.RepositionHead());
		}

		[Test]
		public void Enumerate()
		{
			var pq = CreateReversed(100);

			// enumerate through items (default)
			var nExpected = 1;
			foreach (var item in pq)
				Assert.AreEqual(nExpected++, item);
			Assert.AreEqual(101, nExpected);

			// enumerate through items (IEnumerable<T>)
			nExpected = 1;
			IEnumerable<int> ie = pq;
			foreach (var item in ie)
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

			var sr1 = c.SyncRoot;
			var sr2 = c.SyncRoot;
			Assert.AreEqual(sr1, sr2);

			Assert.AreEqual(0, c.Count);
		}

		[Test]
		public void CopyTo()
		{
			var pq = CreateReversed(25);
			var aExpected = new int[25];
			for (var i = 0; i < aExpected.Length; ++i)
				aExpected[i] = i + 1;

			var aOutput1 = new int[25];
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
			var queue = CreateReversed(10);
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
			var queue = CreateReversed(5);
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
			var pq = new PriorityQueue<int>(null);
			for (var i = 0; i < nCount; ++i)
				pq.Enqueue(nCount - i);
			return pq;
		}

		private static void TestDequeueOrder(PriorityQueue<int> pq, int nMaxItem)
		{
			// check that everything comes out in the correct order
			Assert.AreEqual(nMaxItem, pq.Count);
			for (var nItem = 1; nItem <= nMaxItem; ++nItem)
			{
				Assert.AreEqual(nItem, pq.Peek());
				Assert.AreEqual(nItem, pq.Dequeue());
				Assert.AreEqual(nMaxItem - nItem, pq.Count);
			}
		}
	}
}
