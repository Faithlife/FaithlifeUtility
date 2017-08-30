using System;
using System.IO;
using System.Threading.Tasks;
using Faithlife.Utility.Threading;

namespace Faithlife.Utility
{
	/// <summary>
	/// Utility methods for TextWriter.
	/// </summary>
	public static class TextWriterUtility
	{
		/// <summary>
		/// Writes the specified text, one batch at a time.
		/// </summary>
		/// <param name="writer">The text writer.</param>
		/// <param name="text">The text.</param>
		/// <param name="batchCharCount">The number of characters per batch.</param>
		/// <param name="workState">The work state.</param>
		public static void WriteBatched(this TextWriter writer, string text, int batchCharCount, IWorkState workState)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");
			if (text == null)
				throw new ArgumentNullException("text");

			int charCount = text.Length;
			if (charCount <= batchCharCount)
			{
				writer.Write(text);
			}
			else
			{
				char[] chars = new char[batchCharCount];
				int charIndex = 0;
				while (charCount > 0 && !workState.Canceled)
				{
					int charCountToWrite = Math.Min(batchCharCount, charCount);
					text.CopyTo(charIndex, chars, 0, charCountToWrite);
					writer.Write(chars, 0, charCountToWrite);
					charIndex += charCountToWrite;
					charCount -= charCountToWrite;
				}
			}
		}

		/// <summary>
		/// Writes the specified text, one batch at a time.
		/// </summary>
		/// <param name="writer">The text writer.</param>
		/// <param name="text">The text.</param>
		/// <param name="batchCharCount">The number of characters per batch.</param>
		/// <param name="workState">The work state.</param>
		public static async Task WriteBatchedAsync(this TextWriter writer, string text, int batchCharCount, IWorkState workState)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");
			if (text == null)
				throw new ArgumentNullException("text");

			int charCount = text.Length;
			if (charCount <= batchCharCount)
			{
				await writer.WriteAsync(text).ConfigureAwait(false);
			}
			else
			{
				char[] chars = new char[batchCharCount];
				int charIndex = 0;
				while (charCount > 0 && !workState.Canceled)
				{
					int charCountToWrite = Math.Min(batchCharCount, charCount);
					text.CopyTo(charIndex, chars, 0, charCountToWrite);
					await writer.WriteAsync(chars, 0, charCountToWrite).ConfigureAwait(false);
					charIndex += charCountToWrite;
					charCount -= charCountToWrite;
				}
			}
		}
	}
}
