using System;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class CompareTests
	{
		[Test]
		public void CompareToTest()
		{
			var left = new MyComparable(1);
			var right = new MyComparable(2);
			Assert.IsTrue(left.CompareTo(right) < 0);
			Assert.IsTrue(left.CompareTo(left) == 0);
			Assert.IsTrue(right.CompareTo(left) > 0);
			Assert.IsTrue(left.CompareTo(null) > 0);
			Assert.IsTrue(((IComparable) left).CompareTo(right) < 0);
			Assert.IsTrue(((IComparable) left).CompareTo(left) == 0);
			Assert.IsTrue(((IComparable) right).CompareTo(left) > 0);
			Assert.IsTrue(((IComparable) left).CompareTo(null) > 0);
			Assert.Throws<ArgumentException>(() => { ((IComparable) left).CompareTo(""); });
		}

		[Test]
		public void EqualsTest()
		{
			var left = new MyComparable(1);
			var right = new MyComparable(2);
			var left2 = new MyComparable(1);
			Assert.IsFalse(left.Equals(right));
			Assert.IsTrue(left.Equals(left));
			Assert.IsFalse(right.Equals(left));
			Assert.IsTrue(left.Equals(left2));
			Assert.IsFalse(left.Equals(null));
			Assert.IsFalse(((object) left!).Equals(right));
			Assert.IsTrue(((object) left).Equals(left));
			Assert.IsFalse(((object) right).Equals(left));
			Assert.IsTrue(((object) left).Equals(left2));
			Assert.IsFalse(((object) left).Equals(null));
		}

		private class MyComparable : IComparable<MyComparable>, IComparable, IEquatable<MyComparable>
		{
			public MyComparable(int n)
			{
				m_n = n;
			}

			public int CompareTo(MyComparable? other)
			{
				return other is null ? 1 : m_n.CompareTo(other.m_n);
			}

			int IComparable.CompareTo(object? obj)
			{
				return ComparableImpl.CompareToObject(this, obj);
			}

			public bool Equals(MyComparable? other)
			{
				return other is null ? false : m_n == other.m_n;
			}

			public override bool Equals(object? obj)
			{
				return obj is MyComparable && Equals((MyComparable) obj);
			}

			public override int GetHashCode()
			{
				return m_n;
			}

			private readonly int m_n;
		}
	}
}
