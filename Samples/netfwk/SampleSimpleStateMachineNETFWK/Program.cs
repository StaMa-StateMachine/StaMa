using System;

namespace SampleSimpleStateMachineNETFWK
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Press E to trigger the transition from State1 to State2.");
            Console.WriteLine("Expect 2 seconds delay for timeout transition from State2 to State1.");
            Console.WriteLine("Press X or Space or ESC to exit.");

            SampleSimpleStateMachineNETFWK stateMachine = new SampleSimpleStateMachineNETFWK();

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
                            stateMachine.KeyPressed(key.Key);
                            break;
                        case ConsoleKey.X:
                        case ConsoleKey.Spacebar:
                        case ConsoleKey.Escape:
                            Console.WriteLine("{0} Closing application.", DateTime.Now.ToString("HH:mm:ss.fff"));
                            stateMachine.Finish();
                            exit = true;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    stateMachine.CheckTimeouts();
                }
                System.Threading.Thread.Sleep(100); // Throttle CPU load.
            }
        }
    }
}
