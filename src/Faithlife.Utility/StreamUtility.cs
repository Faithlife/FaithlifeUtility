using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Faithlife.Utility.Threading;

namespace Faithlife.Utility
{
	/// <summary>
	/// Copies data from one stream to another.
	/// </summary>
	public static class StreamUtility
	{
		/// <summary>
		/// Copies all the data after the current position in the source stream to the destination stream.
		/// </summary>
		/// <param name="streamFrom">The source stream.</param>
		/// <param name="streamTo">The destination stream.</param>
		/// <returns>The number of bytes actually copied.</returns>
		public static long CopyStream(Stream streamFrom, Stream streamTo)
		{
			return CopyStream(streamFrom, streamTo, long.MaxValue, c_nStandardBufferSize);
		}

		/// <summary>
		/// Copies up to the specified number of bytes from the source stream to the destination stream.
		/// </summary>
		/// <param name="streamFrom">The source stream.</param>
		/// <param name="streamTo">The destination stream.</param>
		/// <param name="nBytesToCopy">The maximum number of bytes to copy.</param>
		/// <returns>The number of bytes actually copied.</returns>
		public static long CopyStream(Stream streamFrom, Stream streamTo, long nBytesToCopy)
		{
			return CopyStream(streamFrom, streamTo, nBytesToCopy, c_nStandardBufferSize);
		}

		/// <summary>
		/// Copies up to the specified number of bytes from the source stream to the destination stream.
		/// </summary>
		/// <param name="streamFrom">The source stream.</param>
		/// <param name="streamTo">The destination stream.</param>
		/// <param name="nBytesToCopy">The maximum number of bytes to copy.</param>
		/// <param name="nBufferSize">The internal buffer size to use when copying.</param>
		/// <returns>The number of bytes actually copied.</returns>
		public static long CopyStream(Stream streamFrom, Stream streamTo, long nBytesToCopy, int nBufferSize)
		{
			return CopyStream(streamFrom, streamTo, nBytesToCopy, nBufferSize, WorkState.None);
		}

		/// <summary>
		/// Copies up to the specified number of bytes from the source stream to the destination stream.
		/// </summary>
		/// <param name="streamFrom">The source stream.</param>
		/// <param name="streamTo">The destination stream.</param>
		/// <param name="nBytesToCopy">The maximum number of bytes to copy.</param>
		/// <param name="nBufferSize">The internal buffer size to use when copying.</param>
		/// <param name="workState">The work state.</param>
		/// <returns>The number of bytes actually copied.</returns>
		public static long CopyStream(Stream streamFrom, Stream streamTo, long nBytesToCopy, int nBufferSize, IWorkState workState)
		{
			return CopyStream(streamFrom, streamTo, nBytesToCopy, nBufferSize, workState, null);
		}

		/// <summary>
		/// Copies up to the specified number of bytes from the source stream to the destination stream.
		/// </summary>
		/// <param name="streamFrom">The source stream.</param>
		/// <param name="streamTo">The destination stream.</param>
		/// <param name="nBytesToCopy">The maximum number of bytes to copy.</param>
		/// <param name="nBufferSize">The internal buffer size to use when copying.</param>
		/// <param name="workState">The work state.</param>
		/// <param name="notifier">
		///	 Pass in an Action to be called as the stream is copied (gives a chance to update progress). 
		///	 The long passed to the action represents the current total of bytes copied.</param>
		/// <returns>The number of bytes actually copied.</returns>
		public static long CopyStream(Stream streamFrom, Stream streamTo, long nBytesToCopy, int nBufferSize, IWorkState workState, Action<long> notifier)
		{
			if (streamFrom == null)
				throw new ArgumentNullException("streamFrom");
			if (streamTo == null)
				throw new ArgumentNullException("streamTo");

			long nTotalBytesCopied = 0;

			// do nothing if no bytes to copy
			if (nBytesToCopy > 0)
			{
				// allocate temporary buffer
				if (nBufferSize > nBytesToCopy)
					nBufferSize = (int) nBytesToCopy;
				byte[] abyBuffer = new byte[nBufferSize];

				// transfer data from input stream to output stream
				while (nBytesToCopy > 0 && !workState.Canceled)
				{
					// determine bytes to read
					int nBytesToRead = nBytesToCopy < nBufferSize ? (int) nBytesToCopy : nBufferSize;

					// read from input stream
					int nBytesRead = streamFrom.Read(abyBuffer, 0, nBytesToRead);

					// stop loop if done
					if (nBytesRead == 0)
						break;

					// write bytes to output stream
					streamTo.Write(abyBuffer, 0, nBytesRead);

					// update number of bytes to copy
					nTotalBytesCopied += nBytesRead;
					nBytesToCopy -= nBytesRead;

					if (notifier != null)
						notifier(nTotalBytesCopied);
				}
			}

			return nTotalBytesCopied;
		}

		/// <summary>
		/// Writes the specified bytes to the stream, one batch at a time.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="bytes">The bytes.</param>
		/// <param name="byteIndex">The byte index.</param>
		/// <param name="byteCount">The byte count.</param>
		/// <param name="batchByteCount">The number of bytes per batch.</param>
		/// <param name="workState">The work state.</param>
		public static void WriteBatched(this Stream stream, byte[] bytes, int byteIndex, int byteCount, int batchByteCount, IWorkState workState)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");

			while (byteCount > 0 && !workState.Canceled)
			{
				int byteCountToWrite = Math.Min(batchByteCount, byteCount);
				stream.Write(bytes, byteIndex, byteCountToWrite);
				byteIndex += byteCountToWrite;
				byteCount -= byteCountToWrite;
			}
		}

		/// <summary>
		/// Copies the contents of <paramref name="stream"/> to a new <see cref="MemoryStream"/>.
		/// </summary>
		/// <param name="stream">The stream to copy.</param>
		/// <returns>A <see cref="MemoryStream"/> containing all the data read from the stream. The caller is responsible
		/// for disposing this stream.</returns>
		/// <remarks><para>The returned <see cref="MemoryStream"/> will be positioned at the first byte.</para>
		/// <para>This method is useful when <paramref name="stream"/> does not support seeking, but a seekable
		/// stream is required.</para></remarks>
		public static MemoryStream CopyToMemoryStream(Stream stream)
		{
			MemoryStream memoryStream = new MemoryStream();
			CopyStream(stream, memoryStream);
			memoryStream.Position = 0;
			return memoryStream;
		}

		/// <summary>
		/// Reads all bytes from the stream.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <returns>An array of all bytes read from the stream.</returns>
		public static byte[] ReadAllBytes(this Stream stream)
		{
			using (MemoryStream streamMemory = new MemoryStream())
			{
				CopyStream(stream, streamMemory);
				return streamMemory.ToArray();
			}
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
			if (stream == null)
				throw new ArgumentNullException("stream");
			if (buffer == null)
				throw new ArgumentNullException("buffer");
			if (offset < 0 || offset > buffer.Length)
				throw new ArgumentOutOfRangeException("offset");
			if (count < 0 || buffer.Length - offset < count)
				throw new ArgumentOutOfRangeException("count");

			// track total bytes read
			int totalBytesRead = 0;
			while (count > 0)
			{
				// read data
				int bytesRead = stream.Read(buffer, offset, count);

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
				throw new ArgumentOutOfRangeException("count");
			byte[] buffer = new byte[count];
			ReadExactly(stream, buffer, 0, count);
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

			if (length != null)
				stream = new TruncatedStream(stream, length.Value, ownership);

			return stream;
		}

		private const int c_nStandardBufferSize = 64 * 1024;
	}
}
