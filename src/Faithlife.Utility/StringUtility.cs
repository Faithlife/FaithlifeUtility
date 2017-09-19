using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using JetBrains.Annotations;

namespace Faithlife.Utility
{
	/// <summary>
	/// Provides methods for manipulating strings.
	/// </summary>
	public static class StringUtility
	{
		/// <summary>
		/// Returns true if the string is null or empty.
		/// </summary>
		/// <param name="value">The string.</param>
		/// <returns>True if the string is null or empty.</returns>
		public static bool IsNullOrEmpty(this string value)
		{
			return string.IsNullOrEmpty(value);
		}

		/// <summary>
		/// Returns true if the string is <c>null</c>, empty, or consists only of white-space characters.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsNullOrWhiteSpace(this string value)
		{
			return string.IsNullOrWhiteSpace(value);
		}

		/// <summary>
		/// Compare the two strings for similarity
		/// </summary>
		/// <param name="left">The left string.</param>
		/// <param name="right">The right string.</param>
		/// <returns>The percentage that they are similar.</returns>
		public static int CalculateSimilarity(string left, string right)
		{
			if (left == null)
				throw new ArgumentNullException("left");
			if (right == null)
				throw new ArgumentNullException("right");
			return CalculateSimilarity(left, 0, left.Length, right, 0, right.Length);
		}

		/// <summary>
		/// Compare the two strings for similarity
		/// </summary>
		/// <param name="left">The left string.</param>
		/// <param name="startLeft">The start index of the left.</param>
		/// <param name="lengthLeft">The length of the left.</param>
		/// <param name="right">The right string.</param>
		/// <param name="startRight">The start index of the right.</param>
		/// <param name="lengthRight">The length of the right.</param>
		/// <returns>The percentage that they are similar.</returns>
		public static int CalculateSimilarity(string left, int startLeft, int lengthLeft, string right, int startRight, int lengthRight)
		{
			if (left == null)
				throw new ArgumentNullException("left");
			if (right == null)
				throw new ArgumentNullException("right");
			if (startLeft < 0)
				throw new ArgumentOutOfRangeException("startLeft");
			if (startRight < 0)
				throw new ArgumentOutOfRangeException("startRight");
			if (startLeft + lengthLeft > left.Length)
				throw new ArgumentOutOfRangeException("lengthLeft");
			if (startRight + lengthRight > right.Length)
				throw new ArgumentOutOfRangeException("lengthRight");

			{
				StringSegment s1 = new StringSegment(left, startLeft, lengthLeft);
				StringSegment s2 = new StringSegment(right, startRight, lengthRight);

				List<int> st_left1 = new List<int>();
				List<int> st_right1 = new List<int>();
				List<int> st_left2 = new List<int>();
				List<int> st_right2 = new List<int>();

				int iTop = 0;
				int i = 0;
				int j = 0;

				// count of equal chars
				int nScore = 0;

				// max index to examine in first string
				int iEnd1 = lengthLeft - 1;
				int iEnd2 = lengthRight - 1;

				st_left1.Add(0);
				st_right1.Add(iEnd1);
				st_left2.Add(0);
				st_right2.Add(iEnd2);

				// first free element location
				iTop = 1;

				do
				{
					iTop--;
					int iL1 = st_left1[iTop];
					int iR1 = st_right1[iTop];
					int iL2 = st_left2[iTop];
					int iR2 = st_right2[iTop];

					int nCount = 0;

					int nMaxCount = 0;

					int iStart1 = 0;
					int iStart2 = 0;

					int iMaxStart1 = -1;
					int iMaxStart2 = -1;

					for (i = iL1; i <= iR1; i++)
					{
						j = iL2;
						while (j <= iR2)
						{
							if (s1[i] == s2[j])
							{
								iStart1 = i;
								iStart2 = j;

								int tmpi = i;
								while (s1[tmpi] == s2[j])
								{
									tmpi++;
									j++;
									if (tmpi > iR1)
										break;
									if (j > iR2)
										break;
								}

								nCount = tmpi - iStart1; /* at least 1 */

								if (nCount > nMaxCount)
								{
									iMaxStart1 = iStart1;
									iMaxStart2 = iStart2;
									nMaxCount = nCount;
								}

								j--;  /* so the j++ below doesn't go past the first char
				   						after the match we've just looked at. */
							}

							j++;
						}
					}

					// found a substring, so we recurse
					if (nMaxCount > 0)
					{
						nScore += nMaxCount << 1;

						if (iMaxStart1 != iL1 && iMaxStart2 != iL2)
						{
							// add stack entry for left substring
							// add a new element, if necessary
							if (iTop >= st_left1.Count)
							{
								st_left1.Add(0);
								st_right1.Add(0);
								st_left2.Add(0);
								st_right2.Add(0);
							}

							st_left1[iTop] = iL1;
							st_right1[iTop] = iMaxStart1 - 1;
							st_left2[iTop] = iL2;
							st_right2[iTop] = iMaxStart2 - 1;
							iTop++;
#if false
							printf("on `");
							print_range(s1, iL1, iR1);
							printf("' and `");
							print_range(s2, iL2, iR2);

							printf("'\n adding substrings on left:\n");
							printf("  1: [%d -> %d]: ", iL1, iMaxStart1-1);
							print_range(s1, iL1, iMaxStart1-1);
							printf("\n  2: [%d -> %d]: ", iL2, iMaxStart2-1);
							print_range(s2, iL2, iMaxStart2-1);
							printf("\n");
#endif
						}

						int iNewR1 = iMaxStart1 + nMaxCount - 1;
						int iNewR2 = iMaxStart2 + nMaxCount - 1;

						if (iNewR1 < iR1 && iNewR2 < iR2)
						{
							/* add stack entry for right substring */
							if (iTop >= st_left1.Count)
							{
								st_left1.Add(0);
								st_right1.Add(0);
								st_left2.Add(0);
								st_right2.Add(0);
							}
							st_left1[iTop] = iNewR1 + 1;
							st_right1[iTop] = iR1;
							st_left2[iTop] = iNewR2 + 1;
							st_right2[iTop] = iR2;
							iTop++;

#if false
							printf("on `");
							print_range(s1, iL1, iR1);
							printf("' and `");
							print_range(s2, iL2, iR2);

							printf("'\n adding substrings on right:\n");
							printf("  1: [%d -> %d]: ", iNewR1+1, iR1);
							print_range(s1, iNewR1+1, iR1);
							printf("\n  2: [%d -> %d]: ", iNewR2+1, iR2);
							print_range(s2, iNewR2+1, iR2);
							printf("\n");
#endif
						}
					}
					else
					{
#if false
						printf("found no substrings in `");
						print_range(s1, iL1, iR1);
						printf("' or in `");
						print_range(s2, iL2, iR2);
						printf("'\n");
#endif
					}
				}
				while (iTop != 0);

				int nTotalLen = lengthLeft + lengthRight;

				// prevent division by zero -- two empty strings are identical
				return nTotalLen > 0 ? (100 * nScore + ((nTotalLen + 1) / 2)) / nTotalLen : 100;
			}
		}

		/// <summary>
		/// Determines whether a string ends with the specified character.
		/// </summary>
		/// <param name="source">The string.</param>
		/// <param name="value">The character.</param>
		/// <returns><c>true</c> if the last character in the string is <paramref name="value"/>; <c>false</c> otherwise.</returns>
		public static bool EndsWith(this string source, char value)
		{
			int length = source.Length;
			return length != 0 && source[length - 1] == value;
		}

		/// <summary>
		/// Calls string.StartsWith with StringComparison.Ordinal.
		/// </summary>
		public static bool StartsWithOrdinal(this string source, string value)
		{
			return source.StartsWith(value, StringComparison.Ordinal);
		}

		/// <summary>
		/// Calls string.EndsWith with StringComparison.Ordinal.
		/// </summary>
		public static bool EndsWithOrdinal(this string source, string value)
		{
			return source.EndsWith(value, StringComparison.Ordinal);
		}

