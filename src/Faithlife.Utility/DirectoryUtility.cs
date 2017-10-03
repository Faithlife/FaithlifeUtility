using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Faithlife.Utility
{
	/// <summary>
	/// Provides utility methods for working with directories.
	/// </summary>
	public static class DirectoryUtility
	{
		/// <summary>
		/// Recursively finds all files matching <paramref name="searchPatterns"/> in the folders specified by
		/// <paramref name="directoryPaths"/> and their subfolders.
		/// </summary>
		/// <param name="directoryPaths">The directory paths to search.</param>
		/// <param name="searchPatterns">The search patterns to match against the names of files.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A sequence of fully-qualified file paths.</returns>
		public static IEnumerable<string> FindFiles(IEnumerable<string> directoryPaths, IEnumerable<string> searchPatterns, CancellationToken cancellationToken)
		{
			return DoFindFiles(directoryPaths, searchPatterns, DoFindFiles, Directory.GetDirectories, cancellationToken);
		}

		/// <summary>
		/// Recursively finds all files matching <paramref name="searchPatterns"/> in the folders specified by
		/// <paramref name="directoryPaths"/> and their subfolders specified by <paramref name="getDirectories"/>.
		/// </summary>
		/// <param name="directoryPaths">The directory paths to search.</param>
		/// <param name="searchPatterns">The search patterns to match against the names of files.</param>
		/// <param name="getDirectories">A function which takes a path to a directory and returns a list of subdirectories of that path.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A sequence of FileInfo instances.</returns>
		public static IEnumerable<FileInfo> FindFileInfos(IEnumerable<string> directoryPaths, IEnumerable<string> searchPatterns, Func<string, string[]> getDirectories, CancellationToken cancellationToken)
		{
			return DoFindFiles(directoryPaths, searchPatterns, DoFindFileInfos, getDirectories, cancellationToken);
		}

		/// <summary>
		/// Recursively finds all files matching <paramref name="searchPatterns"/> in the folders specified by
		/// <paramref name="directoryPaths"/> and their subfolders.
		/// </summary>
		/// <param name="directoryPaths">The directory paths to search.</param>
		/// <param name="searchPatterns">The search patterns to match against the names of files.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A sequence of FileInfo instances.</returns>
		public static IEnumerable<FileInfo> FindFileInfos(IEnumerable<string> directoryPaths, IEnumerable<string> searchPatterns, CancellationToken cancellationToken)
		{
			return FindFileInfos(directoryPaths, searchPatterns, Directory.GetDirectories, cancellationToken);
		}

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

		private static IEnumerable<T> DoFindFiles<T>(IEnumerable<string> directoryPaths, IEnumerable<string> searchPatterns, Func<string, string, IEnumerable<T>> find, Func<string, string[]> getDirectories, CancellationToken cancellationToken)
		{
			var pathsStack = new Stack<string>(directoryPaths.Reverse());
			var searchPatternsList = searchPatterns.ToList();

			while (pathsStack.Count > 0)
			{
				cancellationToken.ThrowIfCancellationRequested();

				// process current path
				string path = pathsStack.Pop();

				// ensure folder exists
				try
				{
					if (!Directory.Exists(path))
						continue;
				}
				catch (IOException)
				{
					// Directory.Exists can throw PathTooLongException
					continue;
				}

				foreach (string searchPattern in searchPatternsList)
				{
					cancellationToken.ThrowIfCancellationRequested();

					// process all matching files for file type
					IEnumerable<T> files = Enumerable.Empty<T>();
					try
					{
						files = find(path, searchPattern);
					}
					catch (IOException)
					{
					}
					catch (UnauthorizedAccessException)
					{
					}
					catch (ArgumentException)
					{
					}
					foreach (T file in files)
					{
						cancellationToken.ThrowIfCancellationRequested();
						yield return file;
					}
				}

				cancellationToken.ThrowIfCancellationRequested();

				// add sub-folders to collection of paths to process
				// (avoiding recursive iterator design: http://blogs.msdn.com/toub/archive/2004/10/29/249858.aspx)
				// process all matching files for file type
				string[] directories = new string[0];
				try
				{
					directories = getDirectories(path);
				}
				catch (IOException)
				{
				}
				catch (UnauthorizedAccessException)
				{
				}

				foreach (string childDirectoryPath in directories.Reverse())
				{
					// TODO: check for cycles
					pathsStack.Push(childDirectoryPath);
				}
			}
		}

		private static IEnumerable<string> DoFindFiles(string path, string searchPattern) => Directory.GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly);

		private static IEnumerable<FileInfo> DoFindFileInfos(string path, string searchPattern)
		{
			return new DirectoryInfo(path).GetFiles(searchPattern, SearchOption.TopDirectoryOnly).Where(
				info =>
				{
					try
					{
						return info.FullName != null;
					}
					catch (IOException)
					{
						// FullName can throw PathTooLongException
						return false;
					}
				});
		}
	}
}
