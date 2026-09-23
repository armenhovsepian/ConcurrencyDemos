namespace ConcurrencyDemos
{
    public static class TaskExtensions
    {
        public static async Task<T> WithTimeout<T>(this Task<T> task, TimeSpan timeout)
        {
            if (task == await Task.WhenAny(task, Task.Delay(timeout)))
            {
                return await task; // Task completed within timeout
            }
            else
            {
                throw new TimeoutException("The operation has timed out.");
            }
        }

        public static async Task WithCancellation(this Task task, CancellationToken cancellationToken)
        {
            if (task == await Task.WhenAny(task, Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken)))
            {
                await task; // Task completed within timeout
            }
            else
            {
                throw new TimeoutException("The operation has timed out.");
            }
        }


        public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

            using (cancellationToken.Register(state => ((TaskCompletionSource<object>)state!).TrySetResult(null), tcs))
            {
                if (task == await Task.WhenAny(task, tcs.Task))
                {
                    return await task; // Task completed within timeout
                }
                else
                {
                    throw new OperationCanceledException(cancellationToken);
                }
            }
        }
    }
}
