using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Faithlife.Utility
{
	/// <summary>
	/// A stream wrapper that caches all data read from the underlying <see cref="Stream"/>.
	/// </summary>
	/// <remarks>This implementation doesn't enforce any upper bound on the amount of data that will be cached in memory.</remarks>
	public sealed class CachingStream : WrappingStreamBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CachingStream"/> class.
		/// </summary>
		/// <param name="stream">The stream to be cached.</param>
		/// <param name="ownership">Use <see cref="Ownership.Owns"/> if the cached stream should be disposed when this stream is disposed.</param>
		public CachingStream(Stream stream, Ownership ownership)
			: base(stream, ownership)
		{
			m_blocks = new List<byte[]>();
			m_position = stream.Position;
		}

		/// <summary>
		/// Gets a value indicating whether the current stream supports writing.
		/// </summary>
		/// <returns>Always returns <c>false</c> because <see cref="CachingStream"/> doesn't support writing.</returns>
		public override bool CanWrite => false;

		/// <summary>
		/// Disposes or releases the cached stream, based on the value of the Ownership parameter passed to the constructor.
		/// </summary>
		/// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
					m_blocks = null;
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		/// <summary>
		/// Gets or sets the position within the current stream.
		/// </summary>
		public override long Position
		{
			get
			{
				ThrowIfDisposed();
				return m_position;
			}
			set
			{
				ThrowIfDisposed();
				if (value < 0)
					throw new ArgumentOutOfRangeException(nameof(value));
				m_position = value;
			}
		}

		/// <summary>
		/// Reads a sequence of bytes from the current stream and advances the position
		/// within the stream by the number of bytes read.
		/// </summary>
		public override int Read(byte[] buffer, int offset, int count)
		{
			int blockIndex = (int) (Position / c_blockSize);
			byte[] data = LoadData(blockIndex);
			int blockOffset = Math.Min(data.Length, (int) (Position % c_blockSize));
			int bytesToCopy = Math.Max(Math.Min(count, data.Length - blockOffset), 0);
			Array.Copy(data, blockOffset, buffer, offset, bytesToCopy);
			Position += bytesToCopy;
			return bytesToCopy;
		}

		/// <summary>
		/// Reads a sequence of bytes from the current stream and advances the position
		/// within the stream by the number of bytes read.
		/// </summary>
		public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			int blockIndex = (int) (Position / c_blockSize);
			byte[] data = await LoadDataAsync(blockIndex).ConfigureAwait(false);
			int blockOffset = Math.Min(data.Length, (int) (Position % c_blockSize));
			int bytesToCopy = Math.Max(Math.Min(count, data.Length - blockOffset), 0);
			Array.Copy(data, blockOffset, buffer, offset, bytesToCopy);
			Position += bytesToCopy;
			return bytesToCopy;
		}

		/// <summary>
		/// Reads a byte from the stream and advances the position within the stream by one byte, or returns -1 if at the end of the stream.
		/// </summary>
		public override int ReadByte()
		{
			int blockIndex = (int) (Position / c_blockSize);
			int blockOffset = (int) (Position % c_blockSize);
			byte[] data = LoadData(blockIndex);
			if (blockOffset < data.Length)
			{
				Position++;
				return data[blockOffset];
			}
			else
			{
				return -1;
			}
		}

		/// <summary>
		/// Sets the position within the current stream.
		/// </summary>
		/// <param name="offset">A byte offset relative to the <paramref name="origin"/> parameter.</param>
		/// <param name="origin">A value of type <see cref="SeekOrigin"/> indicating the reference point used to obtain the new position.</param>
		/// <returns>The new position within the current stream.</returns>
		public override long Seek(long offset, SeekOrigin origin)
		{
			ThrowIfDisposed();
			WrappedStream.Position = m_position;
			m_position = base.Seek(offset, origin);
			return m_position;
		}

		/// <summary>
		/// Sets the length of the current stream.
		/// </summary>
		/// <param name="value">The desired length of the current stream in bytes.</param>
		public override void SetLength(long value) => throw new NotSupportedException();

		/// <summary>
		/// Writes a sequence of bytes to the current stream and advances the current position
		/// within this stream by the number of bytes written.
		/// </summary>
		public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

		/// <summary>
		/// Asynchronously writes a sequence of bytes to the current stream, advances the current position within this stream by the number of bytes written, and monitors cancellation requests.
		/// </summary>
		public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => throw new NotSupportedException();

		private byte[] LoadData(int blockIndex)
		{
			ThrowIfDisposed();

			if (m_blocks!.Count <= blockIndex)
				m_blocks.AddRange(new byte[blockIndex - m_blocks.Count + 1][]);

			byte[] blockData = m_blocks[blockIndex];
			if (blockData == null)
			{
				WrappedStream.Position = blockIndex * c_blockSize;
				blockData = new byte[c_blockSize];
				int bytesRead = WrappedStream.ReadBlock(blockData, 0, blockData.Length);
				if (bytesRead != c_blockSize)
					Array.Resize(ref blockData, bytesRead);
				m_blocks[blockIndex] = blockData;
			}

			return blockData;
		}

		private async Task<byte[]> LoadDataAsync(int blockIndex)
		{
			ThrowIfDisposed();

			if (m_blocks!.Count <= blockIndex)
				m_blocks.AddRange(new byte[blockIndex - m_blocks.Count + 1][]);

			byte[] blockData = m_blocks[blockIndex];
			if (blockData == null)
			{
				WrappedStream.Position = blockIndex * c_blockSize;
				blockData = new byte[c_blockSize];
				int bytesRead = await WrappedStream.ReadBlockAsync(blockData, 0, blockData.Length).ConfigureAwait(false);
				if (bytesRead != c_blockSize)
					Array.Resize(ref blockData, bytesRead);
				m_blocks[blockIndex] = blockData;
			}

			return blockData;
		}

		const int c_blockSize = 4096;

		List<byte[]>? m_blocks;
		long m_position;
	}
}
