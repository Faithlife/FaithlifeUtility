using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Faithlife.Utility
{
	/// <summary>
	/// Encapsulates a length of characters from a string starting at a particular offset.
	/// </summary>
	[SuppressMessage("Microsoft.Design", "CA1036:OverrideMethodsOnComparableTypes", Justification = "String does not support comparison operators.")]
	[StructLayout(LayoutKind.Auto)]
	public struct StringSegment :
		IEnumerable<char>,
		IEquatable<StringSegment>,
		IComparable<StringSegment>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="StringSegment"/> class.
		/// </summary>
		/// <param name="str">The owner string.</param>
		/// <remarks>Creates a segment that represents the entire owner string.</remarks>
		public StringSegment(string str)
			: this(str, 0, (str != null ? str.Length : 0))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StringSegment"/> class.
		/// </summary>
		/// <param name="str">The owner string.</param>
		/// <param name="nOffset">The offset of the segment.</param>
		/// <remarks>Creates a segment that starts at the specified offset and continues to the end
		/// of the owner string.</remarks>
		/// <exception cref="ArgumentOutOfRangeException">The offset is out of range.</exception>
		public StringSegment(string str, int nOffset)
			: this(str, nOffset, (str != null ? str.Length : 0) - nOffset)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StringSegment"/> class.
		/// </summary>
		/// <param name="str">The owner string.</param>
		/// <param name="nOffset">The offset of the segment.</param>
		/// <param name="nLength">The length of the segment.</param>
		/// <exception cref="ArgumentOutOfRangeException">The offset or length are out of range.</exception>
		public StringSegment(string str, int nOffset, int nLength)
		{
			int nStringLength = str != null ? str.Length : 0;
			if (nOffset < 0 || nOffset > nStringLength)
				throw new ArgumentOutOfRangeException("nOffset");
			if (nLength < 0 || nOffset + nLength > nStringLength)
				throw new ArgumentOutOfRangeException("nLength");

			m_strOwner = str;
			m_nOffset = nOffset;
			m_nLength = nLength;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StringSegment"/> class.
		/// </summary>
		/// <param name="str">The owner string.</param>
		/// <param name="capture">The <see cref="Capture" /> to represent.</param>
		public StringSegment(string str, Capture capture)
			: this(str, capture != null ? capture.Index : 0, capture != null ? capture.Length : 0)
		{
		}

		/// <summary>
		/// An empty segment of the empty string.
		/// </summary>
		public static readonly StringSegment Empty = new StringSegment(string.Empty);

		/// <summary>
		/// Gets the owner string.
		/// </summary>
		/// <value>The owner string of the segment.</value>
		public string Owner
		{
			get { return m_strOwner; }
		}

		/// <summary>
		/// Gets the offset into the owner string.
		/// </summary>
		/// <value>The offset into the owner string of the segment.</value>
		public int Offset
		{
			get { return m_nOffset; }
		}

		/// <summary>
		/// Gets the length of the segment.
		/// </summary>
		/// <value>The length of the segment.</value>
		public int Length
		{
			get { return m_nLength; }
		}

		/// <summary>
		/// Gets the <see cref="Char"/> with the specified index.
		/// </summary>
		/// <value>The character at the specified index.</value>
		[System.Runtime.CompilerServices.IndexerName("Chars")]
		public char this[int nIndex]
		{
			get
			{
				if (nIndex < 0 || nIndex >= m_nLength)
					throw new ArgumentOutOfRangeException("nIndex");
				return m_strOwner[m_nOffset + nIndex];
			}
		}

		/// <summary>
		/// Returns everything that follows this segment in the owner string.
		/// </summary>
		/// <returns>Everything that follows this segment in the owner string.</returns>
		public StringSegment After()
		{
			return new StringSegment(m_strOwner, m_nOffset + m_nLength, m_strOwner.Length - (m_nOffset + m_nLength));
		}

		/// <summary>
		/// Appends the segment to the specified <see cref="StringBuilder" />.
		/// </summary>
		/// <param name="sb">The <see cref="StringBuilder" />.</param>
		public void AppendToStringBuilder(StringBuilder sb)
		{
			if (sb == null)
				throw new ArgumentNullException("sb");
			sb.Append(m_strOwner, m_nOffset, m_nLength);
		}

		/// <summary>
		/// Returns everything that precedes this segment in the owner string.
		/// </summary>
		/// <returns>Everything that precedes this segment in the owner string.</returns>
		public StringSegment Before()
		{
			return new StringSegment(m_strOwner, 0, m_nOffset);
		}

		/// <summary>
		/// Compares two string segments.
		/// </summary>
		/// <param name="segA">The first segment.</param>
		/// <param name="segB">The second segment.</param>
		/// <returns>Zero, a positive integer, or a negative integer, if the first segment is
		/// equal to, greater than, or less than the second segment, respectively.</returns>
		[Obsolete("Use overload with StringComparison.")]
		public static int Compare(StringSegment segA, StringSegment segB)
		{
			return Compare(segA, segB, StringComparison.CurrentCulture);
		}

		/// <summary>
		/// Compares two string segments.
		/// </summary>
		/// <param name="segA">The first segment.</param>
		/// <param name="segB">The second segment.</param>
		/// <param name="sc">The string comparison options.</param>
		/// <returns>Zero, a positive integer, or a negative integer, if the first segment is
		/// equal to, greater than, or less than the second segment, respectively.</returns>
		public static int Compare(StringSegment segA, StringSegment segB, StringComparison sc)
		{
			int result = string.Compare(segA.Owner, segA.m_nOffset, segB.Owner, segB.m_nOffset, Math.Min(segA.m_nLength, segB.m_nLength), sc);
			return CompareHelper(result, segA, segB);
		}

		/// <summary>
		/// Compares two string segments ordinally.
		/// </summary>
		/// <param name="segA">The first segment.</param>
		/// <param name="segB">The second segment.</param>
		/// <returns>Zero, a positive integer, or a negative integer, if the first segment is
		/// equal to, greater than, or less than the second segment, respectively.</returns>
		public static int CompareOrdinal(StringSegment segA, StringSegment segB)
		{
			int result = string.CompareOrdinal(segA.Owner, segA.m_nOffset, segB.Owner, segB.m_nOffset, Math.Min(segA.m_nLength, segB.m_nLength));
			return CompareHelper(result, segA, segB);
		}

		/// <summary>
		/// Compares the current object with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>Zero, a positive integer, or a negative integer, if the first segment is
		/// equal to, greater than, or less than the second segment, respectively.</returns>
		public int CompareTo(StringSegment other)
		{
			return CultureInfo.CurrentCulture.CompareInfo.Compare(m_strOwner, m_nOffset, m_nLength,
				other.m_strOwner, other.m_nOffset, other.m_nLength, CompareOptions.None);
		}

		/// <summary>
		/// Copies the characters of the string segment to an array.
		/// </summary>
		/// <param name="nSourceIndex">The first character in the segment to copy.</param>
		/// <param name="achDestination">The destination array.</param>
		/// <param name="nDestinationIndex">The first index in the destination array.</param>
		/// <param name="nCount">The number of characters to copy.</param>
		public void CopyTo(int nSourceIndex, char[] achDestination, int nDestinationIndex, int nCount)
		{
			m_strOwner.CopyTo(m_nOffset + nSourceIndex, achDestination, nDestinationIndex, nCount);
		}

		/// <summary>
		/// Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <param name="obj">Another object to compare to.</param>
		/// <returns>true if obj and this instance are the same type and represent the same value; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			return obj is StringSegment && Equals((StringSegment) obj);
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
		public bool Equals(StringSegment other)
		{
			if (m_nLength != other.m_nLength)
				return false;

			if (object.ReferenceEquals(m_strOwner, other.m_strOwner) && m_nOffset == other.m_nOffset)
				return true;

			return CompareOrdinal(this, other) == 0;
		}

		/// <summary>
		/// Returns an enumerator that iterates through the characters of the string segment.
		/// </summary>
		/// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to
		/// iterate through the characters of the string segment.</returns>
		public IEnumerator<char> GetEnumerator()
		{
			for (int nChar = 0; nChar < m_nLength; nChar++)
				yield return m_strOwner[m_nOffset + nChar];
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
		/// <remarks>This hash code is identical to the hash code that results from
		/// calling <c>GetHashCode</c> on the result of <c>ToString</c>.</remarks>
		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		/// <summary>
		/// Returns the first index of the specified character in the string segment.
		/// </summary>
		/// <param name="ch">The character to find.</param>
		/// <returns>The first index of the specified character in the string segment,
		/// or -1 if the character cannot be found.</returns>
		public int IndexOf(char ch)
		{
			int nIndex = m_strOwner.IndexOf(ch, m_nOffset, m_nLength);
			return nIndex < 0 ? nIndex : nIndex - m_nOffset;
		}

		/// <summary>
		/// Returns the first index of the specified character in the string segment.
		/// </summary>
		/// <param name="ch">The character to find.</param>
		/// <param name="startIndex">The search starting position.</param>
		/// <returns>The first index of the specified character in the string segment,
		/// or -1 if the character cannot be found.</returns>
		public int IndexOf(char ch, int startIndex)
		{
			if (startIndex < 0 || startIndex >= m_nLength)
				throw new ArgumentOutOfRangeException("startIndex");

			int nIndex = m_strOwner.IndexOf(ch, m_nOffset + startIndex, m_nLength - startIndex);
			return nIndex < 0 ? nIndex : nIndex - m_nOffset;
		}

		/// <summary>
		/// Returns the first index of the specified string in the string segment.
		/// </summary>
		/// <param name="str">The string to find.</param>
		/// <returns>The first index of the specified string in the string segment,
		/// or -1 if the string cannot be found.</returns>
		[Obsolete("Use overload with StringComparison.")]
		public int IndexOf(string str)
		{
			return IndexOf(str, StringComparison.CurrentCulture);
		}

		/// <summary>
		/// Returns the first index of the specified string in the string segment.
		/// </summary>
		/// <param name="str">The string to find.</param>
		/// <param name="sc">The string comparison options.</param>
		/// <returns>The first index of the specified string in the string segment,
		/// or -1 if the string cannot be found.</returns>
		public int IndexOf(string str, StringComparison sc)
		{
			int nIndex = m_strOwner.IndexOf(str, m_nOffset, m_nLength, sc);
			return nIndex < 0 ? nIndex : nIndex - m_nOffset;
		}

		/// <summary>
		/// Returns the first index of any of the specified characters in the string segment.
		/// </summary>
		/// <param name="ach">The characters to find.</param>
		/// <returns>The first index of any of the specified characters in the string segment,
		/// or -1 if none of the characters cannot be found.</returns>
		public int IndexOfAny(params char[] ach)
		{
			int nIndex = m_strOwner.IndexOfAny(ach, m_nOffset, m_nLength);
			return nIndex < 0 ? nIndex : nIndex - m_nOffset;
		}

		/// <summary>
		/// Intersects this segment with another segment of the same string.
		/// </summary>
		/// <param name="seg">The segment to intersect.</param>
		/// <returns>A string segment that encapsulates the portion of the string
		/// contained by both string segments.</returns>
		/// <remarks>If the segments do not intersect, the segment will have a length
		/// of zero, but will be positioned at the offset of the end-most substring.</remarks>
		public StringSegment Intersect(StringSegment seg)
		{
			if (m_strOwner != seg.m_strOwner)
				throw new ArgumentException(OurMessages.Argument_SegmentFromDifferentString, "seg");
			if (seg.m_nOffset >= m_nOffset + m_nLength)
				return Redirect(seg.m_nOffset, 0);
			if (seg.m_nOffset >= m_nOffset)
				return Redirect(seg.m_nOffset, Math.Min(seg.m_nLength, m_nOffset + m_nLength - seg.m_nOffset));
			if (m_nOffset >= seg.m_nOffset + seg.m_nLength)
				return Redirect(m_nOffset, 0);
			return Redirect(m_nOffset, Math.Min(m_nLength, seg.m_nOffset + seg.m_nLength - m_nOffset));
		}

		/// <summary>
		/// Determines whether this segment is identical to the specified segment.
		/// </summary>
		/// <param name="seg">The other segment.</param>
		/// <returns><c>true</c> if the owner strings, offset, and length of this segment
		/// are exactly the same as the other segment.</returns>
		public bool IsIdenticalTo(StringSegment seg)
		{
			return m_nLength == seg.m_nLength && m_nOffset == seg.m_nOffset && m_strOwner == seg.m_strOwner;
		}

		/// <summary>
		/// Returns the last index of the specified character in the string segment.
		/// </summary>
		/// <param name="ch">The character to find.</param>
		/// <returns>The last index of the specified character in the string segment,
		/// or -1 if the character cannot be found.</returns>
		public int LastIndexOf(char ch)
		{
			int nIndex = m_strOwner.LastIndexOf(ch, m_nOffset + m_nLength - 1, m_nLength);
			return nIndex < 0 ? nIndex : nIndex - m_nOffset;
		}

		/// <summary>
		/// Returns the last index of the specified string in the string segment.
		/// </summary>
		/// <param name="str">The string to find.</param>
		/// <returns>The last index of the specified string in the string segment,
		/// or -1 if the string cannot be found.</returns>
		[Obsolete("Use overload with StringComparison.")]
		public int LastIndexOf(string str)
		{
			return LastIndexOf(str, StringComparison.CurrentCulture);
		}

		/// <summary>
		/// Returns the last index of the specified string in the string segment.
		/// </summary>
		/// <param name="str">The string to find.</param>
		/// <param name="sc">The string comparison options.</param>
		/// <returns>The last index of the specified string in the string segment,
		/// or -1 if the string cannot be found.</returns>
		public int LastIndexOf(string str, StringComparison sc)
		{
			int nIndex = m_strOwner.LastIndexOf(str, m_nOffset + m_nLength - 1, m_nLength, sc);
			return nIndex < 0 ? nIndex : nIndex - m_nOffset;
		}

		/// <summary>
		/// Returns the last index of any of the specified characters in the string segment.
		/// </summary>
		/// <param name="ach">The characters to find.</param>
		/// <returns>The last index of any of the specified characters in the string segment,
		/// or -1 if none of the characters cannot be found.</returns>
		public int LastIndexOfAny(params char[] ach)
		{
			int nIndex = m_strOwner.LastIndexOfAny(ach, m_nOffset + m_nLength - 1, m_nLength);
			return nIndex < 0 ? nIndex : nIndex - m_nOffset;
		}

		/// <summary>
		/// Matches the specified regex against the segment.
		/// </summary>
		/// <param name="regex">The regex to match.</param>
		/// <returns>The result of calling <c>regex.Match</c> on the segment.</returns>
		public Match Match(Regex regex)
		{
			if (regex == null)
				throw new ArgumentNullException("regex");
			return regex.Match(m_strOwner, m_nOffset, m_nLength);
		}

		/// <summary>
		/// Returns a new <see cref="StringSegment"/> with the same owner string.
		/// </summary>
		/// <param name="nOffset">Offset of the new string segment.</param>
		/// <returns>A new segment with the same owner string.</returns>
		public StringSegment Redirect(int nOffset)
		{
			return new StringSegment(m_strOwner, nOffset, m_strOwner.Length - nOffset);
		}

		/// <summary>
		/// Returns a new <see cref="StringSegment"/> with the same owner string.
		/// </summary>
		/// <param name="nOffset">Offset of the new string segment.</param>
		/// <param name="nLength">Length of the new string segment.</param>
		/// <returns>A new segment with the same owner string.</returns>
		public StringSegment Redirect(int nOffset, int nLength)
		{
			return new StringSegment(m_strOwner, nOffset, nLength);
		}

		/// <summary>
		/// Returns a new <see cref="StringSegment"/> with the same owner string.
		/// </summary>
		/// <param name="capture">The <see cref="Capture" /> to represent.</param>
		/// <returns>A new segment with the same owner string.</returns>
		public StringSegment Redirect(Capture capture)
		{
			if (capture == null)
				throw new ArgumentNullException("capture");
			return new StringSegment(m_strOwner, capture.Index, capture.Length);
		}

		/// <summary>
		/// Splits the string segment by the specified regular expression.
		/// </summary>
		/// <param name="regex">The regular expression.</param>
		/// <returns>A collection of string segments corresponding to the split.
		/// Consult the <c>Split</c> method of the <c>Regex</c> class
		/// for more information.</returns>
		public IEnumerable<StringSegment> Split(Regex regex)
		{
			// find first match
			Match match = Match(regex);
			if (!match.Success)
			{
				// match not found, so yield this and we're done
				yield return this;
				yield break;
			}

			// ensure not right-to-left
			if (!regex.RightToLeft)
			{
				// loop through matches
				int nResultOffset = m_nOffset;
				do
				{
					// yield segment before match
					yield return Redirect(nResultOffset, match.Index - nResultOffset);
					nResultOffset = match.Index + match.Length;

					// yield captures
					GroupCollection groups = match.Groups;
					for (int nGroup = 1; nGroup < groups.Count; nGroup++)
					{
						Group group = groups[nGroup];
						if (group.Success)
							yield return Redirect(group);
					}

					// next match
					match = match.NextMatch();
				}
				while (match.Success);

				// yield segment after last match
				yield return Redirect(nResultOffset, m_nOffset + m_nLength - nResultOffset);
			}
			else
			{
				// loop through matches right to left
				int nResultOffset = m_nOffset + m_nLength;
				do
				{
					// yield segment before match
					yield return Redirect(match.Index + match.Length, nResultOffset - (match.Index + match.Length));
					nResultOffset = match.Index;

					// yield captures
					GroupCollection groups = match.Groups;
					for (int nGroup = 1; nGroup < groups.Count; nGroup++)
					{
						Group group = groups[nGroup];
						if (group.Success)
							yield return Redirect(group);
					}

					// next match
					match = match.NextMatch();
				}
				while (match.Success);

				// yield segment after last match
				yield return Redirect(m_nOffset, nResultOffset - m_nOffset);
			}
		}

		/// <summary>
		/// Returns a sub-segment of this segment.
		/// </summary>
		/// <param name="nIndex">The start index into this segment.</param>
		/// <returns>A sub-segment of this segment</returns>
		public StringSegment Substring(int nIndex)
		{
			return Redirect(m_nOffset + nIndex, m_nLength - nIndex);
		}

		/// <summary>
		/// Returns a sub-segment of this segment.
		/// </summary>
		/// <param name="nIndex">The start index into this segment.</param>
		/// <param name="nLength">The length of the sub-segment.</param>
		/// <returns>A sub-segment of this segment</returns>
		public StringSegment Substring(int nIndex, int nLength)
		{
			return Redirect(m_nOffset + nIndex, nLength);
		}

		/// <summary>
		/// Trims the whitespace from the start and end of the string segment.
		/// </summary>
		/// <returns>A string segment with the whitespace trimmed.</returns>
		public StringSegment Trim()
		{
			return TrimHelper(c_nTrimTypeBoth);
		}

		/// <summary>
		/// Trims the specified characters from the start and end of the string segment.
		/// </summary>
		/// <param name="achTrim">The characters to trim.</param>
		/// <returns>A string segment with the whitespace trimmed.</returns>
		public StringSegment Trim(params char[] achTrim)
		{
			return TrimHelper(achTrim, c_nTrimTypeBoth);
		}

		/// <summary>
		/// Trims the specified characters from the end of the string segment.
		/// </summary>
		/// <returns>A string segment with the whitespace trimmed.</returns>
		public StringSegment TrimEnd()
		{
			return TrimHelper(c_nTrimTypeEnd);
		}

		/// <summary>
		/// Trims the whitespace from the end of the string segment.
		/// </summary>
		/// <param name="achTrim">The characters to trim.</param>
		/// <returns>A string segment with the whitespace trimmed.</returns>
		public StringSegment TrimEnd(params char[] achTrim)
		{
			return TrimHelper(achTrim, c_nTrimTypeEnd);
		}

		/// <summary>
		/// Trims the specified characters from the start of the string segment.
		/// </summary>
		/// <returns>A string segment with the whitespace trimmed.</returns>
		public StringSegment TrimStart()
		{
			return TrimHelper(c_nTrimTypeStart);
		}

		/// <summary>
		/// Trims the whitespace from the start of the string segment.
		/// </summary>
		/// <param name="achTrim">The characters to trim.</param>
		/// <returns>A string segment with the whitespace trimmed.</returns>
		public StringSegment TrimStart(params char[] achTrim)
		{
			return TrimHelper(achTrim, c_nTrimTypeStart);
		}

		/// <summary>
		/// Returns the array of characters represented by this string segment.
		/// </summary>
		/// <returns></returns>
		public char[] ToCharArray()
		{
			return m_strOwner.ToCharArray(m_nOffset, m_nLength);
		}

		/// <summary>
		/// Returns the string segment as a string.
		/// </summary>
		/// <returns>The string segment as a string.</returns>
		public override string ToString()
		{
			return m_strOwner == null ? "" : m_strOwner.Substring(m_nOffset, m_nLength);
		}

		/// <summary>
		/// Returns the string segment as a <see cref="StringBuilder" />.
		/// </summary>
		/// <returns>The string segment as a <see cref="StringBuilder" />.</returns>
		public StringBuilder ToStringBuilder()
		{
			return new StringBuilder(m_strOwner, m_nOffset, m_nLength, 0);
		}

		/// <summary>
		/// Returns the string segment as a <see cref="StringBuilder"/>.
		/// </summary>
		/// <param name="nCapacity">The capacity of the new <see cref="StringBuilder"/>.</param>
		/// <returns>The string segment as a <see cref="StringBuilder"/>.</returns>
		public StringBuilder ToStringBuilder(int nCapacity)
		{
			return new StringBuilder(m_strOwner, m_nOffset, m_nLength, nCapacity);
		}

		/// <summary>
		/// Returns the union of this segment with another segment of the same string.
		/// </summary>
		/// <param name="seg">Another segment of the same string.</param>
		/// <returns>A string segment that spans both string segments.</returns>
		/// <remarks>If the segments do not intersect, the segment will also include any
		/// characters between the two segments.</remarks>
		public StringSegment Union(StringSegment seg)
		{
			if (m_strOwner != seg.m_strOwner)
				throw new ArgumentException(OurMessages.Argument_SegmentFromDifferentString, "seg");
			int nStart = Math.Min(m_nOffset, seg.m_nOffset);
			int nEnd = Math.Max(m_nOffset + m_nLength, seg.m_nOffset + seg.m_nLength);
			return Redirect(nStart, nEnd - nStart);
		}

		/// <summary>
		/// Compares two string segments for equality.
		/// </summary>
		/// <param name="segA">The first string segment.</param>
		/// <param name="segB">The second string segment.</param>
		/// <returns><c>true</c> if the segments are equal; false otherwise.</returns>
		public static bool operator ==(StringSegment segA, StringSegment segB)
		{
			return segA.Equals(segB);
		}

		/// <summary>
		/// Compares two string segments for inequality.
		/// </summary>
		/// <param name="segA">The first string segment.</param>
		/// <param name="segB">The second string segment.</param>
		/// <returns><c>true</c> if the segments are not equal; false otherwise.</returns>
		public static bool operator !=(StringSegment segA, StringSegment segB)
		{
			return !segA.Equals(segB);
		}

		/// <summary>
		/// Returns an enumerator that iterates through the characters of the string segment.
		/// </summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate
		/// through the characters of the string segment.</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		const int c_nTrimTypeStart = 0;
		const int c_nTrimTypeEnd = 1;
		const int c_nTrimTypeBoth = 2;

		private StringSegment TrimHelper(int nTrimType)
		{
			int nStart = m_nOffset;
			int nEnd = m_nOffset + m_nLength;

			if (nTrimType != c_nTrimTypeEnd)
				while (nStart < nEnd)
					if (char.IsWhiteSpace(m_strOwner[nStart]))
						nStart++;
					else
						break;

			if (nTrimType != c_nTrimTypeStart)
				while (nStart < nEnd)
					if (char.IsWhiteSpace(m_strOwner[nEnd - 1]))
						nEnd--;
					else
						break;

			return Redirect(nStart, nEnd - nStart);
		}

		private StringSegment TrimHelper(char[] achTrim, int nTrimType)
		{
			int nStart = m_nOffset;
			int nEnd = m_nOffset + m_nLength;

			if (nTrimType != c_nTrimTypeEnd)
				while (nStart < nEnd)
					if (Array.IndexOf(achTrim, m_strOwner[nStart]) >= 0)
						nStart++;
					else
						break;

			if (nTrimType != c_nTrimTypeStart)
				while (nStart < nEnd)
					if (Array.IndexOf(achTrim, m_strOwner[nEnd - 1]) >= 0)
						nEnd--;
					else
						break;

			return Redirect(nStart, nEnd - nStart);
		}

		// Compares the string length if the string segments otherwise compare equal
		private static int CompareHelper(int result, StringSegment segA, StringSegment segB)
		{
			return result != 0 ? result : segA.m_nLength.CompareTo(segB.m_nLength);
		}

		readonly string m_strOwner;
		readonly int m_nOffset;
		readonly int m_nLength;
	}
}
