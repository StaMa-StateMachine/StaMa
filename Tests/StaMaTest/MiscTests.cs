#region ExecutionTests.cs file
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
    public class MiscTests
    {
        [Test]
        public void Finish_WithTransitionInExitAction_NotExecuted()
        {
            const string StateA = "StateA";
            const string StateA1A = "StateA1A";
            const string Transi1 = "Transi1";

            const string Event1 = "Event1";
            bool transitionExecuted = false;

            StateMachineTemplate t = new StateMachineTemplate();

            t.Region(StateA, false);
                t.State(StateA, null, (s, ev, args) => s.SendTriggerEvent(Event1));
                    t.Region(StateA1A , false);
                        t.State(StateA1A);
                            t.Transition(Transi1, new string[] { StateA1A }, new string[] { StateA1A }, Event1, null, (s, ev, args) => transitionExecuted = true);
                        t.EndState();
                    t.EndRegion();
                t.EndState();
            t.EndRegion();

            Assert.That(t.StateConfigurationMax, Is.EqualTo(2), "StateConfigurationMax wrong");
            Assert.That(t.ConcurrencyDegree, Is.EqualTo(1), "ConcurrencyDegree wrong");

            StateMachine stateMachine = t.CreateStateMachine(this);

            stateMachine.Startup();
            stateMachine.Finish();

            StateConfiguration nirvana = t.CreateStateConfiguration(new string[] { });
            Assert.That(stateMachine.ActiveStateConfiguration.ToString(), Is.EqualTo(nirvana.ToString()), "Active state not as expected.");

            Assert.That(transitionExecuted, Is.EqualTo(false), "Unintended transition during finish executed.");
        }


        [Test]
        public void StartupSendFinish_WithTraceFunctions_NotifiesPreviousAndActiveStateProperly()
        {
            // Arrange
            const string StateA = "StateA";
            const string StateA1A = "StateA1A";
            const string StateA1B = "StateA1B";
            const string Transi1 = "Transi1";

            const string Event1 = "Event1";

            StateMachineTemplate t = new StateMachineTemplate();

            t.Region(StateA, false);
                t.State(StateA);
                    t.Region(StateA1A, false);
                        t.State(StateA1A);
                            t.Transition(Transi1, new string[] { StateA1B }, Event1);
                        t.EndState();
                        t.State(StateA1B);
                        t.EndState();
                    t.EndRegion();
                t.EndState();
            t.EndRegion();

            StateConfiguration nirvana = t.CreateStateConfiguration(new string[] { });

            StateMachine stateMachine = t.CreateStateMachine(this);
            int stateChangeNotificationCount = 0;
            StateConfiguration stateConfigurationFromNotified = null;
            StateConfiguration stateConfigurationToNotified = null;
            stateMachine.TraceStateChange = delegate(StateMachine stm, StateConfiguration stateConfigurationFrom, StateConfiguration stateConfigurationTo, Transition transition)
                                            {
                                                stateChangeNotificationCount += 1;
                                                stateConfigurationFromNotified = (StateConfiguration)stateConfigurationFrom.Clone();
                                                stateConfigurationToNotified = (StateConfiguration)stateConfigurationTo.Clone();
                                            };

            // Act
            stateMachine.Startup();

            // Assert
            Assert.That(stateChangeNotificationCount, Is.EqualTo(1));
            Assert.That(stateConfigurationFromNotified.ToString(), Is.EqualTo(nirvana.ToString()));
            Assert.That(stateConfigurationToNotified.ToString(), Is.EqualTo(t.CreateStateConfiguration(StateA1A).ToString()));

            // Act
            stateMachine.SendTriggerEvent(Event1);

            // Assert
            Assert.That(stateChangeNotificationCount, Is.EqualTo(2));
            Assert.That(stateConfigurationFromNotified.ToString(), Is.EqualTo(t.CreateStateConfiguration(StateA1A).ToString()));
            Assert.That(stateConfigurationToNotified.ToString(), Is.EqualTo(t.CreateStateConfiguration(StateA1B).ToString()));

            // Act
            stateMachine.Finish();

            // Assert
            Assert.That(stateChangeNotificationCount, Is.EqualTo(3));
            Assert.That(stateConfigurationFromNotified.ToString(), Is.EqualTo(t.CreateStateConfiguration(StateA1B).ToString()));
            Assert.That(stateConfigurationToNotified.ToString(), Is.EqualTo(nirvana.ToString()));
        }
    }
}
