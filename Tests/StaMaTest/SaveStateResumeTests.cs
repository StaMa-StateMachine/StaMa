#region ExecutionTests.cs file
//
// Tests for StaMa state machine controller library
//
// Copyright (c) 2005-2015, Roland Schneider. All rights reserved.
//
#endregion

using System;
using System.IO;
using StaMa;

using System.Collections;
using System.Text;
#if !MF_FRAMEWORK
using NUnit.Framework;
#else
using MFUnitTest.Framework;
using Microsoft.SPOT;
#endif


namespace StaMaTest
{
    [TestFixture]
    public class SaveStateResumeTests
    {
#if !MF_FRAMEWORK
        
        [TestCase(false, TestName = "SaveStateResume_HappyPathNoExecuteEntryActions_BehavesAsExpected")]
        [TestCase(true, TestName = "SaveStateResume_HappyPathWithExecuteEntryActions_BehavesAsExpected")]
        public void SaveStateResume_HappyPath_BehavesAsExpected(bool executeEntryActions)
#else
        public void SaveStateResume_HappyPathNoExecuteEntryActions_BehavesAsExpected() { SaveStateResume_HappyPath_BehavesAsExpected(false); }
        public void SaveStateResume_HappyPathWithExecuteEntryActions_BehavesAsExpected() { SaveStateResume_HappyPath_BehavesAsExpected(true); }
        private void SaveStateResume_HappyPath_BehavesAsExpected(bool executeEntryActions)
#endif
        {
            // Arrange
            ActionRecorder recorder = new ActionRecorder();

            StateMachineTemplate t = new StateMachineTemplate(StateMachineOptions.UseDoActions);

            t.Region("StateA", false);
                t.State("StateA", recorder.CreateAction("EnterA"), recorder.CreateAction("ExitA"), recorder.CreateDoAction("DoA"));
                    t.Transition("T1", "StateB", null);
                t.EndState();
                t.State("StateB", recorder.CreateAction("EnterB"), recorder.CreateAction("ExitB"), recorder.CreateDoAction("DoB"));
                t.EndState();
            t.EndRegion();

            StateMachine s1 = t.CreateStateMachine();
            s1.Startup();
            recorder.Clear();

            // Act
            MemoryStream memoryStream = new MemoryStream();
            s1.SaveState(memoryStream);
            memoryStream.Flush();
            memoryStream.Position = 0;
            StateMachine s2 = t.CreateStateMachine();
            s2.Resume(memoryStream, executeEntryActions);

            // Assert
            Assert.That(s2.ActiveStateConfiguration.ToString(), Is.EqualTo(s1.ActiveStateConfiguration.ToString()), "State mismatch after Resume.");
            String[] expectedActions = executeEntryActions ? new String[] { "EnterA", "DoA" } : new String[] { };
            Assert.That(recorder.RecordedActions, Is.EqualTo(expectedActions), "Unexpected entry or do actions during state machine resume.");
            recorder.Clear();
            s2.SendTriggerEvent(null);
            Assert.That(s2.ActiveStateConfiguration.ToString(), Is.EqualTo(t.CreateStateConfiguration("StateB").ToString()), "Cannot run state machine after Resume.");
            Assert.That(recorder.RecordedActions, Is.EqualTo(new String[] { "ExitA", "EnterB", "DoB" }), "Unexpected entry or do actions during state machine resume.");
        }


