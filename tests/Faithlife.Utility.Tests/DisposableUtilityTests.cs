using System;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class DisposableUtilityTests
	{
		[Test]
		public void Dispose()
		{
			DisposableClass? d = new DisposableClass();
			var dCopy = d;

			Assert.IsFalse(d.IsDisposed);
			DisposableUtility.Dispose(ref d);
			Assert.IsNull(d);
			Assert.IsTrue(dCopy.IsDisposed);
		}

		[Test]
		public void DisposeAfter()
		{
			var d = new DisposableClass();
			Assert.IsFalse(d.IsDisposed);
			Assert.IsFalse(d.DisposeAfter(d2 => d2.IsDisposed));
			Assert.IsTrue(d.IsDisposed);
		}

		[Test]
		public void DisposeDisposableObject()
		{
			var d = new DisposableClass();

			DisposableClass? d2 = d;
			DisposableUtility.DisposeObject(ref d2);
			Assert.IsNull(d2);
			Assert.IsTrue(d.IsDisposed);
		}

		[Test]
		public void DisposeNonDisposableObject()
		{
			object? obj = new object();
			DisposableUtility.DisposeObject(ref obj);
			Assert.IsNull(obj);
		}

		[Test]
		public void DisposeStruct()
		{
			var count = 0;
			var ds = new DisposableStruct(() => count++);

			// disposing should invoke the action
			DisposableUtility.DisposeObject(ref ds);
			Assert.AreEqual(1, count);

			// struct should have been replaced with default value, preventing second increment
			DisposableUtility.DisposeObject(ref ds);
			Assert.AreEqual(1, count);
		}

		private class DisposableClass : IDisposable
		{
			public void Dispose()
			{
				IsDisposed = true;
			}

			public bool IsDisposed { get; private set; }
		}

		private struct DisposableStruct : IDisposable
		{
			public DisposableStruct(Action disposeCallback)
			{
				m_disposeCallback = disposeCallback;
			}

			public void Dispose()
			{
				if (m_disposeCallback != null)
					m_disposeCallback();
			}

			private readonly Action m_disposeCallback;
		}
	}
}
