namespace ConcurrencyDemos
{
    public class OnlyOnePatternHelper
    {
        public static async Task<T> RunAsync<T>(IEnumerable<Func<CancellationToken, Task<T>>> funcs)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var tasks = funcs.Select(func => func(cancellationTokenSource.Token));
            var task = await Task.WhenAny(tasks);
            cancellationTokenSource.Cancel();
            return await task;
        }


        public static async Task<T> RunAsync<T>(params Func<CancellationToken, Task<T>>[] funcs)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var tasks = funcs.Select(func => func(cancellationTokenSource.Token));
            var task = await Task.WhenAny(tasks);
            cancellationTokenSource.Cancel();
            return await task;
        }


        public static async Task<string> GetContent(string name, CancellationToken cancellationToken)
        {
            using var httpClient = new HttpClient();
            using var response = await httpClient.GetAsync($"https://httpbin.org/get?foo={name}", cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

    }
}
