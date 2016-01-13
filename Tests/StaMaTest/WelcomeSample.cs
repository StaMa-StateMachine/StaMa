using System;
using NUnit.Framework;
using StaMa;


namespace StaMaTest
{
    [TestFixture]
    public class WelcomeSample
    {
        [Test]
        public void Main()
        {
            StateMachineTemplate t = new StateMachineTemplate();
            t.Region("Stopped", false);
                t.State("Stopped");
                    t.Transition("T1", "Running", "Play");
                t.EndState();
                t.State("Loaded", StartMotor, StopMotor);
                    t.Transition("T2", "Stopped", "Stop");
                    t.Region("Running", false);
                        t.State("Running", EngageHead, ReleaseHead);
                            t.Transition("T3", "Paused", "Pause");
                        t.EndState();
                        t.State("Paused");
                            t.Transition("T4", "Running", "Play");
                        t.EndState();
                    t.EndRegion();
                t.EndState();
            t.EndRegion();

            StateMachine stateMachine = t.CreateStateMachine();

            stateMachine.Startup();
            stateMachine.SendTriggerEvent("Play");
            stateMachine.SendTriggerEvent("Pause");
            stateMachine.SendTriggerEvent("Stop");
            stateMachine.Finish();
        }

        private void StartMotor(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
            System.Console.WriteLine("StartMotor");
        }

        private void StopMotor(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
            System.Console.WriteLine("StopMotor");
        }

        private void EngageHead(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
            System.Console.WriteLine("EngageHead");
        }

        private void ReleaseHead(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
            System.Console.WriteLine("ReleaseHead");
        }
    }
}

/*
            //## Begin StateMachineTemplate21
            // Generated from <file:S:\StaMa_State_Machine_Controller_Library\StaMaShapesMaster.vst> page "WelcomeSample"
            // at 07-22-2015 22:04:59 using StaMaShapes Version 2300
            t.Region("Stopped", false);
                t.State("Stopped", null, null);
                    t.Transition("T1", "Running", "Play", null, null);
                t.EndState();
                t.State("Loaded", StartMotor, StopMotor);
                    t.Transition("T2", "Stopped", "Stop", null, null);
                    t.Region("Running", false);
                        t.State("Running", EngageHead, ReleaseHead);
                            t.Transition("T3", "Paused", "Pause", null, null);
                        t.EndState();
                        t.State("Paused", null, null);
                            t.Transition("T4", "Running", "Play", null, null);
                        t.EndState();
                    t.EndRegion();
                t.EndState();
            t.EndRegion();
            //## End StateMachineTemplate21
*/
