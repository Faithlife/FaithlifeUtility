using System;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class UriUtilityTests
	{
		[Test]
		public void FromPattern()
		{
			Assert.AreEqual(UriUtility.FromPattern("http://example.com").AbsoluteUri, "http://example.com/");

			Assert.AreEqual(UriUtility.FromPattern("http://example.com/{x}").AbsoluteUri, "http://example.com/%7Bx%7D");
			Assert.AreEqual(UriUtility.FromPattern("http://example.com/{x}", "x", "r&d").AbsoluteUri, "http://example.com/r%26d");
			Assert.AreEqual(UriUtility.FromPattern("http://example.com/{x}", "x", "r&d", "y", "pb&j").AbsoluteUri, "http://example.com/r%26d?y=pb%26j");
			Assert.AreEqual(UriUtility.FromPattern("http://example.com/{x}", "y", "pb&j", "x", "r&d").AbsoluteUri, "http://example.com/r%26d?y=pb%26j");

			Assert.AreEqual(UriUtility.FromPattern("http://example.com/{x}?a={y}").AbsoluteUri, "http://example.com/%7Bx%7D?a=%7By%7D");
			Assert.AreEqual(UriUtility.FromPattern("http://example.com/{x}?a={y}", "x", "r&d", "y", "pb&j").AbsoluteUri, "http://example.com/r%26d?a=pb%26j");
			Assert.AreEqual(UriUtility.FromPattern("http://example.com/{x}?a={y}", "y", "pb&j", "x", "r&d").AbsoluteUri, "http://example.com/r%26d?a=pb%26j");
			Assert.AreEqual(UriUtility.FromPattern("http://example.com/{x}?a={y}", "y", "pb&j", "z", "zed", "x", "r&d").AbsoluteUri, "http://example.com/r%26d?a=pb%26j&z=zed");
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
			Assert.AreEqual(UriUtility.MatchesDomain(uri, "amaps.google.com"), false);
			Assert.AreEqual(UriUtility.MatchesDomain(uri, "maps.google.com"), true);
			Assert.AreEqual(UriUtility.MatchesDomain(uri, "maps.GOOGLE.com"), true);
			Assert.AreEqual(UriUtility.MatchesDomain(uri, "agoogle.com"), false);
			Assert.AreEqual(UriUtility.MatchesDomain(uri, "google.com"), true);
			Assert.AreEqual(UriUtility.MatchesDomain(uri, "GOOGLE.com"), true);
			Assert.AreEqual(UriUtility.MatchesDomain(uri, ""), false);

			uri = new Uri("http://google.com");
			Assert.AreEqual(UriUtility.MatchesDomain(uri, "maps.google.com"), false);
			Assert.AreEqual(UriUtility.MatchesDomain(uri, "maps.GOOGLE.com"), false);
			Assert.AreEqual(UriUtility.MatchesDomain(uri, "agoogle.com"), false);
			Assert.AreEqual(UriUtility.MatchesDomain(uri, "google.com"), true);
			Assert.AreEqual(UriUtility.MatchesDomain(uri, "GOOGLE.com"), true);
			Assert.AreEqual(UriUtility.MatchesDomain(uri, ""), false);

			uri = new Uri("http://maps.GOOGLE.com");
			Assert.AreEqual(UriUtility.MatchesDomain(uri, "maps.google.com"), true);
			Assert.AreEqual(UriUtility.MatchesDomain(uri, "maps.GOOGLE.com"), true);
			Assert.AreEqual(UriUtility.MatchesDomain(uri, "agoogle.com"), false);
			Assert.AreEqual(UriUtility.MatchesDomain(uri, "google.com"), true);
			Assert.AreEqual(UriUtility.MatchesDomain(uri, "GOOGLE.com"), true);

			uri = new Uri("http://office.google.com");
			Assert.AreEqual(UriUtility.MatchesDomain(uri, "maps.google.com"), false);
			Assert.AreEqual(UriUtility.MatchesDomain(uri, "maps.GOOGLE.com"), false);
			Assert.AreEqual(UriUtility.MatchesDomain(uri, "agoogle.com"), false);
			Assert.AreEqual(UriUtility.MatchesDomain(uri, "google.com"), true);
			Assert.AreEqual(UriUtility.MatchesDomain(uri, "GOOGLE.com"), true);
		}
	}
}
