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
		public void GetOrAdd(string? input)
		{
			var stringCache = new StringCache();
			var added = stringCache.GetOrAdd(input);
			Assert.AreEqual(added, input);
		}

		public void AddStringTwice()
		{
			var stringCache = new StringCache();
			var test = "test";
			var added = stringCache.GetOrAdd(test);
			Assert.IsTrue(ReferenceEquals(added, test));

			// build the same string using a StringBuilder, so that it's a different string object
			var sb = new StringBuilder("te");
			sb.Append("st");
			var added2 = stringCache.GetOrAdd(sb.ToString());
			Assert.IsTrue(ReferenceEquals(added, added2));
		}
	}
}
