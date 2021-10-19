using System;

namespace Faithlife.Utility
{
	/// <summary>
	/// Provides helper methods for working with <see cref="UriBuilder"/>.
	/// </summary>
	public static class UriBuilderUtility
	{
		/// <summary>
		/// Appends the specified string to the end of the <see cref="UriBuilder.Query"/>.
		/// </summary>
		/// <param name="builder">The <see cref="UriBuilder"/>.</param>
		/// <param name="query">The query parameter (name and value) to add; this string should not begin with '&amp;'.</param>
		public static UriBuilder AppendQuery(this UriBuilder builder, string query)
		{
			if (query is null)
				throw new ArgumentNullException(nameof(query));

			if (query.Length > 0)
			{
				// NOTE: From http://msdn.microsoft.com/en-us/library/system.uribuilder.query.aspx:
				// Do not append a string directly to this property. If the length of Query is greater than 1, retrieve the property value as a string,
				// remove the leading question mark, append the new query string, and set the property with the combined string.
				if (builder.Query is not null && builder.Query.Length > 1)
#if NETCOREAPP3_1_OR_GREATER
					builder.Query = string.Concat(builder.Query.AsSpan(1), "&", query);
#else
					builder.Query = builder.Query.Substring(1) + "&" + query;
#endif
				else
					builder.Query = query;
			}

			return builder;
		}
	}
}
