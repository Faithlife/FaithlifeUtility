using System;
using System.ComponentModel;
using Faithlife.Utility;
using NUnit.Framework;

// ReSharper disable AccessToModifiedClosure

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class EventInfoTests
	{
		[Test]
		public void AddRemoveHandlerTest()
		{
			EventSource eventSource = new EventSource();

			int nRaiseCount = 0;
			EventHandler fn = (sender, e) => { nRaiseCount++; };
			EventSource.UpdatedEvent.AddHandler(eventSource, fn);
			EventHandler<EventArgs> fn2 = (sender, e) => { nRaiseCount++; };
			EventSource.ClosedEvent.AddHandler(eventSource, fn2);
			EventHandler<EventArgs> fn3 = (sender, e) => { nRaiseCount++; };
			EventSource.TerminatedEvent.AddHandler(eventSource, fn3);

			Assert.AreEqual(0, nRaiseCount);
			eventSource.RaiseEvents();
			Assert.AreEqual(3, nRaiseCount);

			EventSource.UpdatedEvent.RemoveHandler(eventSource, fn);
			EventSource.ClosedEvent.RemoveHandler(eventSource, fn2);
			EventSource.TerminatedEvent.RemoveHandler(eventSource, fn3);

			Assert.AreEqual(3, nRaiseCount);
			eventSource.RaiseEvents();
			Assert.AreEqual(3, nRaiseCount);
		}

		[Test]
		public void SubscribeTest()
		{
			EventSource eventSource = new EventSource();

			int nRaiseCount = 0;
			using (EventSource.UpdatedEvent.Subscribe(eventSource, (sender, e) => { nRaiseCount++; }))
			using (EventSource.ClosedEvent.Subscribe(eventSource, (sender, e) => { nRaiseCount++; }))
			using (EventSource.TerminatedEvent.Subscribe(eventSource, (sender, e) => { nRaiseCount++; }))
			{
				Assert.AreEqual(0, nRaiseCount);
				eventSource.RaiseEvents();
				Assert.AreEqual(3, nRaiseCount);
			}

			Assert.AreEqual(3, nRaiseCount);
			eventSource.RaiseEvents();
			Assert.AreEqual(3, nRaiseCount);
		}

		[Test]
		public void WeakSubscribeDisposedTest()
		{
			EventSource eventSource = new EventSource();

			EventTarget target = new EventTarget(eventSource);
			using (target)
			{
				Assert.AreEqual(0, EventTarget.RaiseCount);
				eventSource.RaiseEvents();
				Assert.AreEqual(3, EventTarget.RaiseCount);
			}

			Assert.AreEqual(3, EventTarget.RaiseCount);
			eventSource.RaiseEvents();
			Assert.AreEqual(3, EventTarget.RaiseCount);
		}

		[Test]
#if __MOBILE__
		[Ignore]
#endif
		public void WeakSubscribeCollectedTest()
		{
			EventSource eventSource = new EventSource();

			CreateEventTarget(eventSource);

			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

			Assert.AreEqual(3, EventTarget.RaiseCount);
			eventSource.RaiseEvents();
			Assert.AreEqual(3, EventTarget.RaiseCount);
		}

		private static void CreateEventTarget(EventSource eventSource)
		{
			EventTarget target = new EventTarget(eventSource);

			Assert.AreEqual(0, EventTarget.RaiseCount);
			eventSource.RaiseEvents();
			Assert.AreEqual(3, EventTarget.RaiseCount);

			GC.KeepAlive(target);
		}

		class EventSource
		{
			public event EventHandler? Updated;
			public static readonly EventInfo<EventSource, EventHandler> UpdatedEvent =
				new EventInfo<EventSource, EventHandler>((x, fn) => x.Updated += fn, (x, fn) => x.Updated -= fn);

			public event EventHandler<EventArgs>? Closed;
			public static readonly EventInfo<EventSource, EventHandler<EventArgs>> ClosedEvent =
				new EventInfo<EventSource, EventHandler<EventArgs>>((x, fn) => x.Closed += fn, (x, fn) => x.Closed -= fn);

			public event EventHandler<EventArgs>? Terminated;
			public static readonly EventInfo<EventSource, EventHandler<EventArgs>> TerminatedEvent =
				new EventInfo<EventSource, EventHandler<EventArgs>>((x, fn) => x.Terminated += fn, (x, fn) => x.Terminated -= fn);

			public void RaiseEvents()
			{
				if (Updated != null)
					Updated(this, EventArgs.Empty);
				if (Closed != null)
					Closed(this, EventArgs.Empty);
				if (Terminated != null)
					Terminated(this, EventArgs.Empty);
			}
		}

		class EventTarget : IDisposable
		{
			public EventTarget(EventSource eventSource)
			{
				RaiseCount = 0;

				m_scopeUpdate = EventSource.UpdatedEvent.WeakSubscribe(eventSource, this, (t, s, e) => t.OnUpdated(s, e));
				m_scopeClose = EventSource.ClosedEvent.WeakSubscribe(eventSource, this, (t, s, e) => t.OnClosed(s, e));
				m_scopeTerminate = EventSource.TerminatedEvent.WeakSubscribe(eventSource, this, (EventTarget t, object s, EventArgs e) => t.OnTerminated(s, e));
			}

			public static int RaiseCount
			{
				get { return m_nRaiseCount; }
				set { m_nRaiseCount = value; }
			}

			public void OnUpdated(object source, EventArgs e)
			{
				RaiseCount++;
			}

			public void OnClosed(object source, EventArgs e)
			{
				RaiseCount++;
			}

			public void OnTerminated(object source, EventArgs e)
			{
				RaiseCount++;
			}

			public void Dispose()
			{
				m_scopeUpdate.Dispose();
				m_scopeClose.Dispose();
				m_scopeTerminate.Dispose();
			}

			readonly Scope m_scopeUpdate;
			readonly Scope m_scopeClose;
			readonly Scope m_scopeTerminate;

			[ThreadStatic]
			static int m_nRaiseCount;
		}

#if false
		class Program
		{
			static void Main()
			{
				EventSource eventSource = new EventSource();


		#region AddHandler/RemoveHandler

				EventHandler fn = () => { Console.Write("Updated! "); };
				EventSource.UpdatedEvent.AddHandler(eventSource, fn);
				EventHandler<CancelEventArgs> fn2 = () => { Console.Write("Closed! "); };
				EventSource.ClosedEvent.AddHandler(eventSource, fn2);
				CancelEventHandler fn3 = () => { Console.Write("Terminated! "); };
				EventSource.TerminatedEvent.AddHandler(eventSource, fn3);

				Console.Write("\nYes: ");
				eventSource.RaiseEvents();

				EventSource.UpdatedEvent.RemoveHandler(eventSource, fn);
				EventSource.ClosedEvent.RemoveHandler(eventSource, fn2);
				EventSource.TerminatedEvent.RemoveHandler(eventSource, fn3);

				Console.Write("\nNo: ");
				eventSource.RaiseEvents();

		#endregion


		#region Subscribe

				using (EventSource.UpdatedEvent.Subscribe(eventSource, () => { Console.Write("Updated! "); }))
				using (EventSource.ClosedEvent.Subscribe(eventSource, () => { Console.Write("Closed! "); }))
				using (EventSource.TerminatedEvent.Subscribe(eventSource, () => { Console.Write("Terminated!"); }))
				{
					Console.Write("\nYes: ");
					eventSource.RaiseEvents();
				}

				Console.Write("\nNo: ");
				eventSource.RaiseEvents();

		#endregion


		#region WeakSubscribe (disposed)

				using (new EventTarget(eventSource))
				{
					Console.Write("\nYes: ");
					eventSource.RaiseEvents();
				}

				Console.Write("\nNo: ");
				eventSource.RaiseEvents();

		#endregion


		#region WeakSubscribe (collected)

				CreateEventTarget(eventSource);

				GC.Collect();

				Console.Write("\nNo: ");
				eventSource.RaiseEvents();

		#endregion


				Console.WriteLine();
			}

			private static void CreateEventTarget(EventSource eventSource)
			{
				EventTarget target = new EventTarget(eventSource);

				Console.Write("\nYes: ");
				eventSource.RaiseEvents();

				GC.KeepAlive(target);
			}
		}
#endif
	}
}
