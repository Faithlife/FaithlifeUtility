using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class ZeroStreamTests
	{
		[Test]
		public void Constructor()
		{
			ZeroStream stream = new ZeroStream();
			Assert.IsTrue(stream.CanRead);
			Assert.IsTrue(stream.CanSeek);
			Assert.IsTrue(stream.CanWrite);
			Assert.AreEqual(0, stream.Length);
			Assert.AreEqual(0, stream.Position);
		}

		[Test]
		public void ConstructorWithLength()
		{
			ZeroStream stream = new ZeroStream(1234);
			Assert.IsTrue(stream.CanRead);
			Assert.IsTrue(stream.CanSeek);
			Assert.IsTrue(stream.CanWrite);
			Assert.AreEqual(1234, stream.Length);
			Assert.AreEqual(0, stream.Position);
		}

		[Test]
		public void ReadEmpty()
		{
			byte[] buffer = new byte[16];
			ZeroStream stream = new ZeroStream();
			Assert.AreEqual(0, stream.Read(buffer, 0, buffer.Length));
			stream.SetLength(7);
			buffer[0] = 1;
			buffer[7] = 1;
			Assert.AreEqual(7, stream.Read(buffer, 0, buffer.Length));
			Assert.AreEqual(0, buffer[0]);
			Assert.AreEqual(1, buffer[7]);
			Assert.AreEqual(0, stream.Read(buffer, 0, buffer.Length));
		}

		[Test]
		public void WriteAndRead()
		{
			ZeroStream stream = new ZeroStream();
			Assert.AreEqual(-1, stream.ReadByte());
			stream.WriteByte(1);
			Assert.AreEqual(-1, stream.ReadByte());
			stream.Position--;
			Assert.AreEqual(0, stream.ReadByte());
			Assert.AreEqual(1, stream.Length);
			Assert.AreEqual(-1, stream.ReadByte());
			Assert.AreEqual(1, stream.Length);
		}

		[Test]
		public void WriteAtPositionPastEnd()
		{
			ZeroStream stream = new ZeroStream();
			stream.Position = 100;
			Assert.AreEqual(0, stream.Length);
			stream.WriteByte(1);
			Assert.AreEqual(101, stream.Length);
		}

		[Test]
		public void Truncate()
		{
			ZeroStream stream = new ZeroStream();
			stream.WriteByte(1);
			Assert.AreEqual(1, stream.Length);
			Assert.AreEqual(1, stream.Position);
			stream.WriteByte(2);
			Assert.AreEqual(2, stream.Length);
			Assert.AreEqual(2, stream.Position);
			stream.SetLength(1);
			Assert.AreEqual(1, stream.Length);
			Assert.AreEqual(1, stream.Position);
			stream.SetLength(0);
			Assert.AreEqual(0, stream.Length);
			Assert.AreEqual(0, stream.Position);
		}
	}
}
