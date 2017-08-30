using System;
using System.IO;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class TruncatedStreamTests
	{
		[SetUp]
		public void SetUp()
		{
			m_buffer = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
			m_stream = new MemoryStream();
			m_stream.Write(m_buffer, 0, m_buffer.Length);
			m_stream.Position = 0;
		}

		[TearDown]
		public void TearDown()
		{
			m_stream.Dispose();
		}

		[Test]
		public void BadConstructorArguments()
		{
			Assert.Throws<ArgumentNullException>(() => new TruncatedStream(null, 5, Ownership.None));
		}

		[Test]
		public void Length()
		{
			using (TruncatedStream stream = CreateTruncatedStream(5))
			{
				Assert.AreEqual(5, stream.Length);
			}
		}

		[Test]
		public void Seek()
		{
			using (TruncatedStream stream = CreateTruncatedStream(5))
			{
				Assert.AreEqual(2, stream.Seek(2, SeekOrigin.Begin));
				Assert.AreEqual(2, stream.Position);
				Assert.AreEqual(2, m_stream.Position);

				Assert.AreEqual(3, stream.Seek(1, SeekOrigin.Current));
				Assert.AreEqual(3, stream.Position);
				Assert.AreEqual(3, m_stream.Position);

				Assert.AreEqual(4, stream.Seek(-1, SeekOrigin.End));
				Assert.AreEqual(4, stream.Position);
				Assert.AreEqual(4, m_stream.Position);
			}
		}

		[Test]
		public void Read()
		{
			using (TruncatedStream stream = CreateTruncatedStream(5))
			{
				byte[] buffer = new byte[16];
				Assert.AreEqual(5, stream.Read(buffer, 0, buffer.Length));
				CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, buffer);
			}
		}

		private TruncatedStream CreateTruncatedStream(int length)
		{
			return new TruncatedStream(m_stream, length, Ownership.None);
		}

		byte[] m_buffer;
		Stream m_stream;
	}
}
