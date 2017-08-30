namespace Faithlife.Utility
{
	/// <summary>
	/// A value that can easily be restored to its current value after it has been set.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	public sealed class Scoped<T>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Scoped{T}"/> class.
		/// </summary>
		public Scoped()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Scoped{T}"/> class.
		/// </summary>
		/// <param name="value">The initial value.</param>
		public Scoped(T value)
		{
			Value = value;
		}

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <value>The value.</value>
		public T Value { get; private set; }

		/// <summary>
		/// Sets the value, but restores the value when the returned scope is disposed.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>A scope to dispose when the value should be restored.</returns>
		public Scope SetValue(T value)
		{
			T valueCurrent = Value;
			Value = value;
			return Scope.Create(delegate { Value = valueCurrent; });
		}
	}
}
