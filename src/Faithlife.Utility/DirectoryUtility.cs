using System;
using System.IO;

namespace Faithlife.Utility
{
	/// <summary>
	/// Provides utility methods for working with directories.
	/// </summary>
	public static class DirectoryUtility
	{
		/// <summary>
		/// Attempts to recursively delete the directory at the specified path, returning <c>true</c> if successful.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns><c>true</c> if the directory was successfully deleted, otherwise <c>false</c>.</returns>
		public static bool TryDelete(string path)
		{
			// delete the directory, ignoring common exceptions (e.g., file in use, insufficient permissions)
			try
			{
				Directory.Delete(path, true);
				return true;
			}
			catch (DirectoryNotFoundException)
			{
				// already deleted
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
	}
}
