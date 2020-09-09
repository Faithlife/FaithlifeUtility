using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class OptionaTests
	{
		[Test]
		public void TestEqualsSameValue()
		{
			DoTestEquals(new Optional<int>(1), new Optional<int>(1), true);
		}

		[Test]
		public void TestEqualsOptionalValueAndValue()
		{
			DoTestEquals(new Optional<int>(1), 1, true); // 1 is promoted
			DoTestObjectEquals(new Optional<int>(1), 1, false);
		}

		[Test]
		public void TestEqualsNoValue()
		{
			DoTestEquals(new Optional<int>(), new Optional<int>(), true);
		}

		[Test]
		public void TestEqualsValueAndNone()
		{
			DoTestEquals(new Optional<int>(1), new Optional<int>(), false);
		}

		[Test]
		public void TestEqualsOptionalNullAndOptionalNull()
		{
			DoTestEquals(new Optional<string?>(null), new Optional<string?>(null), true);
		}

		[Test]
		public void TestEqualsOptionalNullAndNull()
		{
			DoTestEquals(new Optional<string?>(null), null, true); // promotion
			DoTestObjectEquals(new Optional<string?>(null), null, false);
		}

		[Test]
		public void TestEqualsNoneAndNull()
		{
			DoTestEquals(new Optional<string>(), null, false);
		}

		[Test]
		public void TestEqualsNestedOptionalAndValue()
		{
			DoTestObjectEquals(new Optional<Optional<string>>(new Optional<string>("")), "", false);
		}

		[Test]
		public void TestEqualsNestedOptionalAndOptional()
		{
			DoTestEquals(new Optional<Optional<string>>(new Optional<string>("")), new Optional<string>(""), true); // promotion
			DoTestObjectEquals(new Optional<Optional<string>>(new Optional<string>("")), new Optional<string>(""), false);
		}

		[Test]
		public void TestEqualsOptionalOfDiffrerentTypes()
		{
			DoTestObjectEquals(new Optional<int?>(1), new Optional<int>(1), false);
		}

		[Test]
		public void TestValueThrowsForNoValue()
		{
			string? value = null;
			Assert.Throws<InvalidOperationException>(() => value = new Optional<string?>().Value);
			Assert.IsNull(value);
		}

		[Test]
		public void TestCastThrowsForNoValue()
		{
			string? value = null;
			Assert.Throws<InvalidOperationException>(() => value = (string?) new Optional<string?>());
			Assert.IsNull(value);
		}

		[Test]
		public void TestGetHashCodeReturnsZeroForNoValue()
		{
			var actual = new Optional<string>().GetHashCode();
			Assert.AreEqual(0, actual);
		}

		[Test]
		public void TestGetHashCodeReturnsZeroForNull()
		{
			var actual = new Optional<string?>(null).GetHashCode();
			Assert.AreEqual(0, actual);
		}

		[Test]
		public void TestToStringReturnsEmptyForNoValue()
		{
			string actual = new Optional<int>().ToString();
			Assert.AreEqual("", actual);
		}

		[Test]
		public void TestToStringReturnsEmptyForNull()
		{
			string actual = new Optional<string?>(null).ToString();
			Assert.AreEqual("", actual);
		}

		[Test]
		public void TestGetDefaultReturnsDefaultForNoValue()
		{
			string actual = new Optional<string>().GetValueOrDefault();
			Assert.AreEqual(default(string), actual);
		}

		[Test]
		public void TestGetDefaultReturnsProvidedDefaultForNoValue()
		{
			string actual = new Optional<string>().GetValueOrDefault(c_expectedDefault);
			Assert.AreEqual(c_expectedDefault, actual);
		}

		[Test]
		public void TestHasValueIsFalseForNoValue()
		{
			var actual = new Optional<string>().HasValue;
			Assert.AreEqual(false, actual);
		}

		[Test]
		public void TestValueReturnsValue()
		{
			string actual = new Optional<string>(c_expectedValue).Value;
			Assert.AreEqual(c_expectedValue, actual);
		}

		[Test]
		public void TestCastReturnsValue()
		{
			string actual = (string) new Optional<string>(c_expectedValue);
			Assert.AreEqual(c_expectedValue, actual);
		}

		[Test]
		public void TestGetHashCodeReturnsValue()
		{
			var actual = new Optional<string>(c_expectedValue).GetHashCode();
			var expected = c_expectedValue.GetHashCode();
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void TestToStringReturnsValue()
		{
			const int value = 12;
			string actual = new Optional<int>(value).ToString();
			string expected = value.ToString();
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void TestGetDefaultReturnsValue()
		{
			string actual = new Optional<string>(c_expectedValue).GetValueOrDefault();
			Assert.AreEqual(c_expectedValue, actual);
		}

		[Test]
		public void TestGetDefaultWithDefaultReturnsValue()
		{
			string actual = new Optional<string>(c_expectedValue).GetValueOrDefault(c_expectedDefault);
			Assert.AreEqual(c_expectedValue, actual);
		}

		[Test]
		public void TestHasValueIsTrueForValue()
		{
			var actual = new Optional<string>(c_expectedValue).HasValue;
			Assert.AreEqual(true, actual);
		}

		[Test]
		public void TestImplicitCastIsEqualToNewOptional()
		{
			var left = new Optional<string>(c_expectedValue);
			Optional<string> right = c_expectedValue;

			DoTestEquals(left, right, true);
		}

		private static void DoTestEquals<T>(T left, T right, bool expected)
		{
			// Ensure that .Equals is symmetric
			var actualLeftToRight = EqualityComparer<T>.Default.Equals(left, right);
			var actualRightToLeft = EqualityComparer<T>.Default.Equals(right, left);

			Assert.AreEqual(expected, actualLeftToRight);
			Assert.AreEqual(expected, actualRightToLeft);

			DoTestObjectEquals(left, right, expected);
		}

		private static void DoTestObjectEquals(object? left, object? right, bool expected)
		{
			// Ensure that .Equals is symmetric
			var actualLeftToRight = Equals(left, right);
			var actualRightToLeft = Equals(right, left);

			Assert.AreEqual(expected, actualLeftToRight);
			Assert.AreEqual(expected, actualRightToLeft);
		}

		private const string c_expectedValue = "Test Value";
		private const string c_expectedDefault = "Test Default";
	}
}
