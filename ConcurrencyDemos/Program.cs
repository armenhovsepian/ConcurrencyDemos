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


        //private static async Task RunTask()
        //{
        //    var task1 = Task.Run(async () =>
        //    {
        //        await Task.Delay(2000);
        //        return "Task 1 completed";
        //    });
        //    var task2 = Task.Run(async () =>
        //    {
        //        await Task.Delay(1000);
        //        return "Task 2 completed";
        //    });
        //    var result = await task1.WithTimeout(TimeSpan.FromSeconds(3));
        //    Console.WriteLine(result);
        //    try
        //    {
        //        var result2 = await task2.WithTimeout(TimeSpan.FromSeconds(0.5));
        //        Console.WriteLine(result2);
        //    }
        //    catch (TimeoutException ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //}

        private static async Task DoSomethingAsync()
        {
            await Task.Delay(1000);

            var timeout = DateTime.UtcNow.AddSeconds(10);
            do
            {
                Console.WriteLine($"Running in the background thread with ID {Thread.CurrentThread.ManagedThreadId} at {DateTime.Now.ToString("hh:mm:ss")}!");
                await Task.Delay(3000);
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
                await Task.Delay(1000);
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
}
