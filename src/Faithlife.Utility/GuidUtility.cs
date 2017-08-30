using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Faithlife.Utility
{
	/// <summary>
	/// Helper methods for working with <see cref="Guid"/>.
	/// </summary>
	public static class GuidUtility
	{
		/// <summary>
		/// Tries to parse the specified string as a <see cref="Guid"/>.  A return value indicates whether the operation succeeded.
		/// </summary>
		/// <param name="strGuid">The GUID string to attempt to parse.</param>
		/// <param name="guid">When this method returns, contains the <see cref="Guid"/> equivalent to the GUID
		/// contained in <paramref name="strGuid"/>, if the conversion succeeded, or Guid.Empty if the conversion failed.</param>
		/// <returns><c>true</c> if a GUID was successfully parsed; <c>false</c> otherwise.</returns>
		public static bool TryParse(string strGuid, out Guid guid)
		{
			return Guid.TryParse(strGuid, out guid);
		}

		/// <summary>
		/// Converts a GUID to a short string.
		/// </summary>
		/// <param name="guid">The GUID.</param>
		/// <returns>The short string, which is a 22-character case-sensitive string consisting
		/// of ASCII numbers, letters, hyphens, and/or underscores.</returns>
		public static string ToShortString(this Guid guid)
		{
			// use base-64, but with URL-safe replacements for the slash and plus, and omitting the two trailing equal signs
			return Convert.ToBase64String(guid.ToByteArray())
				.Replace('/', c_chShortStringBase64SlashReplacement)
				.Replace('+', c_chShortStringBase64PlusReplacement)
				.Substring(0, 22);
		}

		/// <summary>
		/// Converts a short string to a GUID.
		/// </summary>
		/// <param name="shortString">The short string.</param>
		/// <returns>The GUID.</returns>
		/// <exception cref="FormatException">The argument is not a valid GUID short string.</exception>
		public static Guid FromShortString(string shortString)
		{
			if (shortString == null)
				throw new ArgumentNullException("shortString");

			// short GUID strings are always 22 characters long
			const string errorMessage = "The input is not a valid GUID short string.";
			if (shortString.Length != 22)
				throw new FormatException(errorMessage);

			// start building the base-64 string, which always ends with two equal signs
			char[] shortChars = (shortString + "==").ToCharArray();

			// convert URL-safe replacements back to slash and plus, and make sure
			//  there is no whitespace (which Convert.FromBase64CharArray ignores)
			for (int index = 0; index < 22; index++)
			{
				char ch = shortChars[index];
				if (ch == c_chShortStringBase64PlusReplacement)
					shortChars[index] = '+';
				else if (ch == c_chShortStringBase64SlashReplacement)
					shortChars[index] = '/';
				else if (!((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9')))
					throw new FormatException(errorMessage);
			}

			try
			{
				// convert base-64 to bytes, and bytes to GUID
				return new Guid(Convert.FromBase64CharArray(shortChars, 0, 24));
			}
			catch (FormatException x)
			{
				throw new FormatException(errorMessage, x);
			}
		}

		/// <summary>
		/// Converts a GUID to a lowercase string with no dashes.
		/// </summary>
		/// <param name="guid">The GUID.</param>
		/// <returns>The GUID as a lowercase string with no dashes.</returns>
		public static string ToLowerNoDashString(this Guid guid)
		{
			return guid.ToString("N");
		}

		/// <summary>
		/// Converts a lowercase, no dashes string to a GUID.
		/// </summary>
		/// <param name="value">The string.</param>
		/// <returns>The GUID.</returns>
		/// <exception cref="FormatException">The argument is not a valid GUID short string.</exception>
		public static Guid FromLowerNoDashString(string value)
		{
			Guid? guid = TryFromLowerNoDashString(value);
			if (guid == null)
				throw new FormatException("The string '{0}' is not a no-dash lowercase GUID.".FormatInvariant(value ?? "(null)"));
			return guid.Value;
		}

		/// <summary>
		/// Attempts to convert a lowercase, no dashes string to a GUID.
		/// </summary>
		/// <param name="value">The string.</param>
		/// <returns>The GUID, if the string could be converted; otherwise, null.</returns>
		public static Guid? TryFromLowerNoDashString(string value)
		{
			Guid guid;
			if (!TryParse(value, out guid) || value != guid.ToLowerNoDashString())
				return null;
			return guid;
		}

		/// <summary>
		/// Creates a name-based UUID using the algorithm from RFC 4122 §4.3.
		/// </summary>
		/// <param name="namespaceId">The ID of the namespace.</param>
		/// <param name="name">The name (within that namespace).</param>
		/// <returns>A UUID derived from the namespace and name.</returns>
		public static Guid Create(Guid namespaceId, string name)
		{
			return Create(namespaceId, name, 5);
		}

		/// <summary>
		/// Creates a name-based UUID using the algorithm from RFC 4122 §4.3.
		/// </summary>
		/// <param name="namespaceId">The ID of the namespace.</param>
		/// <param name="name">The name (within that namespace).</param>
		/// <param name="version">The version number of the UUID to create; this value must be either
		/// 3 (for MD5 hashing) or 5 (for SHA-1 hashing).</param>
		/// <returns>A UUID derived from the namespace and name.</returns>
		public static Guid Create(Guid namespaceId, string name, int version)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			// convert the name to a sequence of octets (as defined by the standard or conventions of its namespace) (step 3)
			// ASSUME: UTF-8 encoding is always appropriate
			byte[] nameBytes = Encoding.UTF8.GetBytes(name);

			return Create(namespaceId, nameBytes, version);
		}

		/// <summary>
		/// Creates a name-based UUID using the algorithm from RFC 4122 §4.3.
		/// </summary>
		/// <param name="namespaceId">The ID of the namespace.</param>
		/// <param name="nameBytes">The name (within that namespace).</param>
		/// <returns>A UUID derived from the namespace and name.</returns>
		public static Guid Create(Guid namespaceId, byte[] nameBytes)
		{
			return Create(namespaceId, nameBytes, 5);
		}

		/// <summary>
		/// Creates a name-based UUID using the algorithm from RFC 4122 §4.3.
		/// </summary>
		/// <param name="namespaceId">The ID of the namespace.</param>
		/// <param name="nameBytes">The name (within that namespace).</param>
		/// <param name="version">The version number of the UUID to create; this value must be either
		/// 3 (for MD5 hashing) or 5 (for SHA-1 hashing).</param>
		/// <returns>A UUID derived from the namespace and name.</returns>
		public static Guid Create(Guid namespaceId, byte[] nameBytes, int version)
		{
			if (version != 3 && version != 5)
				throw new ArgumentOutOfRangeException("version", "version must be either 3 or 5.");

			// convert the namespace UUID to network order (step 3)
			byte[] namespaceBytes = namespaceId.ToByteArray();
			SwapByteOrder(namespaceBytes);

			// compute the hash of the namespace ID concatenated with the name (step 4)
			byte[] data = namespaceBytes.Concat(nameBytes).ToArray();
			byte[] hash;
			using (HashAlgorithm algorithm = version == 3 ? (HashAlgorithm) MD5.Create() : SHA1.Create())
				hash = algorithm.ComputeHash(data);

			// most bytes from the hash are copied straight to the bytes of the new GUID (steps 5-7, 9, 11-12)
			byte[] newGuid = new byte[16];
			Array.Copy(hash, 0, newGuid, 0, 16);

			// set the four most significant bits (bits 12 through 15) of the time_hi_and_version field to the appropriate 4-bit version number from Section 4.1.3 (step 8)
			newGuid[6] = (byte) ((newGuid[6] & 0x0F) | (version << 4));

			// set the two most significant bits (bits 6 and 7) of the clock_seq_hi_and_reserved to zero and one, respectively (step 10)
			newGuid[8] = (byte) ((newGuid[8] & 0x3F) | 0x80);

			// convert the resulting UUID to local byte order (step 13)
			SwapByteOrder(newGuid);
			return new Guid(newGuid);
		}

		/// <summary>
		/// The namespace for fully-qualified domain names (from RFC 4122, Appendix C).
		/// </summary>
		public static readonly Guid DnsNamespace = new Guid("6ba7b810-9dad-11d1-80b4-00c04fd430c8");

		/// <summary>
		/// The namespace for URLs (from RFC 4122, Appendix C).
		/// </summary>
		public static readonly Guid UrlNamespace = new Guid("6ba7b811-9dad-11d1-80b4-00c04fd430c8");

		/// <summary>
		/// The namespace for ISO OIDs (from RFC 4122, Appendix C).
		/// </summary>
		public static readonly Guid IsoOidNamespace = new Guid("6ba7b812-9dad-11d1-80b4-00c04fd430c8");

		// Converts a GUID (expressed as a byte array) to/from network order (MSB-first).
		internal static void SwapByteOrder(byte[] guid)
		{
			SwapBytes(guid, 0, 3);
			SwapBytes(guid, 1, 2);
			SwapBytes(guid, 4, 5);
			SwapBytes(guid, 6, 7);
		}

		private static void SwapBytes(byte[] guid, int left, int right)
		{
			byte temp = guid[left];
			guid[left] = guid[right];
			guid[right] = temp;
		}

		const char c_chShortStringBase64SlashReplacement = '_';
		const char c_chShortStringBase64PlusReplacement = '-';
	}
}
