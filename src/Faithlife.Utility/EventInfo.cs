using System;

namespace Faithlife.Utility
{
	/// <summary>
	/// Provides methods for adding and removing handlers from an event.
	/// </summary>
	/// <typeparam name="TSource">The type of the source.</typeparam>
	/// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
	/// <remarks>This class does not have a mechanism for raising the event. It is designed to provide
	/// clients with generic-type-compatible access to events.</remarks>
	public class EventInfo<TSource, TEventHandler>
		where TEventHandler : class
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EventInfo&lt;TSource, TEventHandler&gt;"/> class.
		/// </summary>
		/// <param name="addHandler">A delegate that adds an event handler to the event of the specified source.</param>
		/// <param name="removeHandler">A delegate that removes an event handler from the event of the specified source.</param>
		public EventInfo(Action<TSource, TEventHandler> addHandler, Action<TSource, TEventHandler> removeHandler)
		{
			m_addHandler = addHandler;
			m_removeHandler = removeHandler;
		}

		/// <summary>
		/// Adds an event handler to the event of the specified source.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="handler">The event handler.</param>
		public void AddHandler(TSource source, TEventHandler handler) => m_addHandler(source, handler);

		/// <summary>
		/// Removes an event handler from the event of the specified source.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="handler">The event handler.</param>
		public void RemoveHandler(TSource source, TEventHandler handler) => m_removeHandler(source, handler);

		/// <summary>
		/// Subscribes to the event of the specified source.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="handler">The event handler.</param>
		/// <returns>A Scope that unsubscribes from the event when disposed.</returns>
		/// <remarks>The Scope holds a strong reference to the event handler, which generally
		/// holds a strong reference to the target. To subscribe to an event with a weak reference,
		/// call EventInfo.WeakSubscribe.</remarks>
		public Scope Subscribe(TSource source, TEventHandler handler)
		{
			AddHandler(source, handler);
			return Scope.Create(() => RemoveHandler(source, handler));
		}

		readonly Action<TSource, TEventHandler> m_addHandler;
		readonly Action<TSource, TEventHandler> m_removeHandler;
	}
}
