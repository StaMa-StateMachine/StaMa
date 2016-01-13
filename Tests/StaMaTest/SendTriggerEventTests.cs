#region SendTriggerEventTests.cs file
//
// Tests for StaMa state machine controller library
//
// Copyright (c) 2005-2015, Roland Schneider. All rights reserved.
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
    public class SendTriggerEventTests
    {
        private static readonly StateMachineDoActionCallback NullDo = (StateMachineDoActionCallback)null;


#if !MF_FRAMEWORK
        [TestCase(false, TestName = "SendTriggerEvent_NoTransitionWithoutDo_ExecutesExpected")]
        [TestCase(true, TestName = "SendTriggerEvent_NoTransitionWithDo_ExecutesExpected")]
        public void SendTriggerEvent_NoTransition_ExecutesExpected(bool useDoActions)
#else
        public void SendTriggerEvent_NoTransitionWithoutDo_ExecutesExpected() { SendTriggerEvent_NoTransition_ExecutesExpected(false); }
        public void SendTriggerEvent_NoTransitionWithDo_ExecutesExpected() { SendTriggerEvent_NoTransition_ExecutesExpected(true); }
        private void SendTriggerEvent_NoTransition_ExecutesExpected(bool useDoActions)
#endif
        {
            // Arrange
            string[] expectedExec;
            if (useDoActions)
            {
                expectedExec = new string[] { "EnS1", "DoS1",
                                              "CHK", "DoS1", "CHK",
                                              "CHK", "DoS1", "CHK",
                                              "CHK", "DoS1", "CHK",
                                              "CHK", "DoS1", "CHK",
                                              "CHK", "DoS1", "CHK" };
            }
            else
            {
                expectedExec = new string[] { "EnS1",
                                              "CHK",
                                              "CHK",
                                              "CHK",
                                              "CHK",
                                              "CHK" };
            }

            ActionRecorder recorder = new ActionRecorder();

            StateMachineTemplate t = new StateMachineTemplate(useDoActions ? StateMachineOptions.UseDoActions : StateMachineOptions.None);
            t.Region("S1", false);
                t.State("S1", recorder.CreateAction("EnS1"), recorder.CreateAction("ExS1"), useDoActions ? recorder.CreateDoAction("DoS1") : NullDo);
                t.EndState();
            t.EndRegion();
            StateMachine stateMachine = t.CreateStateMachine();
            stateMachine.TraceDispatchTriggerEvent = recorder.CreateTraceMethod("CHK");
            stateMachine.Startup();

            // Act
            for (int i = 0; i < 5; i++)
            {
                stateMachine.SendTriggerEvent(null);
            }

            // Assert
            Assert.That(recorder.RecordedActions, Is.EqualTo(expectedExec), "Unexpected action invocations");
        }


#if !MF_FRAMEWORK
        [TestCase(false, TestName = "SendTriggerEvent_WithOneTransitionWithoutDo_ExecutesExpected")]
        [TestCase(true, TestName = "SendTriggerEvent_WithOneTransitionWithDo_ExecutesExpected")]
        public void SendTriggerEvent_WithOneTransition_ExecutesExpected(bool useDoActions)
#else
        public void SendTriggerEvent_WithOneTransitionWithoutDo_ExecutesExpected() { SendTriggerEvent_WithOneTransition_ExecutesExpected(false); }
        public void SendTriggerEvent_WithOneTransitionWithDo_ExecutesExpected() { SendTriggerEvent_WithOneTransition_ExecutesExpected(true); }
        private void SendTriggerEvent_WithOneTransition_ExecutesExpected(bool useDoActions)
#endif
        {
            // Arrange
            string[] expectedExec;
            if (useDoActions)
            {
                expectedExec = new string[] { "EnS1", "DoS1",
                                              "CHK", "ExS1", "EnS1", "DoS1", "CHK",
                                              "CHK", "ExS1", "EnS1", "DoS1", "CHK",
                                              "CHK", "ExS1", "EnS1", "DoS1", "CHK",
                                              "CHK", "ExS1", "EnS1", "DoS1", "CHK",
                                              "CHK", "ExS1", "EnS1", "DoS1", "CHK", };
            }
            else
            {
                expectedExec = new string[] { "EnS1",
                                              "CHK", "ExS1", "EnS1", "CHK", 
                                              "CHK", "ExS1", "EnS1", "CHK", 
                                              "CHK", "ExS1", "EnS1", "CHK", 
                                              "CHK", "ExS1", "EnS1", "CHK", 
                                              "CHK", "ExS1", "EnS1", "CHK",  };
            }

            ActionRecorder recorder = new ActionRecorder();
            bool transitionEnabled = false;

            StateMachineTemplate t = new StateMachineTemplate(useDoActions ? StateMachineOptions.UseDoActions : StateMachineOptions.None);
            t.Region("S1", false);
                t.State("S1", recorder.CreateAction("EnS1"), recorder.CreateAction("ExS1"), useDoActions ? recorder.CreateDoAction("DoS1") : NullDo);
                    t.Transition("T1", "S1", "S1", null, (stm, ev, args) => transitionEnabled, (stm, ev, args) => { transitionEnabled = false; });
                t.EndState();
            t.EndRegion();
            StateMachine stateMachine = t.CreateStateMachine();
            stateMachine.TraceDispatchTriggerEvent = recorder.CreateTraceMethod("CHK");
            stateMachine.Startup();

            // Act
            for (int i = 0; i < 5; i++)
            {
                transitionEnabled = true;
                stateMachine.SendTriggerEvent(null);
            }

            // Assert
            Assert.That(recorder.RecordedActions, Is.EqualTo(expectedExec), "Unexpected action invocations");
        }


#if !MF_FRAMEWORK
        [TestCase(false, TestName = "SendTriggerEvent_WithTwoTransitionsWithoutDo_ExecutesExpected")]
        [TestCase(true, TestName = "SendTriggerEvent_WithTwoTransitionsWithDo_ExecutesExpected")]
        public void SendTriggerEvent_WithTwoTransitions_ExecutesExpected(bool useDoActions)
#else
        public void SendTriggerEvent_WithTwoTransitionsWithoutDo_ExecutesExpected() { SendTriggerEvent_WithTwoTransitions_ExecutesExpected(false); }
        public void SendTriggerEvent_WithTwoTransitionsWithDo_ExecutesExpected() { SendTriggerEvent_WithTwoTransitions_ExecutesExpected(true); }
        private void SendTriggerEvent_WithTwoTransitions_ExecutesExpected(bool useDoActions)
#endif
        {
            // Arrange
            string[] expectedExec;
            if (useDoActions)
            {
                expectedExec = new string[] { "EnS1", "DoS1",
                                              "CHK", "ExS1", "EnS2", "DoS2", "CHK", "ExS2", "EnS1", "DoS1", "CHK",
                                              "CHK", "ExS1", "EnS2", "DoS2", "CHK", "ExS2", "EnS1", "DoS1", "CHK",
                                              "CHK", "ExS1", "EnS2", "DoS2", "CHK", "ExS2", "EnS1", "DoS1", "CHK",
                                              "CHK", "ExS1", "EnS2", "DoS2", "CHK", "ExS2", "EnS1", "DoS1", "CHK",
                                              "CHK", "ExS1", "EnS2", "DoS2", "CHK", "ExS2", "EnS1", "DoS1", "CHK", };
            }
            else
            {
                expectedExec = new string[] { "EnS1",
                                              "CHK", "ExS1", "EnS2", "CHK", "ExS2", "EnS1", "CHK", 
                                              "CHK", "ExS1", "EnS2", "CHK", "ExS2", "EnS1", "CHK", 
                                              "CHK", "ExS1", "EnS2", "CHK", "ExS2", "EnS1", "CHK", 
                                              "CHK", "ExS1", "EnS2", "CHK", "ExS2", "EnS1", "CHK", 
                                              "CHK", "ExS1", "EnS2", "CHK", "ExS2", "EnS1", "CHK", };
            }

            ActionRecorder recorder = new ActionRecorder();
            bool transition1Enabled = false;
            bool transition2Enabled = false;

            StateMachineTemplate t = new StateMachineTemplate(useDoActions ? StateMachineOptions.UseDoActions : StateMachineOptions.None);
            t.Region("S1", false);
                t.State("S1", recorder.CreateAction("EnS1"), recorder.CreateAction("ExS1"), useDoActions ? recorder.CreateDoAction("DoS1") : NullDo);
                    t.Transition("T1", "S1", "S2", null, (stm, ev, args) => transition1Enabled, (stm, ev, args) => { transition1Enabled = false; transition2Enabled = true; });
                t.EndState();
                t.State("S2", recorder.CreateAction("EnS2"), recorder.CreateAction("ExS2"), useDoActions ? recorder.CreateDoAction("DoS2") : NullDo);
                    t.Transition("T2", "S2", "S1", null, (stm, ev, args) => transition2Enabled, (stm, ev, args) => {  transition2Enabled = false; });
                t.EndState();
            t.EndRegion();
            StateMachine stateMachine = t.CreateStateMachine();
            stateMachine.TraceDispatchTriggerEvent = recorder.CreateTraceMethod("CHK");
            stateMachine.Startup();

            // Act
            for (int i = 0; i < 5; i++)
            {
                transition1Enabled = true;
                stateMachine.SendTriggerEvent(null);
            }

            // Assert
            Assert.That(recorder.RecordedActions, Is.EqualTo(expectedExec), "Unexpected action invocations");
        }


        [Test]
        public void SendTriggerEvent_WithTwoTransitionsAndDoChangesGuard_ExecutesExpected()
        {
            // Arrange
            string[] expectedExec;
            expectedExec = new string[] { "EnS1", "DoS1",
                                          "CHK", "ExS1", "EnS2", "DoS2", "CHK", "ExS2", "EnS1", "DoS1", "CHK",
                                          "CHK", "ExS1", "EnS2", "DoS2", "CHK", "ExS2", "EnS1", "DoS1", "CHK",
                                          "CHK", "ExS1", "EnS2", "DoS2", "CHK", "ExS2", "EnS1", "DoS1", "CHK",
                                          "CHK", "ExS1", "EnS2", "DoS2", "CHK", "ExS2", "EnS1", "DoS1", "CHK",
                                          "CHK", "ExS1", "EnS2", "DoS2", "CHK", "ExS2", "EnS1", "DoS1", "CHK", };

            ActionRecorder recorder = new ActionRecorder();
            bool transition1Enabled = false;
            bool transition2Enabled = false;

            StateMachineTemplate t = new StateMachineTemplate(StateMachineOptions.UseDoActions);
            t.Region("S1", false);
                t.State("S1", recorder.CreateAction("EnS1"), recorder.CreateAction("ExS1"), recorder.CreateDoAction("DoS1"));
                    t.Transition("T1", "S1", "S2", null, (stm, ev, args) => transition1Enabled, (stm, ev, args) => { transition1Enabled = false; });
                t.EndState();
                t.State("S2", recorder.CreateAction("EnS2"), recorder.CreateAction("ExS2"), (stm) => { recorder.Add("DoS2"); transition2Enabled = true; });
                    t.Transition("T2", "S2", "S1", null, (stm, ev, args) => transition2Enabled, (stm, ev, args) => {  transition2Enabled = false; });
                t.EndState();
            t.EndRegion();
            StateMachine stateMachine = t.CreateStateMachine();
            stateMachine.TraceDispatchTriggerEvent = recorder.CreateTraceMethod("CHK");
            stateMachine.Startup();

            // Act
            for (int i = 0; i < 5; i++)
            {
                transition1Enabled = true;
                stateMachine.SendTriggerEvent(null);
            }

            // Assert
            Assert.That(recorder.RecordedActions, Is.EqualTo(expectedExec), "Unexpected action invocations");
        }


        [Test]
        public void SendTriggerEvent_WithOneTransitionsAndDoChangesGuard_ExecutesExpected()
        {
            // Arrange
            string[] expectedExec;
            expectedExec = new string[] { "EnS1", "DoS1",
                                          "CHK", "DoS1", "CHK", "ExS1", "EnS2", "DoS2", "CHK", "ExS2", "EnS1", "DoS1", "CHK",
                                          "CHK", "DoS1", "CHK", "ExS1", "EnS2", "DoS2", "CHK", "ExS2", "EnS1", "DoS1", "CHK",
                                          "CHK", "DoS1", "CHK", "ExS1", "EnS2", "DoS2", "CHK", "ExS2", "EnS1", "DoS1", "CHK",
                                          "CHK", "DoS1", "CHK", "ExS1", "EnS2", "DoS2", "CHK", "ExS2", "EnS1", "DoS1", "CHK",
                                          "CHK", "DoS1", "CHK", "ExS1", "EnS2", "DoS2", "CHK", "ExS2", "EnS1", "DoS1", "CHK", };

            ActionRecorder recorder = new ActionRecorder();
            int transition1State = 0;

            StateMachineTemplate t = new StateMachineTemplate(StateMachineOptions.UseDoActions);
            t.Region("S1", false);
            t.State("S1", recorder.CreateAction("EnS1"), recorder.CreateAction("ExS1"), (stm) => { recorder.Add("DoS1"); transition1State += 1; });
                    t.Transition("T1", "S1", "S2", null, (stm, ev, args) => transition1State == 1, null);
                t.EndState();
                t.State("S2", recorder.CreateAction("EnS2"), recorder.CreateAction("ExS2"), recorder.CreateDoAction("DoS2"));
                    t.Transition("T2", "S2", "S1", null, (stm, ev, args) => true, null);
                t.EndState();
            t.EndRegion();
            StateMachine stateMachine = t.CreateStateMachine();
            stateMachine.TraceDispatchTriggerEvent = recorder.CreateTraceMethod("CHK");
            stateMachine.Startup();

            // Act
            for (int i = 0; i < 5; i++)
            {
                transition1State = 0;
                stateMachine.SendTriggerEvent(null);
            }

            // Assert
            Assert.That(recorder.RecordedActions, Is.EqualTo(expectedExec), "Unexpected action invocations");
        }

    }
}
