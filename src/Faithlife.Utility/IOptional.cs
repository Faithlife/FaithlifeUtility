using System;

namespace Faithlife.Utility
{
	/// <summary>
	/// Represents an optional value.
	/// </summary>
	public interface IOptional
	{
		/// <summary>
		/// Gets a value indicating whether the current IOptional object has a value.
		/// </summary>
		bool HasValue { get; }

		/// <summary>
		/// Gets the value of the current IOptional value.
		/// </summary>
		/// <exception cref="InvalidOperationException">The HasValue property is false.</exception>
		object Value { get; }

		/// <summary>
		/// Gets the type of the value that can be stored by this IOptional instance.
		/// </summary>
		/// <value>The type of the value that can be stored by this IOptional instance.</value>
		Type ValueType { get; }
	}
}
