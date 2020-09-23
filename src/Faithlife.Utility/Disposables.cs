using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Faithlife.Utility
{
	/// <summary>
	/// A collection of disposable objects, disposed in reverse order when the collection is disposed.
	/// </summary>
	/// <remarks>This class is thread-safe. Null disposables are legal and ignored. Objects cannot be
	/// added to the collection after it has been disposed. The collection cannot be enumerated.</remarks>
	[SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface", Justification = "Collection initialization syntax.")]
	public sealed class Disposables : IDisposable, IEnumerable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Disposables"/> class.
		/// </summary>
		public Disposables()
		{
			m_lock = new object();
			m_list = new List<IDisposable?>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Disposables"/> class.
		/// </summary>
		/// <param name="disposables">The initial disposables.</param>
		public Disposables(IEnumerable<IDisposable?> disposables)
		{
			m_lock = new object();
			m_list = new List<IDisposable?>(disposables);
		}

		/// <summary>
		/// Adds the specified disposable.
		/// </summary>
		/// <param name="disposable">The disposable.</param>
		/// <exception cref="System.ObjectDisposedException">Illegal to add after Dispose.</exception>
		public void Add(IDisposable? disposable)
		{
			lock (m_lock)
			{
				if (m_list is null)
					throw new ObjectDisposedException("Illegal to add after Dispose.");

				m_list.Add(disposable);
			}
		}

		/// <summary>
		/// Adds the specified disposables.
		/// </summary>
		/// <param name="disposables">The disposables.</param>
		/// <exception cref="System.ObjectDisposedException">AddRange called after Dispose.</exception>
		public void AddRange(IEnumerable<IDisposable?> disposables)
		{
			foreach (var disposable in disposables)
				Add(disposable);
		}

		/// <summary>
		/// Disposes all added disposables, in reverse order.
		/// </summary>
		public void Dispose()
		{
			List<IDisposable?>? list;
			lock (m_lock)
			{
				list = m_list;
				m_list = null;
			}

			if (list is object)
			{
				var count = list.Count;
				for (var index = count - 1; index >= 0; index--)
					list[index]?.Dispose();
			}
		}

		/// <summary>
		/// Implemented only for collection initialization syntax; throws if called.
		/// </summary>
		IEnumerator IEnumerable.GetEnumerator() => throw new NotSupportedException("IEnumerable.GetEnumerator implemented only for collection initialization syntax.");

		private readonly object m_lock;
		private List<IDisposable?>? m_list;
	}
}
