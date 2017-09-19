using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Faithlife.Utility
{
	/// <summary>
	/// Provides methods for manipulating IEnumerable collections.
	/// </summary>
	public static class EnumerableUtility
	{
		/// <summary>
		/// Returns a new sequence that conditionally has the specified item appended to the end (appended iff it doesn't equal an existing sequence item)
		/// </summary>
		/// <typeparam name="T">The type of the sequence items.</typeparam>
		/// <param name="sequence">The sequence.</param>
		/// <param name="item">The item to conditionally append.</param>
		/// <param name="comparer">Comparer used to check whether item is already present. If not provided, default comparer is used.</param>
		/// <returns></returns>
		public static IEnumerable<T> AppendIfNotAlreadyPresent<T>(this IEnumerable<T> sequence, T item, IEqualityComparer<T> comparer = null)
		{
			if (sequence == null)
				throw new ArgumentNullException(nameof(sequence));

			if (comparer == null)
				comparer = EqualityComparer<T>.Default;

			bool isAlreadyPresent = false;

			foreach (T sequenceItem in sequence)
			{
				if (!isAlreadyPresent && comparer.Equals(sequenceItem, item))
					isAlreadyPresent = true;

				yield return sequenceItem;
			}

			if (!isAlreadyPresent)
				yield return item;
		}

		/// <summary>
		/// Returns a value indicating whether the specified sequences are equal. Supports one or both sequences being null.
		/// </summary>
		/// <param name="first">The first sequence.</param>
		/// <param name="second">The second sequence.</param>
		/// <returns><c>True</c> if the sequences are equal.</returns>
		public static bool AreEqual<T>(IEnumerable<T> first, IEnumerable<T> second)
		{
			return first != null ?
				second != null && first.SequenceEqual(second) :
				second == null;
		}

		/// <summary>
		/// Returns a value indicating whether the specified sequences are equal using the specified equality comparer. Supports one or both sequences being null.
		/// </summary>
		/// <param name="first">The first sequence.</param>
		/// <param name="second">The second sequence.</param>
		/// <param name="comparer">The comparer.</param>
		/// <returns><c>True</c> if the sequences are equal.</returns>
		public static bool AreEqual<T>(IEnumerable<T> first, IEnumerable<T> second, IEqualityComparer<T> comparer)
		{
			return first != null ?
				second != null && first.SequenceEqual(second, comparer) :
				second == null;
		}

		/// <summary>
		/// Returns a value indicating whether the specified sequences are equal using the specified equality comparer. Supports one or both sequences being null.
		/// </summary>
		/// <param name="first">The first sequence.</param>
		/// <param name="second">The second sequence.</param>
		/// <param name="equals">Returns true if two items are equal.</param>
		/// <returns><c>True</c> if the sequences are equal.</returns>
		public static bool AreEqual<T>(IEnumerable<T> first, IEnumerable<T> second, Func<T, T, bool> equals)
		{
			return AreEqual(first, second, equals == null ? null : ObjectUtility.CreateEqualityComparer(equals));
		}

		/// <summary>
		/// Returns true if the count is as specified.
		/// </summary>
		/// <typeparam name="T">The type of the element.</typeparam>
		/// <param name="sequence">The sequence.</param>
		/// <param name="count">The count.</param>
		/// <returns>True if the count is as specified.</returns>
		/// <remarks>This method will often be faster than calling Enumerable.Count() and testing that value
		/// when the count may be much larger than the count being tested.</remarks>
		public static bool CountIsExactly<T>(this IEnumerable<T> sequence, int count)
		{
			if (sequence == null)
				throw new ArgumentNullException(nameof(sequence));
			if (count < 0)
				throw new ArgumentOutOfRangeException(nameof(count));

			ICollection<T> coll = sequence as ICollection<T>;
			if (coll != null)
			{
				// use ICollection<T>.Count if available
				return coll.Count == count;
			}
			else
			{
				// iterate the sequence
				using (IEnumerator<T> it = sequence.GetEnumerator())
				{
					while (it.MoveNext())
					{
						if (count == 0)
							return false;
						count--;
					}
					return count == 0;
				}
			}
		}

		/// <summary>
		/// Returns true if the count is greater than or equal to the specified value.
		/// </summary>
		/// <typeparam name="T">The type of the element.</typeparam>
		/// <param name="sequence">The sequence.</param>
		/// <param name="count">The count.</param>
		/// <returns>True if the count is greater than or equal to the specified value.</returns>
		/// <remarks>This method will often be faster than calling Enumerable.Count() and testing that value
		/// when the count may be much larger than the count being tested.</remarks>
		public static bool CountIsAtLeast<T>(this IEnumerable<T> sequence, int count)
		{
			if (sequence == null)
				throw new ArgumentNullException(nameof(sequence));
			if (count < 0)
				throw new ArgumentOutOfRangeException(nameof(count));
			if (count == 0)
				return true;

			ICollection<T> coll = sequence as ICollection<T>;
			if (coll != null)
			{
				// use ICollection<T>.Count if available
				return coll.Count >= count;
			}
			else
			{
				// iterate the sequence
				using (IEnumerator<T> it = sequence.GetEnumerator())
				{
					while (it.MoveNext())
					{
						count--;
						if (count == 0)
							return true;
					}
					return false;
				}
			}
		}

		/// <summary>
		/// Returns the Cartesian cross-product of a sequence of sequences.
		/// </summary>
		/// <param name="sequences">The sequences.</param>
		/// <returns>The cross product.</returns>
		public static IEnumerable<IEnumerable<TItem>> CrossProduct<TSequence, TItem>(this IEnumerable<TSequence> sequences)
			where TSequence : IEnumerable<TItem>
		{
			return CrossProduct(sequences.Select(u => u.AsEnumerable()));
		}

		/// <summary>
		/// Returns the Cartesian cross-product of a sequence of sequences.
		/// </summary>
		/// <param name="sequences">The sequences.</param>
		/// <returns>A new sequence of sequences, where the first item in each sequence is from the first input sequence,
		/// the second item is from the second input sequence, and so on.</returns>
		public static IEnumerable<IEnumerable<T>> CrossProduct<T>(this IEnumerable<IEnumerable<T>> sequences)
		{
			if (sequences == null)
				throw new ArgumentNullException(nameof(sequences));
			var collections = sequences.Select(x => x.AsReadOnlyList()).AsReadOnlyList();
			if (collections.Count == 0)
				throw new ArgumentException("There must be at least one sequence.", nameof(sequences));

			return DoCrossProduct(collections);
		}

		private static IEnumerable<IEnumerable<T>> DoCrossProduct<T>(IReadOnlyList<IReadOnlyList<T>> collections)
		{
			var indexes = new int[collections.Count];
			var lengths = collections.Select(x => x.Count).ToArray();

			while (true)
			{
				// yield current permutation
				var result = new T[indexes.Length];
				for (int i = 0; i < indexes.Length; i++)
					result[i] = collections[i][indexes[i]];
				yield return result;

				// find the index of the next permutation
				int index = indexes.Length - 1;
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

		/// <summary>
		/// Returns distinct elements from a sequence based on a key by using the default equality comparer.
		/// </summary>
		/// <typeparam name="TSource">The type of the object in the sequence.</typeparam>
		/// <typeparam name="TKey">The type of the key used for equality comparison.</typeparam>
		/// <param name="sequence">The sequence to remove duplicate objects from.</param>
		/// <param name="keySelector">The function that determines the key.</param>
		/// <returns>An <see cref="IEnumerable{T}"/> that contains distinct elements from the source sequence.</returns>
		public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> sequence, Func<TSource, TKey> keySelector)
		{
			return DistinctBy(sequence, keySelector, null);
		}

		/// <summary>
		/// Returns distinct elements from a sequence based on a key by using a specified <see cref="IEqualityComparer{T}"/> to compare values.
		/// </summary>
		/// <typeparam name="TSource">The type of the object in the sequence.</typeparam>
		/// <typeparam name="TKey">The type of the key used for equality comparison.</typeparam>
		/// <param name="sequence">The sequence to remove duplicate objects from.</param>
		/// <param name="keySelector">The function that determines the key.</param>
		/// <param name="equalityComparer">The <see cref="IEqualityComparer{T}"/> used to compare keys; if <c>null</c>, the default comparer will be used.</param>
		/// <returns>An <see cref="IEnumerable{T}"/> that contains distinct elements from the source sequence.</returns>
		public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> sequence, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> equalityComparer)
		{
			if (sequence == null)
				throw new ArgumentNullException(nameof(sequence));
			if (keySelector == null)
				throw new ArgumentNullException(nameof(keySelector));

			return sequence.Distinct(new KeyEqualityComparer<TSource, TKey>(keySelector, equalityComparer ?? EqualityComparer<TKey>.Default));
		}

		/// <summary>
		/// Enumerates the specified collection, casting each element to a derived type.
		/// </summary>
		/// <param name="sequence">The collection to enumerate.</param>
		/// <returns>The elements of the specified collection, cast to the destination type.</returns>
		/// <remarks>This method can only be used when the destination type is derived from the source type.</remarks>
		/// <exception cref="InvalidCastException">One of the elements could not be cast to the derived type.</exception>
		/// <seealso cref="Upcast{TSource,TDest}" />
		public static IEnumerable<TDest> Downcast<TSource, TDest>(this IEnumerable<TSource> sequence) where TDest : TSource
		{
			if (sequence == null)
				throw new ArgumentNullException(nameof(sequence));
			return sequence.Select(obj => (TDest) obj);
		}

		/// <summary>
		/// Returns the specified sequence, or an empty sequence if it is null.
		/// </summary>
		/// <typeparam name="T">The type of object in the sequence.</typeparam>
		/// <param name="sequence">The sequence.</param>
		/// <returns>The specified sequence, or an empty sequence if it is null.</returns>
		public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> sequence)
		{
			return sequence ?? Enumerable.Empty<T>();
		}

		/// <summary>
		/// Returns <c>true</c> if the sequence is null or contains no elements.
		/// </summary>
		/// <param name="sequence">The sequence.</param>
		/// <returns><c>true</c> if the sequence is null or contains no elements.</returns>
		public static bool IsNullOrEmpty<T>(this IEnumerable<T> sequence)
		{
			if (sequence == null)
				return true;

			// use ICollection<T>.Count if available
			ICollection<T> coll = sequence as ICollection<T>;
			if (coll != null)
				return coll.Count == 0;

			// iterate the sequence
			using (IEnumerator<T> it = sequence.GetEnumerator())
			{
				if (it.MoveNext())
					return false;
			}

			return true;
		}

		/// <summary>
		/// Returns <c>true</c> if the sequence is null or contains no elements.
		/// </summary>
		/// <param name="sequence">The sequence.</param>
		/// <returns><c>true</c> if the sequence is null or contains no elements.</returns>
		public static bool IsNullOrEmpty(this IEnumerable sequence)
		{
			if (sequence == null)
				return true;

			// use ICollection.Count if available
			ICollection coll = sequence as ICollection;
			if (coll != null)
				return coll.Count == 0;

			// iterate the sequence
			IEnumerator enumerator = sequence.GetEnumerator();
			try
			{
				return !enumerator.MoveNext();
			}
			finally
			{
				DisposableUtility.DisposeObject(ref enumerator);
			}
		}

		/// <summary>
		/// Returns the specified sequence, or an empty sequence if it is null.
		/// </summary>
		/// <param name="sequence">The sequence.</param>
		/// <returns>The specified sequence, or an empty sequence if it is null.</returns>
		public static IEnumerable EmptyIfNull(this IEnumerable sequence)
		{
			return sequence ?? Enumerable.Empty<object>();
		}

		/// <summary>
		/// Enumerates the specified argument.
		/// </summary>
		/// <param name="item">The argument to enumerate.</param>
		/// <returns>The enumerated arguments.</returns>
		public static IEnumerable<T> Enumerate<T>(T item)
		{
			yield return item;
		}

		/// <summary>
		/// Enumerates the specified arguments.
		/// </summary>
		/// <param name="items">The arguments to enumerate.</param>
		/// <returns>The enumerated arguments.</returns>
		public static IEnumerable<T> Enumerate<T>(params T[] items)
		{
			return items.AsReadOnly();
		}

		/// <summary>
		/// Enumerates a sequence of elements in batches.
		/// </summary>
		/// <typeparam name="T">The type of object in the sequence.</typeparam>
		/// <param name="sequence">The sequence of elements to process in batches.</param>
		/// <param name="batchSize">The batch size.</param>
		/// <returns>Batches of collections containing the elements from the source sequence.</returns>
		/// <remarks>The contents of each batch are eagerly enumerated, to avoid potential errors caused by
		/// not fully evaluating each batch (the inner enumerator) before advancing the outer enumerator.</remarks>
		public static IEnumerable<ReadOnlyCollection<T>> EnumerateBatches<T>(this IEnumerable<T> sequence, int batchSize)
		{
			if (sequence == null)
				throw new ArgumentNullException(nameof(sequence));
			if (batchSize < 1)
				throw new ArgumentOutOfRangeException(nameof(batchSize));

			// optimize for small lists
			IList<T> seqAsList = sequence as IList<T>;
			if (seqAsList != null)
			{
				int count = seqAsList.Count;
				if (count <= batchSize)
					return count != 0 ? Enumerate(seqAsList.AsReadOnly()) : Enumerable.Empty<ReadOnlyCollection<T>>();
			}

			// iterator in a separate function causes arguments to be validated immediately
			return EnumerateBatchesIterator(sequence, batchSize);
		}

		/// <summary>
		/// Enumerates a sequence of elements in batches.
		/// </summary>
		/// <typeparam name="T">The type of object in the sequence.</typeparam>
		/// <param name="sequence">The sequence of elements to process in batches.</param>
		/// <param name="startsNewBatch">Should return true if the item should start a new batch.</param>
		/// <returns>Batches of collections containing the elements from the source sequence.</returns>
		/// <remarks>The contents of each batch are eagerly enumerated, to avoid potential errors caused by
		/// not fully evaluating each batch (the inner enumerator) before advancing the outer enumerator.</remarks>
		public static IEnumerable<ReadOnlyCollection<T>> EnumerateBatches<T>(this IEnumerable<T> sequence, Func<T, bool> startsNewBatch)
		{
			if (sequence == null)
				throw new ArgumentNullException(nameof(sequence));
			if (startsNewBatch == null)
				throw new ArgumentNullException(nameof(startsNewBatch));

			// iterator in a separate function causes arguments to be validated immediately
			return EnumerateBatchesIterator(sequence, startsNewBatch);
		}

		/// <summary>
		///	 Like SingleOrDefault, but doesn't throw an exception if there is more than one (instead returns default value).
		/// </summary>
		/// <typeparam name="T">The type of object in the sequence.</typeparam>
		/// <param name="sequence">The sequence of elements.</param>
		/// <returns>The first item in the sequence if there is exactly one item in the sequence; otherwise, the default value.</returns>
		public static T ExactlyOneOrDefault<T>(this IEnumerable<T> sequence)
		{
			if (sequence == null)
				throw new ArgumentNullException(nameof(sequence));

			T result = default(T);
			bool foundResult = false;

			foreach (T element in sequence)
			{
				if (foundResult)
					return default(T);

				result = element;
				foundResult = true;
			}

			return result;
		}

		/// <summary>
		///	 Like SingleOrDefault, but doesn't throw an exception if there is more than one (instead returns default value).
		/// </summary>
		/// <typeparam name="T">The type of object in the sequence.</typeparam>
		/// <param name="sequence">The sequence of elements.</param>
		/// <param name="predicate">The predicate used to determine whether there is one matching element that should be returned.</param>
		/// <returns>The first item in the sequence if there is exactly one item in the sequence; otherwise, the default value.</returns>
		public static T ExactlyOneOrDefault<T>(this IEnumerable<T> sequence, Func<T, bool> predicate)
		{
			if (sequence == null)
				throw new ArgumentNullException(nameof(sequence));
			if (predicate == null)
				throw new ArgumentNullException(nameof(predicate));

			return sequence.Where(predicate).ExactlyOneOrDefault();
		}

		/// <summary>
		/// Returns the first element of a sequence or a default value if no such element is found.
		/// </summary>
		/// <typeparam name="T">The type of the elements of <paramref name="sequence"/>.</typeparam>
		/// <param name="sequence">An IEnumerable&lt;TSource&gt; to return an element from.</param>
		/// <param name="defaultValue">The value to return if no element is found.</param>
		/// <returns>
		/// <paramref name="defaultValue"/> if <paramref name="sequence"/> is empty; otherwise, the first element in <paramref name="sequence"/>.
		/// </returns>
		public static T FirstOrDefault<T>(this IEnumerable<T> sequence, T defaultValue)
		{
			T found;
			return sequence.TryFirst(out found) ? found : defaultValue;
		}

		/// <summary>
		/// Returns the first element of a sequence that satisfies a condition or a default value if no such element is found.
		/// </summary>
		/// <typeparam name="T">The type of the elements of <paramref name="sequence"/>.</typeparam>
		/// <param name="sequence">An IEnumerable&lt;TSource&gt; to return an element from.</param>
		/// <param name="predicate">A function to test each element for a condition.</param>
		/// <param name="defaultValue">The value to return if no element satisfies the condition.</param>
		/// <returns>
		/// <paramref name="defaultValue"/> if <paramref name="sequence"/> is empty or if no element passes the test
		/// specified by <paramref name="predicate"/>; otherwise, the first element in <paramref name="sequence"/> that
		/// passes the test specified by <paramref name="predicate"/>.
		/// </returns>
		public static T FirstOrDefault<T>(this IEnumerable<T> sequence, Func<T, bool> predicate, T defaultValue)
		{
			T found;
			return sequence.TryFirst(predicate, out found) ? found : defaultValue;
		}

		/// <summary>
		/// Executes the specified action for each item in the sequence.
		/// </summary>
		/// <typeparam name="T">The type of object in the sequence.</typeparam>
		/// <param name="sequence">The sequence.</param>
		/// <param name="action">The action.</param>
		public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action)
		{
			foreach (T item in sequence)
				action(item);
		}

		/// <summary>
		/// Executes the specified action for each item in the sequence, including a zero-based index.
		/// </summary>
		/// <typeparam name="T">The type of object in the sequence.</typeparam>
		/// <param name="sequence">The sequence.</param>
		/// <param name="action">The action (including the zero-based index).</param>
		public static void ForEach<T>(this IEnumerable<T> sequence, Action<T, int> action)
		{
			int nIndex = 0;
			foreach (T item in sequence)
				action(item, nIndex++);
		}

		/// <summary>
		/// Intersperses the specified value between the elements of the source collection.
		/// </summary>
		/// <typeparam name="T">The element type of the source sequence.</typeparam>
		/// <param name="sequence">The source sequence.</param>
		/// <param name="value">The value to intersperse.</param>
		/// <returns>A sequence of elements from the source collection, with value interspersed.</returns>
		public static IEnumerable<T> Intersperse<T>(this IEnumerable<T> sequence, T value)
		{
			if (sequence == null)
				throw new ArgumentNullException(nameof(sequence));

			// iterator in a separate function causes arguments to be validated immediately
			return IntersperseIterator(sequence, value);
		}

		/// <summary>
		/// Determines whether the specified sequence is sorted.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="sequence">The sequence.</param>
		/// <returns><c>true</c> if the specified sequence is sorted; otherwise, <c>false</c>.</returns>
		public static bool IsSorted<T>(this IEnumerable<T> sequence)
		{
			return sequence.IsSorted(null);
		}

		/// <summary>
		/// Determines whether the specified sequence is sorted.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="sequence">The sequence.</param>
		/// <param name="comparer">The comparer.</param>
		/// <returns><c>true</c> if the specified sequence is sorted; otherwise, <c>false</c>.</returns>
		public static bool IsSorted<T>(this IEnumerable<T> sequence, IComparer<T> comparer)
		{
			if (sequence == null)
				throw new ArgumentNullException(nameof(sequence));

			if (comparer == null)
				comparer = Comparer<T>.Default;

			using (IEnumerator<T> it = sequence.GetEnumerator())
			{
				if (it.MoveNext())
				{
					T last = it.Current;

					while (it.MoveNext())
					{
						T item = it.Current;
						if (comparer.Compare(last, item) > 0)
							return false;
						last = item;
					}
				}
			}

			return true;
		}

		/// <summary>
		/// Given two sequences, finds the longest common slice (i.e., contiguous sub-sequence) contained in both of them, and returns its length.
		/// </summary>
		/// <param name="first">The first sequence.</param>
		/// <param name="second">The second sequence.</param>
		/// <param name="firstIndex">The position within the first sequence at which the longest common slice starts, or -1 if none is found.</param>
		/// <param name="secondIndex">The position within the second sequence at which the longest common slice starts, or -1 if none is found.</param>
		/// <returns>The length of the longest common slice.</returns>
		public static int LongestCommonSlice<T>(IEnumerable<T> first, IEnumerable<T> second, out int firstIndex, out int secondIndex)
		{
			// check arguments
			if (first == null)
				throw new ArgumentNullException(nameof(first));
			if (second == null)
				throw new ArgumentNullException(nameof(second));

			// convert to lists (for easy random access)
			List<T> listFirst = first.ToList();
			List<T> listSecond = second.ToList();
			EqualityComparer<T> comparer = EqualityComparer<T>.Default;

			// no common slice found yet
			firstIndex = -1;
			secondIndex = -1;

			// build table of maximum slice lengths found at each starting position
			int[,] lengths = new int[listFirst.Count, listSecond.Count];
			int nMaxLength = 0;

			// try every pair of starting positions to find the one that gives the maximal length
			for (int nFirst = 0; nFirst < listFirst.Count; nFirst++)
			{
				for (int nSecond = 0; nSecond < listSecond.Count; nSecond++)
				{
					if (!comparer.Equals(listFirst[nFirst], listSecond[nSecond]))
						lengths[nFirst, nSecond] = 0;
					else
					{
						if ((nFirst == 0) || (nSecond == 0))
							lengths[nFirst, nSecond] = 1;
						else
							lengths[nFirst, nSecond] = 1 + lengths[nFirst - 1, nSecond - 1];

						if (lengths[nFirst, nSecond] > nMaxLength)
						{
							nMaxLength = lengths[nFirst, nSecond];
							firstIndex = nFirst - nMaxLength + 1;
							secondIndex = nSecond - nMaxLength + 1;
						}
					}
				}
			}

			return nMaxLength;
		}

		/// <summary>
		/// Finds the maximum element in the specified collection, using the specified comparison.
		/// </summary>
		/// <typeparam name="T">The type of element in the collection.</typeparam>
		/// <param name="sequence">The sequence to search.</param>
		/// <param name="comparison">The comparison function.</param>
		/// <returns>The maximum element, as evaluated by the specified comparision.</returns>
		public static T Max<T>(this IEnumerable<T> sequence, Func<T, T, int> comparison)
		{
			if (sequence == null)
				throw new ArgumentNullException(nameof(sequence));
			if (comparison == null)
				throw new ArgumentNullException(nameof(comparison));

			using (IEnumerator<T> it = sequence.GetEnumerator())
			{
				if (!it.MoveNext())
					throw new ArgumentException("Specified collection has no elements.", nameof(sequence));

				T max = it.Current;

				while (it.MoveNext())
				{
					T current = it.Current;
					if (comparison(current, max) > 0)
						max = current;
				}

				return max;
			}
		}

		/// <summary>
		/// Returns the sum of a sequence of nullable values.
		/// </summary>
		/// <param name="values">A sequence of nullable values.</param>
		/// <returns>The sum of the values, or <c>null</c> if any value is <c>null</c>.</returns>
		public static int? NullableSum(this IEnumerable<int?> values)
		{
			return values.Aggregate((int?) 0, (sum, value) => sum + value);
		}

		/// <summary>
		/// Returns the sum of a sequence of nullable values.
		/// </summary>
		/// <param name="values">A sequence of nullable values.</param>
		/// <returns>The sum of the values, or <c>null</c> if any value is <c>null</c>.</returns>
		public static long? NullableSum(this IEnumerable<long?> values)
		{
			return values.Aggregate((long?) 0, (sum, value) => sum + value);
		}

		/// <summary>
		/// Returns the specified sequence, or null if it is empty.
		/// </summary>
		/// <typeparam name="T">The type of object in the sequence.</typeparam>
		/// <param name="sequence">The sequence.</param>
		/// <returns>The specified sequence, or null if it is empty.</returns>
		public static T NullIfEmpty<T>(this T sequence) where T : class, IEnumerable
		{
			return IsNullOrEmpty(sequence) ? null : sequence;
		}

		/// <summary>
		/// Returns a range of values.
		/// </summary>
		/// <typeparam name="T">The type of object in the sequence.</typeparam>
		/// <param name="start">The initial value.</param>
		/// <param name="count">The number of values.</param>
		/// <param name="increment">Increments the value.</param>
		/// <returns>A sequence of values starting with the initial value.</returns>
		public static IEnumerable<T> Range<T>(T start, int count, Func<T, T> increment)
		{
			if (count < 0)
				throw new ArgumentException("Count must not be negative.", nameof(count));

			return DoRange(start, count, increment);
		}

		private static IEnumerable<T> DoRange<T>(T start, int count, Func<T, T> increment)
		{
			if (count > 0)
			{
				yield return start;

				while (--count > 0)
				{
					start = increment(start);
					yield return start;
				}
			}
		}

		/// <summary>
		/// Returns a range of integers.
		/// </summary>
		/// <param name="start">The initial value.</param>
		/// <param name="stop">The final value.</param>
		/// <returns>A sequence of values starting with the initial value and ending with the final value.</returns>
		/// <remarks>This method will increment or decrement, as necessary. If the initial and final values are the same,
		/// the sequence contains only that value.</remarks>
		public static IEnumerable<int> RangeTo(int start, int stop)
		{
			while (true)
			{
				yield return start;

				if (start < stop)
					start++;
				else if (start == stop)
					break;
				else
					start--;
			}
		}

		/// <summary>
		/// Returns a range of values.
		/// </summary>
		/// <typeparam name="T">The type of object in the sequence.</typeparam>
		/// <param name="start">The initial value.</param>
		/// <param name="stop">The final value.</param>
		/// <param name="increment">Increments the value.</param>
		/// <returns>A sequence of values starting with the initial value and ending with the final value.</returns>
		/// <remarks>If the initial and final values are the same, the sequence contains only that value.</remarks>
		public static IEnumerable<T> RangeTo<T>(T start, T stop, Func<T, T> increment)
		{
			EqualityComparer<T> comparer = EqualityComparer<T>.Default;

			while (true)
			{
				yield return start;

				if (comparer.Equals(start, stop))
					break;

				start = increment(start);
			}
		}

		/// <summary>
		/// Compares two sequences.
		/// </summary>
		/// <typeparam name="T">The type of the elements.</typeparam>
		/// <param name="first">The first sequence.</param>
		/// <param name="second">The second sequence.</param>
		/// <returns>0 if they are equal; less than zero if the first is less than the second; greater than zero if the first is greater than the second.</returns>
		public static int SequenceCompare<T>(this IEnumerable<T> first, IEnumerable<T> second)
		{
			return first.SequenceCompare(second, null);
		}

		/// <summary>
		/// Compares two sequences.
		/// </summary>
		/// <typeparam name="T">The type of the elements.</typeparam>
		/// <param name="first">The first sequence.</param>
		/// <param name="second">The second sequence.</param>
		/// <param name="comparer">The comparer.</param>
		/// <returns>0 if they are equal; less than zero if the first is less than the second; greater than zero if the first is greater than the second.</returns>
		public static int SequenceCompare<T>(this IEnumerable<T> first, IEnumerable<T> second, IComparer<T> comparer)
		{
			if (first == null)
				throw new ArgumentNullException(nameof(first));
			if (second == null)
				throw new ArgumentNullException(nameof(second));

			if (comparer == null)
				comparer = Comparer<T>.Default;

			using (IEnumerator<T> itFirst = first.GetEnumerator())
			using (IEnumerator<T> itSecond = second.GetEnumerator())
			{
				while (itFirst.MoveNext())
				{
					if (!itSecond.MoveNext())
						return 1;

					int nCompare = comparer.Compare(itFirst.Current, itSecond.Current);
					if (nCompare != 0)
						return nCompare;
				}

				if (itSecond.MoveNext())
					return -1;
			}

			return 0;
		}

		/// <summary>
		/// Gets the hash code for a sequence.
		/// </summary>
		/// <typeparam name="T">The type of the elements.</typeparam>
		/// <param name="sequence">The sequence.</param>
		/// <returns>A valid hash code for the sequence, assuming Enumerable.SequenceEqual is used to compare them.</returns>
		/// <remarks>If the sequence is null, zero is returned.</remarks>
		public static int SequenceHashCode<T>(this IEnumerable<T> sequence)
		{
			return sequence.SequenceHashCode(null);
		}

		/// <summary>
		/// Gets the hash code for a sequence.
		/// </summary>
		/// <typeparam name="T">The type of the elements.</typeparam>
		/// <param name="sequence">The sequence.</param>
		/// <param name="comparer">The comparer.</param>
		/// <returns>A valid hash code for the sequence, assuming Enumerable.SequenceEqual is used to compare them.</returns>
		/// <remarks>If the sequence is null, zero is returned.</remarks>
		public static int SequenceHashCode<T>(this IEnumerable<T> sequence, IEqualityComparer<T> comparer)
		{
			if (sequence == null)
				return 0;

			if (comparer == null)
				comparer = EqualityComparer<T>.Default;

			return HashCodeUtility.CombineHashCodes(sequence.Select(x => comparer.GetHashCode(x)).ToArray());
		}

		/// <summary>
		/// Splits the <paramref name="sequence"/> sequence into <paramref name="binCount"/> equal-sized bins.
		/// If <paramref name="binCount"/> does not evenly divide the total element count, then the first
		/// (total count % <paramref name="binCount"/>) bins will have one more element than the following bins.
		/// </summary>
		/// <param name="sequence">The sequence to split.</param>
		/// <param name="binCount">The desired number of bins.</param>
		/// <returns>A sequence of sub-sequences of the original sequence.</returns>
		/// <remarks>
		/// WARNING: Calls Enumerable.Count(), which may enumerate the underlying sequence (if it is not an <see cref="ICollection"/> or array type).
		/// To avoid this, you may want to call EnumerateBatches instead, although
		/// that method saves all "uneveness" to the last batch, where as this one distributes it.
		/// For example, a 12-item sequence split into 5 bins will yield batches of (3, 3, 2, 2, 2);
		/// EnumerateBatches can give us (3, 3, 3, 3) or (2, 2, 2, 2, 2, 2), but cannot give us exactly 5 batches.
		/// </remarks>
		public static IEnumerable<ReadOnlyCollection<T>> SplitIntoBins<T>(this IEnumerable<T> sequence, int binCount)
		{
			if (sequence == null)
				throw new ArgumentNullException(nameof(sequence));
			if (binCount < 1)
				throw new ArgumentOutOfRangeException(nameof(binCount));

			int nSeqCount = sequence.Count();

			// iterator in a separate function causes arguments to be validated immediately
			return SplitIntoBinsIterator(sequence, binCount, nSeqCount);
		}

		// Iterator block for SplitIntoBins method above
		private static IEnumerable<ReadOnlyCollection<T>> SplitIntoBinsIterator<T>(IEnumerable<T> sequence, int nBinCount, int nSeqCount)
		{
			// initial bin size is the sequence count divided by the number of bins, rounded up
			int binSize = nSeqCount / nBinCount;
			int remainder = nSeqCount % nBinCount;
			if (remainder > 0)
				binSize++;

			List<T> batch = new List<T>(binSize);

			foreach (T item in sequence)
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

		/// <summary>
		/// Sorts the sequence. A shortcut for OrderBy(x => x).
		/// </summary>
		/// <param name="sequence">The sequence.</param>
		/// <returns>The sorted sequence.</returns>
		public static IEnumerable<T> Order<T>(this IEnumerable<T> sequence)
		{
			return sequence.OrderBy(x => x);
		}

		/// <summary>
		/// Sorts the sequence. A shortcut for OrderBy(x => x, comparer).
		/// </summary>
		/// <param name="sequence">The sequence.</param>
		/// <param name="comparer">The comparer.</param>
		/// <returns>The sorted sequence.</returns>
		public static IEnumerable<T> Order<T>(this IEnumerable<T> sequence, IComparer<T> comparer)
		{
			return sequence.OrderBy(x => x, comparer);
		}

		/// <summary>
		/// Lazily merges two sorted sequences, maintaining sort order.  Does not remove duplicates.
		/// </summary>
		/// <param name="sequence1">a sorted sequence</param>
		/// <param name="sequence2">a sorted sequence</param>
		/// <param name="comparer">a comparer by which both input sequences must already be sorted, and by which the result will be sorted</param>
		/// <returns>a sorted sequence</returns>
		public static IEnumerable<T> Merge<T>(IEnumerable<T> sequence1, IEnumerable<T> sequence2, IComparer<T> comparer)
		{
			if (sequence1 == null)
				throw new ArgumentNullException(nameof(sequence1));
			if (sequence2 == null)
				throw new ArgumentNullException(nameof(sequence2));
			if (comparer == null)
				throw new ArgumentNullException(nameof(comparer));

			return DoMerge(sequence1, sequence2, comparer);
		}

		/// <summary>
		/// Returns the specified number of items from the end of the sequence.
		/// </summary>
		/// <typeparam name="T">The type of the sequence.</typeparam>
		/// <param name="sequence">The source sequence.</param>
		/// <param name="count">The number of items to take from the end of the sequence.</param>
		/// <returns>A collection with at most <paramref name="count"/> items, taken from the end of the sequence.</returns>
		/// <remarks>The source sequence is only evaluated once.</remarks>
		public static ReadOnlyCollection<T> TakeLast<T>(this IEnumerable<T> sequence, int count)
		{
			if (sequence == null)
				throw new ArgumentNullException(nameof(sequence));
			if (count < 0)
				throw new ArgumentOutOfRangeException(nameof(count));
			else if (count == 0)
				return ListUtility.CreateReadOnlyCollection<T>();

			Queue<T> queue = new Queue<T>(count);
			foreach (T item in sequence)
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
		/// <param name="sequence">The sequence.</param>
		/// <returns>An <see cref="IReadOnlyList{T}"/> containing the items in the sequence.</returns>
		/// <remarks>If the sequence is an <see cref="IReadOnlyList{T}"/>, it is returned directly.
		/// If it is an <see cref="IList{T}"/>, an adapter is created to wrap it. Otherwise, the sequence
		/// is copied into a <see cref="List{T}"/> and then wrapped in a <see cref="ReadOnlyCollection{T}"/>.
		/// This method is useful for forcing evaluation of a potentially lazy sequence while retaining reasonable
		/// performance for sequences that are already an <see cref="IReadOnlyList{T}"/> or <see cref="IList{T}"/>.</remarks>
		public static IReadOnlyList<T> AsReadOnlyList<T>(this IEnumerable<T> sequence) =>
			sequence as IReadOnlyList<T> ??
				(sequence is IList<T> list ?
					(IReadOnlyList<T>) new ReadOnlyListAdapter<T>(list) :
					sequence.ToList().AsReadOnly());

		/// <summary>
		/// Returns a new set of the elements in the specified sequence.
		/// </summary>
		/// <typeparam name="T">The type of element in the source sequence.</typeparam>
		/// <param name="sequence">The sequence.</param>
		/// <returns>A set of elements.</returns>
		/// <remarks>
		/// Note that the <code>AsSet{T}</code> method may be more performant because it will not copy the input to a new HashSet unless it has to.
		/// </remarks>
		public static HashSet<T> ToSet<T>(this IEnumerable<T> sequence)
		{
			if (sequence == null)
				throw new ArgumentNullException(nameof(sequence));

			return new HashSet<T>(sequence);
		}

		/// <summary>
		/// Returns a set of the elements in the specified sequence.
		/// </summary>
		/// <typeparam name="T">The type of element in the source sequence.</typeparam>
		/// <param name="sequence">The sequence.</param>
		/// <returns>A set of elements.</returns>
		/// <remarks>
		/// Unlike the <see cref="ToSet{T}(System.Collections.Generic.IEnumerable{T})"/> method, if the original IEnumerable is a HashSet it will be returned as such (rather than copying to a new HashSet).
		/// </remarks>
		public static ISet<T> AsSet<T>(this IEnumerable<T> sequence)
		{
			if (sequence == null)
				throw new ArgumentNullException(nameof(sequence));

			return sequence as ISet<T> ?? new HashSet<T>(sequence);
		}

		/// <summary>
		/// Returns a set of the elements in the specified sequence, using the specified equality comparer.
		/// </summary>
		/// <typeparam name="T">The type of element in the source sequence.</typeparam>
		/// <param name="sequence">The sequence.</param>
		/// <param name="comparer">The equality comparer.</param>
		/// <returns>A set of elements.</returns>
		public static HashSet<T> ToSet<T>(this IEnumerable<T> sequence, IEqualityComparer<T> comparer)
		{
			if (sequence == null)
				throw new ArgumentNullException(nameof(sequence));
			if (comparer == null)
				throw new ArgumentNullException(nameof(comparer));

			return new HashSet<T>(sequence, comparer);
		}

		/// <summary>
		/// Converts the sequence to a sequence of strings by calling ToString on each item.
		/// </summary>
		/// <param name="sequence">The sequence.</param>
		/// <returns>A sequence of strings.</returns>
		public static IEnumerable<string> ToStrings(this IEnumerable sequence)
		{
			foreach (object item in sequence)
				yield return item.ToString();
		}

		/// <summary>
		/// Converts the sequence to a sequence of strings by calling ToString on each item.
		/// </summary>
		/// <param name="sequence">The sequence.</param>
		/// <returns>A sequence of strings.</returns>
		public static IEnumerable<string> ToStrings<T>(this IEnumerable<T> sequence)
		{
			foreach (T item in sequence)
				yield return item.ToString();
		}

		/// <summary>
		/// Removes items from the end of the specified sequence that match the given predicate.
		/// </summary>
		/// <typeparam name="T">The type of the sequence.</typeparam>
		/// <param name="sequence">The sequence to enumerate</param>
		/// <param name="fnPredicate">The predicate to apply to the items in the sequence.</param>
		/// <returns>The elements of the sequence without the matching items at the end of the sequence.</returns>
		public static IEnumerable<T> TrimEndWhere<T>(this IEnumerable<T> sequence, Func<T, bool> fnPredicate)
		{
			using (IEnumerator<T> it = sequence.GetEnumerator())
			{
				List<T> list = new List<T>();
				while (it.MoveNext())
				{
					T item = it.Current;
					if (fnPredicate(item))
					{
						list.Add(item);
						bool bMatch = true;
						while (bMatch && it.MoveNext())
						{
							item = it.Current;
							bMatch = fnPredicate(item);
							list.Add(item);
						}

						if (!bMatch)
						{
							foreach (T saved in list)
								yield return saved;
							list.Clear();
						}
						else
						{
							break;
						}
					}
					else
					{
						yield return item;
					}
				}
			}
		}

		/// <summary>
		/// Returns true if the sequence is not empty and provides the first element.
		/// </summary>
		/// <typeparam name="T">The type of the elements of <paramref name="sequence"/>.</typeparam>
		/// <param name="sequence">The source sequence.</param>
		/// <param name="found">The first item, if any.</param>
		/// <returns><c>True</c> if the sequence is not empty; otherwise, <c>false</c>.</returns>
		public static bool TryFirst<T>(this IEnumerable<T> sequence, out T found)
		{
			if (sequence == null)
				throw new ArgumentNullException(nameof(sequence));

			foreach (T item in sequence)
			{
				found = item;
				return true;
			}

			found = default(T);
			return false;
		}

		/// <summary>
		/// Determines whether any element of a sequence satisfies a condition.
		/// </summary>
		/// <typeparam name="T">The type of the elements of <paramref name="sequence"/>.</typeparam>
		/// <param name="sequence">The source sequence.</param>
		/// <param name="predicate">The predicate.</param>
		/// <param name="found">The first item that satisfies the predicate, if any.</param>
		/// <returns><c>True</c> if any elements in the source sequence pass the test in the specified predicate; otherwise, <c>false</c>.</returns>
		public static bool TryFirst<T>(this IEnumerable<T> sequence, Func<T, bool> predicate, out T found)
		{
			if (sequence == null)
				throw new ArgumentNullException(nameof(sequence));
			if (predicate == null)
				throw new ArgumentNullException(nameof(predicate));

			foreach (T item in sequence)
			{
				if (predicate(item))
				{
					found = item;
					return true;
				}
			}

			found = default(T);
			return false;
		}

		/// <summary>
		/// Enumerates the specified collection, casting each element to a base type.
		/// </summary>
		/// <param name="sequence">The collection to enumerate.</param>
		/// <returns>The elements of the specified collection, cast to the destination type.</returns>
		/// <remarks>This method can only be used when the source type is derived from the destination type.
		/// Therefore this method will never throw <see cref="InvalidCastException" />.</remarks>
		/// <seealso cref="Downcast{TSource,TDest}" />
		public static IEnumerable<TDest> Upcast<TSource, TDest>(this IEnumerable<TSource> sequence) where TSource : TDest
		{
			if (sequence == null)
				throw new ArgumentNullException(nameof(sequence));
			return sequence.Select<TSource, TDest>(obj => obj);
		}

		/// <summary>
		/// Enumerates the specified collection, returning all the elements that are not null.
		/// </summary>
		/// <param name="sequence">The sequence to enumerate.</param>
		/// <returns>The non null items.</returns>
		public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> sequence)
			where T : class
		{
			if (sequence == null)
				throw new ArgumentNullException(nameof(sequence));
			return sequence.Where(x => x != null);
		}

		/// <summary>
		/// Enumerates the specified collection, returning all the elements that are not null.
		/// </summary>
		/// <param name="sequence">The sequence to enumerate.</param>
		/// <returns>The non null items.</returns>
		public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> sequence)
			where T : struct
		{
			if (sequence == null)
				throw new ArgumentNullException(nameof(sequence));
			return WhereNotNullImpl(sequence);
		}

		private static IEnumerable<T> WhereNotNullImpl<T>(this IEnumerable<T?> sequence)
			where T : struct
		{
			foreach (T? t in sequence)
			{
				if (t.HasValue)
					yield return t.Value;
			}
		}

		/// <summary>
		/// Combines two same sized sequences.
		/// </summary>
		/// <param name="first">An IEnumerable whos elements will be returned as ValueTuple.First.</param>
		/// <param name="second">An IEnumerable whos elements will be returned as ValueTuple.Second.</param>
		/// <returns>A sequence of tuples combining the input items. Throws if the sequences don't have the same number of items.</returns>
		public static IEnumerable<ValueTuple<T1, T2>> Zip<T1, T2>(this IEnumerable<T1> first, IEnumerable<T2> second)
		{
			if (first == null)
				throw new ArgumentNullException(nameof(first));
			if (second == null)
				throw new ArgumentNullException(nameof(second));

			return ZipImpl(first, second, UnbalancedZipStrategy.Throw);
		}

		/// <summary>
		/// Combines two sequences.
		/// </summary>
		/// <param name="first">An IEnumerable whos elements will be returned as ValueTuple.First.</param>
		/// <param name="second">An IEnumerable whos elements will be returned as ValueTuple.Second.</param>
		/// <returns>A sequence of tuples combining the input items. If the sequences don't have the same number of items, it stops at the end of the shorter sequence.</returns>
		public static IEnumerable<ValueTuple<T1, T2>> ZipTruncate<T1, T2>(this IEnumerable<T1> first, IEnumerable<T2> second)
		{
			if (first == null)
				throw new ArgumentNullException(nameof(first));
			if (second == null)
				throw new ArgumentNullException(nameof(second));

			return ZipImpl(first, second, UnbalancedZipStrategy.Truncate);
		}

		/// <summary>
		/// Makes distinct and then removes a single item from a sequence.
		/// </summary>
		/// <param name="sequence">A sequence whose elements not matching the specified item will be returned.</param>
		/// <param name="item">An item that will not occur in the resulting sequence.</param>
		/// <returns>A sequence consisting of every distinct item in the specified sequence that does not match the specified item according to the provided (or default if not provided) equality comparer.</returns>
		public static IEnumerable<T> Except<T>(this IEnumerable<T> sequence, T item)
		{
			return sequence.Except(Enumerate(item));
		}

		/// <summary>
		/// Makes distinct and then removes a single item from a sequence.
		/// </summary>
		/// <param name="sequence">A sequence whose elements not matching the specified item will be returned.</param>
		/// <param name="item">An item that will not occur in the resulting sequence.</param>
		/// <param name="comparer">Comparer used for exclusion. If not provided, default comparer is used.</param>
		/// <returns>A sequence consisting of every distinct item in the specified sequence that does not match the specified item according to the provided (or default if not provided) equality comparer.</returns>
		public static IEnumerable<T> Except<T>(this IEnumerable<T> sequence, T item, IEqualityComparer<T> comparer)
		{
			return sequence.Except(Enumerate(item), comparer);
		}

		/// <summary>
		/// Groups the elements of a sequence according to a specified key selector function.  Each group consists of *consecutive* items having the same key. Order is preserved.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of source.</typeparam>
		/// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
		/// <param name="sequence">A sequence whose elements to group.</param>
		/// <param name="keySelector">A function to extract the key for each element.</param>
		/// <returns>A sequence of IGroupings containing a sequence of objects and a key.</returns>
		public static IEnumerable<IGrouping<TKey, TSource>> GroupConsecutiveBy<TSource, TKey>(this IEnumerable<TSource> sequence, Func<TSource, TKey> keySelector)
		{
			if (sequence == null)
				throw new ArgumentNullException(nameof(sequence));
			if (keySelector == null)
				throw new ArgumentNullException(nameof(keySelector));

			return GroupConsecutiveImpl(sequence, keySelector);
		}

		private static IEnumerable<IGrouping<TKey, TSource>> GroupConsecutiveImpl<TSource, TKey>(this IEnumerable<TSource> sequence, Func<TSource, TKey> keySelector)
		{
			using (IEnumerator<TSource> e = sequence.GetEnumerator())
			{
				if (!e.MoveNext())
					yield break;

				EqualityComparer<TKey> comparer = EqualityComparer<TKey>.Default;

				TKey keyLast = keySelector(e.Current);
				List<TSource> listValues = new List<TSource> { e.Current };

				while (e.MoveNext())
				{
					TKey keyCurrent = keySelector(e.Current);
					if (comparer.Equals(keyLast, keyCurrent))
					{
						listValues.Add(e.Current);
					}
					else
					{
						yield return new Grouping<TKey, TSource>(keyLast, listValues.AsReadOnly());
						keyLast = keyCurrent;
						listValues = new List<TSource> { e.Current };
					}
				}

				yield return new Grouping<TKey, TSource>(keyLast, listValues.AsReadOnly());
			}
		}

		/// <summary>
		/// Groups the elements of a sequence that occur near each other in time.  Items are presumed to be in chronological order and an item is considered part of the group if the time between the previous item is within the provided time span. Thus, the total time between the first and last items in the group may be greater than the time span.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of source.</typeparam>
		/// <param name="sequence">A sequence whose elements to group.</param>
		/// <param name="timeSpan">The difference by which a consecutive element is considered to be in the same group.</param>
		/// <param name="dateSelector">A function to extract the date for each element.</param>
		/// <returns>A sequence of IGroupings containing a sequence of objects and the date of the last element.</returns>
		public static IEnumerable<IGrouping<DateTimeOffset, TSource>> GroupConsecutiveByTimespan<TSource>(this IEnumerable<TSource> sequence, TimeSpan timeSpan, Func<TSource, DateTimeOffset> dateSelector)
		{
			if (sequence == null)
				throw new ArgumentNullException(nameof(sequence));
			if (dateSelector == null)
				throw new ArgumentNullException(nameof(dateSelector));

			return GroupConsecutiveByTimespanImpl(sequence, timeSpan, dateSelector);
		}

		private static IEnumerable<IGrouping<DateTimeOffset, T>> GroupConsecutiveByTimespanImpl<T>(IEnumerable<T> items, TimeSpan timeSpan, Func<T, DateTimeOffset> dateSelector)
		{
			using (IEnumerator<T> e = items.GetEnumerator())
			{
				if (!e.MoveNext())
					yield break;

				DateTimeOffset lastTime = dateSelector(e.Current);
				List<T> groupedItems = new List<T> { e.Current };

				while (e.MoveNext())
				{
					DateTimeOffset date = dateSelector(e.Current);

					if (lastTime.Subtract(date).Duration().CompareTo(timeSpan) <= 0)
					{
						groupedItems.Add(e.Current);
					}
					else
					{
						yield return new Grouping<DateTimeOffset, T>(lastTime, groupedItems.AsReadOnly());
						groupedItems = new List<T> { e.Current };
					}

					lastTime = date;
				}

				yield return new Grouping<DateTimeOffset, T>(lastTime, groupedItems.AsReadOnly());
			}
		}

		private class Grouping<TKey, TSource> : IGrouping<TKey, TSource>
		{
			public Grouping(TKey key, ReadOnlyCollection<TSource> sequence)
			{
				m_key = key;
				m_seq = sequence;
			}

			public IEnumerator<TSource> GetEnumerator()
			{
				return m_seq.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return m_seq.GetEnumerator();
			}

			public TKey Key
			{
				get { return m_key; }
			}

			readonly TKey m_key;
			readonly ReadOnlyCollection<TSource> m_seq;
		}

		private enum UnbalancedZipStrategy
		{
			Throw,
			Truncate,
		}

		private static IEnumerable<ValueTuple<T1, T2>> ZipImpl<T1, T2>(IEnumerable<T1> seqFirst, IEnumerable<T2> seqSecond, UnbalancedZipStrategy strategy)
		{
			using (IEnumerator<T1> e1 = seqFirst.GetEnumerator())
			using (IEnumerator<T2> e2 = seqSecond.GetEnumerator())
			{
				while (e1.MoveNext())
				{
					if (e2.MoveNext())
						yield return ValueTuple.Create(e1.Current, e2.Current);
					else
						if (strategy == UnbalancedZipStrategy.Truncate)
						yield break;
					else
						throw new ArgumentException("Both sequences must be of the same size, first is larger.", nameof(seqFirst));
				}
				if (e2.MoveNext())
					if (strategy == UnbalancedZipStrategy.Truncate)
						yield break;
					else
						throw new ArgumentException("Both sequences must be of the same size, second is larger.", nameof(seqSecond));
			}
		}

		private static IEnumerable<ReadOnlyCollection<T>> EnumerateBatchesIterator<T>(IEnumerable<T> sequence, int batchSize)
		{
			// prepare batches of the desired size
			List<T> batch = new List<T>(batchSize);

			foreach (T item in sequence)
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

		private static IEnumerable<ReadOnlyCollection<T>> EnumerateBatchesIterator<T>(IEnumerable<T> sequence, Func<T, bool> startsNewBatch)
		{
			// prepare batches
			List<T> batch = new List<T>();

			foreach (T item in sequence)
			{
				// return current batch if this item should start a new one
				if (startsNewBatch(item) && batch.Count != 0)
				{
					yield return batch.AsReadOnly();
					batch = new List<T>();
				}

				// add to current batch
				batch.Add(item);
			}

			// return the last (incomplete) batch, if any
			if (batch.Count != 0)
				yield return batch.AsReadOnly();
		}

		private static IEnumerable<T> IntersperseIterator<T>(IEnumerable<T> sequence, T value)
		{
			using (IEnumerator<T> it = sequence.GetEnumerator())
			{
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
		/// A helper function for Segment. Collects all of the values that belong in a single segment.
		/// </summary>
		/// <typeparam name="T">The type of the iterator.</typeparam>
		/// <param name="it">An iterator into the source list.</param>
		/// <param name="fnSplitCondition">The condition upon which the list should terminate</param>
		/// <returns>A list of values contained in a single segment.</returns>
		private static IEnumerable<T> SingleSegment<T>(IEnumerator<T> it, Func<T, bool> fnSplitCondition)
		{
			do
			{
				var item = it.Current;
				if (fnSplitCondition(item))
					break;
				yield return item;
			}
			while (it.MoveNext());
		}

		/// <summary>
		/// A helper function for Merge, which lazily merges two sorted sequences, maintaining sort order.  Does not remove duplicates.
		/// </summary>
		/// <param name="seq1">a sorted sequence</param>
		/// <param name="seq2">a sorted sequence</param>
		/// <param name="comparer">a comparer by which both input sequences must already be sorted, and by which the result will be sorted</param>
		/// <returns>a sorted sequence</returns>
		private static IEnumerable<T> DoMerge<T>(IEnumerable<T> seq1, IEnumerable<T> seq2, IComparer<T> comparer)
		{
			using (IEnumerator<T> enumerator1 = seq1.GetEnumerator())
			using (IEnumerator<T> enumerator2 = seq2.GetEnumerator())
			{
				bool bMore1 = enumerator1.MoveNext();
				bool bMore2 = enumerator2.MoveNext();
				while (bMore1 && bMore2)
				{
					if (comparer.Compare(enumerator1.Current, enumerator2.Current) <= 0)
					{
						yield return enumerator1.Current;
						bMore1 = enumerator1.MoveNext();
					}
					else
					{
						yield return enumerator2.Current;
						bMore2 = enumerator2.MoveNext();
					}
				}
				while (bMore1)
				{
					yield return enumerator1.Current;
					bMore1 = enumerator1.MoveNext();
				}
				while (bMore2)
				{
					yield return enumerator2.Current;
					bMore2 = enumerator2.MoveNext();
				}
			}
		}
	}
}
