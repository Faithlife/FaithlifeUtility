using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class ObjectUtilityTests
	{
		[Test]
		public void GetHashCodeNull()
		{
			ObjectUtility.GetHashCode(default(string));
		}

		[Test]
		public void GetHashCodeNotNull()
		{
			const string str = "happy";
			Assert.AreEqual(str.GetHashCode(), ObjectUtility.GetHashCode(str));
		}

		[Test]
		public void GetHashCodeValue()
		{
			Assert.AreEqual(3.GetHashCode(), ObjectUtility.GetHashCode(3));
		}
	}
}
