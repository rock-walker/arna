// http://code.msdn.microsoft.com/Samples-for-Parallel-b4b76364

namespace System.Threading.Tasks
{
	/// <summary>Extension methods for TaskCompletionSource.</summary>
	public static class TaskCompletionSourceExtensions
	{
		/// <summary>Transfers the result of a Task to the TaskCompletionSource.</summary>
		/// <typeparam name="TResult">Specifies the type of the result.</typeparam>
		/// <param name="resultSetter">The TaskCompletionSource.</param>
		/// <param name="task">The task whose completion results should be transferred.</param>
		public static void SetFromTask<TResult>(this TaskCompletionSource<TResult> resultSetter, Task task)
		{
			switch (task.Status)
			{
				case TaskStatus.RanToCompletion: 
					resultSetter.SetResult(task is Task<TResult> ? ((Task<TResult>)task).Result : default(TResult)); 
					break;
				case TaskStatus.Faulted: resultSetter.SetException(task.Exception.InnerExceptions); 
					break;
				case TaskStatus.Canceled: resultSetter.SetCanceled(); break;
				default: throw new InvalidOperationException("The task was not completed.");
			}
		}

		/// <summary>Transfers the result of a Task to the TaskCompletionSource.</summary>
		/// <typeparam name="TResult">Specifies the type of the result.</typeparam>
		/// <param name="resultSetter">The TaskCompletionSource.</param>
		/// <param name="task">The task whose completion results should be transfered.</param>
		public static void SetFromTask<TResult>(this TaskCompletionSource<TResult> resultSetter, Task<TResult> task)
		{
			SetFromTask(resultSetter, (Task)task);
		}

		/// <summary>Attempts to transfer the result of a Task to the TaskCompletionSource.</summary>
		/// <typeparam name="TResult">Specifies the type of the result.</typeparam>
		/// <param name="resultSetter">The TaskCompletionSource.</param>
		/// <param name="task">The task whose completion results should be transfered.</param>
		/// <returns>Whether the transfer could be completed.</returns>
		public static bool TrySetFromTask<TResult>(this TaskCompletionSource<TResult> resultSetter, Task task)
		{
			switch (task.Status)
			{
				case TaskStatus.RanToCompletion: return resultSetter.TrySetResult(task is Task<TResult> ? ((Task<TResult>)task).Result : default(TResult));
				case TaskStatus.Faulted: return resultSetter.TrySetException(task.Exception.InnerExceptions);
				case TaskStatus.Canceled: return resultSetter.TrySetCanceled();
				default: throw new InvalidOperationException("The task was not completed.");
			}
		}

		/// <summary>Attempts to transfer the result of a Task to the TaskCompletionSource.</summary>
		/// <typeparam name="TResult">Specifies the type of the result.</typeparam>
		/// <param name="resultSetter">The TaskCompletionSource.</param>
		/// <param name="task">The task whose completion results should be transfered.</param>
		/// <returns>Whether the transfer could be completed.</returns>
		public static bool TrySetFromTask<TResult>(this TaskCompletionSource<TResult> resultSetter, Task<TResult> task)
		{
			return TrySetFromTask(resultSetter, (Task)task);
		}

		public static void FromAsync<TResult>(this TaskCompletionSource<TResult> tcs, IAsyncResult iar, Func<IAsyncResult, TResult> endMethod)
		{
			Exception error = null;
			OperationCanceledException canceled = null;
			TResult result = default(TResult);

			try
			{
				result = endMethod(iar);
			}
			catch (OperationCanceledException e)
			{
				canceled = e;
			}
			catch (Exception e)
			{
				error = e;
			}
			finally
			{
				if (canceled != null)
					tcs.TrySetCanceled();
				else if (error != null)
					tcs.TrySetException(error);
				else
					tcs.TrySetResult(result);
			}
		}
	}
}
