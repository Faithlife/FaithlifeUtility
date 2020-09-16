using System;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class CollectionImplTests
	{
		[Test]
		public void NullArray()
		{
			Assert.Throws<ArgumentNullException>(() => CollectionImpl.CheckCopyToParameters(null!, 0, 0));
		}

		[Test]
		public void NegativeIndex()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => CollectionImpl.CheckCopyToParameters(new int[1], -1, 0));
		}

		[Test]
		public void NotEnoughSpace()
		{
			Assert.Throws<ArgumentException>(() => CollectionImpl.CheckCopyToParameters(new int[1], 0, 2));
		}

		[Test]
		public void MultiDimensionalArray()
		{
			Assert.Throws<ArgumentException>(() => CollectionImpl.CheckCopyToParameters(new int[1, 1], 0, 1));
		}

		[Test]
		public void NonZeroBasedArray()
		{
			var a = Array.CreateInstance(typeof(int), new int[] { 2 }, new int[] { 1 });
			Assert.Throws<ArgumentException>(() => CollectionImpl.CheckCopyToParameters(a, 1, 1));
		}

		[Test]
		public void Valid()
		{
			CollectionImpl.CheckCopyToParameters(new int[1], 0, 1);
			CollectionImpl.CheckCopyToParameters(new int[2], 1, 1);
			CollectionImpl.CheckCopyToParameters(new int[0], 0, 0);
		}
	}
}
