using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Faithlife.Utility
{
	/// <summary>
	/// Provides methods for manipulating stacks.
	/// </summary>
	public static class StackUtility
	{
		/// <summary>
		/// Returns the object at the top of the stack without moving it, or a default value if the stack is empty.
		/// </summary>
		/// <typeparam name="T">The type of element on the stack.</typeparam>
		/// <param name="stack">The stack.</param>
		/// <returns>The object at the top of the stack without moving it, or a default value if the stack is empty.</returns>
		public static T? PeekOrDefault<T>(this Stack<T> stack) => stack.Count != 0 ? stack.Peek() : default;

		/// <summary>
		/// Returns the object at the top of the stack without moving it, or a default value if the stack is empty.
		/// </summary>
		/// <typeparam name="T">The type of element on the stack.</typeparam>
		/// <param name="stack">The stack.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <returns>The object at the top of the stack without moving it, or a default value if the stack is empty.</returns>
		[return: NotNullIfNotNull("defaultValue")]
		public static T? PeekOrDefault<T>(this Stack<T> stack, T? defaultValue) => stack.Count != 0 ? stack.Peek() : defaultValue;
	}
}
