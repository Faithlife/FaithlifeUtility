using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class ReadOnlyStreamTests
	{
		[SetUp]
		public void SetUp()
		{
			m_memStream = new MemoryStream(m_abyStreamData, true);
			m_stream = new ReadOnlyStream(m_memStream);
		}

		[TearDown]
		public void TearDown()
		{
			m_stream = null!;
			m_memStream = null!;
		}

		[Test]
		public void Constructor()
		{
			Assert.IsTrue(m_stream.CanRead);
			Assert.IsTrue(m_stream.CanSeek);
			Assert.IsFalse(m_stream.CanWrite);
			Assert.AreEqual(m_abyStreamData.Length, m_stream.Length);
		}

		[Test]
		public void ConstructorNull()
		{
			Assert.Throws<ArgumentNullException>(() => new ReadOnlyStream(null!));
		}

		[Test]
		public void Dispose()
		{
			m_stream.Dispose();
			m_stream.Dispose();

			Assert.IsTrue(m_memStream.CanRead);
			Assert.IsFalse(m_stream.CanRead);
			Assert.IsFalse(m_stream.CanSeek);

			Assert.Throws<ObjectDisposedException>(() =>
			{
				var i = m_stream.Length;
			});
			Assert.Throws<ObjectDisposedException>(() =>
			{
				var i = m_stream.Position;
			});
			Assert.Throws<ObjectDisposedException>(() => { m_stream.Position = 0; });
			Assert.Throws<ObjectDisposedException>(() => { m_stream.Flush(); });
			Assert.Throws<ObjectDisposedException>(() => { m_stream.Read(new byte[1], 0, 1); });
			Assert.Throws<ObjectDisposedException>(() => { m_stream.ReadByte(); });
			Assert.Throws<ObjectDisposedException>(() => { m_stream.Seek(0, SeekOrigin.Begin); });
		}

		[Test]
		public void Flush()
		{
			m_stream.Flush();
		}

		[Test]
		public void Read()
		{
			var aby = new byte[m_abyStreamData.Length];
			Assert.AreEqual(aby.Length, m_stream.Read(aby, 0, aby.Length));
			CollectionAssert.AreEqual(m_abyStreamData, aby);
		}

		[Test]
		public void ReadByte()
		{
			Assert.AreEqual(m_abyStreamData[0], m_stream.ReadByte());
			Assert.AreEqual(m_abyStreamData[1], m_stream.ReadByte());
			Assert.AreEqual(m_abyStreamData[2], m_stream.ReadByte());
			m_stream.Seek(1, SeekOrigin.Current);
			Assert.AreEqual(m_abyStreamData[4], m_stream.ReadByte());
			m_stream.Position = m_stream.Position - 2;
			Assert.AreEqual(m_abyStreamData[3], m_stream.ReadByte());
			m_stream.Seek(7, SeekOrigin.Begin);
			Assert.AreEqual(m_abyStreamData[7], m_stream.ReadByte());
			m_stream.Seek(0, SeekOrigin.End);
			Assert.AreEqual(-1, m_stream.ReadByte());
		}

		[Test]
		public async Task ReadAsync()
		{
			var aby = new byte[m_abyStreamData.Length];
			Assert.AreEqual(aby.Length, await m_stream.ReadAsync(aby, 0, aby.Length));
			CollectionAssert.AreEqual(m_abyStreamData, aby);
		}

		[Test]
		public void CopyTo()
		{
			var aby = new byte[m_abyStreamData.Length];
			var destination = new MemoryStream(aby);
			m_stream.CopyTo(destination);
			CollectionAssert.AreEqual(m_abyStreamData, aby);
		}

		[Test]
		public async Task CopyToAsync()
		{
			var aby = new byte[m_abyStreamData.Length];
			var destination = new MemoryStream(aby);
			await m_stream.CopyToAsync(destination);
			CollectionAssert.AreEqual(m_abyStreamData, aby);
		}

		[Test]
		public void Write()
		{
			Assert.Throws<NotSupportedException>(() => { m_stream.Write(m_abyStreamData, 0, 1); });
			Assert.Throws<NotSupportedException>(() => { m_stream.WriteByte(0); });
			Assert.Throws<NotSupportedException>(() => { m_stream.BeginWrite(m_abyStreamData, 0, 1, null!, null); });
			Assert.Throws<NotSupportedException>(() => { m_stream.WriteAsync(m_abyStreamData, 0, 1); });
			Assert.Throws<NotSupportedException>(() => { m_stream.SetLength(0); });
		}

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
		private Stream m_memStream;
		private Stream m_stream;
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
		private readonly byte[] m_abyStreamData = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 };
	}
}
