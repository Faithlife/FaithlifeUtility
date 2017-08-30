namespace Faithlife.Utility
{
	/// <summary>
	/// Implemented by reference classes that do not want to implement IEquatable{T},
	/// but do want to support some form of equivalence.
	/// </summary>
	/// <typeparam name="T">The class type.</typeparam>
	/// <remarks>This interface is similar to IEquatable{T}, but avoids the baggage of
	/// hash codes, overloaded equality operators, immutability requirements, etc.</remarks>
	public interface IHasEquivalence<T>
	{
		/// <summary>
		/// Determines whether the object is equivalent to the specified object.
		/// </summary>
		/// <param name="other">The specified object.</param>
		/// <returns>True if the object is equivalent to the specified object.</returns>
		/// <remarks>As with equality (Object.Equals), equivalence should be reflexive, symmetric,
		/// transitive, and consistent (as long as the objects in question are not modified). An object
		/// should never be equivalent to null.</remarks>
		bool IsEquivalentTo(T other);
	}
}
