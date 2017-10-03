using System.Collections.Generic;

namespace Faithlife.Utility
{
	/// <summary>
	/// Provides methods for working with <see cref="ISet{T}"/>.
	/// </summary>
	public static class SetUtility
	{
		/// <summary>
		/// Returns a read-only wrapper around <paramref name="set"/>.
		/// </summary>
		/// <typeparam name="T">The type of object in the set.</typeparam>
		/// <param name="set">The <see cref="ISet{T}"/> to wrap.</param>
		/// <returns>A new <see cref="ReadOnlySet{T}"/> that wraps <paramref name="set"/>.</returns>
		public static ReadOnlySet<T> AsReadOnly<T>(this ISet<T> set)
		{
			return new ReadOnlySet<T>(set);
		}
	}
}
