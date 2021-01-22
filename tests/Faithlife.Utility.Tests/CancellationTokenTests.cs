using System.Threading;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	// CancellationToken is built into .NET 4; these tests verify that the Rx implementation and the Mono
	// implementation exhibit the behavior we depend on.
	[TestFixture]
	public class CancellationTokenTests
	{
		[Test]
		public void Cancel()
		{
			using (var source = new CancellationTokenSource())
			{
				var token = source.Token;
				Assert.IsFalse(token.IsCancellationRequested);
				Assert.IsTrue(token.CanBeCanceled);

				source.Cancel();
				Assert.IsTrue(token.IsCancellationRequested);
			}
		}

		[Test]
		public void LinkedToken()
		{
			using (var source1 = new CancellationTokenSource())
			using (var source2 = new CancellationTokenSource())
			using (var sourceLinked = CancellationTokenSource.CreateLinkedTokenSource(source1.Token, source2.Token, CancellationToken.None))
			{
				Assert.IsTrue(source1.Token.CanBeCanceled);
				Assert.IsTrue(source2.Token.CanBeCanceled);
				Assert.IsTrue(sourceLinked.Token.CanBeCanceled);
			}
		}

		[Test]
		public void Register()
		{
			using (var source = new CancellationTokenSource())
			{
				var canceled = false;
				using (source.Token.Register(() => canceled = true))
				{
					Assert.IsFalse(canceled);
					source.Cancel();
					Assert.IsTrue(canceled);
				}
			}
		}

		[Test]
		public void RegisterCanceled()
		{
			using (var source = new CancellationTokenSource())
			{
				var canceled = false;
				source.Cancel();
				using (source.Token.Register(() => canceled = true))
				{
					Assert.IsTrue(canceled);
				}
			}
		}
	}
}
