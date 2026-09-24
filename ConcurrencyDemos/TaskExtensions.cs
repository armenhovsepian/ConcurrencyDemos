namespace ConcurrencyDemos
{
    public static class TaskExtensions
    {
        public static async Task WithTimeout(this Task task, TimeSpan timeout)
        {
            var delayTask = Task.Delay(timeout);
            var completed = await Task.WhenAny(task, delayTask).ConfigureAwait(false);
            if (completed != task)
                throw new TimeoutException($"The operation has timed out after {timeout}.");
            await task.ConfigureAwait(false);
        }

        public static async Task<T> WithTimeout<T>(this Task<T> task, TimeSpan timeout)
        {
            var delayTask = Task.Delay(timeout);
            var completed = await Task.WhenAny(task, delayTask).ConfigureAwait(false);
            if (completed != task)
                throw new TimeoutException($"The operation has timed out after {timeout}.");
            return await task.ConfigureAwait(false);
        }

        public static async Task WithCancellation(this Task task, CancellationToken cancellationToken)
        {
            var completed = await Task.WhenAny(task, Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken));
            if (task == completed)
            {
                await task;
            }
            else
            {
                throw new OperationCanceledException(cancellationToken);
            }
        }


        public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

            using (cancellationToken.Register(state => ((TaskCompletionSource<object>)state!).TrySetResult(null), tcs))
            {
                var completed = await Task.WhenAny(task, tcs.Task).ConfigureAwait(false);
                if (task == completed)
                {
                    return await task;
                }
                else
                {
                    throw new OperationCanceledException(cancellationToken);
                }
            }
        }
    }
}