		/// <summary>
		/// Determines whether the beginning of this string instance matches the specified string when compared using the specified culture.
		/// </summary>
		/// <param name="source">The string.</param>
		/// <param name="prefix">The string to compare.</param>
		/// <param name="ignoreCase"><c>true</c> to ignore case during the comparison; otherwise, <c>false</c>.</param>
		/// <param name="cultureInfo">Cultural information that determines how this string and value are compared. If culture is <c>null</c>, the current culture is used.</param>
		/// <returns><c>true</c>, if the value parameter matches the beginning of this string; otherwise, <c>false</c>.</returns>
		public static bool StartsWith(this string source, string prefix, bool ignoreCase, CultureInfo cultureInfo)
		{
			return source.StartsWith(prefix, ignoreCase, cultureInfo);
		}

		/// <summary>
		/// Calls string.IndexOf with StringComparison.Ordinal.
		/// </summary>
		public static int IndexOfOrdinal(this string source, string value)
		{
			return source.IndexOf(value, StringComparison.Ordinal);
		}

		/// <summary>
		/// Calls string.IndexOf with StringComparison.Ordinal.
		/// </summary>
		public static int IndexOfOrdinal(this string source, string value, int startIndex)
		{
			return source.IndexOf(value, startIndex, StringComparison.Ordinal);
		}

		/// <summary>
		/// Calls string.IndexOf with StringComparison.Ordinal.
		/// </summary>
		public static int IndexOfOrdinal(this string source, string value, int startIndex, int count)
		{
			return source.IndexOf(value, startIndex, count, StringComparison.Ordinal);
		}

		/// <summary>
		/// Calls string.LastIndexOf with StringComparison.Ordinal.
		/// </summary>
		public static int LastIndexOfOrdinal(this string source, string value)
		{
			return source.LastIndexOf(value, StringComparison.Ordinal);
		}

		/// <summary>
		/// Calls string.LastIndexOf with StringComparison.Ordinal.
		/// </summary>
		public static int LastIndexOfOrdinal(this string source, string value, int startIndex)
		{
			return source.LastIndexOf(value, startIndex, StringComparison.Ordinal);
		}

		/// <summary>
		/// Calls string.LastIndexOf with StringComparison.Ordinal.
		/// </summary>
		public static int LastIndexOfOrdinal(this string source, string value, int startIndex, int count)
		{
			return source.LastIndexOf(value, startIndex, count, StringComparison.Ordinal);
		}

		/// <summary>
		/// Compares two specified <see cref="String"/> objects by comparing successive Unicode code points. This method differs from
		/// <see cref="String.CompareOrdinal(string, string)"/> in that this method considers supplementary characters (which are
		/// encoded as two surrogate code units) to be greater than characters in the base multilingual plane (because they have higher
		/// Unicode code points). This method sorts strings in code point order, which is the same as a byte-wise comparison of UTF-8 or
		/// UTF-32 encoded strings.
		/// </summary>
		/// <param name="left">The first string.</param>
		/// <param name="right">The second string.</param>
		/// <returns>Less than zero if <paramref name="left"/> is less than <paramref name="right"/>; zero if the strings are equal;
		/// greater than zero if <paramref name="left"/> is greater than <paramref name="right"/>.</returns>
		public static int CompareByCodePoint(string left, string right)
		{
			// null sorts less than anything else (same as String.CompareOrdinal)
			if (left == null)
				return right == null ? 0 : -1;
			else if (right == null)
				return 1;

			// get the length of both strings
			int nLeftLength = left.Length;
			int nRightLength = right.Length;

			// compare at most the number of characters the strings have in common
			int nMaxIndex = Math.Min(nLeftLength, nRightLength);
			for (int nIndex = 0; nIndex < nMaxIndex; nIndex++)
			{
				char chLeft = left[nIndex];
				char chRight = right[nIndex];

				// algorithm from the Unicode Standard 5.0, Section 5.17 (Binary Order), page 183
				if (chLeft != chRight)
					return unchecked((char) (chLeft + s_mapUtf16FixUp[chLeft >> 11]) - (char) (chRight + s_mapUtf16FixUp[chRight >> 11]));
			}

			// the shorter string (if any) is less than the other
			return nLeftLength - nRightLength;
		}

		/// <summary>
		/// Compares two specified <see cref="String"/> objects, ignoring or honoring their case, and using culture-specific information
		/// to influence the comparison, and returns an integer that indicates their relative position in the sort order.
		/// </summary>
		/// <param name="left">The first string to compare.</param>
		/// <param name="right">The second string to compare.</param>
		/// <param name="ignoreCase"><c>true</c> to ignore case during the comparison; otherwise, <c>false</c>.</param>
		/// <param name="cultureInfo">An object that supplies culture-specific comparison information.</param>
		/// <returns>A 32-bit signed integer that indicates the lexical relationship between the two comparands.</returns>
		public static int Compare(string left, string right, bool ignoreCase, CultureInfo cultureInfo)
		{
			if (cultureInfo == null)
				throw new ArgumentNullException("cultureInfo");
			return cultureInfo.CompareInfo.Compare(left, right, ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);
		}

		/// <summary>
		/// Compares two specified <see cref="String"/> objects, ignoring or honoring their case, and using culture-specific information
		/// to influence the comparison, and returns an integer that indicates their relative position in the sort order.
		/// </summary>
		/// <param name="left">The first string to compare.</param>
		/// <param name="offsetLeft">The position of the substring within <paramref name="left"/>.</param>
		/// <param name="right">The second string to compare.</param>
		/// <param name="offsetRight">The position of the substring within <paramref name="right"/>.</param>
		/// <param name="length">The maximum number of characters in the substrings to compare.</param>
		/// <param name="ignoreCase"><c>true</c> to ignore case during the comparison; otherwise, <c>false</c>.</param>
		/// <param name="cultureInfo">An object that supplies culture-specific comparison information.</param>
		/// <returns>A 32-bit signed integer that indicates the lexical relationship between the two comparands.</returns>
		public static int Compare(string left, int offsetLeft, string right, int offsetRight, int length, bool ignoreCase, CultureInfo cultureInfo)
		{
			if (cultureInfo == null)
				throw new ArgumentNullException("cultureInfo");
			return cultureInfo.CompareInfo.Compare(left, offsetLeft, length, right, offsetRight, length, ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);
		}

		/// <summary>
		/// Creates a <see cref="StringComparer" /> ignoring or honoring the strings' case, and using culture-specific information
		/// to influence the comparison.
		/// </summary>
		/// <returns>The comparer.</returns>
		/// <param name="cultureInfo">Culture info.</param>
		/// <param name="ignoreCase">If set to <c>true</c> ignore case.</param>
		public static StringComparer CreateComparer(CultureInfo cultureInfo, bool ignoreCase)
		{
			// PCL only supports culture-aware string comparers for the current culture
			if (cultureInfo == CultureInfo.CurrentCulture)
				return ignoreCase ? StringComparer.CurrentCultureIgnoreCase : StringComparer.CurrentCulture;

			// use reflection in case we are running on a platform that supports StringComparer.Create
			MethodInfo create = typeof(StringComparer)
				.GetTypeInfo()
				.GetDeclaredMethods("Create")
				.FirstOrDefault(x => x.IsStatic && x.GetParameters().Select(p => p.ParameterType).SequenceEqual(new[] { typeof(CultureInfo), typeof(bool) }));
			if (create != null)
				return (StringComparer) create.Invoke(null, new object[] { cultureInfo, ignoreCase });

			// return comparer that implements Compare and Equals but not GetHashCode
			return new PortableStringComparer(cultureInfo, ignoreCase);
		}

