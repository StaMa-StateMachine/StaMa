#region TestSyntaxCheck.cs file
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
    public class SyntaxCheckTests
    {
        const string Transi1    = "Transi1";
        const string Transi2    = "Transi2";
        const string Transi3    = "Transi3";

        const string State1 = "State1";
        const string State2 = "State2";
        const string State1a1 = "State1a1";
        const string State1a2 = "State1a2";
        const string State1a2a1 = "State1a2a1";
        const string State1a2a2 = "State1a2a2";
        const string State2a1 = "State2a1";
        const string State2a2 = "State2a2";
        const string State1b1 = "State1b1";
        const string State1b2 = "State1b2";
        const string State1a1a1 = "State1a1a1";
        const string State1a1a2 = "State1a1a2";
        const string State2a1a1 = "State2a1a1";
        const string State2a1a2 = "State2a1a2";


        private struct TestData
        {
            public TestData(string stateName, bool expectedIsValid)
            {
                StateName = stateName;
                ExpectedIsValid = expectedIsValid;
            }
            public string StateName;
            public bool ExpectedIsValid;
        }


        [Test]
        public void IsValidIdentifier_WithSuspectStateNames_ReturnsExpected()
        {
            TestData[] testDataArray = new TestData[]
            {
                new TestData("", false),
                new TestData("&", false),
                new TestData("6", true),
                new TestData("_", true),
                new TestData("__", true),
                new TestData("__6", true),
                new TestData("__b", true),
                new TestData("B_", true),
                new TestData("6B", false),
                new TestData("B6", true),
                new TestData("__X", true),
                new TestData("X", true),
                new TestData("X\nx", false),
            };
            
            foreach (TestData testData in testDataArray)
            {
                bool isValid = StateMachineTemplate.IsValidIdentifier(testData.StateName);
                Assert.That(isValid, Is.EqualTo(testData.ExpectedIsValid), "Test string \"" + testData.StateName + "\" didn't match expectation.");
            }
        }

        [Test]
        public void IsValidIdentifier_WithNull_Throws()
        {
            Assert.That(() => { StateMachineTemplate.IsValidIdentifier(null); }, Throws.TypeOf(typeof(ArgumentNullException)));
        }

        [Test]
        public void StateMachineTemplate_WithValidStructure_IsInitialized()
        {
            // Set up a typical tree without problems
            StateMachineTemplate t = new StateMachineTemplate();
            t.Region(State1, false);
                t.State(State1);
                    t.Region(State1a1, false);
                        t.State(State1a1);
                        t.EndState();
                        t.State(State1a2);
                            t.Region(State1a2a1, false);
                                t.State(State1a2a1);
                                    t.Transition(Transi3, new string[] { State1a2a1 }, new string[] { State1a2a2 }, 1, null, null);
                                t.EndState();
                                t.State(State1a2a2);
                                t.EndState();
                            t.EndRegion();
                        t.EndState();
                    t.EndRegion();
                    t.Region(State1b1, false);
                        t.State(State1b1);
                        t.EndState();
                    t.EndRegion();
                t.EndState();
                t.State(State2);
                t.EndState();
            t.EndRegion();

            Assert.That(t.Root, Is.Not.Null, "State machine template not complete.");
            Assert.That(t.StateConfigurationMax, Is.EqualTo(4), "StateConfigurationMax wrong");
            Assert.That(t.ConcurrencyDegree, Is.EqualTo(2), "ConcurrencyDegree wrong");
            Assert.That(t.HistoryMax, Is.EqualTo(0), "HistoryMax wrong");
        }

        [Test]
        public void StateMachineTemplate_WithValidStructure2_IsInitialized()
        {
            // Set up a typical tree without problems
            StateMachineTemplate t = new StateMachineTemplate();
            t.Region(State1, false);
                t.State(State1);
                    t.Transition(Transi1, new string[] { State1 }, new string[] { State2 }, 1, null, null);
                    t.Region(State1a1, true);
                        t.State(State1a1);
                            t.Transition(Transi3, new string[] { State1a1 }, new string[] { State1a2 }, 1, null, null);
                        t.EndState();
                        t.State(State1a2);
                        t.EndState();
                    t.EndRegion();
                t.EndState();
                t.State(State2);
                    t.Transition(Transi2, new string[] { State2 }, new string[] { State1 }, 1, null, null);
                t.EndState();
            t.EndRegion();

            Assert.That(t.Root, Is.Not.Null, "State machine template not complete.");
            Assert.That(t.StateConfigurationMax, Is.EqualTo(2), "StateConfigurationMax wrong");
            Assert.That(t.ConcurrencyDegree, Is.EqualTo(1), "ConcurrencyDegree wrong");
            Assert.That(t.HistoryMax, Is.EqualTo(1), "HistoryMax wrong");
        }


        [Test]
        public void StateMachineTemplate_WithOpenRegion_IsNotInitialized()
        {
            StateMachineTemplate t = new StateMachineTemplate();

            t.Region(State1, false);

            Assert.That(t.Root, Is.Null, "Unexpected StateMachineTemplate initialization.");
        }


        [Test]
        public void StateMachineTemplate_WithOpenRegionAndStates_IsNotInitialized()
        {
            StateMachineTemplate t = new StateMachineTemplate();

            t.Region(State1, false);
            t.State(State1);
            t.EndState();

            Assert.That(t.Root, Is.Null, "Unexpected StateMachineTemplate initialization.");
        }


        [Test]
        public void StateMachineTemplate_WithMissingEndState_Throws()
        {
            StateMachineTemplate t = new StateMachineTemplate();

            t.Region(State1, false);
                t.State(State1);
                // t.EndState() omitted.
            Assert.That(() => { t.State(State2); }, Throws.TypeOf(typeof(StateMachineException)));
        }

        
        [Test]
        public void StateMachineTemplate_WithMissingEndRegion_Throws()
        {
            StateMachineTemplate t = new StateMachineTemplate();

            t.Region(State1, false);
                t.State(State1);
                    t.Region(State1a1, false);
                        t.State(State1a1);
                        t.EndState();
                 // t.EndRegion() omitted.
            Assert.That(() => { t.Region(State1b1, false); }, Throws.TypeOf(typeof(StateMachineException)));
        }

        
        [Test]
        public void StateMachineTemplate_WithMissingCompositeState_Throws()
        {
            StateMachineTemplate t = new StateMachineTemplate();

            t.Region(State1, false);
             // t.State(State1) omitted.
            Assert.That(() => { t.Region(State2, false); }, Throws.TypeOf(typeof(StateMachineException)));
        }

        
        [Test]
        public void StateMachineTemplate_EndRegionWithoutRegion_Throws()
        {
            StateMachineTemplate t = new StateMachineTemplate();

            Assert.That(() => { t.EndRegion(); }, Throws.TypeOf(typeof(StateMachineException)));
        }

        
        [Test]
        public void StateMachineTemplate_NestedEndRegionWithoutRegion_Throws()
        {
            StateMachineTemplate t = new StateMachineTemplate();

            t.Region(State1, false);
                t.State(State1);
                // t.EndState() omitted.
            Assert.That(() => { t.EndRegion(); }, Throws.TypeOf(typeof(StateMachineException)));
        }


        [Test]
        public void StateMachineTemplate_WithConflictingTransitionTargetStateDefinition_Throws()
        {
            StateMachineTemplate t = new StateMachineTemplate();

            Exception thrownException = null;
            try
            {
                //## Begin BadTransiTargetsTest1
                // Generated from <file:S:\StaMa_State_Machine_Controller_Library\StaMaShapesMaster.vst> page "UT_SyntaxCheckTests"
                // at 07-22-2015 22:05:03 using StaMaShapes Version 2300
                t.Region(State1, false);
                    t.State(State1, null, null);
                        t.Region(State1a1, false);
                            t.State(State1a1, null, null);
                                t.Region(State1a1a1, false);
                                    t.State(State1a1a1, null, null);
                                    t.EndState();
                                    t.State(State1a1a2, null, null);
                                    t.EndState();
                                t.EndRegion();
                            t.EndState();
                            t.State(State1a2, null, null);
                            t.EndState();
                        t.EndRegion();
                    t.EndState();
                    t.State(State2, null, null);
                        t.Transition(Transi1, new string[] {State1a1, State1a2}, 1, null, null);
                    t.EndState();
                t.EndRegion();
                //## End BadTransiTargetsTest1
            }
            catch (Exception ex)
            {
                thrownException = ex;
            }
            Assert.That(thrownException, Is.Not.Null);
            Assert.That(thrownException.GetType(), Is.EqualTo(typeof(StateMachineException)));
        }


        [Test]
        public void StateMachineTemplate_WithTransitionSourceNotContainedInTransitionAnchor_Throws()
        {
            StateMachineTemplate t = new StateMachineTemplate();

            Exception thrownException = null;
            try
            {
                //## Begin BadTransiTargetsTest2
                // Generated from <file:S:\StaMa_State_Machine_Controller_Library\StaMaShapesMaster.vst> page "UT_SyntaxCheckTests"
                // at 07-22-2015 22:05:03 using StaMaShapes Version 2300
                t.Region(State1, false);
                    t.State(State1, null, null);
                        t.Region(State1a1, false);
                            t.State(State1a1, null, null);
                                t.Transition(Transi1, State1, State2, 1, null, null);
                                t.Region(State1a1a1, false);
                                    t.State(State1a1a1, null, null);
                                    t.EndState();
                                    t.State(State1a1a2, null, null);
                                    t.EndState();
                                t.EndRegion();
                            t.EndState();
                            t.State(State1a2, null, null);
                            t.EndState();
                        t.EndRegion();
                    t.EndState();
                    t.State(State2, null, null);
                    t.EndState();
                t.EndRegion();
                //## End BadTransiTargetsTest2
            }
            catch (Exception ex)
            {
                thrownException = ex;
            }
            Assert.That(thrownException, Is.Not.Null);
            Assert.That(thrownException.GetType(), Is.EqualTo(typeof(StateMachineException)));
        }


        [Test]
        public void StateMachineTemplate_WithTransitionSourceNotContainedInTransitionAnchor2_Throws()
        {
            StateMachineTemplate t = new StateMachineTemplate();

            Exception thrownException = null;
            try
            {
                //## Begin BadTransiTargetsTest3
                // Generated from <file:S:\StaMa_State_Machine_Controller_Library\StaMaShapesMaster.vst> page "UT_SyntaxCheckTests"
                // at 07-22-2015 22:05:03 using StaMaShapes Version 2300
                t.Region(State1, false);
                    t.State(State1, null, null);
                        t.Transition(Transi1, State2, State1, 1, null, null);
                        t.Region(State1a1, false);
                            t.State(State1a1, null, null);
                                t.Region(State1a1a1, false);
                                    t.State(State1a1a1, null, null);
                                    t.EndState();
                                    t.State(State1a1a2, null, null);
                                    t.EndState();
                                t.EndRegion();
                            t.EndState();
                            t.State(State1a2, null, null);
                            t.EndState();
                        t.EndRegion();
                    t.EndState();
                    t.State(State2, null, null);
                    t.EndState();
                t.EndRegion();
                //## End BadTransiTargetsTest3
            }
            catch (Exception ex)
            {
                thrownException = ex;
            }
            Assert.That(thrownException, Is.Not.Null);
            Assert.That(thrownException.GetType(), Is.EqualTo(typeof(StateMachineException)));
        }

        
        [Test]
        public void StateMachineTemplate_WithTransitionSourceNotContainedInTransitionAnchor3_Throws()
        {
            StateMachineTemplate t = new StateMachineTemplate();

            Exception thrownException = null;
            try
            {
                //## Begin BadTransiTargetsTest4
                // Generated from <file:S:\StaMa_State_Machine_Controller_Library\StaMaShapesMaster.vst> page "UT_SyntaxCheckTests"
                // at 07-22-2015 22:05:03 using StaMaShapes Version 2300
                t.Region(State1, false);
                    t.State(State1, null, null);
                        t.Transition(Transi1, State2a1a1, State1, 1, null, null);
                        t.Region(State1a1, false);
                            t.State(State1a1, null, null);
                                t.Region(State1a1a1, false);
                                    t.State(State1a1a1, null, null);
                                    t.EndState();
                                    t.State(State1a1a2, null, null);
                                    t.EndState();
                                t.EndRegion();
                            t.EndState();
                            t.State(State1a2, null, null);
                            t.EndState();
                        t.EndRegion();
                    t.EndState();
                    t.State(State2, null, null);
                        t.Region(State2a1, false);
                            t.State(State2a1, null, null);
                                t.Region(State2a1a1, false);
                                    t.State(State2a1a1, null, null);
                                    t.EndState();
                                    t.State(State2a1a2, null, null);
                                    t.EndState();
                                t.EndRegion();
                            t.EndState();
                            t.State(State2a2, null, null);
                            t.EndState();
                        t.EndRegion();
                    t.EndState();
                t.EndRegion();
                //## End BadTransiTargetsTest4
            }
            catch (Exception ex)
            {
                thrownException = ex;
            }
            Assert.That(thrownException, Is.Not.Null);
            Assert.That(thrownException.GetType(), Is.EqualTo(typeof(StateMachineException)));
        }


        [Test]
        public void StateMachineTemplate_WithDuplicateStateName_Throws()
        {
            StateMachineTemplate t = new StateMachineTemplate();

            t.Region(State1, false);
                t.State(State1);
                t.EndState();
                t.State(State2);
                    t.Region(State2a1, false);
                        t.State(State2a1);
                        t.EndState();
                        Assert.That(() => { t.State(State1); }, Throws.TypeOf(typeof(ArgumentOutOfRangeException)));
        }


        [Test]
        public void StateMachineTemplate_WithDuplicateTransitionName_Throws()
        {
            StateMachineTemplate t = new StateMachineTemplate();

            t.Region(State1, false);
                t.State(State1);
                    t.Transition(Transi1, new string[] { State1 }, new string[] { State2 }, 1, null, null);
                    Assert.That(() => { t.Transition(Transi1, new string[] { State1 }, new string[] { State2 }, 1, null, null); }, Throws.TypeOf(typeof(ArgumentOutOfRangeException)));
        }
    }
}
