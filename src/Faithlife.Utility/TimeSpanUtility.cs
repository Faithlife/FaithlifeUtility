using System;

namespace Faithlife.Utility
{
	/// <summary>
	/// Helper methods for working with <see cref="TimeSpan"/>.
	/// </summary>
	public static class TimeSpanUtility
	{
		/// <summary>
		/// Formats <paramref name="timeSpan"/> as a "friendly" string; the precision of the returned string (which is not culture-sensitive)
		/// depends on the duration of the <see cref="TimeSpan"/>.
		/// </summary>
		/// <param name="timeSpan">The TimeSpan to render.</param>
		/// <returns>A culture-invariant string with a "friendly" rendering of the TimeSpan.</returns>
		[Obsolete("Use FormatForLogging instead.")]
		public static string ToFriendlyString(TimeSpan timeSpan)
		{
			return TimeSpanUtility.FormatForLogging(timeSpan);
		}

		/// <summary>
		/// Formats <paramref name="timeSpan"/> as a concise string suitable for logging; the precision of the returned
		/// string (which is not culture-sensitive) depends on the duration of the <see cref="TimeSpan"/>.
		/// </summary>
		/// <param name="timeSpan">The TimeSpan to render.</param>
		/// <returns>A culture-invariant string with a concise rendering of the TimeSpan.</returns>
		public static string FormatForLogging(TimeSpan timeSpan)
		{
			// log the timespan in the most appropriate format for the actual duration
			if (Math.Abs(timeSpan.TotalSeconds) < 10)
				return "{0}ms".FormatInvariant((int) Math.Round(timeSpan.TotalMilliseconds, 0));
			else if (Math.Abs(timeSpan.TotalMinutes) < 1)
				return "{0:0.00}s".FormatInvariant(GetSeconds(timeSpan, 59.99));
			else if (Math.Abs(timeSpan.TotalHours) < 1)
				return "{0}m {1:0.0}s".FormatInvariant(timeSpan.Minutes, Math.Abs(GetSeconds(timeSpan, 59.9)));
			else if (Math.Abs(timeSpan.TotalDays) < 1)
				return "{0}h {1}m {2:0}s".FormatInvariant((int) timeSpan.TotalHours, Math.Abs(timeSpan.Minutes), Math.Abs(GetSeconds(timeSpan, 59)));
			else
				return "{0}d {1}h {2}m {3:0}s".FormatInvariant((int) timeSpan.TotalDays, Math.Abs(timeSpan.Hours), Math.Abs(timeSpan.Minutes), Math.Abs(GetSeconds(timeSpan, 59)));
		}

		private static double GetSeconds(TimeSpan ts, double limit)
		{
			double seconds = ts.Seconds + ts.Milliseconds / 1000.0;
			return ts.Ticks > 0 ? Math.Min(seconds, limit) : Math.Max(seconds, -limit);
		}
	}
}
