using System;
using System.Collections.Generic;
using System.Linq;

namespace Faithlife.Utility
{
	/// <summary>
	/// Methods for working with IHasEquivalence.
	/// </summary>
	public static class Equivalence
	{
		/// <summary>
		/// True if the objects are equivalent.
		/// </summary>
		/// <typeparam name="T">The object type.</typeparam>
		/// <param name="left">The left object.</param>
		/// <param name="right">The right object.</param>
		public static bool AreEquivalent<T>(T? left, T? right)
			where T : IHasEquivalence<T>
			=> left is null ? right is null : left.IsEquivalentTo(right);

		/// <summary>
		/// True if the sequences are equivalent.
		/// </summary>
		/// <typeparam name="T">The object type.</typeparam>
		/// <param name="left">The left sequence.</param>
		/// <param name="right">The right sequence.</param>
		/// <returns>True if the sequence are equivalent.</returns>
		public static bool AreSequencesEquivalent<T>(IEnumerable<T>? left, IEnumerable<T>? right)
			where T : IHasEquivalence<T>
			=> left is null ? right is null : right is not null && left.SequenceEquivalent(right);

		/// <summary>
		/// Returns an equality comparer that calls IHasEquivalence.IsEquivalentTo.
		/// </summary>
		/// <typeparam name="T">The object type.</typeparam>
		/// <returns>The equality comparer.</returns>
		public static IEqualityComparer<T> GetEqualityComparer<T>()
			where T : IHasEquivalence<T>
			=> new EquivalenceComparer<T, T>();

		/// <summary>
		/// Returns an equality comparer that calls IHasEquivalence.IsEquivalentTo.
		/// </summary>
		/// <typeparam name="TDerived">The object type.</typeparam>
		/// <typeparam name="TBase">The base type that implements IHasEquivalence.</typeparam>
		/// <returns>The equality comparer.</returns>
		public static IEqualityComparer<TDerived> GetEqualityComparer<TDerived, TBase>()
			where TDerived : TBase, IHasEquivalence<TBase>
			=> new EquivalenceComparer<TDerived, TBase>();

		/// <summary>
		/// Returns an equality comparer that calls IHasEquivalence.IsEquivalentTo.
		/// </summary>
		/// <typeparam name="T">The object type.</typeparam>
		/// <returns>The equality comparer.</returns>
		/// <remarks>If T does not implement IHasEquivalence{T}, this method returns
		/// EqualityComparer{T}.Default as a fallback instead.</remarks>
		public static IEqualityComparer<T> GetEqualityComparerOrFallback<T>()
			=> GetEqualityComparerOrFallback(EqualityComparer<T>.Default);

		/// <summary>
		/// Returns an equality comparer that calls IHasEquivalence.IsEquivalentTo.
		/// </summary>
		/// <typeparam name="T">The object type.</typeparam>
		/// <returns>The equality comparer.</returns>
		/// <remarks>If T does not implement IHasEquivalence{T}, this method returns
		/// the specified fallback instead.</remarks>
		public static IEqualityComparer<T> GetEqualityComparerOrFallback<T>(IEqualityComparer<T> fallback)
			=> EquivalenceComparerCache<T>.Instance ?? fallback;

		/// <summary>
		/// True if the sequences are equivalent.
		/// </summary>
		/// <typeparam name="T">The object type.</typeparam>
		/// <param name="source">The sequence.</param>
		/// <param name="other">The other sequence.</param>
		public static bool SequenceEquivalent<T>(this IEnumerable<T> source, IEnumerable<T> other)
			where T : IHasEquivalence<T>
			=> source.SequenceEqual(other, GetEqualityComparer<T>());

		private static class EquivalenceComparerCache<T>
		{
			public static readonly IEqualityComparer<T>? Instance = CreateInstance();

			private static IEqualityComparer<T>? CreateInstance()
			{
				var type = typeof(T);
				do
				{
					if (typeof(IHasEquivalence<>).MakeGenericType(type).IsAssignableFrom(typeof(T)))
						return (IEqualityComparer<T>) Activator.CreateInstance(typeof(EquivalenceComparer<,>).MakeGenericType(typeof(T), type))!;
					type = type.GetBaseType();
				}
				while (type is not null);

				return null;
			}
		}

		private sealed class EquivalenceComparer<TDerived, TBase> : EqualityComparer<TDerived>
			where TDerived : TBase, IHasEquivalence<TBase>
		{
			public override bool Equals(TDerived? left, TDerived? right) => left?.IsEquivalentTo(right) ?? right is null;

			public override int GetHashCode(TDerived value) => throw new NotImplementedException();
		}
	}
}
