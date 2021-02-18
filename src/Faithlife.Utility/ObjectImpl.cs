using System;
using System.Diagnostics.CodeAnalysis;

namespace Faithlife.Utility
{
	/// <summary>
	/// Provides methods for manipulating objects.
	/// </summary>
	[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "Legacy.")]
	public static class ObjectImpl
	{
		/// <summary>
		/// Standard implementation of the equality operator.
		/// </summary>
		/// <param name="left">The left item.</param>
		/// <param name="right">The right item.</param>
		/// <returns>True if the items are equal.</returns>
		public static bool OperatorEquality<T>(T? left, T? right)
			where T : class, IEquatable<T>
		{
			if (ReferenceEquals(left, right))
				return true;
			else if (left is null || right is null)
				return false;
			else
				return left.Equals(right);
		}

		/// <summary>
		/// Standard implementation of the inequality operator.
		/// </summary>
		/// <param name="left">The left item.</param>
		/// <param name="right">The right item.</param>
		/// <returns>True if the items are not equal.</returns>
		public static bool OperatorInequality<T>(T? left, T? right)
			where T : class, IEquatable<T>
		{
			if (ReferenceEquals(left, right))
				return false;
			else if (left is null || right is null)
				return true;
			else
				return !left.Equals(right);
		}
	}
}