        [Test]
        public void SaveStateResume_SampleCode_BehavesAsExpected()
        {
#region DevelopersGuide_SaveStateAndResume
            StateMachineTemplate t = new StateMachineTemplate();
            t.Region("State1", false);
                t.State("State1");
                    t.Transition("T1", "State2", "Event1");
                t.EndState();
                t.State("State2");
                    t.Transition("T2", "State1", "Event2");
                t.EndState();
            t.EndRegion();

            // Create a state machine and start it
            StateMachine stateMachine1 = t.CreateStateMachine();
            stateMachine1.Startup();

            // stateMachine1 is now ready for receiving events
            stateMachine1.SendTriggerEvent("Event1");
            // Check that stateMachine1 has executed transition "T1" to state "State2"
            if (stateMachine1.ActiveStateConfiguration.ToString() != t.CreateStateConfiguration("State2").ToString())
            {
                throw new Exception("stateMachine1 in unexpected state");
            }

            // Save the current state of stateMachine1
            MemoryStream stream = new MemoryStream();
            stateMachine1.SaveState(stream);
            stream.Flush(); // Might write to persistent storage when using FileStream instead

            // For demonstration purposes reset the MemoryStream to enable reading from it
            // Real applications would open a FileStream
            stream.Position = 0;

            // Create a new state machine using the same structure and resume from saved state
            StateMachine stateMachine2 = t.CreateStateMachine();
            stateMachine2.Resume(stream, false);

            // stateMachine2 is ready for receiving events
            stateMachine2.SendTriggerEvent("Event2");
            // Check that stateMachine2 has executed transition "T2" to state "State1"
            if (stateMachine2.ActiveStateConfiguration.ToString() != t.CreateStateConfiguration("State1").ToString())
            {
                throw new Exception("stateMachine2 in unexpected state");
            }
#endregion DevelopersGuide_SaveStateAndResume
        }

        
        [Test]
        public void SaveStateResume_StructureChanged_ThrowsAndStateMachineNotCorrupt()
        {
            // Arrange
            StateMachineTemplate t = new StateMachineTemplate();
            t.Region("State1", false);
                t.State("State1");
                    t.Transition("T1", "State2", null);
                t.EndState();
                t.State("State2");
                t.EndState();
            t.EndRegion();
            StateMachine s1 = t.CreateStateMachine();
            s1.Startup();

            StateMachineTemplate t2 = new StateMachineTemplate();
            t2.Region("State1", false);
                t2.State("State1");
                    t2.Transition("T1", "State3", null);
                t2.EndState();
                t2.State("State3");
                t2.EndState();
            t2.EndRegion();
            StateMachine s2 = t2.CreateStateMachine();

            MemoryStream memoryStream = new MemoryStream();
            s1.SaveState(memoryStream);
            memoryStream.Flush();
            memoryStream.Position = 0;

            // Act, Assert
            Assert.That(new TestDelegate(delegate() { s2.Resume(memoryStream, false); }), Throws.TypeOf(typeof(ArgumentException)).With.Message.ContainsSubstring("state machine structure").IgnoreCase);

            // Act
            s2.Startup();
            s2.SendTriggerEvent(null);
            Assert.That(s2.ActiveStateConfiguration.ToString(), Is.EqualTo(t2.CreateStateConfiguration("State3").ToString()), "Cannot run state machine after failed Resume.");
        }


