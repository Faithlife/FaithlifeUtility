using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class DisposablesTests
	{
		[Test]
		public void DisposeEmptyDoesNothing()
		{
			var disposables = new Disposables();
			disposables.Dispose();
		}

		[Test]
		public void DisposeSingleItem()
		{
			var disposed = false;
			using (var disposables = new Disposables())
			{
				disposables.Add(Scope.Create(() => disposed = true));
				Assert.IsFalse(disposed);
			}
			Assert.IsTrue(disposed);
		}

		[Test]
		public void DisposeInReverseOrder()
		{
			var test = "";
			using (var disposables = new Disposables())
			{
				disposables.Add(Scope.Create(() => test += "a"));
				disposables.Add(Scope.Create(() => test += "b"));
				disposables.Add(Scope.Create(() => test += "c"));
			}
			Assert.AreEqual("cba", test);
		}

		[Test]
		public void AddRangeEnumerable()
		{
			var test = "";
			using (var disposables = new Disposables())
			{
				disposables.AddRange(
					new List<IDisposable>
					{
						Scope.Create(() => test += "a"),
						Scope.Create(() => test += "b"),
						Scope.Create(() => test += "c"),
					});
			}
			Assert.AreEqual("cba", test);
		}

		[Test]
		public void CtorEnumerable()
		{
			var test = "";
			var disposables = new Disposables(new List<IDisposable>
			{
				Scope.Create(() => test += "a"),
				Scope.Create(() => test += "b"),
				Scope.Create(() => test += "c"),
			});
			disposables.Dispose();
			Assert.AreEqual("cba", test);
		}

		[Test]
		public void CollectionInitialization()
		{
			var test = "";
			var disposables = new Disposables
			{
				Scope.Create(() => test += "a"),
				Scope.Create(() => test += "b"),
				Scope.Create(() => test += "c"),
			};
			disposables.Dispose();
			Assert.AreEqual("cba", test);
		}

		[Test]
		public void IgnoreNull()
		{
			var test = "";
			using (var disposables = new Disposables())
			{
				disposables.Add(Scope.Create(() => test += "a"));
				disposables.Add(null);
				disposables.Add(Scope.Create(() => test += "c"));
			}
			Assert.AreEqual("ca", test);
		}

		[Test]
		public void OnlyDisposeOnce()
		{
			var counter = new DisposeCounter();
			var disposables = new Disposables();
			disposables.Add(counter);
			Assert.AreEqual(0, counter.DisposeCount);
			disposables.Dispose();
			Assert.AreEqual(1, counter.DisposeCount);
			disposables.Dispose();
			Assert.AreEqual(1, counter.DisposeCount);
		}

		[Test]
		public void NoAddAfterDispose()
		{
			var disposables = new Disposables();
			disposables.Dispose();
			Assert.Throws<ObjectDisposedException>(() => disposables.Add(Scope.Create(() => { })));
		}

		[Test]
		public void EnumerationNotSupported()
		{
			Assert.Throws<NotSupportedException>(() =>
			{
				foreach (IDisposable? disposable in new Disposables())
					Assert.IsNotNull(disposable);
			});
		}

		private sealed class DisposeCounter : IDisposable
		{
			public int DisposeCount { get; private set; }

			public void Dispose()
			{
				DisposeCount++;
			}
		}
	}
}
