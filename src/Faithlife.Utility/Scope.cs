using System;

namespace Faithlife.Utility
{
	/// <summary>
	/// Executes the specified delegate when disposed.
	/// </summary>
	public sealed class Scope : IDisposable
	{
		/// <summary>
		/// Creates a <see cref="Scope" /> for the specified delegate.
		/// </summary>
		/// <param name="dispose">The delegate.</param>
		/// <returns>An instance of <see cref="Scope" /> that calls the delegate when disposed.</returns>
		/// <remarks>If dispose is null, the instance does nothing when disposed.</remarks>
		public static Scope Create(Action? dispose) => new Scope(dispose);

		/// <summary>
		/// Creates a <see cref="Scope" /> that disposes the specified object.
		/// </summary>
		/// <param name="disposable">The object to dispose.</param>
		/// <returns>An instance of <see cref="Scope" /> that disposes the object when disposed.</returns>
		/// <remarks>If disposable is null, the instance does nothing when disposed.</remarks>
		public static Scope Create<T>(T disposable)
			where T : IDisposable => disposable is null ? Empty : new Scope(disposable.Dispose);

		/// <summary>
		/// An empty scope, which does nothing when disposed.
		/// </summary>
		public static readonly Scope Empty = new Scope(null);

		/// <summary>
		/// Cancel the call to the encapsulated delegate.
		/// </summary>
		/// <remarks>After calling this method, disposing this instance does nothing.</remarks>
		public void Cancel() => m_dispose = null;

		/// <summary>
		/// Returns a new Scope that will call the encapsulated delegate.
		/// </summary>
		/// <returns>A new Scope that will call the encapsulated delegate.</returns>
		/// <remarks>After calling this method, disposing this instance does nothing.</remarks>
		public Scope Transfer()
		{
			var scope = new Scope(m_dispose);
			m_dispose = null;
			return scope;
		}

		/// <summary>
		/// Calls the encapsulated delegate.
		/// </summary>
		public void Dispose()
		{
			if (m_dispose is not null)
			{
				m_dispose();
				m_dispose = null;
			}
		}

		private Scope(Action? dispose) => m_dispose = dispose;

		private Action? m_dispose;
	}
}
