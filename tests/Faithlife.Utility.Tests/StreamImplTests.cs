using System;
using System.IO;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class StreamImplTests
	{
		[SetUp]
		public void Setup()
		{
			m_abyBuffer = new byte[100];
		}

		[TearDown]
		public void TearDown()
		{
			m_abyBuffer = null;
		}

		[Test]
		public void ReadNullBuffer()
		{
			Assert.Throws<ArgumentNullException>(() => StreamImpl.CheckReadParameters(null, 0, 0, true, true));
		}

		[Test]
		public void ReadNegativeOffset()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => StreamImpl.CheckReadParameters(m_abyBuffer, -1, 0, true, true));
		}

		[Test]
		public void ReadNegativeLength()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => StreamImpl.CheckReadParameters(m_abyBuffer, 0, -1, true, true));
		}

		[Test]
		public void ReadTooMuch()
		{
			Assert.Throws<ArgumentException>(() => StreamImpl.CheckReadParameters(m_abyBuffer, 0, 200, true, true));
		}

		[Test]
		public void ReadPastEnd()
		{
			Assert.Throws<ArgumentException>(() => StreamImpl.CheckReadParameters(m_abyBuffer, 95, 10, true, true));
		}

		[Test]
		public void ReadClosedStream()
		{
			Assert.Throws<ObjectDisposedException>(() => StreamImpl.CheckReadParameters(m_abyBuffer, 0, 10, false, true));
		}

		[Test]
		public void ReadUnreadableStream()
		{
			Assert.Throws<NotSupportedException>(() => StreamImpl.CheckReadParameters(m_abyBuffer, 0, 10, true, false));
		}

		[Test]
		public void Read()
		{
			StreamImpl.CheckReadParameters(m_abyBuffer, 0, 10, true, true);
		}

		[Test]
		public void WriteNullBuffer()
		{
			Assert.Throws<ArgumentNullException>(() => StreamImpl.CheckWriteParameters(null, 0, 0, true, true));
		}

		[Test]
		public void WriteNegativeOffset()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => StreamImpl.CheckWriteParameters(m_abyBuffer, -1, 0, true, true));
		}

		[Test]
		public void WriteNegativeLength()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => StreamImpl.CheckWriteParameters(m_abyBuffer, 0, -1, true, true));
		}

		[Test]
		public void WriteTooMuch()
		{
			Assert.Throws<ArgumentException>(() => StreamImpl.CheckWriteParameters(m_abyBuffer, 0, 200, true, true));
		}

		[Test]
		public void WritePastEnd()
		{
			Assert.Throws<ArgumentException>(() => StreamImpl.CheckWriteParameters(m_abyBuffer, 95, 10, true, true));
		}

		[Test]
		public void WriteClosedStream()
		{
			Assert.Throws<ObjectDisposedException>(() => StreamImpl.CheckWriteParameters(m_abyBuffer, 0, 10, false, true));
		}

		[Test]
		public void WriteUnWriteableStream()
		{
			Assert.Throws<NotSupportedException>(() => StreamImpl.CheckWriteParameters(m_abyBuffer, 0, 10, true, false));
		}

		[Test]
		public void Write()
		{
			StreamImpl.CheckWriteParameters(m_abyBuffer, 0, 10, true, true);
		}

		private byte[] m_abyBuffer;
	}
}