		/// <summary>
		/// Performs a full case folding as defined by Section 5.18 of the Unicode Standard 5.0.
		/// </summary>
		/// <param name="value">The string to be case-folded.</param>
		/// <returns>A case-folded version of the input string. This value may be longer than the input.</returns>
		/// <remarks><para>This function is generated from the Unicode case folding data by Tools/src/GenerateCaseFolding.</para>
		/// <para>From http://unicode.org/reports/tr21/tr21-5.html#Caseless_Matching: Case-folding is the process of mapping
		/// strings to a canonical form where case differences are erased. Case-folding allows for fast caseless matches in lookups,
		/// since only binary comparison is required. Case-folding is more than just conversion to lowercase.</para>
		/// <para>Case folding is not culture aware. String comparisons that should be culture aware should not use this method.</para></remarks>
		public static string FoldCase(this string value)
		{
			// check parameters
			if (value == null)
				throw new ArgumentNullException("value");

			// process each character in the input string
			StringBuilder sb = new StringBuilder();
			foreach (char nCodeUnit in value)
			{
				if (nCodeUnit < 0x100)
				{
					if (nCodeUnit >= 0x0041 && nCodeUnit <= 0x005a)
						sb.Append((char) (nCodeUnit + 32));
					else if (nCodeUnit == 0x00b5)
						sb.Append('\u03bc');
					else if (nCodeUnit >= 0x00c0 && nCodeUnit <= 0x00d6)
						sb.Append((char) (nCodeUnit + 32));
					else if (nCodeUnit >= 0x00d8 && nCodeUnit <= 0x00de)
						sb.Append((char) (nCodeUnit + 32));
					else if (nCodeUnit == 0x00df)
						sb.Append("\u0073\u0073");
					else
						sb.Append(nCodeUnit);
				}
				else if (nCodeUnit < 0x590)
				{
					if (nCodeUnit >= 0x0100 && nCodeUnit <= 0x012e && nCodeUnit % 2 == 0)
						sb.Append((char) (nCodeUnit + 1));
					else if (nCodeUnit == 0x0130)
						sb.Append("\u0069\u0307");
					else if (nCodeUnit >= 0x0132 && nCodeUnit <= 0x0136 && nCodeUnit % 2 == 0)
						sb.Append((char) (nCodeUnit + 1));
					else if (nCodeUnit >= 0x0139 && nCodeUnit <= 0x0147 && nCodeUnit % 2 == 1)
						sb.Append((char) (nCodeUnit + 1));
					else if (nCodeUnit == 0x0149)
						sb.Append("\u02bc\u006e");
					else if (nCodeUnit >= 0x014a && nCodeUnit <= 0x0176 && nCodeUnit % 2 == 0)
						sb.Append((char) (nCodeUnit + 1));
					else if (nCodeUnit == 0x0178)
						sb.Append('\u00ff');
					else if (nCodeUnit >= 0x0179 && nCodeUnit <= 0x017d && nCodeUnit % 2 == 1)
						sb.Append((char) (nCodeUnit + 1));
					else if (nCodeUnit == 0x017f)
						sb.Append('\u0073');
					else if (nCodeUnit == 0x0181)
						sb.Append('\u0253');
					else if (nCodeUnit >= 0x0182 && nCodeUnit <= 0x0184 && nCodeUnit % 2 == 0)
						sb.Append((char) (nCodeUnit + 1));
					else if (nCodeUnit == 0x0186)
						sb.Append('\u0254');
					else if (nCodeUnit == 0x0187)
						sb.Append('\u0188');
					else if (nCodeUnit >= 0x0189 && nCodeUnit <= 0x018a)
						sb.Append((char) (nCodeUnit + 205));
					else if (nCodeUnit == 0x018b)
						sb.Append('\u018c');
					else if (nCodeUnit == 0x018e)
						sb.Append('\u01dd');
					else if (nCodeUnit == 0x018f)
						sb.Append('\u0259');
					else if (nCodeUnit == 0x0190)
						sb.Append('\u025b');
					else if (nCodeUnit == 0x0191)
						sb.Append('\u0192');
					else if (nCodeUnit == 0x0193)
						sb.Append('\u0260');
					else if (nCodeUnit == 0x0194)
						sb.Append('\u0263');
					else if (nCodeUnit == 0x0196)
						sb.Append('\u0269');
					else if (nCodeUnit == 0x0197)
						sb.Append('\u0268');
					else if (nCodeUnit == 0x0198)
						sb.Append('\u0199');
					else if (nCodeUnit == 0x019c)
						sb.Append('\u026f');
					else if (nCodeUnit == 0x019d)
						sb.Append('\u0272');
					else if (nCodeUnit == 0x019f)
						sb.Append('\u0275');
					else if (nCodeUnit >= 0x01a0 && nCodeUnit <= 0x01a4 && nCodeUnit % 2 == 0)
						sb.Append((char) (nCodeUnit + 1));
					else if (nCodeUnit == 0x01a6)
						sb.Append('\u0280');
					else if (nCodeUnit == 0x01a7)
						sb.Append('\u01a8');
					else if (nCodeUnit == 0x01a9)
						sb.Append('\u0283');
					else if (nCodeUnit == 0x01ac)
						sb.Append('\u01ad');
					else if (nCodeUnit == 0x01ae)
						sb.Append('\u0288');
					else if (nCodeUnit == 0x01af)
						sb.Append('\u01b0');
					else if (nCodeUnit >= 0x01b1 && nCodeUnit <= 0x01b2)
						sb.Append((char) (nCodeUnit + 217));
					else if (nCodeUnit >= 0x01b3 && nCodeUnit <= 0x01b5 && nCodeUnit % 2 == 1)
						sb.Append((char) (nCodeUnit + 1));
					else if (nCodeUnit == 0x01b7)
						sb.Append('\u0292');
					else if (nCodeUnit >= 0x01b8 && nCodeUnit <= 0x01bc && nCodeUnit % 4 == 0)
						sb.Append((char) (nCodeUnit + 1));
					else if (nCodeUnit == 0x01c4)
						sb.Append('\u01c6');
					else if (nCodeUnit == 0x01c5)
						sb.Append('\u01c6');
					else if (nCodeUnit == 0x01c7)
						sb.Append('\u01c9');
					else if (nCodeUnit == 0x01c8)
						sb.Append('\u01c9');
					else if (nCodeUnit == 0x01ca)
						sb.Append('\u01cc');
					else if (nCodeUnit >= 0x01cb && nCodeUnit <= 0x01db && nCodeUnit % 2 == 1)
						sb.Append((char) (nCodeUnit + 1));
					else if (nCodeUnit >= 0x01de && nCodeUnit <= 0x01ee && nCodeUnit % 2 == 0)
						sb.Append((char) (nCodeUnit + 1));
					else if (nCodeUnit == 0x01f0)
						sb.Append("\u006a\u030c");
					else if (nCodeUnit == 0x01f1)
						sb.Append('\u01f3');
					else if (nCodeUnit >= 0x01f2 && nCodeUnit <= 0x01f4 && nCodeUnit % 2 == 0)
						sb.Append((char) (nCodeUnit + 1));
					else if (nCodeUnit == 0x01f6)
						sb.Append('\u0195');
					else if (nCodeUnit == 0x01f7)
						sb.Append('\u01bf');
					else if (nCodeUnit >= 0x01f8 && nCodeUnit <= 0x021e && nCodeUnit % 2 == 0)
						sb.Append((char) (nCodeUnit + 1));
					else if (nCodeUnit == 0x0220)
						sb.Append('\u019e');
					else if (nCodeUnit >= 0x0222 && nCodeUnit <= 0x0232 && nCodeUnit % 2 == 0)
						sb.Append((char) (nCodeUnit + 1));
					else if (nCodeUnit == 0x023a)
						sb.Append('\u2c65');
					else if (nCodeUnit == 0x023b)
						sb.Append('\u023c');
					else if (nCodeUnit == 0x023d)
						sb.Append('\u019a');
					else if (nCodeUnit == 0x023e)
						sb.Append('\u2c66');
					else if (nCodeUnit == 0x0241)
						sb.Append('\u0242');
					else if (nCodeUnit == 0x0243)
						sb.Append('\u0180');
					else if (nCodeUnit == 0x0244)
						sb.Append('\u0289');
					else if (nCodeUnit == 0x0245)
						sb.Append('\u028c');
					else if (nCodeUnit >= 0x0246 && nCodeUnit <= 0x024e && nCodeUnit % 2 == 0)
						sb.Append((char) (nCodeUnit + 1));
					else if (nCodeUnit == 0x0345)
						sb.Append('\u03b9');
					else if (nCodeUnit == 0x0386)
						sb.Append('\u03ac');
					else if (nCodeUnit >= 0x0388 && nCodeUnit <= 0x038a)
						sb.Append((char) (nCodeUnit + 37));
					else if (nCodeUnit == 0x038c)
						sb.Append('\u03cc');
					else if (nCodeUnit >= 0x038e && nCodeUnit <= 0x038f)
						sb.Append((char) (nCodeUnit + 63));
					else if (nCodeUnit == 0x0390)
						sb.Append("\u03b9\u0308\u0301");
					else if (nCodeUnit >= 0x0391 && nCodeUnit <= 0x03a1)
						sb.Append((char) (nCodeUnit + 32));
					else if (nCodeUnit >= 0x03a3 && nCodeUnit <= 0x03ab)
						sb.Append((char) (nCodeUnit + 32));
					else if (nCodeUnit == 0x03b0)
						sb.Append("\u03c5\u0308\u0301");
					else if (nCodeUnit == 0x03c2)
						sb.Append('\u03c3');
					else if (nCodeUnit == 0x03d0)
						sb.Append('\u03b2');
					else if (nCodeUnit == 0x03d1)
						sb.Append('\u03b8');
					else if (nCodeUnit == 0x03d5)
						sb.Append('\u03c6');
					else if (nCodeUnit == 0x03d6)
						sb.Append('\u03c0');
					else if (nCodeUnit >= 0x03d8 && nCodeUnit <= 0x03ee && nCodeUnit % 2 == 0)
						sb.Append((char) (nCodeUnit + 1));
					else if (nCodeUnit == 0x03f0)
						sb.Append('\u03ba');
					else if (nCodeUnit == 0x03f1)
						sb.Append('\u03c1');
					else if (nCodeUnit == 0x03f4)
						sb.Append('\u03b8');
					else if (nCodeUnit == 0x03f5)
						sb.Append('\u03b5');
					else if (nCodeUnit == 0x03f7)
						sb.Append('\u03f8');
					else if (nCodeUnit == 0x03f9)
						sb.Append('\u03f2');
					else if (nCodeUnit == 0x03fa)
						sb.Append('\u03fb');
					else if (nCodeUnit >= 0x03fd && nCodeUnit <= 0x03ff)
						sb.Append((char) (nCodeUnit - 130));
					else if (nCodeUnit >= 0x0400 && nCodeUnit <= 0x040f)
						sb.Append((char) (nCodeUnit + 80));
					else if (nCodeUnit >= 0x0410 && nCodeUnit <= 0x042f)
						sb.Append((char) (nCodeUnit + 32));
					else if (nCodeUnit >= 0x0460 && nCodeUnit <= 0x0480 && nCodeUnit % 2 == 0)
						sb.Append((char) (nCodeUnit + 1));
					else if (nCodeUnit >= 0x048a && nCodeUnit <= 0x04be && nCodeUnit % 2 == 0)
						sb.Append((char) (nCodeUnit + 1));
					else if (nCodeUnit == 0x04c0)
						sb.Append('\u04cf');
					else if (nCodeUnit >= 0x04c1 && nCodeUnit <= 0x04cd && nCodeUnit % 2 == 1)
						sb.Append((char) (nCodeUnit + 1));
					else if (nCodeUnit >= 0x04d0 && nCodeUnit <= 0x0512 && nCodeUnit % 2 == 0)
						sb.Append((char) (nCodeUnit + 1));
					else if (nCodeUnit >= 0x0531 && nCodeUnit <= 0x0556)
						sb.Append((char) (nCodeUnit + 48));
					else if (nCodeUnit == 0x0587)
						sb.Append("\u0565\u0582");
					else
						sb.Append(nCodeUnit);
				}
				else if (nCodeUnit >= 0x10a0 && nCodeUnit <= 0x10c5)
				{
					sb.Append((char) (nCodeUnit + 7264));
				}
				else if (nCodeUnit >= 0x1e00)
				{
					if (nCodeUnit >= 0x1e00 && nCodeUnit <= 0x1e94 && nCodeUnit % 2 == 0)
						sb.Append((char) (nCodeUnit + 1));
					else if (nCodeUnit == 0x1e96)
						sb.Append("\u0068\u0331");
					else if (nCodeUnit == 0x1e97)
						sb.Append("\u0074\u0308");
					else if (nCodeUnit == 0x1e98)
						sb.Append("\u0077\u030a");
					else if (nCodeUnit == 0x1e99)
						sb.Append("\u0079\u030a");
					else if (nCodeUnit == 0x1e9a)
						sb.Append("\u0061\u02be");
					else if (nCodeUnit == 0x1e9b)
						sb.Append('\u1e61');
					else if (nCodeUnit >= 0x1ea0 && nCodeUnit <= 0x1ef8 && nCodeUnit % 2 == 0)
						sb.Append((char) (nCodeUnit + 1));
					else if (nCodeUnit >= 0x1f08 && nCodeUnit <= 0x1f0f)
						sb.Append((char) (nCodeUnit - 8));
					else if (nCodeUnit >= 0x1f18 && nCodeUnit <= 0x1f1d)
						sb.Append((char) (nCodeUnit - 8));
					else if (nCodeUnit >= 0x1f28 && nCodeUnit <= 0x1f2f)
						sb.Append((char) (nCodeUnit - 8));
					else if (nCodeUnit >= 0x1f38 && nCodeUnit <= 0x1f3f)
						sb.Append((char) (nCodeUnit - 8));
					else if (nCodeUnit >= 0x1f48 && nCodeUnit <= 0x1f4d)
						sb.Append((char) (nCodeUnit - 8));
					else if (nCodeUnit == 0x1f50)
						sb.Append("\u03c5\u0313");
					else if (nCodeUnit == 0x1f52)
						sb.Append("\u03c5\u0313\u0300");
					else if (nCodeUnit == 0x1f54)
						sb.Append("\u03c5\u0313\u0301");
					else if (nCodeUnit == 0x1f56)
						sb.Append("\u03c5\u0313\u0342");
					else if (nCodeUnit >= 0x1f59 && nCodeUnit <= 0x1f5f && nCodeUnit % 2 == 1)
						sb.Append((char) (nCodeUnit - 8));
					else if (nCodeUnit >= 0x1f68 && nCodeUnit <= 0x1f6f)
						sb.Append((char) (nCodeUnit - 8));
					else if (nCodeUnit == 0x1f80)
						sb.Append("\u1f00\u03b9");
					else if (nCodeUnit == 0x1f81)
						sb.Append("\u1f01\u03b9");
					else if (nCodeUnit == 0x1f82)
						sb.Append("\u1f02\u03b9");
					else if (nCodeUnit == 0x1f83)
						sb.Append("\u1f03\u03b9");
					else if (nCodeUnit == 0x1f84)
						sb.Append("\u1f04\u03b9");
					else if (nCodeUnit == 0x1f85)
						sb.Append("\u1f05\u03b9");
					else if (nCodeUnit == 0x1f86)
						sb.Append("\u1f06\u03b9");
					else if (nCodeUnit == 0x1f87)
						sb.Append("\u1f07\u03b9");
					else if (nCodeUnit == 0x1f88)
						sb.Append("\u1f00\u03b9");
					else if (nCodeUnit == 0x1f89)
						sb.Append("\u1f01\u03b9");
					else if (nCodeUnit == 0x1f8a)
						sb.Append("\u1f02\u03b9");
					else if (nCodeUnit == 0x1f8b)
						sb.Append("\u1f03\u03b9");
					else if (nCodeUnit == 0x1f8c)
						sb.Append("\u1f04\u03b9");
					else if (nCodeUnit == 0x1f8d)
						sb.Append("\u1f05\u03b9");
					else if (nCodeUnit == 0x1f8e)
						sb.Append("\u1f06\u03b9");
					else if (nCodeUnit == 0x1f8f)
						sb.Append("\u1f07\u03b9");
					else if (nCodeUnit == 0x1f90)
						sb.Append("\u1f20\u03b9");
					else if (nCodeUnit == 0x1f91)
						sb.Append("\u1f21\u03b9");
					else if (nCodeUnit == 0x1f92)
						sb.Append("\u1f22\u03b9");
					else if (nCodeUnit == 0x1f93)
						sb.Append("\u1f23\u03b9");
					else if (nCodeUnit == 0x1f94)
						sb.Append("\u1f24\u03b9");
					else if (nCodeUnit == 0x1f95)
						sb.Append("\u1f25\u03b9");
					else if (nCodeUnit == 0x1f96)
						sb.Append("\u1f26\u03b9");
					else if (nCodeUnit == 0x1f97)
						sb.Append("\u1f27\u03b9");
					else if (nCodeUnit == 0x1f98)
						sb.Append("\u1f20\u03b9");
					else if (nCodeUnit == 0x1f99)
						sb.Append("\u1f21\u03b9");
					else if (nCodeUnit == 0x1f9a)
						sb.Append("\u1f22\u03b9");
					else if (nCodeUnit == 0x1f9b)
						sb.Append("\u1f23\u03b9");
					else if (nCodeUnit == 0x1f9c)
						sb.Append("\u1f24\u03b9");
					else if (nCodeUnit == 0x1f9d)
						sb.Append("\u1f25\u03b9");
					else if (nCodeUnit == 0x1f9e)
						sb.Append("\u1f26\u03b9");
					else if (nCodeUnit == 0x1f9f)
						sb.Append("\u1f27\u03b9");
					else if (nCodeUnit == 0x1fa0)
						sb.Append("\u1f60\u03b9");
					else if (nCodeUnit == 0x1fa1)
						sb.Append("\u1f61\u03b9");
					else if (nCodeUnit == 0x1fa2)
						sb.Append("\u1f62\u03b9");
					else if (nCodeUnit == 0x1fa3)
						sb.Append("\u1f63\u03b9");
					else if (nCodeUnit == 0x1fa4)
						sb.Append("\u1f64\u03b9");
					else if (nCodeUnit == 0x1fa5)
						sb.Append("\u1f65\u03b9");
					else if (nCodeUnit == 0x1fa6)
						sb.Append("\u1f66\u03b9");
					else if (nCodeUnit == 0x1fa7)
						sb.Append("\u1f67\u03b9");
					else if (nCodeUnit == 0x1fa8)
						sb.Append("\u1f60\u03b9");
					else if (nCodeUnit == 0x1fa9)
						sb.Append("\u1f61\u03b9");
					else if (nCodeUnit == 0x1faa)
						sb.Append("\u1f62\u03b9");
					else if (nCodeUnit == 0x1fab)
						sb.Append("\u1f63\u03b9");
					else if (nCodeUnit == 0x1fac)
						sb.Append("\u1f64\u03b9");
					else if (nCodeUnit == 0x1fad)
						sb.Append("\u1f65\u03b9");
					else if (nCodeUnit == 0x1fae)
						sb.Append("\u1f66\u03b9");
					else if (nCodeUnit == 0x1faf)
						sb.Append("\u1f67\u03b9");
					else if (nCodeUnit == 0x1fb2)
						sb.Append("\u1f70\u03b9");
					else if (nCodeUnit == 0x1fb3)
						sb.Append("\u03b1\u03b9");
					else if (nCodeUnit == 0x1fb4)
						sb.Append("\u03ac\u03b9");
					else if (nCodeUnit == 0x1fb6)
						sb.Append("\u03b1\u0342");
					else if (nCodeUnit == 0x1fb7)
						sb.Append("\u03b1\u0342\u03b9");
					else if (nCodeUnit >= 0x1fb8 && nCodeUnit <= 0x1fb9)
						sb.Append((char) (nCodeUnit - 8));
					else if (nCodeUnit >= 0x1fba && nCodeUnit <= 0x1fbb)
						sb.Append((char) (nCodeUnit - 74));
					else if (nCodeUnit == 0x1fbc)
						sb.Append("\u03b1\u03b9");
					else if (nCodeUnit == 0x1fbe)
						sb.Append('\u03b9');
					else if (nCodeUnit == 0x1fc2)
						sb.Append("\u1f74\u03b9");
					else if (nCodeUnit == 0x1fc3)
						sb.Append("\u03b7\u03b9");
					else if (nCodeUnit == 0x1fc4)
						sb.Append("\u03ae\u03b9");
					else if (nCodeUnit == 0x1fc6)
						sb.Append("\u03b7\u0342");
					else if (nCodeUnit == 0x1fc7)
						sb.Append("\u03b7\u0342\u03b9");
					else if (nCodeUnit >= 0x1fc8 && nCodeUnit <= 0x1fcb)
						sb.Append((char) (nCodeUnit - 86));
					else if (nCodeUnit == 0x1fcc)
						sb.Append("\u03b7\u03b9");
					else if (nCodeUnit == 0x1fd2)
						sb.Append("\u03b9\u0308\u0300");
					else if (nCodeUnit == 0x1fd3)
						sb.Append("\u03b9\u0308\u0301");
					else if (nCodeUnit == 0x1fd6)
						sb.Append("\u03b9\u0342");
					else if (nCodeUnit == 0x1fd7)
						sb.Append("\u03b9\u0308\u0342");
					else if (nCodeUnit >= 0x1fd8 && nCodeUnit <= 0x1fd9)
						sb.Append((char) (nCodeUnit - 8));
					else if (nCodeUnit >= 0x1fda && nCodeUnit <= 0x1fdb)
						sb.Append((char) (nCodeUnit - 100));
					else if (nCodeUnit == 0x1fe2)
						sb.Append("\u03c5\u0308\u0300");
					else if (nCodeUnit == 0x1fe3)
						sb.Append("\u03c5\u0308\u0301");
					else if (nCodeUnit == 0x1fe4)
						sb.Append("\u03c1\u0313");
					else if (nCodeUnit == 0x1fe6)
						sb.Append("\u03c5\u0342");
					else if (nCodeUnit == 0x1fe7)
						sb.Append("\u03c5\u0308\u0342");
					else if (nCodeUnit >= 0x1fe8 && nCodeUnit <= 0x1fe9)
						sb.Append((char) (nCodeUnit - 8));
					else if (nCodeUnit >= 0x1fea && nCodeUnit <= 0x1feb)
						sb.Append((char) (nCodeUnit - 112));
					else if (nCodeUnit == 0x1fec)
						sb.Append('\u1fe5');
					else if (nCodeUnit == 0x1ff2)
						sb.Append("\u1f7c\u03b9");
					else if (nCodeUnit == 0x1ff3)
						sb.Append("\u03c9\u03b9");
					else if (nCodeUnit == 0x1ff4)
						sb.Append("\u03ce\u03b9");
					else if (nCodeUnit == 0x1ff6)
						sb.Append("\u03c9\u0342");
					else if (nCodeUnit == 0x1ff7)
						sb.Append("\u03c9\u0342\u03b9");
					else if (nCodeUnit >= 0x1ff8 && nCodeUnit <= 0x1ff9)
						sb.Append((char) (nCodeUnit - 128));
					else if (nCodeUnit >= 0x1ffa && nCodeUnit <= 0x1ffb)
						sb.Append((char) (nCodeUnit - 126));
					else if (nCodeUnit == 0x1ffc)
						sb.Append("\u03c9\u03b9");
					else if (nCodeUnit == 0x2126)
						sb.Append('\u03c9');
					else if (nCodeUnit == 0x212a)
						sb.Append('\u006b');
					else if (nCodeUnit == 0x212b)
						sb.Append('\u00e5');
					else if (nCodeUnit == 0x2132)
						sb.Append('\u214e');
					else if (nCodeUnit >= 0x2160 && nCodeUnit <= 0x216f)
						sb.Append((char) (nCodeUnit + 16));
					else if (nCodeUnit == 0x2183)
						sb.Append('\u2184');
					else if (nCodeUnit >= 0x24b6 && nCodeUnit <= 0x24cf)
						sb.Append((char) (nCodeUnit + 26));
					else if (nCodeUnit >= 0x2c00 && nCodeUnit <= 0x2c2e)
						sb.Append((char) (nCodeUnit + 48));
					else if (nCodeUnit == 0x2c60)
						sb.Append('\u2c61');
					else if (nCodeUnit == 0x2c62)
						sb.Append('\u026b');
					else if (nCodeUnit == 0x2c63)
						sb.Append('\u1d7d');
					else if (nCodeUnit == 0x2c64)
						sb.Append('\u027d');
					else if (nCodeUnit >= 0x2c67 && nCodeUnit <= 0x2c6b && nCodeUnit % 2 == 1)
						sb.Append((char) (nCodeUnit + 1));
					else if (nCodeUnit >= 0x2c75 && nCodeUnit <= 0x2c80 && nCodeUnit % 11 == 7)
						sb.Append((char) (nCodeUnit + 1));
					else if (nCodeUnit >= 0x2c82 && nCodeUnit <= 0x2ce2 && nCodeUnit % 2 == 0)
						sb.Append((char) (nCodeUnit + 1));
					else if (nCodeUnit == 0xfb00)
						sb.Append("\u0066\u0066");
					else if (nCodeUnit == 0xfb01)
						sb.Append("\u0066\u0069");
					else if (nCodeUnit == 0xfb02)
						sb.Append("\u0066\u006c");
					else if (nCodeUnit == 0xfb03)
						sb.Append("\u0066\u0066\u0069");
					else if (nCodeUnit == 0xfb04)
						sb.Append("\u0066\u0066\u006c");
					else if (nCodeUnit == 0xfb05)
						sb.Append("\u0073\u0074");
					else if (nCodeUnit == 0xfb06)
						sb.Append("\u0073\u0074");
					else if (nCodeUnit == 0xfb13)
						sb.Append("\u0574\u0576");
					else if (nCodeUnit == 0xfb14)
						sb.Append("\u0574\u0565");
					else if (nCodeUnit == 0xfb15)
						sb.Append("\u0574\u056b");
					else if (nCodeUnit == 0xfb16)
						sb.Append("\u057e\u0576");
					else if (nCodeUnit == 0xfb17)
						sb.Append("\u0574\u056d");
					else if (nCodeUnit >= 0xff21 && nCodeUnit <= 0xff3a)
						sb.Append((char) (nCodeUnit + 32));
					else
						sb.Append(nCodeUnit);
				}
				else
				{
					sb.Append(nCodeUnit);
				}
			}
			return sb.ToString();
		}

