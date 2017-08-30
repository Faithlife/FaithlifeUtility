using System;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class UriBuilderUtilityTests
	{
		[TestCase("http://www.google.com/search", "", "http://www.google.com/search")]
		[TestCase("http://www.google.com/search", "q=hello", "http://www.google.com/search?q=hello")]
		[TestCase("http://www.google.com/search", "q=hello&a=b", "http://www.google.com/search?q=hello&a=b")]
		[TestCase("http://www.google.com/search?a=b", "", "http://www.google.com/search?a=b")]
		[TestCase("http://www.google.com/search?a=b", "q=hello", "http://www.google.com/search?a=b&q=hello")]
		[TestCase("http://www.google.com/search?a=b%26c", "d=e&f=g", "http://www.google.com/search?a=b%26c&d=e&f=g")]
		public void AppendQuery(string baseUri, string query, string expectedUri)
		{
			UriBuilder builder = new UriBuilder(baseUri);
			builder.AppendQuery(query);
			Assert.AreEqual(expectedUri, builder.Uri.AbsoluteUri);
		}
	}
}
