using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class StreamUtilityTests
	{
		[Test]
		public void ReadExactlyBadArguments()
		{
			using (Stream stream = new MemoryStream())
			{
				Assert.Throws<ArgumentNullException>(() => StreamUtility.ReadExactly(null, new byte[1], 0, 1));
				Assert.Throws<ArgumentNullException>(() => stream.ReadExactly(null, 0, 1));
				Assert.Throws<ArgumentOutOfRangeException>(() => stream.ReadExactly(new byte[1], -1, 1));
				Assert.Throws<ArgumentOutOfRangeException>(() => stream.ReadExactly(new byte[1], 1, 1));
				Assert.Throws<ArgumentOutOfRangeException>(() => stream.ReadExactly(new byte[1], 0, -1));
				Assert.Throws<ArgumentOutOfRangeException>(() => stream.ReadExactly(new byte[1], 0, 2));
				Assert.Throws<ArgumentOutOfRangeException>(() => stream.ReadExactly(new byte[1], 1, 1));
				Assert.Throws<ArgumentOutOfRangeException>(() => stream.ReadExactly(-1));
			}
		}

		[Test]
		public void ReadExactlyZeroBytes()
		{
			using (Stream stream = new MemoryStream())
			{
				Assert.IsNotNull(stream.ReadExactly(0));
				Assert.AreEqual(0, stream.ReadExactly(0).Length);
			}
		}

		[Test]
		public void ReadExactly()
		{
			byte[] abySource = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

			using (Stream streamSource = new MemoryStream(abySource))
			using (Stream stream = new SlowStream(streamSource))
			{
				byte[] read = stream.ReadExactly(5);
				CollectionAssert.AreEqual(abySource.Take(5), read);

				read = stream.ReadExactly(6);
				CollectionAssert.AreEqual(abySource.Skip(5), read);

				Assert.Throws<EndOfStreamException>(() => stream.ReadExactly(1));
			}

			using (Stream streamSource = new MemoryStream(abySource))
			using (Stream stream = new SlowStream(streamSource))
			{
				byte[] read = stream.ReadExactly(11);
				CollectionAssert.AreEqual(abySource, read);
			}

			using (Stream streamSource = new MemoryStream(abySource))
			using (Stream stream = new SlowStream(streamSource))
			{
				Assert.Throws<EndOfStreamException>(() => stream.ReadExactly(12));
			}
		}

		[Test]
		public async Task ReadExactlyAsync()
		{
			byte[] abySource = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

			using (Stream streamSource = new MemoryStream(abySource))
			using (Stream stream = new SlowStream(streamSource))
			{
				byte[] read = await stream.ReadExactlyAsync(5);
				CollectionAssert.AreEqual(abySource.Take(5), read);

				read = await stream.ReadExactlyAsync(6);
				CollectionAssert.AreEqual(abySource.Skip(5), read);

				Assert.ThrowsAsync<EndOfStreamException>(() => stream.ReadExactlyAsync(1));
			}

			using (Stream streamSource = new MemoryStream(abySource))
			using (Stream stream = new SlowStream(streamSource))
			{
				byte[] read = await stream.ReadExactlyAsync(11);
				CollectionAssert.AreEqual(abySource, read);
			}

			using (Stream streamSource = new MemoryStream(abySource))
			using (Stream stream = new SlowStream(streamSource))
			{
				Assert.ThrowsAsync<EndOfStreamException>(() => stream.ReadExactlyAsync(12));
			}
		}

		[Test]
		public void ReadExactlyArray()
		{
			byte[] abySource = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

			using (Stream streamSource = new MemoryStream(abySource))
			using (Stream stream = new SlowStream(streamSource))
			{
				byte[] buffer = new byte[20];
				stream.ReadExactly(buffer, 3, 8);
				CollectionAssert.AreEqual(new byte[3].Concat(abySource.Take(8)).Concat(new byte[9]), buffer);
			}

			using (Stream streamSource = new MemoryStream(abySource))
			using (Stream stream = new SlowStream(streamSource))
			{
				byte[] buffer = new byte[20];
				stream.ReadExactly(buffer, 5, 11);
				CollectionAssert.AreEqual(new byte[5].Concat(abySource).Concat(new byte[4]), buffer);
			}

			using (Stream streamSource = new MemoryStream(abySource))
			using (Stream stream = new SlowStream(streamSource))
			{
				byte[] buffer = new byte[20];
				Assert.Throws<EndOfStreamException>(() => stream.ReadExactly(buffer, 5, 12));
			}
		}

		[Test]
		public void PartialStream()
		{
			byte[] bytes = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
			using (MemoryStream memoryStream = new MemoryStream())
			{
				memoryStream.Write(bytes, 0, bytes.Length);

				Stream partialStream1 = StreamUtility.CreatePartialStream(memoryStream, 3, 4, Ownership.None);
				CollectionAssert.AreEqual(new byte[] { 3, 4, 5, 6 }, partialStream1.ReadAllBytes());

				Stream partialStream2 = StreamUtility.CreatePartialStream(memoryStream, 6, null, Ownership.None);
				CollectionAssert.AreEqual(new byte[] { 6, 7, 8, 9, 10 }, partialStream2.ReadAllBytes());
			}
		}

		[Test]
		public void PartialStreamCopyTo()
		{
			byte[] bytes = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
			using (MemoryStream memoryStream = new MemoryStream())
			{
				memoryStream.Write(bytes, 0, bytes.Length);

				Stream partialStream1 = StreamUtility.CreatePartialStream(memoryStream, 3, 4, Ownership.None);
				using (var destination = new MemoryStream())
				{
					partialStream1.CopyTo(destination);
					CollectionAssert.AreEqual(new byte[] { 3, 4, 5, 6 }, destination.ToArray());
				}

				Stream partialStream2 = StreamUtility.CreatePartialStream(memoryStream, 6, null, Ownership.None);
				using (var destination = new MemoryStream())
				{
					partialStream2.CopyTo(destination);
					CollectionAssert.AreEqual(new byte[] { 6, 7, 8, 9, 10 }, destination.ToArray());
				}
			}
		}

		[Test]
		public async Task PartialStreamCopyToAsync()
		{
			byte[] bytes = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
			using (MemoryStream memoryStream = new MemoryStream())
			{
				memoryStream.Write(bytes, 0, bytes.Length);

				Stream partialStream1 = StreamUtility.CreatePartialStream(memoryStream, 3, 4, Ownership.None);
				using (var destination = new MemoryStream())
				{
					await partialStream1.CopyToAsync(destination);
					CollectionAssert.AreEqual(new byte[] { 3, 4, 5, 6 }, destination.ToArray());
				}

				Stream partialStream2 = StreamUtility.CreatePartialStream(memoryStream, 6, null, Ownership.None);
				using (var destination = new MemoryStream())
				{
					await partialStream2.CopyToAsync(destination);
					CollectionAssert.AreEqual(new byte[] { 6, 7, 8, 9, 10 }, destination.ToArray());
				}
			}
		}

		private class SlowStream : WrappingStreamBase
		{
			public SlowStream(Stream stream)
				: base(stream, Ownership.None)
			{
			}

			public override int Read(byte[] buffer, int offset, int count)
			{
				return base.Read(buffer, offset, Math.Min(count, 2));
			}
		}
	}
}
