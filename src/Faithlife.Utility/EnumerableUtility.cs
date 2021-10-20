using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Faithlife.Utility
{
	/// <summary>
	/// Provides methods for manipulating IEnumerable collections.
	/// </summary>
	public static class EnumerableUtility
	{
		/// <summary>
		/// Returns a value indicating whether the specified sequences are equal. Supports one or both sequences being null.
		/// </summary>
		/// <param name="first">The first sequence.</param>
		/// <param name="second">The second sequence.</param>
		/// <returns><c>True</c> if the sequences are equal.</returns>
		public static bool AreEqual<T>(IEnumerable<T>? first, IEnumerable<T>? second)
			=> first is not null ? second is not null && first.SequenceEqual(second) : second is null;

		/// <summary>
		/// Returns a value indicating whether the specified sequences are equal using the specified equality comparer. Supports one or both sequences being null.
		/// </summary>
		/// <param name="first">The first sequence.</param>
		/// <param name="second">The second sequence.</param>
		/// <param name="comparer">The comparer.</param>
		/// <returns><c>True</c> if the sequences are equal.</returns>
		public static bool AreEqual<T>(IEnumerable<T>? first, IEnumerable<T>? second, IEqualityComparer<T>? comparer)
			=> first is not null ? second is not null && first.SequenceEqual(second, comparer) : second is null;

		/// <summary>
		/// Returns a value indicating whether the specified sequences are equal using the specified equality comparer. Supports one or both sequences being null.
		/// </summary>
		/// <param name="first">The first sequence.</param>
		/// <param name="second">The second sequence.</param>
		/// <param name="equals">Returns true if two items are equal.</param>
		/// <returns><c>True</c> if the sequences are equal.</returns>
		public static bool AreEqual<T>(IEnumerable<T>? first, IEnumerable<T>? second, Func<T?, T?, bool>? equals)
			=> AreEqual(first, second, equals is null ? null : ObjectUtility.CreateEqualityComparer(equals));

		/// <summary>
		/// Returns true if the count is as specified.
		/// </summary>
		/// <typeparam name="T">The type of the element.</typeparam>
		/// <param name="source">The sequence.</param>
		/// <param name="count">The count.</param>
		/// <returns>True if the count is as specified.</returns>
		/// <remarks>This method will often be faster than calling Enumerable.Count() and testing that value
		/// when the count may be much larger than the count being tested.</remarks>
		public static bool CountIsExactly<T>(this IEnumerable<T> source, int count)
		{
			if (source is null)
				throw new ArgumentNullException(nameof(source));
			if (count < 0)
				throw new ArgumentOutOfRangeException(nameof(count));

			if (source is ICollection<T> collection)
			{
				// use ICollection<T>.Count if available
				return collection.Count == count;
			}
			else
			{
				// iterate the sequence
				using var enumerator = source.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (count == 0)
						return false;
					count--;
				}
				return count == 0;
			}
		}

		/// <summary>
		/// Returns true if the count is greater than or equal to the specified value.
		/// </summary>
		/// <typeparam name="T">The type of the element.</typeparam>
		/// <param name="source">The sequence.</param>
		/// <param name="count">The count.</param>
		/// <returns>True if the count is greater than or equal to the specified value.</returns>
		/// <remarks>This method will often be faster than calling Enumerable.Count() and testing that value
		/// when the count may be much larger than the count being tested.</remarks>
		public static bool CountIsAtLeast<T>(this IEnumerable<T> source, int count)
		{
			if (source is null)
				throw new ArgumentNullException(nameof(source));
			if (count < 0)
				throw new ArgumentOutOfRangeException(nameof(count));
			if (count == 0)
				return true;

			if (source is ICollection<T> collection)
			{
				// use ICollection<T>.Count if available
				return collection.Count >= count;
			}
			else
			{
				// iterate the sequence
				using var enumerator = source.GetEnumerator();
				while (enumerator.MoveNext())
				{
					count--;
					if (count == 0)
						return true;
				}
				return false;
			}
		}

		/// <summary>
		/// Returns the Cartesian cross-product of a sequence of sequences.
		/// </summary>
		/// <param name="sources">The sources.</param>
		/// <returns>The cross product.</returns>
		public static IEnumerable<IEnumerable<TItem>> CrossProduct<TSequence, TItem>(this IEnumerable<TSequence> sources)
			where TSequence : IEnumerable<TItem>
		{
			return CrossProduct(sources.Select(u => u.AsEnumerable()));
		}

		/// <summary>
		/// Returns the Cartesian cross-product of a sequence of sequences.
		/// </summary>
		/// <param name="sources">The sources.</param>
		/// <returns>A new sequence of sequences, where the first item in each sequence is from the first input sequence,
		/// the second item is from the second input sequence, and so on.</returns>
		public static IEnumerable<IEnumerable<T>> CrossProduct<T>(this IEnumerable<IEnumerable<T>> sources)
		{
			if (sources is null)
				throw new ArgumentNullException(nameof(sources));
			var collections = sources.Select(x => x.AsReadOnlyList()).AsReadOnlyList();
			if (collections.Count == 0)
				throw new ArgumentException("There must be at least one source.", nameof(sources));

			return doCrossProduct();

			IEnumerable<IEnumerable<T>> doCrossProduct()
			{
				var indexes = new int[collections.Count];
				var lengths = collections.Select(x => x.Count).ToArray();

				while (true)
				{
					// yield current permutation
					var result = new T[indexes.Length];
					for (var i = 0; i < indexes.Length; i++)
						result[i] = collections[i][indexes[i]];
					yield return result;

					// find the index of the next permutation
					var index = indexes.Length - 1;
					while (true)
					{
						indexes[index]++;
						if (indexes[index] < lengths[index])
							break;

						if (index == 0)
							yield break;

						indexes[index] = 0;
						index--;
					}
				}
			}
		}

		/// <summary>
		/// Returns distinct elements from a sequence based on a key by using the default equality comparer.
		/// </summary>
		/// <typeparam name="TSource">The type of the object in the sequence.</typeparam>
		/// <typeparam name="TKey">The type of the key used for equality comparison.</typeparam>
		/// <param name="source">The sequence to remove duplicate objects from.</param>
		/// <param name="keySelector">The function that determines the key.</param>
		/// <returns>An <see cref="IEnumerable{T}"/> that contains distinct elements from the source sequence.</returns>#if NET6_0_OR_GREATER
#if NET6_0_OR_GREATER
		[Obsolete("Use System.Linq.Enumerable.DistinctBy instead")]
#endif
		public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
#if !NET6_0_OR_GREATER
			this
#endif
			IEnumerable<TSource> source, Func<TSource, TKey?> keySelector)
		{
			return DistinctBy(source, keySelector, null);
		}

		/// <summary>
		/// Returns distinct elements from a sequence based on a key by using a specified <see cref="IEqualityComparer{T}"/> to compare values.
		/// </summary>
		/// <typeparam name="TSource">The type of the object in the sequence.</typeparam>
		/// <typeparam name="TKey">The type of the key used for equality comparison.</typeparam>
		/// <param name="source">The sequence to remove duplicate objects from.</param>
		/// <param name="keySelector">The function that determines the key.</param>
		/// <param name="equalityComparer">The <see cref="IEqualityComparer{T}"/> used to compare keys; if <c>null</c>, the default comparer will be used.</param>
		/// <returns>An <see cref="IEnumerable{T}"/> that contains distinct elements from the source sequence.</returns>
