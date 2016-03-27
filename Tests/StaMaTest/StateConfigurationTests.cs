#region StateConfigurationTests.cs file
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
    public class StateConfigurationTests
    {
        const string StateA = "StateA";
        const string StateB = "StateB";
        const string StateA1A = "StateA1A";
        const string StateA1B = "StateA1B";
        const string StateA1A1A = "StateA1A1A";
        const string StateA1A1B = "StateA1A1B";
        const string StateA1A2A = "StateA1A2A";

        private static StateMachineTemplate m_stmTmplat;

        static StateConfigurationTests()
        {
            StateMachineTemplate t = new StateMachineTemplate();

            //## Begin TestCreateStateConfiguration
            // Generated from <file:S:\StaMa_State_Machine_Controller_Library\StaMaShapesMaster.vst> page "UT_StateConfigurationTests"
            // at 07-22-2015 22:05:06 using StaMaShapes Version 2300
            t.Region(StateA, false);
                t.State(StateA, null, null);
                    t.Region(StateA1A, false);
                        t.State(StateA1A, null, null);
                            t.Region(StateA1A1A, false);
                                t.State(StateA1A1A, null, null);
                                t.EndState();
                                t.State(StateA1A1B, null, null);
                                t.EndState();
                            t.EndRegion();
                            t.Region(StateA1A2A, false);
                                t.State(StateA1A2A, null, null);
                                t.EndState();
                            t.EndRegion();
                        t.EndState();
                        t.State(StateA1B, null, null);
                        t.EndState();
                    t.EndRegion();
                t.EndState();
                t.State(StateB, null, null);
                t.EndState();
            t.EndRegion();
            //## End TestCreateStateConfiguration
            
            m_stmTmplat = t;
        }


        [Test]
        public void CreateStateConfiguration_SingleState_Successful()
        {
            // Act
            StateConfiguration s = m_stmTmplat.CreateStateConfiguration(StateB);

            // Assert
            Assert.That(s, Is.Not.Null);
            Assert.That(s, Is.Not.EqualTo(m_stmTmplat.CreateStateConfiguration(new string[] { StateA })).Using(StateConfigurationIsMatching.Instance));
            Assert.That(s.ToString(), Is.EqualTo("StateB"));
        }


        [Test]
        public void CreateStateConfiguration_Unspecified_Successful()
        {
            // Act
            StateConfiguration s = m_stmTmplat.CreateStateConfiguration(new string[] { });

            // Assert
            Assert.That(s, Is.Not.Null);
            Assert.That(s, Is.EqualTo(m_stmTmplat.CreateStateConfiguration(new string[] { StateA })).Using(StateConfigurationIsMatching.Instance));
            Assert.That(s, Is.EqualTo(m_stmTmplat.CreateStateConfiguration(new string[] { StateB })).Using(StateConfigurationIsMatching.Instance));
            Assert.That(s.ToString(), Is.EqualTo("*"));
        }


        [Test]
        public void CreateStateConfiguration_OrthogonalRegions_Successful()
        {
            // Act
            StateConfiguration s = m_stmTmplat.CreateStateConfiguration(new string[] { StateA1A1A, StateA1A2A });

            // Assert
            Assert.That(s, Is.Not.Null);
            Assert.That(s.ToString(), Is.EqualTo("StateA(StateA1A(StateA1A1A,StateA1A2A))"));
        }


        [Test]
        public void CreateStateConfiguration_PartialUnspecifiedOrthogonalRegions_Successful()
        {
            // Act
            StateConfiguration s = m_stmTmplat.CreateStateConfiguration(new string[] { StateA1A2A });

            // Assert
            Assert.That(s, Is.Not.Null);
            Assert.That(s, Is.EqualTo(m_stmTmplat.CreateStateConfiguration(new string[] { StateA1A1A, StateA1A2A })).Using(StateConfigurationIsMatching.Instance));
            Assert.That(s, Is.EqualTo(m_stmTmplat.CreateStateConfiguration(new string[] { StateA1A1B, StateA1A2A })).Using(StateConfigurationIsMatching.Instance));
            Assert.That(s, Is.EqualTo(m_stmTmplat.CreateStateConfiguration(new string[] { StateA })).Using(StateConfigurationIsMatching.Instance));
            Assert.That(s, Is.EqualTo(m_stmTmplat.CreateStateConfiguration(new string[] { StateA1A })).Using(StateConfigurationIsMatching.Instance));
            Assert.That(s, Is.Not.EqualTo(m_stmTmplat.CreateStateConfiguration(new string[] { StateB })).Using(StateConfigurationIsMatching.Instance));
            Assert.That(s, Is.Not.EqualTo(m_stmTmplat.CreateStateConfiguration(new string[] { StateA1B })).Using(StateConfigurationIsMatching.Instance));
            Assert.That(s.ToString(), Is.EqualTo("StateA(StateA1A(*,StateA1A2A))"));
        }


        [Test]
        public void CreateStateConfiguration_IncompatibleSameRegion_Throws()
        {
            Assert.That(delegate() { m_stmTmplat.CreateStateConfiguration(new string[] { StateA1A1A, StateA1B }); }, Throws.TypeOf(typeof(ArgumentOutOfRangeException)));
        }


        [Test]
        public void CreateStateConfiguration_Unknown_Throws()
        {
            Assert.That(delegate() { m_stmTmplat.CreateStateConfiguration(new string[] { "wrztlbrnft" }); }, Throws.TypeOf(typeof(ArgumentOutOfRangeException)));
        }


        [Test]
        public void CreateStateConfiguration_NullStateList_Throws()
        {
            Assert.That(delegate() { m_stmTmplat.CreateStateConfiguration((string[])null); }, Throws.TypeOf(typeof(ArgumentNullException)));
        }


        [Test]
        public void CreateStateConfiguration_NullSingleState_Throws()
        {
            Assert.That(delegate() { m_stmTmplat.CreateStateConfiguration((string)null); }, Throws.TypeOf(typeof(ArgumentNullException)));
        }
    }
}
