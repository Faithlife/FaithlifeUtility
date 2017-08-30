using System;

namespace Faithlife.Utility
{
	/// <summary>
	/// Provides methods for implementing <see cref="System.IComparable" />.
	/// </summary>
	public static class ComparableImpl
	{
		/// <summary>
		/// Implements <b>IComparable.CompareTo</b> using <b>IComparable{T}.CompareTo</b>.
		/// </summary>
		/// <param name="that">An instance of IComparable{T}.</param>
		/// <param name="obj">The object to compare to.</param>
		/// <returns>The result of <b>IComparable{T}.CompareTo</b>.</returns>
		/// <exception cref="System.ArgumentException"><paramref name="obj"/> is not an
		/// instance of T.</exception>
		/// <remarks>Does not check whether <paramref name="that"/> is null,
		/// since <c>this</c> should always be used.</remarks>
		public static int CompareToObject<T>(T that, object obj) where T : IComparable<T>
		{
			if (object.ReferenceEquals(obj, null))
				return 1;
			if (!(obj is T))
				throw new ArgumentException(OurMessages.Argument_CompareToInvalidObject.FormatInvariant(typeof(T).Name), "obj");
			return that.CompareTo((T) obj);
		}

		/// <summary>
		/// Standard implementation of the less than operator.
		/// </summary>
		/// <param name="left">The left item.</param>
		/// <param name="right">The right item.</param>
		/// <returns>True if the left is less than the right.</returns>
		public static bool OperatorLessThan<T>(T left, T right)
			where T : class, IComparable<T>
		{
			if (object.ReferenceEquals(left, right))
				return false;
			else if (object.ReferenceEquals(left, null))
				return true;
			else if (object.ReferenceEquals(right, null))
				return false;
			else
				return left.CompareTo(right) < 0;
		}

		/// <summary>
		/// Standard implementation of the less than or equal to operator.
		/// </summary>
		/// <param name="left">The left item.</param>
		/// <param name="right">The right item.</param>
		/// <returns>True if the left is less than or equal to the right.</returns>
		public static bool OperatorLessThanOrEqual<T>(T left, T right)
			where T : class, IComparable<T>
		{
			if (object.ReferenceEquals(left, right))
				return true;
			else if (object.ReferenceEquals(left, null))
				return true;
			else if (object.ReferenceEquals(right, null))
				return false;
			else
				return left.CompareTo(right) <= 0;
		}

		/// <summary>
		/// Standard implementation of the greater than operator.
		/// </summary>
		/// <param name="left">The left item.</param>
		/// <param name="right">The right item.</param>
		/// <returns>True if the left is greater than the right.</returns>
		public static bool OperatorGreaterThan<T>(T left, T right)
			where T : class, IComparable<T>
		{
			if (object.ReferenceEquals(left, right))
				return false;
			else if (object.ReferenceEquals(left, null))
				return false;
			else if (object.ReferenceEquals(right, null))
				return true;
			else
				return left.CompareTo(right) > 0;
		}

		/// <summary>
		/// Standard implementation of the greater than or equal to operator.
		/// </summary>
		/// <param name="left">The left item.</param>
		/// <param name="right">The right item.</param>
		/// <returns>True if the left is greater than or equal to the right.</returns>
		public static bool OperatorGreaterThanOrEqual<T>(T left, T right)
			where T : class, IComparable<T>
		{
			if (object.ReferenceEquals(left, right))
				return true;
			else if (object.ReferenceEquals(left, null))
				return false;
			else if (object.ReferenceEquals(right, null))
				return true;
			else
				return left.CompareTo(right) >= 0;
		}
	}
}
