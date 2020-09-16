using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class RebasedStreamTests
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
			Assert.Throws<ArgumentNullException>(() => new RebasedStream(null!));
		}

		[Test]
		public void Length()
		{
			using (var stream = CreateRebasedStream(2))
			{
				Assert.AreEqual(m_buffer.Length - 2, stream.Length);
			}
		}

		[Test]
		public void Position()
		{
			using (var stream = CreateRebasedStream(3))
			{
				stream.Position = 4;
				Assert.AreEqual(4, stream.Position);
				Assert.AreEqual(7, m_stream.Position);

				stream.Position = 0;
				Assert.AreEqual(0, stream.Position);
				Assert.AreEqual(3, m_stream.Position);
			}
		}

		[Test]
		public void Seek()
		{
			using (var stream = CreateRebasedStream(5))
			{
				Assert.AreEqual(0, stream.Seek(0, SeekOrigin.Begin));
				Assert.AreEqual(0, stream.Position);
				Assert.AreEqual(5, m_stream.Position);

				Assert.AreEqual(2, stream.Seek(2, SeekOrigin.Begin));
				Assert.AreEqual(2, stream.Position);
				Assert.AreEqual(7, m_stream.Position);

				Assert.AreEqual(4, stream.Seek(2, SeekOrigin.Current));
				Assert.AreEqual(4, stream.Position);
				Assert.AreEqual(9, m_stream.Position);

				Assert.AreEqual(6, stream.Seek(-1, SeekOrigin.End));
				Assert.AreEqual(6, stream.Position);
				Assert.AreEqual(11, m_stream.Position);
			}
		}

		[Test]
		public void SetLengthShorter()
		{
			using (var stream = CreateRebasedStream(4))
			{
				stream.SetLength(5);
				Assert.AreEqual(5, stream.Length);
				Assert.AreEqual(9, m_stream.Length);
			}
		}

		[Test]
		public void SetLengthLonger()
		{
			using (var stream = CreateRebasedStream(4))
			{
				stream.SetLength(15);
				Assert.AreEqual(15, stream.Length);
				Assert.AreEqual(19, m_stream.Length);
			}
		}

		[Test]
		public void Read()
		{
			using (var stream = CreateRebasedStream(3))
			{
				var buffer = new byte[16];
				Assert.AreEqual(9, stream.Read(buffer, 0, buffer.Length));
				CollectionAssert.AreEqual(new byte[] { 4, 5, 6, 7, 8, 9, 10, 11, 12, 0, 0, 0, 0, 0, 0, 0 }, buffer);
			}
		}

		[Test]
		public async Task ReadAsync()
		{
			using (var stream = CreateRebasedStream(3))
			{
				var buffer = new byte[16];
				Assert.AreEqual(9, await stream.ReadAsync(buffer, 0, buffer.Length));
				CollectionAssert.AreEqual(new byte[] { 4, 5, 6, 7, 8, 9, 10, 11, 12, 0, 0, 0, 0, 0, 0, 0 }, buffer);
			}
		}

		[Test]
		public void CopyTo()
		{
			using (var stream = CreateRebasedStream(3))
			{
				var buffer = new byte[16];
				var destination = new MemoryStream(buffer);
				stream.CopyTo(destination);
				CollectionAssert.AreEqual(new byte[] { 4, 5, 6, 7, 8, 9, 10, 11, 12, 0, 0, 0, 0, 0, 0, 0 }, buffer);
			}
		}

		[Test]
		public async Task CopyToAsync()
		{
			using (var stream = CreateRebasedStream(3))
			{
				var buffer = new byte[16];
				var destination = new MemoryStream(buffer);
				await stream.CopyToAsync(destination);
				CollectionAssert.AreEqual(new byte[] { 4, 5, 6, 7, 8, 9, 10, 11, 12, 0, 0, 0, 0, 0, 0, 0 }, buffer);
			}
		}

		[Test]
		public void Dispose()
		{
			using (var stream = CreateRebasedStream(3))
			{
				stream.Dispose();
				Assert.Throws<ObjectDisposedException>(() =>
				{
					var p = stream.Position;
				});
				Assert.Throws<ObjectDisposedException>(() => stream.Position = 3);
				Assert.Throws<ObjectDisposedException>(() =>
				{
					var p = stream.Length;
				});
				Assert.Throws<ObjectDisposedException>(() => stream.SetLength(20));
				Assert.Throws<ObjectDisposedException>(() => stream.Seek(0, SeekOrigin.Begin));
			}
		}

		private RebasedStream CreateRebasedStream(int offset)
		{
			m_stream.Position = offset;
			return new RebasedStream(m_stream);
		}

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
		private byte[] m_buffer;
		private Stream m_stream;
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
	}
}
