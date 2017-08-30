using System;
using System.IO;

namespace Faithlife.Utility
{
	/// <summary>
	/// Provides implementations of common methods needed by an implementer of <see cref="Stream"/>.
	/// </summary>
	public static class StreamImpl
	{
		/// <summary>
		/// Performs standard validation of parameters passed to an implementation of the <see cref="Stream.Read"/> method.
		/// </summary>
		/// <param name="buffer">An array of bytes.</param>
		/// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin storing the data read from the current stream.</param>
		/// <param name="count">The maximum number of bytes to be read from the current stream.</param>
		/// <param name="bStreamOpen">Whether the stream is currently open (<c>true</c>) or closed (<c>false</c>).</param>
		/// <param name="bCanRead">Whether the current stream supports reading.</param>
		/// <exception cref="T:System.ArgumentException">The sum of <paramref name="offset"/> and <paramref name="count"/> is larger than the buffer length.</exception>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="buffer"/> is a null reference.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="offset"/> or <paramref name="count"/> is negative.</exception>
		/// <exception cref="T:System.NotSupportedException">The stream does not support reading.</exception>
		/// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
		public static void CheckReadParameters(byte[] buffer, int offset, int count, bool bStreamOpen, bool bCanRead)
		{
			// check parameters
			CheckCoreReadWriteParameters(buffer, offset, count, bStreamOpen);

			// check that stream supports reading
			if (!bCanRead)
				throw new NotSupportedException(OurMessages.NotSupported_StreamCannotRead);
		}

		/// <summary>
		/// Performs standard validation of parameters passed to an implementation of the <see cref="Stream.Write"/> method.
		/// </summary>
		/// <param name="buffer">An array of bytes.</param>
		/// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin copying bytes to the current stream.</param>
		/// <param name="count">The number of bytes to be written to the current stream.</param>
		/// <param name="bStreamOpen">Whether the stream is currently open (<c>true</c>) or closed (<c>false</c>).</param>
		/// <param name="bCanWrite">Whether the current stream supports writing.</param>
		/// <exception cref="T:System.ArgumentException">The sum of <paramref name="offset"/> and <paramref name="count"/> is larger than the buffer length.</exception>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="buffer"/> is a null reference.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="offset"/> or <paramref name="count"/> is negative.</exception>
		/// <exception cref="T:System.NotSupportedException">The stream does not support writing.</exception>
		/// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
		public static void CheckWriteParameters(byte[] buffer, int offset, int count, bool bStreamOpen, bool bCanWrite)
		{
			// check parameters
			CheckCoreReadWriteParameters(buffer, offset, count, bStreamOpen);

			// check that stream supports writing
			if (!bCanWrite)
				throw new NotSupportedException(OurMessages.NotSupported_StreamCannotWrite);
		}

		private static void CheckCoreReadWriteParameters(byte[] buffer, int offset, int count, bool bStreamOpen)
		{
			// check that stream is open
			if (!bStreamOpen)
				throw new ObjectDisposedException(null, OurMessages.StreamClosed);

			// check for null parameters
			if (buffer == null)
				throw new ArgumentNullException("buffer", OurMessages.ArgumentNull_Buffer);

			// check for out-of-range parameters
			if (offset < 0)
				throw new ArgumentOutOfRangeException("offset", OurMessages.ArgumentOutOfRange_MustBeNonNegative);
			if (count < 0)
				throw new ArgumentOutOfRangeException("count", OurMessages.ArgumentOutOfRange_MustBeNonNegative);

			// check for sufficient buffer size
			if ((buffer.Length - offset) < count)
				throw new ArgumentException(OurMessages.Argument_InvalidOffsetCount);
		}
	}
}