#if NET6_0_OR_GREATER
		[Obsolete("Use System.Linq.Enumerable.DistinctBy instead")]
#endif
		public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
#if !NET6_0_OR_GREATER
			this
#endif
			IEnumerable<TSource> source, Func<TSource, TKey?> keySelector, IEqualityComparer<TKey>? equalityComparer)
		{
			if (source is null)
				throw new ArgumentNullException(nameof(source));
			if (keySelector is null)
				throw new ArgumentNullException(nameof(keySelector));

			return source.Distinct(new KeyEqualityComparer<TSource, TKey>(keySelector!, equalityComparer ?? EqualityComparer<TKey>.Default));
		}

		/// <summary>
		/// Enumerates a sequence of elements in batches.
		/// </summary>
		/// <typeparam name="T">The type of object in the sequence.</typeparam>
		/// <param name="source">The sequence of elements to process in batches.</param>
		/// <param name="batchSize">The batch size.</param>
		/// <returns>Batches of collections containing the elements from the source sequence.</returns>
		/// <remarks>The contents of each batch are eagerly enumerated, to avoid potential errors caused by
		/// not fully evaluating each batch (the inner enumerator) before advancing the outer enumerator.</remarks>
#if NET6_0_OR_GREATER
		[Obsolete("Use System.Linq.Enumerable.Chunk instead")]
