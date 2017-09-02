using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class ReadOnlyListAdapterTests
	{
		[Test]
		public void NullConstructorArgument()
		{
			Assert.Throws<ArgumentNullException>(() => new ReadOnlyListAdapter<int>(null));
		}

		[Test]
		public void Count()
		{
			var list = new List<int> { 1, 2, 3 };
			var readOnlyList = new ReadOnlyListAdapter<int>(list);
			Assert.AreEqual(3, readOnlyList.Count);
			list.Add(4);
			Assert.AreEqual(4, readOnlyList.Count);
		}

		[Test]
		public void Item()
		{
			var list = new List<int> { 1, 2, 3 };
			var readOnlyList = new ReadOnlyListAdapter<int>(list);
			Assert.AreEqual(2, readOnlyList[1]);
			Assert.Throws<ArgumentOutOfRangeException>(() => { var _ = readOnlyList[4]; });
		}

		[Test]
		public void Enumerate()
		{
			var list = new List<int> { 1, 2, 3 };
			var readOnlyList = new ReadOnlyListAdapter<int>(list);
			CollectionAssert.AreEqual(new[] { 1, 2, 3 }, readOnlyList);
		}
	}
}
