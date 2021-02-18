using System;

namespace Faithlife.Utility
{
	/// <summary>
	/// Helper methods for working with EventInfo.
	/// </summary>
	public static class EventInfoUtility
	{
		/// <summary>
		/// Subscribes to the event of the specified source with a weak reference to the subscriber.
		/// </summary>
		/// <typeparam name="TSource">The type of the source.</typeparam>
		/// <typeparam name="TTarget">The type of the target.</typeparam>
		/// <param name="info">The event info.</param>
		/// <param name="source">The source.</param>
		/// <param name="target">The target.</param>
		/// <param name="action">The action, which generally delegates to the actual event handler on the target.</param>
		/// <returns>A Scope that unsubscribes from the event when disposed.</returns>
		public static Scope WeakSubscribe<TSource, TTarget>(
			this EventInfo<TSource, EventHandler> info,
			TSource source, TTarget target, Action<TTarget, object?, EventArgs> action)
			where TTarget : class
		{
			var weakTarget = new WeakReference(target, false);

			EventHandler handler = null!;
			handler =
				(s, e) =>
				{
					var t = (TTarget?) weakTarget.Target;
					if (t is not null)
						action(t, s, e);
					else
						info.RemoveHandler(source, handler);
				};
			return info.Subscribe(source, handler);
		}

		/// <summary>
		/// Subscribes to the event of the specified source with a weak reference to the subscriber.
		/// </summary>
		/// <typeparam name="TSource">The type of the source.</typeparam>
		/// <typeparam name="TTarget">The type of the target.</typeparam>
		/// <typeparam name="TEventArgs">The type of the event args.</typeparam>
		/// <param name="info">The event info.</param>
		/// <param name="source">The source.</param>
		/// <param name="target">The target.</param>
		/// <param name="action">The action, which generally delegates to the actual event handler on the target.</param>
		/// <returns>A Scope that unsubscribes from the event when disposed.</returns>
		public static Scope WeakSubscribe<TSource, TTarget, TEventArgs>(
			this EventInfo<TSource, EventHandler<TEventArgs>> info,
			TSource source, TTarget target,
			Action<TTarget, object?, TEventArgs> action)
			where TTarget : class
			where TEventArgs : EventArgs
		{
			var weakTarget = new WeakReference(target, false);

			EventHandler<TEventArgs> handler = null!;
			handler =
				(s, e) =>
				{
					var t = (TTarget?) weakTarget.Target;
					if (t is not null)
						action(t, s, e);
					else
						info.RemoveHandler(source, handler);
				};
			return info.Subscribe(source, handler);
		}
	}
}
