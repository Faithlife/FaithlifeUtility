using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class CollectionUtilityTests
	{
		[Test]
		public void AddNullRange()
		{
			ICollection<int> listNumbers = new List<int>();
			Assert.Throws<ArgumentNullException>(() => listNumbers.AddRange(null));
		}

		[Test]
		public void AddRangeToNullCollection()
		{
			ICollection<int> listNumbers = null;
			Assert.Throws<ArgumentNullException>(() => listNumbers.AddRange(new int[] { 1, 2, 3 }));
		}

		[Test]
		public void AddRangeToReadOnlyCollection()
		{
			ICollection<int> collReadOnly = (new int[] { }).AsReadOnly();
			Assert.Throws<ArgumentException>(() => collReadOnly.AddRange(new int[] { 1, 2, 3 }));
		}

		[Test]
		public void AddRange()
		{
			int[] anNumbers = { 1, 2, 3, 4 };
			ICollection<int> listNumbers = new List<int>();
			listNumbers.AddRange(anNumbers);

			CollectionAssert.AreEqual(anNumbers, listNumbers);
		}

		[Test]
		public void TestAddIfNotNullToListOfStructs()
		{
			List<DateTime> listOfDates = new List<DateTime>();

			DateTime? undefined = null;
			DateTime? today = DateTime.Now;

			listOfDates.AddIfNotNull(undefined);
			Assert.AreEqual(0, listOfDates.Count);
			listOfDates.AddIfNotNull(today);
			Assert.AreEqual(1, listOfDates.Count);
		}

		[Test]
		public void TestAddIfNotNullToListOfObjects()
		{
			List<object> listOfObjects = new List<object>();

			listOfObjects.AddIfNotNull(null);
			Assert.AreEqual(0, listOfObjects.Count);
			listOfObjects.AddIfNotNull(new object());
			Assert.AreEqual(1, listOfObjects.Count);
		}

		[Test]
		public void TestAddIfNotNullToListOfInts()
		{
			List<int> listOfInts = new List<int>();

			int? undefined = null;
			int? five = 5;

			listOfInts.AddIfNotNull(undefined);
			Assert.AreEqual(0, listOfInts.Count);
			listOfInts.AddIfNotNull(five);
			Assert.AreEqual(1, listOfInts.Count);
		}
	}
}
