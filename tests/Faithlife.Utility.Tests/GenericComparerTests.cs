using System;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class GenericComparerTests
	{
		[Test]
		public void NullConstructor()
		{
			Assert.Throws<ArgumentNullException>(() => { new GenericComparer<int>(null); });
		}

		[TestCase(2, 3)]
		[TestCase(2, 2)]
		[TestCase(4, 1)]
		public void TestCompare(int x, int y)
		{
			Func<int, int, int> compare = DoCompare;
			GenericComparer<int> gc = new GenericComparer<int>(compare);

			m_bCompareCalled = false;
			Assert.AreEqual(x - y, gc.Compare(x, y));
			Assert.IsTrue(m_bCompareCalled);
		}

		private int DoCompare(int x, int y)
		{
			m_bCompareCalled = true;
			return x - y;
		}

		bool m_bCompareCalled;
	}
}
