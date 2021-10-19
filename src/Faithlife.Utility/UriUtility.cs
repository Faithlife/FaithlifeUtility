using System;
using System.Collections.Generic;
using System.Linq;

namespace Faithlife.Utility
{
	/// <summary>
	/// Provides utilities for working with Uris.
	/// </summary>
	public static class UriUtility
	{
		/// <summary>
		/// Builds a URI from a pattern and parameters.
		/// </summary>
		/// <param name="uriPattern">The pattern.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns>Each pair of parameters represents a key and a value; see the other overload
		/// for details.</returns>
		public static Uri FromPattern(string uriPattern, params string[] parameters)
			=> FromPattern(uriPattern, CreatePairsFromStrings(parameters));

		/// <summary>
		/// Builds a URI from a pattern and an object containing parameters.
		/// </summary>
		/// <param name="uriPattern">The pattern.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns>Each value is converted to a string with the invariant culture; see the other overload
		/// for more details.</returns>
		public static Uri FromPattern(string uriPattern, IEnumerable<KeyValuePair<string, object?>> parameters)
			=> FromPattern(uriPattern, parameters.Select(x => new KeyValuePair<string, string?>(x.Key, x.Value is null ? null : InvariantConvert.ToInvariantString(x.Value))));

		/// <summary>
		/// Builds a URI from a pattern and parameters.
		/// </summary>
		/// <param name="uriPattern">The pattern.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns>This method looks for "{key}" in the pattern, where "key" is one of the keys
		/// in the list of parameters, and replaces the key with the corresponding value, properly encoded
		/// for the URI. Any parameter that doesn't appear in the pattern is added to the end of the
		/// URI as a query parameter. The same key must not appear multiple times in the pattern or in
		/// the list of parameters (the behavior is undefined). If the key or value is null, the pair is ignored.</returns>
		public static Uri FromPattern(string uriPattern, IEnumerable<KeyValuePair<string, string?>> parameters)
		{
			var hasQuery = uriPattern.IndexOfOrdinal('?') != -1;

			foreach (var parameter in parameters)
			{
				if (parameter.Key is not null && parameter.Value is not null)
				{
					var bracketedKey = "{" + parameter.Key + "}";
					var bracketedKeyIndex = uriPattern.IndexOf(bracketedKey, StringComparison.Ordinal);
					if (bracketedKeyIndex != -1)
					{
#if NETCOREAPP3_1_OR_GREATER
						uriPattern = string.Concat(uriPattern.AsSpan(0, bracketedKeyIndex), Uri.EscapeDataString(parameter.Value), uriPattern.AsSpan(bracketedKeyIndex + bracketedKey.Length));
#else
						uriPattern = uriPattern.Substring(0, bracketedKeyIndex) + Uri.EscapeDataString(parameter.Value) + uriPattern.Substring(bracketedKeyIndex + bracketedKey.Length);
#endif
					}
					else
					{
						uriPattern += (hasQuery ? "&" : "?") + Uri.EscapeDataString(parameter.Key) + "=" + Uri.EscapeDataString(parameter.Value);
						hasQuery = true;
					}
				}
			}

			return new Uri(uriPattern);
		}

		/// <summary>
		/// Indicates whether a URI targets a domain.
		/// </summary>
		/// <param name="uri">A URI.</param>
		/// <param name="domain">A domain name string.</param>
		/// <returns>True if the supplied URI targets the supplied domain name string, otherwise false.</returns>
		public static bool MatchesDomain(this Uri uri, string domain)
		{
			if (domain is null)
				throw new ArgumentNullException(nameof(domain));
			if (uri is null)
				throw new ArgumentNullException(nameof(uri));
			if (!uri.IsAbsoluteUri)
				throw new ArgumentException("The argument must be an absolute URI.", nameof(uri));

			var host = uri.Host;
			return host.EndsWith(domain, StringComparison.OrdinalIgnoreCase) && (host.Length == domain.Length || host[host.Length - domain.Length - 1] == '.');
		}

		private static IEnumerable<KeyValuePair<string, string?>> CreatePairsFromStrings(string[] strings)
		{
			var stringCount = strings.Length;
			if (stringCount % 2 == 1)
				throw new ArgumentException("The number of strings must be even.");

			for (var stringIndex = 0; stringIndex < stringCount; stringIndex += 2)
				yield return new KeyValuePair<string, string?>(strings[stringIndex], strings[stringIndex + 1]);
		}
	}
}
