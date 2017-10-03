using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace Faithlife.Utility
{
	/// <summary>
	/// Provides helper methods for working with files.
	/// </summary>
	public static class FileUtility
	{
		/// <summary>
		/// Deletes the specified file, even if it is marked read-only.
		/// </summary>
		/// <param name="path">The name of the file to be deleted.</param>
		public static void Delete(string path)
		{
			// attempt to clear the read-only attribute
			if (File.Exists(path))
				File.SetAttributes(path, FileAttributes.Normal);
			File.Delete(path);
		}

		/// <summary>
		/// Attempts to delete the file at the specified path, returning <c>true</c> if successful.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns><c>true</c> if the file was successfully deleted or does not exist, otherwise <c>false</c>.</returns>
		public static bool TryDelete(string path)
		{
			// delete the file, ignoring common exceptions (e.g., file in use, insufficient permissions)
			try
			{
				FileUtility.Delete(path);
				return true;
			}
			catch (IOException)
			{
			}
			catch (UnauthorizedAccessException)
			{
			}

			return false;
		}

		/// <summary>
		/// Attempts to move the specified file to a new location, returning <c>true</c> if successful.
		/// </summary>
		/// <param name="sourceFileName">The name of the file to move.</param>
		/// <param name="destinationFileName">The new path for the file.</param>
		/// <returns><c>true</c> if the file was successfully moved, otherwise <c>false</c>.</returns>
		/// <remarks><paramref name="destinationFileName"/> must not exist.</remarks>
		public static bool TryMove(string sourceFileName, string destinationFileName)
		{
			// move the file, ignoring common exceptions (e.g., destination exists, insufficient permissions)
			try
			{
				File.Move(sourceFileName, destinationFileName);
				return true;
			}
			catch (FileNotFoundException)
			{
			}
			catch (IOException)
			{
			}
			catch (UnauthorizedAccessException)
			{
			}

			return false;
		}
	}
}
