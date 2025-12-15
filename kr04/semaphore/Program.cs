using System;
using System.Threading;
namespace semaphore;
class Program
{
    static void Main()
    {
        var semaphore = new SemaphoreSlim(3);
        var threads = new Thread[10];

        for (int i = 0; i < threads.Length; i++)
        {
            int id = i;
            threads[i] = new Thread(() =>
            {
                semaphore.Wait();
                try
                {
                    Console.WriteLine($"начали поток айдишник {id}");
                    Thread.Sleep(1000);
                    Console.WriteLine($"завершили поток айдишинк {id}");
                }
                finally
                {
                    semaphore.Release();
                }
            });
            threads[i].Start();
        }

        foreach (var thread in threads)
            thread.Join();
    }
}