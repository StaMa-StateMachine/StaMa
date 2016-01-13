using System;
using System.Collections;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Input;

namespace SampleSimpleStateMachineNETMF
{
    public class Program
    {
        public static void Main()
        {
            GPIOButtonInputProvider buttonInputProvider = new GPIOButtonInputProvider(GPIOButtonInputProvider_ButtonInput);
            Dispatcher.Run();
        }

        private static void GPIOButtonInputProvider_ButtonInput(InputReportArgs arg)
        {
            InputReportArgs args = (InputReportArgs)arg;
            RawButtonInputReport report = (RawButtonInputReport)args.Report;
            string info = report.Timestamp.ToLocalTime().ToString("HH:mm:ss.fff") +
                          " Button=" + report.Button.ToString() +
                          " Action=" + report.Actions.ToString();
            Debug.Print(info);
            Display.WriteLine(info);
        }
    }
}
