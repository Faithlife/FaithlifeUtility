using System;
using System.Threading;

namespace Faithlife.Utility
{
	/// <summary>
	/// Tracks the time left for a timeout.
	/// </summary>
	public sealed class TimeoutTimer
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TimeoutTimer"/> class.
		/// </summary>
		/// <remarks>Creates an already timed out timer.</remarks>
		public TimeoutTimer() => Reset(0);

		/// <summary>
		/// Initializes a new instance of the <see cref="TimeoutTimer"/> class.
		/// </summary>
		/// <param name="millisecondsUntilTimeout">The timeout in milliseconds.</param>
		/// <remarks>Use int.MaxValue (or Timeout.Infinite) for a timer that never times out.</remarks>
		public TimeoutTimer(int millisecondsUntilTimeout) => Reset(millisecondsUntilTimeout);

		/// <summary>
		/// Initializes a new instance of the <see cref="TimeoutTimer"/> class.
		/// </summary>
		/// <param name="timeSpanUntilTimeout">The time span for the timeout.</param>
		/// <remarks>Use at least int.MaxValue (or Timeout.Infinite) milliseconds for a timer that never times out.</remarks>
		public TimeoutTimer(TimeSpan timeSpanUntilTimeout) => Reset(timeSpanUntilTimeout);

		/// <summary>
		/// True if there is no time left.
		/// </summary>
		/// <value><c>true</c> if there is no time left; otherwise, <c>false</c>.</value>
		public bool TimedOut => RemainingMilliseconds == 0;

		/// <summary>
		/// Returns true for a timer that never times out.
		/// </summary>
		/// <value>True for a timer that never times out.</value>
		public bool IsInfinite => m_timeoutTicks == int.MaxValue;

		/// <summary>
		/// Gets the time left, in milliseconds.
		/// </summary>
		/// <value>The time left, in milliseconds, or zero if there is no time left.</value>
		/// <remarks>Returns int.MaxValue for a timer that never times out.</remarks>
		public int RemainingMilliseconds
		{
			get
			{
				if (m_timeoutTicks == int.MaxValue)
					return int.MaxValue;

				var nTicksSinceStart = unchecked(Environment.TickCount - m_startTickCount);
				return Math.Max(0, m_timeoutTicks - nTicksSinceStart);
			}
		}

		/// <summary>
		/// Gets the time left.
		/// </summary>
		/// <value>The time left.</value>
		/// <remarks>Returns int.MaxValue milliseconds for a timer that never times out.</remarks>
		public TimeSpan RemainingTimeSpan => new TimeSpan(0, 0, 0, 0, RemainingMilliseconds);

		/// <summary>
		/// Gets the time elapsed, in milliseconds.
		/// </summary>
		/// <value>The time elapsed, in milliseconds.</value>
		public int ElapsedMilliseconds => unchecked(Environment.TickCount - m_startTickCount);

		/// <summary>
		/// Gets the time elapsed, in milliseconds.
		/// </summary>
		/// <value>The time elapsed, in milliseconds.</value>
		public TimeSpan ElapsedTimeSpan => new TimeSpan(0, 0, 0, 0, ElapsedMilliseconds);

		/// <summary>
		/// Resets the timer.
		/// </summary>
		/// <param name="millisecondsUntilTimeout">The timeout in milliseconds.</param>
		/// <remarks>Use int.MaxValue (or Timeout.Infinite) for a timer that never times out.</remarks>
		public void Reset(int millisecondsUntilTimeout)
		{
			if (millisecondsUntilTimeout == Timeout.Infinite)
				millisecondsUntilTimeout = int.MaxValue;

			if (millisecondsUntilTimeout < 0)
				throw new ArgumentException("Time until timeout cannot be negative.");

			m_timeoutTicks = millisecondsUntilTimeout;
			m_startTickCount = Environment.TickCount;
		}

		/// <summary>
		/// Resets the timer.
		/// </summary>
		/// <remarks>Use at least int.MaxValue (or Timeout.Infinite) milliseconds for a timer that never times out.</remarks>
		public void Reset(TimeSpan timeSpanUntilTimeout) => Reset((int) Math.Min(timeSpanUntilTimeout.TotalMilliseconds, int.MaxValue));

		private int m_startTickCount;
		private int m_timeoutTicks;
	}
}
