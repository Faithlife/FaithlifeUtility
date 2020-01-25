using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Faithlife.Utility
{
	/// <summary>
	/// A <see cref="Stream"/> that wraps another stream. One major feature of <see cref="WrappingStream"/> is that it does not dispose the
	/// underlying stream when it is disposed if Ownership.None is used; this is useful when using classes such as <see cref="BinaryReader"/>
	/// that take ownership of the stream passed to their constructors.
	/// </summary>
	public sealed class WrappingStream : Stream
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="WrappingStream"/> class.
		/// </summary>
		/// <param name="stream">The wrapped stream.</param>
		/// <param name="ownership">Use Owns if the wrapped stream should be disposed when this stream is disposed.</param>
		public WrappingStream(Stream stream, Ownership ownership)
		{
			m_wrappedStream = stream ?? throw new ArgumentNullException(nameof(stream));
			m_ownership = ownership;
		}

		/// <summary>
		/// Gets a value indicating whether the current stream supports reading.
		/// </summary>
		/// <returns><c>true</c> if the stream supports reading; otherwise, <c>false</c>.</returns>
		public override bool CanRead => m_wrappedStream?.CanRead ?? false;

		/// <summary>
		/// Gets a value indicating whether the current stream supports seeking.
		/// </summary>
		/// <returns><c>true</c> if the stream supports seeking; otherwise, <c>false</c>.</returns>
		public override bool CanSeek => m_wrappedStream?.CanSeek ?? false;

		/// <summary>
		/// Gets a value that determines whether the current stream can time out.
		/// </summary>
		/// <value>A value that determines whether the current stream can time out.</value>
		public override bool CanTimeout => m_wrappedStream?.CanTimeout ?? false;

		/// <summary>
		/// Gets a value indicating whether the current stream supports writing.
		/// </summary>
		/// <returns><c>true</c> if the stream supports writing; otherwise, <c>false</c>.</returns>
		public override bool CanWrite => m_wrappedStream?.CanWrite ?? false;

		/// <summary>
		/// Gets the length in bytes of the stream.
		/// </summary>
		public override long Length => WrappedStream.Length;

		/// <summary>
		/// Gets or sets the position within the current stream.
		/// </summary>
		public override long Position
		{
			get => WrappedStream.Position;
			set => WrappedStream.Position = value;
		}

#if !NETSTANDARD1_4
		/// <summary>
		/// Begins an asynchronous read operation.
		/// </summary>
		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state) =>
			WrappedStream.BeginRead(buffer, offset, count, callback, state);

		/// <summary>
		/// Begins an asynchronous write operation.
		/// </summary>
		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state) =>
			WrappedStream.BeginWrite(buffer, offset, count, callback, state);

		/// <summary>
		/// Waits for the pending asynchronous read to complete.
		/// </summary>
		public override int EndRead(IAsyncResult asyncResult) => WrappedStream.EndRead(asyncResult);

		/// <summary>
		/// Ends an asynchronous write operation.
		/// </summary>
		public override void EndWrite(IAsyncResult asyncResult) => WrappedStream.EndWrite(asyncResult);
#endif

		/// <summary>
		/// Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
		/// </summary>
		public override void Flush() => WrappedStream.Flush();

		/// <summary>
		/// Asynchronously clears all buffers for this stream, causes any buffered data to be written to the underlying device, and monitors cancellation requests.
		/// </summary>
		public override Task FlushAsync(CancellationToken cancellationToken) => WrappedStream.FlushAsync(cancellationToken);

		/// <summary>
		/// Reads a sequence of bytes from the current stream and advances the position
		/// within the stream by the number of bytes read.
		/// </summary>
		public override int Read(byte[] buffer, int offset, int count) => WrappedStream.Read(buffer, offset, count);

		/// <summary>
		/// Reads a byte from the stream and advances the position within the stream by one byte, or returns -1 if at the end of the stream.
		/// </summary>
		public override int ReadByte() => WrappedStream.ReadByte();

		/// <summary>
		/// Gets or sets a value, in milliseconds, that determines how long the stream will attempt to read before timing out.
		/// </summary>
		/// <value>A value, in milliseconds, that determines how long the stream will attempt to read before timing out.</value>
		public override int ReadTimeout
		{
			get => WrappedStream.ReadTimeout;
			set => WrappedStream.ReadTimeout = value;
		}

		/// <summary>
		/// Asynchronously reads a sequence of bytes from the current stream, advances the position within the stream by the number of bytes read, and monitors cancellation requests.
		/// </summary>
		public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
			WrappedStream.ReadAsync(buffer, offset, count, cancellationToken);

		/// <summary>
		/// Asynchronously reads the bytes from the current stream and writes them to another stream, using a specified buffer size and cancellation token.
		/// </summary>
		public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken) =>
			WrappedStream.CopyToAsync(destination, bufferSize, cancellationToken);

		/// <summary>
		/// Sets the position within the current stream.
		/// </summary>
		/// <param name="offset">A byte offset relative to the <paramref name="origin"/> parameter.</param>
		/// <param name="origin">A value of type <see cref="T:System.IO.SeekOrigin"/> indicating the reference point used to obtain the new position.</param>
		/// <returns>The new position within the current stream.</returns>
		public override long Seek(long offset, SeekOrigin origin) => WrappedStream.Seek(offset, origin);

		/// <summary>
		/// Sets the length of the current stream.
		/// </summary>
		/// <param name="value">The desired length of the current stream in bytes.</param>
		public override void SetLength(long value) => WrappedStream.SetLength(value);

		/// <summary>
		/// Writes a sequence of bytes to the current stream and advances the current position
		/// within this stream by the number of bytes written.
		/// </summary>
		public override void Write(byte[] buffer, int offset, int count) => WrappedStream.Write(buffer, offset, count);

		/// <summary>
		/// Writes a byte to the current position in the stream and advances the position within the stream by one byte.
		/// </summary>
		public override void WriteByte(byte value) => WrappedStream.WriteByte(value);

		/// <summary>
		/// Gets or sets a value, in milliseconds, that determines how long the stream will attempt to write before timing out.
		/// </summary>
		/// <value>A value, in milliseconds, that determines how long the stream will attempt to write before timing out.</value>
		public override int WriteTimeout
		{
			get => WrappedStream.WriteTimeout;
			set => WrappedStream.WriteTimeout = value;
		}

		/// <summary>
		/// Asynchronously writes a sequence of bytes to the current stream, advances the current position within this stream by the number of bytes written, and monitors cancellation requests.
		/// </summary>
		public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
			WrappedStream.WriteAsync(buffer, offset, count, cancellationToken);

		/// <summary>
		/// Disposes or releases the wrapped stream, based on the value of the Ownership parameter passed to the constructor.
		/// </summary>
		/// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
			try
			{
				// if m_ownership is Ownership.Owns, we dispose the wrapped stream; otherwise, we don't close the wrapped stream,
				// but just release it to prevent future access to it through this WrappingStream (and allow it to be collected)
				if (disposing)
				{
					if (m_ownership == Ownership.Owns)
						m_wrappedStream?.Dispose();
					m_wrappedStream = null;
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		/// <summary>
		/// Gets the wrapped stream.
		/// </summary>
		/// <value>The wrapped stream.</value>
		private Stream WrappedStream
		{
			get
			{
				// throws an ObjectDisposedException if this object has been disposed
				if (m_wrappedStream == null)
					throw new ObjectDisposedException(nameof(WrappingStream));
				return m_wrappedStream;
			}
		}

		Stream? m_wrappedStream;
		readonly Ownership m_ownership;
	}
}
