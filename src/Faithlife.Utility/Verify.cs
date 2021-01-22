using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Faithlife.Utility
{
	/// <summary>
	/// Provides methods for throwing <see cref="InvalidOperationException" /> for "impossible" conditions.
	/// </summary>
	public static class Verify
	{
		/// <summary>
		/// Throws <see cref="InvalidOperationException" /> if the parameter is not true.
		/// </summary>
		/// <param name="value">The parameter to check.</param>
		/// <exception cref="InvalidOperationException"><paramref name="value"/> is false.</exception>
		[DebuggerNonUserCode]
		[AssertionMethod]
		public static void IsTrue(
			[AssertionCondition(AssertionConditionType.IS_TRUE)]
			bool value)
		{
			if (!value)
				throw new InvalidOperationException();
		}

		/// <summary>
		/// Throws <see cref="InvalidOperationException" /> if the parameter is not false.
		/// </summary>
		/// <param name="value">The parameter to check.</param>
		/// <exception cref="InvalidOperationException"><paramref name="value"/> is true.</exception>
		[DebuggerNonUserCode]
		[AssertionMethod]
		public static void IsFalse(
			[AssertionCondition(AssertionConditionType.IS_FALSE)]
			bool value)
		{
			if (value)
				throw new InvalidOperationException();
		}

		/// <summary>
		/// Throws <see cref="InvalidOperationException" /> if the parameter is not null.
		/// </summary>
		/// <param name="obj">The parameter to check.</param>
		/// <exception cref="InvalidOperationException"><paramref name="obj"/> is not null.</exception>
		[DebuggerNonUserCode]
		[AssertionMethod]
		public static void IsNull(
			[AssertionCondition(AssertionConditionType.IS_NULL)]
			object? obj)
		{
			if (obj is object)
				throw new InvalidOperationException();
		}

		/// <summary>
		/// Throws <see cref="InvalidOperationException" /> if the parameter is null.
		/// </summary>
		/// <param name="obj">The parameter to check.</param>
		/// <exception cref="InvalidOperationException"><paramref name="obj"/> is null.</exception>
		[DebuggerNonUserCode]
		[AssertionMethod]
		public static void IsNotNull(
			[AssertionCondition(AssertionConditionType.IS_NOT_NULL)]
			object? obj)
		{
			if (obj is null)
				throw new InvalidOperationException();
		}

		/// <summary>
		/// Throws <see cref="InvalidOperationException" /> if the parameters are not the same object.
		/// </summary>
		[DebuggerNonUserCode]
		public static void AreSame(object? objA, object? objB)
		{
			if (objA != objB)
				throw new InvalidOperationException();
		}

		/// <summary>
		/// Throws <see cref="InvalidOperationException" /> if the parameters are the same object.
		/// </summary>
		[DebuggerNonUserCode]
		public static void AreNotSame(object? objA, object? objB)
		{
			if (objA == objB)
				throw new InvalidOperationException();
		}
	}
}
