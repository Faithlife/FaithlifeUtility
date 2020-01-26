using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class WrappingStreamTests
	{
		[SetUp]
		public void SetUp()
		{
			m_memStream = new MemoryStream();
			m_memStream.Write(s_abyStreamData, 0, s_abyStreamData.Length);

			m_stream = new WrappingStream(m_memStream, Ownership.None);
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
			Assert.IsTrue(m_stream.CanWrite);
			Assert.AreEqual(s_abyStreamData.Length, m_stream.Length);
		}

		[Test]
		public void ConstructorNull()
		{
			Assert.Throws<ArgumentNullException>(() => new WrappingStream(null!, Ownership.None));
		}

		[Test]
		public void Dispose()
		{
			m_stream.Dispose();
			m_stream.Dispose();

			Assert.IsTrue(m_memStream.CanRead);
			Assert.IsTrue(m_memStream.CanSeek);
			Assert.IsTrue(m_memStream.CanWrite);
			Assert.IsFalse(m_stream.CanRead);
			Assert.IsFalse(m_stream.CanSeek);
			Assert.IsFalse(m_stream.CanWrite);

			Assert.Throws<ObjectDisposedException>(() => { long i = m_stream.Length; });
			Assert.Throws<ObjectDisposedException>(() => { long i = m_stream.Position; });
			Assert.Throws<ObjectDisposedException>(() => { m_stream.Position = 0; });
			Assert.Throws<ObjectDisposedException>(() => { m_stream.BeginRead(new byte[1], 0, 1, null!, null); });
			Assert.Throws<ObjectDisposedException>(() => { m_stream.EndRead(null!); });
			Assert.Throws<ObjectDisposedException>(() => { m_stream.BeginWrite(new byte[1], 0, 1, null!, null); });
			Assert.Throws<ObjectDisposedException>(() => { m_stream.EndWrite(null!); });
			Assert.Throws<ObjectDisposedException>(() => { m_stream.Flush(); });
			Assert.Throws<ObjectDisposedException>(() => { m_stream.Read(new byte[1], 0, 1); });
			Assert.Throws<ObjectDisposedException>(() => { m_stream.ReadByte(); });
			Assert.Throws<ObjectDisposedException>(() => { m_stream.Write(new byte[1], 0, 1); });
			Assert.Throws<ObjectDisposedException>(() => { m_stream.WriteByte(0); });
			Assert.Throws<ObjectDisposedException>(() => { m_stream.Seek(0, SeekOrigin.Begin); });
			Assert.Throws<ObjectDisposedException>(() => { m_stream.SetLength(16); });
		}

		[Test]
		public void Flush()
		{
			m_stream.Flush();
		}

		[Test]
		public async Task FlushAsync()
		{
			await m_stream.FlushAsync().ConfigureAwait(false);
		}

		[Test]
		public void Read()
		{
			m_stream.Position = 0;
			byte[] aby = new byte[s_abyStreamData.Length];
			Assert.AreEqual(aby.Length, m_stream.Read(aby, 0, aby.Length));
			CollectionAssert.AreEqual(s_abyStreamData, aby);
		}

		[Test]
		public void BeginRead()
		{
			m_stream.Position = 0;
			byte[] aby = new byte[s_abyStreamData.Length];
			IAsyncResult ar = m_stream.BeginRead(aby, 0, 8, null!, null);
			Assert.AreEqual(aby.Length, m_stream.EndRead(ar));
			CollectionAssert.AreEqual(s_abyStreamData, aby);
		}

		[Test]
		public async Task ReadAsync()
		{
			m_stream.Position = 0;
			byte[] aby = new byte[s_abyStreamData.Length];
			Assert.AreEqual(aby.Length, await m_stream.ReadAsync(aby, 0, 8, CancellationToken.None).ConfigureAwait(false));
			CollectionAssert.AreEqual(s_abyStreamData, aby);
		}

		[Test]
		public void ReadByte()
		{
			m_stream.Position = 0;
			Assert.AreEqual(s_abyStreamData[0], m_stream.ReadByte());
			Assert.AreEqual(s_abyStreamData[1], m_stream.ReadByte());
			Assert.AreEqual(s_abyStreamData[2], m_stream.ReadByte());
			m_stream.Seek(1, SeekOrigin.Current);
			Assert.AreEqual(s_abyStreamData[4], m_stream.ReadByte());
			m_stream.Position = m_stream.Position - 2;
			Assert.AreEqual(s_abyStreamData[3], m_stream.ReadByte());
			m_stream.Seek(7, SeekOrigin.Begin);
			Assert.AreEqual(s_abyStreamData[7], m_stream.ReadByte());
			m_stream.Seek(0, SeekOrigin.End);
			Assert.AreEqual(-1, m_stream.ReadByte());
		}

		[Test]
		public void Timeout()
		{
			Assert.IsFalse(m_stream.CanTimeout);
			Assert.Throws<InvalidOperationException>(() => { m_stream.ReadTimeout = 1000; });
			Assert.Throws<InvalidOperationException>(() => { m_stream.WriteTimeout = 1000; });
		}

		[Test]
		public void WriteByte()
		{
			m_stream.WriteByte(9);
			Assert.AreEqual(s_abyStreamData.Length + 1, m_stream.Length);
			m_stream.Position = m_stream.Position - 1;
			Assert.AreEqual(9, m_stream.ReadByte());
			Assert.AreEqual(-1, m_stream.ReadByte());
		}

		[Test]
		public void Write()
		{
			m_stream.Write(s_abyStreamData, 0, s_abyStreamData.Length);
			VerifyWrite();
		}

		[Test]
		public void BeginWrite()
		{
			IAsyncResult ar = m_stream.BeginWrite(s_abyStreamData, 0, s_abyStreamData.Length, null!, null);
			m_stream.EndWrite(ar);
			VerifyWrite();
		}

		[Test]
		public async Task WriteAsync()
		{
			await m_stream.WriteAsync(s_abyStreamData, 0, s_abyStreamData.Length, CancellationToken.None).ConfigureAwait(false);
			VerifyWrite();
		}

		[Test]
		public void SetLength()
		{
			m_stream.SetLength(4);
			Assert.AreEqual(4, m_stream.Length);
			Assert.AreEqual(4, m_stream.Position);

			m_stream.SetLength(0);
			Assert.AreEqual(0, m_stream.Length);
			Assert.AreEqual(0, m_stream.Position);

			m_stream.SetLength(256);
			Assert.AreEqual(256, m_stream.Length);
			Assert.AreEqual(0, m_stream.Position);
		}

		private void VerifyWrite()
		{
			Assert.AreEqual(s_abyStreamData.Length * 2, m_stream.Length);

			m_stream.Position = 0;

			byte[] aby = new byte[s_abyStreamData.Length * 2];
			m_stream.Read(aby, 0, aby.Length);
			CollectionAssert.AreEqual(s_abyStreamData, aby.Take(s_abyStreamData.Length));
			CollectionAssert.AreEqual(s_abyStreamData, aby.Skip(s_abyStreamData.Length));
		}

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
		Stream m_memStream;
		Stream m_stream;
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

		static readonly byte[] s_abyStreamData = { 0, 1, 2, 3, 4, 5, 6, 7 };
	}
}
