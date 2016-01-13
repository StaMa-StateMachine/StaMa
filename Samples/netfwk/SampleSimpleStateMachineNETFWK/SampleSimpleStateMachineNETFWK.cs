using System;
using StaMa;

namespace SampleSimpleStateMachineNETFWK
{
    class SampleSimpleStateMachineNETFWK
    {
        private StateMachine m_stateMachine;
        private DateTime m_state2Entered;


        public SampleSimpleStateMachineNETFWK()
        {
            StateMachineTemplate t = new StateMachineTemplate();

            //## Begin Structure
            // Generated from <file:S:\StaMa_State_Machine_Controller_Library\StaMaShapesMaster.vst> page "SampleSimpleStateMachineNETFWK"
            // at 07-22-2015 22:09:50 using StaMaShapes Version 2300
            t.Region("State1", false);
                t.State("State1", EnterState1, ExitState1);
                    t.Transition("Transition1to2", "State2", "Event1", null, null);
                t.EndState();
                t.State("State2", EnterState2, ExitState2);
                    t.Transition("Transition2to1", "State1", null, IsState2Timeout, null);
                t.EndState();
            t.EndRegion();
            //## End Structure

            m_stateMachine = t.CreateStateMachine();
            m_stateMachine.TraceStateChange = this.TraceStateChange;

            m_stateMachine.Startup();
        }


        public void Finish()
        {
            m_stateMachine.Finish();
        }


        public void KeyPressed(ConsoleKey key)
        {
            m_stateMachine.SendTriggerEvent("Event1");
        }


        public void CheckTimeouts()
        {
            m_stateMachine.SendTriggerEvent(null);
        }


        private void EnterState1(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
            Console.WriteLine("Called EnterState1");
        }


        private void ExitState1(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
            Console.WriteLine("Called ExitState1");
        }


        private void EnterState2(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
            Console.WriteLine("Called EnterState2");
            m_state2Entered = DateTime.Now;
        }


        private void ExitState2(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
            Console.WriteLine("Called ExitState2");
        }


        private bool IsState2Timeout(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
            return DateTime.Now - m_state2Entered > new TimeSpan(0, 0, 0, 2);
        }


        private void TraceStateChange(StateMachine stateMachine,
                                      StateConfiguration stateConfigurationFrom,
                                      StateConfiguration stateConfigurationTo,
                                      Transition transition)
        {
            Console.WriteLine("{0} ActiveState={1} entered through Transition={2}",
                              DateTime.Now.ToString("HH:mm:ss.fff"),
                              stateConfigurationTo.ToString(),
                              (transition != null) ? transition.Name : "Startup/Finish");
        }
    }
}