		/// <summary>
		/// Formats the string using the invariant culture.
		/// </summary>
		/// <param name="format">The format string.</param>
		/// <param name="args">The format arguments.</param>
		/// <returns>The formatted string.</returns>
		[StringFormatMethod("format")]
		public static string FormatInvariant(this string format, params object[] args)
		{
			return string.Format(CultureInfo.InvariantCulture, format, args);
		}

		/// <summary>
		/// Replaces all of the {0}, {1} etc. formatting elements with the empty string.
		/// </summary>
		/// <param name="format">The format string.</param>
		/// <returns>The formatted string.</returns>
		public static string EmptyFormat(this string format)
		{
			return s_regexFormatReplace.Value.Replace(format, string.Empty);
		}

		/// <summary>
		/// Joins the specified strings into one string.
		/// </summary>
		/// <param name="strings">The strings.</param>
		/// <returns>All of the strings concatenated with no separator.</returns>
		public static string Join(this IEnumerable<string> strings)
		{
			return Join(strings, default(string));
		}

		/// <summary>
		/// Joins the specified strings using the specified separator.
		/// </summary>
		/// <param name="strings">The strings.</param>
		/// <param name="separator">The separator. (The empty string is used if null.)</param>
		/// <returns>All of the strings concatenated with the specified separator.</returns>
		public static string Join(this IEnumerable<string> strings, string separator)
		{
			if (strings == null)
				throw new ArgumentNullException("strings");

			using (var enumerator = strings.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					var sb = new StringBuilder(enumerator.Current);
					while (enumerator.MoveNext())
					{
						sb.Append(separator);
						sb.Append(enumerator.Current);
					}
					return sb.ToString();
				}
			}

