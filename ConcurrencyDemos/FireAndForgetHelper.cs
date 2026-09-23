namespace ConcurrencyDemos
{
    public static class FireAndForgetHelper
    {

        // If the async method does some heavy synchronous work before its first await,
        // that part runs on the calling thread and blocks it.
        // To push the whole thing onto the thread pool, use Task.Run:

        public static void Run(Action action)
        {
            _ = Task.Run(action);
        }

        public static void Run(Func<Task> func)
        {
            _ = Task.Run(func);
        }

        public static void SafeRun(Func<Task> func, Action<Exception>? onError = null)
        {
            _ = Task.Run(func).ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    // Handle exceptions from the background task
                    Console.WriteLine($"Background task error: {t.Exception}");
                    onError?.Invoke(t.Exception);
                }
            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        public static void SafeRun(Action action, Action<Exception>? onError = null)
        {
            _ = Task.Run(action).ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    // Handle exceptions from the background task
                    Console.WriteLine($"Background task error: {t.Exception}");
                    onError?.Invoke(t.Exception);
                }
            }, TaskContinuationOptions.OnlyOnFaulted);
        }

    }
}
