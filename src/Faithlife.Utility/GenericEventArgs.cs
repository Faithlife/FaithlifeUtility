using System;

namespace Faithlife.Utility
{
	/// <summary>
	/// Event arguments that contain a single value.
	/// </summary>
	public class GenericEventArgs<T> : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GenericEventArgs&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="value">The value.</param>
		public GenericEventArgs(T value) => Value = value;

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <value>The value.</value>
		public T Value { get; }
	}
}
