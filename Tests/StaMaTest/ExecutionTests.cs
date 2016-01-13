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
    public class ExecutionTests
    {
        [Test]
        public void SendTriggerEvent_WithOrthogonalRegions_ExecutesActionsInExpectedSequence()
        {
            // Arrange
            const string Startup = "*Startup*";
            const string Finish = "*Finish*";

            //## Begin StateAndTransitionNames57
            // Generated from <file:S:\StaMa_State_Machine_Controller_Library\StaMaShapesMaster.vst> page "UT_Execution1_OrthogonalRegions"
            // at 07-22-2015 22:05:11 using StaMaShapes Version 2300
            const string StateB = "StateB";
            const string Transi8 = "Transi8";
            const string Event6 = "Event6";
            const string Transi6 = "Transi6";
            const string Transi3 = "Transi3";
            const string Event2 = "Event2";
            const string Transi5 = "Transi5";
            const string Event5 = "Event5";
            const string StateB1A = "StateB1A";
            const string StateB1B = "StateB1B";
            const string StateB2A = "StateB2A";
            const string StateB2B = "StateB2B";
            const string Transi9 = "Transi9";
            const string Event4 = "Event4";
            const string StateB2B1A = "StateB2B1A";
            const string StateB2B2A = "StateB2B2A";
            const string Transi4 = "Transi4";
            const string Event3 = "Event3";
            const string StateB2B2B = "StateB2B2B";
            const string StateB2B3A = "StateB2B3A";
            const string Transi7 = "Transi7";
            const string StateB2B3B = "StateB2B3B";
            const string StateC = "StateC";
            const string StateA = "StateA";
            const string Transi1 = "Transi1";
            const string Event1 = "Event1";
            //## End StateAndTransitionNames57

            //## Begin ActionNames
            // Generated from <file:S:\StaMa_State_Machine_Controller_Library\StaMaShapesMaster.vst> page "UT_Execution1_OrthogonalRegions"
            // at 07-22-2015 22:05:11 using StaMaShapes Version 2300
            const string EnterB = "EnterB";
            const string ExitB = "ExitB";
            const string DoB = "DoB";
            const string EnterB1A = "EnterB1A";
            const string ExitB1A = "ExitB1A";
            const string DoB1A = "DoB1A";
            const string EnterB1B = "EnterB1B";
            const string ExitB1B = "ExitB1B";
            const string DoB1B = "DoB1B";
            const string EnterB2A = "EnterB2A";
            const string ExitB2A = "ExitB2A";
            const string DoB2A = "DoB2A";
            const string EnterB2B = "EnterB2B";
            const string ExitB2B = "ExitB2B";
            const string DoB2B = "DoB2B";
            const string EnterB2B1A = "EnterB2B1A";
            const string ExitB2B1A = "ExitB2B1A";
            const string DoB2B1A = "DoB2B1A";
            const string EnterB2B2A = "EnterB2B2A";
            const string ExitB2B2A = "ExitB2B2A";
            const string DoB2B2A = "DoB2B2A";
            const string EnterB2B2B = "EnterB2B2B";
            const string ExitB2B2B = "ExitB2B2B";
            const string DoB2B2B = "DoB2B2B";
            const string EnterB2B3A = "EnterB2B3A";
            const string ExitB2B3A = "ExitB2B3A";
            const string DoB2B3A = "DoB2B3A";
            const string EnterB2B3B = "EnterB2B3B";
            const string ExitB2B3B = "ExitB2B3B";
            const string DoB2B3B = "DoB2B3B";
            const string EnterC = "EnterC";
            const string ExitC = "ExitC";
            const string DoC = "DoC";
            const string EnterA = "EnterA";
            const string ExitA = "ExitA";
            const string DoA = "DoA";
            //## End ActionNames

            ActionRecorder recorder = new ActionRecorder();

            StateMachineTemplate t = new StateMachineTemplate(StateMachineOptions.UseDoActions);

            //## Begin StateMachineTemplate4
            // Generated from <file:S:\StaMa_State_Machine_Controller_Library\StaMaShapesMaster.vst> page "UT_Execution1_OrthogonalRegions"
            // at 07-22-2015 22:05:12 using StaMaShapes Version 2300
            t.Region(StateA, false);
                t.State(StateB, recorder.CreateAction(EnterB), recorder.CreateAction(ExitB), recorder.CreateDoAction(DoB));
                    t.Transition(Transi8, StateB2A, StateC, Event6, null, null);
                    t.Transition(Transi6, new string[] {StateB1B, StateB2B}, StateA, Event6, null, null);
                    t.Transition(Transi3, StateB1A, new string[] {StateB1B, StateB2B}, Event2, null, null);
                    t.Transition(Transi5, StateB2B2A, new string[] {StateB1B, StateB2B2A}, Event5, null, null);
                    t.Region(StateB1A, false);
                        t.State(StateB1A, recorder.CreateAction(EnterB1A), recorder.CreateAction(ExitB1A), recorder.CreateDoAction(DoB1A));
                        t.EndState();
                        t.State(StateB1B, recorder.CreateAction(EnterB1B), recorder.CreateAction(ExitB1B), recorder.CreateDoAction(DoB1B));
                        t.EndState();
                    t.EndRegion();
                    t.Region(StateB2A, false);
                        t.State(StateB2A, recorder.CreateAction(EnterB2A), recorder.CreateAction(ExitB2A), recorder.CreateDoAction(DoB2A));
                        t.EndState();
                        t.State(StateB2B, recorder.CreateAction(EnterB2B), recorder.CreateAction(ExitB2B), recorder.CreateDoAction(DoB2B));
                            t.Transition(Transi9, StateB2B, Event4, null, null);
                            t.Region(StateB2B1A, false);
                                t.State(StateB2B1A, recorder.CreateAction(EnterB2B1A), recorder.CreateAction(ExitB2B1A), recorder.CreateDoAction(DoB2B1A));
                                t.EndState();
                            t.EndRegion();
                            t.Region(StateB2B2A, false);
                                t.State(StateB2B2A, recorder.CreateAction(EnterB2B2A), recorder.CreateAction(ExitB2B2A), recorder.CreateDoAction(DoB2B2A));
                                    t.Transition(Transi4, StateB2B2B, Event3, null, null);
                                t.EndState();
                                t.State(StateB2B2B, recorder.CreateAction(EnterB2B2B), recorder.CreateAction(ExitB2B2B), recorder.CreateDoAction(DoB2B2B));
                                t.EndState();
                            t.EndRegion();
                            t.Region(StateB2B3A, true);
                                t.State(StateB2B3A, recorder.CreateAction(EnterB2B3A), recorder.CreateAction(ExitB2B3A), recorder.CreateDoAction(DoB2B3A));
                                    t.Transition(Transi7, StateB2B3B, Event3, null, null);
                                t.EndState();
                                t.State(StateB2B3B, recorder.CreateAction(EnterB2B3B), recorder.CreateAction(ExitB2B3B), recorder.CreateDoAction(DoB2B3B));
                                t.EndState();
                            t.EndRegion();
                        t.EndState();
                    t.EndRegion();
                t.EndState();
                t.State(StateC, recorder.CreateAction(EnterC), recorder.CreateAction(ExitC), recorder.CreateDoAction(DoC));
                t.EndState();
                t.State(StateA, recorder.CreateAction(EnterA), recorder.CreateAction(ExitA), recorder.CreateDoAction(DoA));
                    t.Transition(Transi1, StateB, Event1, null, null);
                t.EndState();
            t.EndRegion();
            //## End StateMachineTemplate4

            StateMachine stateMachine = t.CreateStateMachine(this);

            stateMachine.TraceStateChange = delegate(StateMachine stateMachinex, StateConfiguration stateConfigurationFrom, StateConfiguration stateConfigurationTo, Transition transition)
                                            {
                                                System.Console.WriteLine("TestExecConcurrent: Transition from {0} to {1} using {2}",
                                                                        stateConfigurationFrom.ToString(),
                                                                        stateConfigurationTo.ToString(),
                                                                        (transition != null) ? transition.Name : "*");
                                            };

            stateMachine.TraceTestTransition = delegate(StateMachine stateMachinex, Transition transition, object triggerEvent, EventArgs eventArgs)
                                               {
                                                   System.Console.WriteLine("TestExecConcurrent: Test transition {0} with event {1} in state {2}",
                                                                            transition.ToString(),
                                                                            (triggerEvent != null) ? triggerEvent.ToString() : "*",
                                                                            stateMachine.ActiveStateConfiguration.ToString());
                                               };

            stateMachine.TraceDispatchTriggerEvent = delegate(StateMachine stateMachinex, object triggerEvent, EventArgs eventArgs)
                                                    {
                                                        string eventName = (triggerEvent != null) ? triggerEvent.ToString() : "*";
                                                        System.Console.WriteLine("TestExecConcurrent: Dispatch event {0} in state {1}", eventName, stateMachine.ActiveStateConfiguration.ToString());
                                                    };

            Assert.That(recorder.RecordedActions, Is.EqualTo(new String[] {}), "Precondition not met: Actions were executed during state machine creation.");

            foreach (TestData testData in new TestData[]
                                          {
                                                new TestData()
                                                {
                                                    EventToSend = Startup,
                                                    ExpectedState = t.CreateStateConfiguration(new String[] { StateA }),
                                                    ExpectedActions = new String[] { EnterA, DoA, },
                                                },
                                                new TestData()
                                                {
                                                    EventToSend = Event1,
                                                    ExpectedState = t.CreateStateConfiguration(new String[] { StateB1A, StateB2A }),
                                                    ExpectedActions = new String[] { ExitA, EnterB, EnterB1A, EnterB2A, DoB, DoB1A, DoB2A, },
                                                },
                                                new TestData()
                                                {
                                                    EventToSend = Event2,
                                                    ExpectedState = t.CreateStateConfiguration(new String[] { StateB1B, StateB2B1A, StateB2B2A, StateB2B3A }),
                                                    ExpectedActions = new String[] { ExitB2A, ExitB1A, ExitB, EnterB, EnterB1B, EnterB2B, EnterB2B1A, EnterB2B2A, EnterB2B3A, DoB, DoB1B, DoB2B, DoB2B1A, DoB2B2A, DoB2B3A, },
                                                },
                                                new TestData()
                                                {
                                                    EventToSend = Event3,
                                                    ExpectedState = t.CreateStateConfiguration(new String[] { StateB1B, StateB2B1A, StateB2B2B, StateB2B3B }),
                                                    ExpectedActions = new String[] { ExitB2B2A, EnterB2B2B, ExitB2B3A, EnterB2B3B, DoB, DoB1B, DoB2B, DoB2B1A, DoB2B2B, DoB2B3B, },
                                                },
                                                new TestData()
                                                {
                                                    EventToSend = Event4,
                                                    ExpectedState = t.CreateStateConfiguration(new String[] { StateB1B, StateB2B1A, StateB2B2A, StateB2B3B }),
                                                    ExpectedActions = new String[] { ExitB2B3B, ExitB2B2B, ExitB2B1A, ExitB2B, EnterB2B, EnterB2B1A, EnterB2B2A, EnterB2B3B, DoB, DoB1B, DoB2B, DoB2B1A, DoB2B2A, DoB2B3B },
                                                },
                                                new TestData()
                                                {
                                                    EventToSend = Event5,
                                                    ExpectedState = t.CreateStateConfiguration(new String[] { StateB1B, StateB2B1A, StateB2B2A, StateB2B3B }),
                                                    ExpectedActions = new String[] { ExitB2B3B, ExitB2B2A, ExitB2B1A, ExitB2B, ExitB1B, ExitB, EnterB, EnterB1B, EnterB2B, EnterB2B1A, EnterB2B2A, EnterB2B3B, DoB, DoB1B, DoB2B, DoB2B1A, DoB2B2A, DoB2B3B, },
                                                },
                                                new TestData()
                                                {
                                                    EventToSend = Event6,
                                                    ExpectedState = t.CreateStateConfiguration(new String[] { StateA }),
                                                    ExpectedActions = new String[] { ExitB2B3B, ExitB2B2A, ExitB2B1A, ExitB2B, ExitB1B, ExitB, EnterA, DoA, },
                                                },
                                                new TestData()
                                                {
                                                    EventToSend = Finish,
                                                    ExpectedState = t.CreateStateConfiguration(new String[] { }),
                                                    ExpectedActions = new String[] { ExitA },
                                                },
                                          })
            {
                recorder.Clear();

                // Act
                switch (testData.EventToSend)
                {
                    case Startup:
                        stateMachine.Startup();
                        break;
                    case Finish:
                        stateMachine.Finish();
                        break;
                    default:
                        stateMachine.SendTriggerEvent(testData.EventToSend);
                        break;
                }
                
                // Assert
                Assert.That(stateMachine.ActiveStateConfiguration.ToString(), Is.EqualTo(testData.ExpectedState.ToString()), testData.EventToSend + ": Active state not as expected.");
                Assert.That(recorder.RecordedActions, Is.EqualTo(testData.ExpectedActions), testData.EventToSend + ": Unexpected entry and exit actions during state machine processing.");
            }
        }


        [Test]
        public void SendTriggerEvent_WithOrthogonalRegionsEarlyRegionExit_ExecutesActionsInExpectedSequence()
        {
            // Arrange
            const string Startup = "*Startup*";
            const string Finish = "*Finish*";

            //## Begin StateAndTransitionNames19
            // Generated from <file:S:\StaMa_State_Machine_Controller_Library\StaMaShapesMaster.vst> page "UT_Execution2_OrthogonalRegionsEarlyExit"
            // at 07-22-2015 22:05:14 using StaMaShapes Version 2300
            const string StateA = "StateA";
            const string StateA1A = "StateA1A";
            const string StateA1A1A = "StateA1A1A";
            const string Transi1 = "Transi1";
            const string StateA1A2A = "StateA1A2A";
            const string Transi2 = "Transi2";
            const string StateA1A2B = "StateA1A2B";
            const string StateA1B = "StateA1B";
            const string StateA2A = "StateA2A";
            const string Transi3 = "Transi3";
            const string StateA2B = "StateA2B";
            //## End StateAndTransitionNames19

            //## Begin EventNames
            // Generated from <file:S:\StaMa_State_Machine_Controller_Library\StaMaShapesMaster.vst> page "UT_Execution2_OrthogonalRegionsEarlyExit"
            // at 07-22-2015 22:05:14 using StaMaShapes Version 2300
            const string Event = "Event";
            //## End EventNames

            //## Begin StateAndTransitionNames30
            // Generated from <file:S:\StaMa_State_Machine_Controller_Library\StaMaShapesMaster.vst> page "UT_Execution2_OrthogonalRegionsEarlyExit"
            // at 07-22-2015 22:05:15 using StaMaShapes Version 2300
            const string EnterA = "EnterA";
            const string ExitA = "ExitA";
            const string EnterA1A = "EnterA1A";
            const string ExitA1A = "ExitA1A";
            const string EnterA1A1A = "EnterA1A1A";
            const string ExitA1A1A = "ExitA1A1A";
            const string EnterA1A2A = "EnterA1A2A";
            const string ExitA1A2A = "ExitA1A2A";
            const string EnterA1A2B = "EnterA1A2B";
            const string ExitA1A2B = "ExitA1A2B";
            const string EnterA1B = "EnterA1B";
            const string ExitA1B = "ExitA1B";
            const string EnterA2A = "EnterA2A";
            const string ExitA2A = "ExitA2A";
            const string EnterA2B = "EnterA2B";
            const string ExitA2B = "ExitA2B";
            //## End StateAndTransitionNames30
            
            ActionRecorder recorder = new ActionRecorder();

            StateMachineTemplate t = new StateMachineTemplate(StateMachineOptions.UseDoActions);

            //## Begin StateMachineTemplate18
            // Generated from <file:S:\StaMa_State_Machine_Controller_Library\StaMaShapesMaster.vst> page "UT_Execution2_OrthogonalRegionsEarlyExit"
            // at 07-22-2015 22:05:15 using StaMaShapes Version 2300
            t.Region(StateA, false);
                t.State(StateA, recorder.CreateAction(EnterA), recorder.CreateAction(ExitA));
                    t.Region(StateA1A, false);
                        t.State(StateA1A, recorder.CreateAction(EnterA1A), recorder.CreateAction(ExitA1A));
                            t.Region(StateA1A1A, false);
                                t.State(StateA1A1A, recorder.CreateAction(EnterA1A1A), recorder.CreateAction(ExitA1A1A));
                                    t.Transition(Transi1, StateA1B, Event, null, null);
                                t.EndState();
                            t.EndRegion();
                            t.Region(StateA1A2A, false);
                                t.State(StateA1A2A, recorder.CreateAction(EnterA1A2A), recorder.CreateAction(ExitA1A2A));
                                    t.Transition(Transi2, StateA1A2B, Event, null, null);
                                t.EndState();
                                t.State(StateA1A2B, recorder.CreateAction(EnterA1A2B), recorder.CreateAction(ExitA1A2B));
                                t.EndState();
                            t.EndRegion();
                        t.EndState();
                        t.State(StateA1B, recorder.CreateAction(EnterA1B), recorder.CreateAction(ExitA1B));
                        t.EndState();
                    t.EndRegion();
                    t.Region(StateA2A, false);
                        t.State(StateA2A, recorder.CreateAction(EnterA2A), recorder.CreateAction(ExitA2A));
                            t.Transition(Transi3, StateA2B, Event, null, null);
                        t.EndState();
                        t.State(StateA2B, recorder.CreateAction(EnterA2B), recorder.CreateAction(ExitA2B));
                        t.EndState();
                    t.EndRegion();
                t.EndState();
            t.EndRegion();
            //## End StateMachineTemplate18

            Assert.That(recorder.RecordedActions, Is.EqualTo(new String[] {}), "Precondition not met: Actions were executed during state machine creation.");

            StateMachine stateMachine = t.CreateStateMachine(this);

            foreach (TestData testData in new TestData[]
                                          {
                                                new TestData()
                                                {
                                                    EventToSend = Startup,
                                                    ExpectedState = t.CreateStateConfiguration(new String[] { StateA1A1A, StateA1A2A, StateA2A }),
                                                    ExpectedActions = new String[] { EnterA, EnterA1A, EnterA1A1A, EnterA1A2A, EnterA2A },
                                                },

                                                new TestData()
                                                {
                                                    EventToSend = Event,
                                                    ExpectedState = t.CreateStateConfiguration(new String[] { StateA1B, StateA2B }),
                                                    ExpectedActions = new String[] { ExitA1A2A, ExitA1A1A, ExitA1A, EnterA1B, ExitA2A, EnterA2B },
                                                },

                                                new TestData()
                                                {
                                                    EventToSend = Finish,
                                                    ExpectedState = t.CreateStateConfiguration(new String[] { }),
                                                    ExpectedActions = new String[] { ExitA2B, ExitA1B, ExitA },
                                                }
                                          })
            {
                recorder.Clear();

                // Act
                switch (testData.EventToSend)
                {
                    case Startup:
                        stateMachine.Startup();
                        break;
                    case Finish:
                        stateMachine.Finish();
                        break;
                    default:
                        stateMachine.SendTriggerEvent(testData.EventToSend);
                        break;
                }

                // Assert
                Assert.That(stateMachine.ActiveStateConfiguration.ToString(), Is.EqualTo(testData.ExpectedState.ToString()), testData.EventToSend + ": Active state not as expected.");
                Assert.That(recorder.RecordedActions, Is.EqualTo(testData.ExpectedActions), testData.EventToSend + ": Unexpected entry and exit actions during state machine processing.");
            }
        }



        [Test]
        public void SendTriggerEvent_WithOrthogonalRegionsGuardState_ExecutesActionsInExpectedSequence()
        {
            // Arrange
            const string Startup = "*Startup*";
            const string Finish = "*Finish*";

            //## Begin StateAndTransitionNames27
            // Generated from <file:S:\StaMa_State_Machine_Controller_Library\StaMaShapesMaster.vst> page "UT_Execution3_OrthogonalRegionsGuardState"
            // at 07-22-2015 22:05:18 using StaMaShapes Version 2300
            const string StateA = "StateA";
            const string Transi3 = "Transi3";
            const string StateA1A = "StateA1A";
            const string Transi2 = "Transi2";
            const string StateA1B = "StateA1B";
            const string StateA2A = "StateA2A";
            const string Transi1 = "Transi1";
            const string StateA2B = "StateA2B";
            const string StateA2C = "StateA2C";
            //## End StateAndTransitionNames27

            //## Begin StateAndTransitionNames26
            // Generated from <file:S:\StaMa_State_Machine_Controller_Library\StaMaShapesMaster.vst> page "UT_Execution3_OrthogonalRegionsGuardState"
            // at 07-22-2015 22:05:17 using StaMaShapes Version 2300
            const string Event3 = "Event3";
            const string Event2 = "Event2";
            const string Event1 = "Event1";
            //## End StateAndTransitionNames26

            //## Begin StateAndTransitionNames29
            // Generated from <file:S:\StaMa_State_Machine_Controller_Library\StaMaShapesMaster.vst> page "UT_Execution3_OrthogonalRegionsGuardState"
            // at 07-22-2015 22:05:18 using StaMaShapes Version 2300
            const string EnterA = "EnterA";
            const string ExitA = "ExitA";
            const string EnterA1A = "EnterA1A";
            const string ExitA1A = "ExitA1A";
            const string EnterA1B = "EnterA1B";
            const string ExitA1B = "ExitA1B";
            const string EnterA2A = "EnterA2A";
            const string ExitA2A = "ExitA2A";
            const string EnterA2B = "EnterA2B";
            const string ExitA2B = "ExitA2B";
            const string EnterA2C = "EnterA2C";
            const string ExitA2C = "ExitA2C";
            //## End StateAndTransitionNames29

            ActionRecorder recorder = new ActionRecorder();

            StateMachineTemplate t = new StateMachineTemplate(StateMachineOptions.UseDoActions);

            //## Begin StateMachineTemplate28
            // Generated from <file:S:\StaMa_State_Machine_Controller_Library\StaMaShapesMaster.vst> page "UT_Execution3_OrthogonalRegionsGuardState"
            // at 07-22-2015 22:05:18 using StaMaShapes Version 2300
            t.Region(StateA, false);
                t.State(StateA, recorder.CreateAction(EnterA), recorder.CreateAction(ExitA));
                    t.Transition(Transi3, new string[] {StateA1B, StateA2B}, StateA2C, Event3, null, null);
                    t.Region(StateA1A, false);
                        t.State(StateA1A, recorder.CreateAction(EnterA1A), recorder.CreateAction(ExitA1A));
                            t.Transition(Transi2, new string[] {StateA1A, StateA2B}, StateA1B, Event2, null, null);
                        t.EndState();
                        t.State(StateA1B, recorder.CreateAction(EnterA1B), recorder.CreateAction(ExitA1B));
                        t.EndState();
                    t.EndRegion();
                    t.Region(StateA2A, false);
                        t.State(StateA2A, recorder.CreateAction(EnterA2A), recorder.CreateAction(ExitA2A));
                            t.Transition(Transi1, StateA2B, Event1, null, null);
                        t.EndState();
                        t.State(StateA2B, recorder.CreateAction(EnterA2B), recorder.CreateAction(ExitA2B));
                        t.EndState();
                        t.State(StateA2C, recorder.CreateAction(EnterA2C), recorder.CreateAction(ExitA2C));
                        t.EndState();
                    t.EndRegion();
                t.EndState();
            t.EndRegion();
            //## End StateMachineTemplate28

            Assert.That(recorder.RecordedActions, Is.EqualTo(new String[] {}), "Precondition not met: Actions were executed during state machine creation.");

            StateMachine stateMachine = t.CreateStateMachine(this);

            foreach (TestData testData in new TestData[]
                                          {
                                                new TestData()
                                                {
                                                    EventToSend = Startup,
                                                    ExpectedState = t.CreateStateConfiguration(new String[] { StateA1A, StateA2A }),
                                                    ExpectedActions = new String[] { EnterA, EnterA1A, EnterA2A },
                                                },

                                                new TestData()
                                                {
                                                    EventToSend = Event2,
                                                    ExpectedState = t.CreateStateConfiguration(new string[] { StateA1A, StateA2A }),
                                                    ExpectedActions = new String[] { }
                                                },

                                                new TestData()
                                                {
                                                    EventToSend = Event1,
                                                    ExpectedState = t.CreateStateConfiguration(new string[] { StateA1A, StateA2B }),
                                                    ExpectedActions = new String[] { ExitA2A, EnterA2B }
                                                },

                                                new TestData()
                                                {
                                                    EventToSend = Event2,
                                                    ExpectedState = t.CreateStateConfiguration(new string[] { StateA1B, StateA2B }),
                                                    ExpectedActions = new String[] { ExitA1A, EnterA1B }
                                                },

                                                new TestData()
                                                {
                                                    EventToSend = Event3,
                                                    ExpectedState = t.CreateStateConfiguration(new string[] { StateA1A, StateA2C }),
                                                    ExpectedActions = new String[] { ExitA2B, ExitA1B, ExitA, EnterA, EnterA1A, EnterA2C }
                                                },

                                                new TestData()
                                                {
                                                    EventToSend = Finish,
                                                    ExpectedState = t.CreateStateConfiguration(new string[] { }),
                                                    ExpectedActions = new String[] { ExitA2C, ExitA1A, ExitA }
                                                },
                                          })
            {
                recorder.Clear();

                // Act
                switch (testData.EventToSend)
                {
                    case Startup:
                        stateMachine.Startup();
                        break;
                    case Finish:
                        stateMachine.Finish();
                        break;
                    default:
                        stateMachine.SendTriggerEvent(testData.EventToSend);
                        break;
                }

                // Assert
                Assert.That(stateMachine.ActiveStateConfiguration.ToString(), Is.EqualTo(testData.ExpectedState.ToString()), testData.EventToSend + ": Active state not as expected.");
                Assert.That(recorder.RecordedActions, Is.EqualTo(testData.ExpectedActions), testData.EventToSend + ": Unexpected entry and exit actions during state machine processing.");
            }
        }


        [Test]
        public void SendTriggerEvent_WithDeepNesting_ExecutesActionsInExpectedSequence()
        {
            bool flag = false;

            // Arrange
            const string Startup = "*Startup*";
            const string Finish = "*Finish*";

            //## Begin StateAndTransitionNames5
            // Generated from <file:S:\StaMa_State_Machine_Controller_Library\StaMaShapesMaster.vst> page "UT_Execution4_DeepNesting"
            // at 07-22-2015 22:05:21 using StaMaShapes Version 2300
            const string State1 = "State1";
            const string Transi1 = "Transi1";
            const string Ev5 = "Ev5";
            const string State2 = "State2";
            const string Transi2 = "Transi2";
            const string Ev1 = "Ev1";
            const string State3 = "State3";
            const string Transi3 = "Transi3";
            const string Ev2 = "Ev2";
            const string State4 = "State4";
            const string State5 = "State5";
            const string State7 = "State7";
            const string State6 = "State6";
            const string Transi4 = "Transi4";
            const string Transi5 = "Transi5";
            const string State8 = "State8";
            const string State9 = "State9";
            const string State10 = "State10";
            const string Transi6 = "Transi6";
            const string Ev4 = "Ev4";
            const string State11 = "State11";
            const string Transi7 = "Transi7";
            const string Ev3 = "Ev3";
            const string Transi8 = "Transi8";
            const string State12 = "State12";
            const string Transi9 = "Transi9";
            const string State13 = "State13";
            //## End StateAndTransitionNames5

            //## Begin StateAndTransitionNames4
            // Generated from <file:S:\StaMa_State_Machine_Controller_Library\StaMaShapesMaster.vst> page "UT_Execution4_DeepNesting"
            // at 07-22-2015 22:05:21 using StaMaShapes Version 2300
            const string TransAct1To13 = "TransAct1To13";
            const string Entry2 = "Entry2";
            const string Exit2 = "Exit2";
            const string TransAct2To6 = "TransAct2To6";
            const string Entry3 = "Entry3";
            const string Exit3 = "Exit3";
            const string TransAct3To10 = "TransAct3To10";
            const string Entry7 = "Entry7";
            const string Exit7 = "Exit7";
            const string Entry6 = "Entry6";
            const string Exit6 = "Exit6";
            const string TransAct10To3 = "TransAct10To3";
            const string Entry11 = "Entry11";
            const string Exit11 = "Exit11";
            const string TransAct11To12 = "TransAct11To12";
            const string Entry12 = "Entry12";
            const string Exit12 = "Exit12";
            //## End StateAndTransitionNames4

            ActionRecorder recorder = new ActionRecorder();

            StateMachineTemplate t = new StateMachineTemplate(StateMachineOptions.UseDoActions);

            //## Begin StateMachineTemplate6
            // Generated from <file:S:\StaMa_State_Machine_Controller_Library\StaMaShapesMaster.vst> page "UT_Execution4_DeepNesting"
            // at 07-22-2015 22:05:21 using StaMaShapes Version 2300
            t.Region(State1, false);
                t.State(State1, null, null);
                    t.Transition(Transi1, State13, Ev5, null, recorder.CreateAction(TransAct1To13));
                    t.Region(State2, false);
                        t.State(State2, recorder.CreateAction(Entry2), recorder.CreateAction(Exit2));
                            t.Transition(Transi2, State6, Ev1, null, recorder.CreateAction(TransAct2To6));
                            t.Region(State3, false);
                                t.State(State3, recorder.CreateAction(Entry3), recorder.CreateAction(Exit3));
                                    t.Transition(Transi3, State10, Ev2, null, recorder.CreateAction(TransAct3To10));
                                    t.Region(State4, false);
                                        t.State(State4, null, null);
                                            t.Region(State5, false);
                                                t.State(State5, null, null);
                                                    t.Region(State7, true);
                                                        t.State(State7, recorder.CreateAction(Entry7), recorder.CreateAction(Exit7));
                                                        t.EndState();
                                                        t.State(State6, recorder.CreateAction(Entry6), recorder.CreateAction(Exit6));
                                                            t.Transition(Transi4, State6, null, (stm, ev, args) => false, null);
                                                            t.Transition(Transi5, State6, Ev2, (stm, ev, args) => false, null);
                                                        t.EndState();
                                                    t.EndRegion();
                                                t.EndState();
                                            t.EndRegion();
                                        t.EndState();
                                    t.EndRegion();
                                t.EndState();
                            t.EndRegion();
                        t.EndState();
                        t.State(State8, null, null);
                            t.Region(State9, false);
                                t.State(State9, null, null);
                                    t.Region(State10, false);
                                        t.State(State10, null, null);
                                            t.Transition(Transi6, State3, Ev4, null, recorder.CreateAction(TransAct10To3));
                                            t.Region(State11, true);
                                                t.State(State11, recorder.CreateAction(Entry11), recorder.CreateAction(Exit11));
                                                    t.Transition(Transi7, State12, Ev3, null, recorder.CreateAction(TransAct11To12));
                                                    t.Transition(Transi8, State12, null, (stm, ev, args) => flag, null);
                                                t.EndState();
                                                t.State(State12, recorder.CreateAction(Entry12), recorder.CreateAction(Exit12));
                                                    t.Transition(Transi9, State11, null, (stm, ev, args) => !flag, (stm, ev, args) => flag = true);
                                                t.EndState();
                                            t.EndRegion();
                                        t.EndState();
                                    t.EndRegion();
                                t.EndState();
                            t.EndRegion();
                        t.EndState();
                    t.EndRegion();
                t.EndState();
                t.State(State13, null, null);
                t.EndState();
            t.EndRegion();
            //## End StateMachineTemplate6

            Assert.That(recorder.RecordedActions, Is.EqualTo(new String[] { }), "Precondition not met: Actions were executed during state machine creation.");

            StateMachine stateMachine = t.CreateStateMachine(this);

            stateMachine.TraceStateChange = delegate(StateMachine stateMachinex, StateConfiguration stateConfigurationFrom, StateConfiguration stateConfigurationTo, Transition transition)
                                            {
                                                System.Console.WriteLine("TestExecDeepNest: Transition from {0} to {1} using {2}",
                                                                        stateConfigurationFrom.ToString(),
                                                                        stateConfigurationTo.ToString(),
                                                                        (transition != null) ? transition.Name : "*");
                                            };

            stateMachine.TraceTestTransition = delegate(StateMachine stateMachinex, Transition transition, object triggerEvent, EventArgs eventArgs)
                                                {
                                                    System.Console.WriteLine("TestExecDeepNest: Test transition {0} with event {1} in state {2}",
                                                                                transition.ToString(),
                                                                                (triggerEvent != null) ? triggerEvent.ToString() : "*",
                                                                                stateMachine.ActiveStateConfiguration.ToString());
                                                };

            stateMachine.TraceDispatchTriggerEvent = delegate(StateMachine stateMachinex, object triggerEvent, EventArgs eventArgs)
                                                    {
                                                        string eventName = (triggerEvent != null) ? triggerEvent.ToString() : "*";
                                                        System.Console.WriteLine("TestExecDeepNest: Dispatch event {0} in state {1}", eventName, stateMachine.ActiveStateConfiguration.ToString());
                                                    };

            foreach (TestData testData in new TestData[]
                                          {
                                                new TestData()
                                                {
                                                    EventToSend = Startup,
                                                    ExpectedState = t.CreateStateConfiguration(new String[] { State7 }),
                                                    ExpectedActions = new String[] { Entry2, Entry3, Entry7 },
                                                },

                                                new TestData()
                                                {
                                                    EventToSend = Ev1,
                                                    ExpectedState = t.CreateStateConfiguration(new string[] { State6 }),
                                                    ExpectedActions = new String[] { Exit7, Exit3, Exit2, TransAct2To6, Entry2, Entry3, Entry6 }
                                                },

                                                new TestData()
                                                {
                                                    EventToSend = Ev2,
                                                    ExpectedState = t.CreateStateConfiguration(new string[] { State11 }),
                                                    ExpectedActions = new String[] { Exit6, Exit3, Exit2, TransAct3To10, Entry11 }
                                                },

                                                new TestData()
                                                {
                                                    EventToSend = Ev3,
                                                    ExpectedState = t.CreateStateConfiguration(new string[] { State12 }),
                                                    ExpectedActions = new String[] { Exit11, TransAct11To12, Entry12, Exit12, Entry11, Exit11, Entry12 }
                                                },

                                                new TestData()
                                                {
                                                    EventToSend = Ev4,
                                                    ExpectedState = t.CreateStateConfiguration(new string[] { State6 }),
                                                    ExpectedActions = new String[] { Exit12, TransAct10To3, Entry2, Entry3, Entry6 }
                                                },

                                                new TestData()
                                                {
                                                    EventToSend = Ev5,
                                                    ExpectedState = t.CreateStateConfiguration(new string[] { State13 }),
                                                    ExpectedActions = new String[] { Exit6, Exit3, Exit2, TransAct1To13 }
                                                },

                                                new TestData()
                                                {
                                                    EventToSend = Finish,
                                                    ExpectedState = t.CreateStateConfiguration(new string[] { }),
                                                    ExpectedActions = new String[] { }
                                                },
                                          })
            {
                recorder.Clear();

                // Act
                switch (testData.EventToSend)
                {
                    case Startup:
                        stateMachine.Startup();
                        break;
                    case Finish:
                        stateMachine.Finish();
                        break;
                    default:
                        stateMachine.SendTriggerEvent(testData.EventToSend);
                        break;
                }

                // Assert
                Assert.That(stateMachine.ActiveStateConfiguration.ToString(), Is.EqualTo(testData.ExpectedState.ToString()), testData.EventToSend + ": Active state not as expected.");
                Assert.That(recorder.RecordedActions, Is.EqualTo(testData.ExpectedActions), testData.EventToSend + ": Unexpected entry and exit actions during state machine processing.");
            }
        }


        private class TestData
        {
            public String EventToSend { get; set; }
            public StateConfiguration ExpectedState { get; set; }
            public String[] ExpectedActions { get; set; }
        }
    }
}
