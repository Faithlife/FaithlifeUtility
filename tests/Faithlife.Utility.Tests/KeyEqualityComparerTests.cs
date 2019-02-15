using System;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class KeyEqualityComparerTests
	{
		[Test]
		public void NullConstructor()
		{
			Assert.Throws<ArgumentNullException>(() => { new KeyEqualityComparer<int, int>(null); });
		}

		[TestCase(1, 1, true)]
		[TestCase(1, 2, false)]
		public void TestDefaultCompare(int x, int y, bool ret)
		{
			var keyComparer = new KeyEqualityComparer<int, int>(i => i);
			Assert.AreEqual(keyComparer.Equals(x, y), ret);
		}

		[TestCase(1, 1, true)]
		[TestCase(2, 4, true)]
		[TestCase(1, 2, false)]
		public void TestInts(int x, int y, bool ret)
		{
			var keyComparer = new KeyEqualityComparer<int, int>(i => i % 2);
			Assert.AreEqual(keyComparer.Equals(x, y), ret);
		}

		[TestCase("test", "test", true)]
		[TestCase("test", "test1", false)]
		[TestCase("test", "tes", false)]
		[TestCase("test", "TEST", true)]
		public void TestStrings(string str1, string str2, bool bExpected)
		{
			var keyComparer = new KeyEqualityComparer<string, string>(str => str.ToLower());
			Assert.AreEqual(keyComparer.Equals(str1, str2), bExpected);
		}

		[TestCase("test", "blah", true)]
		[TestCase("test", "blahplus", false)]
		[TestCase(" ", "t ", false)]
		[TestCase("", "", true)]
		public void TestStringAndInts(string str1, string str2, bool bExpected)
		{
			var keyComparer = new KeyEqualityComparer<string, int>(str => str.Length);
			Assert.AreEqual(keyComparer.Equals(str1, str2), bExpected);
		}

		[TestCase("test", 4)]
		[TestCase(null, 0)]
		[TestCase("a long string", 13)]
		public void GetHashCode(string strTest, int nHashCode)
		{
			var keyComparer = new KeyEqualityComparer<string, int>(str => str.Length);
			Assert.AreEqual(nHashCode, keyComparer.GetHashCode(strTest));
		}
	}
}