			return "";
		}

		/// <summary>
		/// Joins the specified strings using the specified separator.
		/// </summary>
		/// <param name="strings">The strings.</param>
		/// <param name="separator">The separator.</param>
		/// <returns>All of the strings concatenated with the specified separator.</returns>
		public static string Join(this IEnumerable<string> strings, char separator)
		{
			return Join(strings, new string(separator, 1));
		}

		/// <summary>
		/// Joins the specified strings into one string.
		/// </summary>
		/// <param name="array">The strings.</param>
		/// <returns>All of the strings concatenated with no separator.</returns>
		public static string Join(this string[] array)
		{
			return string.Join(string.Empty, array);
		}

		/// <summary>
		/// Joins the specified strings using the specified separator.
		/// </summary>
		/// <param name="array">The strings.</param>
		/// <param name="separator">The separator. (The empty string is used if null.)</param>
		/// <returns>All of the strings concatenated with the specified separator.</returns>
		public static string Join(this string[] array, string separator)
		{
			return string.Join(separator, array);
		}

		/// <summary>
		/// Joins the specified strings using the specified separator format.
		/// </summary>
		/// <param name="strings">The strings.</param>
		/// <param name="separatorFormat">The separator format, e.g. "{0}, {1}".</param>
		/// <returns>All of the strings concatenated with the specified separator format.</returns>
		public static string JoinFormat(this IEnumerable<string> strings, string separatorFormat)
		{
			if (separatorFormat == null)
				throw new ArgumentNullException("separatorFormat");

			return strings.Aggregate(string.Empty, (acc, src) => acc.Length == 0 ? src : separatorFormat.FormatInvariant(acc, src));
		}

