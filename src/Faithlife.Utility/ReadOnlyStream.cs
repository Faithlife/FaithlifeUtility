using System;
using System.IO;

namespace Faithlife.Utility
{
	/// <summary>
	/// A read-only stream wrapper.
	/// </summary>
	public sealed class ReadOnlyStream : WrappingStreamBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReadOnlyStream"/> class.
		/// </summary>
		/// <param name="stream">The wrapped stream.</param>
		public ReadOnlyStream(Stream stream)
			: base(stream, Ownership.None)
		{
		}

		/// <summary>
		/// Gets a value indicating whether the current stream supports writing.
		/// </summary>
		/// <returns><c>true</c> if the stream supports writing; otherwise, <c>false</c>.</returns>
		public override bool CanWrite => false;

		/// <summary>
		/// Sets the length of the current stream.
		/// </summary>
		public override void SetLength(long value) => throw new NotSupportedException();

		/// <summary>
		/// Writes a sequence of bytes to the current stream and advances the current position
		/// within this stream by the number of bytes written.
		/// </summary>
		public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
	}
}
