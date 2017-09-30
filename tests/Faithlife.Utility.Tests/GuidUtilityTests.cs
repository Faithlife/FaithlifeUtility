using System;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class GuidUtilityTests
	{
		[TestCase(null, "00000000-0000-0000-0000-000000000000")]
		[TestCase("", "00000000-0000-0000-0000-000000000000")]
		[TestCase("foobar", "00000000-0000-0000-0000-000000000000")]
		public void TryParse_Failure(string strGuid, string strExpected)
		{
			Guid guid;
			Assert.IsFalse(GuidUtility.TryParse(strGuid, out guid));
			Assert.AreEqual(new Guid(strExpected), guid);
		}

		[TestCase("{19820C32-CF08-4684-8C3D-843F188DAB12}", "19820C32-CF08-4684-8C3D-843F188DAB12")]
		[TestCase("19820C32-CF08-4684-8C3D-843F188DAB12", "19820C32-CF08-4684-8C3D-843F188DAB12")]
		[TestCase("19820C32CF0846848C3D843F188DAB12", "19820C32-CF08-4684-8C3D-843F188DAB12")]
		public void TryParse_Success(string strGuid, string strExpected)
		{
			Guid guid;
			Assert.IsTrue(GuidUtility.TryParse(strGuid, out guid));
			Assert.AreEqual(new Guid(strExpected), guid);
		}

		[TestCase("dfd9d0c0fe7b4bfdae6b604b43f71f06", "dfd9d0c0-fe7b-4bfd-ae6b-604b43f71f06")]
		[TestCase("5c6c53583af54905a8ad27184a0bb1b7", "5c6c5358-3af5-4905-a8ad-27184a0bb1b7")]
		[TestCase("00000000000000000000000000000000", "00000000-0000-0000-0000-000000000000")]
		[TestCase("ffffffffffffffffffffffffffffffff", "FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF")]
		public void RoundTripLowerNoDash(string shortString, string longString)
		{
			Guid guid = new Guid(longString);
			Assert.AreEqual(shortString, guid.ToLowerNoDashString());
			Assert.AreEqual(guid, GuidUtility.FromLowerNoDashString(shortString));
		}

		[TestCase("")]
		[TestCase("dfd9d0c0")]
		[TestCase("dfd9d0c0-fe7b-4bfd-ae6b-604b43f71f06")]
		[TestCase("dfd9d0c0fe7b4bfdae6b604b43f71f060")]
		[TestCase("DFD9D0C0FE7B4BFDAE6B604B43F71F06")]
		public void FromLowerNoDashFailure(string lowerNoDashString)
		{
			Assert.Throws<FormatException>(() => GuidUtility.FromLowerNoDashString(lowerNoDashString));
		}

		[Test]
		public void ToNetworkOrder()
		{
			Guid guid = new Guid(0x01020304, 0x0506, 0x0708, 9, 10, 11, 12, 13, 14, 15, 16);
			byte[] bytes = guid.ToByteArray();
			CollectionAssert.AreEqual(new byte[] { 4, 3, 2, 1, 6, 5, 8, 7, 9, 10, 11, 12, 13, 14, 15, 16 }, bytes);

			GuidUtility.SwapByteOrder(bytes);
			CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }, bytes);
		}

#if PORTABLE
		[Ignore("Requires platform-specific implementation not present in PORTABLE builds.")]
#endif
		[Test]
		public void CreateVersion3FromWidgetsCom()
		{
			// run the test case from RFC 4122 Appendix B, as updated by http://www.rfc-editor.org/errata_search.php?rfc=4122
			Guid guid = GuidUtility.Create(GuidUtility.DnsNamespace, "www.widgets.com", 3);
			Assert.AreEqual(new Guid("3d813cbb-47fb-32ba-91df-831e1593ac29"), guid);
		}

#if PORTABLE
		[Ignore("Requires platform-specific implementation not present in PORTABLE builds.")]
#endif
		[Test]
		public void CreateVersion3FromPythonOrg()
		{
			// run the test case from the Python implementation (http://docs.python.org/library/uuid.html#uuid-example)
			Guid guid = GuidUtility.Create(GuidUtility.DnsNamespace, "python.org", 3);
			Assert.AreEqual(new Guid("6fa459ea-ee8a-3ca4-894e-db77e160355e"), guid);
		}

#if PORTABLE
		[Ignore("Requires platform-specific implementation not present in PORTABLE builds.")]
#endif
		[Test]
		public void CreateVersion5FromPythonOrg()
		{
			// run the test case from the Python implementation (http://docs.python.org/library/uuid.html#uuid-example)
			Guid guid = GuidUtility.Create(GuidUtility.DnsNamespace, "python.org", 5);
			Assert.AreEqual(new Guid("886313e1-3b8a-5372-9b90-0c9aee199e5d"), guid);
		}

		[Test]
		public void CreateNullName()
		{
			Assert.Throws<ArgumentNullException>(() => GuidUtility.Create(GuidUtility.DnsNamespace, default(string)));
		}

		[Test]
		public void CreateInvalidVersion()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => GuidUtility.Create(GuidUtility.DnsNamespace, "www.example.com", 4));
		}
	}
}
