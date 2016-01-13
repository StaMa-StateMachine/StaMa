#region ExceptionBehaviorTests.cs file
//
// Tests for StaMa state machine controller library
//
// Copyright (c) 2005-2015, Roland Schneider. All rights reserved.
//
#endregion

using System;
using StaMa;

#if !MF_FRAMEWORK
using NUnit.Framework;
#else
using MFUnitTest.Framework;
using Microsoft.SPOT;
#endif


namespace StaMaTest
{
    [TestFixture]
    public class ExceptionBehaviorTests
    {
        [Test]
        public void SendTriggerEvent_WithExceptionThrowingExitAction_AllowsContinueAndExecutesExitActions()
        {
            const string StateA = "StateA";
            const string StateA1A = "StateA1A";
            const string StateA1B = "StateA1B";
            const string StateB = "StateB";

            const string Transi1 = "Transi1";
            const string Transi2 = "Transi2";

            const string Event1 = "Event1";
            const string EventGotoErrorState = "EventGotoErrorState";

            bool exceptionThrownOnce = false;

            ActionRecorder recorder = new ActionRecorder();

            StateMachineTemplate t = new StateMachineTemplate();

            t.Region(StateA, false);
                t.State(StateA, null, recorder.CreateAction("ExitA"));
                    t.Transition(Transi2, new string[] { StateA }, new string[] { StateB }, EventGotoErrorState, null, recorder.CreateAction("Transit2"));
                    t.Region(StateA1A , false);
                        t.State(StateA1A, null, (s, ev, args) => { if (!exceptionThrownOnce) { exceptionThrownOnce = true; throw new Exception(); }});
                            t.Transition(Transi1, new string[] { StateA1A }, new string[] { StateA1B }, Event1, null, recorder.CreateAction("Transit1"));
                        t.EndState();
                        t.State(StateA1B, recorder.CreateAction("EnterA1B"), recorder.CreateAction("ExitA1B"));
                        t.EndState();
                    t.EndRegion();
                t.EndState();
                t.State(StateB, recorder.CreateAction("EnterB"), null);
                t.EndState();
            t.EndRegion();

            //StateMachine s = t.CreateStateMachine(this, InvokeActivityWrapper); // Just an idea.
            StateMachine stateMachine = t.CreateStateMachine();

            stateMachine.Startup();
            try
            {
                stateMachine.SendTriggerEvent(Event1);
            }
            catch (Exception)
            {
                stateMachine.SendTriggerEvent(EventGotoErrorState);
            }

            Assert.That(stateMachine.ActiveStateConfiguration.ToString(), Is.EqualTo(t.CreateStateConfiguration(new string[] { StateB }).ToString()), "Active state after error escape transition not as expected.");
            Assert.That(recorder.RecordedActions, Is.EqualTo(new String[] { "ExitA", "Transit2", "EnterB" }), "Unexpected sequence of actions during error escape transition.");
        }


        //private static void InvokeActivityWrapper(StateMachineActivityCallback stateMachineActivityCallback, StateMachine stateMachine, object triggerEvent, EventArgs eventArgs)
        //{
        //    try
        //    {
        //        stateMachineActivityCallback(stateMachine, triggerEvent, eventArgs);
        //    }
        //    catch (Exception ex)
        //    {
        //        stateMachine.SendTriggerEvent(EventId.Error);
        //    }
        //}
    }
}
