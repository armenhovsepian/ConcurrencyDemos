using System.Xml.Linq;

namespace ConcurrencyDemos
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Program started");

            //FireAndForgetDemo();

            //await OnlyOnePattrnDemo();

            await RunTaskWithCancellationDemo();

            Console.WriteLine("Main thread ended...");

            Console.ReadLine();
        }

        private static void FireAndForgetDemo()
        {
            FireAndForgetHelper.Run(DoSomething);
            FireAndForgetHelper.SafeRun(DoSomethingWithException, (ex) => Console.WriteLine($"Background task failed: {ex.Message}"));

            FireAndForgetHelper.Run(DoSomethingAsync);
            FireAndForgetHelper.SafeRun(DoSomethingWithExceptionAsync, (ex) => Console.WriteLine($"Background task failed: {ex.Message}"));
        }

        private static async Task OnlyOnePattrnDemo()
        {
            var names = new string[] { "Jack", "Jane", "Joe" };
            var tasks = names.Select(name =>
            {
                Func<CancellationToken, Task<string>> func = (ct) => OnlyOnePatternHelper.GetContent(name, ct);
                return func;
            });

            // option 2
            //var tasks = names.Select(name => new Func<CancellationToken, Task<string>>((cancellationToken) =>
            //{
            //    return OnlyOnePatternHelper.GetContent(name, cancellationToken);
            //}));

            var content = await OnlyOnePatternHelper.RunAsync(tasks);
            Console.WriteLine(content);
        }

        private static async Task RunTaskWithCancellationDemo()
        {
            //await DoSomethingAsync().WithTimeout(TimeSpan.FromSeconds(7));

            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(5));

            //_ = Task.Run(async () =>
            //{
            //    await Task.Delay(TimeSpan.FromSeconds(5));
            //    Console.WriteLine("Cancelling the task...");
            //    cts.Cancel();
            //});

            await DoSomethingAsync().WithCancellation(cts.Token);
        }

        private static async Task DoSomethingAsync()
        {
            await Task.Delay(1000);

            var timeout = DateTime.UtcNow.AddSeconds(10);
            do
            {
                Console.WriteLine($"Running in the background thread with ID {Thread.CurrentThread.ManagedThreadId} at {DateTime.Now.ToString("hh:mm:ss")}!");
                await Task.Delay(TimeSpan.FromSeconds(2));
            } while (DateTime.UtcNow < timeout);

            Console.WriteLine("Done");
        }

        private static async Task DoSomethingWithExceptionAsync()
        {
            await Task.Delay(1000);

            var timeout = DateTime.UtcNow.AddSeconds(10);
            do
            {
                Console.WriteLine($"Running in the background thread with ID {Thread.CurrentThread.ManagedThreadId} at {DateTime.Now.ToString("hh:mm:ss")}!");
                await Task.Delay(TimeSpan.FromSeconds(2));
                throw new Exception($"Background task error in {nameof(DoSomethingWithExceptionAsync)}");
            } while (DateTime.UtcNow < timeout);
        }

        public static void DoSomething()
        {
            Thread.Sleep(1000);

            var timeout = DateTime.UtcNow.AddSeconds(10);
            do
            {
                Console.WriteLine($"Running in the background thread with ID {Thread.CurrentThread.ManagedThreadId} at {DateTime.Now.ToString("hh:mm:ss")}!");
                Thread.Sleep(1000);

            } while (DateTime.UtcNow < timeout);
        }

        private static void DoSomethingWithException()
        {
            Thread.Sleep(1000);

            var timeout = DateTime.UtcNow.AddSeconds(10);
            do
            {
                Console.WriteLine($"Running in the background thread with ID {Thread.CurrentThread.ManagedThreadId} at {DateTime.Now.ToString("hh:mm:ss")}!");
                Thread.Sleep(3000);

                throw new Exception($"Background task error in {nameof(DoSomethingWithException) }");
            } while (DateTime.UtcNow < timeout);
        }
    }

    //public static class TaskExtensions
    //{
    //    public static async Task WithTimeout(this Task task, TimeSpan timeout)
    //    {
    //        var delayTask = Task.Delay(timeout);
    //        var completed = await Task.WhenAny(task, delayTask).ConfigureAwait(false);
    //        if (completed != task)
    //            throw new TimeoutException($"The operation has timed out after {timeout}.");
    //        await task.ConfigureAwait(false);
    //    }

    //    public static async Task<T> WithTimeout<T>(this Task<T> task, TimeSpan timeout)
    //    {
    //        var delayTask = Task.Delay(timeout);
    //        var completed = await Task.WhenAny(task, delayTask).ConfigureAwait(false);
    //        if (completed != task)
    //            throw new TimeoutException($"The operation has timed out after {timeout}.");
    //        return await task.ConfigureAwait(false);
    //    }
    //}
}
