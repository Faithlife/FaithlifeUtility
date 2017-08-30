using System;
using System.Text;

namespace Faithlife.Utility
{
	/// <summary>
	/// Stores settings used for encoding and decoding URL-style strings.
	/// </summary>
	public sealed class UrlEncodingSettings
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UrlEncodingSettings"/> class.
		/// </summary>
		public UrlEncodingSettings()
		{
			ShouldEncodeChar = ch =>
				!(ch >= '0' && ch <= '9') &&
				!(ch >= 'A' && ch <= 'Z') &&
				!(ch >= 'a' && ch <= 'z');
			EncodedBytePrefixChar = '%';
			TextEncoding = Encoding.UTF8;
		}

		/// <summary>
		/// Gets or sets the delegate used to determine whether a character should be encoded or not.
		/// </summary>
		/// <value>The delegate used to determine whether a character should be encoded or not.</value>
		/// <remarks>By default, only ASCII alphanumerics are not encoded.</remarks>
		public Predicate<char> ShouldEncodeChar { get; set; }

		/// <summary>
		/// Gets or sets the character used when encoding a space.
		/// </summary>
		/// <value>If specified, the character used when encoding a space.</value>
		/// <remarks>This property is null by default.</remarks>
		public char? EncodedSpaceChar { get; set; }

		/// <summary>
		/// Gets or sets the character used as the prefix for each encoded byte.
		/// </summary>
		/// <value>The character used as the prefix for each encoded byte.</value>
		/// <remarks>The default value for this property is '%'.</remarks>
		public char EncodedBytePrefixChar { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether hex digits are encoded in uppercase.
		/// </summary>
		/// <value>True if hex digits are encoded in uppercase.</value>
		public bool UppercaseHexDigits { get; set; }

		/// <summary>
		/// True if the encoder should prevent double-encoding.
		/// </summary>
		/// <value>True if the encoder should prevent double-encoding.</value>
		/// <remarks>The default value for this property is false. If set to true, the encoder
		/// will avoid encoding the byte prefix character if it looks like it is already being used
		/// to encode a byte.</remarks>
		public bool PreventDoubleEncoding { get; set; }

		/// <summary>
		/// Gets or sets the text encoding used when encoding or decoding bytes.
		/// </summary>
		/// <value>The text encoding used when encoding or decoding bytes.</value>
		/// <remarks>The default value for this property is Encoding.UTF8.</remarks>
		public Encoding TextEncoding { get; set; }

		/// <summary>
		/// Clones this instance.
		/// </summary>
		/// <returns>A clone of this instance.</returns>
		public UrlEncodingSettings Clone()
		{
			return new UrlEncodingSettings(this);
		}

		/// <summary>
		/// Returns a <see cref="UrlEncodingSettings"/> object that matches the behavior of <c>HttpUtility.UrlEncode(string)</c> in .NET 4.
		/// </summary>
		public static UrlEncodingSettings HttpUtilitySettings
		{
			get
			{
				return new UrlEncodingSettings
				{
					EncodedBytePrefixChar = '%',
					EncodedSpaceChar = '+',
					ShouldEncodeChar = ch => !((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9') ||
						ch == '(' || ch == ')' || ch == '*' || ch == '-' || ch == '.' || ch == '_' || ch == '!'),
					TextEncoding = Encoding.UTF8,
				};
			}
		}

		private UrlEncodingSettings(UrlEncodingSettings settings)
		{
			ShouldEncodeChar = settings.ShouldEncodeChar;
			EncodedSpaceChar = settings.EncodedSpaceChar;
			EncodedBytePrefixChar = settings.EncodedBytePrefixChar;
			PreventDoubleEncoding = settings.PreventDoubleEncoding;
			UppercaseHexDigits = settings.UppercaseHexDigits;
			TextEncoding = settings.TextEncoding;
		}
	}
}
