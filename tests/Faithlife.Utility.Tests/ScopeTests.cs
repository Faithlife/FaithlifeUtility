using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class ScopeTests
	{
		[Test]
		public void SimpleTest()
		{
			var bDone = false;
			using (Scope.Create(() => { bDone = true; }))
				Assert.IsFalse(bDone);
			Assert.IsTrue(bDone);
		}

		[Test]
		public void Transfer()
		{
			var bDone = false;
			Scope scope2;
			using (Scope scope = Scope.Create(() => { bDone = true; }))
			{
				Assert.IsFalse(bDone);
				scope2 = scope.Transfer();
				Assert.IsFalse(bDone);
			}
			Assert.IsFalse(bDone);
			scope2.Dispose();
			Assert.IsTrue(bDone);
		}

		[Test]
		public void ClosableTest()
		{
			MyClosable x = new MyClosable();
			using (Scope.Create(x.Close))
				Assert.IsFalse(x.IsClosed);
			Assert.IsTrue(x.IsClosed);
		}

		[Test]
		public void ScopeNull()
		{
			using (Scope.Create(null))
			{
			}
		}

		[Test]
		public void ScopeDoubleDispose()
		{
			MyClosable x = new MyClosable();
			Scope scope = Scope.Create(x.Close);
			Assert.IsFalse(x.IsClosed);
			scope.Dispose();
			Assert.IsTrue(x.IsClosed);
			scope.Dispose();
			Assert.IsTrue(x.IsClosed);
		}

		private class MyClosable
		{
			public bool IsClosed { get { return m_bClosed; } }
			public void Close() { m_bClosed = true; }
			public void AnotherClose() { m_bClosed = true; }
			private bool m_bClosed;
		}
	}
}
