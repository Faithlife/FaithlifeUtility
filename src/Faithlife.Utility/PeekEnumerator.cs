using System;
using System.Collections;
using System.Collections.Generic;

namespace Faithlife.Utility
{
	/// <summary>
	/// An enumerator that can peek ahead.
	/// </summary>
	public sealed class PeekEnumerator<T> : IEnumerator<T>
	{
		/// <summary>
		/// Create an enumerator that can peek ahead.
		/// </summary>
		/// <param name="enumerator">The underlying enumerator.</param>
		public PeekEnumerator(IEnumerator<T> enumerator)
		{
			m_enumerator = enumerator;
			Setup();
		}

		/// <summary>
		/// Sets the enumerator to its initial position, which is before the first element in the collection.
		/// </summary>
		/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
		/// <filterpriority>2</filterpriority>
		public void Reset()
		{
			m_enumerator.Reset();
			Setup();
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose()
		{
			m_enumerator.Dispose();
		}

		/// <summary>
		/// Advances the enumerator to the next element of the collection.
		/// </summary>
		/// <returns>
		/// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
		/// </returns>
		/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
		/// <filterpriority>2</filterpriority>
		public bool MoveNext()
		{
			if (m_bHasNext)
			{
				m_current = m_enumerator.Current;
				m_bHasCurrent = true;
				m_bHasNext = m_enumerator.MoveNext();
				return true;
			}

			m_bHasCurrent = false;
			m_current = default(T);
			return false;
		}

		/// <summary>
		/// Gets the element in the collection at the current position of the enumerator.
		/// </summary>
		/// <returns>
		/// The element in the collection at the current position of the enumerator.
		/// </returns>
		/// <exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element. </exception>
		public T Current
		{
			get
			{
				if (!m_bHasCurrent)
					throw new InvalidOperationException();

				return m_current;
			}
		}

		/// <summary>
		/// Checks whether the enumerator has another element.
		/// </summary>
		public bool HasNext
		{
			get { return m_bHasNext; }
		}

		/// <summary>
		/// Gets the element in the collection at the next position of the enumerator.
		/// </summary>
		/// <returns>
		/// The element in the collection at the next position of the enumerator.
		/// </returns>
		/// <exception cref="T:System.InvalidOperationException">The enumerator is positioned at or after the last element. </exception>
		public T Next
		{
			get { return m_enumerator.Current; }
		}

		/// <summary>
		/// Gets the current element in the collection.
		/// </summary>
		/// <returns>
		/// The current element in the collection.
		/// </returns>
		/// <exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element. </exception>
		/// <filterpriority>2</filterpriority>
		object IEnumerator.Current
		{
			get { return Current; }
		}

		private void Setup()
		{
			m_bHasCurrent = false;
			m_bHasNext = m_enumerator.MoveNext();
		}

		readonly IEnumerator<T> m_enumerator;
		bool m_bHasNext;
		bool m_bHasCurrent;
		T m_current;
	}
}