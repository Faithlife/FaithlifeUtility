using System.IO;
using System.IO.Compression;

namespace Faithlife.Utility
{
	/// <summary>
	/// Methods for working with gzip.
	/// </summary>
	public static class GzipUtility
	{
		/// <summary>
		/// Creates a compressing write stream.
		/// </summary>
		/// <param name="compressedWriteStream">The destination for the compressed bytes.</param>
		/// <param name="ownership">The ownership of the specified stream.</param>
		/// <returns>The stream to write the uncompressed bytes to.</returns>
		public static Stream CreateCompressingWriteStream(Stream compressedWriteStream, Ownership ownership)
		{
			return new GZipStream(compressedWriteStream, CompressionMode.Compress, ownership != Ownership.Owns);
		}

		/// <summary>
		/// Creates a decompressing read stream.
		/// </summary>
		/// <param name="compressedReadStream">The source of the compressed bytes.</param>
		/// <param name="ownership">The ownership of the specified stream.</param>
		/// <returns>The stream to read the uncompressed bytes from.</returns>
		public static Stream CreateDecompressingReadStream(Stream compressedReadStream, Ownership ownership)
		{
			return new GZipStream(compressedReadStream, CompressionMode.Decompress, ownership != Ownership.Owns);
		}
	}
}