#endif
		public static IEnumerable<IReadOnlyList<T>> EnumerateBatches<T>(this IEnumerable<T> source, int batchSize)
		{
			if (source is null)
				throw new ArgumentNullException(nameof(source));
			if (batchSize < 1)
				throw new ArgumentOutOfRangeException(nameof(batchSize));

			// optimize for small lists
			if (source is ICollection<T> sourceAsCollection)
			{
				var count = sourceAsCollection.Count;
				if (count <= batchSize)
					return count != 0 ? new[] { sourceAsCollection.AsReadOnlyList() } : Enumerable.Empty<IReadOnlyList<T>>();
			}

			// iterator in a separate function causes arguments to be validated immediately
			return doEnumerateBatches();

			IEnumerable<ReadOnlyCollection<T>> doEnumerateBatches()
			{
				// prepare batches of the desired size
				var batch = new List<T>(batchSize);

				foreach (var item in source)
				{
					// add to current batch
					batch.Add(item);

					// return current batch if it's reached the desired size
					if (batch.Count == batchSize)
					{
						yield return batch.AsReadOnly();
						batch = new List<T>(batchSize);
					}
				}

				// return the last (incomplete) batch, if any
				if (batch.Count > 0)
					yield return batch.AsReadOnly();
			}
		}

		/// <summary>
		/// Returns the first element of a sequence or a default value if no such element is found.
		/// </summary>
		/// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
		/// <param name="source">An IEnumerable&lt;TSource&gt; to return an element from.</param>
		/// <param name="defaultValue">The value to return if no element is found.</param>
		/// <returns>
		/// <paramref name="defaultValue"/> if <paramref name="source"/> is empty; otherwise, the first element in <paramref name="source"/>.
		/// </returns>
#if NET6_0_OR_GREATER
		[Obsolete("Use System.Linq.Enumerable.FirstOrDefault instead")]
#endif
		[return: NotNullIfNotNull("defaultValue")]
		public static T? FirstOrDefault<T>(
#if !NET6_0_OR_GREATER
			this
#endif
			IEnumerable<T> source, T? defaultValue) => source.TryFirst(out var found) ? found : defaultValue;

		/// <summary>
		/// Returns the first element of a sequence that satisfies a condition or a default value if no such element is found.
		/// </summary>
		/// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
		/// <param name="source">An IEnumerable&lt;TSource&gt; to return an element from.</param>
		/// <param name="predicate">A function to test each element for a condition.</param>
		/// <param name="defaultValue">The value to return if no element satisfies the condition.</param>
		/// <returns>
		/// <paramref name="defaultValue"/> if <paramref name="source"/> is empty or if no element passes the test
		/// specified by <paramref name="predicate"/>; otherwise, the first element in <paramref name="source"/> that
		/// passes the test specified by <paramref name="predicate"/>.
		/// </returns>
#if NET6_0_OR_GREATER
		[Obsolete("Use System.Linq.Enumerable.FirstOrDefault instead")]