		/// <summary>
		/// Enumerates the indices of all instances of the specified character in a string.
		/// </summary>
		/// <param name="source">The string to search.</param>
		/// /// <param name="character">The character to find.</param>
		/// <returns>The positions of all instances of the specified character in the string.</returns>
		public static IEnumerable<int> GetIndexesOf(this string source, char character)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			int nIndex = source.IndexOf(character);
			while (nIndex != -1)
			{
				yield return nIndex;
				nIndex = source.IndexOf(character, nIndex + 1);
			}
		}

		/// <summary>
		/// Gets a hash code for the specified string; this hash code is guaranteed not to change in future.
		/// </summary>
		/// <param name="value">The string to hash.</param>
		/// <returns>A hash code for the specified string</returns>
		/// <remarks>Based on SuperFastHash: http://www.azillionmonkeys.com/qed/hash.html</remarks>
		public static int GetPersistentHashCode(this string value)
		{
			unchecked
			{
				// check for degenerate input
				if (string.IsNullOrEmpty(value))
					return 0;

				int nLength = value.Length;
				uint nHash = (uint) nLength;
				uint nTemp;

				int nRemainder = nLength & 1;
				nLength >>= 1;

				// main loop
				int nIndex = 0;
				for (; nLength > 0; nLength--)
				{
					nHash += value[nIndex];
					nTemp = (uint) (value[nIndex + 1] << 11) ^ nHash;
					nHash = (nHash << 16) ^ nTemp;
					nIndex += 2;
					nHash += nHash >> 11;
				}

				// handle odd string length
				if (nRemainder == 1)
				{
					nHash += value[nIndex];
					nHash ^= nHash << 11;
					nHash += nHash >> 17;
				}

				// Force "avalanching" of final 127 bits
				nHash ^= nHash << 3;
				nHash += nHash >> 5;
				nHash ^= nHash << 4;
				nHash += nHash >> 17;
				nHash ^= nHash << 25;
				nHash += nHash >> 6;

				return (int) nHash;
			}
		}

		/// <summary>
		/// Reverses the specified string.
		/// </summary>
		/// <param name="value">The string to reverse.</param>
		/// <returns>The input string, reversed.</returns>
		/// <remarks>This method correctly reverses strings containing supplementary characters (which are encoded with two surrogate code units).</remarks>
		public static string Reverse(this string value)
		{
			if (value == null)
				throw new ArgumentNullException("value");

			// allocate a buffer to hold the output
			char[] achOutput = new char[value.Length];
			for (int nOutputIndex = 0, nInputIndex = value.Length - 1; nOutputIndex < value.Length; nOutputIndex++, nInputIndex--)
			{
				// check for surrogate pair
				if (value[nInputIndex] >= 0xDC00 && value[nInputIndex] <= 0xDFFF &&
					nInputIndex > 0 && value[nInputIndex - 1] >= 0xD800 && value[nInputIndex - 1] <= 0xDBFF)
				{
					// preserve the order of the surrogate pair code units
					achOutput[nOutputIndex + 1] = value[nInputIndex];
					achOutput[nOutputIndex] = value[nInputIndex - 1];
					nOutputIndex++;
					nInputIndex--;
				}
				else
				{
					achOutput[nOutputIndex] = value[nInputIndex];
				}
			}

			return new string(achOutput);
		}

		/// <summary>
		/// Splits the string on whitespace.
		/// </summary>
		/// <param name="value">The string.</param>
		/// <returns>The array of substrings.</returns>
		/// <remarks>See the documentation for string.Split for the white-space characters recognized by this method.</remarks>
		public static string[] SplitOnWhitespace(this string value)
		{
			return value.Split((char[]) null);
		}

		/// <summary>
		/// Splits the string on whitespace.
		/// </summary>
		/// <param name="value">The string.</param>
		/// <param name="options">The options.</param>
		/// <returns>The array of substrings.</returns>
		/// <remarks>See the documentation for string.Split for the white-space characters recognized by this method.</remarks>
		public static string[] SplitOnWhitespace(this string value, StringSplitOptions options)
		{
			return value.Split((char[]) null, options);
		}

		/// <summary>
		/// Replaces a string with a default value, if the string is null or empty.
		/// </summary>
		/// <param name="value">The string to test for contents.</param>
		/// <param name="defaultValue">The default string to replace with.</param>
		/// <returns>Either the tested string, or the default value, depending on the contents of the initial string.</returns>
		public static string DefaultIfEmpty(this string value, string defaultValue)
		{
			return value.IsNullOrEmpty() ? defaultValue : value;
		}

		/// <summary>
		/// Returns <paramref name="input"/>, truncated to <paramref name="length"/> characters if it is longer than <paramref name="length"/>.
		/// </summary>
		/// <param name="input">The string to truncate.</param>
		/// <param name="length">The maximum length of the returned string.</param>
		/// <returns>A substring of <paramref name="input"/> that is no longer than <paramref name="length"/>.</returns>
		/// <remarks>This method is not linguistically-aware, not Unicode-aware, and not suitable for display (generally, an ellipsis should be added).
		/// Carefully consider whether any new usages of this method should use a smarter method to shorten a string.</remarks>
		public static string Truncate(string input, int length)
		{
			return input == null ? null :
				input.Length > length ? input.Substring(0, length) :
				input;
		}

		/// <summary>
		/// Compresses a string.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns>null if text is null, empty byte[] if text is empty, otherwise the compressed byte array.</returns>
		public static byte[] Compress(string text)
		{
			if (text == null)
				return null;

			if (text.Length == 0)
				return new byte[0];

			using (MemoryStream stream = new MemoryStream())
			{
				using (TextWriter textWriter = CreateCompressingTextWriter(stream, Ownership.None))
					textWriter.Write(text);
				return stream.ToArray();
			}
		}

		/// <summary>
		/// Decompresses a compressed string.
		/// </summary>
		/// <param name="compressedText">The compressed string.</param>
		/// <returns>null if compressedText is null, empty string if compressedText length is 0, otherwise the decompressed text.</returns>
		/// <remarks>The compressed text should have been created with the Compress or CreateCompressingTextWriter methods.</remarks>
		public static string Decompress(byte[] compressedText)
		{
			if (compressedText == null)
				return null;

			if (compressedText.Length == 0)
				return string.Empty;

			const string invalidFormatMessage = "The compressed string has an invalid format.";
			try
			{
				using (MemoryStream stream = new MemoryStream(compressedText, 0, compressedText.Length, false))
				using (TextReader textReader = CreateDecompressingTextReader(stream, Ownership.None))
					return textReader.ReadToEnd();
			}
			catch (InvalidDataException x)
			{
				throw new FormatException(invalidFormatMessage, x);
			}
			catch (EndOfStreamException x)
			{
				throw new FormatException(invalidFormatMessage, x);
			}
			catch (DecoderFallbackException x)
			{
				throw new FormatException(invalidFormatMessage, x);
			}
#if __MOBILE__
			catch (IOException x)
			{
				throw new FormatException(invalidFormatMessage, x);
			}
#endif
		}

		/// <summary>
		/// Creates a TextWriter that writes compressed text to a stream that matches the format used by Compress.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="ownership">The ownership of the stream.</param>
		/// <returns>The TextWriter.</returns>
		/// <remarks>The stream must be seekable so that the stream header can be finalized once the compression
		/// is copmlete. The contents of the stream are not valid until the TextWriter has been closed.</remarks>
		public static TextWriter CreateCompressingTextWriter(Stream stream, Ownership ownership)
		{
			return new CompressingTextWriter(stream, ownership);
		}

		/// <summary>
		/// Creates a TextReader that reads compressed text from a stream that matches the format used by Compress.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="ownership">The ownership of the stream.</param>
		/// <returns>The TextReader.</returns>
		public static TextReader CreateDecompressingTextReader(Stream stream, Ownership ownership)
		{
			return new DecompressingTextReader(stream, ownership);
		}

		private sealed class CompressingTextWriter : TextWriter
		{
			public CompressingTextWriter(Stream stream, Ownership ownership)
				: base(CultureInfo.InvariantCulture)
			{
				if (stream == null)
					throw new ArgumentNullException("stream");
				if (!stream.CanWrite)
					throw new ArgumentException("Stream must support writing.", "stream");
				if (!stream.CanSeek)
					throw new ArgumentException("Stream must support seeking.", "stream");

				m_stream = stream;
				m_ownership = ownership;
				m_encoding = Encoding.UTF8;
				m_encoder = m_encoding.GetEncoder();
			}

			public override Encoding Encoding
			{
				get { return m_encoding; }
			}

			public override void Write(char value)
			{
				Write(new[] { value }, 0, 1);
			}

			public override void Write(char[] buffer, int index, int count)
			{
				VerifyNotDisposed();

				// loop until all characters are processed
				while (count > 0)
				{
					// convert characters to UTF-8
					int charsUsed;
					int bytesUsed;
					bool completed;
					byte[] bytes = new byte[m_encoding.GetMaxByteCount(count)];
					m_encoder.Convert(buffer, index, count, bytes, 0, bytes.Length, false, out charsUsed, out bytesUsed, out completed);

					// write UTF-8 to stream
					WriteToStream(bytes, bytesUsed);

					// continue loop
					index += charsUsed;
					count -= charsUsed;
				}
			}

			protected override void Dispose(bool disposing)
			{
				try
				{
					if (disposing && !m_isDisposed)
					{
						// only dispose once
						m_isDisposed = true;

						// flush encoder
						bool completed = false;
						while (!completed)
						{
							int charsUsed;
							int bytesUsed;
							byte[] bytes = new byte[m_encoding.GetMaxByteCount(0)];
							m_encoder.Convert(new char[0], 0, 0, bytes, 0, bytes.Length, true, out charsUsed, out bytesUsed, out completed);
							WriteToStream(bytes, bytesUsed);
						}

						if (m_holdingStream != null)
						{
							// write holding stream without compression
							int uncompressedByteCount = (int) m_holdingStream.Length;
							m_stream.WriteByte(c_compressedStringUsingUtf8);
							m_stream.Write(BitConverter.GetBytes(uncompressedByteCount), 0, 4);
							m_stream.Write(m_holdingStream.ToArray(), 0, uncompressedByteCount);
							DisposableUtility.Dispose(ref m_holdingStream);
						}
						else if (m_zipStream != null)
						{
							// flush and dispose compression stream
							DisposableUtility.Dispose(ref m_zipStream);

							// write uncompressed byte count to the header
							long endPosition = m_stream.Position;
							m_stream.Position = m_uncompressedByteCountSeekPosition;
							m_stream.Write(BitConverter.GetBytes(m_uncompressedByteCount), 0, 4);
							m_stream.Position = endPosition;
						}

						// flush or dispose the stream
						if (m_ownership == Ownership.Owns)
							m_stream.Dispose();
						else
							m_stream.Flush();
					}
				}
				finally
				{
					base.Dispose(disposing);
				}
			}

			private void WriteToStream(byte[] bytes, int count)
			{
				// make sure we're writing something
				if (count != 0)
				{
					// don't compress short strings
					m_uncompressedByteCount += count;
					if (m_zipStream == null && m_uncompressedByteCount < c_minimumByteCountToCompress)
					{
						if (m_holdingStream == null)
							m_holdingStream = new MemoryStream();
						m_holdingStream.Write(bytes, 0, count);
					}
					else
					{
						// write the header if we haven't already, storing the seek position of the uncompressed byte count
						if (m_zipStream == null)
						{
							// the first byte is the version; the next four bytes is the uncompressed byte count
							// (based on http://www.csharphelp.com/2007/09/compress-and-decompress-strings-in-c/)
							m_stream.WriteByte(c_compressedStringUsingGzip);
							m_uncompressedByteCountSeekPosition = m_stream.Position;
							m_stream.Write(BitConverter.GetBytes(0), 0, 4);
							m_zipStream = GzipUtility.CreateCompressingWriteStream(m_stream, Ownership.None);

							// write any held bytes
							if (m_holdingStream != null)
							{
								m_zipStream.Write(m_holdingStream.ToArray(), 0, (int) m_holdingStream.Length);
								DisposableUtility.Dispose(ref m_holdingStream);
							}
						}

						m_zipStream.Write(bytes, 0, count);
					}
				}
			}

			private void VerifyNotDisposed()
			{
				if (m_isDisposed)
					throw new ObjectDisposedException("CompressingTextWriter");
			}

			const int c_minimumByteCountToCompress = 512;

			readonly Stream m_stream;
			readonly Ownership m_ownership;
			readonly Encoding m_encoding;
			readonly Encoder m_encoder;
			MemoryStream m_holdingStream;
			Stream m_zipStream;
			long m_uncompressedByteCountSeekPosition;
			int m_uncompressedByteCount;
			bool m_isDisposed;
		}

		private sealed class DecompressingTextReader : TextReader
		{
			public DecompressingTextReader(Stream stream, Ownership ownership)
			{
				if (stream == null)
					throw new ArgumentNullException("stream");
				if (!stream.CanRead)
					throw new ArgumentException("Stream must support reading.", "stream");

				m_stream = stream;
				m_ownership = ownership;
				m_encoding = Encoding.UTF8;
				m_decoder = m_encoding.GetDecoder();

				ReadHeader();
			}

			public override int Peek()
			{
				VerifyNotDisposed();

				// read current character unless at end of stream
				return m_buffer == null ? -1 : m_buffer[m_bufferIndex];
			}

			public override int Read()
			{
				VerifyNotDisposed();

				// check for end of stream
				if (m_buffer == null)
					return -1;

				// read current character and advance
				int next = m_buffer[m_bufferIndex];
				AdvanceBufferIndex(1);
				return next;
			}

			public override int Read(char[] buffer, int index, int count)
			{
				VerifyNotDisposed();

				// check for end of stream
				if (m_buffer == null)
					return 0;

				// limit count to characters remaining in buffer
				int maxCount = m_bufferLength - m_bufferIndex;
				if (count > maxCount)
					count = maxCount;

				// copy characters to buffer and advance
				Array.Copy(m_buffer, m_bufferIndex, buffer, index, count);
				AdvanceBufferIndex(count);
				return count;
			}

			protected override void Dispose(bool disposing)
			{
				try
				{
					if (disposing && !m_isDisposed)
					{
						// only dispose once
						m_isDisposed = true;

						// flush and dispose decompression stream, if any
						DisposableUtility.Dispose(ref m_unzipStream);

						// flush or dispose the stream
						if (m_ownership == Ownership.Owns)
							m_stream.Dispose();
						else
							m_stream.Flush();
					}
				}
				finally
				{
					base.Dispose(disposing);
				}
			}

			private void ReadHeader()
			{
				// check the first byte
				int firstByte = m_stream.ReadByte();
				if (firstByte == -1)
				{
					// the stream is empty, which represents the empty string
				}
				else if (firstByte == c_compressedStringUsingGzip || firstByte == c_compressedStringUsingUtf8)
				{
					// read uncompressed byte count
					byte[] uncompressedByteCountBuffer = new byte[4];
					int bytesRead = m_stream.ReadBlock(uncompressedByteCountBuffer, 0, 4);
					if (bytesRead != 4)
						throw new InvalidDataException("The compressed string has a bad header.");
					m_uncompressedByteCount = BitConverter.ToInt32(uncompressedByteCountBuffer, 0);

					// if there is anything to decompress, start decompressing
					if (m_uncompressedByteCount != 0)
					{
						if (firstByte == c_compressedStringUsingGzip)
							m_unzipStream = GzipUtility.CreateDecompressingReadStream(m_stream, Ownership.None);
						ReadNextBuffer();
					}
				}
				else
				{
					throw new InvalidDataException("The compressed string has an invalid version.");
				}
			}

			private void AdvanceBufferIndex(int count)
			{
				m_bufferIndex += count;

				// read the next buffer when we get to the end
				if (m_bufferIndex == m_bufferLength)
					ReadNextBuffer();
			}

			private void ReadNextBuffer()
			{
				// decompress some UTF-8
				int byteCount = Math.Min(m_uncompressedByteCount - m_uncompressedByteIndex, 4096);
				byte[] bytes = new byte[byteCount];
				(m_unzipStream ?? m_stream).ReadExactly(bytes, 0, byteCount);
				m_uncompressedByteIndex += byteCount;

				// prepare character buffer
				m_buffer = new char[m_encoding.GetMaxCharCount(byteCount)];
				m_bufferIndex = 0;
				m_bufferLength = 0;

				// loop over bytes
				int byteIndex = 0;
				while (byteCount > 0)
				{
					// convert UTF-8 to characters
					int charsUsed;
					int bytesUsed;
					bool completed;
					bool flush = m_uncompressedByteIndex == m_uncompressedByteCount;
					m_decoder.Convert(bytes, byteIndex, byteCount, m_buffer, m_bufferLength, m_buffer.Length - m_bufferLength, flush, out bytesUsed, out charsUsed, out completed);
					byteIndex += bytesUsed;
					byteCount -= bytesUsed;
					m_bufferLength += charsUsed;
				}

				// if no characters are left, we are at the end of the stream
				if (m_bufferLength == 0)
					m_buffer = null;
			}

			private void VerifyNotDisposed()
			{
				if (m_isDisposed)
					throw new ObjectDisposedException("CompressingTextWriter");
			}

			readonly Stream m_stream;
			readonly Ownership m_ownership;
			readonly Encoding m_encoding;
			readonly Decoder m_decoder;
			Stream m_unzipStream;
			int m_uncompressedByteCount;
			int m_uncompressedByteIndex;
			char[] m_buffer;
			int m_bufferIndex;
			int m_bufferLength;
			bool m_isDisposed;
		}

		const int c_compressedStringUsingGzip = 1;
		const int c_compressedStringUsingUtf8 = 2;

		static readonly ReadOnlyCollection<int> s_mapUtf16FixUp = new ReadOnlyCollection<int>(new[]
		{
			0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0x2000, 0xf800, 0xf800, 0xf800, 0xf800
		});

		static readonly Lazy<Regex> s_regexFormatReplace = new Lazy<Regex>(() => new Regex("{[^}]+}"));
	}

	/// <summary>
	/// Culture-aware string comparer.
	/// </summary>
	internal sealed class PortableStringComparer : StringComparer
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Faithlife.Utility.PortableStringComparer"/> class.
		/// </summary>
		/// <param name="cultureInfo">Culture info.</param>
		/// <param name="ignoreCase">If set to <c>true</c> ignore case.</param>
		public PortableStringComparer(CultureInfo cultureInfo, bool ignoreCase)
		{
			m_cultureInfo = cultureInfo;
			m_ignoreCase = ignoreCase;
		}

		/// <summary>
		/// Compare the specified strings.
		/// </summary>
		public override int Compare(string x, string y)
		{
			if (object.ReferenceEquals(x, y))
				return 0;
			if (x == null)
				return -1;
			if (y == null)
				return 1;
			return m_cultureInfo.CompareInfo.Compare(x, y, m_ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);
		}

		/// <summary>
		/// Check the specified strings for equality.
		/// </summary>
		public override bool Equals(string x, string y)
		{
			return Compare(x, y) == 0;
		}

		/// <Docs>The object for which the hash code is to be returned.</Docs>
		/// <para>Returns a hash code for the specified object.</para>
		/// <returns>A hash code for the specified object.</returns>
		/// <summary>
		/// Gets the hash code.
		/// </summary>
		/// <param name="obj">Object.</param>
		public override int GetHashCode(string obj)
		{
			if (m_cultureInfo == CultureInfo.CurrentCulture)
				return m_ignoreCase ? obj.ToLower().GetHashCode() : obj.GetHashCode();

			// PCL doesn't support StringComparer.Create or any other way to get the hash code of a string for an arbitrary culture
			throw new NotSupportedException("StringComparer.GetHashCode only supported for current culture.");
		}

		readonly CultureInfo m_cultureInfo;
		readonly bool m_ignoreCase;
	}
}
