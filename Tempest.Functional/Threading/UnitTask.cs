using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tempest.Functional.Threading
{
    /// <summary>
    /// Defines useful Task related code for a unit
    /// </summary>
    public static class UnitTask
    {
        /// <summary>
        /// A unit value task that has already completed successfully
        /// </summary>
        public static readonly ValueTask<Unit> CompletedValueTask = default;

        /// <summary>
        /// A unit task that has already completed successfully
        /// </summary>
        public static readonly Task<Unit> CompletedTask = Task.FromResult(new Unit());

        /// <summary>
        /// Returns a unit task that has completed with a specified exception
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">exception is null</exception>
        public static Task<Unit> FromException(Exception exception)
        {
            ArgumentNullException.ThrowIfNull(nameof(exception));

            var tcs = new TaskCompletionSource<Unit>();
            tcs.SetException(exception);

            return tcs.Task;
        }

        /// <summary>
        /// Returns a task that has been cancelled with a given token
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task<Unit> FromCanceled(CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<Unit>();
            tcs.SetCanceled(cancellationToken);

            return tcs.Task;
        }

        /// <summary>
        /// Returns a task that has been cancelled
        /// </summary>
        /// <returns></returns>
        public static Task<Unit> FromCanceled()
        {
            var tcs = new TaskCompletionSource<Unit>();
            tcs.SetCanceled();

            return tcs.Task;
        }


        /// <summary>
        /// Turns a task that returns nothing into a task that returns something
        /// </summary>
        /// <param name="task">The task to convert</param>
        /// <returns>A task that returns Unit</returns>
        public static Task<Unit> ToUnitTask(this Task task)
        {
            ArgumentNullException.ThrowIfNull(task);

            if(task.IsCompletedSuccessfully) return CompletedTask;

            return Execute(task);

            static async Task<Unit> Execute(Task task)
            {
                await task.ConfigureAwait(false);
                return default;
            }
        }

        /// <summary>
        /// Turns a value task that returns nothing into a value task that returns something
        /// </summary>
        /// <param name="task">The task to convert</param>
        /// <returns>A task that returns Unit</returns>
        public static ValueTask<Unit> ToUnitTask(this ValueTask task)
        {
            if(task.IsCompletedSuccessfully) return CompletedValueTask;

            return Execute(task);

            static async ValueTask<Unit> Execute(ValueTask task)
            {
                await task.ConfigureAwait(false);
                return default;
            }
        }

    }
}
