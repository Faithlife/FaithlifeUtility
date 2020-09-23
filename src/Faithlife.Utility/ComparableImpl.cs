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
		public static int CompareToObject<T>(T that, object? obj) where T : IComparable<T> =>
			obj switch
			{
				null => 1,
				T t => that.CompareTo(t),
				_ => throw new ArgumentException("obj is not a {0}".FormatInvariant(typeof(T).Name), nameof(obj)),
			};

		/// <summary>
		/// Standard implementation of the less than operator.
		/// </summary>
		/// <param name="left">The left item.</param>
		/// <param name="right">The right item.</param>
		/// <returns>True if the left is less than the right.</returns>
		public static bool OperatorLessThan<T>(T? left, T? right)
			where T : class, IComparable<T> =>
			(left, right) switch
			{
				(_, _) when ReferenceEquals(left, right) => false,
				(null, _) => true,
				(_, null) => false,
				(_, _) => left!.CompareTo(right!) < 0, // https://github.com/dotnet/roslyn/issues/37136
			};

		/// <summary>
		/// Standard implementation of the less than or equal to operator.
		/// </summary>
		/// <param name="left">The left item.</param>
		/// <param name="right">The right item.</param>
		/// <returns>True if the left is less than or equal to the right.</returns>
		public static bool OperatorLessThanOrEqual<T>(T? left, T? right)
			where T : class, IComparable<T> =>
			(left, right) switch
			{
				(_, _) when ReferenceEquals(left, right) => true,
				(null, _) => true,
				(_, null) => false,
				(_, _) => left!.CompareTo(right!) <= 0, // https://github.com/dotnet/roslyn/issues/37136
			};

		/// <summary>
		/// Standard implementation of the greater than operator.
		/// </summary>
		/// <param name="left">The left item.</param>
		/// <param name="right">The right item.</param>
		/// <returns>True if the left is greater than the right.</returns>
		public static bool OperatorGreaterThan<T>(T? left, T? right)
			where T : class, IComparable<T> =>
			(left, right) switch
			{
				(_, _) when ReferenceEquals(left, right) => false,
				(null, _) => false,
				(_, null) => true,
				(_, _) => left!.CompareTo(right!) > 0, // https://github.com/dotnet/roslyn/issues/37136
			};

		/// <summary>
		/// Standard implementation of the greater than or equal to operator.
		/// </summary>
		/// <param name="left">The left item.</param>
		/// <param name="right">The right item.</param>
		/// <returns>True if the left is greater than or equal to the right.</returns>
		public static bool OperatorGreaterThanOrEqual<T>(T? left, T? right)
			where T : class, IComparable<T> =>
			(left, right) switch
			{
				(_, _) when ReferenceEquals(left, right) => true,
				(null, _) => false,
				(_, null) => true,
				(_, _) => left!.CompareTo(right!) >= 0, // https://github.com/dotnet/roslyn/issues/37136
			};
	}
}
