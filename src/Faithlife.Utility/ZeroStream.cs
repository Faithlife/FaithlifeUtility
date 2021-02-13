using System;
using System.IO;

namespace Faithlife.Utility
{
	/// <summary>
	/// A stream of zeroes.
	/// </summary>
	/// <remarks>The stream tracks position and length like any stream, but any bytes
	/// written to the stream will be read as zeroes.</remarks>
	public class ZeroStream : Stream
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ZeroStream"/> class.
		/// </summary>
		public ZeroStream()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ZeroStream"/> class.
		/// </summary>
		/// <param name="length">The length of the stream in bytes.</param>
		public ZeroStream(long length) => DoSetLength(length);

		/// <summary>
		/// Returns whether the current stream supports reading.
		/// </summary>
		/// <value>True if the stream supports reading; otherwise, false.</value>
		public override bool CanRead => true;

		/// <summary>
		/// Returns whether the current stream supports writing.
		/// </summary>
		/// <value>True if the stream supports writing; otherwise, false.</value>
		public override bool CanWrite => true;

		/// <summary>
		/// Returns whether the current stream supports seeking.
		/// </summary>
		/// <value>True if the stream supports seeking; otherwise, false.</value>
		public override bool CanSeek => true;

		/// <summary>
		/// Gets or sets the position within the current stream.
		/// </summary>
		/// <value>The current position within the stream.</value>
		public override long Position
		{
			get => m_position;
			set => m_position = value;
		}

		/// <summary>
		/// Gets the length in bytes of the stream.
		/// </summary>
		/// <value>The length of the stream in bytes.</value>
		public override long Length => m_length;

		/// <summary>
		/// Reads zeroes from the current stream and advances the position within the stream by the number of bytes read.
		/// </summary>
		/// <param name="buffer">The target array.</param>
		/// <param name="offset">The zero-based byte offset in the buffer.</param>
		/// <param name="count">The maximum number of bytes to be read from the current stream.</param>
		/// <returns>The total number of bytes read into the buffer. This can be less than the number of bytes requested if that
		/// many bytes are not currently available, or zero (0) if the end of the stream has been reached.</returns>
		public override int Read(byte[] buffer, int offset, int count)
		{
			if (buffer is null)
				throw new ArgumentNullException(nameof(buffer));
			if (offset < 0)
				throw new ArgumentOutOfRangeException(nameof(offset), "Offset cannot be negative.");
			if (count < 0)
				throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be negative.");
			if (offset + count > buffer.Length)
				throw new ArgumentException("Offset and count extend past the end of the buffer.");

			var actualByteCount = (int) Math.Min(count, m_length - m_position);
			if (actualByteCount <= 0)
				return 0;

			Array.Clear(buffer, offset, actualByteCount);

			m_position += actualByteCount;
			return actualByteCount;
		}

		/// <summary>
		/// Writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
		/// </summary>
		/// <param name="buffer">The source array.</param>
		/// <param name="offset">The zero-based byte offset in the buffer.</param>
		/// <param name="count">The number of bytes to be written to the current stream.</param>
		/// <remarks>The bytes are not actually stored and will be read as zeroes.</remarks>
		public override void Write(byte[] buffer, int offset, int count)
		{
			if (buffer is null)
				throw new ArgumentNullException(nameof(buffer));
			if (offset < 0)
				throw new ArgumentOutOfRangeException(nameof(offset), "Offset cannot be negative.");
			if (count < 0)
				throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be negative.");
			if (offset + count > buffer.Length)
				throw new ArgumentException("Offset and count extend past the end of the buffer.");

			var newPosition = m_position + count;
			if (newPosition < 0)
				throw new IOException("Stream exceeded the maximum length.");

			m_position = newPosition;

			if (newPosition > m_length)
				m_length = newPosition;
		}

		/// <summary>
		/// Sets the position within the current stream.
		/// </summary>
		/// <param name="offset">A byte offset relative to the <paramref name="origin"/> parameter.</param>
		/// <param name="origin">A value of type <see cref="SeekOrigin"/> indicating the reference point
		/// used to obtain the new position.</param>
		/// <returns>The new position within the current stream.</returns>
		public override long Seek(long offset, SeekOrigin origin)
		{
			var newPosition = origin switch
			{
				SeekOrigin.Begin => offset,
				SeekOrigin.Current => m_position + offset,
				SeekOrigin.End => m_length + offset,
				_ => throw new ArgumentException("Invalid seek origin.", nameof(origin)),
			};

			if (newPosition < 0)
				throw new IOException("Cannot seek before begin.");

			m_position = newPosition;
			return m_position;
		}

		/// <summary>
		/// Sets the length of the current stream.
		/// </summary>
		/// <param name="value">The desired length of the current stream in bytes.</param>
		public override void SetLength(long value) => DoSetLength(value);

		/// <summary>
		/// Flushes the stream.
		/// </summary>
		/// <remarks>This method does nothing.</remarks>
		public override void Flush()
		{
		}

		private void DoSetLength(long value)
		{
			if (value < 0)
				throw new ArgumentOutOfRangeException(nameof(value), "Stream length cannot be negative.");

			m_length = value;

			if (m_position > value)
				m_position = value;
		}

		private long m_position;
		private long m_length;
	}
}