#endif
		[return: NotNullIfNotNull("defaultValue")]
		public static T? FirstOrDefault<T>(
#if !NET6_0_OR_GREATER
			this
#endif
			IEnumerable<T> source, Func<T, bool> predicate, T? defaultValue) => source.TryFirst(predicate, out var found) ? found : defaultValue;

		/// <summary>
		/// Intersperses the specified value between the elements of the source collection.
		/// </summary>
		/// <typeparam name="T">The element type of the source sequence.</typeparam>
		/// <param name="source">The source sequence.</param>
		/// <param name="value">The value to intersperse.</param>
		/// <returns>A sequence of elements from the source collection, with value interspersed.</returns>
		public static IEnumerable<T> Intersperse<T>(this IEnumerable<T> source, T value)
		{
			if (source is null)
				throw new ArgumentNullException(nameof(source));

			// iterator in a separate function causes arguments to be validated immediately
			return doIntersperse();

			IEnumerable<T> doIntersperse()
			{
				using var it = source.GetEnumerator();
				if (it.MoveNext())
					yield return it.Current;

				while (it.MoveNext())
				{
					yield return value;
					yield return it.Current;
				}
			}
		}

		/// <summary>
		/// Determines whether the specified sequence is sorted.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="source">The sequence.</param>
		/// <returns><c>true</c> if the specified sequence is sorted; otherwise, <c>false</c>.</returns>
		public static bool IsSorted<T>(this IEnumerable<T> source) => source.IsSorted(null);

		/// <summary>
		/// Determines whether the specified sequence is sorted.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="source">The sequence.</param>
		/// <param name="comparer">The comparer.</param>
		/// <returns><c>true</c> if the specified sequence is sorted; otherwise, <c>false</c>.</returns>
		public static bool IsSorted<T>(this IEnumerable<T> source, IComparer<T>? comparer)
		{
			if (source is null)
				throw new ArgumentNullException(nameof(source));

			comparer ??= Comparer<T>.Default;

			using var it = source.GetEnumerator();
			if (it.MoveNext())
			{
				var last = it.Current;

				while (it.MoveNext())
				{
					var item = it.Current;
					if (comparer.Compare(last, item) > 0)
						return false;
					last = item;
				}
			}

			return true;
		}

		/// <summary>
		/// Returns the sum of a sequence of nullable values.
		/// </summary>
		/// <param name="values">A sequence of nullable values.</param>
		/// <returns>The sum of the values, or <c>null</c> if any value is <c>null</c>.</returns>
		public static int? NullableSum(this IEnumerable<int?> values) => values.Aggregate((int?) 0, (sum, value) => sum + value);

		/// <summary>
		/// Returns the sum of a sequence of nullable values.
		/// </summary>
		/// <param name="values">A sequence of nullable values.</param>
		/// <returns>The sum of the values, or <c>null</c> if any value is <c>null</c>.</returns>
		public static long? NullableSum(this IEnumerable<long?> values) => values.Aggregate((long?) 0, (sum, value) => sum + value);

		/// <summary>
		/// Compares two sequences.
		/// </summary>
		/// <typeparam name="T">The type of the elements.</typeparam>
		/// <param name="first">The first sequence.</param>
		/// <param name="second">The second sequence.</param>
		/// <returns>0 if they are equal; less than zero if the first is less than the second; greater than zero if the first is greater than the second.</returns>
		public static int SequenceCompare<T>(this IEnumerable<T> first, IEnumerable<T> second) => first.SequenceCompare(second, null);

		/// <summary>
		/// Compares two sequences.
		/// </summary>
		/// <typeparam name="T">The type of the elements.</typeparam>
		/// <param name="first">The first sequence.</param>
		/// <param name="second">The second sequence.</param>
		/// <param name="comparer">The comparer.</param>
		/// <returns>0 if they are equal; less than zero if the first is less than the second; greater than zero if the first is greater than the second.</returns>
		public static int SequenceCompare<T>(this IEnumerable<T> first, IEnumerable<T> second, IComparer<T>? comparer)
		{
			if (first is null)
				throw new ArgumentNullException(nameof(first));
			if (second is null)
				throw new ArgumentNullException(nameof(second));

			comparer ??= Comparer<T>.Default;

			using var firstEnumerator = first.GetEnumerator();
			using var secondEnumerator = second.GetEnumerator();
			while (firstEnumerator.MoveNext())
			{
				if (!secondEnumerator.MoveNext())
					return 1;

				var compare = comparer.Compare(firstEnumerator.Current, secondEnumerator.Current);
				if (compare != 0)
					return compare;
			}

			if (secondEnumerator.MoveNext())
				return -1;

			return 0;
		}

		/// <summary>
		/// Gets the hash code for a sequence.
		/// </summary>
		/// <typeparam name="T">The type of the elements.</typeparam>
		/// <param name="source">The sequence.</param>
		/// <returns>A valid hash code for the sequence, assuming Enumerable.SequenceEqual is used to compare them.</returns>
		/// <remarks>If the sequence is null, zero is returned.</remarks>
		public static int SequenceHashCode<T>(this IEnumerable<T>? source) => source.SequenceHashCode(null);

		/// <summary>
		/// Gets the hash code for a sequence.
		/// </summary>
		/// <typeparam name="T">The type of the elements.</typeparam>
		/// <param name="source">The sequence.</param>
		/// <param name="comparer">The comparer.</param>
		/// <returns>A valid hash code for the sequence, assuming Enumerable.SequenceEqual is used to compare them.</returns>
		/// <remarks>If the sequence is null, zero is returned.</remarks>
		public static int SequenceHashCode<T>(this IEnumerable<T>? source, IEqualityComparer<T>? comparer)
		{
			if (source is null)
				return 0;

			comparer ??= EqualityComparer<T>.Default;

			return HashCodeUtility.CombineHashCodes(source.Select(x => comparer.GetHashCode(x!)).ToArray());
		}

		/// <summary>
		/// Splits the <paramref name="source"/> sequence into <paramref name="binCount"/> equal-sized bins.
		/// If <paramref name="binCount"/> does not evenly divide the total element count, then the first
		/// (total count % <paramref name="binCount"/>) bins will have one more element than the following bins.
		/// </summary>
		/// <param name="source">The sequence to split.</param>
		/// <param name="binCount">The desired number of bins.</param>
		/// <returns>A sequence of sub-sequences of the original sequence.</returns>
		/// <remarks>
		/// WARNING: Calls Enumerable.Count(), which may enumerate the underlying sequence (if it is not an <see cref="ICollection"/> or array type).
		/// To avoid this, you may want to call EnumerateBatches instead, although
		/// that method saves all "uneveness" to the last batch, where as this one distributes it.
		/// For example, a 12-item sequence split into 5 bins will yield batches of (3, 3, 2, 2, 2);
		/// EnumerateBatches can give us (3, 3, 3, 3) or (2, 2, 2, 2, 2, 2), but cannot give us exactly 5 batches.
		/// </remarks>
		public static IEnumerable<ReadOnlyCollection<T>> SplitIntoBins<T>(this IEnumerable<T> source, int binCount)
		{
			if (source is null)
				throw new ArgumentNullException(nameof(source));
			if (binCount < 1)
				throw new ArgumentOutOfRangeException(nameof(binCount));

			var sourceCount = source.Count();

			// iterator in a separate function causes arguments to be validated immediately
			return doSplitIntoBins();

			IEnumerable<ReadOnlyCollection<T>> doSplitIntoBins()
			{
				// initial bin size is the sequence count divided by the number of bins, rounded up
				var binSize = sourceCount / binCount;
				var remainder = sourceCount % binCount;
				if (remainder > 0)
					binSize++;

				var batch = new List<T>(binSize);

				foreach (var item in source)
				{
					// add to current batch
					batch.Add(item);

					// return current batch if it's reached the desired size
					if (batch.Count == binSize)
					{
						yield return batch.AsReadOnly();

						// if we've used up all of our "remainder" from the integer division then round down the bin size
						remainder--;
						if (remainder == 0)
							binSize--;

						batch = new List<T>(binSize);
					}
				}

				// there will be no partial batches remaining
			}
		}

		/// <summary>
		/// Sorts the sequence. A shortcut for OrderBy(x => x).
		/// </summary>
		/// <param name="source">The sequence.</param>
		/// <returns>The sorted sequence.</returns>
		public static IEnumerable<T> Order<T>(this IEnumerable<T> source) => source.OrderBy(x => x);

		/// <summary>
		/// Sorts the sequence. A shortcut for OrderBy(x => x, comparer).
		/// </summary>
		/// <param name="source">The sequence.</param>
		/// <param name="comparer">The comparer.</param>
		/// <returns>The sorted sequence.</returns>
		public static IEnumerable<T> Order<T>(this IEnumerable<T> source, IComparer<T> comparer) => source.OrderBy(x => x, comparer);

		/// <summary>
		/// Lazily merges two sorted sequences, maintaining sort order. Does not remove duplicates.
		/// </summary>
		public static IEnumerable<T> MergeSorted<T>(IEnumerable<T> source1, IEnumerable<T> source2, IComparer<T> comparer)
		{
			if (source1 is null)
				throw new ArgumentNullException(nameof(source1));
			if (source2 is null)
				throw new ArgumentNullException(nameof(source2));

			comparer ??= Comparer<T>.Default;

			return doMerge();

			IEnumerable<T> doMerge()
			{
				using var enumerator1 = source1.GetEnumerator();
				using var enumerator2 = source2.GetEnumerator();
				var hasMore1 = enumerator1.MoveNext();
				var hasMore2 = enumerator2.MoveNext();
				while (hasMore1 && hasMore2)
				{
					if (comparer.Compare(enumerator1.Current, enumerator2.Current) <= 0)
					{
						yield return enumerator1.Current;
						hasMore1 = enumerator1.MoveNext();
					}
					else
					{
						yield return enumerator2.Current;
						hasMore2 = enumerator2.MoveNext();
					}
				}
				while (hasMore1)
				{
					yield return enumerator1.Current;
					hasMore1 = enumerator1.MoveNext();
				}
				while (hasMore2)
				{
					yield return enumerator2.Current;
					hasMore2 = enumerator2.MoveNext();
				}
			}
		}

		/// <summary>
		/// Returns the specified number of items from the end of the sequence.
		/// </summary>
		/// <typeparam name="T">The type of the sequence.</typeparam>
		/// <param name="source">The source sequence.</param>
		/// <param name="count">The number of items to take from the end of the sequence.</param>
		/// <returns>A collection with at most <paramref name="count"/> items, taken from the end of the sequence.</returns>
		/// <remarks>The source sequence is only evaluated once.</remarks>
