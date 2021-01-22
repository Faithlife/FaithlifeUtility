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
		public PriorityQueue()
			: this(0, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PriorityQueue{T}"/> class that is empty, has the specified initial capacity,
		/// and is sorted according to the default <see cref="Comparer{T}"/> for the type of the data.
		/// </summary>
		/// <param name="capacity">The initial capacity.</param>
		public PriorityQueue(int capacity)
			: this(capacity, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PriorityQueue{T}"/> class that is empty, has the default initial capacity,
		/// and is sorted according to the specified <see cref="IComparer{T}"/>.
		/// </summary>
		/// <param name="comparer">The <see cref="IComparer{T}"/> implementation to use when comparing items.<br/>
		/// -or-<br/>
		/// a null reference to use the default <see cref="Comparer{T}"/> for the type of the data.</param>
		public PriorityQueue(IComparer<T>? comparer)
			: this(0, comparer)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PriorityQueue{T}"/> class that is empty, has the specified initial capacity,
		/// and is sorted according to the specified <see cref="IComparer{T}"/>.
		/// </summary>
		/// <param name="capacity">The initial capacity.</param>
		/// <param name="comparer">The <see cref="IComparer{T}"/> implementation to use when comparing items.<br/>
		/// -or-<br/>
		/// a null reference to use the default <see cref="Comparer{T}"/> for the type of the data.</param>
		public PriorityQueue(int capacity, IComparer<T>? comparer)
		{
			if (capacity < 0)
				throw new ArgumentOutOfRangeException(nameof(capacity), "The parameter must be a non-negative number.");

			m_comparer = comparer ?? Comparer<T>.Default;
			m_array = s_emptyArray;
			if (capacity > 0)
				SetCapacity(capacity);
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
				return m_array.Length;
			}
			set
			{
				// check argument
				if (value < m_size)
					throw new ArgumentOutOfRangeException(nameof(value), "Capacity cannot be less than the current size.");

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
			if (m_size == 0)
				throw new InvalidOperationException("The queue is empty.");

			// remove the first item and return it
			var item = m_array[0];
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
			if (m_size == 0)
				throw new InvalidOperationException("The queue is empty.");

			// return the item at the top of the heap
			return m_array[0];
		}

		/// <summary>
		/// Adds an object to the <see cref="PriorityQueue{T}"/>. This operation takes O(log n) time.
		/// </summary>
		public void Enqueue(T item)
		{
			// make sure the array is big enough
			EnsureCapacity(m_size + 1);

			// add the new item to the bottom of the heap, then move it up to the correct location
			m_array[m_size] = item;
			SiftUpHeap(m_size);
			++m_size;
		}

		/// <summary>
		/// Removes the first item equal to <paramref name="item"/> in the priority queue.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		public void Remove(T item)
		{
			for (var index = 0; index < m_size; index++)
			{
				if (m_comparer.Compare(m_array[index], item) == 0)
				{
					RemoveAt(index);
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
			CollectionImpl.CheckCopyToParameters(array, index, m_size);

			// copy each item to the destination array
			foreach (var item in this)
				array[index++] = item;
		}

		/// <summary>
		/// Puts the item at the head of the queue into the correct place. If you modify the item returned by <see cref="Peek"/> such that its
		/// "priority" changes, call this method to restore the correct order in the <see cref="PriorityQueue{T}"/>.
		/// </summary>
		public void RepositionHead()
		{
			// check for empty queue
			if (m_size == 0)
				throw new InvalidOperationException("The queue is empty.");

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
			if (m_size == 0)
				throw new InvalidOperationException("The queue is empty.");

			// replace the head of the queue with the new item
			m_array[0] = item;

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
			CollectionImpl.CheckCopyToParameters(array, index, m_size);

			try
			{
				// copy each item to the destination array
				foreach (var item in this)
					array.SetValue(item, index++);
			}
			catch (InvalidCastException)
			{
				// convert exception thrown by Array.SetValue to the type that ICollection.CopyTo is expected to throw.
				throw new ArgumentException("The type of the source collection cannot be cast automatically to the type of the destination array.", nameof(array));
			}
		}

		/// <summary>
		/// Gets the number of elements contained in the <see cref="PriorityQueue{T}"/>.
		/// </summary>
		/// <returns>The number of elements contained in the <see cref="PriorityQueue{T}"/>.</returns>
		public int Count => m_size;

		/// <summary>
		/// Gets an object that can be used to synchronize access to the <see cref="PriorityQueue{T}"/>.
		/// </summary>
		/// <returns>An object that can be used to synchronize access to the <see cref="PriorityQueue{T}"/>.</returns>
		object ICollection.SyncRoot
		{
			get
			{
				if (m_syncRoot is null)
					Interlocked.CompareExchange(ref m_syncRoot, new object(), null);

				return m_syncRoot;
			}
		}

		/// <summary>
		/// Gets a value indicating whether access to the <see cref="PriorityQueue{T}"/> is synchronized (thread safe).
		/// </summary>
		/// <returns><c>true</c> if access to the <see cref="PriorityQueue{T}"/> is synchronized (thread safe); otherwise, false.</returns>
		bool ICollection.IsSynchronized => false;

		/// <summary>
		/// Returns an enumerator that iterates through the items in the <see cref="PriorityQueue{T}"/>, in order. This operation
		/// is potentially very expensive; clients should prefer to repeatedly call <see cref="Dequeue"/>.
		/// </summary>
		/// <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the collection.</returns>
		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>) this).GetEnumerator();

		/// <summary>
		/// Returns an enumerator that iterates through the items in the <see cref="PriorityQueue{T}"/>, in order. This operation
		/// is potentially very expensive; clients should prefer to repeatedly call <see cref="Dequeue"/>.
		/// </summary>
		/// <returns>A <see cref="IEnumerator{T}"/> that can be used to iterate through the collection.</returns>
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			// make a copy of the current data
			var copy = new PriorityQueue<T>(this);

			// return each item, in order
			while (copy.Count > 0)
				yield return copy.Dequeue();
		}

		private PriorityQueue(PriorityQueue<T> pq)
		{
			// make a copy of the other PriorityQueue
			m_comparer = pq.m_comparer;
			m_size = pq.m_size;
			m_array = new T[m_size];
			if (m_size > 0)
				Array.Copy(pq.m_array, m_array, m_size);
		}

		private void EnsureCapacity(int desiredSize)
		{
			// check if the array is big enough
			if (desiredSize > m_array.Length)
			{
				// not big enough; resize (to either two times the current size or the current size plus four, whichever is larger)
				var newCapacity = Math.Max(m_array.Length * 2, m_array.Length + 4);
				SetCapacity(newCapacity);
			}
		}

		// Sets the capacity of the priority queue to the specified amount.
		private void SetCapacity(int capacity)
		{
			if (capacity > 0)
			{
				// allocate new array and copy any values across to it
				var newData = new T[capacity];
				if (m_size > 0)
					Array.Copy(m_array, newData, m_size);
				m_array = newData;
			}
			else
			{
				// use empty array
				m_array = s_emptyArray;
			}
		}

		// Removes the item at index 'index' from the queue.
		private void RemoveAt(int index)
		{
			// replace the item with the last item
			m_size--;
			m_array[index] = m_array[m_size];
			m_array[m_size] = default!;

			// restore the heap order
			SiftDownHeap(index);
		}

		// Moves the item at index index up the heap to the correct position.
		private void SiftUpHeap(int index)
		{
			// we can't move the item at the top of the heap up any further
			if (index > 0)
			{
				// get the item to be moved
				var item = m_array[index];

				// keep replacing the item's parent with it (while it is less than the parent)
				var parentIndex = (index - 1) / 2;
				while (index > 0 && m_comparer.Compare(m_array[parentIndex], item) > 0)
				{
					m_array[index] = m_array[parentIndex];
					index = parentIndex;
					parentIndex = (index - 1) / 2;
				}

				// move the item into the correct position
				m_array[index] = item;
			}
		}

		// Moves the item at index index down the heap to the correct position.
		private void SiftDownHeap(int index)
		{
			// we can't move items at the bottom of the heap down any further
			var maxIndex = m_size / 2 - 1;
			if (index <= maxIndex)
			{
				// get the item to be moved
				var item = m_array[index];

				// walk down the heap until we get to the bottom level
				while (index <= maxIndex)
				{
					// find the smaller child
					var childIndex = index * 2 + 1;
					if (childIndex < m_size - 1 && m_comparer.Compare(m_array[childIndex], m_array[childIndex + 1]) > 0)
						childIndex++;

					// is this item smaller than the child?
					if (m_comparer.Compare(item, m_array[childIndex]) <= 0)
						break;

					// this item is not smaller, so move the child up
					m_array[index] = m_array[childIndex];
					index = childIndex;
				}

				// move the item into the correct position
				m_array[index] = item;
			}
		}

		private readonly IComparer<T> m_comparer;
		private T[] m_array;
		private int m_size;
		private object? m_syncRoot;

		private static readonly T[] s_emptyArray = new T[0];
	}
}
