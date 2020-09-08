using System;
using System.Threading;

namespace Faithlife.Utility
{
	/// <summary>
	/// Extension methods for ReaderWriterLockSlim
	/// </summary>
	public static class ReaderWriterLockSlimUtility
	{
		/// <summary>
		/// Enters a read lock and returns a disposable object that, when disposed, will exit the lock.
		/// </summary>
		/// <param name="theLock">The lock on which the read lock should be entered.</param>
		/// <returns>A disposable object that, when disposed, will exit the lock.</returns>
		public static IDisposable AcquireReadLock(this ReaderWriterLockSlim theLock)
		{
			theLock.EnterReadLock();
			return Scope.Create(() => theLock.ExitReadLock());
		}

		/// <summary>
		/// Enters a write lock and returns a disposable object that, when disposed, will exit the lock.
		/// </summary>
		/// <param name="theLock">The lock on which the write lock should be entered.</param>
		/// <returns>A disposable object that, when disposed, will exit the lock.</returns>
		public static IDisposable AcquireWriteLock(this ReaderWriterLockSlim theLock)
		{
			theLock.EnterWriteLock();
			return Scope.Create(() => theLock.ExitWriteLock());
		}

		/// <summary>
		/// Enters an upgradeable read lock and returns a disposable object that, when disposed, will exit the lock.
		/// </summary>
		/// <param name="theLock">The lock on which the upgradeable read lock should be entered.</param>
		/// <returns>A disposable object that, when disposed, will exit the lock.</returns>
		public static IDisposable AcquireUpgradeableReadLock(this ReaderWriterLockSlim theLock)
		{
			theLock.EnterUpgradeableReadLock();
			return Scope.Create(() => theLock.ExitUpgradeableReadLock());
		}
	}
}