#if !NETSTANDARD2_0
		[Obsolete("Use System.Linq.Enumerable.TakeLast instead (available in netcoreapp2.0, netstandard2.1)")]
#endif
		public static IReadOnlyList<T> TakeLast<T>(
#if NETSTANDARD2_0
			this
#endif
			IEnumerable<T> source, int count)
		{
			if (source is null)
				throw new ArgumentNullException(nameof(source));
			if (count < 0)
				throw new ArgumentOutOfRangeException(nameof(count));
			else if (count == 0)
				return Array.Empty<T>();

			var queue = new Queue<T>(count);
			foreach (var item in source)
			{
				if (queue.Count >= count)
					queue.Dequeue();
				queue.Enqueue(item);
			}

			return queue.ToList().AsReadOnly();
		}

		/// <summary>
		/// Represents the sequence as an <see cref="IReadOnlyList{T}"/>.
		/// </summary>
		/// <typeparam name="T">The type of items in the collection.</typeparam>
		/// <param name="source">The sequence.</param>
		/// <returns>An <see cref="IReadOnlyList{T}"/> containing the items in the sequence.</returns>
		/// <remarks>If the sequence is an <see cref="IReadOnlyList{T}"/>, it is returned directly.
		/// If it is an <see cref="IList{T}"/>, an adapter is created to wrap it. Otherwise, the sequence
		/// is copied into a <see cref="List{T}"/> and then wrapped in a <see cref="ReadOnlyCollection{T}"/>.
		/// This method is useful for forcing evaluation of a potentially lazy sequence while retaining reasonable
		/// performance for sequences that are already an <see cref="IReadOnlyList{T}"/> or <see cref="IList{T}"/>.</remarks>
		public static IReadOnlyList<T> AsReadOnlyList<T>(this IEnumerable<T> source) =>
			source as IReadOnlyList<T> ??
			(source is IList<T> list ? (IReadOnlyList<T>) new ReadOnlyListAdapter<T>(list) : source.ToList().AsReadOnly());

		/// <summary>
		/// Returns a set of the elements in the specified sequence.
		/// </summary>
		/// <typeparam name="T">The type of element in the source sequence.</typeparam>
		/// <param name="source">The sequence.</param>
		/// <returns>A set of elements.</returns>
		/// <remarks>
		/// Unlike the ToHashSet method, if the original IEnumerable is an ISet{T} it will be returned as such (rather than copying to a new HashSet).
		/// </remarks>
		public static ISet<T> AsSet<T>(this IEnumerable<T> source)
		{
			if (source is null)
				throw new ArgumentNullException(nameof(source));

			return source as ISet<T> ?? new HashSet<T>(source);
		}

		/// <summary>
		/// Returns true if the sequence is not empty and provides the first element.
		/// </summary>
		/// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
		/// <param name="source">The source sequence.</param>
		/// <param name="found">The first item, if any.</param>
		/// <returns><c>True</c> if the sequence is not empty; otherwise, <c>false</c>.</returns>
		public static bool TryFirst<T>(this IEnumerable<T> source, [MaybeNullWhen(false)] out T? found)
		{
			if (source is null)
				throw new ArgumentNullException(nameof(source));

			foreach (var item in source)
			{
				found = item;
				return true;
			}

			found = default!;
			return false;
		}

		/// <summary>
		/// Determines whether any element of a sequence satisfies a condition.
		/// </summary>
		/// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
		/// <param name="source">The source sequence.</param>
		/// <param name="predicate">The predicate.</param>
		/// <param name="found">The first item that satisfies the predicate, if any.</param>
		/// <returns><c>True</c> if any elements in the source sequence pass the test in the specified predicate; otherwise, <c>false</c>.</returns>
		public static bool TryFirst<T>(this IEnumerable<T> source, Func<T, bool> predicate, [MaybeNullWhen(false)] out T? found)
		{
			if (source is null)
				throw new ArgumentNullException(nameof(source));
			if (predicate is null)
				throw new ArgumentNullException(nameof(predicate));

			foreach (var item in source)
			{
				if (predicate(item))
				{
					found = item;
					return true;
				}
			}

			found = default!;
			return false;
		}

		/// <summary>
		/// Enumerates the specified collection, returning all the elements that are not null.
		/// </summary>
		/// <param name="source">The sequence to enumerate.</param>
		/// <returns>The non null items.</returns>
		public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source)
			where T : class
		{
			return DoWhereNotNull(source ?? throw new ArgumentNullException(nameof(source)));

			static IEnumerable<T> DoWhereNotNull(IEnumerable<T?> s)
			{
				foreach (var t in s)
				{
					if (t is not null)
						yield return t;
				}
			}
		}

		/// <summary>
		/// Enumerates the specified collection, returning all the elements that are not null.
		/// </summary>
		/// <param name="source">The sequence to enumerate.</param>
		/// <returns>The non null items.</returns>
		public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source)
			where T : struct
		{
			return DoWhereNotNull(source ?? throw new ArgumentNullException(nameof(source)));

			static IEnumerable<T> DoWhereNotNull(IEnumerable<T?> s)
			{
				foreach (var t in s)
				{
					if (t.HasValue)
						yield return t.Value;
				}
			}
		}

		/// <summary>
		/// Combines two same sized sequences.
		/// </summary>
		/// <param name="first">An IEnumerable whose elements will be returned as ValueTuple.First.</param>
		/// <param name="second">An IEnumerable whose elements will be returned as ValueTuple.Second.</param>
		/// <returns>A sequence of tuples combining the input items. Throws if the sequences don't have the same number of items.</returns>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1414:Tuple types in signatures should have element names", Justification = "By design.")]
		public static IEnumerable<(T1, T2)> Zip<T1, T2>(
#if NETSTANDARD || NETCOREAPP2_1
			this
#endif
				IEnumerable<T1> first, IEnumerable<T2> second) =>
			ZipImpl(first ?? throw new ArgumentNullException(nameof(first)), second ?? throw new ArgumentNullException(nameof(second)), UnbalancedZipStrategy.Throw);

		/// <summary>
		/// Combines two sequences.
		/// </summary>
		/// <param name="first">An IEnumerable whose elements will be returned as ValueTuple.First.</param>
		/// <param name="second">An IEnumerable whose elements will be returned as ValueTuple.Second.</param>
		/// <returns>A sequence of tuples combining the input items. If the sequences don't have the same number of items, it stops at the end of the shorter sequence.</returns>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1414:Tuple types in signatures should have element names", Justification = "By design.")]
		public static IEnumerable<(T1, T2)> ZipTruncate<T1, T2>(this IEnumerable<T1> first, IEnumerable<T2> second) =>
			ZipImpl(first ?? throw new ArgumentNullException(nameof(first)), second ?? throw new ArgumentNullException(nameof(second)), UnbalancedZipStrategy.Truncate);

		/// <summary>
		/// Makes distinct and then removes a single item from a sequence.
		/// </summary>
		/// <param name="source">A sequence whose elements not matching the specified item will be returned.</param>
		/// <param name="item">An item that will not occur in the resulting sequence.</param>
		/// <returns>A sequence consisting of every distinct item in the specified sequence that does not match the specified item according to the provided (or default if not provided) equality comparer.</returns>
		public static IEnumerable<T> Except<T>(this IEnumerable<T> source, T item) => source.Except(new[] { item });

		/// <summary>
		/// Makes distinct and then removes a single item from a sequence.
		/// </summary>
		/// <param name="source">A sequence whose elements not matching the specified item will be returned.</param>
		/// <param name="item">An item that will not occur in the resulting sequence.</param>
		/// <param name="comparer">Comparer used for exclusion. If not provided, default comparer is used.</param>
		/// <returns>A sequence consisting of every distinct item in the specified sequence that does not match the specified item according to the provided (or default if not provided) equality comparer.</returns>
		public static IEnumerable<T> Except<T>(this IEnumerable<T> source, T item, IEqualityComparer<T> comparer) => source.Except(new[] { item }, comparer);

		/// <summary>
		/// Groups the elements of a sequence according to a specified key selector function.  Each group consists of *consecutive* items having the same key. Order is preserved.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of source.</typeparam>
		/// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
		/// <param name="source">A sequence whose elements to group.</param>
		/// <param name="keySelector">A function to extract the key for each element.</param>
		/// <returns>A sequence of IGroupings containing a sequence of objects and a key.</returns>
		public static IEnumerable<IGrouping<TKey, TSource>> GroupConsecutiveBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			if (source is null)
				throw new ArgumentNullException(nameof(source));
			if (keySelector is null)
				throw new ArgumentNullException(nameof(keySelector));

			return doGroupConsecutive();

			IEnumerable<IGrouping<TKey, TSource>> doGroupConsecutive()
			{
				using var enumerator = source.GetEnumerator();
				if (!enumerator.MoveNext())
					yield break;

				var comparer = EqualityComparer<TKey>.Default;

				var lastKey = keySelector(enumerator.Current);
				var values = new List<TSource>
				{
					enumerator.Current,
				};

				while (enumerator.MoveNext())
				{
					var currentKey = keySelector(enumerator.Current);
					if (comparer.Equals(lastKey, currentKey))
					{
						values.Add(enumerator.Current);
					}
					else
					{
						yield return new Grouping<TKey, TSource>(lastKey, values.AsReadOnly());
						lastKey = currentKey;
						values = new List<TSource>
						{
							enumerator.Current,
						};
					}
				}

				yield return new Grouping<TKey, TSource>(lastKey, values.AsReadOnly());
			}
		}

		private class Grouping<TKey, TSource> : IGrouping<TKey, TSource>
		{
			public Grouping(TKey key, ReadOnlyCollection<TSource> source)
			{
				Key = key;
				m_source = source;
			}

			public TKey Key { get; }

			public IEnumerator<TSource> GetEnumerator() => m_source.GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator() => m_source.GetEnumerator();

			private readonly ReadOnlyCollection<TSource> m_source;
		}

		private enum UnbalancedZipStrategy
		{
			Throw,
			Truncate,
		}

		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1414:Tuple types in signatures should have element names", Justification = "By design.")]
		private static IEnumerable<(T1, T2)> ZipImpl<T1, T2>(IEnumerable<T1> first, IEnumerable<T2> second, UnbalancedZipStrategy strategy)
		{
			using var firstEnumerator = first.GetEnumerator();
			using var secondEnumerator = second.GetEnumerator();
			while (firstEnumerator.MoveNext())
			{
				if (secondEnumerator.MoveNext())
					yield return (firstEnumerator.Current, secondEnumerator.Current);
				else if (strategy == UnbalancedZipStrategy.Truncate)
					yield break;
				else
					throw new ArgumentException("Both sequences must be of the same size, first is larger.", nameof(first));
			}
			if (secondEnumerator.MoveNext())
			{
				if (strategy == UnbalancedZipStrategy.Truncate)
					yield break;
				else
					throw new ArgumentException("Both sequences must be of the same size, second is larger.", nameof(second));
			}
		}
	}
}
