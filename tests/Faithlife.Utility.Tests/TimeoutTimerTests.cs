using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class TimeoutTimerTests
	{
		[Test]
		public async Task DefaultTimeout()
		{
			TimeoutTimer t = new TimeoutTimer();
			Assert.AreEqual(t.RemainingMilliseconds, 0);
			Assert.AreEqual(t.RemainingTimeSpan, TimeSpan.Zero);
			Assert.IsTrue(t.TimedOut);

			var nDelay = 100;
			await Task.Delay(2 * nDelay);
			Assert.AreEqual(t.RemainingMilliseconds, 0);
			Assert.AreEqual(t.RemainingTimeSpan, TimeSpan.Zero);
			Assert.IsTrue(t.TimedOut);
		}

		[Test]
		public async Task TimeoutMilliseconds()
		{
			var nTime = 1000;
			TimeoutTimer t = new TimeoutTimer(nTime);
			Assert.GreaterOrEqual(t.RemainingMilliseconds, 0);
			Assert.LessOrEqual(t.RemainingMilliseconds, nTime);
			Assert.GreaterOrEqual(t.RemainingTimeSpan, TimeSpan.Zero);
			Assert.LessOrEqual(t.RemainingTimeSpan, new TimeSpan(0, 0, 0, 0, nTime));
			Assert.IsFalse(t.TimedOut);

			var nDelay = 100;
			await Task.Delay(2 * nDelay);
			Assert.GreaterOrEqual(t.RemainingMilliseconds, 0);
			Assert.GreaterOrEqual(t.RemainingTimeSpan, TimeSpan.Zero);
			Assert.LessOrEqual(t.RemainingMilliseconds, nTime - nDelay);
			Assert.LessOrEqual(t.RemainingTimeSpan, new TimeSpan(0, 0, 0, 0, nTime - nDelay));
		}

		[Test]
		public void TimeoutNegative()
		{
			Assert.Throws<ArgumentException>(() => new TimeoutTimer(-100));
		}

		[Test]
		public void TimeoutTimespan()
		{
			var nTime = 1000;
			TimeoutTimer t = new TimeoutTimer(TimeSpan.FromMilliseconds(nTime));
			Assert.GreaterOrEqual(t.RemainingMilliseconds, 0);
			Assert.LessOrEqual(t.RemainingMilliseconds, nTime);
		}

		[Test]
		public async Task TimeoutInfinite()
		{
			TimeoutTimer t = new TimeoutTimer(Timeout.Infinite);
			Assert.AreEqual(int.MaxValue, t.RemainingMilliseconds);
			await Task.Delay(50);
			Assert.AreEqual(int.MaxValue, t.RemainingMilliseconds);
			Assert.IsFalse(t.TimedOut);

			t.Reset(0);
			Assert.AreEqual(t.RemainingMilliseconds, 0);
			Assert.AreEqual(t.RemainingTimeSpan, TimeSpan.Zero);
			Assert.IsTrue(t.TimedOut);
		}

		[Test]
		public async Task TimeoutElapse()
		{
			var nTime = 50;
			TimeoutTimer t = new TimeoutTimer(nTime);
			Assert.GreaterOrEqual(t.RemainingMilliseconds, 0);
			Assert.LessOrEqual(t.RemainingMilliseconds, nTime);

			await Task.Delay(nTime * 2);
			Assert.AreEqual(0, t.RemainingMilliseconds);
			Assert.IsTrue(t.TimedOut);
		}
	}
}
