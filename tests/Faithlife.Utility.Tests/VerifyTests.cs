using System;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class VerifyTests
	{
		[Test]
		public void TestIsTrueTrue()
		{
			Verify.IsTrue(true);
		}

		[Test]
		public void TestIsTrueFalse()
		{
			Assert.Throws<InvalidOperationException>(() => Verify.IsTrue(false));
		}

		[Test]
		public void TestIsFalseTrue()
		{
			Assert.Throws<InvalidOperationException>(() => Verify.IsFalse(true));
		}

		[Test]
		public void TestIsFalseFalse()
		{
			Verify.IsFalse(false);
		}

		[Test]
		public void TestIsNullNull()
		{
			Verify.IsNull(null);
		}

		[Test]
		public void TestIsNullObject()
		{
			Assert.Throws<InvalidOperationException>(() => Verify.IsNull(new object()));
		}

		[Test]
		public void TestIsNotNullNull()
		{
			Assert.Throws<InvalidOperationException>(() => Verify.IsNotNull(null));
		}

		[Test]
		public void TestIsNotNullObject()
		{
			Verify.IsNotNull(new object());
		}

		[Test]
		public void TestAreSameSame()
		{
			var s = "test";
			Verify.AreSame(s, s);
		}

		[Test]
		public void TestAreSameDifferent()
		{
			var s1 = "test";
			var s2 = "test2";
			Assert.Throws<InvalidOperationException>(() => Verify.AreSame(s1, s2));
		}

		[Test]
		public void TestAreNotSameSame()
		{
			var s = "test";
			Assert.Throws<InvalidOperationException>(() => Verify.AreNotSame(s, s));
		}

		[Test]
		public void TestAreNotSameDifferent()
		{
			var s1 = "test";
			var s2 = "test2";
			Verify.AreNotSame(s1, s2);
		}
	}
}
