using System;
using Microsoft.SPOT;
using StaMa;

namespace SampleSimpleStateMachineNETMF
{
    #region DevelopersGuide_classSampleSimpleStateMachineNETMFMultiInstance
    class SampleSimpleStateMachineNETMFMultiInstance
    {
        private static StateMachineTemplate m_stateMachineTemplate;
        private StateMachine m_stateMachine;
        private DispatcherTimer m_timeoutTimer;


        private static StateMachineTemplate GetStateMachineTemplate()
        {
            if (m_stateMachineTemplate == null)
            {
                StateMachineTemplate t = new StateMachineTemplate();

                t.Region("State1", false);
                    t.State("State1", EnterState1, ExitState1);
                        t.Transition("Transition1to2", "State2", "Event1", null, null);
                    t.EndState();
                    t.State("State2", EnterState2, ExitState2);
                        t.Transition("Transition2to1", "State1", "TimeoutState2", null, null);
                    t.EndState();
                t.EndRegion();

                m_stateMachineTemplate = t;
            }

            return m_stateMachineTemplate;
        }


        public SampleSimpleStateMachineNETMFMultiInstance()
        {
            StateMachineTemplate stateMachineTemplate = GetStateMachineTemplate();
            m_stateMachine = stateMachineTemplate.CreateStateMachine(this);
            m_stateMachine.TraceStateChange = this.TraceStateChange;

            m_timeoutTimer = new DispatcherTimer();
            m_timeoutTimer.Tick += TimeoutTimer_Tick;

            m_stateMachine.Startup();
        }


        private static void EnterState1(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
            ((SampleSimpleStateMachineNETMFMultiInstance)stateMachine.Context).EnterState1Instance(stateMachine, triggerEvent, eventArgs);
        }


        private static void ExitState1(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
            ((SampleSimpleStateMachineNETMFMultiInstance)stateMachine.Context).ExitState1Instance(stateMachine, triggerEvent, eventArgs);
        }


        private static void EnterState2(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
            ((SampleSimpleStateMachineNETMFMultiInstance)stateMachine.Context).EnterState2Instance(stateMachine, triggerEvent, eventArgs);
        }


        private static void ExitState2(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
            ((SampleSimpleStateMachineNETMFMultiInstance)stateMachine.Context).ExitState2Instance(stateMachine, triggerEvent, eventArgs);
        }


        private void EnterState1Instance(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
            Display.WriteLine("EnterState1Instance");
        }


        private void ExitState1Instance(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
            Display.WriteLine("ExitState1Instance");
        }


        private void EnterState2Instance(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
            Display.WriteLine("EnterState2Instance");
            m_timeoutTimer.Tag = "TimeoutState2";
            m_timeoutTimer.Interval = new TimeSpan(0, 0, 2);
            m_timeoutTimer.Start();
        }


        private void ExitState2Instance(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        {
            m_timeoutTimer.Stop();
            Display.WriteLine("ExitState2Instance");
        }


        void TimeoutTimer_Tick(object sender, EventArgs e)
        {
            m_timeoutTimer.Stop();
            m_stateMachine.SendTriggerEvent(m_timeoutTimer.Tag);
        }


        private void TraceStateChange(StateMachine stateMachine, StateConfiguration stateConfigurationFrom, StateConfiguration stateConfigurationTo, Transition transition)
        {
            string info = DateTime.Now.ToString("HH:mm:ss.fff") + " ActiveState=\"" + stateConfigurationTo.ToString() + "\"" + " Transition=" + ((transition != null) ? "\"" + transition.Name + "\"" : "Startup/Finish");
            Debug.Print(info);
            Display.WriteLine(info);
        }


        public void ButtonUp()
        {
            m_stateMachine.SendTriggerEvent("Event1");
        }
    }
    #endregion
}
