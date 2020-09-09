using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Faithlife.Utility
{
	/// <summary>
	/// Copies data from one stream to another.
	/// </summary>
	public static class StreamUtility
	{
		/// <summary>
		/// Reads all bytes from the stream.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <returns>An array of all bytes read from the stream.</returns>
		public static byte[] ReadAllBytes(this Stream stream)
		{
			using var streamMemory = new MemoryStream();
			stream.CopyTo(streamMemory);
			return streamMemory.ToArray();
		}

		/// <summary>
		/// Reads all bytes from the stream.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>An array of all bytes read from the stream.</returns>
		public static async Task<byte[]> ReadAllBytesAsync(this Stream stream, CancellationToken cancellationToken = default)
		{
			using var streamMemory = new MemoryStream();
			await stream.CopyToAsync(streamMemory, 81920, cancellationToken);
			return streamMemory.ToArray();
		}

		/// <summary>
		/// Reads <paramref name="count"/> bytes from <paramref name="stream"/> into
		/// <paramref name="buffer"/>, starting at the byte given by <paramref name="offset"/>.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		/// <param name="buffer">The buffer to read data into.</param>
		/// <param name="offset">The offset within the buffer at which data is first written.</param>
		/// <param name="count">The count of bytes to read.</param>
		/// <remarks>Unlike Stream.Read, this method will not return fewer bytes than requested
		/// unless the end of the stream is reached.</remarks>
		public static int ReadBlock(this Stream stream, byte[] buffer, int offset, int count)
		{
			// check arguments
			if (stream is null)
				throw new ArgumentNullException(nameof(stream));
			if (buffer is null)
				throw new ArgumentNullException(nameof(buffer));
			if (offset < 0 || offset > buffer.Length)
				throw new ArgumentOutOfRangeException(nameof(offset));
			if (count < 0 || buffer.Length - offset < count)
				throw new ArgumentOutOfRangeException(nameof(count));

			// track total bytes read
			var totalBytesRead = 0;
			while (count > 0)
			{
				// read data
				var bytesRead = stream.Read(buffer, offset, count);

				// check for end of stream
				if (bytesRead == 0)
					break;

				// move to next block
				offset += bytesRead;
				count -= bytesRead;
				totalBytesRead += bytesRead;
			}
			return totalBytesRead;
		}

		/// <summary>
		/// Reads <paramref name="count"/> bytes from <paramref name="stream"/> into
		/// <paramref name="buffer"/>, starting at the byte given by <paramref name="offset"/>.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		/// <param name="buffer">The buffer to read data into.</param>
		/// <param name="offset">The offset within the buffer at which data is first written.</param>
		/// <param name="count">The count of bytes to read.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <remarks>Unlike Stream.ReadAsync, this method will not return fewer bytes than requested
		/// unless the end of the stream is reached.</remarks>
		public static async Task<int> ReadBlockAsync(this Stream stream, byte[] buffer, int offset, int count, CancellationToken cancellationToken = default)
		{
			// check arguments
			if (stream is null)
				throw new ArgumentNullException(nameof(stream));
			if (buffer is null)
				throw new ArgumentNullException(nameof(buffer));
			if (offset < 0 || offset > buffer.Length)
				throw new ArgumentOutOfRangeException(nameof(offset));
			if (count < 0 || buffer.Length - offset < count)
				throw new ArgumentOutOfRangeException(nameof(count));

			// track total bytes read
			var totalBytesRead = 0;
			while (count > 0)
			{
				// read data
				var bytesRead = await stream.ReadAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);

				// check for end of stream
				if (bytesRead == 0)
					break;

				// move to next block
				offset += bytesRead;
				count -= bytesRead;
				totalBytesRead += bytesRead;
			}
			return totalBytesRead;
		}

		/// <summary>
		/// Reads exactly <paramref name="count"/> bytes from <paramref name="stream"/>.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		/// <param name="count">The count of bytes to read.</param>
		/// <returns>A new byte array containing the data read from the stream.</returns>
		public static byte[] ReadExactly(this Stream stream, int count)
		{
			if (count < 0)
				throw new ArgumentOutOfRangeException(nameof(count));
			byte[] buffer = new byte[count];
			ReadExactly(stream, buffer, 0, count);
			return buffer;
		}

		/// <summary>
		/// Reads exactly <paramref name="count"/> bytes from <paramref name="stream"/>.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		/// <param name="count">The count of bytes to read.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A new byte array containing the data read from the stream.</returns>
		public static async Task<byte[]> ReadExactlyAsync(this Stream stream, int count, CancellationToken cancellationToken = default)
		{
			if (count < 0)
				throw new ArgumentOutOfRangeException(nameof(count));
			byte[] buffer = new byte[count];
			await ReadExactlyAsync(stream, buffer, 0, count, cancellationToken).ConfigureAwait(false);
			return buffer;
		}

		/// <summary>
		/// Reads exactly <paramref name="count"/> bytes from <paramref name="stream"/> into
		/// <paramref name="buffer"/>, starting at the byte given by <paramref name="offset"/>.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		/// <param name="buffer">The buffer to read data into.</param>
		/// <param name="offset">The offset within the buffer at which data is first written.</param>
		/// <param name="count">The count of bytes to read.</param>
		public static void ReadExactly(this Stream stream, byte[] buffer, int offset, int count)
		{
			if (stream.ReadBlock(buffer, offset, count) != count)
				throw new EndOfStreamException();
		}

		/// <summary>
		/// Reads exactly <paramref name="count"/> bytes from <paramref name="stream"/> into
		/// <paramref name="buffer"/>, starting at the byte given by <paramref name="offset"/>.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		/// <param name="buffer">The buffer to read data into.</param>
		/// <param name="offset">The offset within the buffer at which data is first written.</param>
		/// <param name="count">The count of bytes to read.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		public static async Task ReadExactlyAsync(this Stream stream, byte[] buffer, int offset, int count, CancellationToken cancellationToken = default)
		{
			if (await stream.ReadBlockAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false) != count)
				throw new EndOfStreamException();
		}

		/// <summary>
		/// Uses <see cref="RebasedStream"/> and/or <see cref="TruncatedStream"/> to create a
		/// read-only partial stream wrapper.
		/// </summary>
		/// <param name="stream">The stream to wrap.</param>
		/// <param name="offset">The desired offset into the wrapped stream, which is immediately
		/// seeked to that position.</param>
		/// <param name="length">The desired length of the partial stream (optional).</param>
		/// <param name="ownership">Indicates the ownership of the wrapped stream.</param>
		/// <returns>The read-only partial stream wrapper.</returns>
		/// <remarks>If <paramref name="offset"/> is zero and <paramref name="length"/> is null,
		/// the stream is returned unwrapped.</remarks>
		public static Stream CreatePartialStream(Stream stream, long offset, long? length, Ownership ownership)
		{
			if (offset != 0)
			{
				stream.Position = offset;
				stream = new RebasedStream(stream, ownership);
			}

			if (length.HasValue)
				stream = new TruncatedStream(stream, length.Value, ownership);

			return stream;
		}
	}
}
