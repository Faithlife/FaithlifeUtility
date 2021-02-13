#nullable disable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Faithlife.Utility
{
	/// <summary>
	/// Represents an optional value.
	/// </summary>
	/// <typeparam name="T">The type of the optional value.</typeparam>
	[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", Justification = "Optional is not reserved in C#.")]
	public readonly struct Optional<T> : IEquatable<Optional<T>>, IOptional
	{
		/// <summary>
		/// Initializes a new instance of the Optional{T} structure to the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		public Optional(T value)
		{
			m_value = value;
			HasValue = true;
		}

		/// <summary>
		/// Gets a value indicating whether the current Optional{T} object has a value.
		/// </summary>
		public bool HasValue { get; }

		/// <summary>
		/// Gets the value of the current Optional{T} value.
		/// </summary>
		/// <exception cref="InvalidOperationException">The HasValue property is false.</exception>
		public T Value
		{
			get
			{
				if (!HasValue)
					throw new InvalidOperationException("The HasValue property is false.");

				return m_value;
			}
		}

		/// <summary>
		/// Gets the value of the current Optional{T} value.
		/// </summary>
		/// <exception cref="InvalidOperationException">The HasValue property is false.</exception>
		object IOptional.Value => Value;

		/// <summary>
		/// Gets the type of the value that can be stored by this IOptional instance.
		/// </summary>
		/// <value>The type of the value that can be stored by this IOptional instance.</value>
		Type IOptional.ValueType => typeof(T);

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>True if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.</returns>
		public bool Equals(Optional<T> other) => HasValue && other.HasValue ? EqualityComparer<T>.Default.Equals(m_value, other.m_value) : HasValue == other.HasValue;

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="obj">An object to compare with this object.</param>
		/// <returns>True if the current object is equal to the <paramref name="obj"/> parameter; otherwise, false.</returns>
		public override bool Equals(object obj) => obj is Optional<T> optional && Equals(optional);

		/// <summary>
		/// Retrieves the hash code of the object returned by the Value property.
		/// </summary>
		/// <returns>The hash code of the object returned by the Value property if the HasValue property is true and the Value is not null; otherwise zero.</returns>
		public override int GetHashCode() => HasValue && m_value is not null ? EqualityComparer<T>.Default.GetHashCode(m_value) : 0;

		/// <summary>
		/// Retrieves the value of the current Optional{T} object, or the object's default value.
		/// </summary>
		/// <returns>The value of the Value property if the HasValue property is true; otherwise, the default value of the type T.</returns>
		public T GetValueOrDefault() => GetValueOrDefault(default);

		/// <summary>
		/// Retrieves the value of the current Optional{T} object, or the specified default value.
		/// </summary>
		/// <param name="defaultValue">A value to return if the HasValue property is false.</param>
		/// <returns>The value of the Value property if the HasValue property is true; otherwise, the defaultValue parameter.</returns>
		public T GetValueOrDefault(T defaultValue) => HasValue ? m_value : defaultValue;

		/// <summary>
		/// Returns the text representation of the value of the current Optional{T} object.
		/// </summary>
		/// <returns>The text representation of the object returned by the Value property if the HasValue property is true and the Value is not null; otherwise the empty string.</returns>
		public override string ToString() => HasValue && m_value is not null ? m_value.ToString() : "";

		/// <summary>
		/// Creates a new Optional{T} object initialized to a specified value.
		/// </summary>
		/// <param name="value">A value type.</param>
		/// <returns>An Optional{T} object whose Value property is initialized with the value parameter.</returns>
		[SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "By design.")]
		public static implicit operator Optional<T>(T value) => new Optional<T>(value);

		/// <summary>
		/// Returns the value of a specified Optional{T} value.
		/// </summary>
		/// <param name="optional">An Optional{T} value.</param>
		/// <returns>The value of the Value property for the value parameter.</returns>
		/// <exception cref="InvalidOperationException">The HasValue property is false.</exception>
		[SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "By design.")]
		public static explicit operator T(Optional<T> optional) => optional.Value;

		/// <summary>
		/// Compares two instances for equality.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns><c>true</c> the instances are equal; otherwise, <c>false</c>.</returns>
		public static bool operator ==(Optional<T> left, Optional<T> right) => left.Equals(right);

		/// <summary>
		/// Compares two instances for inequality.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns><c>true</c> if the instances are not equal; otherwise, <c>false</c>.</returns>
		public static bool operator !=(Optional<T> left, Optional<T> right) => !left.Equals(right);

		private readonly T m_value;
	}
}
