namespace ConcurrencyDemos
{
    internal class Program
    {
        static void Main(string[] args)
        {
            FireAndForgetHelper.Run(FireAndForgetHelper.FireAndForget);


            Console.ReadLine();
        }
    }
}
