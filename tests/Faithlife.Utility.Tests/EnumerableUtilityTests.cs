using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class EnumerableUtilityTests
	{
		[Test]
		public void TryFirstBadArguments()
		{
			int i;
			Assert.Throws<ArgumentNullException>(() => EnumerableUtility.TryFirst(null, n => n == 2, out i));
			Assert.Throws<ArgumentNullException>(() => EnumerableUtility.TryFirst(new int[0], null, out i));
		}

		[Test]
		public void TryFirstInt()
		{
			int found;
			int[] list = new[] { 1, 2, 3, 4 };

			Assert.IsFalse(list.TryFirst(n => n == 10, out found));
			Assert.AreEqual(default(int), found);

			Assert.IsTrue(list.TryFirst(n => n > 2, out found));
			Assert.AreEqual(3, found);
		}

		[Test]
		public void TryFirstString()
		{
			string found;
			string[] list = new[] { "test", "hello", "there", "more" };

			Assert.IsFalse(list.TryFirst(n => n.Length == 10, out found));
			Assert.AreEqual(default(string), found);

			Assert.IsTrue(list.TryFirst(n => n.Length == 5, out found));
			Assert.AreEqual("hello", found);
		}

		[Test]
		public void AppendBadArguments()
		{
			Assert.Throws<ArgumentNullException>(() => EnumerableUtility.Append(default(IEnumerable<int>), 1));
		}

		[Test]
		public void Append()
		{
			CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, new[] { 1, 2, 3 }.Append(4));
			CollectionAssert.AreEqual(new[] { "test", null, "hello", null }, new[] { "test", null, "hello" }.Append(null));
		}

		[Test]
		public void GivenBadArgumentsWhenCallingAppendIfNotAlreadyPresentShouldThrow()
		{
			Assert.Throws<ArgumentNullException>(() => EnumerableUtility.AppendIfNotAlreadyPresent(default(IEnumerable<int>), 1).ToList());
		}

		[Test]
		public void GivenUniqueItemWhenCallingAppendIfNotAlreadyPresentShouldAppend()
		{
			CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, new[] { 1, 2, 3 }.AppendIfNotAlreadyPresent(4));
			CollectionAssert.AreEqual(new[] { "test", "hello", null }, new[] { "test", "hello" }.AppendIfNotAlreadyPresent(null));
		}

		[Test]
		public void GivenNonUniqueItemWhenCallingAppendIfNotAlreadyPresentShouldNotAppend()
		{
			CollectionAssert.AreEqual(new[] { 1, 2, 3 }, new[] { 1, 2, 3 }.AppendIfNotAlreadyPresent(2));
			CollectionAssert.AreEqual(new[] { "test", "hello" }, new[] { "test", "hello" }.AppendIfNotAlreadyPresent("hello"));
		}

		[Test]
		public void GivenNonDefaultComparerWhenCallingAppendIfNotAlreadyPresentShouldUseNonDefaultComparer()
		{
			CollectionAssert.AreEqual(new[] { 1, 2 }, new[] { 1, 2 }.AppendIfNotAlreadyPresent(3, new OddEvenEqualityComparer()));
			CollectionAssert.AreEqual(new[] { 1, 2 }, new[] { 1 }.AppendIfNotAlreadyPresent(2, new OddEvenEqualityComparer()));
		}

		[Test]
		public void GivenBadArgumentsWhenCallingExactlyOneOrDefaultShouldThrow()
		{
			Assert.Throws<ArgumentNullException>(() => EnumerableUtility.ExactlyOneOrDefault(default(IEnumerable<int>)));
			Assert.Throws<ArgumentNullException>(() => EnumerableUtility.ExactlyOneOrDefault(default(IEnumerable<int>), i => i == 1));
			Assert.Throws<ArgumentNullException>(() => new[] { 1, 2 }.ExactlyOneOrDefault(null));
		}

		[Test]
		public void GivenNoItemsExactlyOneOrDefaultShouldReturnDefault()
		{
			Assert.AreEqual(default(string), new string[0].ExactlyOneOrDefault());
		}

		[Test]
		public void GivenOneItemExactlyOneOrDefaultShouldReturnItem()
		{
			Assert.AreEqual("hi", new[] { "hi" }.ExactlyOneOrDefault());
		}

		[Test]
		public void GivenTwoItemsExactlyOneOrDefaultShouldReturnDefault()
		{
			Assert.AreEqual(default(string), new[] { "hi", "hello" }.ExactlyOneOrDefault());
		}

		[Test]
		public void GivenPredicateUniqueToOneItemWhenCallingExactlyOneOrDefaultShouldReturnItem()
		{
			Assert.AreEqual("hi", new[] { "hi", "hello" }.ExactlyOneOrDefault(s => s.Length == 2));
		}

		[Test]
		public void GivenPredicateDescribingMultipleItemsWhenCallingExactlyOneOrDefaultShouldReturnDefault()
		{
			Assert.AreEqual(default(string), new[] { "hi", "hello" }.ExactlyOneOrDefault(s => s.Length < 100));
		}

		[Test]
		public void ToSetGivenBadArgumentShouldThrow()
		{
			Assert.Throws<ArgumentNullException>(() => EnumerableUtility.ToSet(default(IEnumerable<int>)));
		}

		[Test]
		public void ToSetGivenListShouldGenerateSet()
		{
			CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, new[] { 1, 2, 3, 4, 4, 4, 4 }.ToSet().Order());
		}

		[Test]
		public void ToSetGivenHashSetShouldGenerateNewHashSet()
		{
			IEnumerable<int> hashSet = new HashSet<int>(new[] { 1, 2, 3, 4 });
			Assert.AreNotSame(hashSet, hashSet.ToSet());
		}

		[Test]
		public void AsSetGivenBadArgumentShouldThrow()
		{
			Assert.Throws<ArgumentNullException>(() => EnumerableUtility.AsSet(default(IEnumerable<int>)));
		}

		[Test]
		public void AsSetGivenListShouldGenerateSet()
		{
			CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, new[] { 1, 2, 3, 4, 4, 4, 4 }.AsSet().Order());
		}

		[Test]
		public void AsSetGivenHashSetShouldReturnSameHashSet()
		{
			IEnumerable<int> hashSet = new HashSet<int>(new[] { 1, 2, 3, 4 });
			Assert.AreSame(hashSet, hashSet.AsSet());
		}

		[Test]
		public void CountIsExactlyBadArguments()
		{
			Assert.Throws<ArgumentNullException>(() => EnumerableUtility.CountIsExactly(default(IEnumerable<int>), 1));
			Assert.Throws<ArgumentOutOfRangeException>(() => EnumerableUtility.CountIsExactly(new int[0], -1));
		}

		[TestCase(new int[0], 0, true)]
		[TestCase(new int[0], 1, false)]
		[TestCase(new[] { 1, 2 }, 0, false)]
		[TestCase(new[] { 1, 2 }, 1, false)]
		[TestCase(new[] { 1, 2 }, 2, true)]
		[TestCase(new[] { 1, 2 }, 3, false)]
		[TestCase(new[] { 1, 2 }, 4, false)]
		public void CountIsExactly(int[] input, int count, bool expected)
		{
			Assert.AreEqual(expected, input.CountIsExactly(count));
			Assert.AreEqual(expected, ToEnumerable(input).CountIsExactly(count));
		}

		[Test]
		public void CountIsAtLeastBadArguments()
		{
			Assert.Throws<ArgumentNullException>(() => EnumerableUtility.CountIsAtLeast(default(IEnumerable<int>), 1));
			Assert.Throws<ArgumentOutOfRangeException>(() => EnumerableUtility.CountIsAtLeast(new int[0], -1));
		}

		[TestCase(new int[0], 0, true)]
		[TestCase(new int[0], 1, false)]
		[TestCase(new[] { 1, 2 }, 0, true)]
		[TestCase(new[] { 1, 2 }, 1, true)]
		[TestCase(new[] { 1, 2 }, 2, true)]
		[TestCase(new[] { 1, 2 }, 3, false)]
		[TestCase(new[] { 1, 2 }, 4, false)]
		public void CountIsAtLeast(int[] input, int count, bool expected)
		{
			Assert.AreEqual(expected, input.CountIsAtLeast(count));
			Assert.AreEqual(expected, ToEnumerable(input).CountIsAtLeast(count));
		}

		[Test]
		public void ZipTest()
		{
			var seq1 = new[] { 1, 2 };
			var seq2 = new[] { "a", "b" };
			var expected = new[] { ValueTuple.Create(1, "a"), ValueTuple.Create(2, "b") };
			var zipped = seq1.Zip(seq2).ToArray();

			Assert.AreEqual(expected[0], zipped[0]);
			Assert.AreEqual(expected[1], zipped[1]);

			var seqLong = new[] { 1, 2, 3 };
			Assert.Throws<ArgumentException>(() => seq1.Zip(seqLong).ToList());
			Assert.Throws<ArgumentException>(() => seqLong.Zip(seq2).ToList());
			Assert.Throws<ArgumentNullException>(() => seq1.Zip((IEnumerable<int>) null));
			Assert.Throws<ArgumentNullException>(() => EnumerableUtility.Zip((IEnumerable<int>) null, seq1));
		}

		// forces a sequence to be IEnumerable<T>, but not ICollection<T>
		private static IEnumerable<T> ToEnumerable<T>(IEnumerable<T> seq)
		{
			foreach (T t in seq)
				yield return t;
		}

		[Test]
		public void CrossProductBadArguments()
		{
			Assert.Throws<ArgumentNullException>(() => EnumerableUtility.CrossProduct((IEnumerable<IEnumerable<int>>) null));
			Assert.Throws<ArgumentException>(() => EnumerableUtility.CrossProduct(Enumerable.Empty<IEnumerable<int>>()));
		}

		[Test]
		public void CrossProduct1()
		{
			var expected = new[]
			{
				new[] { 1 },
				new[] { 2 },
			};

			var s1 = new[] { 1, 2 };

			DoTestCrossProduct(expected, s1);
		}

		[Test]
		public void CrossProduct3()
		{
			var expected = new[]
			{
				new[] { 1, 4, 6 },
				new[] { 1, 4, 7 },
				new[] { 1, 4, 8 },
				new[] { 1, 5, 6 },
				new[] { 1, 5, 7 },
				new[] { 1, 5, 8 },
				new[] { 2, 4, 6 },
				new[] { 2, 4, 7 },
				new[] { 2, 4, 8 },
				new[] { 2, 5, 6 },
				new[] { 2, 5, 7 },
				new[] { 2, 5, 8 },
				new[] { 3, 4, 6 },
				new[] { 3, 4, 7 },
				new[] { 3, 4, 8 },
				new[] { 3, 5, 6 },
				new[] { 3, 5, 7 },
				new[] { 3, 5, 8 },
			};

			var s1 = new[] { 1, 2, 3 };
			var s2 = new[] { 4, 5 };
			var s3 = new[] { 6, 7, 8 };

			DoTestCrossProduct(expected, s1, s2, s3);
		}

		private static void DoTestCrossProduct(int[][] expected, params int[][] sequences)
		{
			var actual = sequences.CrossProduct<int[], int>();

			var expectedStrings = expected.Select(a => a.Select(i => i.ToString()).Join(", "));
			var actualStrings = actual.Select(cp => cp.Select(i => i.ToString()).Join(", "));

			CollectionAssert.AreEqual(expectedStrings, actualStrings);
		}

		[Test]
		public void DistinctBy()
		{
			Assert.Throws<ArgumentNullException>(() => ((IEnumerable<int>) null).DistinctBy(i => i));
			Assert.Throws<ArgumentNullException>(() => new[] { 1 }.DistinctBy((Func<int, int>) null));

			CollectionAssert.AreEqual(new[] { 2.1, 1, 2, 3, 1.4 }.DistinctBy(d => Math.Floor(d)), new[] { 2.1, 1, 3 });
			CollectionAssert.AreEqual(new double[] { 1, 2, 1, 3, 1 }.DistinctBy(d => Math.Floor(d)), new double[] { 1, 2, 3 });
			CollectionAssert.AreEqual(new double[] { 1, 2, 1, 3, 1 }.DistinctBy(d => Math.Floor(d), null), new double[] { 1, 2, 3 });

			CollectionAssert.AreEqual(new[] { "a", "ab", "abc", "abcd" }.DistinctBy(s => s.Length), new[] { "a", "ab", "abc", "abcd" });
			CollectionAssert.AreEqual(new[] { "a", "b", "c", "ab", "abc", "abcd", "bcd", }.DistinctBy(s => s.Length), new[] { "a", "ab", "abc", "abcd" });
		}

		[Test]
		public void DistinctByComparerArgumentNull()
		{
			Assert.Throws<ArgumentNullException>(() => ((IEnumerable<int>) null).DistinctBy(i => i, EqualityComparer<int>.Default));
			Assert.Throws<ArgumentNullException>(() => new[] { 1 }.DistinctBy(null, EqualityComparer<int>.Default));
		}

		[Test]
		public void DistinctByComparer()
		{
			CollectionAssert.AreEqual(new[] { "a", "ab" },
				new[] { "a", "ab", "abc", "abcd" }.DistinctBy(s => s.Length, new OddEvenEqualityComparer()));
			CollectionAssert.AreEqual(new[] { "a", "ab" },
				new[] { "a", "b", "c", "ab", "abc", "abcd", "bcd", }.DistinctBy(s => s.Length, new OddEvenEqualityComparer()));
		}

		private class OddEvenEqualityComparer : EqualityComparer<int>
		{
			public override bool Equals(int x, int y)
			{
				return (x % 2 == 0) == (y % 2 == 0);
			}

			public override int GetHashCode(int obj)
			{
				return obj % 2;
			}
		}

		[Test]
		public void DowncastTest()
		{
			Base[] coll = new Base[] { new Derived(1), new Derived(2) };
			List<Derived> list = new List<Derived>(coll.Downcast<Base, Derived>());
			Assert.AreEqual(1, list[0].Value);
			Assert.AreEqual(2, list[1].Value);
		}

		[Test]
		public void DowncastValueToValueTest()
		{
			int[] coll = new[] { 1, 2 };
			List<int> list = new List<int>(coll.Downcast<int, int>());
			Assert.AreEqual(1, list[0]);
			Assert.AreEqual(2, list[1]);
		}

		[Test]
		public void DowncastObjectToValueTest()
		{
			object[] coll = new object[] { 1, 2 };
			List<int> list = new List<int>(coll.Downcast<object, int>());
			Assert.AreEqual(1, list[0]);
			Assert.AreEqual(2, list[1]);
		}

		[Test]
		public void FailedDowncastTest()
		{
			Base[] coll = new[] { new Derived(1), new Base(2) };
			Assert.Throws<InvalidCastException>(() => coll.Downcast<Base, Derived>().ToList());
		}

		[Test]
		public void DowncastNull()
		{
			Assert.Throws<ArgumentNullException>(() => EnumerableUtility.Downcast<object, object>(null));
		}

		[Test]
		public void IsNullOrEmptyEnumerable()
		{
			Assert.IsTrue(EnumerableUtility.IsNullOrEmpty(default(IEnumerable)));
			Assert.IsTrue(EnumerateInts(0).IsNullOrEmpty());
			Assert.IsFalse(EnumerateInts(100).IsNullOrEmpty());
		}

		[Test]
		public void IsNullOrEmptyEnumerableT()
		{
			Assert.IsTrue(EnumerableUtility.IsNullOrEmpty(default(IEnumerable<int>)));
			Assert.AreEqual(true, EnumerateEmpty<int>().IsNullOrEmpty());
			Assert.AreEqual(false, InfiniteInts().IsNullOrEmpty());
		}

		[Test]
		public void IsNullOrEmptyCollectionT()
		{
			Assert.IsTrue(EnumerableUtility.IsNullOrEmpty(default(int[])));
			Assert.IsTrue(new int[0].IsNullOrEmpty());
			Assert.IsFalse(new[] { 1 }.IsNullOrEmpty());
		}

		[Test]
		public void EmptyIfNull()
		{
			CollectionAssert.AreEqual(new int[0], ((int[]) null).EmptyIfNull());
			CollectionAssert.AreEqual(new int[0], new int[0].EmptyIfNull());
			CollectionAssert.AreEqual(new[] { 1 }, new[] { 1 }.EmptyIfNull());
		}

		[Test]
		public void EmptyIfNullEnumerable()
		{
			IEnumerable seq = ((IEnumerable) null).EmptyIfNull();
			IEnumerator it = seq.GetEnumerator();
			Assert.IsFalse(it.MoveNext());
		}

		[Test]
		public void Enumerate()
		{
			CollectionAssert.AreEqual(new[] { 1 }, EnumerableUtility.Enumerate(1));
			CollectionAssert.AreEqual(new string[] { null }, EnumerableUtility.Enumerate(default(string)));

			CollectionAssert.AreEqual(new[] { 1, 2, 3 }, EnumerableUtility.Enumerate(1, 2, 3));
			CollectionAssert.AreEqual(new[] { "hello", null, "world" }, EnumerableUtility.Enumerate("hello", null, "world"));
		}

		[Test]
		public void EnumerateTest()
		{
			int nTotal = 0;
			foreach (int n in EnumerableUtility.Enumerate(1, 2, 4, 8))
				nTotal += n;
			Assert.AreEqual(15, nTotal);
		}

		[Test]
		public void EnumerateBatchesNull()
		{
			Assert.Throws<ArgumentNullException>(() => EnumerableUtility.EnumerateBatches<int>(null, 1));
		}

		[Test]
		public void EnumerateBatchesInvalidBatchSize()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => EnumerableUtility.EnumerateBatches(new int[0], 0));
		}

		/// <summary>
		/// Tests that EnumerateBatches:
		/// - Enumerates the original sequence in order.
		/// - Yields batches of the desired size.
		/// - Enumerates the original sequence exactly once.
		/// </summary>
		[Test]
		public void EnumerateBatches()
		{
			const int nBatchSize = 3;
			int[] coll = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
			EnumerableMonitor<int> seq = new EnumerableMonitor<int>(coll);

			int nCurrentIndex = 0;
			foreach (IEnumerable<int> batch in EnumerableUtility.EnumerateBatches(seq, nBatchSize))
			{
				foreach (int nValue in batch)
				{
					Assert.AreEqual(coll[nCurrentIndex], nValue);
					int requestCount = Math.Min(((nCurrentIndex / nBatchSize) + 1) * nBatchSize, coll.Length);
					Assert.AreEqual(requestCount, seq.RequestCount);
					nCurrentIndex++;
				}

				// if we're not at the end, ensure that the batch was the right size
				if (nCurrentIndex != coll.Length)
					Assert.AreEqual(0, nCurrentIndex % nBatchSize);
			}

			Assert.AreEqual(nCurrentIndex, coll.Length);
			Assert.AreEqual(nCurrentIndex, seq.RequestCount);
		}

		[Test]
		public void EnumerateBatchesTests()
		{
			string[] testStrings = { "a", "b", "c", "d", "e", "f", "g", "h" };

			// "should" yield 3 batches with counts (3, 3, 2)
			IEnumerable<ReadOnlyCollection<string>> batches = testStrings.EnumerateBatches(3);

			// how many batches were returned? Count() should give us a definite answer
			Assert.AreEqual(3, batches.Count());

			// enumerating the outer sequence first shouldn't mess up the batches
			List<ReadOnlyCollection<string>> outerToList = batches.ToList();
			Assert.AreEqual(3, outerToList.Count());

			// enumerating the inner sequence first should behave as expected (just like enumerating the nested sequence "in order")
			IEnumerable<List<string>> innerToList = batches.Select(batch => batch.ToList());
			Assert.AreEqual(3, innerToList.Count());

			// enumerating innerToList yields the correct values
			int counter = 0;
			foreach (List<string> batch in innerToList)
				foreach (string actualString in batch)
					Assert.AreEqual(testStrings[counter++], actualString);

			foreach (IEnumerable<string> batch in outerToList)
			{
				Assert.IsNotNull(batch);
				Assert.IsTrue(batch.Any());
				using (IEnumerator<string> enumerator = batch.GetEnumerator())
				{
					Assert.IsNotNull(enumerator);
					Assert.IsTrue(enumerator.MoveNext());
				}
			}

			// what if we just want the first two items from each batch?
			int batchCount = 0;
			foreach (IEnumerable<string> batch in batches)
			{
				batchCount++;
				for (int i = 0; i < 2; i++)
					Assert.AreEqual(1, batch.ElementAt(i).Length);
			}
			Assert.AreEqual(3, batchCount);
		}

		[Test]
		public void EnumerateSingleBatch()
		{
			// test single batch with or without optimization
			string[] testStrings = { "a", "b", "c", "d", "e", "f", "g", "h" };
			IEnumerable<ReadOnlyCollection<string>> batches = testStrings.EnumerateBatches(testStrings.Length);
			CollectionAssert.AreEqual(testStrings, batches.Single());
			batches = testStrings.Select(x => x).EnumerateBatches(testStrings.Length);
			CollectionAssert.AreEqual(testStrings, batches.Single());
		}

		[Test]
		public void EnumerateEmptyBatch()
		{
			// test empty batch with or without optimization
			string[] testStrings = new string[0];
			IEnumerable<ReadOnlyCollection<string>> batches = testStrings.EnumerateBatches(1);
			Assert.IsFalse(batches.Any());
			batches = testStrings.Select(x => x).EnumerateBatches(1);
			Assert.IsFalse(batches.Any());
		}

		[Test]
		public void EnumerateBatchesWithPrimeFunc()
		{
			int[] coll = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
			List<ReadOnlyCollection<int>> batches =
				coll.EnumerateBatches(n => n == 1 || n == 2 || n == 3 || n == 5 || n == 7).ToList();
			Assert.AreEqual(5, batches.Count);
			CollectionAssert.AreEqual(new[] { 1 }, batches[0]);
			CollectionAssert.AreEqual(new[] { 2 }, batches[1]);
			CollectionAssert.AreEqual(new[] { 3, 4 }, batches[2]);
			CollectionAssert.AreEqual(new[] { 5, 6 }, batches[3]);
			CollectionAssert.AreEqual(new[] { 7, 8, 9, 10 }, batches[4]);
		}

		[Test]
		public void EnumerateBatchesWithFalseFunc()
		{
			int[] coll = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
			List<ReadOnlyCollection<int>> batches = coll.EnumerateBatches(n => false).ToList();
			Assert.AreEqual(1, batches.Count);
			CollectionAssert.AreEqual(coll, batches[0]);
		}

		[Test]
		public void EnumerateBatchesWithTrueFunc()
		{
			int[] coll = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
			List<ReadOnlyCollection<int>> batches = coll.EnumerateBatches(n => true).ToList();
			Assert.AreEqual(10, batches.Count);
			for (int n = 0; n < 10; n++)
				CollectionAssert.AreEqual(new[] { n + 1 }, batches[n]);
		}

		[Test]
		public void EnumerateBatchesWithFuncEmpty()
		{
			int[] coll = new int[0];
			List<ReadOnlyCollection<int>> batches = coll.EnumerateBatches(n => true).ToList();
			Assert.AreEqual(0, batches.Count);
		}

		[Test]
		public void SplitIntoBinsNull()
		{
			Assert.Throws<ArgumentNullException>(() => EnumerableUtility.SplitIntoBins<int>(null, 1));
		}

		[Test]
		public void SplitIntoBinsInvalidBatchSize()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => EnumerableUtility.SplitIntoBins<int>(new int[0], 0));
		}

		/// <summary>
		/// Tests that SplitIntoBins:
		/// - Enumerates the original sequence in order.
		/// - Enumerates the original sequence the expected number of times (twice, in general, because of the Count() call).
		/// - Yields the correct number of bins.
		/// - Yields bins of the correct size.
		/// </summary>
		[Test]
		public void SplitIntoBinsEnumerable()
		{
			int[] coll = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
			const int nBinCount = 5;
			int[] expectedBinSizes = { 3, 3, 2, 2, 2 };
			EnumerableMonitor<int> seq = new EnumerableMonitor<int>(coll);
			int baseRequestCount = coll.Length; // SplitIntoBins calls Count(), which enumerates the sequence

			int nCurrentIndex = 0;
			int nCurrentBin = 0;
			foreach (IEnumerable<int> bin in EnumerableUtility.SplitIntoBins<int>(seq, nBinCount))
			{
				int nBinSize = 0;
				foreach (int nValue in bin)
				{
					Assert.AreEqual(coll[nCurrentIndex], nValue);
					nCurrentIndex++;
					nBinSize++;
				}
				Assert.AreEqual(nCurrentIndex + baseRequestCount, seq.RequestCount);
				Assert.AreEqual(expectedBinSizes[nCurrentBin], nBinSize);
				nCurrentBin++;
			}

			Assert.AreEqual(nBinCount, nCurrentBin);
			Assert.AreEqual(coll.Length, nCurrentIndex);
			Assert.AreEqual(nCurrentIndex + baseRequestCount, seq.RequestCount);
		}

		/// <summary>
		/// Tests that SplitIntoBins:
		/// - Enumerates the original sequence in order.
		/// - Enumerates the original sequence the expected number of times (once total, because Count() is constant time for Collections).
		/// </summary>
		[Test]
		public void SplitIntoBinsCollection()
		{
			int[] coll = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
			const int nBinCount = 5;
			CollectionMonitor<int> seq = new CollectionMonitor<int>(coll);

			int nCurrentIndex = 0;
			foreach (IEnumerable<int> batch in EnumerableUtility.SplitIntoBins<int>(seq, nBinCount))
			{
				foreach (int nValue in batch)
				{
					Assert.AreEqual(coll[nCurrentIndex], nValue);
					nCurrentIndex++;
				}
				Assert.AreEqual(nCurrentIndex, seq.RequestCount);
			}

			Assert.AreEqual(coll.Length, nCurrentIndex);
			Assert.AreEqual(nCurrentIndex, seq.RequestCount);
		}

		[Test]
		public void SplitIntoBinsTests()
		{
			string[] testStrings = { "a", "b", "c", "d", "e", "f", "g", "h" };

			// "should" yield 3 batches with counts (3, 3, 2)
			IEnumerable<ReadOnlyCollection<string>> batches = testStrings.SplitIntoBins(3);

			// how many batches were returned? Count() should give us a definite answer
			Assert.AreEqual(3, batches.Count());

			// enumerating the outer sequence first shouldn't mess up the batches
			List<ReadOnlyCollection<string>> outerToList = batches.ToList();
			Assert.AreEqual(3, outerToList.Count());

			// enumerating the inner sequence first should behave as expected (just like enumerating the nested sequence "in order")
			IEnumerable<List<string>> innerToList = batches.Select(batch => batch.ToList());
			Assert.AreEqual(3, innerToList.Count());

			// enumerating innerToList yields the correct values
			int counter = 0;
			foreach (List<string> batch in innerToList)
				foreach (string actualString in batch)
					Assert.AreEqual(testStrings[counter++], actualString);

			foreach (IEnumerable<string> batch in outerToList)
			{
				Assert.IsNotNull(batch);
				Assert.IsTrue(batch.Any());
				using (IEnumerator<string> enumerator = batch.GetEnumerator())
				{
					Assert.IsNotNull(enumerator);
					Assert.IsTrue(enumerator.MoveNext());
				}
			}

			// what if we just want the first two items from each batch?
			int batchCount = 0;
			foreach (IEnumerable<string> batch in batches)
			{
				batchCount++;
				for (int i = 0; i < 2; i++)
					Assert.AreEqual(1, batch.ElementAt(i).Length);
			}
			Assert.AreEqual(3, batchCount);
		}

		[Test]
		public void IntersperseNull()
		{
			Assert.Throws<ArgumentNullException>(() => ((int[]) null).Intersperse(0));
		}

		[TestCase(new[] { 0 }, 42)]
		[TestCase(new[] { 1, 2 }, 99)]
		[TestCase(new[] { 5, 4, 3, 2, 1 }, -1)]
		public void Intersperse(int[] anNumbers, int nInterspersed)
		{
			int nItems = 0;
			foreach (int nCurrent in anNumbers.Intersperse(nInterspersed))
			{
				nItems++;

				if (nItems % 2 == 0)
					Assert.AreEqual(nInterspersed, nCurrent);
				else
					Assert.AreEqual(anNumbers[nItems / 2], nCurrent);
			}

			Assert.AreEqual(anNumbers.Length + (anNumbers.Length - 1), nItems);
		}

		[Test]
		public void IsSorted()
		{
			var ascending = new[] { 1, 2, 3, 4, 5, 6 };
			var descending = new[] { 6, 5, 4, 3, 2, 1 };
			var neither = new[] { 6, 1, 5, 2, 4, 3 };

			Assert.IsTrue(ascending.IsSorted());
			Assert.IsFalse(descending.IsSorted());
			Assert.IsFalse(neither.IsSorted());

			var inverse = ComparisonUtility.CreateComparer<int>((x, y) => -x.CompareTo(y));

			Assert.IsFalse(ascending.IsSorted(inverse));
			Assert.IsTrue(descending.IsSorted(inverse));
			Assert.IsFalse(neither.IsSorted(inverse));
		}

		[Test]
		public void ForEach()
		{
			IEnumerable<int> seq1 = new[] { 1, 2, 3, 4, 5, 6 };
			List<int> list = new List<int>();
			IEnumerable<int> seqExpected = new[] { 1, 4, 9, 16, 25, 36 };

			seq1.ForEach(n => list.Add(n * n));

			CollectionAssert.AreEqual(seqExpected, list);
		}

		[Test]
		public void ForEachWithIndex()
		{
			IEnumerable<int> seq1 = new[] { 1, 2, 3, 4, 5, 6 };
			List<int> list = new List<int>();
			IEnumerable<int> seqExpected = new[] { 0, 1, 4, 9, 16, 25 };

			seq1.ForEach((_, n) => list.Add(n * n));

			CollectionAssert.AreEqual(seqExpected, list);
		}

		[Test]
		public void LongestCommonSliceBadArguments()
		{
			int nFirstIndex;
			int nSecondIndex;
			Assert.Throws<ArgumentNullException>(() => EnumerableUtility.LongestCommonSlice(null, new[] { 1 }, out nFirstIndex, out nSecondIndex));
			Assert.Throws<ArgumentNullException>(() => EnumerableUtility.LongestCommonSlice(new[] { 1 }, null, out nFirstIndex, out nSecondIndex));
		}

		[TestCase(new[] { 1, 2, 3 }, new[] { 1, 2, 3 }, 0, 0, 3)]
		[TestCase(new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, -1, -1, 0)]
		[TestCase(new[] { 1, 2, 3, 4 }, new[] { 3, 4, 5, 6 }, 2, 0, 2)]
		[TestCase(new[] { 1, 2, 3, 4 }, new[] { 5, 6, 1 }, 0, 2, 1)]
		public void LongestCommonSlice(int[] first, int[] second, int posFirstExpected, int posSecondExpected, int nLengthExpected)
		{
			int posFirst;
			int posSecond;
			int nLength = EnumerableUtility.LongestCommonSlice(first, second, out posFirst, out posSecond);
			Assert.AreEqual(posFirstExpected, posFirst);
			Assert.AreEqual(posSecondExpected, posSecond);
			Assert.AreEqual(nLengthExpected, nLength);
		}

		[Test]
		public void MaxNullCollection()
		{
			Assert.Throws<ArgumentNullException>(() => ((int[]) null).Max((first, second) => first.CompareTo(second)));
		}

		[Test]
		public void MaxNullComparison()
		{
			Assert.Throws<ArgumentNullException>(() => new int[0].Max(null));
		}

		[TestCase(1, new[] { 1 })]
		[TestCase(5, new[] { 5 })]
		[TestCase(9, new[] { 1, 2, 1, 9, 2 })]
		public void Max(int nExpectedMax, int[] anNumbers)
		{
			Assert.AreEqual(nExpectedMax, anNumbers.Max((left, right) => left.CompareTo(right)));
		}

		[Test]
		public void MaxEmpty()
		{
			Assert.Throws<ArgumentException>(() => (new int[0]).Max((left, right) => left.CompareTo(right)));
		}

		[Test]
		public void ToStrings()
		{
			CollectionAssert.AreEqual(new[] { "1", "2" }, new[] { 1, 2 }.ToStrings());
			CollectionAssert.AreEqual(new[] { "1", "2" }, (new[] { 1, 2 } as IEnumerable).ToStrings());
		}

		[Test]
		public void NullIfEmptyNull()
		{
			IEnumerable nullSequence = null;
			Assert.IsNull(nullSequence.NullIfEmpty());
		}

		[Test]
		public void NullIfEmptyArray()
		{
			Assert.IsNull(new int[0].NullIfEmpty());
			CollectionAssert.AreEqual(EnumerableUtility.Enumerate(1), new[] { 1 }.NullIfEmpty());
		}

		[Test]
		public void NullIfEmptySequence()
		{
			SequenceEnumerable sequenceEnumerable = new SequenceEnumerable(0);
			Assert.IsNull(sequenceEnumerable.NullIfEmpty());
			Assert.AreEqual(1, sequenceEnumerable.DisposeCount);

			sequenceEnumerable = new SequenceEnumerable(2);
			IEnumerable nullIfEmpty = sequenceEnumerable.NullIfEmpty();
			Assert.AreEqual(1, sequenceEnumerable.DisposeCount);
			CollectionAssert.AreEqual(EnumerableUtility.Enumerate(1, 2), nullIfEmpty.Cast<int>());
		}

		private class SequenceEnumerable : IEnumerable
		{
			public SequenceEnumerable(int size)
			{
				m_size = size;
			}

			public IEnumerator GetEnumerator()
			{
				return new SequenceEnumerator(this);
			}

			public int DisposeCount { get; private set; }

			private class SequenceEnumerator : IEnumerator, IDisposable
			{
				public SequenceEnumerator(SequenceEnumerable enumerable)
				{
					m_enumerable = enumerable;
					m_index = 0;
				}

				public bool MoveNext()
				{
					if (m_index < m_enumerable.m_size)
					{
						m_index++;
						return true;
					}
					else
					{
						return false;
					}
				}

				public void Reset()
				{
					throw new NotImplementedException();
				}

				public object Current
				{
					get
					{
						if (m_index < 0 || m_index > m_enumerable.m_size)
							throw new InvalidOperationException();
						return m_index;
					}
				}

				public void Dispose()
				{
					m_enumerable.DisposeCount++;
					m_index = -1;
				}

				readonly SequenceEnumerable m_enumerable;
				int m_index;
			}

			readonly int m_size;
		}

		[Test]
		public void PrependBadArguments()
		{
			Assert.Throws<ArgumentNullException>(() => EnumerableUtility.Prepend(default(IEnumerable<int>), 1));
		}

		[Test]
		public void Prepend()
		{
			CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, new[] { 2, 3, 4 }.Prepend(1));
			CollectionAssert.AreEqual(new[] { null, "test", null, "hello" }, new[] { "test", null, "hello" }.Prepend(null));
		}

		[Test]
		public void SequenceCompare()
		{
			var ascending = new[] { 1, 2, 3, 4, 5, 6 };
			var neither = new[] { 1, 6, 5, 2, 3, 4 };

			Assert.Less(ascending.SequenceCompare(neither), 0);
			Assert.Less(0, neither.SequenceCompare(ascending));

			Assert.AreEqual(0, ascending.SequenceCompare(neither.OrderBy(x => x)));
			Assert.AreEqual(0, neither.OrderBy(x => x).SequenceCompare(ascending));

			Assert.AreEqual(0, ascending.SequenceCompare(neither, ComparisonUtility.CreateComparer<int>((x, y) => (x % 2).CompareTo(y % 2))));
		}

		[Test]
		public void SequenceHashCode()
		{
			var ascending = new[] { 1, 2, 3, 4, 5, 6 };
			var neither = new[] { 1, 6, 5, 2, 3, 4 };

			Assert.AreNotEqual(ascending.SequenceHashCode(), neither.SequenceHashCode()); // not guaranteed, but should be very unlikely
			Assert.AreEqual(ascending.SequenceHashCode(), neither.OrderBy(x => x).SequenceHashCode());

			var equality = ObjectUtility.CreateEqualityComparer<int>((x, y) => x % 2 == y % 2, x => x % 2);
			Assert.AreEqual(ascending.SequenceHashCode(equality), neither.SequenceHashCode(equality));
		}

		[Test]
		public void TakeLastArguments()
		{
			Assert.Throws<ArgumentNullException>(() => ((IEnumerable<int>) null).TakeLast(1));
			Assert.Throws<ArgumentOutOfRangeException>(() => new[] { 1 }.TakeLast(-1));
		}

		[Test]
		public void TakeLastZero()
		{
			CollectionAssert.AreEqual(new int[0], new[] { 1 }.TakeLast(0));
		}

		[Test]
		public void TakeLastEmpty()
		{
			CollectionAssert.AreEqual(new int[0], Enumerable.Empty<int>().TakeLast(100));
		}

		[Test]
		public void TakeLastPartialSequence()
		{
			var sequence = new EnumerableMonitor<int>(Enumerable.Range(1, 100));
			CollectionAssert.AreEqual(new[] { 98, 99, 100 }, sequence.TakeLast(3));
			Assert.AreEqual(100, sequence.RequestCount);
		}

		[Test]
		public void TakeLastEntireSequence()
		{
			var sequence = new EnumerableMonitor<int>(Enumerable.Range(1, 3));
			CollectionAssert.AreEqual(new[] { 1, 2, 3 }, sequence.TakeLast(5));
			Assert.AreEqual(3, sequence.RequestCount);
		}

		[Test]
		public void ReadOnlyCollectionToReadOnlyCollection()
		{
			// ToReadOnlyCollection should not rewrap a ReadOnlyCollection
			List<int> list = new List<int> { 1, 2 };
			IEnumerable<int> readOnlyList = new ReadOnlyCollection<int>(list);
			Assert.AreSame(readOnlyList, readOnlyList.ToReadOnlyCollection());
		}

		[Test]
		public void ListToReadOnlyCollection()
		{
			// ToReadOnlyCollection should not duplicate an IList
			List<int> list = new List<int> { 1, 2 };
			ReadOnlyCollection<int> readOnlyList = list.ToReadOnlyCollection();
			list.Add(3);
			Assert.AreEqual(3, readOnlyList.Count);
		}

		[Test]
		public void DictionaryToReadOnlyCollection()
		{
			// ToReadOnlyCollection must duplicate a non-IList
			Dictionary<int, int> dictionary = new Dictionary<int, int> { { 2, 4 } };
			ReadOnlyCollection<KeyValuePair<int, int>> readOnlyList = dictionary.ToReadOnlyCollection();
			dictionary.Add(3, 9);
			Assert.AreEqual(1, readOnlyList.Count);
			Assert.AreEqual(new KeyValuePair<int, int>(2, 4), readOnlyList[0]);
		}

		[Test]
		public void TrimEndWhere()
		{
			var anItems = new[] { 1, 2, 3, 4, 5 };
			var listResults = anItems.TrimEndWhere(n => n > 3).ToList();

			Assert.AreEqual(3, listResults.Count);
			Assert.AreEqual(1, listResults[0]);
			Assert.AreEqual(2, listResults[1]);
			Assert.AreEqual(3, listResults[2]);

			anItems = new[] { 1, 2, 4, 3, 4, 5 };
			listResults = anItems.TrimEndWhere(n => n > 3).ToList();

			Assert.AreEqual(4, listResults.Count);
			Assert.AreEqual(1, listResults[0]);
			Assert.AreEqual(2, listResults[1]);
			Assert.AreEqual(4, listResults[2]);
			Assert.AreEqual(3, listResults[3]);

			anItems = new[] { 1, 4, 5, 2, 3, 4, 5 };
			listResults = anItems.TrimEndWhere(n => n > 3).ToList();

			Assert.AreEqual(5, listResults.Count);
			Assert.AreEqual(1, listResults[0]);
			Assert.AreEqual(4, listResults[1]);
			Assert.AreEqual(5, listResults[2]);
			Assert.AreEqual(2, listResults[3]);
			Assert.AreEqual(3, listResults[4]);
		}

		[Test]
		public void UpcastTest()
		{
			Derived[] coll = new[] { new Derived(1), new Derived(2) };
			List<Base> list = new List<Base>(coll.Upcast<Derived, Base>());
			Assert.AreEqual(1, list[0].Value);
			Assert.AreEqual(2, list[1].Value);
		}

		[Test]
		public void UpcastValueToValueTest()
		{
			int[] coll = new[] { 1, 2 };
			List<int> list = new List<int>(coll.Upcast<int, int>());
			Assert.AreEqual(1, list[0]);
			Assert.AreEqual(2, list[1]);
		}

		[Test]
		public void UpcastValueToObjectTest()
		{
			int[] coll = new[] { 1, 2 };
			List<object> list = new List<object>(coll.Upcast<int, object>());
			Assert.AreEqual(1, list[0]);
			Assert.AreEqual(2, list[1]);
		}

		[Test]
		public void UpcastNull()
		{
			Assert.Throws<ArgumentNullException>(() => EnumerableUtility.Upcast<object, object>(null));
		}

		[Test]
		public void WhereNotNullClass()
		{
			IEnumerable<string> input = new[] { "this", null, "is", "a", null, "test", null };
			CollectionAssert.AreEqual("this is a test".SplitOnWhitespace(), input.WhereNotNull());

			Assert.AreEqual(0, new string[] { null, null, null }.WhereNotNull().Count());
		}

		[Test]
		public void WhereNotNullStruct()
		{
			IEnumerable<int?> input = new int?[] { 0, null, 1, 2, null, 3, null };
			CollectionAssert.AreEqual(Enumerable.Range(0, 4), input.WhereNotNull());

			Assert.AreEqual(0, new int?[] { null, null, null }.WhereNotNull().Count());
		}

		[Test]
		public void GroupConsecutiveByTest1()
		{
			IEnumerable<int> seq = InfiniteInts();
			IEnumerable<IGrouping<int, int>> groups = seq.GroupConsecutiveBy(n => n / 3);
			int nExpectedKey = 0;
			foreach (IGrouping<int, int> grouping in groups)
			{
				Assert.IsTrue(grouping.Key == nExpectedKey);
				Assert.IsTrue(grouping.Count() == 3);
				if (nExpectedKey++ > 100)
					break;
			}
		}

		[Test]
		public void GroupConsecutiveByTest2()
		{
			IEnumerable<int> seq = InfiniteInts();
			IEnumerable<IGrouping<int, int>> groups = seq.GroupConsecutiveBy(n => (n / 4) % 3).Take(10);
			int nCountDistinct = groups.DistinctBy(g => g.Key).Count();
			Assert.AreEqual(3, nCountDistinct);
			int nTotalCount1 = groups.Sum(g => g.Count());
			int nTotalCount2 = groups.SelectMany(g => g).Count();
			Assert.AreEqual(40, nTotalCount1);
			Assert.AreEqual(40, nTotalCount2);
		}

		[Test]
		public void GroupConsecutiveByTimespan()
		{
			DateTimeOffset baseDate = new DateTimeOffset(2010, 3, 14, 1, 0, 0, 0, TimeSpan.FromHours(-8));

			IEnumerable<IGrouping<DateTimeOffset, int>> groups = new List<int> { 1, 5, 7, 10, 15, 25, 35, 40, 45, 50 }.GroupConsecutiveByTimespan(TimeSpan.FromSeconds(5), x => baseDate + TimeSpan.FromSeconds(x));

			Assert.AreEqual(3, groups.Count());

			using (IEnumerator<IGrouping<DateTimeOffset, int>> group = groups.GetEnumerator())
			{
				Assert.IsTrue(group.MoveNext());
				Assert.AreEqual(baseDate + TimeSpan.FromSeconds(15), group.Current.Key);
				CollectionAssert.AreEqual(new List<int> { 1, 5, 7, 10, 15 }, group.Current);

				Assert.IsTrue(group.MoveNext());
				Assert.AreEqual(baseDate + TimeSpan.FromSeconds(25), group.Current.Key);
				CollectionAssert.AreEqual(new List<int> { 25 }, group.Current);

				Assert.IsTrue(group.MoveNext());
				Assert.AreEqual(baseDate + TimeSpan.FromSeconds(50), group.Current.Key);
				CollectionAssert.AreEqual(new List<int> { 35, 40, 45, 50 }, group.Current);

				Assert.IsFalse(group.MoveNext());
			}
		}

		[Test]
		public void GroupConsecutiveByTimespanThrowsOnBadArguments()
		{
			IEnumerable<int> nullEnumerable = null;
			Assert.Throws<ArgumentNullException>(() => nullEnumerable.GroupConsecutiveByTimespan(TimeSpan.FromMinutes(10), x => DateTimeOffset.Now));
			Assert.Throws<ArgumentNullException>(() => new List<int>().GroupConsecutiveByTimespan(TimeSpan.FromMinutes(10), null));

			// Empty enumerables are fine
			Assert.AreEqual(0, new List<int>().GroupConsecutiveByTimespan(TimeSpan.FromMinutes(10), x => DateTimeOffset.Now).Count());
		}

		[Test]
		public void Range()
		{
			CollectionAssert.AreEqual(new[] { 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024 }, EnumerableUtility.Range(2, 10, x => x * 2));
			CollectionAssert.AreEqual(new[] { 2 }, EnumerableUtility.Range(2, 1, x => x * 2));
			CollectionAssert.AreEqual(new int[0], EnumerableUtility.Range(2, 0, x => x * 2));
			Assert.Throws<ArgumentException>(() => EnumerableUtility.Range(2, -1, x => x * 2));
		}

		[Test]
		public void AreEqualTests()
		{
			DoAreEqualTest(true, null, null);
			DoAreEqualTest(false, null, new int[0]);
			DoAreEqualTest(true, new int[0], new int[0]);
			DoAreEqualTest(false, new int[0], new[] { 1 });
			DoAreEqualTest(true, new[] { 1 }, new[] { 1 });
			DoAreEqualTest(false, new[] { 1 }, new[] { 1, 2 });
			DoAreEqualTest(true, new[] { 1, 2 }, new[] { 1, 2 });
			DoAreEqualTest(false, new[] { 2, 1 }, new[] { 1, 2 });

			Func<int, int, bool> absEquals = (a, b) => Math.Abs(a) == Math.Abs(b);
			DoAreEqualTest(false, new[] { 1, 2 }, new[] { 1, -2 });
			DoAreEqualTest(true, new[] { 1, 2 }, new[] { 1, -2 }, absEquals);
			DoAreEqualTest(false, new[] { 1, 2 }, new[] { 1, -3 }, absEquals);
		}

		private void DoAreEqualTest(bool areEqual, IEnumerable<int> left, IEnumerable<int> right)
		{
			Assert.AreEqual(areEqual, EnumerableUtility.AreEqual(left, right));
			Assert.AreEqual(areEqual, EnumerableUtility.AreEqual(right, left));
		}

		private void DoAreEqualTest(bool areEqual, IEnumerable<int> left, IEnumerable<int> right, Func<int, int, bool> equals)
		{
			Assert.AreEqual(areEqual, EnumerableUtility.AreEqual(left, right, equals));
			Assert.AreEqual(areEqual, EnumerableUtility.AreEqual(right, left, equals));
		}

		private static IEnumerable<int> InfiniteInts()
		{
			int n = 0;
			for (;;)
				yield return n++;
		}

		private static IEnumerable<T> EnumerateEmpty<T>()
		{
			yield break;
		}

		private static IEnumerable EnumerateInts(int count)
		{
			for (int i = 0; i < count; i++)
				yield return i;
		}

		private class EnumerableMonitor<T> : IEnumerable<T>
		{
			public EnumerableMonitor(IEnumerable<T> seq)
			{
				m_seq = seq;
				m_nRequested = 0;
			}

			public int RequestCount
			{
				get { return m_nRequested; }
			}

			public IEnumerator<T> GetEnumerator()
			{
				foreach (var item in m_seq)
				{
					m_nRequested++;
					yield return item;
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			protected readonly IEnumerable<T> m_seq;
			int m_nRequested;
		}

		/// <summary>
		/// Like <c>EnumerableMonitor</c>, this class counts how many times
		/// the underlying sequence is accessed via the enumerator.  However, because
		/// it is an <see cref="ICollection"/>, <c>Count</c> is an O(1) operation
		/// rather than requiring enumeration of the whole sequence.
		/// </summary>
		private class CollectionMonitor<T> : EnumerableMonitor<T>, ICollection<T>
		{
			public CollectionMonitor(ICollection<T> seq)
				: base(seq)
			{
			}

			int ICollection<T>.Count
			{
				get { return ((ICollection<T>) m_seq).Count; }
			}

			#region not implemented ICollection members

			public void Add(T item)
			{
				throw new NotImplementedException();
			}

			public void Clear()
			{
				throw new NotImplementedException();
			}

			public bool Contains(T item)
			{
				throw new NotImplementedException();
			}

			public void CopyTo(T[] array, int arrayIndex)
			{
				throw new NotImplementedException();
			}

			public bool Remove(T item)
			{
				throw new NotImplementedException();
			}

			public bool IsReadOnly
			{
				get { throw new NotImplementedException(); }
			}

			#endregion
		}

		private class Base
		{
			public Base(int value) { Value = value; }
			public int Value;
		}

		private class Derived : Base
		{
			public Derived(int value) : base(value) { }
		}
	}
}
