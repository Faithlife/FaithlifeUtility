using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class ReadOnlySetTests
	{
		[SetUp]
		public void SetUp()
		{
			m_set = new HashSet<int> { 1, 2, 3 }.AsReadOnly();
		}

		[Test]
		public void InvalidConstructorArgument()
		{
			Assert.Throws<ArgumentNullException>(() => new ReadOnlySet<int>(null));
		}

		[Test]
		public void IsReadOnly()
		{
			Assert.IsTrue(m_set.IsReadOnly);
		}

		[Test]
		public void ModifyingCollectionThrows()
		{
			ICollection<int> set = m_set;
			Assert.Throws<NotSupportedException>(() => set.Add(4));
			Assert.Throws<NotSupportedException>(() => set.Remove(1));
			Assert.Throws<NotSupportedException>(() => set.Clear());
		}

		[Test]
		public void ModifyingSetThrows()
		{
			ISet<int> set = m_set;
			Assert.Throws<NotSupportedException>(() => set.ExceptWith(m_set));
			Assert.Throws<NotSupportedException>(() => set.IntersectWith(m_set));
			Assert.Throws<NotSupportedException>(() => set.SymmetricExceptWith(m_set));
			Assert.Throws<NotSupportedException>(() => set.UnionWith(m_set));
		}

		[Test]
		public void Contains()
		{
			Assert.IsFalse(m_set.Contains(0));
			Assert.IsTrue(m_set.Contains(1));
		}

		[Test]
		public void Count()
		{
			Assert.AreEqual(3, m_set.Count);
		}

		[Test]
		public void IsSubsetOf()
		{
			Assert.IsTrue(m_set.IsSubsetOf(new[] { 1, 2, 3 }));
			Assert.IsTrue(m_set.IsSubsetOf(new[] { 1, 2, 3, 4 }));
			Assert.IsFalse(m_set.IsSubsetOf(new[] { 2, 3, 4 }));
		}

		[Test]
		public void IsProperSubsetOf()
		{
			Assert.IsFalse(m_set.IsProperSubsetOf(new[] { 1, 2, 3 }));
			Assert.IsTrue(m_set.IsProperSubsetOf(new[] { 1, 2, 3, 4 }));
			Assert.IsFalse(m_set.IsProperSubsetOf(new[] { 2, 3, 4 }));
		}

		[Test]
		public void IsSupersetOf()
		{
			Assert.IsTrue(m_set.IsSupersetOf(new[] { 1, 2, 3 }));
			Assert.IsFalse(m_set.IsSupersetOf(new[] { 1, 2, 3, 4 }));
			Assert.IsTrue(m_set.IsSupersetOf(new[] { 2, 3 }));
		}

		[Test]
		public void IsProperSupersetOf()
		{
			Assert.IsFalse(m_set.IsProperSupersetOf(new[] { 1, 2, 3 }));
			Assert.IsFalse(m_set.IsProperSupersetOf(new[] { 1, 2, 3, 4 }));
			Assert.IsTrue(m_set.IsProperSupersetOf(new[] { 2, 3 }));
		}

		[Test]
		public void Overlaps()
		{
			Assert.IsTrue(m_set.Overlaps(new[] { 2, 3, 4 }));
			Assert.IsFalse(m_set.Overlaps(new[] { 4, 5, 6 }));
		}

		[Test]
		public void SetEquals()
		{
			Assert.IsFalse(m_set.SetEquals(new[] { 2, 3, 4 }));
			Assert.IsTrue(m_set.SetEquals(new[] { 1, 2, 3 }));
		}

		ReadOnlySet<int> m_set;
	}
}
