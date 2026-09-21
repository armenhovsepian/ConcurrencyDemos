using System.Xml.Linq;

namespace ConcurrencyDemos
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            //FireAndForgetHelper.Run(FireAndForgetHelper.FireAndForget);

            await OnlyOnePattrn();

            Console.ReadLine();
        }

        private static async Task OnlyOnePattrn()
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
    }
}
