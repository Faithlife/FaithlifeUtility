namespace Faithlife.Utility
{
	/// <summary>
	/// The major class of a Unicode character's general category.
	/// <seealso cref="System.Globalization.UnicodeCategory"/>
	/// </summary>
	public enum UnicodeCharacterClass
	{
		/// <summary>
		/// Indicates that the character is a letter; this class includes the Unicode categories "Lu", "Ll", "Lt", "Lm", "Lo".
		/// </summary>
		Letter,

		/// <summary>
		/// Indicates that the character is a mark; this class includes the Unicode categories "Mn", "Mc", "Me".
		/// </summary>
		Mark,

		/// <summary>
		/// Indicates that the character is a number; this class includes the Unicode categories "Nd", "Nl", "No".
		/// </summary>
		Number,

		/// <summary>
		/// Indicates that the character is punctuation; this class includes the Unicode categories "Pc", "Pd", "Po", "Pc", "Pi", "Pf", "Po".
		/// </summary>
		Punctuation,

		/// <summary>
		/// Indicates that the character is a symbol; this class includes the Unicode categories "Sm", "Sc", "Sk", "So".
		/// </summary>
		Symbol,

		/// <summary>
		/// Indicates that the character is a separator; this class includes the Unicode categories "Zs", "Zl", "Zp".
		/// </summary>
		Separator,

		/// <summary>
		/// Indicates that the character is a control, format, private use, surrogate, or unassigned code point; this class includes the Unicode categories "Cc", "Cf", "Cs", "Co", "Cn".
		/// </summary>
		Other
	}
}
