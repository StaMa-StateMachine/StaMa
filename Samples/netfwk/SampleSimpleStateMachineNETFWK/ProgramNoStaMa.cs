using System;

namespace SampleSimpleStateMachineNETFWK
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            bool exit = false;
            while (!exit)
            {
                DateTime startTime = DateTime.Now;
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    switch (key.Key)
                    {
                        case ConsoleKey.E:
                            Console.WriteLine("{0} You pressed E!", DateTime.Now.ToString("HH:mm:ss.fff"));
                            break;
                        case ConsoleKey.X:
                        case ConsoleKey.Spacebar:
                        case ConsoleKey.Escape:
                            Console.WriteLine("{0} Closing application.", DateTime.Now.ToString("HH:mm:ss.fff"));
                            exit = true;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                }
                System.Threading.Thread.Sleep(100); // Throttle CPU load.
            }
        }
    }
}
