#region CompositeStateTransitionTests.cs file
//
// Tests for StaMa state machine controller library
//
// Copyright (c) 2005-2014, Roland Schneider. All rights reserved.
//
#endregion

using System;
using StaMa;

using System.Collections;
#if !MF_FRAMEWORK
using NUnit.Framework;
#else
using MFUnitTest.Framework;
using Microsoft.SPOT;
#endif


namespace StaMaTest
{
    [TestFixture]
    public class CompositeStateTransitionTests
    {
#if !MF_FRAMEWORK
        [TestCase("Ev1", new string[] { "EnS1", "ExS1", "EnS2", "EnS2A2" }, TestName = "CompositeStateTransition_ToCompositeState_ExecutesActionsOrdered")]
        [TestCase("Ev2", new string[] { "EnS1", "ExS1", "EnS2", "EnS2A1" }, TestName = "CompositeStateTransition_ToSubState_ExecutesActionsOrdered")]
        public void CompositeStateTransition_ToCompositeState_ExecutesActionsOrdered(string signalEvent, string[] expectedActions)
#else
        public void CompositeStateTransition_ToCompositeState_ExecutesActionsOrdered() { CompositeStateTransition_ToCompositeState_ExecutesActionsOrdered("Ev1", new string[] { "EnS1", "ExS1", "EnS2", "EnS2A2" }); }
        public void CompositeStateTransition_ToSubState_ExecutesActionsOrdered() { CompositeStateTransition_ToCompositeState_ExecutesActionsOrdered("Ev2", new string[] { "EnS1", "ExS1", "EnS2", "EnS2A1" }); }
        private void CompositeStateTransition_ToCompositeState_ExecutesActionsOrdered(string signalEvent, string[] expectedActions)
#endif
        {
            // Arrange
            ActionRecorder recorder = new ActionRecorder();

            StateMachineTemplate t = new StateMachineTemplate();
            t.Region("S1", false);
                t.State("S1", recorder.CreateAction("EnS1"), recorder.CreateAction("ExS1"));
                    t.Transition("T1", "S1", "S2", "Ev1", null, null);
                    t.Transition("T2", "S1", "S2A1", "Ev2", null, null);
                t.EndState();
                t.State("S2", recorder.CreateAction("EnS2"), recorder.CreateAction("ExS2"));
                    t.Region("S2A2", false);
                        t.State("S2A1", recorder.CreateAction("EnS2A1"), recorder.CreateAction("ExS2A1"));
                        t.EndState();
                        t.State("S2A2", recorder.CreateAction("EnS2A2"), recorder.CreateAction("ExS2A2"));
                        t.EndState();
                    t.EndRegion();
                t.EndState();
            t.EndRegion();
            StateMachine stateMachine = t.CreateStateMachine();
            stateMachine.Startup();

            // Act
            stateMachine.SendTriggerEvent(signalEvent);

            // Assert
            Assert.That(recorder.RecordedActions, Is.EqualTo(expectedActions), "Unexpected action invocations");
        }



#if !MF_FRAMEWORK
        [TestCase("Ev1", new string[] { "EnS2", "EnS2A1", "ExS2A1", "ExS2", "EnS1" }, TestName = "CompositeStateTransition_FromSubStateToSimpleState_ExecutesActionsOrdered")]
        [TestCase("Ev2", new string[] { "EnS2", "EnS2A1", "ExS2A1", "ExS2", "EnS1" }, TestName = "CompositeStateTransition_FromCompositeStateToSimpleState_ExecutesActionsOrdered")]
        [TestCase("Ev3", new string[] { "EnS2", "EnS2A1", "ExS2A1", "EnS2A2" }, TestName = "CompositeStateTransition_FromSubStateToSubState_ExecutesActionsOrdered")]
        [TestCase("Ev4", new string[] { "EnS2", "EnS2A1", "ExS2A1", "ExS2", "EnS2", "EnS2A2" }, TestName = "CompositeStateTransition_FromSubStateToSubStateExternal_ExecutesActionsOrdered")]
        public void CompositeStateTransition_FromCompositeState_ExecutesActionsOrdered(string signalEvent, string[] expectedActions)
#else
        public void CompositeStateTransition_FromSubStateToSimpleState_ExecutesActionsOrdered() { CompositeStateTransition_FromCompositeState_ExecutesActionsOrdered("Ev1", new string[] { "EnS2", "EnS2A1", "ExS2A1", "ExS2", "EnS1" }); }
        public void CompositeStateTransition_FromCompositeStateToSimpleState_ExecutesActionsOrdered() { CompositeStateTransition_FromCompositeState_ExecutesActionsOrdered("Ev2", new string[] { "EnS2", "EnS2A1", "ExS2A1", "ExS2", "EnS1" }); }
        public void CompositeStateTransition_FromSubStateToSubState_ExecutesActionsOrdered() { CompositeStateTransition_FromCompositeState_ExecutesActionsOrdered("Ev3", new string[] { "EnS2", "EnS2A1", "ExS2A1", "EnS2A2" }); }
        public void CompositeStateTransition_FromSubStateToSubStateExternal_ExecutesActionsOrdered() { CompositeStateTransition_FromCompositeState_ExecutesActionsOrdered("Ev4", new string[] { "EnS2", "EnS2A1", "ExS2A1", "ExS2", "EnS2", "EnS2A2" }); }
        private void CompositeStateTransition_FromCompositeState_ExecutesActionsOrdered(string signalEvent, string[] expectedActions)
#endif
        {
            // Arrange
            ActionRecorder recorder = new ActionRecorder();

            StateMachineTemplate t = new StateMachineTemplate();
            t.Region("S2", false);
                t.State("S1", recorder.CreateAction("EnS1"), recorder.CreateAction("ExS1"));
                t.EndState();
                t.State("S2", recorder.CreateAction("EnS2"), recorder.CreateAction("ExS2"));
                    t.Transition("T2", "S2", "S1", "Ev2", null, null);
                    t.Transition("T4", "S2A1", "S2A2", "Ev4", null, null);
                    t.Region("S2A1", false);
                        t.State("S2A1", recorder.CreateAction("EnS2A1"), recorder.CreateAction("ExS2A1"));
                            t.Transition("T1", "S2A1", "S1", "Ev1", null, null);
                            t.Transition("T3", "S2A1", "S2A2", "Ev3", null, null);
                            t.EndState();
                        t.State("S2A2", recorder.CreateAction("EnS2A2"), recorder.CreateAction("ExS2A2"));
                        t.EndState();
                    t.EndRegion();
                t.EndState();
            t.EndRegion();
            StateMachine stateMachine = t.CreateStateMachine();
            stateMachine.Startup();

            // Act
            stateMachine.SendTriggerEvent(signalEvent);

            // Assert
            Assert.That(recorder.RecordedActions, Is.EqualTo(expectedActions), "Unexpected action invocations");
        }
    }
}
