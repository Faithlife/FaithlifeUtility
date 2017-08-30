using System;
using System.IO;
using System.Linq;
using System.Text;
using Faithlife.Utility.Threading;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class StreamUtilityTests
	{
		[Test]
		public void CopyNullSourceStream()
		{
			Assert.Throws<ArgumentNullException>(() => StreamUtility.CopyStream(null, new MemoryStream()));
		}

		[Test]
		public void CopyNullDestinationStream()
		{
			Assert.Throws<ArgumentNullException>(() => StreamUtility.CopyStream(new MemoryStream(), null));
		}

		[TestCase(1024)]
		[TestCase(65536)]
		[TestCase(123456)]
		public void CopyStream(int nSize)
		{
			byte[] abySource = CreateSourceData(nSize);
			MemoryStream streamSource = new MemoryStream(abySource, false);

			MemoryStream streamDest = new MemoryStream(nSize);

			Assert.AreEqual((long) nSize, StreamUtility.CopyStream(streamSource, streamDest));

			byte[] abyDest = streamDest.ToArray();
			CollectionAssert.AreEqual(abySource, abyDest);
		}

		[TestCase(1024, 512, null)]
		[TestCase(1024, 2048, null)]
		[TestCase(12345, 10000, null)]
		[TestCase(1024, 512, 200)]
		[TestCase(1024, 2048, 200)]
		[TestCase(12345, 10000, 200)]
		public void CopyStreamWithSize(int nSize, int nBytesToCopy, object nBufferSize)
		{
			byte[] abySource = CreateSourceData(nSize);
			MemoryStream streamSource = new MemoryStream(abySource, false);

			int nActualCopy = Math.Min(nSize, nBytesToCopy);
			MemoryStream streamDest = new MemoryStream(nActualCopy);

			long nBytesCopied;
			if (nBufferSize is int)
				nBytesCopied = StreamUtility.CopyStream(streamSource, streamDest, nBytesToCopy, (int) nBufferSize);
			else
				nBytesCopied = StreamUtility.CopyStream(streamSource, streamDest, nBytesToCopy);
			Assert.AreEqual((long) nActualCopy, nBytesCopied);

			byte[] abyDest = streamDest.ToArray();
			Assert.AreEqual(nActualCopy, abyDest.Length);
			byte[] abyActualSource = new byte[nActualCopy];
			Array.ConstrainedCopy(abySource, 0, abyActualSource, 0, nActualCopy);
			CollectionAssert.AreEqual(abyActualSource, abyDest);
		}

		private byte[] CreateSourceData(int nSize)
		{
			byte[] abySource = new byte[nSize];
			for (int nByte = 0; nByte < nSize; ++nByte)
			{
				abySource[nByte] = (byte) (nByte % 256);
			}
			return abySource;
		}

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
		public void WriteBatched()
		{
			byte[] bytes = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
			using (MemoryStream target = new MemoryStream())
			{
				target.WriteBatched(bytes, 0, bytes.Length, 3, WorkState.None);
				target.Flush();
				CollectionAssert.AreEqual(bytes, target.ToArray());
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

		private class SlowStream : WrappingStream
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
