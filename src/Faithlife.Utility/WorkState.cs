using System.Threading;

namespace Faithlife.Utility
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
		public static IWorkState FromCancellationToken(CancellationToken token) => new CancellationTokenWorkState(token);

		/// <summary>
		/// A non-cancellable work state that can be used when the caller does not want to support work cancellation.
		/// </summary>
		public static IWorkState None { get; } = new NonCancellableWorkState();

		/// <summary>
		/// This use of <see cref="WorkState"/> needs to be investigated and replaced with either a real <see cref="IWorkState"/> or <see cref="None"/> as appropriate.
		/// </summary>
		public static IWorkState ToDo { get; } = new NonCancellableWorkState();

		private sealed class NonCancellableWorkState : IWorkState
		{
			public bool Canceled => false;
			public CancellationToken CancellationToken => CancellationToken.None;
		}

		private sealed class CancellationTokenWorkState : IWorkState
		{
			public CancellationTokenWorkState(CancellationToken token) => m_token = token;
			public bool Canceled => m_token.IsCancellationRequested;
			public CancellationToken CancellationToken => m_token;
			readonly CancellationToken m_token;
		}
	}
}
