using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Faithlife.Utility
{
	/// <summary>
	/// <see cref="TruncatedStream"/> is a read-only <see cref="WrappingStream"/> that will not read past the specified length.
	/// </summary>
	public sealed class TruncatedStream : WrappingStream
	{
		/// <summary>
		/// Creates a new truncated stream.
		/// </summary>
		/// <param name="length">The length of the truncated stream.</param>
		/// <param name="streamBase">The base stream.</param>
		/// <param name="ownership">The ownership of the base stream.</param>
		public TruncatedStream(Stream streamBase, long length, Ownership ownership)
			: base(streamBase, ownership)
		{
			m_length = length;
		}

		/// <summary>
		/// Returns false; writes are not supported.
		/// </summary>
		public override bool CanWrite
		{
			get { return false; }
		}

		/// <summary>
		/// Returns the (truncated) length of the stream.
		/// </summary>
		public override long Length
		{
			get { return Math.Min(m_length, base.Length); }
		}

		/// <summary>
		/// The current position in the stream.
		/// </summary>
		public override long Position
		{
			get { return base.Position; }
			set { m_offset = base.Position = value; }
		}

#if !NETSTANDARD1_4
		/// <summary>
		/// Starts an asynchronous read.
		/// </summary>
		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			return base.BeginRead(buffer, offset, TruncateCount(count), callback, state);
		}

		/// <summary>
		/// Throws an exception; writes are not supported.
		/// </summary>
		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			throw CreateWriteNotSupportedException();
		}

		/// <summary>
		/// Finishes an asynchronous read.
		/// </summary>
		public override int EndRead(IAsyncResult asyncResult)
		{
			int byteCount = base.EndRead(asyncResult);
			m_offset += byteCount;
			return byteCount;
		}

		/// <summary>
		/// Throws an exception; writes are not supported.
		/// </summary>
		public override void EndWrite(IAsyncResult asyncResult)
		{
			throw CreateWriteNotSupportedException();
		}
#endif

		/// <summary>
		/// Reads from the stream.
		/// </summary>
		public override int Read(byte[] buffer, int offset, int count)
		{
			int byteCount = base.Read(buffer, offset, TruncateCount(count));
			m_offset += byteCount;
			return byteCount;
		}

		/// <summary>
		/// Reads from the stream asynchronously.
		/// </summary>
		public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			int byteCount = await base.ReadAsync(buffer, offset, TruncateCount(count), cancellationToken).ConfigureAwait(false);
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

			int ch = base.ReadByte();
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
				offset -= base.Length - m_length;

			return m_offset = base.Seek(offset, origin);
		}

		/// <summary>
		/// Throws an exception; writes are not supported.
		/// </summary>
		public override void SetLength(long value)
		{
			throw CreateWriteNotSupportedException();
		}

		/// <summary>
		/// Throws an exception; writes are not supported.
		/// </summary>
		public override void Write(byte[] buffer, int offset, int count)
		{
			throw CreateWriteNotSupportedException();
		}

		/// <summary>
		/// Throws an exception; writes are not supported.
		/// </summary>
		public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			throw CreateWriteNotSupportedException();
		}

		/// <summary>
		/// Throws an exception; writes are not supported.
		/// </summary>
		public override void WriteByte(byte value)
		{
			throw CreateWriteNotSupportedException();
		}

		private int TruncateCount(int count)
		{
			int maxCount = (int) Math.Min(int.MaxValue, m_length - m_offset);
			return Math.Max(Math.Min(count, maxCount), 0);
		}

		private static Exception CreateWriteNotSupportedException()
		{
			return new NotSupportedException("Write not supported.");
		}

		readonly long m_length;
		long m_offset;
	}
}
