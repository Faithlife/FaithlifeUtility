using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Faithlife.Utility
{
	/// <summary>
	/// Provides utilities for working with Uris.
	/// </summary>
	public static class UriUtility
	{
		/// <summary>
		/// Parses the text as a web URI.
		/// </summary>
		/// <param name="uriText">The URI text.</param>
		/// <returns>A web URI, or <c>null</c>.</returns>
		/// <remarks>This does not guarantee the URI contains a valid hostname, or refers to a valid resource on that server;
		/// it simply determines if it can create a valid URI beginning with <c>http://</c> or <c>https://</c>.</remarks>
		public static Uri ParseWebUrl(string uriText)
		{
			// try to parse the URI
			Uri uri;
			if (!Uri.TryCreate(uriText, UriKind.Absolute, out uri))
			{
				// failed; try parsing again with explicit "http://" prefix
				const string httpProtocol = "http://";
				Uri.TryCreate(httpProtocol + uriText, UriKind.Absolute, out uri);
			}

			// check for well-formed HTTP or HTTPS URI
			return uri != null && uri.IsWellFormedOriginalString() && (uri.Scheme == "http" || uri.Scheme == "https") ? uri : null;
		}

		/// <summary>
		/// Builds a URI from a pattern and parameters.
		/// </summary>
		/// <param name="uriPattern">The pattern.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns>Each pair of parameters represents a key and a value; see the other overload
		/// for details.</returns>
		public static Uri FromPattern(string uriPattern, params string[] parameters)
		{
			return FromPattern(uriPattern, CreatePairsFromStrings(parameters));
		}

		/// <summary>
		/// Builds a URI from a pattern and an object containing parameters.
		/// </summary>
		/// <param name="uriPattern">The pattern.</param>
		/// <param name="parameters">Object containing the parameters.</param>
		/// <returns>Each property name and value represents a key and a value; see the other overloads
		/// for more details. The other overloads are used if the parameters object is of the corresponding type,
		/// e.g. if the object is an ExpandoObject.</returns>
		public static Uri FromPattern(string uriPattern, object parameters)
		{
			// use overload if possible
			var stringToStringParameters = parameters as IEnumerable<KeyValuePair<string, string>>;
			if (stringToStringParameters != null)
				return FromPattern(uriPattern, stringToStringParameters);

			// use overload if possible
			var stringToObjectParameters = parameters as IEnumerable<KeyValuePair<string, object>>;
			if (stringToObjectParameters != null)
				return FromPattern(uriPattern, stringToObjectParameters);

			return FromPattern(uriPattern, ReflectionUtility.CreatePairsFromObject(parameters));
		}

		/// <summary>
		/// Builds a URI from a pattern and an object containing parameters.
		/// </summary>
		/// <param name="uriPattern">The pattern.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns>Each value is converted to a string with the invariant culture; see the other overload
		/// for more details.</returns>
		public static Uri FromPattern(string uriPattern, IEnumerable<KeyValuePair<string, object>> parameters)
		{
			return FromPattern(uriPattern, parameters.Select(x => new KeyValuePair<string, string>(x.Key, x.Value == null ? null : Convert.ToString(x.Value, CultureInfo.InvariantCulture))));
		}

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
		public static Uri FromPattern(string uriPattern, IEnumerable<KeyValuePair<string, string>> parameters)
		{
			bool hasQuery = uriPattern.IndexOf('?') != -1;

			foreach (KeyValuePair<string, string> parameter in parameters)
			{
				if (parameter.Key != null && parameter.Value != null)
				{
					string bracketedKey = "{" + parameter.Key + "}";
					int bracketedKeyIndex = uriPattern.IndexOf(bracketedKey, StringComparison.Ordinal);
					if (bracketedKeyIndex != -1)
					{
						uriPattern = uriPattern.Substring(0, bracketedKeyIndex) +
							Uri.EscapeDataString(parameter.Value) + uriPattern.Substring(bracketedKeyIndex + bracketedKey.Length);
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
			if (domain == null)
				throw new ArgumentNullException("domain");
			if (uri == null)
				throw new ArgumentNullException("uri");
			if (!uri.IsAbsoluteUri)
				throw new ArgumentException("The argument must be an absolute URI.", "uri");

			var host = uri.Host;
			return host.EndsWith(domain, StringComparison.OrdinalIgnoreCase) && (host.Length == domain.Length || host[host.Length - domain.Length - 1] == '.');
		}

		private static IEnumerable<KeyValuePair<string, string>> CreatePairsFromStrings(string[] strings)
		{
			int stringCount = strings.Length;
			if (stringCount % 2 == 1)
				throw new ArgumentException("The number of strings must be even.");

			for (int stringIndex = 0; stringIndex < stringCount; stringIndex += 2)
				yield return new KeyValuePair<string, string>(strings[stringIndex], strings[stringIndex + 1]);
		}
	}
}
