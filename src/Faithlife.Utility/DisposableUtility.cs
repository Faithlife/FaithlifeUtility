using System;

namespace Faithlife.Utility
{
	/// <summary>
	/// Provides methods for manipulating disposable objects.
	/// </summary>
	public static class DisposableUtility
	{
		/// <summary>
		/// Disposes and nulls the specified object.
		/// </summary>
		/// <param name="obj">The object to dispose and null.</param>
		public static void Dispose<T>(ref T obj) where T : class, IDisposable
		{
			if (obj != null)
			{
				obj.Dispose();
				obj = null;
			}
		}

		/// <summary>
		/// Disposes and nulls the specified object.
		/// </summary>
		/// <param name="obj">The object to dispose and null.</param>
		/// <remarks>This method is similar to <see cref="Dispose{T}"/>, but doesn't require that the parameter implement <see cref="IDisposable"/>; it will be
		/// checked at runtime.</remarks>
		public static void DisposeObject<T>(ref T obj)
		{
			IDisposable disposable = obj as IDisposable;
			if (disposable != null)
				disposable.Dispose();
			obj = default(T);
		}

		/// <summary>
		/// Disposes the specified object after executing the specified delegate.
		/// </summary>
		/// <typeparam name="TInput">The type of the input.</typeparam>
		/// <typeparam name="TOutput">The type of the output.</typeparam>
		/// <param name="disposable">The object to dispose.</param>
		/// <param name="action">The delegate to execute before disposing the object.</param>
		/// <returns>The value returned by the delegate.</returns>
		public static TOutput DisposeAfter<TInput, TOutput>(this TInput disposable, Func<TInput, TOutput> action) where TInput : IDisposable
		{
			using (disposable)
				return action(disposable);
		}

		/// <summary>
		/// Disposes the specified object after executing the specified delegate.
		/// </summary>
		/// <typeparam name="TInput">The type of the input.</typeparam>
		/// <typeparam name="TOutput">The type of the output.</typeparam>
		/// <param name="disposable">The object to dispose.</param>
		/// <param name="action">The delegate to execute before disposing the object.</param>
		/// <returns>The value returned by the delegate.</returns>
		public static TOutput DisposeAfter<TInput, TOutput>(this TInput disposable, Func<TOutput> action) where TInput : IDisposable
		{
			using (disposable)
				return action();
		}

		/// <summary>
		/// Disposes the specified object after executing the specified delegate.
		/// </summary>
		/// <typeparam name="T">The type of the input</typeparam>
		/// <param name="disposable">The object to dispose.</param>
		/// <param name="action">The delegate to execute before disposing the object.</param>
		public static void DisposeAfter<T>(this T disposable, Action<T> action) where T : IDisposable
		{
			using (disposable)
				action(disposable);
		}

		/// <summary>
		/// Disposes the specified object after executing the specified delegate.
		/// </summary>
		/// <typeparam name="T">The type of the input</typeparam>
		/// <param name="disposable">The object to dispose.</param>
		/// <param name="action">The delegate to execute before disposing the object.</param>
		public static void DisposeAfter<T>(this T disposable, Action action) where T : IDisposable
		{
			using (disposable)
				action();
		}

		/// <summary>
		/// Adds the specified <see cref="IDisposable"/> object to <paramref name="disposables"/> and returns it.
		/// </summary>
		/// <typeparam name="T">The type of the <see cref="IDisposable"/> object.</typeparam>
		/// <param name="disposable">The <see cref="IDisposable"/> object.</param>
		/// <param name="disposables">A <paramref name="disposables"/> that will dispose <paramref name="disposable"/> when it is disposed.</param>
		/// <returns>The <see cref="IDisposable"/> object that was added to <paramref name="disposables"/>.</returns>
		public static T DisposeWith<T>(this T disposable, Disposables disposables)
			where T : IDisposable
		{
			disposables.Add(disposable);
			return disposable;
		}
	}
}
