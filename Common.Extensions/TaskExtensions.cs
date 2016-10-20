using System;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Extensions
{
    public static class TaskExtensions
    {
        public static T WaitForResult<T>(this Task<T> task)
        {
            task.Wait();
            return task.Result;
        }
        public static T WaitForResult<T>(this Task<T> task, CancellationToken cancellationToken)
        {
            task.Wait(cancellationToken);
            return task.Result;
        }
        public static T WaitForResult<T>(this Task<T> task, TimeSpan timeout)
        {
            task.Wait(timeout);
            return task.Result;
        }
        public static T WaitForResult<T>(this Task<T> task, int millisecondsTimeout)
        {
            task.Wait(millisecondsTimeout);
            return task.Result;
        }
        public static T WaitForResult<T>(this Task<T> task, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            task.Wait(millisecondsTimeout, cancellationToken);
            return task.Result;
        }
    }
}
