using System.Threading;

namespace Faithlife.Utility.Threading
{
	/// <summary>
	/// Standard implementations of <see cref="IWorkState" />.
	/// </summary>
	public static class WorkState
	{
		/// <summary>
		/// Returns an <see cref="IWorkState"/> that will be canceled when the specified <see cref="CancellationToken"/> is canceled.
		/// </summary>
		/// <param name="token">The cancellation token.</param>
		/// <returns>An <see cref="IWorkState"/> wrapping <paramref name="token"/>.</returns>
		public static IWorkState FromCancellationToken(CancellationToken token)
		{
			return new CancellationTokenWorkState(token);
		}

		/// <summary>
		/// A non-cancellable work state that can be used when the caller does not want to support work cancellation.
		/// </summary>
		public static IWorkState None
		{
			get { return s_none; }
		}

		/// <summary>
		/// This use of <see cref="WorkState"/> needs to be investigated and replaced with either a real <see cref="IWorkState"/> or <see cref="None"/> as appropriate.
		/// </summary>
		public static IWorkState ToDo
		{
			get { return s_toDo; }
		}

		private sealed class NonCancellableWorkState : IWorkState
		{
			public bool Canceled
			{
				get { return false; }
			}

			public CancellationToken CancellationToken
			{
				get { return CancellationToken.None; }
			}
		}

		private sealed class CancellationTokenWorkState : IWorkState
		{
			public CancellationTokenWorkState(CancellationToken token)
			{
				m_token = token;
			}

			public bool Canceled
			{
				get { return m_token.IsCancellationRequested; }
			}

			public CancellationToken CancellationToken
			{
				get { return m_token; }
			}

			readonly CancellationToken m_token;
		}

		static readonly IWorkState s_none = new NonCancellableWorkState();
		static readonly IWorkState s_toDo = new NonCancellableWorkState();
	}
}
