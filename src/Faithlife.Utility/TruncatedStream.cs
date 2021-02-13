using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Faithlife.Utility
{
	/// <summary>
	/// <see cref="TruncatedStream"/> is a read-only stream wrapper that will not read past the specified length.
	/// </summary>
	public sealed class TruncatedStream : WrappingStreamBase
	{
		/// <summary>
		/// Creates a new truncated stream.
		/// </summary>
		/// <param name="length">The length of the truncated stream.</param>
		/// <param name="stream">The base stream.</param>
		/// <param name="ownership">The ownership of the base stream.</param>
		public TruncatedStream(Stream stream, long length, Ownership ownership)
			: base(stream, ownership)
		{
			m_length = length;
		}

		/// <summary>
		/// Returns false; writes are not supported.
		/// </summary>
		public override bool CanWrite => false;

		/// <summary>
		/// Returns the (truncated) length of the stream.
		/// </summary>
		public override long Length => Math.Min(m_length, WrappedStream.Length);

		/// <summary>
		/// The current position in the stream.
		/// </summary>
		public override long Position
		{
			get => WrappedStream.Position;
			set => m_offset = WrappedStream.Position = value;
		}

		/// <summary>
		/// Reads from the stream.
		/// </summary>
		public override int Read(byte[] buffer, int offset, int count)
		{
			var byteCount = WrappedStream.Read(buffer, offset, TruncateCount(count));
			m_offset += byteCount;
			return byteCount;
		}

		/// <summary>
		/// Reads from the stream asynchronously.
		/// </summary>
		public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
#if NETSTANDARD2_0
			var byteCount = await WrappedStream.ReadAsync(buffer, offset, TruncateCount(count), cancellationToken).ConfigureAwait(false);
#else
			var byteCount = await WrappedStream.ReadAsync(buffer.AsMemory(offset, TruncateCount(count)), cancellationToken).ConfigureAwait(false);
#endif
			m_offset += byteCount;
			return byteCount;
		}

		/// <summary>
		/// Reads a byte from the stream.
		/// </summary>
		public override int ReadByte()
		{
			if (m_offset >= m_length)
				return -1;

			var ch = base.ReadByte();
			if (ch != -1)
				m_offset++;
			return ch;
		}

		/// <summary>
		/// Changes the current position in the stream.
		/// </summary>
		public override long Seek(long offset, SeekOrigin origin)
		{
			if (origin == SeekOrigin.End)
				offset -= WrappedStream.Length - m_length;

			return m_offset = WrappedStream.Seek(offset, origin);
		}

		/// <summary>
		/// Throws an exception; writes are not supported.
		/// </summary>
		public override void SetLength(long value) => throw CreateWriteNotSupportedException();

		/// <summary>
		/// Throws an exception; writes are not supported.
		/// </summary>
		public override void Write(byte[] buffer, int offset, int count) => throw CreateWriteNotSupportedException();

		/// <summary>
		/// Asynchronously writes a sequence of bytes to the current stream, advances the current position within this stream by the number of bytes written, and monitors cancellation requests.
		/// </summary>
		public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => throw CreateWriteNotSupportedException();

		/// <summary>
		/// Throws an exception; writes are not supported.
		/// </summary>
		public override void Flush() => throw CreateWriteNotSupportedException();

		private int TruncateCount(int count)
		{
			var maxCount = (int) Math.Min(int.MaxValue, m_length - m_offset);
			return Math.Max(Math.Min(count, maxCount), 0);
		}

		private static Exception CreateWriteNotSupportedException() => new NotSupportedException("Write not supported.");

		private readonly long m_length;
		private long m_offset;
	}
}
