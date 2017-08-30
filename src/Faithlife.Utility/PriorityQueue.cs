using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Faithlife.Utility
{
	/// <summary>
	/// Implements a <a href="http://en.wikipedia.org/wiki/Priority_queue">priority queue</a> using a <a href="http://en.wikipedia.org/wiki/Binary_heap">binary heap</a>.
	/// The priority queue is sorted so that the smallest item is removed from the queue first.
	/// </summary>
	/// <typeparam name="T">The type of data to be sorted in the priority queue.</typeparam>
	public sealed class PriorityQueue<T> : ICollection, IEnumerable<T>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PriorityQueue{T}"/> class that is empty, has the default initial capacity,
		/// and is sorted according to the default <see cref="Comparer{T}"/> for the type of the data. 
		/// </summary>
		public PriorityQueue() : this(0, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PriorityQueue{T}"/> class that is empty, has the specified initial capacity,
		/// and is sorted according to the default <see cref="Comparer{T}"/> for the type of the data.
		/// </summary>
		/// <param name="nCapacity">The initial capacity.</param>
		public PriorityQueue(int nCapacity) : this(nCapacity, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PriorityQueue{T}"/> class that is empty, has the default initial capacity,
		/// and is sorted according to the specified <see cref="IComparer{T}"/>. 
		/// </summary>
		/// <param name="comparer">The <see cref="IComparer{T}"/> implementation to use when comparing items.<br/>
		/// -or-<br/>
		/// a null reference to use the default <see cref="Comparer{T}"/> for the type of the data.</param>
		public PriorityQueue(IComparer<T> comparer) : this(0, comparer)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PriorityQueue{T}"/> class that is empty, has the specified initial capacity,
		/// and is sorted according to the specified <see cref="IComparer{T}"/>.
		/// </summary>
		/// <param name="nCapacity">The initial capacity.</param>
		/// <param name="comparer">The <see cref="IComparer{T}"/> implementation to use when comparing items.<br/>
		/// -or-<br/>
		/// a null reference to use the default <see cref="Comparer{T}"/> for the type of the data.</param>
		public PriorityQueue(int nCapacity, IComparer<T> comparer)
		{
			if (nCapacity < 0)
				throw new ArgumentOutOfRangeException("nCapacity", OurMessages.ArgumentOutOfRange_MustBeNonNegative);

			m_comparer = comparer ?? Comparer<T>.Default;
			m_aData = s_aEmpty;
			if (nCapacity > 0)
				SetCapacity(nCapacity);
		}

		/// <summary>
		/// Gets or sets the number of elements that the <see cref="PriorityQueue{T}"/> can contain.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Capacity is set to a value that is less than <see cref="Count"/>.</exception>
		public int Capacity
		{
			get
			{
				// return size of the backing array
				return m_aData.Length;
			}
			set
			{
				// check argument
				if (value < m_nSize)
					throw new ArgumentOutOfRangeException("value", OurMessages.ArgumentOutOfRange_SmallCapacity);

				// set the capacity
				SetCapacity(value);
			}
		}

		/// <summary>
		/// Removes and returns the smallest item from the <see cref="PriorityQueue{T}"/>. This operation takes O(log n) time.
		/// </summary>
		/// <returns>The smallest item in the <see cref="PriorityQueue{T}"/>.</returns>
		/// <exception cref="InvalidOperationException">The priority queue is empty.</exception>
		public T Dequeue()
		{
			// check for empty queue
			if (m_nSize == 0)
				throw new InvalidOperationException(OurMessages.InvalidOperation_EmptyQueue);

			// remove the first item and return it
			T item = m_aData[0];
			RemoveAt(0);
			return item;
		}

		/// <summary>
		/// Returns (without removing) the smallest item in the <see cref="PriorityQueue{T}"/>. This operation takes O(1) time.
		/// </summary>
		/// <returns>The smallest item in the <see cref="PriorityQueue{T}"/>.</returns>
		/// <exception cref="InvalidOperationException">The priority queue is empty.</exception>
		public T Peek()
		{
			// check for empty queue
			if (m_nSize == 0)
				throw new InvalidOperationException(OurMessages.InvalidOperation_EmptyQueue);

			// return the item at the top of the heap
			return m_aData[0];
		}

		/// <summary>
		/// Adds an object to the <see cref="PriorityQueue{T}"/>. This operation takes O(log n) time.
		/// </summary>
		public void Enqueue(T item)
		{
			// make sure the array is big enough
			EnsureCapacity(m_nSize + 1);

			// add the new item to the bottom of the heap, then move it up to the correct location
			m_aData[m_nSize] = item;
			SiftUpHeap(m_nSize);
			++m_nSize;
		}

		/// <summary>
		/// Removes the first item equal to <paramref name="item"/> in the priority queue.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		public void Remove(T item)
		{
			for (int nIndex = 0; nIndex < m_nSize; nIndex++)
			{
				if (m_comparer.Compare(m_aData[nIndex], item) == 0)
				{
					RemoveAt(nIndex);
					break;
				}
			}
		}

		/// <summary>
		/// Copies the elements of the <see cref="PriorityQueue{T}"/> to an existing one-dimensional <see cref="Array"/>, starting at a particular array index.
		/// </summary>
		/// <param name="array">The one-dimensional <see cref="Array"></see> that is the destination of the elements
		/// copied from the <see cref="PriorityQueue{T}"/>. The <see cref="Array"></see> must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
		public void CopyTo(T[] array, int index)
		{
			// check parameters
			CollectionImpl.CheckCopyToParameters(array, index, m_nSize);

			// copy each item to the destination array
			foreach (T item in this)
				array[index++] = item;
		}

		/// <summary>
		/// Puts the item at the head of the queue into the correct place. If you modify the item returned by <see cref="Peek"/> such that its
		/// "priority" changes, call this method to restore the correct order in the <see cref="PriorityQueue{T}"/>.
		/// </summary>
		public void RepositionHead()
		{
			// check for empty queue
			if (m_nSize == 0)
				throw new InvalidOperationException(OurMessages.InvalidOperation_EmptyQueue);

			// restore the heap order
			SiftDownHeap(0);
		}

		/// <summary>
		/// Swaps the item at the head of the queue with a new item. The new item will be placed into the correct location in the priority queue.
		/// </summary>
		/// <remarks>This method is more efficient than calling <see cref="Dequeue"/> then <see cref="Enqueue"/>.</remarks>
		/// <param name="item">The new item.</param>
		public void SwapHead(T item)
		{
			// check for empty queue
			if (m_nSize == 0)
				throw new InvalidOperationException(OurMessages.InvalidOperation_EmptyQueue);

			// replace the head of the queue with the new item
			m_aData[0] = item;

			// restore the heap order
			SiftDownHeap(0);
		}

		/// <summary>
		/// Copies the elements of the <see cref="PriorityQueue{T}"/> to an existing one-dimensional <see cref="Array"/>, starting at a particular array index.
		/// </summary>
		/// <param name="array">The one-dimensional <see cref="Array"></see> that is the destination of the elements
		/// copied from the <see cref="PriorityQueue{T}"/>. The <see cref="Array"></see> must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
		void ICollection.CopyTo(Array array, int index)
		{
			// check parameters
			CollectionImpl.CheckCopyToParameters(array, index, m_nSize);

			try
			{
				// copy each item to the destination array
				foreach (T item in this)
					array.SetValue(item, index++);
			}
			catch (InvalidCastException)
			{
				// convert exception thrown by Array.SetValue to the type that ICollection.CopyTo is expected to throw.
				throw new ArgumentException(OurMessages.Argument_CannotCastToArray, "array");
			}
		}

		/// <summary>
		/// Gets the number of elements contained in the <see cref="PriorityQueue{T}"/>.
		/// </summary>
		/// <returns>The number of elements contained in the <see cref="PriorityQueue{T}"/>.</returns>
		public int Count
		{
			get { return m_nSize; }
		}

		/// <summary>
		/// Gets an object that can be used to synchronize access to the <see cref="PriorityQueue{T}"/>.
		/// </summary>
		/// <returns>An object that can be used to synchronize access to the <see cref="PriorityQueue{T}"/>.</returns>
		object ICollection.SyncRoot
		{
			get
			{
				if (m_syncRoot == null)
					Interlocked.CompareExchange(ref m_syncRoot, new object(), null);

				return m_syncRoot;
			}
		}

		/// <summary>
		/// Gets a value indicating whether access to the <see cref="PriorityQueue{T}"/> is synchronized (thread safe).
		/// </summary>
		/// <returns><c>true</c> if access to the <see cref="PriorityQueue{T}"/> is synchronized (thread safe); otherwise, false.</returns>
		bool ICollection.IsSynchronized
		{
			get { return false; }
		}

		/// <summary>
		/// Returns an enumerator that iterates through the items in the <see cref="PriorityQueue{T}"/>, in order. This operation
		/// is potentially very expensive; clients should prefer to repeatedly call <see cref="Dequeue"/>.
		/// </summary>
		/// <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the collection.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<T>) this).GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through the items in the <see cref="PriorityQueue{T}"/>, in order. This operation
		/// is potentially very expensive; clients should prefer to repeatedly call <see cref="Dequeue"/>.
		/// </summary>
		/// <returns>A <see cref="IEnumerator{T}"/> that can be used to iterate through the collection.</returns>
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			// make a copy of the current data
			PriorityQueue<T> pqCopy = new PriorityQueue<T>(this);

			// return each item, in order
			while (pqCopy.Count > 0)
				yield return pqCopy.Dequeue();
		}

		private PriorityQueue(PriorityQueue<T> pq)
		{
			// make a copy of the other PriorityQueue
			m_comparer = pq.m_comparer;
			m_nSize = pq.m_nSize;
			m_aData = new T[m_nSize];
			if (m_nSize > 0)
				Array.Copy(pq.m_aData, m_aData, m_nSize);
		}

		private void EnsureCapacity(int nDesiredSize)
		{
			// check if the array is big enough
			if (nDesiredSize > m_aData.Length)
			{
				// not big enough; resize (to either two times the current size or the current size plus four, whichever is larger)
				int nNewCapacity = Math.Max(m_aData.Length * 2, m_aData.Length + 4);
				SetCapacity(nNewCapacity);
			}
		}

		// Sets the capacity of the priority queue to the specified amount.
		private void SetCapacity(int nCapacity)
		{
			if (nCapacity > 0)
			{
				// allocate new array and copy any values across to it
				T[] aNewData = new T[nCapacity];
				if (m_nSize > 0)
					Array.Copy(m_aData, aNewData, m_nSize);
				m_aData = aNewData;
			}
			else
			{
				// use empty array
				m_aData = s_aEmpty;
			}
		}

		// Removes the item at index 'nIndex' from the queue.
		private void RemoveAt(int nIndex)
		{
			// replace the item with the last item
			m_nSize--;
			m_aData[nIndex] = m_aData[m_nSize];
			m_aData[m_nSize] = default(T);

			// restore the heap order
			SiftDownHeap(nIndex);
		}

		// Moves the item at index nIndex up the heap to the correct position.
		private void SiftUpHeap(int nIndex)
		{
			// we can't move the item at the top of the heap up any further
			if (nIndex > 0)
			{
				// get the item to be moved
				T item = m_aData[nIndex];

				// keep replacing the item's parent with it (while it is less than the parent)
				int nParentIndex = (nIndex - 1) / 2;
				while (nIndex > 0 && m_comparer.Compare(m_aData[nParentIndex], item) > 0)
				{
					m_aData[nIndex] = m_aData[nParentIndex];
					nIndex = nParentIndex;
					nParentIndex = (nIndex - 1) / 2;
				}

				// move the item into the correct position
				m_aData[nIndex] = item;
			}
		}

		// Moves the item at index nIndex down the heap to the correct position.
		private void SiftDownHeap(int nIndex)
		{
			// we can't move items at the bottom of the heap down any further
			int nMaxIndex = m_nSize / 2 - 1;
			if (nIndex <= nMaxIndex)
			{
				// get the item to be moved
				T item = m_aData[nIndex];

				// walk down the heap until we get to the bottom level
				while (nIndex <= nMaxIndex)
				{
					// find the smaller child
					int nChildIndex = nIndex * 2 + 1;
					if (nChildIndex < m_nSize - 1 && m_comparer.Compare(m_aData[nChildIndex], m_aData[nChildIndex + 1]) > 0)
						nChildIndex++;

					// is this item smaller than the child?
					if (m_comparer.Compare(item, m_aData[nChildIndex]) <= 0)
						break;

					// this item is not smaller, so move the child up
					m_aData[nIndex] = m_aData[nChildIndex];
					nIndex = nChildIndex;
				}

				// move the item into the correct position
				m_aData[nIndex] = item;
			}
		}

		readonly IComparer<T> m_comparer;
		T[] m_aData;
		int m_nSize;
		object m_syncRoot;

		static readonly T[] s_aEmpty = new T[0];
	}
}
