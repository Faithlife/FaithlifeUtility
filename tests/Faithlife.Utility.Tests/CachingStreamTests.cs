using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class CachingStreamTests
	{
		public void Dispose()
		{
			var data = GenerateData(1000);
			using (var memoryStream = new MemoryStream(data, writable: false))
			using (var cachingStream = new CachingStream(memoryStream, Ownership.Owns))
			{
				cachingStream.Dispose();

				Assert.Throws<ObjectDisposedException>(() => { cachingStream.Flush(); });
				Assert.Throws<ObjectDisposedException>(() => { var temp = cachingStream.Length; });
				Assert.Throws<ObjectDisposedException>(() => { var temp = cachingStream.Position; });
				Assert.Throws<ObjectDisposedException>(() => { cachingStream.Position = 1; });
				Assert.Throws<ObjectDisposedException>(() => cachingStream.ReadByte());
				Assert.Throws<ObjectDisposedException>(() => cachingStream.Read(new byte[10], 0, 1));
				Assert.Throws<ObjectDisposedException>(() => cachingStream.Seek(1, SeekOrigin.Begin));
			}
		}

		[TestCase(0)]
		[TestCase(1)]
		[TestCase(1000)]
		[TestCase(2000)]
		public void Position(int position)
		{
			var data = GenerateData(1000);
			using (var memoryStream = new MemoryStream(data, writable: false))
			using (var cachingStream = new CachingStream(memoryStream, Ownership.Owns))
			{
				cachingStream.Position = position;
				Assert.AreEqual(position, cachingStream.Position);
			}
		}

		[TestCase(-1)]
		public void PositionOutOfRange(int position)
		{
			var data = GenerateData(1000);
			using (var memoryStream = new MemoryStream(data, writable: false))
			using (var cachingStream = new CachingStream(memoryStream, Ownership.Owns))
			{
				Assert.Throws<ArgumentOutOfRangeException>(() => cachingStream.Position = position);
			}
		}

		[TestCase(0)]
		[TestCase(1)]
		[TestCase(100)]
		[TestCase(4095)]
		[TestCase(4096)]
		[TestCase(4097)]
		[TestCase(8191)]
		[TestCase(8192)]
		[TestCase(8193)]
		public void ReadBlock(int size)
		{
			var data = GenerateData(size);
			using (var memoryStream = new MemoryStream(data, writable: false))
			using (var cachingStream = new CachingStream(memoryStream, Ownership.Owns))
			{
				var buffer = new byte[size + 1];
				var read = cachingStream.ReadBlock(buffer, 0, buffer.Length);
				Assert.AreEqual(size, read);
				Assert.AreEqual(data, buffer.Take(size));
				Assert.AreEqual(size, (int) cachingStream.Position);
			}
		}

		[TestCase(0)]
		[TestCase(1)]
		[TestCase(100)]
		[TestCase(4095)]
		[TestCase(4096)]
		[TestCase(4097)]
		[TestCase(8191)]
		[TestCase(8192)]
		[TestCase(8193)]
		public void ReadByte(int size)
		{
			var data = GenerateData(size);
			using (var memoryStream = new MemoryStream(data, writable: false))
			using (var cachingStream = new CachingStream(memoryStream, Ownership.Owns))
			{
				for (int i = 0; i < size; i++)
					Assert.AreEqual(data[i], cachingStream.ReadByte());
				Assert.AreEqual(size, (int) cachingStream.Position);

				Assert.AreEqual(-1, cachingStream.ReadByte());
				Assert.AreEqual(size, (int) cachingStream.Position);
			}
		}

		[Test]
		public void SeekAndRead()
		{
			Random random = new Random(1);
			var data = GenerateData(1234567);
			using (var memoryStream = new MemoryStream(data, writable: false))
			using (var cachingStream = new CachingStream(memoryStream, Ownership.Owns))
			{
				for (int i = 0; i < 100; i++)
				{
					int offset = random.Next(data.Length - 100);
					int length = random.Next(1, data.Length - offset);

					Assert.AreEqual(offset, cachingStream.Seek(offset, SeekOrigin.Begin));
					var read = cachingStream.ReadExactly(length);
					Assert.AreEqual(data.Skip(offset).Take(length), read);
				}
			}
		}

		[Test]
		public async Task SeekAndReadAsync()
		{
			Random random = new Random(1);
			var data = GenerateData(1234567);
			using (var memoryStream = new MemoryStream(data, writable: false))
			using (var cachingStream = new CachingStream(memoryStream, Ownership.Owns))
			{
				for (int i = 0; i < 100; i++)
				{
					int offset = random.Next(data.Length - 100);
					int length = random.Next(1, data.Length - offset);

					Assert.AreEqual(offset, cachingStream.Seek(offset, SeekOrigin.Begin));
					var read = await cachingStream.ReadExactlyAsync(length);
					Assert.AreEqual(data.Skip(offset).Take(length), read);
				}
			}
		}

		[Test]
		public void SeekAndCopyTo()
		{
			Random random = new Random(1);
			var data = GenerateData(54321);
			using (var memoryStream = new MemoryStream(data, writable: false))
			using (var cachingStream = new CachingStream(memoryStream, Ownership.Owns))
			{
				for (int i = 0; i < 100; i++)
				{
					int offset = random.Next(data.Length - 100);

					Assert.AreEqual(offset, cachingStream.Seek(offset, SeekOrigin.Begin));

					using (var destination = new MemoryStream(data.Length))
					{
						cachingStream.CopyTo(destination);
						Assert.AreEqual(data.Skip(offset), destination.ToArray());
					}
				}
			}
		}

		[Test]
		public async Task SeekAndCopyToAsync()
		{
			Random random = new Random(1);
			var data = GenerateData(54321);
			using (var memoryStream = new MemoryStream(data, writable: false))
			using (var cachingStream = new CachingStream(memoryStream, Ownership.Owns))
			{
				for (int i = 0; i < 100; i++)
				{
					int offset = random.Next(data.Length - 100);

					Assert.AreEqual(offset, cachingStream.Seek(offset, SeekOrigin.Begin));

					using (var destination = new MemoryStream(data.Length))
					{
						await cachingStream.CopyToAsync(destination);
						Assert.AreEqual(data.Skip(offset), destination.ToArray());
					}
				}
			}
		}

		[TestCase(1000)]
		[TestCase(1001)]
		[TestCase(4000)]
		[TestCase(5000)]
		[TestCase(50000)]
		public void SeekAndReadAfterEnd(int position)
		{
			var data = GenerateData(1000);
			using (var memoryStream = new MemoryStream(data, writable: false))
			using (var cachingStream = new CachingStream(memoryStream, Ownership.Owns))
			{
				cachingStream.Position = position;
				byte[] buffer = new byte[10];
				Assert.AreEqual(0, cachingStream.Read(buffer, 0, buffer.Length));
			}
		}

		[Test]
		public void SeekAndReadByte()
		{
			Random random = new Random(2);
			var data = GenerateData(123456);
			using (var memoryStream = new MemoryStream(data, writable: false))
			using (var cachingStream = new CachingStream(memoryStream, Ownership.Owns))
			{
				for (int i = 0; i < 100; i++)
				{
					int offset = random.Next(data.Length - 1);
					cachingStream.Position = offset;
					Assert.AreEqual(data[offset], cachingStream.ReadByte());
				}
			}
		}

		[Test]
		public void Write()
		{
			var data = GenerateData(1000);
			using (var memoryStream = new MemoryStream(data, writable: true))
			using (var cachingStream = new CachingStream(memoryStream, Ownership.Owns))
			{
				Assert.IsFalse(cachingStream.CanWrite);
				Assert.Throws<NotSupportedException>(() => cachingStream.SetLength(2000));
				Assert.Throws<NotSupportedException>(() => cachingStream.Write(new byte[1], 0, 1));
				Assert.Throws<NotSupportedException>(() => cachingStream.WriteByte(1));
				Assert.Throws<NotSupportedException>(() => cachingStream.BeginWrite(new byte[1], 0, 1, null, null));
				Assert.Throws<NotSupportedException>(() => cachingStream.WriteAsync(new byte[1], 0, 1));
			}
		}

		private static byte[] GenerateData(int size)
		{
			byte[] results = new byte[size];
			for (int index = 0; index < size; index++)
			{
				int value = index / 4;
				var temp = BitConverter.GetBytes(value);
				results[index] = temp[index % 4];
			}
			return results;
		}
	}
}
