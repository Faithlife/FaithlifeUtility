using System;
using System.Collections.Generic;

namespace Faithlife.Utility
{
	/// <summary>
	/// Provides methods for manipulating objects.
	/// </summary>
	public static class ObjectUtility
	{
		/// <summary>
		/// Gets the hash code for the specified object.
		/// </summary>
		/// <param name="obj">The object for which to get a hash code.</param>
		/// <returns>The hash code for the specified object, or zero if the object is null.</returns>
		public static int GetHashCode<T>(T obj) => obj is null ? 0 : obj.GetHashCode();

		/// <summary>
		/// Creates an equality comparer from delegates.
		/// </summary>
		/// <typeparam name="T">The type to compare.</typeparam>
		/// <param name="equals">The equals delegate.</param>
		/// <returns>The equality comparer.</returns>
		/// <remarks>If GetHashCode is called, it will throw a NotImplementedException.</remarks>
		public static EqualityComparer<T> CreateEqualityComparer<T>(Func<T?, T?, bool> equals) => new GenericEqualityComparer<T>(equals);

		/// <summary>
		/// Creates an equality comparer from delegates.
		/// </summary>
		/// <typeparam name="T">The type to compare.</typeparam>
		/// <param name="equals">The equals delegate.</param>
		/// <param name="getHashCode">The hash code delegate.</param>
		/// <returns>The equality comparer.</returns>
		/// <remarks>If getHashCode is null and GetHashCode is called, it will throw a NotImplementedException.</remarks>
		public static EqualityComparer<T> CreateEqualityComparer<T>(Func<T?, T?, bool> equals, Func<T, int> getHashCode) => new GenericEqualityComparer<T>(equals, getHashCode);
	}
}
