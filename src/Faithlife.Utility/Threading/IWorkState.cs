using System.Threading;

namespace Faithlife.Utility.Threading
{
	/// <summary>
	/// Implemented by any worker method's state to support cancellation.
	/// </summary>
	public interface IWorkState
	{
		/// <summary>
		/// Gets a value indicating whether the work is canceled.
		/// </summary>
		/// <value><c>true</c> if canceled; otherwise, <c>false</c>.</value>
		/// <remarks>This property should be polled periodically by the work callback
		/// to determine whether the work has been canceled. If this property is <c>true</c>,
		/// the callback should return as quickly as possible, cancelling any work that it
		/// was doing.</remarks>
		bool Canceled { get; }

		/// <summary>
		/// Returns the <see cref="CancellationToken"/> controlling the work.
		/// </summary>
		/// <remarks>The <see cref="System.Threading.CancellationToken.IsCancellationRequested"/> property of <see cref="CancellationToken"/> should return the same value as <see cref="Canceled"/>.</remarks>
		CancellationToken CancellationToken { get; }
	}
}
