using System;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class ObjectImplTests
	{
		[Test]
		public void OperatorEquality()
		{
			EquatableClass? eNull = null;
			EquatableClass e1 = new EquatableClass(1);
			EquatableClass e1b = new EquatableClass(1);
			EquatableClass e2 = new EquatableClass(2);

			Assert.IsTrue(ObjectImpl.OperatorEquality(eNull, null));
			Assert.IsTrue(ObjectImpl.OperatorEquality(e1, e1));
			Assert.IsTrue(ObjectImpl.OperatorEquality(e1, e1b));
			Assert.IsFalse(ObjectImpl.OperatorEquality(e1, eNull));
			Assert.IsFalse(ObjectImpl.OperatorEquality(eNull, e1));
			Assert.IsFalse(ObjectImpl.OperatorEquality(e1, e2));
		}

		[Test]
		public void OperatorInequality()
		{
			EquatableClass? eNull = null;
			EquatableClass e1 = new EquatableClass(1);
			EquatableClass e1b = new EquatableClass(1);
			EquatableClass e2 = new EquatableClass(2);

			Assert.IsFalse(ObjectImpl.OperatorInequality(eNull, null));
			Assert.IsFalse(ObjectImpl.OperatorInequality(e1, e1));
			Assert.IsFalse(ObjectImpl.OperatorInequality(e1, e1b));
			Assert.IsTrue(ObjectImpl.OperatorInequality(e1, eNull));
			Assert.IsTrue(ObjectImpl.OperatorInequality(eNull, e1));
			Assert.IsTrue(ObjectImpl.OperatorInequality(e1, e2));
		}

		public class EquatableClass : IEquatable<EquatableClass>
		{
			public EquatableClass(int i)
			{
				m_i = i;
			}

			public bool Equals(EquatableClass? other)
			{
				return other != null && m_i == other.m_i;
			}

			private int m_i;
		}
	}
}
