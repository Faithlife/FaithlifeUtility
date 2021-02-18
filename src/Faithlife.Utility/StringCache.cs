using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Faithlife.Utility
{
	/// <summary>
	/// <see cref="StringCache"/> provides an append-only cache of strings that can be used
	/// to reuse the same string object instance when a string is being dynamically created at runtime
	/// (e.g., loaded from an XML file or database).
	/// </summary>
	/// <remarks><para>Using this class is similar to calling <c>string.Intern</c> except that all
	/// strings cached by <see cref="StringCache"/> can be collected when this instance is GCed;
	/// interned strings never get freed.</para>
	/// <para>To free the strings held by this string cache, release all references to this object, and to
	/// other objects that hold the strings in the cache.</para></remarks>
	public sealed class StringCache
	{
		/// <summary>
		/// Constructs a new instance of the <see cref="StringCache"/> class.
		/// </summary>
		public StringCache() => m_cache = new(StringComparer.Ordinal);

		/// <summary>
		/// Gets an existing string from the cache, or adds it if it's not currently in the cache.
		/// </summary>
		/// <param name="value">The string value to look up.</param>
		/// <returns>The unique cached instance of a <see cref="string"/> that is equal to <paramref name="value"/>.</returns>
		[return: NotNullIfNotNull("value")]
		public string? GetOrAdd(string? value)
		{
			// check for trivial cases
			if (value is null)
				return null;
			if (value.Length == 0)
				return "";

			// use string equality to find the instance in the cache if it exists
			if (m_cache.TryGetValue(value, out var cachedString))
				return cachedString;

			// otherwise, cache this string (it becomes the canonical instance)
#if NETSTANDARD2_0
			m_cache.Add(value, value);
#else
			m_cache.Add(value);
#endif
			return value;
		}

#if NETSTANDARD2_0
		private readonly Dictionary<string, string> m_cache;
#else
		private readonly HashSet<string> m_cache;
#endif
	}
}
