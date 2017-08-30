using System.Text;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class StringCacheTests
	{
		[TestCase(null)]
		[TestCase("")]
		[TestCase("test")]
		public void GetOrAdd(string input)
		{
			StringCache stringCache = new StringCache();
			string added = stringCache.GetOrAdd(input);
			Assert.AreEqual(added, input);
		}

		public void AddStringTwice()
		{
			StringCache stringCache = new StringCache();
			string test = "test";
			string added = stringCache.GetOrAdd(test);
			Assert.IsTrue(object.ReferenceEquals(added, test));

			// build the same string using a StringBuilder, so that it's a different string object
			StringBuilder sb = new StringBuilder("te");
			sb.Append("st");
			string added2 = stringCache.GetOrAdd(sb.ToString());
			Assert.IsTrue(object.ReferenceEquals(added, added2));
		}
	}
}
