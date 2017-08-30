using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class PeekEnumeratorTests
	{
		[Test]
		public void Test()
		{
			TestSequence(EnumerableUtility.Enumerate(1, 2, 3, 5, 7, 11, 13, 17, 19));
			TestSequence(EnumerableUtility.Enumerate("here", "we", "go", "here", "we", "go", "come", "on", "come", "on"));
		}

		private void TestSequence<T>(IEnumerable<T> seq)
		{
			IEnumerator<T> enumerator = seq.GetEnumerator();
			PeekEnumerator<T> peekEnumerator = new PeekEnumerator<T>(seq.GetEnumerator());

			TestEnumerators(peekEnumerator, enumerator);

			enumerator.Reset();
			peekEnumerator.Reset();

			TestEnumerators(peekEnumerator, enumerator);
		}

		private void TestEnumerators<T>(PeekEnumerator<T> peekEnumerator, IEnumerator<T> enumerator)
		{
			Assert.Throws<InvalidOperationException>(delegate { T item = enumerator.Current; });
			Assert.Throws<InvalidOperationException>(delegate { T item = peekEnumerator.Current; });
			while (peekEnumerator.HasNext)
			{
				T next = peekEnumerator.Next;
				Assert.IsTrue(peekEnumerator.MoveNext());
				Assert.IsTrue(enumerator.MoveNext());
				Assert.AreEqual(next, peekEnumerator.Current);
				Assert.AreEqual(next, enumerator.Current);
			}
			Assert.IsFalse(enumerator.MoveNext());
			Assert.IsFalse(peekEnumerator.MoveNext());
			Assert.Throws<InvalidOperationException>(delegate { T item = enumerator.Current; });
			Assert.Throws<InvalidOperationException>(delegate { T item = peekEnumerator.Current; });
		}
	}
}
