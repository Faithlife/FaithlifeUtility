using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class UriUtilityTests
	{
		[Test]
		public void FromPattern()
		{
			Assert.AreEqual("http://example.com/", UriUtility.FromPattern("http://example.com").AbsoluteUri);

			Assert.AreEqual("http://example.com/%7Bx%7D", UriUtility.FromPattern("http://example.com/{x}").AbsoluteUri);
			Assert.AreEqual("http://example.com/r%26d", UriUtility.FromPattern("http://example.com/{x}", "x", "r&d").AbsoluteUri);
			Assert.AreEqual("http://example.com/r%26d?y=pb%26j", UriUtility.FromPattern("http://example.com/{x}", "x", "r&d", "y", "pb&j").AbsoluteUri);
			Assert.AreEqual("http://example.com/r%26d?y=pb%26j", UriUtility.FromPattern("http://example.com/{x}", "y", "pb&j", "x", "r&d").AbsoluteUri);

			Assert.AreEqual("http://example.com/%7Bx%7D?a=%7By%7D", UriUtility.FromPattern("http://example.com/{x}?a={y}").AbsoluteUri);
			Assert.AreEqual("http://example.com/r%26d?a=pb%26j", UriUtility.FromPattern("http://example.com/{x}?a={y}", "x", "r&d", "y", "pb&j").AbsoluteUri);
			Assert.AreEqual("http://example.com/r%26d?a=pb%26j", UriUtility.FromPattern("http://example.com/{x}?a={y}", "y", "pb&j", "x", "r&d").AbsoluteUri);
			Assert.AreEqual("http://example.com/r%26d?a=pb%26j&z=zed", UriUtility.FromPattern("http://example.com/{x}?a={y}", "y", "pb&j", "z", "zed", "x", "r&d").AbsoluteUri);
		}

		[Test]
		public void FromPatternKeyValuePairs()
		{
			var parameters = new SortedDictionary<string, object>()
			{
				["x"] = "r&d",
				["y"] = "pb&j",
				["z"] = "zed",
				["a"] = true,
				["b"] = 0,
				["c"] = TimeSpan.Zero,
				["d"] = null,
			};
			Assert.AreEqual("http://example.com/r%26d?y=pb%26j&a=true&b=0&c=00%3A00%3A00&z=zed", UriUtility.FromPattern("http://example.com/{x}?y={y}", parameters).AbsoluteUri);
		}

		[TestCase(null, null)]
		[TestCase(null, "somewhere.com")]
		[TestCase("http://maps.google.com", null)]
		public void MatchesDomainArgumentNullException(string uristring, string domain)
		{
			Uri uri = uristring != null ? new Uri(uristring) : null;
			Assert.Throws<ArgumentNullException>(() => UriUtility.MatchesDomain(uri, domain));
		}

		[TestCase("maps.google.com", "someplace.com")]
		public void MatchesDomainArgumentException(string uristring, string domain)
		{
			Uri uri = new Uri(uristring, UriKind.Relative);
			Assert.Throws<ArgumentException>(() => UriUtility.MatchesDomain(uri, domain));
		}

		[Test]
		public void MatchesDomain()
		{
			Uri uri = new Uri("http://maps.google.com");
			Assert.IsFalse(UriUtility.MatchesDomain(uri, "amaps.google.com"));
			Assert.IsTrue(UriUtility.MatchesDomain(uri, "maps.google.com"));
			Assert.IsTrue(UriUtility.MatchesDomain(uri, "maps.GOOGLE.com"));
			Assert.IsFalse(UriUtility.MatchesDomain(uri, "agoogle.com"));
			Assert.IsTrue(UriUtility.MatchesDomain(uri, "google.com"));
			Assert.IsTrue(UriUtility.MatchesDomain(uri, "GOOGLE.com"));
			Assert.IsFalse(UriUtility.MatchesDomain(uri, ""));

			uri = new Uri("http://google.com");
			Assert.IsFalse(UriUtility.MatchesDomain(uri, "maps.google.com"));
			Assert.IsFalse(UriUtility.MatchesDomain(uri, "maps.GOOGLE.com"));
			Assert.IsFalse(UriUtility.MatchesDomain(uri, "agoogle.com"));
			Assert.IsTrue(UriUtility.MatchesDomain(uri, "google.com"));
			Assert.IsTrue(UriUtility.MatchesDomain(uri, "GOOGLE.com"));
			Assert.IsFalse(UriUtility.MatchesDomain(uri, ""));

			uri = new Uri("http://maps.GOOGLE.com");
			Assert.IsTrue(UriUtility.MatchesDomain(uri, "maps.google.com"));
			Assert.IsTrue(UriUtility.MatchesDomain(uri, "maps.GOOGLE.com"));
			Assert.IsFalse(UriUtility.MatchesDomain(uri, "agoogle.com"));
			Assert.IsTrue(UriUtility.MatchesDomain(uri, "google.com"));
			Assert.IsTrue(UriUtility.MatchesDomain(uri, "GOOGLE.com"));

			uri = new Uri("http://office.google.com");
			Assert.IsFalse(UriUtility.MatchesDomain(uri, "maps.google.com"));
			Assert.IsFalse(UriUtility.MatchesDomain(uri, "maps.GOOGLE.com"));
			Assert.IsFalse(UriUtility.MatchesDomain(uri, "agoogle.com"));
			Assert.IsTrue(UriUtility.MatchesDomain(uri, "google.com"));
			Assert.IsTrue(UriUtility.MatchesDomain(uri, "GOOGLE.com"));
		}
	}
}
