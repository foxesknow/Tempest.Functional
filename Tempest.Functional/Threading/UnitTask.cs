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
        /// Gets a unit task has already completed successfully
        /// </summary>
        public static readonly Task<Unit> CompletedFast = Task.FromResult(new Unit());

        /// <summary>
        /// Returns a unit task that has completed with a specified exception
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">exception is null</exception>
        public static Task<Unit> FromException(Exception exception)
        {
            if(exception is null) throw new ArgumentNullException(nameof(exception));

            var tcs = new TaskCompletionSource<Unit>();
            tcs.SetException(exception);

            return tcs.Task;
        }

#if NET6_0_OR_GREATER
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
#endif
    
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

    }
}
