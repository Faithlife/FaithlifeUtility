using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class StringSegmentTests
	{
		[Test]
		public void TestConstructorStringSegmentStr()
		{
			var seg = new StringSegment("hey");
			Assert.AreEqual(seg.Source, "hey");
			Assert.AreEqual(seg.Offset, 0);
			Assert.AreEqual(seg.Length, 3);
		}

		[Test]
		public void TestConstructorStringSegmentStrNOffset()
		{
			var seg = new StringSegment("hey", 1);
			Assert.AreEqual(seg.Source, "hey");
			Assert.AreEqual(seg.Offset, 1);
			Assert.AreEqual(seg.Length, 2);
		}

		[Test]
		public void TestConstructorStringSegmentStrNOffsetNLength()
		{
			var seg = new StringSegment("hey", 1, 1);
			Assert.AreEqual(seg.Source, "hey");
			Assert.AreEqual(seg.Offset, 1);
			Assert.AreEqual(seg.Length, 1);
		}

		[Test]
		public void TestConstructorStringSegmentDegenerateStart()
		{
			var seg = new StringSegment("hey", 0, 0);
			Assert.AreEqual(seg.Source, "hey");
			Assert.AreEqual(seg.Offset, 0);
			Assert.AreEqual(seg.Length, 0);
		}

		[Test]
		public void TestConstructorStringSegmentDegenerateEnd()
		{
			var seg = new StringSegment("hey", 3, 0);
			Assert.AreEqual(seg.Source, "hey");
			Assert.AreEqual(seg.Offset, 3);
			Assert.AreEqual(seg.Length, 0);
		}

		[Test]
		public void TestConstructorStringSegmentDegenerateMiddle()
		{
			var seg = new StringSegment("hey", 1, 0);
			Assert.AreEqual(seg.Source, "hey");
			Assert.AreEqual(seg.Offset, 1);
			Assert.AreEqual(seg.Length, 0);
		}

		[Test]
		public void TestConstructorNull()
		{
			var seg = new StringSegment(null);
			Assert.AreEqual("", seg.Source);
			Assert.AreEqual(0, seg.Offset);
			Assert.AreEqual(0, seg.Length);
		}

		[Test]
		public void TestConstructorStringSegmentOutOfRange()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => { new StringSegment(null, 1); });
			Assert.Throws<ArgumentOutOfRangeException>(() => { new StringSegment(null, 1, 0); });
			Assert.Throws<ArgumentOutOfRangeException>(() => { new StringSegment(null, 1, 1); });
			Assert.Throws<ArgumentOutOfRangeException>(() => { new StringSegment("hey", 4, 0); });
			Assert.Throws<ArgumentOutOfRangeException>(() => { new StringSegment("hey", 3, 1); });
			Assert.Throws<ArgumentOutOfRangeException>(() => { new StringSegment("hey", 2, 2); });
			Assert.Throws<ArgumentOutOfRangeException>(() => { new StringSegment("hey", 1, 3); });
			Assert.Throws<ArgumentOutOfRangeException>(() => { new StringSegment("hey", -1, 2); });
			Assert.Throws<ArgumentOutOfRangeException>(() => { new StringSegment("hey", 2, -1); });
		}

		[Test]
		public void TestConstructorStringSegmentStrCapture()
		{
			var seg = new StringSegment("hey", Regex.Match("hey", "e"));
			Assert.AreEqual(seg.Source, "hey");
			Assert.AreEqual(seg.Offset, 1);
			Assert.AreEqual(seg.Length, 1);
		}

		[Test]
		public void TestIndexer()
		{
			var seg = new StringSegment("hey!", 1, 2);
			Assert.Throws<ArgumentOutOfRangeException>(() => { Assert.AreNotEqual(seg[-2], 'h'); });
			Assert.Throws<ArgumentOutOfRangeException>(() => { Assert.AreNotEqual(seg[-1], 'h'); });
			Assert.AreEqual(seg[0], 'e');
			Assert.AreEqual(seg[1], 'y');
			Assert.Throws<ArgumentOutOfRangeException>(() => { Assert.AreNotEqual(seg[2], '!'); });
			Assert.Throws<ArgumentOutOfRangeException>(() => { Assert.AreNotEqual(seg[3], '!'); });
		}

		[Test]
		public void TestAfter()
		{
			var seg = new StringSegment("hey", 1, 1).After();
			Assert.AreEqual(seg.Source, "hey");
			Assert.AreEqual(seg.Offset, 2);
			Assert.AreEqual(seg.Length, 1);
		}

		[Test]
		public void TestAppendToStringBuilder()
		{
			var seg = new StringSegment("hey", 1, 1);
			StringBuilder sb = new StringBuilder();
			seg.AppendToStringBuilder(sb);
			Assert.AreEqual("e", sb.ToString());
		}

		[Test]
		public void TestAppendToNullStringBuilder()
		{
			var seg = new StringSegment("hey", 1, 1);
			Assert.Throws<ArgumentNullException>(() => seg.AppendToStringBuilder(null!));
		}

		[Test]
		public void TestBefore()
		{
			var seg = new StringSegment("hey", 1, 1).Before();
			Assert.AreEqual(seg.Source, "hey");
			Assert.AreEqual(seg.Offset, 0);
			Assert.AreEqual(seg.Length, 1);
		}

		[Test]
		public void TestCompareSegASegB()
		{
			var segA = new StringSegment("Aaa", 0, 2);
			var segB = new StringSegment("Aaa", 1, 2);
			Assert.Greater(StringSegment.Compare(segA, segB, StringComparison.InvariantCulture), 0);
		}

		[Test]
		public void TestCompareSegASegBSc()
		{
			var segA = new StringSegment("Aaa", 0, 2);
			var segB = new StringSegment("Aaa", 1, 2);
			Assert.AreEqual(0, StringSegment.Compare(segA, segB, StringComparison.OrdinalIgnoreCase));
		}

		[Test]
		public void TestCompareOrdinal()
		{
			var segA = new StringSegment("Aaa", 0, 2);
			var segB = new StringSegment("Aaa", 1, 2);
			Assert.Less(StringSegment.CompareOrdinal(segA, segB), 0);
		}

		[Test]
		public void TestCompareDifferentLengths()
		{
			var segA = new StringSegment("This is a test", 5, 4);
			var segB = new StringSegment("This is a test", 5, 6);

			Assert.Less(segA.CompareTo(segB), 0);
			Assert.Less(StringSegment.Compare(segA, segB, StringComparison.Ordinal), 0);
			Assert.Less(StringSegment.CompareOrdinal(segA, segB), 0);
		}

		[Test]
		public void TestCompareTo()
		{
			var segA = new StringSegment("Aaa", 0, 2);
			var segB = new StringSegment("Aaa", 1, 2);
			Assert.Greater(segA.CompareTo(segB), 0);
		}

		[Test]
		public void TestCopyTo()
		{
			var seg = new StringSegment("hey", 1);
			char[] ach = { 'o', 'x' };
			seg.CopyTo(1, ach, 1, 1);
			CollectionAssert.AreEqual(new char[] { 'o', 'y' }, ach);
		}

		[Test]
		public void TestEqualsSelf()
		{
			var segA = new StringSegment("hey", 1);
			Assert.AreEqual((object) segA, (object) segA);
			Assert.AreEqual(segA, segA);
		}

		[Test]
		public void TestEqualsIdentical()
		{
			var segA = new StringSegment("hey", 1);
			var segB = new StringSegment("hey", 1);
			Assert.AreEqual((object) segA, (object) segB);
			Assert.AreEqual(segA, segB);
			Assert.IsTrue(segA == segB);
			Assert.IsFalse(segA != segB);
		}

		[Test]
		public void TestEqualsDifferentOffset()
		{
			var segA = new StringSegment("hey you", 2, 1);
			var segB = new StringSegment("hey you", 4, 1);
			Assert.AreEqual((object) segA, (object) segB);
			Assert.AreEqual(segA, segB);
			Assert.IsTrue(segA == segB);
			Assert.IsFalse(segA != segB);
		}

		[Test]
		public void TestEqualsDifferentOwner()
		{
			var segA = new StringSegment("hey", 2, 1);
			var segB = new StringSegment("you", 0, 1);
			Assert.AreEqual((object) segA, (object) segB);
			Assert.AreEqual(segA, segB);
			Assert.IsTrue(segA == segB);
			Assert.IsFalse(segA != segB);
		}

		[Test]
		public void TestNotEqualsDifferentLength()
		{
			var segA = new StringSegment("hey", 1, 1);
			var segB = new StringSegment("hey", 1, 2);
			Assert.AreNotEqual((object) segA, (object) segB);
			Assert.AreNotEqual(segA, segB);
			Assert.IsTrue(segA != segB);
			Assert.IsFalse(segA == segB);
		}

		[Test]
		public void TestNotEqualsDifferentText()
		{
			var segA = new StringSegment("hey", 1, 1);
			var segB = new StringSegment("hey", 2, 1);
			Assert.AreNotEqual((object) segA, (object) segB);
			Assert.AreNotEqual(segA, segB);
			Assert.IsTrue(segA != segB);
			Assert.IsFalse(segA == segB);
		}

		[Test]
		public void TestNotEqualsCase()
		{
			var segA = new StringSegment("AAA", 1, 1);
			var segB = new StringSegment("aaa", 2, 1);
			Assert.AreNotEqual((object) segA, (object) segB);
			Assert.AreNotEqual(segA, segB);
			Assert.IsTrue(segA != segB);
			Assert.IsFalse(segA == segB);
		}

		[Test]
		public void TestGetEnumerator()
		{
			string str = "";
			foreach (var ch in new StringSegment("hey", 1))
				str += ch;
#pragma warning disable CS8605 // Unboxing a possibly null value.
			foreach (char ch in (IEnumerable) new StringSegment("hey", 1))
#pragma warning restore CS8605 // Unboxing a possibly null value.
				str += ch;
			Assert.AreEqual("eyey", str);
		}

		[Test]
		public void TestGetHashCode()
		{
			Assert.AreEqual("ey".GetHashCode(), new StringSegment("hey", 1).GetHashCode());
			Assert.AreEqual("ey".GetHashCode(), new StringSegment("eye", 0, 2).GetHashCode());
		}

		[Test]
		public void TestIndexOfCh()
		{
			var seg = new StringSegment("hey you", 1, 5);
			Assert.AreEqual(-1, seg.IndexOf('h'));
			Assert.AreEqual(0, seg.IndexOf('e'));
			Assert.AreEqual(1, seg.IndexOf('y'));
			Assert.AreEqual(-1, seg.IndexOf('u'));
			Assert.AreEqual(-1, seg.IndexOf('x'));
		}

		[Test]
		public void TestIndexOfCharWithStartOffset()
		{
			var seg = new StringSegment("hey you", 1, 5);
			Assert.AreEqual(-1, seg.IndexOf('h', 0));
			Assert.AreEqual(0, seg.IndexOf('e', 0));
			Assert.AreEqual(-1, seg.IndexOf('e', 1));
			Assert.AreEqual(1, seg.IndexOf('y', 0));
			Assert.AreEqual(1, seg.IndexOf('y', 1));
			Assert.AreEqual(3, seg.IndexOf('y', 2));
			Assert.AreEqual(-1, seg.IndexOf('y', 4));
			Assert.AreEqual(-1, seg.IndexOf('u', 0));
			Assert.AreEqual(-1, seg.IndexOf('x', 0));
			Assert.Throws<ArgumentOutOfRangeException>(() => seg.IndexOf('h', -1));
			Assert.Throws<ArgumentOutOfRangeException>(() => seg.IndexOf('u', 5));
		}

		[Test]
		public void TestIndexOfStr()
		{
			var seg = new StringSegment("hey you", 1, 5);
			Assert.AreEqual(-1, seg.IndexOf("he", StringComparison.Ordinal));
			Assert.AreEqual(0, seg.IndexOf("ey", StringComparison.Ordinal));
			Assert.AreEqual(1, seg.IndexOf("y", StringComparison.Ordinal));
			Assert.AreEqual(-1, seg.IndexOf("ou", StringComparison.Ordinal));
			Assert.AreEqual(-1, seg.IndexOf("ox", StringComparison.Ordinal));
		}

		[Test]
		public void TestIndexOfAny()
		{
			var seg = new StringSegment("hey you", 1, 5);
			Assert.AreEqual(-1, seg.IndexOfAny(new char[] { 'h' }));
			Assert.AreEqual(0, seg.IndexOfAny(new char[] { 'h', 'e' }));
			Assert.AreEqual(1, seg.IndexOfAny('h', 'y', 'x'));
			Assert.AreEqual(-1, seg.IndexOfAny(new char[] { 'u', 'v' }));
			Assert.AreEqual(-1, seg.IndexOfAny(new char[] { 'h' }));
		}

		[Test]
		public void TestIntersect()
		{
			string str = "01234567";
			var segFull = new StringSegment(str);
			var segFirstHalf = new StringSegment(str, 0, 4);
			var segLastHalf = new StringSegment(str, 4);
			var segMiddle = new StringSegment(str, 2, 4);
			var segSecondChar = new StringSegment(str, 1, 1);
			var segLastChar = new StringSegment(str, 7, 1);
			var segCenter = new StringSegment(str, 4, 0);

			Assert.IsTrue(segFull.Intersect(segFull).IsIdenticalTo(segFull));
			Assert.IsTrue(segFull.Intersect(segFirstHalf).IsIdenticalTo(segFirstHalf));
			Assert.IsTrue(segFull.Intersect(segLastHalf).IsIdenticalTo(segLastHalf));
			Assert.IsTrue(segFull.Intersect(segMiddle).IsIdenticalTo(segMiddle));
			Assert.IsTrue(segFull.Intersect(segSecondChar).IsIdenticalTo(segSecondChar));
			Assert.IsTrue(segFull.Intersect(segLastChar).IsIdenticalTo(segLastChar));
			Assert.IsTrue(segFull.Intersect(segCenter).IsIdenticalTo(segCenter));

			Assert.IsTrue(segFirstHalf.Intersect(segFull).IsIdenticalTo(segFirstHalf));
			Assert.IsTrue(segFirstHalf.Intersect(segFirstHalf).IsIdenticalTo(segFirstHalf));
			Assert.IsTrue(segFirstHalf.Intersect(segLastHalf).IsIdenticalTo(segCenter));
			Assert.IsTrue(segFirstHalf.Intersect(segMiddle).IsIdenticalTo(new StringSegment(str, 2, 2)));
			Assert.IsTrue(segFirstHalf.Intersect(segSecondChar).IsIdenticalTo(segSecondChar));
			Assert.IsTrue(segFirstHalf.Intersect(segLastChar).IsIdenticalTo(new StringSegment(str, 7, 0)));
			Assert.IsTrue(segFirstHalf.Intersect(segCenter).IsIdenticalTo(segCenter));

			Assert.IsTrue(segLastHalf.Intersect(segFull).IsIdenticalTo(segLastHalf));
			Assert.IsTrue(segLastHalf.Intersect(segFirstHalf).IsIdenticalTo(segCenter));
			Assert.IsTrue(segLastHalf.Intersect(segLastHalf).IsIdenticalTo(segLastHalf));
			Assert.IsTrue(segLastHalf.Intersect(segMiddle).IsIdenticalTo(new StringSegment(str, 4, 2)));
			Assert.IsTrue(segLastHalf.Intersect(segSecondChar).IsIdenticalTo(segCenter));
			Assert.IsTrue(segLastHalf.Intersect(segLastChar).IsIdenticalTo(segLastChar));
			Assert.IsTrue(segLastHalf.Intersect(segCenter).IsIdenticalTo(segCenter));

			Assert.IsTrue(segMiddle.Intersect(segFull).IsIdenticalTo(segMiddle));
			Assert.IsTrue(segMiddle.Intersect(segFirstHalf).IsIdenticalTo(new StringSegment(str, 2, 2)));
			Assert.IsTrue(segMiddle.Intersect(segLastHalf).IsIdenticalTo(new StringSegment(str, 4, 2)));
			Assert.IsTrue(segMiddle.Intersect(segMiddle).IsIdenticalTo(segMiddle));
			Assert.IsTrue(segMiddle.Intersect(segSecondChar).IsIdenticalTo(new StringSegment(str, 2, 0)));
			Assert.IsTrue(segMiddle.Intersect(segLastChar).IsIdenticalTo(new StringSegment(str, 7, 0)));
			Assert.IsTrue(segMiddle.Intersect(segCenter).IsIdenticalTo(segCenter));

			Assert.IsTrue(segSecondChar.Intersect(segFull).IsIdenticalTo(segSecondChar));
			Assert.IsTrue(segSecondChar.Intersect(segFirstHalf).IsIdenticalTo(segSecondChar));
			Assert.IsTrue(segSecondChar.Intersect(segLastHalf).IsIdenticalTo(segCenter));
			Assert.IsTrue(segSecondChar.Intersect(segMiddle).IsIdenticalTo(new StringSegment(str, 2, 0)));
			Assert.IsTrue(segSecondChar.Intersect(segSecondChar).IsIdenticalTo(segSecondChar));
			Assert.IsTrue(segSecondChar.Intersect(segLastChar).IsIdenticalTo(new StringSegment(str, 7, 0)));
			Assert.IsTrue(segSecondChar.Intersect(segCenter).IsIdenticalTo(segCenter));

			Assert.IsTrue(segLastChar.Intersect(segFull).IsIdenticalTo(segLastChar));
			Assert.IsTrue(segLastChar.Intersect(segFirstHalf).IsIdenticalTo(new StringSegment(str, 7, 0)));
			Assert.IsTrue(segLastChar.Intersect(segLastHalf).IsIdenticalTo(segLastChar));
			Assert.IsTrue(segLastChar.Intersect(segMiddle).IsIdenticalTo(new StringSegment(str, 7, 0)));
			Assert.IsTrue(segLastChar.Intersect(segSecondChar).IsIdenticalTo(new StringSegment(str, 7, 0)));
			Assert.IsTrue(segLastChar.Intersect(segLastChar).IsIdenticalTo(segLastChar));
			Assert.IsTrue(segLastChar.Intersect(segCenter).IsIdenticalTo(new StringSegment(str, 7, 0)));

			Assert.IsTrue(segCenter.Intersect(segFull).IsIdenticalTo(segCenter));
			Assert.IsTrue(segCenter.Intersect(segFirstHalf).IsIdenticalTo(segCenter));
			Assert.IsTrue(segCenter.Intersect(segLastHalf).IsIdenticalTo(segCenter));
			Assert.IsTrue(segCenter.Intersect(segMiddle).IsIdenticalTo(segCenter));
			Assert.IsTrue(segCenter.Intersect(segSecondChar).IsIdenticalTo(segCenter));
			Assert.IsTrue(segCenter.Intersect(segLastChar).IsIdenticalTo(new StringSegment(str, 7, 0)));
			Assert.IsTrue(segCenter.Intersect(segCenter).IsIdenticalTo(segCenter));
		}

		[Test]
		public void TestIntersectDifferent()
		{
			Assert.Throws<ArgumentException>(() => new StringSegment("hey", 1, 2).Intersect(new StringSegment("eye", 0, 2)));
		}

		[Test]
		public void TestLastIndexOfCh()
		{
			var seg = new StringSegment("hey you", 1, 5);
			Assert.AreEqual(-1, seg.LastIndexOf('h'));
			Assert.AreEqual(0, seg.LastIndexOf('e'));
			Assert.AreEqual(3, seg.LastIndexOf('y'));
			Assert.AreEqual(-1, seg.LastIndexOf('u'));
			Assert.AreEqual(-1, seg.LastIndexOf('x'));
		}

		[Test]
		public void TestLastIndexOfStr()
		{
			var seg = new StringSegment("hey you", 1, 5);
			Assert.AreEqual(-1, seg.LastIndexOf("he", StringComparison.Ordinal));
			Assert.AreEqual(0, seg.LastIndexOf("ey", StringComparison.Ordinal));
			Assert.AreEqual(3, seg.LastIndexOf("y", StringComparison.Ordinal));
			Assert.AreEqual(-1, seg.LastIndexOf("ou", StringComparison.Ordinal));
			Assert.AreEqual(-1, seg.LastIndexOf("ox", StringComparison.Ordinal));
		}

		[Test]
		public void TestLastIndexOfAny()
		{
			var seg = new StringSegment("hey you", 1, 5);
			Assert.AreEqual(-1, seg.LastIndexOfAny(new char[] { 'h' }));
			Assert.AreEqual(0, seg.LastIndexOfAny(new char[] { 'h', 'e' }));
			Assert.AreEqual(3, seg.LastIndexOfAny('h', 'y', 'x'));
			Assert.AreEqual(-1, seg.LastIndexOfAny(new char[] { 'u', 'v' }));
			Assert.AreEqual(-1, seg.LastIndexOfAny(new char[] { 'h' }));
		}

		[Test]
		public void TestMatch()
		{
			string str = "hey you";
			var seg = new StringSegment(str, 1, 5);
			Regex regex = new Regex("[bcdfghjklmnpqrstvwxyz]");
			Assert.IsTrue(new StringSegment(str, seg.Match(regex)).IsIdenticalTo(new StringSegment(str, 2, 1)));
		}

		[Test]
		public void TestMatchNull()
		{
			string str = "hey you";
			var seg = new StringSegment(str, 1, 5);
			Regex? regex = null;
			Assert.Throws<ArgumentNullException>(() => seg.Match(regex!));
		}

		[Test]
		public void TestMatchAnchors()
		{
			string str = "hey you";
			var seg = new StringSegment(str, 4, 2);
			Regex regex = new Regex("^yo$");
			Assert.IsTrue(seg.Match(regex).Success);
		}

		[Test]
		public void TestRedirect()
		{
			string str = "hey you";
			var seg = new StringSegment(str, 1, 5);
			Assert.IsTrue(seg.Redirect(3).IsIdenticalTo(new StringSegment(str, 3)));
			Assert.IsTrue(seg.Redirect(3, 2).IsIdenticalTo(new StringSegment(str, 3, 2)));
			Assert.IsTrue(seg.Redirect(Regex.Match(str, "ey")).IsIdenticalTo(new StringSegment(str, 1, 2)));
		}

		[Test]
		public void TestRedirectNull()
		{
			string str = "hey you";
			var seg = new StringSegment(str, 1, 5);
			Capture? capture = null;
			Assert.Throws<ArgumentNullException>(() => seg.Redirect(capture!));
		}

		[Test]
		public void TestSplitSimple()
		{
			string str = "shiny happy people";
			var seg = new StringSegment(str);
			Regex regex = new Regex(@"\s", RegexOptions.CultureInvariant);
			List<StringSegment> listSplit = new List<StringSegment>(seg.Split(regex));
			Assert.AreEqual(3, listSplit.Count);
			Assert.AreEqual(new StringSegment(str, 0, 5), listSplit[0]);
			Assert.AreEqual(new StringSegment(str, 6, 5), listSplit[1]);
			Assert.AreEqual(new StringSegment(str, 12, 6), listSplit[2]);

			Assert.AreEqual(3, regex.Split(seg.ToString()).Length);
		}

		[Test]
		public void TestSplitSegment()
		{
			string str = "shiny happy people";
			var seg = new StringSegment(str, 1, 16);
			Regex regex = new Regex(@"\s", RegexOptions.CultureInvariant);
			List<StringSegment> listSplit = new List<StringSegment>(seg.Split(regex));
			Assert.AreEqual(3, listSplit.Count);
			Assert.AreEqual(new StringSegment(str, 1, 4), listSplit[0]);
			Assert.AreEqual(new StringSegment(str, 6, 5), listSplit[1]);
			Assert.AreEqual(new StringSegment(str, 12, 5), listSplit[2]);

			Assert.AreEqual(3, regex.Split(seg.ToString()).Length);
		}

		[Test]
		public void TestSplitEdges()
		{
			string str = "shiny happy people";
			var seg = new StringSegment(str, 5, 7);
			Regex regex = new Regex(@"\s", RegexOptions.CultureInvariant);
			List<StringSegment> listSplit = new List<StringSegment>(seg.Split(regex));
			Assert.AreEqual(3, listSplit.Count);
			Assert.AreEqual(new StringSegment(str, 5, 0), listSplit[0]);
			Assert.AreEqual(new StringSegment(str, 6, 5), listSplit[1]);
			Assert.AreEqual(new StringSegment(str, 12, 0), listSplit[2]);

			Assert.AreEqual(3, regex.Split(seg.ToString()).Length);
		}

		[Test]
		public void TestSplitTrivial()
		{
			string str = "shiny happy people";
			var seg = new StringSegment(str, 6, 5);
			Regex regex = new Regex(@"\s", RegexOptions.CultureInvariant);
			List<StringSegment> listSplit = new List<StringSegment>(seg.Split(regex));
			Assert.AreEqual(1, listSplit.Count);
			Assert.AreEqual(new StringSegment(str, 6, 5), listSplit[0]);

			Assert.AreEqual(1, regex.Split(seg.ToString()).Length);
		}

		[Test]
		public void TestSplitSegmentGroups()
		{
			string str = "shiny happy people";
			var seg = new StringSegment(str, 1, 16);
			Regex regex = new Regex(@"(\s)", RegexOptions.CultureInvariant);
			List<StringSegment> listSplit = new List<StringSegment>(seg.Split(regex));
			Assert.AreEqual(5, listSplit.Count);
			Assert.AreEqual(new StringSegment(str, 1, 4), listSplit[0]);
			Assert.AreEqual(new StringSegment(str, 5, 1), listSplit[1]);
			Assert.AreEqual(new StringSegment(str, 6, 5), listSplit[2]);
			Assert.AreEqual(new StringSegment(str, 11, 1), listSplit[3]);
			Assert.AreEqual(new StringSegment(str, 12, 5), listSplit[4]);

			Assert.AreEqual(5, regex.Split(seg.ToString()).Length);
		}

		[Test]
		public void TestSplitSegmentGroupsRightToLeft()
		{
			string str = "shiny happy people";
			var seg = new StringSegment(str, 1, 16);
			Regex regex = new Regex(@"(\s)", RegexOptions.CultureInvariant | RegexOptions.RightToLeft);
			List<StringSegment> listSplit = new List<StringSegment>(seg.Split(regex));
			Assert.AreEqual(5, listSplit.Count, "listSplit has incorrect count.");
			Assert.AreEqual(new StringSegment(str, 1, 4), listSplit[4]);
			Assert.AreEqual(new StringSegment(str, 5, 1), listSplit[3]);
			Assert.AreEqual(new StringSegment(str, 6, 5), listSplit[2]);
			Assert.AreEqual(new StringSegment(str, 11, 1), listSplit[1]);
			Assert.AreEqual(new StringSegment(str, 12, 5), listSplit[0]);

			Assert.AreEqual(5, regex.Split(seg.ToString()).Length, "regex.Split() has incorrect length.");
		}

		[Test]
		public void TestSubstringNIndex()
		{
			string str = "hey you";
			var seg = new StringSegment(str, 1, 5).Substring(1);
			Assert.IsTrue(seg.IsIdenticalTo(new StringSegment(str, 2, 4)));
		}

		[Test]
		public void TestSubstringNIndexNLength()
		{
			string str = "hey you";
			var seg = new StringSegment(str, 1, 5).Substring(1, 3);
			Assert.IsTrue(seg.IsIdenticalTo(new StringSegment(str, 2, 3)));
		}

		[Test]
		public void TestToCharArray()
		{
			CollectionAssert.AreEqual("hey".ToCharArray(), new StringSegment("hey").ToCharArray());
			CollectionAssert.AreEqual("ey".ToCharArray(), new StringSegment("hey", 1).ToCharArray());
			CollectionAssert.AreEqual("e".ToCharArray(), new StringSegment("hey", 1, 1).ToCharArray());
		}

		[Test]
		public void TestTrim()
		{
			string str = "one two\t three";

			Assert.AreEqual(new StringSegment(str), new StringSegment(str).Trim());

			Assert.AreEqual(new StringSegment(str, 1), new StringSegment(str, 1).Trim());
			Assert.AreEqual(new StringSegment(str, 2), new StringSegment(str, 2).Trim());
			Assert.AreEqual(new StringSegment(str, 4), new StringSegment(str, 3).Trim());
			Assert.AreEqual(new StringSegment(str, 4), new StringSegment(str, 4).Trim());

			Assert.AreEqual(new StringSegment(str, 0, 10), new StringSegment(str, 0, 10).Trim());
			Assert.AreEqual(new StringSegment(str, 0, 7), new StringSegment(str, 0, 9).Trim());
			Assert.AreEqual(new StringSegment(str, 0, 7), new StringSegment(str, 0, 8).Trim());
			Assert.AreEqual(new StringSegment(str, 0, 7), new StringSegment(str, 0, 7).Trim());

			Assert.AreEqual(new StringSegment(str, 4, 3), new StringSegment(str, 3, 6).Trim());
		}

		[Test]
		public void TestTrimStart()
		{
			string str = "one two\t three";

			Assert.AreEqual(new StringSegment(str), new StringSegment(str).TrimStart());

			Assert.AreEqual(new StringSegment(str, 1), new StringSegment(str, 1).TrimStart());
			Assert.AreEqual(new StringSegment(str, 2), new StringSegment(str, 2).TrimStart());
			Assert.AreEqual(new StringSegment(str, 4), new StringSegment(str, 3).TrimStart());
			Assert.AreEqual(new StringSegment(str, 4), new StringSegment(str, 4).TrimStart());

			Assert.AreEqual(new StringSegment(str, 0, 10), new StringSegment(str, 0, 10).TrimStart());
			Assert.AreEqual(new StringSegment(str, 0, 9), new StringSegment(str, 0, 9).TrimStart());
			Assert.AreEqual(new StringSegment(str, 0, 8), new StringSegment(str, 0, 8).TrimStart());
			Assert.AreEqual(new StringSegment(str, 0, 7), new StringSegment(str, 0, 7).TrimStart());

			Assert.AreEqual(new StringSegment(str, 4, 5), new StringSegment(str, 3, 6).TrimStart());
		}

		[Test]
		public void TestTrimEnd()
		{
			string str = "one two\t three";

			Assert.AreEqual(new StringSegment(str), new StringSegment(str).TrimEnd());

			Assert.AreEqual(new StringSegment(str, 1), new StringSegment(str, 1).TrimEnd());
			Assert.AreEqual(new StringSegment(str, 2), new StringSegment(str, 2).TrimEnd());
			Assert.AreEqual(new StringSegment(str, 3), new StringSegment(str, 3).TrimEnd());
			Assert.AreEqual(new StringSegment(str, 4), new StringSegment(str, 4).TrimEnd());

			Assert.AreEqual(new StringSegment(str, 0, 10), new StringSegment(str, 0, 10).TrimEnd());
			Assert.AreEqual(new StringSegment(str, 0, 7), new StringSegment(str, 0, 9).TrimEnd());
			Assert.AreEqual(new StringSegment(str, 0, 7), new StringSegment(str, 0, 8).TrimEnd());
			Assert.AreEqual(new StringSegment(str, 0, 7), new StringSegment(str, 0, 7).TrimEnd());

			Assert.AreEqual(new StringSegment(str, 3, 4), new StringSegment(str, 3, 6).TrimEnd());
		}

		[Test]
		public void TestTrimChar()
		{
			string str = "one two\t three";

			Assert.AreEqual(new StringSegment(str, 0, 12), new StringSegment(str).Trim('e'));

			Assert.AreEqual(new StringSegment(str, 1, 11), new StringSegment(str, 1).Trim('e'));
			Assert.AreEqual(new StringSegment(str, 3, 9), new StringSegment(str, 2).Trim('e'));
			Assert.AreEqual(new StringSegment(str, 3, 9), new StringSegment(str, 3).Trim('e'));
			Assert.AreEqual(new StringSegment(str, 4, 8), new StringSegment(str, 4).Trim('e'));

			Assert.AreEqual(new StringSegment(str, 0, 10), new StringSegment(str, 0, 10).Trim('e'));
			Assert.AreEqual(new StringSegment(str, 0, 9), new StringSegment(str, 0, 9).Trim('e'));
			Assert.AreEqual(new StringSegment(str, 0, 8), new StringSegment(str, 0, 8).Trim('e'));
			Assert.AreEqual(new StringSegment(str, 0, 7), new StringSegment(str, 0, 7).Trim('e'));

			Assert.AreEqual(new StringSegment(str, 3, 6), new StringSegment(str, 3, 6).Trim('e'));
		}

		[Test]
		public void TestTrimStartChar()
		{
			string str = "one two\t three";

			Assert.AreEqual(new StringSegment(str), new StringSegment(str).TrimStart('e'));

			Assert.AreEqual(new StringSegment(str, 1), new StringSegment(str, 1).TrimStart('e'));
			Assert.AreEqual(new StringSegment(str, 3), new StringSegment(str, 2).TrimStart('e'));
			Assert.AreEqual(new StringSegment(str, 3), new StringSegment(str, 3).TrimStart('e'));
			Assert.AreEqual(new StringSegment(str, 4), new StringSegment(str, 4).TrimStart('e'));

			Assert.AreEqual(new StringSegment(str, 0, 10), new StringSegment(str, 0, 10).TrimStart('e'));
			Assert.AreEqual(new StringSegment(str, 0, 9), new StringSegment(str, 0, 9).TrimStart('e'));
			Assert.AreEqual(new StringSegment(str, 0, 8), new StringSegment(str, 0, 8).TrimStart('e'));
			Assert.AreEqual(new StringSegment(str, 0, 7), new StringSegment(str, 0, 7).TrimStart('e'));

			Assert.AreEqual(new StringSegment(str, 3, 6), new StringSegment(str, 3, 6).TrimStart('e'));
		}

		[Test]
		public void TestTrimEndChar()
		{
			string str = "one two\t three";

			Assert.AreEqual(new StringSegment(str, 0, 12), new StringSegment(str).TrimEnd('e'));

			Assert.AreEqual(new StringSegment(str, 1, 11), new StringSegment(str, 1).TrimEnd('e'));
			Assert.AreEqual(new StringSegment(str, 2, 10), new StringSegment(str, 2).TrimEnd('e'));
			Assert.AreEqual(new StringSegment(str, 3, 9), new StringSegment(str, 3).TrimEnd('e'));
			Assert.AreEqual(new StringSegment(str, 4, 8), new StringSegment(str, 4).TrimEnd('e'));

			Assert.AreEqual(new StringSegment(str, 0, 10), new StringSegment(str, 0, 10).TrimEnd('e'));
			Assert.AreEqual(new StringSegment(str, 0, 9), new StringSegment(str, 0, 9).TrimEnd('e'));
			Assert.AreEqual(new StringSegment(str, 0, 8), new StringSegment(str, 0, 8).TrimEnd('e'));
			Assert.AreEqual(new StringSegment(str, 0, 7), new StringSegment(str, 0, 7).TrimEnd('e'));

			Assert.AreEqual(new StringSegment(str, 3, 6), new StringSegment(str, 3, 6).TrimEnd('e'));
		}

		[Test]
		public void TestToString()
		{
			Assert.AreEqual("hey", new StringSegment("hey").ToString());
			Assert.AreEqual("ey", new StringSegment("hey", 1).ToString());
			Assert.AreEqual("e", new StringSegment("hey", 1, 1).ToString());
		}

		[Test]
		public void TestToStringBuilder()
		{
			Assert.AreEqual("hey", new StringSegment("hey").ToStringBuilder().ToString());
			Assert.AreEqual("ey", new StringSegment("hey", 1).ToStringBuilder().ToString());
			Assert.AreEqual("e", new StringSegment("hey", 1, 1).ToStringBuilder().ToString());
		}

		[Test]
		public void TestToStringBuilderNCapacity()
		{
			Assert.AreEqual(10, new StringSegment("hey").ToStringBuilder(10).Capacity);
		}

		[Test]
		public void TestUnion()
		{
			string str = "01234567";
			var segFull = new StringSegment(str);
			var segFirstHalf = new StringSegment(str, 0, 4);
			var segLastHalf = new StringSegment(str, 4);
			var segMiddle = new StringSegment(str, 2, 4);
			var segSecondChar = new StringSegment(str, 1, 1);
			var segLastChar = new StringSegment(str, 7, 1);
			var segCenter = new StringSegment(str, 4, 0);

			Assert.IsTrue(segFull.Union(segFull).IsIdenticalTo(segFull));
			Assert.IsTrue(segFull.Union(segFirstHalf).IsIdenticalTo(segFull));
			Assert.IsTrue(segFull.Union(segLastHalf).IsIdenticalTo(segFull));
			Assert.IsTrue(segFull.Union(segMiddle).IsIdenticalTo(segFull));
			Assert.IsTrue(segFull.Union(segSecondChar).IsIdenticalTo(segFull));
			Assert.IsTrue(segFull.Union(segLastChar).IsIdenticalTo(segFull));
			Assert.IsTrue(segFull.Union(segCenter).IsIdenticalTo(segFull));

			Assert.IsTrue(segFirstHalf.Union(segFull).IsIdenticalTo(segFull));
			Assert.IsTrue(segFirstHalf.Union(segFirstHalf).IsIdenticalTo(segFirstHalf));
			Assert.IsTrue(segFirstHalf.Union(segLastHalf).IsIdenticalTo(segFull));
			Assert.IsTrue(segFirstHalf.Union(segMiddle).IsIdenticalTo(new StringSegment(str, 0, 6)));
			Assert.IsTrue(segFirstHalf.Union(segSecondChar).IsIdenticalTo(segFirstHalf));
			Assert.IsTrue(segFirstHalf.Union(segLastChar).IsIdenticalTo(segFull));
			Assert.IsTrue(segFirstHalf.Union(segCenter).IsIdenticalTo(segFirstHalf));

			Assert.IsTrue(segLastHalf.Union(segFull).IsIdenticalTo(segFull));
			Assert.IsTrue(segLastHalf.Union(segFirstHalf).IsIdenticalTo(segFull));
			Assert.IsTrue(segLastHalf.Union(segLastHalf).IsIdenticalTo(segLastHalf));
			Assert.IsTrue(segLastHalf.Union(segMiddle).IsIdenticalTo(new StringSegment(str, 2, 6)));
			Assert.IsTrue(segLastHalf.Union(segSecondChar).IsIdenticalTo(new StringSegment(str, 1, 7)));
			Assert.IsTrue(segLastHalf.Union(segLastChar).IsIdenticalTo(segLastHalf));
			Assert.IsTrue(segLastHalf.Union(segCenter).IsIdenticalTo(segLastHalf));

			Assert.IsTrue(segMiddle.Union(segFull).IsIdenticalTo(segFull));
			Assert.IsTrue(segMiddle.Union(segFirstHalf).IsIdenticalTo(new StringSegment(str, 0, 6)));
			Assert.IsTrue(segMiddle.Union(segLastHalf).IsIdenticalTo(new StringSegment(str, 2, 6)));
			Assert.IsTrue(segMiddle.Union(segMiddle).IsIdenticalTo(segMiddle));
			Assert.IsTrue(segMiddle.Union(segSecondChar).IsIdenticalTo(new StringSegment(str, 1, 5)));
			Assert.IsTrue(segMiddle.Union(segLastChar).IsIdenticalTo(new StringSegment(str, 2, 6)));
			Assert.IsTrue(segMiddle.Union(segCenter).IsIdenticalTo(segMiddle));

			Assert.IsTrue(segSecondChar.Union(segFull).IsIdenticalTo(segFull));
			Assert.IsTrue(segSecondChar.Union(segFirstHalf).IsIdenticalTo(segFirstHalf));
			Assert.IsTrue(segSecondChar.Union(segLastHalf).IsIdenticalTo(new StringSegment(str, 1, 7)));
			Assert.IsTrue(segSecondChar.Union(segMiddle).IsIdenticalTo(new StringSegment(str, 1, 5)));
			Assert.IsTrue(segSecondChar.Union(segSecondChar).IsIdenticalTo(segSecondChar));
			Assert.IsTrue(segSecondChar.Union(segLastChar).IsIdenticalTo(new StringSegment(str, 1, 7)));
			Assert.IsTrue(segSecondChar.Union(segCenter).IsIdenticalTo(new StringSegment(str, 1, 3)));

			Assert.IsTrue(segLastChar.Union(segFull).IsIdenticalTo(segFull));
			Assert.IsTrue(segLastChar.Union(segFirstHalf).IsIdenticalTo(segFull));
			Assert.IsTrue(segLastChar.Union(segLastHalf).IsIdenticalTo(segLastHalf));
			Assert.IsTrue(segLastChar.Union(segMiddle).IsIdenticalTo(new StringSegment(str, 2, 6)));
			Assert.IsTrue(segLastChar.Union(segSecondChar).IsIdenticalTo(new StringSegment(str, 1, 7)));
			Assert.IsTrue(segLastChar.Union(segLastChar).IsIdenticalTo(segLastChar));
			Assert.IsTrue(segLastChar.Union(segCenter).IsIdenticalTo(segLastHalf));

			Assert.IsTrue(segCenter.Union(segFull).IsIdenticalTo(segFull));
			Assert.IsTrue(segCenter.Union(segFirstHalf).IsIdenticalTo(segFirstHalf));
			Assert.IsTrue(segCenter.Union(segLastHalf).IsIdenticalTo(segLastHalf));
			Assert.IsTrue(segCenter.Union(segMiddle).IsIdenticalTo(segMiddle));
			Assert.IsTrue(segCenter.Union(segSecondChar).IsIdenticalTo(new StringSegment(str, 1, 3)));
			Assert.IsTrue(segCenter.Union(segLastChar).IsIdenticalTo(segLastHalf));
			Assert.IsTrue(segCenter.Union(segCenter).IsIdenticalTo(segCenter));
		}

		[Test]
		public void TestUnionDifferent()
		{
			Assert.Throws<ArgumentException>(() => new StringSegment("hey", 1, 2).Union(new StringSegment("eye", 0, 2)));
		}

		[Test]
		public void TestDefaultStringSegment()
		{
			var segment = default(StringSegment);

			Assert.AreEqual(segment.ToString(), "");
		}
	}

	public static class StringSegmentTestsExtensions
	{
		public static bool IsIdenticalTo(this StringSegment segA, StringSegment segB) => segA.Length == segB.Length && segA.Offset == segB.Offset && segA.Source == segB.Source;
	}
}
