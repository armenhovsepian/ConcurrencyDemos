namespace ConcurrencyDemos
{
    public static class FireAndForgetHelper
    {

        public static void SafeRun(Action action)
        {
            _ = Task.Run(action);
        }

        public static void SafeRun(Func<Task> func)
        {
            _ = Task.Run(func);
        }

        public static void Run(Func<Task> func)
        {
            try
            {
                _ = Task.Run(func).ContinueWith(t =>
                {
                    if (t.Exception != null)
                    {
                        // Handle exceptions from the background task
                        Console.WriteLine($"Background task error: {t.Exception}");
                        //throw Exception()
                    }
                }, TaskContinuationOptions.OnlyOnFaulted);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static void Run(Action action)
        {
            try
            {
                _ = Task.Run(action).ContinueWith(t =>
                {
                    if (t.Exception != null)
                    {
                        // Handle exceptions from the background task
                        Console.WriteLine($"Background task error: {t.Exception}");
                        //throw Exception()
                    }
                }, TaskContinuationOptions.OnlyOnFaulted);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public static async Task FireAndForgetAsync()
        {
            await Task.Delay(1000);

            var timeout = DateTime.UtcNow.AddSeconds(10);
            do
            {
                Console.WriteLine($"Running in the background thread with ID {Thread.CurrentThread.ManagedThreadId} at {DateTime.Now.ToString("hh:mm:ss")}!");
                await Task.Delay(3000);
            } while (DateTime.UtcNow < timeout);
        }


        public static void FireAndForget()
        {
            Thread.Sleep(1000);

            var timeout = DateTime.UtcNow.AddSeconds(10);
            do
            {
                Console.WriteLine($"Running in the background thread with ID {Thread.CurrentThread.ManagedThreadId} at {DateTime.Now.ToString("hh:mm:ss")}!");
                Thread.Sleep(1000);

                throw new Exception("Background task error");

            } while (DateTime.UtcNow < timeout);
        }
    }
}
