using System;
using System.Collections;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Input;
using SampleSimpleStateMachineNETMF;

namespace SampleSimpleStateMachineNETMF
{
    public class Program
    {
        private static SampleSimpleStateMachineNETMF m_sampleSimpleStateMachine;

        public static void Main()
        {
            m_sampleSimpleStateMachine = new SampleSimpleStateMachineNETMF();
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

            if (report.Actions == RawButtonActions.ButtonUp)
            {
                m_sampleSimpleStateMachine.ButtonUp();
            }
        }
    }
}