        delegate StateMachineTemplate TemplateFactory();

#if !MF_FRAMEWORK
        [TestCase(false, TestName = "SaveStateResume_WithHistoryNoExecuteEntryActions_UsesHistoryStateAfterResume")]  
        [TestCase(true, TestName = "SaveStateResume_WithHistoryExecuteEntryActions_UsesHistoryStateAfterResume")] 
        public void SaveStateResume_WithHistory_UsesHistoryStateAfterResume(bool executeEntryActions)
#else
        public void SaveStateResume_WithHistoryNoExecuteEntryActions_UsesHistoryStateAfterResume() { SaveStateResume_WithHistory_UsesHistoryStateAfterResume(false); }
        public void SaveStateResume_WithHistoryExecuteEntryActions_UsesHistoryStateAfterResume() { SaveStateResume_WithHistory_UsesHistoryStateAfterResume(true); }
        private void SaveStateResume_WithHistory_UsesHistoryStateAfterResume(bool executeEntryActions)
#endif
        {
            ActionRecorder recorder = new ActionRecorder();

            TemplateFactory createTemplate = delegate()
            {
                StateMachineTemplate t = new StateMachineTemplate(StateMachineOptions.UseDoActions);
                //## Begin StateMachineTemplateSaveStateResumeWithHistory
                // Generated from <file:S:\StaMa_State_Machine_Controller_Library\StaMaShapesMaster.vst> page "UT_SaveStateResumeTests"
                // at 07-25-2015 16:55:36 using StaMaShapes Version 2300
                t.Region("StateA", false);
                    t.State("StateA", recorder.CreateAction("EnterA"), recorder.CreateAction("ExitA"), recorder.CreateDoAction("DoA"));
                        t.Region("StateA1A", true);
                            t.State("StateA1A", recorder.CreateAction("EnterA1A"), recorder.CreateAction("ExitA1A"), recorder.CreateDoAction("DoA1A"));
                                t.Transition("T1", "StateA1B", null, null, null);
                            t.EndState();
                            t.State("StateA1B", recorder.CreateAction("EnterA1B"), recorder.CreateAction("ExitA1B"), recorder.CreateDoAction("DoA1B"));
                                t.Transition("T2", "StateB1A", "Event1", null, null);
                            t.EndState();
                        t.EndRegion();
                    t.EndState();
                    t.State("StateB", recorder.CreateAction("EnterB"), recorder.CreateAction("ExitB"), recorder.CreateDoAction("DoB"));
                        t.Transition("T4", "StateA", "Event2", null, null);
                        t.Region("StateB1A", true);
                            t.State("StateB1A", recorder.CreateAction("EnterB1A"), recorder.CreateAction("ExitB1A"), recorder.CreateDoAction("DoB1A"));
                                t.Transition("T3", "StateB1B", null, null, null);
                            t.EndState();
                            t.State("StateB1B", recorder.CreateAction("EnterB1B"), recorder.CreateAction("ExitB1B"), recorder.CreateDoAction("DoB1B"));
                            t.EndState();
                        t.EndRegion();
                    t.EndState();
                t.EndRegion();
                //## End StateMachineTemplateSaveStateResumeWithHistory
                return t;
            };

            StateMachineTemplate t1 = createTemplate();
            t1.SerializationSignatureGenerator = TestSignatureGenerator;
            StateMachine s1 = t1.CreateStateMachine();
            s1.Startup();
            s1.SendTriggerEvent(null);
            s1.SendTriggerEvent("Event1");

            StateConfiguration expectedActiveStateS1 = t1.CreateStateConfiguration("StateB1B");
            Assert.That(s1.ActiveStateConfiguration.ToString(), Is.EqualTo(expectedActiveStateS1.ToString()), "Precondition not met: Unexpected state save state.");
            Assert.That(recorder.RecordedActions, Is.EqualTo(new String[] { "EnterA", "EnterA1A", "DoA", "DoA1A", "ExitA1A", "EnterA1B", "DoA", "DoA1B", "ExitA1B", "ExitA", "EnterB", "EnterB1A", "DoB", "DoB1A", "ExitB1A", "EnterB1B", "DoB", "DoB1B" }), "Precondition not met: Unexpected action execution sequence.");
            recorder.Clear();
            
            // Act
            MemoryStream memoryStream = new MemoryStream();
            s1.SaveState(memoryStream);
            memoryStream.Flush();
            memoryStream.Position = 0;

            StateMachine s2 = t1.CreateStateMachine();
            s2.Resume(memoryStream, executeEntryActions);

            // Assert
            Assert.That(s2.ActiveStateConfiguration.ToString(), Is.EqualTo(s1.ActiveStateConfiguration.ToString()), "State mismatch after Resume.");
            String[] expectedActions = executeEntryActions ? new String[] { "EnterB", "EnterB1B", "DoB", "DoB1B" } : new String[] { };
            Assert.That(recorder.RecordedActions, Is.EqualTo(expectedActions), "Unexpected entry or do actions during state machine resume with history.");
            recorder.Clear();
            s2.SendTriggerEvent("Event2");
            Assert.That(s2.ActiveStateConfiguration.ToString(), Is.EqualTo(t1.CreateStateConfiguration("StateA1B").ToString()), "Unexpected state machine behavior after Resume.");
            Assert.That(recorder.RecordedActions, Is.EqualTo(new String[] { "ExitB1B", "ExitB", "EnterA", "EnterA1B", "DoA", "DoA1B" }), "Unexpected entry or do actions during state machine resume.");

            // Compatibility test assertions
            // Data obtained from
            // FileStream fileStream = new FileStream("TestData.dat", FileMode.Create);
            // s1.SaveState(fileStream);
            // fileStream.Close();
            // PowerShell> gc -encoding byte "TestData.dat" |% {write-host ("0x{0:X2}," -f $_) -noNewline ""}; write-host
            StringBuilder sb = new StringBuilder();
            Byte[] actualBytes = memoryStream.ToArray();
            for (int i = 0; i < actualBytes.Length; i++)
            {
                sb.Append("0x" + actualBytes[i].ToString("X2") + ", ");
            }
#if !MF_FRAMEWORK
            Console.WriteLine("Actual bytes:");
            Console.WriteLine(sb.ToString());
#else
            Debug.Print("Actual bytes:");
            Debug.Print(sb.ToString());
#endif
            Byte[] compatibilityTestData2300 = new Byte[] { 0xAA, 0x00, 0x23, 0xA1, 0x47, 0x28, 0x7E, 0x31, 0x2D, 0x7B, 0x53, 0x74, 0x61, 0x74, 0x65, 0x41, 0x28, 0x7E, 0x31, 0x23, 0x7B, 0x53, 0x74, 0x61, 0x74, 0x65, 0x41, 0x31, 0x41, 0x2C, 0x53, 0x74, 0x61, 0x74, 0x65, 0x41, 0x31, 0x42, 0x2C, 0x7D, 0x29, 0x2C, 0x53, 0x74, 0x61, 0x74, 0x65, 0x42, 0x28, 0x7E, 0x31, 0x23, 0x7B, 0x53, 0x74, 0x61, 0x74, 0x65, 0x42, 0x31, 0x41, 0x2C, 0x53, 0x74, 0x61, 0x74, 0x65, 0x42, 0x31, 0x42, 0x2C, 0x7D, 0x29, 0x2C, 0x7D, 0x29, 0xA2, 0x01, 0x00, 0xA4, 0x01, 0x00, 0xA5, 0x08, 0x53, 0x74, 0x61, 0x74, 0x65, 0x42, 0x31, 0x42, 0xA3, 0x02, 0x00, 0xA4, 0x01, 0x00, 0xA5, 0x08, 0x53, 0x74, 0x61, 0x74, 0x65, 0x41, 0x31, 0x42, 0xA4, 0x01, 0x00, 0xA5, 0x08, 0x53, 0x74, 0x61, 0x74, 0x65, 0x42, 0x31, 0x42, };
            Assert.That(memoryStream.ToArray(), Is.EqualTo(compatibilityTestData2300), "Compatibility test failed. Written data different from version 2300.");

            MemoryStream memoryStream2300 = new MemoryStream(compatibilityTestData2300);
            StateMachineTemplate t3 = createTemplate();
            t3.SerializationSignatureGenerator = null;
            StateMachine s3 = t3.CreateStateMachine();
            s3.Resume(memoryStream2300, false);
            Assert.That(s3.ActiveStateConfiguration.ToString(), Is.EqualTo(expectedActiveStateS1.ToString()), "State mismatch after Resume.");
        }


#if !MF_FRAMEWORK
        [TestCase("(null)", true, TestName = "SerializationSignatureGenerator_NullValue_CalculatesSignatureAsExpected")]
        [TestCase("TestSignatureGenerator", true, TestName = "SerializationSignatureGenerator_ExternalGenerator_CalculatesSignatureAsExpected")]
        [TestCase("TestSignatureGenerator", false, TestName = "SerializationSignatureGenerator_ExternalGeneratorOtherHistory_CalculatesSignatureAsExpected")]
        [TestCase("Default", true, TestName = "SerializationSignatureGenerator_DefaultGenerator_CalculatesSignatureAsExpected")]
        public void SerializationSignatureGenerator_WithValue_CalculatesSignatureAsExpected(String serializationSignatureGeneratorName, bool hasHistoryStateB1A)
#else
        public void SerializationSignatureGenerator_NullValue_CalculatesSignatureAsExpected() { SerializationSignatureGenerator_WithValue_CalculatesSignatureAsExpected("(null)", true); }
        public void SerializationSignatureGenerator_ExternalGenerator_CalculatesSignatureAsExpected() { SerializationSignatureGenerator_WithValue_CalculatesSignatureAsExpected("TestSignatureGenerator", true); }
        public void SerializationSignatureGenerator_ExternalGeneratorOtherHistory_CalculatesSignatureAsExpected() { SerializationSignatureGenerator_WithValue_CalculatesSignatureAsExpected("TestSignatureGenerator", false); }
        public void SerializationSignatureGenerator_DefaultGenerator_CalculatesSignatureAsExpected() { SerializationSignatureGenerator_WithValue_CalculatesSignatureAsExpected("Default", true); }
        private void SerializationSignatureGenerator_WithValue_CalculatesSignatureAsExpected(String serializationSignatureGeneratorName, bool hasHistoryStateB1A)
#endif
        {
            // Arrange
            StateMachineTemplate t = new StateMachineTemplate(StateMachineOptions.UseDoActions);
            t.Region("StateA", false);
                t.State("StateA");
                    t.Region("StateA1A", true);
                        t.State("StateA1A");
                            t.Transition("T1", "StateA1B", null, null, null);
                        t.EndState();
                        t.State("StateA1B");
                            t.Transition("T2", "StateB1A", "Event1", null, null);
                        t.EndState();
                    t.EndRegion();
                t.EndState();
                t.State("StateB");
                    t.Transition("T4", "StateA", "Event2", null, null);
                    t.Region("StateB1A", hasHistoryStateB1A);
                        t.State("StateB1A");
                            t.Transition("T3", "StateB1B", null, null, null);
                        t.EndState();
                        t.State("StateB1B");
                        t.EndState();
                    t.EndRegion();
                t.EndState();
            t.EndRegion();

            String expectedSignature;
            SignatureGenerator serializationSignatureGenerator;
            switch (serializationSignatureGeneratorName)
            {
                case "(null)":
                    serializationSignatureGenerator = null;
                    expectedSignature = String.Empty;
                    break;
                case "TestSignatureGenerator":
                    serializationSignatureGenerator = TestSignatureGenerator;
                    expectedSignature = "(~1-{StateA(~1#{StateA1A,StateA1B,}),StateB(~1" + (hasHistoryStateB1A ? "#" : "-") + "{StateB1A,StateB1B,}),})";
                    break;
                case "Default":
                    Assert.That(t.SerializationSignatureGenerator, Is.Not.Null, "Precondition not met: Unexpected initial StateMachineTemplate.SerializationSignatureGenerator value.");
                    serializationSignatureGenerator = t.SerializationSignatureGenerator;
#if !MF_FRAMEWORK
                    expectedSignature = "6755BD36";
#else
                    expectedSignature = "9AD6D03F";
#endif
                    break;
                default:
                    throw new ArgumentException("Invalid generator name.", "serializationSignatureGeneratorName");
            }

            // Act
            t.SerializationSignatureGenerator = serializationSignatureGenerator;

            // Assert
            Assert.That(t.SerializationSignatureGenerator, Is.EqualTo(serializationSignatureGenerator));

            // Act
            String signature = t.Signature;
            Console.WriteLine(signature);

            // Assert
            Assert.That(signature, Is.EqualTo(expectedSignature));
            Assert.That(new TestDelegate(() => { t.SerializationSignatureGenerator = TestSignatureGenerator; }), Throws.TypeOf(typeof(InvalidOperationException)));
        }


        private static String TestSignatureGenerator(String input)
        {
            return input;
        }
    }
}
