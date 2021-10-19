using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Faithlife.Utility
{
	/// <summary>
	/// <see cref="RebasedStream"/> is a stream wrapper that changes the effective origin of the wrapped stream.
	/// </summary>
	public sealed class RebasedStream : WrappingStreamBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RebasedStream"/> class; the current position in <paramref name="stream"/>
		/// will be the origin of the <see cref="RebasedStream"/>.
		/// </summary>
		/// <param name="stream">The base stream.</param>
		public RebasedStream(Stream stream)
			: this(stream, Ownership.None)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RebasedStream"/> class; the current position in <paramref name="stream"/>
		/// will be the origin of the <see cref="RebasedStream"/>.
		/// </summary>
		/// <param name="stream">The base stream.</param>
		/// <param name="ownership">The ownership of the base stream.</param>
		public RebasedStream(Stream stream, Ownership ownership)
			: base(stream, ownership)
		{
			m_baseOffset = stream.Position;
		}

		/// <summary>
		/// Gets the length in bytes of the stream.
		/// </summary>
		public override long Length => base.Length - m_baseOffset;

		/// <summary>
		/// Gets or sets the position within the current stream.
		/// </summary>
		public override long Position
		{
			get => base.Position - m_baseOffset;
			set => base.Position = value + m_baseOffset;
		}

		/// <summary>
		/// Sets the position within the current stream.
		/// </summary>
		/// <param name="offset">A byte offset relative to the <paramref name="origin"/> parameter.</param>
		/// <param name="origin">A value of type <see cref="SeekOrigin"/> indicating the reference point used to obtain the new position.</param>
		/// <returns>The new position within the current stream.</returns>
		public override long Seek(long offset, SeekOrigin origin)
		{
			if (origin == SeekOrigin.Begin)
				offset += m_baseOffset;

			return base.Seek(offset, origin) - m_baseOffset;
		}

		/// <summary>
		/// Sets the length of the current stream.
		/// </summary>
		/// <param name="value">The desired length of the current stream in bytes.</param>
		public override void SetLength(long value) => base.SetLength(value + m_baseOffset);

		/// <summary>
		/// Reads a sequence of bytes from the current stream and advances the position
		/// within the stream by the number of bytes read.
		/// </summary>
		public override int Read(byte[] buffer, int offset, int count) => WrappedStream.Read(buffer, offset, count);

		/// <summary>
		/// Asynchronously reads a sequence of bytes from the current stream, advances the position within the stream by the number of bytes read, and monitors cancellation requests.
		/// </summary>
		public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
			WrappedStream.ReadAsync(buffer, offset, count, cancellationToken);

#if NET6_0
		/// <summary>
		/// Asynchronously reads a sequence of bytes from the current stream, advances the position within the stream by the number of bytes read, and monitors cancellation requests.
		/// </summary>
		public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken) =>
			WrappedStream.ReadAsync(buffer, cancellationToken);
#endif

		/// <summary>
		/// Writes a sequence of bytes to the current stream and advances the current position
		/// within this stream by the number of bytes written.
		/// </summary>
		public override void Write(byte[] buffer, int offset, int count) => WrappedStream.Write(buffer, offset, count);

		/// <summary>
		/// Asynchronously writes a sequence of bytes to the current stream, advances the current position within this stream by the number of bytes written, and monitors cancellation requests.
		/// </summary>
		public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
			WrappedStream.WriteAsync(buffer, offset, count, cancellationToken);

#if NET6_0
		/// <summary>
		/// Asynchronously writes a sequence of bytes to the current stream, advances the current position within this stream by the number of bytes written, and monitors cancellation requests.
		/// </summary>
		public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken) =>
			WrappedStream.WriteAsync(buffer, cancellationToken);
#endif

		// the offset within the base stream where this stream begins
		private readonly long m_baseOffset;
	}
}
