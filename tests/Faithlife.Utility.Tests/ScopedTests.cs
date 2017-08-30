using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class ScopedTests
	{
		[Test]
		public void ScopedBool()
		{
			Scoped<bool> sBool = new Scoped<bool>();
			Assert.IsFalse(sBool.Value);
			using (sBool.SetValue(true))
			{
				Assert.IsTrue(sBool.Value);
			}
			Assert.IsFalse(sBool.Value);
		}
		
		[Test]
		public void ScopedInt()
		{
			Scoped<int> sInt = new Scoped<int>(4);
			Assert.AreEqual(4, sInt.Value);
			using (sInt.SetValue(5))
			{
				Assert.AreEqual(5, sInt.Value);
			}
			Assert.AreEqual(4, sInt.Value);
		}
	}
}
