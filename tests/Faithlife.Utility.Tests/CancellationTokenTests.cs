using System.Threading;
using System.Threading.Tasks;
using Faithlife.Utility.Threading;
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
			using (CancellationTokenSource source = new CancellationTokenSource())
			{
				CancellationToken token = source.Token;
				Assert.IsFalse(token.IsCancellationRequested);
				Assert.IsTrue(token.CanBeCanceled);

				source.Cancel();
				Assert.IsTrue(token.IsCancellationRequested);
			}
		}

		[Test]
		public void LinkedToken()
		{
			using (CancellationTokenSource source1 = new CancellationTokenSource())
			using (CancellationTokenSource source2 = new CancellationTokenSource())
			using (CancellationTokenSource sourceLinked = CancellationTokenSource.CreateLinkedTokenSource(source1.Token, source2.Token, CancellationToken.None))
			{
				Assert.IsTrue(source1.Token.CanBeCanceled);
				Assert.IsTrue(source2.Token.CanBeCanceled);
				Assert.IsTrue(sourceLinked.Token.CanBeCanceled);
			}
		}

		[Test]
		public void Register()
		{
			using (CancellationTokenSource source = new CancellationTokenSource())
			{
				bool canceled = false;
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
			using (CancellationTokenSource source = new CancellationTokenSource())
			{
				bool canceled = false;
				source.Cancel();
				using (source.Token.Register(() => canceled = true))
				{
					Assert.IsTrue(canceled);
				}
			}
		}

		// TODO: fix this without using Thread.Sleep()
		//[Test]
		//public void DisposeRegistration()
		//{
		//	// try to cause CTS.Cancel and CTR.Dispose to run simultaneously; this should not throw
		//	for (int i = 0; i < 2000; i++)
		//	{
		//		m_state = 0;
		//		using (CancellationTokenSource source = new CancellationTokenSource())
		//		{
		//			CancellationTokenRegistration registration = source.Token.Register(() => Thread.Sleep(10));

		//			Task.Run(() =>
		//			{
		//				m_state = 1;
		//				source.Cancel();
		//				m_state = 2;
		//			});

		//			while (m_state == 0)
		//				;
		//			registration.Dispose();
		//			while (m_state == 1)
		//				;
		//		}
		//	}
		//}

		//volatile int m_state;
	}
}
