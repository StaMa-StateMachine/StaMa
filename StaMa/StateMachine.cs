#region StateMachine.cs file
//
// StaMa state machine controller library, http://stama.codeplex.com/
//
// Copyright (c) 2005-2015, Roland Schneider. All rights reserved.
//
#endregion

using System;
using System.Reflection;
#if !MF_FRAMEWORK
using System.Collections.Generic;
#else
using System.Collections;
#endif
using System.Diagnostics;
using System.IO;
#if MF_FRAMEWORK
using Microsoft.SPOT;
#endif

#if STAMA_COMPATIBLE21
#pragma warning disable 0618
#endif

namespace StaMa
{
#if !STAMA_COMPATIBLE21
    /// <summary>
    /// Represents a method that will be called when a <see cref="State"/> entry, exit or <see cref="Transition"/> action shall be executed.
    /// </summary>
    /// <param name="stateMachine">
    /// The <see cref="StateMachine"/> instance that requests invocation of the method.
    /// </param>
    /// <param name="triggerEvent">
    /// An <see cref="Object"/> that represents the trigger event that was sent to the state machine.
    /// </param>
    /// <param name="eventArgs">
    /// An <see cref="EventArgs"/> instance that carries additional parameters accompanying the trigger event.
    /// </param>
    public delegate void StateMachineActionCallback(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs);
#else
    /// <summary>
    /// Obsolete. The term "activity" is improper and has been replaced with "action". Please use the properly named delegate StateMachineActionCallback instead.
    /// </summary>
    [Obsolete("The term \"activity\" is improper and has been replaced with \"action\". Please use the properly named delegate StateMachineActionCallback instead.")]
    public delegate void StateMachineActivityCallback(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs);
#endif


    /// <summary>
    /// Represents a method that will be called when a <see cref="Transition"/> guard shall be executed.
    /// </summary>
    /// <param name="stateMachine">
    /// The <see cref="StateMachine"/> instance that requests invocation of the method.
    /// </param>
    /// <param name="triggerEvent">
    /// An <see cref="Object"/> that represents the trigger event that was sent to the state machine.
    /// </param>
    /// <param name="eventArgs">
    /// An <see cref="EventArgs"/> instance that carries additional parameters accompanying the trigger event.
    /// </param>
    /// <returns>
    /// <c>true</c> if the <see cref="Transition"/> shall be enabled; otherwise, <c>false</c>.
    /// </returns>
    public delegate bool StateMachineGuardCallback(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs);


    /// <summary>
    /// Represents a method that will be called when a <see cref="State"/> do action shall be executed.
    /// </summary>
    /// <param name="stateMachine">
    /// The <see cref="StateMachine"/> instance that requests invocation of the method.
    /// </param>
    public delegate void StateMachineDoActionCallback(StateMachine stateMachine);


    /// <summary>
    /// Contains the active state of a state machine. Receives the trigger events and
    /// executes the actions according to the state machine structure defined by the <see cref="StateMachineTemplate"/> from which the <see cref="StateMachine"/> instance is created.
    /// </summary>
    /// <remarks>
    /// Instances of this class are created through the <see cref="StateMachineTemplate.CreateStateMachine()"/> method.
    /// </remarks>
    [DebuggerDisplay("StateMachine ActiveState={ToString()}")]
    public class StateMachine
    {
        /// <summary>
        /// Represents a method that will be called for tracing state changes in the <see cref="StateMachine"/>.
        /// </summary>
        /// <param name="stateMachine">
        /// The source <see cref="StateMachine"/> instance that raises the event.
        /// </param>
        /// <param name="stateConfigurationFrom">
        /// A <see cref="StateConfiguration"/> instance that represents the old state.
        /// </param>
        /// <param name="stateConfigurationTo">
        /// A <see cref="StateConfiguration"/> instance that represents the new state.
        /// </param>
        /// <param name="transition">
        /// The <see cref="Transition"/> that conveys the state machine from the old state to the new state.
        /// </param>
        /// <remarks>
        /// This event handler is not intended to interfere with the state machine excution.
        /// Actions within the state machine shall solely be executed through the <see cref="State.EntryAction"/>,
        /// <see cref="State.ExitAction"/> and <see cref="Transition.TransitionAction"/>.
        /// The purpose of this event handler is purely tracing for analysis purposes or other.
        /// </remarks>
#if !MF_FRAMEWORK
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Delegate type is only used in context with this class.")]
#endif
        public delegate void TraceStateChangeEventHandler(StateMachine stateMachine, StateConfiguration stateConfigurationFrom, StateConfiguration stateConfigurationTo, Transition transition);

        /// <summary>
        /// Represents a method that will be called whenever the event queue dispatches an event to the state machine.
        /// </summary>
        /// <param name="stateMachine">
        /// The source <see cref="StateMachine"/> instance that raises the event.
        /// </param>
        /// <param name="triggerEvent">
        /// An <see cref="Object"/> that represents the trigger event that was sent to the state machine.
        /// </param>
        /// <param name="eventArgs">
        /// An <see cref="EventArgs"/> instance that carries additional parameters accompanying the trigger event.
        /// </param>
#if !MF_FRAMEWORK
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Delegate type is only used in context with this class.")]
#endif
        public delegate void TraceDispatchTriggerEventEventHandler(StateMachine stateMachine, object triggerEvent, EventArgs eventArgs);

        /// <summary>
        /// Represents a method that will be called for tracing which transitions are
        /// considered by the state machine as active during a <see cref="SendTriggerEvent(object)"/> or <see cref="SendTriggerEvent(object,EventArgs)"/> call.
        /// </summary>
        /// <param name="stateMachine">
        /// The source <see cref="StateMachine"/> instance that raises the event.
        /// </param>
        /// <param name="transition">
        /// The <see cref="Transition"/> instance that is evaluated
        /// </param>
        /// <param name="triggerEvent">
        /// An <see cref="Object"/> that represents the trigger event that was sent to the state machine.
        /// </param>
        /// <param name="eventArgs">
        /// An <see cref="EventArgs"/> instance that carries additional parameters accompanying the trigger event.
        /// </param>
#if !MF_FRAMEWORK
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Delegate type is only used in context with this class.")]
#endif
        public delegate void TraceTestTransitionEventHandler(StateMachine stateMachine, Transition transition, object triggerEvent, EventArgs eventArgs);


        private StateMachineTemplate m_stateMachineTemplate;
        private StateConfiguration m_activeStateConfiguration;
        private StateConfiguration m_previousStateConfiguration;
        private TraceStateChangeEventHandler m_fnTraceStateChange;
        private TraceTestTransitionEventHandler m_fnTraceTestTransition;
        private TraceDispatchTriggerEventEventHandler m_fnTraceDispatchTriggerEvent;
#if !MF_FRAMEWORK
        private Queue<Object> m_eventQueue;
#else
        private Queue m_eventQueue;
#endif
        private bool m_eventProcessingEnabled;
        private StateConfiguration m_unalteredStateConfiguration; // Used in method DispatchTriggerEvent to keep the starting point while some previous transitions of orthogonal regions might already have changed m_activeStateConfiguration.
        private State[] m_historyStatesRepository;
        private object m_context;
        private static EventQueueItem NullEvent = new EventQueueItem() { m_triggerEvent = null, m_eventArgs = EventArgs.Empty };
        private const short saveStateResumeVersion = 0x2300;


        /// <summary>
        /// Initializes a new <see cref="StateMachine"/> instance.
        /// </summary>
        /// <param name="stateMachineTemplate">
        /// The <see cref="StateMachineTemplate"/> instance that defines the structure for this <see cref="StateMachine"/>.
        /// </param>
        /// <param name="context">
        /// A <see cref="Object"/> instance that may be used to provide additional context information
        /// within an action or guard condition given through <see cref="StateMachineActionCallback"/> and <see cref="StateMachineGuardCallback"/>.
        /// The given value will be accessible through the <see cref="Context"/> property.
        /// </param>
#if !MF_FRAMEWORK
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Method is internal.")]
#endif
        internal protected StateMachine(StateMachineTemplate stateMachineTemplate, object context)
        {
            m_stateMachineTemplate = stateMachineTemplate;
            m_context = context;
            m_activeStateConfiguration = stateMachineTemplate.CreateStateConfiguration(new string[] {});
            m_previousStateConfiguration = stateMachineTemplate.CreateStateConfiguration(new string[] {});
#if !MF_FRAMEWORK
            m_eventQueue = new Queue<Object>(10);
#else
            m_eventQueue = new Queue();
#endif
            m_eventProcessingEnabled = false;
            m_unalteredStateConfiguration = stateMachineTemplate.CreateStateConfiguration(new string[] { });
            m_historyStatesRepository = new State[stateMachineTemplate.HistoryMax];
        }


        /// <summary>
        /// Gets additional client provided context information.
        /// </summary>
        /// <value>
        /// A <see cref="Object"/> instance that may be used to provide additional context information
        /// within an action or guard condition given through <see cref="StateMachineActionCallback"/> and <see cref="StateMachineGuardCallback"/>.
        /// </value>
        /// <remarks>
        /// The <see cref="Context"/> property may be used in case the <see cref="StateMachineActionCallback"/>
        /// are coosen as static methods of the client class of the <see cref="StateMachine"/>. In this case the
        /// <see cref="Context"/> property may carry the client instance in order to
        /// forward the action to non-static methods of the client instance.
        /// This may be a reasonable strategy in case there are many <see cref="StateMachine"/> instances
        /// with the same <see cref="StateMachineTemplate"/>. Coosing the <see cref="StateMachineTemplate"/>
        /// as a static member of the client class could then improve startup performance of state
        /// machines doing the structural checks in the <see cref="StateMachineTemplate"/> only once for all
        /// <see cref="StateMachine"/> instances.
        /// </remarks>
        public object Context
        {
            get
            {
                return m_context;
            }
        }

        /// <summary>
        /// Sets the context in case it is not available before construction of the <see cref="StateMachine"/>
        /// instance.
        /// </summary>
        /// <param name="context">
        /// A <see cref="Object"/> instance that may be used to provide additional context information
        /// within an action or guard condition given through <see cref="StateMachineActionCallback"/> and <see cref="StateMachineGuardCallback"/>.
        /// </param>
        protected void SetContext(object context)
        {
            m_context = context;
        }


        /// <summary>
        /// Gets or sets a callback that can be used for tracing state changes in the <see cref="StateMachine"/>.
        /// </summary>
        /// <remarks>
        /// The delegate will be called when the <see cref="StateMachine"/> changes its active state.
        /// <para>
        /// The <see cref="StateMachine"/> passes the following parameters to the callback:
        /// <list type="number">
        /// <item>
        /// The <see cref="StateMachine"/> instance that changes the state.
        /// </item>
        /// <item>
        /// The <see cref="StateConfiguration"/> instance that represents the old state.
        /// </item>
        /// <item>
        /// The <see cref="StateConfiguration"/> instance that represents the new state.
        /// </item>
        /// <item>
        /// The <see cref="Transition"/> that conveys the state machine from the old state to the new state.
        /// </item>
        /// </list>
        /// </para>
        /// <para>
        /// The intended purpose of this callback is purely for tracing or analysis purposes.
        /// The callback is expected to not interfere with the state machine excution.
        /// Actions within a state machine shall solely be executed through the <see cref="State.EntryAction"/>,
        /// <see cref="State.ExitAction"/> and <see cref="Transition.TransitionAction"/>.
        /// </para>
        /// </remarks>
        public TraceStateChangeEventHandler TraceStateChange
        {
            get
            {
                return m_fnTraceStateChange;
            }
            set
            {
                m_fnTraceStateChange = value;
            }
        }


        /// <summary>
        /// Gets or sets a callback that can be used for tracing the <see cref="StateMachine"/> operation.
        /// </summary>
        /// <remarks>
        /// The delegate will be called whenever the event queue dispatches an event to the state machine.
        /// <para>
        /// The <see cref="StateMachine"/> passes the following parameters to the callback:
        /// <list type="number">
        /// <item>
        /// The <see cref="StateMachine"/> instance that dispatches the event.
        /// </item>
        /// <item>
        /// The <see cref="Object"/> that represents the trigger event that was sent to the state machine.
        /// </item>
        /// <item>
        /// The <see cref="EventArgs"/> instance that carries additional parameters accompanying the trigger event.
        /// </item>
        /// </list>
        /// </para>
        /// <para>
        /// The intended purpose of this callback is purely for tracing or analysis purposes.
        /// The callback is expected to not interfere with the state machine excution.
        /// Actions within a state machine shall solely be executed through the <see cref="State.EntryAction"/>,
        /// <see cref="State.ExitAction"/> and <see cref="Transition.TransitionAction"/>.
        /// </para>
        /// </remarks>
        public TraceDispatchTriggerEventEventHandler TraceDispatchTriggerEvent
        {
            get
            {
                return m_fnTraceDispatchTriggerEvent;
            }
            set
            {
                m_fnTraceDispatchTriggerEvent = value;
            }
        }


        /// <summary>
        /// Gets or sets a callback that can be used for tracing and analyzing the <see cref="StateMachine"/> operation.
        /// </summary>
        /// <remarks>
        /// The delegate will be called when the event queue dispatches an event to the state machine for every <see cref="Transition"/> that is regarded as active.
        /// <para>
        /// The <see cref="StateMachine"/> passes the following parameters to the callback:
        /// <list type="number">
        /// <item>
        /// The <see cref="StateMachine"/> instance that investigates the event.
        /// </item>
        /// <item>
        /// The <see cref="Transition"/> instance that investigates the event.
        /// </item>
        /// <item>
        /// The <see cref="Object"/> that represents the trigger event that was sent to the state machine.
        /// </item>
        /// <item>
        /// The <see cref="EventArgs"/> instance that carries additional parameters accompanying the trigger event.
        /// </item>
        /// </list>
        /// </para>
        /// <para>
        /// The intended purpose of this callback is purely for tracing or analysis purposes.
        /// The callback is expected to not interfere with the state machine excution.
        /// Actions within a state machine shall solely be executed through the <see cref="State.EntryAction"/>,
        /// <see cref="State.ExitAction"/> and <see cref="Transition.TransitionAction"/>.
        /// </para>
        /// </remarks>
        public TraceTestTransitionEventHandler TraceTestTransition
        {
            get
            {
                return m_fnTraceTestTransition;
            }
            set
            {
                m_fnTraceTestTransition = value;
            }
        }


        /// <summary>
        /// Returns the human readable representation of the active state <see cref="StateConfiguration"/> of this <see cref="StateMachine"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="String"/> with a textual representation of the <see cref="ActiveStateConfiguration"/> property.
        /// </returns>
        public override string ToString()
        {
            return m_activeStateConfiguration.ToString();
        }


        /// <summary>
        /// Brings the <see cref="StateMachine"/> in the initial state by invoking the entry actions
        /// of the initial states. Resets the event queue and initializes the history.
        /// </summary>
        /// <remarks>
        /// In order to trigger any completion transitions emanating from the initial states
        /// a <see cref="SendTriggerEvent(object)"/> with <c>null</c> must be sent to the state machine.
        /// </remarks>
        public void Startup()
        {
            this.StartupInternal(null, null, true);
        }


        private void StartupInternal(StateConfiguration startupStateConfiguration, State[] startupHistoryStates, bool executeEntryActions)
        {
            // Remove all pending events that may have been sent during our disabled phase
            this.ClearEventQueue();

            // Initialize to 'nirwana' to be sure we don't start with garbage from a previous run of this state machine, i.e. Startup()-Finish()-Startup()
            m_activeStateConfiguration.SetNirwana();

            if (startupStateConfiguration != null)
            {
                // Expect startupStateConfiguration in m_previousStateConfiguration instance to foster reuse of existing StateConfiguration instances.
                if (startupStateConfiguration != m_previousStateConfiguration)
                {
                    throw new ArgumentException("Unexpected instance passed in.", "startupStateConfiguration");
                }

                Array.Copy(startupHistoryStates, m_historyStatesRepository, m_historyStatesRepository.Length);
            }
            else
            {
                // Iterate through all regions and initialize history with initial states
                this.InitHistory(m_stateMachineTemplate.Root);

                startupStateConfiguration = m_previousStateConfiguration;

                // Calculate the initial state, use same algorithm as for an imaginary transition from 'nirwana' (in m_activeStateConfiguration) to the root region's initial state.
                startupStateConfiguration.InitializeAndResolve(null,
                    m_activeStateConfiguration,
                    m_stateMachineTemplate.Root,
                    m_historyStatesRepository);
            }

            // If desired: Announce the state change
            if (m_fnTraceStateChange != null)
            {
                m_fnTraceStateChange(this, m_activeStateConfiguration, startupStateConfiguration, null);
            }

            // Swap active state
            m_previousStateConfiguration = m_activeStateConfiguration;
            m_activeStateConfiguration = startupStateConfiguration;

            if (executeEntryActions)
            {
                // Execute entry actions of the new state
                this.EnterState(m_activeStateConfiguration, m_stateMachineTemplate.Root, null, null);

                if ((m_stateMachineTemplate.StateMachineOptions & StateMachineOptions.UseDoActions) != 0)
                {
                    this.ExecuteDoActions(m_activeStateConfiguration, m_stateMachineTemplate.Root);
                }
            }

            // Now we may enable event processing for the client
            m_eventProcessingEnabled = true;
        }


        private enum SerializationDataTag : byte
        {
            Invalid = 0,
            Magic = 0xAA,
            Signature = 0xA1,
            States = 0xA2,
            History = 0xA3,
            State = 0xA4,
            StateName = 0xA5,
            Region = 0xA6,
            RegionName = 0xA7,
        };


        /// <summary>
        /// Saves the state machine active state and history to a <see cref="Stream"/> that may be used to resume with this state later e.g. after the hosting process has been restarted.
        /// </summary>
        /// <param name="outputStream">
        /// A <see cref="Stream"/> that receives the serialized <see cref="ActiveStateConfiguration"/> and history states.
        /// </param>
        /// <remarks>
        /// The data written to the <paramref name="outputStream"/> is used as the input for the <see cref="Resume"/> method.
        /// </remarks>
        /// <example>
        /// <para>
        /// The following code shows how to use the <see cref="SaveState(System.IO.Stream)">SaveState</see> and <see cref="Resume(System.IO.Stream,System.Boolean)">Resume</see> methods:
        /// </para>
        /// <code source="..\Tests\StaMaTest\SaveStateResumeTests.cs" region="DevelopersGuide_SaveStateAndResume" lang="C#" title="SaveState and Resume" />
        /// </example>
        /// <exception cref="ArgumentNullException">The <paramref name="outputStream"/> parameter is <c>null</c></exception>
        /// <exception cref="ArgumentException">The <paramref name="outputStream"/> cannot be written.</exception>
        /// <exception cref="StateMachineException">This <see cref="StateMachine"/> instance is not yet started or already finished and thus has no valid <see cref="ActiveStateConfiguration"/>.</exception>
        public void SaveState(Stream outputStream)
        {
            if (outputStream == null)
            {
                throw new ArgumentNullException("outputStream");
            }
            if (!outputStream.CanWrite)
            {
                throw new ArgumentException("Unable to write outputStream.", "outputStream");
            }

            if (!m_eventProcessingEnabled)
            {
                throw new StateMachineException("This state machine is not yet started up or already finished. Saving the state machine state is only permitted when the state machine is in a defined state.");
            }

            SaveStateVisitor saveStateVisitor = new SaveStateVisitor();
            m_activeStateConfiguration.PassThrough(saveStateVisitor);

            State[] baseStates = saveStateVisitor.BaseStates;

            BinaryWriter writer = new BinaryWriter(outputStream, System.Text.Encoding.UTF8);
            writer.Write((byte)SerializationDataTag.Magic);
            writer.Write(saveStateResumeVersion);

            writer.Write((byte)SerializationDataTag.Signature);
            string signature = m_stateMachineTemplate.Signature;
            writer.Write(signature != null ? signature : String.Empty);

            writer.Write((byte)SerializationDataTag.States);
            writer.Write((short)baseStates.Length);
            foreach (State state in baseStates)
            {
                writer.Write((byte)SerializationDataTag.State);
                writer.Write((short)1);
                writer.Write((byte)SerializationDataTag.StateName);
                writer.Write(state.Name);
            }
            writer.Write((byte)SerializationDataTag.History);
            writer.Write((short)m_historyStatesRepository.Length);
            foreach (State state in m_historyStatesRepository)
            {
                writer.Write((byte)SerializationDataTag.State);
                writer.Write((short)1);
                writer.Write((byte)SerializationDataTag.StateName);
                writer.Write(state.Name);
            }
        }


        /// <summary>
        /// Initializes the <see cref="ActiveStateConfiguration"/> and history settings from a <see cref="Stream"/> and resumes state machine execution with the stored state.
        /// </summary>
        /// <param name="inputStream">
        /// A <see cref="Stream"/> that contains the serialized <see cref="ActiveStateConfiguration"/> and history states.
        /// </param>
        /// <param name="executeEntryActions">
        /// Indicates if the entry actions and do actions of the serialized <see cref="ActiveStateConfiguration"/> shall be executed. 
        /// </param>
        /// <remarks>
        /// <para>
        /// The <see cref="Resume"/> method is an alternative to the <see cref="Startup"/> and leaves the state machine in the operable state that accepts events through the <see cref="SendTriggerEvent(object)"/>.
        /// </para>
        /// <para>
        /// It is assumed that the contents of the given <paramref name="inputStream"/> can be read and that the structure of the state machine template has not changed since the
        /// contents were written.
        /// Compatibility of the structure is tested by comparing a signature computed from the current <see cref="StateMachineTemplate"/> structure with the signature that was computed and embedded when the state was saved.
        /// The default algorithm for computing the signature is the platform string hash code algorithm applied to a string that describes the entire <see cref="StateMachineTemplate"/> structure.
        /// Applications might change the signature algorithm by setting the <see cref="StateMachineTemplate.SerializationSignatureGenerator"/> property.
        /// </para>
        /// </remarks>
        /// <example>
        /// <para>
        /// The following code shows how to use the <see cref="SaveState(System.IO.Stream)">SaveState</see> and <see cref="Resume(System.IO.Stream,System.Boolean)">Resume</see> methods:
        /// </para>
        /// <code source="..\Tests\StaMaTest\SaveStateResumeTests.cs" region="DevelopersGuide_SaveStateAndResume" lang="C#" title="SaveState and Resume" />
        /// </example>
        /// <exception cref="ArgumentNullException">The <paramref name="inputStream"/> parameter is <c>null</c></exception>
        /// <exception cref="ArgumentException">The contents of the <paramref name="inputStream"/> are not compatible with the current <see cref="StateMachineTemplate"/> structure,
        /// e.g. states or regions might have been added to or removed from the <see cref="StateMachineTemplate"/> since the content was created.</exception>
        /// <exception cref="IOException">The <paramref name="inputStream"/> is corrupt.</exception>
        public void Resume(Stream inputStream, bool executeEntryActions)
        {
            if (inputStream == null)
            {
                throw new ArgumentNullException("inputStream");
            }
            if (!inputStream.CanRead)
            {
                throw new ArgumentException("Unable to read from inputStream.", "inputStream");
            }

            BinaryReader reader = new BinaryReader(inputStream, System.Text.Encoding.UTF8);

            // Read stream magic, version and state machine structure signature.
            byte tagMagic = reader.ReadByte();
            if (tagMagic != (byte)SerializationDataTag.Magic)
            {
                throw new ArgumentException("Could not resume from given Stream. Failed reading SerializationDataTag.Magic", "inputStream");
            }
            short version = reader.ReadInt16();
            if (version != saveStateResumeVersion)
            {
                throw new ArgumentException("Could not resume from given Stream. Failed reading serialization version.", "inputStream");
            }
            byte tagSignature = reader.ReadByte();
            if (tagSignature != (byte)SerializationDataTag.Signature)
            {
                throw new ArgumentException("Could not resume from given Stream. Failed reading SerializationDataTag.Signature", "inputStream");
            }
            string signature = reader.ReadString();
            string signatureCurrent = m_stateMachineTemplate.Signature;
            if (!FrameworkAbstractionUtils.StringIsNullOrEmpty(signatureCurrent))
            {
                if (signature != signatureCurrent)
                {
                    throw new ArgumentException("Could not resume from given Stream. Stored state machine signature doesn't match current signature which indicates that the state machine structure might have changed.", "inputStream");
                }
            }

            // Read states of active state configuration.
            byte tagStates = reader.ReadByte();
            if (tagStates != (byte)SerializationDataTag.States)
            {
                throw new ArgumentException("Could not resume from given Stream. Failed reading SerializationDataTag.States", "inputStream");
            }
            short statesCount = reader.ReadInt16();
            string[] baseStates = new string[statesCount];
            for (short i = 0; i < statesCount; i++)
            {
                byte tagState = reader.ReadByte();
                if (tagState != (byte)SerializationDataTag.State)
                {
                    throw new ArgumentException("Could not resume from given Stream. Failed reading SerializationDataTag.State", "inputStream");
                }
                short statePathCount = reader.ReadInt16();
                if (statePathCount != 1)
                {
                    throw new ArgumentException("Could not resume from given Stream. Unexpected state unique name parts count.", "inputStream");
                }
                byte tagStateName = reader.ReadByte();
                if (tagStateName != (byte)SerializationDataTag.StateName)
                {
                    throw new ArgumentException("Could not resume from given Stream. Failed reading SerializationDataTag.StateName", "inputStream");
                }
                string stateName = reader.ReadString();

                baseStates[i] = stateName;
            }

            // Read history states.
            byte tagHistory = reader.ReadByte();
            if (tagHistory != (byte)SerializationDataTag.History)
            {
                throw new ArgumentException("Could not resume from given Stream. Failed reading SerializationDataTag.History", "inputStream");
            }
            short historyCount = reader.ReadInt16();
            if (historyCount != m_historyStatesRepository.Length)
            {
                throw new ArgumentException("Could not resume from given Stream. History states: Unexpected history states count.", "inputStream");
            }
            State[] historyStates = new State[historyCount];
            for (short i = 0; i < historyCount; i++)
            {
                byte tagState = reader.ReadByte();
                if (tagState != (byte)SerializationDataTag.State)
                {
                    throw new ArgumentException("Could not resume from given Stream. History states: Failed reading SerializationDataTag.State", "inputStream");
                }
                short statePathCount = reader.ReadInt16();
                if (statePathCount != 1)
                {
                    throw new ArgumentException("Could not resume from given Stream. History states: Unexpected state unique name parts count.", "inputStream");
                }
                byte tagStateName = reader.ReadByte();
                if (tagStateName != (byte)SerializationDataTag.StateName)
                {
                    throw new ArgumentException("Could not resume from given Stream. History states: Failed reading SerializationDataTag.StateName.", "inputStream");
                }
                string stateName = reader.ReadString();

                State state = m_stateMachineTemplate.FindState(stateName);
                if (state == null)
                {
                    throw new ArgumentException("Could not resume from given Stream. History states: Could not find state.", "inputStream");
                }
                Region region = state.Parent;
                int historyIndex = region.HistoryIndex;
                if (historyIndex != i)
                {
                    throw new ArgumentException("Could not resume from given Stream. History states: Regions with history changed.", "inputStream");
                }
                historyStates[historyIndex] = state;
            }

            try
            {
                m_previousStateConfiguration.Initialize(baseStates);
            }
            catch (Exception)
            {
                throw new ArgumentException("Could not resume from given Stream. Failed to create a state configuration from stored states.", "inputStream");
            }

            this.StartupInternal(m_previousStateConfiguration, historyStates, executeEntryActions);
        }


        private class SaveStateVisitor : IStateConfigurationVisitor
        {
#if !STAMA_COMPATIBLE21
#if !MF_FRAMEWORK
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "StaMa.State.Name is guaranteed to be valid through the StaMa.State constructor.")]
#endif
            public void State(State state)
            {
                if (state.Regions.Count == 0)
                {
                    m_baseStates.Add(state);
                }
            }
#else
            public void State(string stateName)
            {
                throw new NotSupportedException("SaveState and Resume is not supported with STAMA_COMPATIBLE21.");
            }
#endif
            
            public void StateAny()
            {
                throw new InvalidOperationException("Encountered unspecified state while traversing the current state.");
            }

            public void BeginSubStates()
            {
            }

            public void EndSubStates()
            {
            }

            public void NextSubState()
            {
            }

#if !MF_FRAMEWORK
            private List<State> m_baseStates = new List<State>();

            public State[] BaseStates
            {
                get
                {
                    return m_baseStates.ToArray();
                }
            }
#else
            private ArrayList m_baseStates = new ArrayList();

            public State[] BaseStates
            {
                get
                {
                    return (State[])m_baseStates.ToArray(typeof(State));
                }
            }
#endif
        }


        /// <summary>
        /// Finalizes the <see cref="StateMachine"/> by executing the
        /// exit actions of all active states.
        /// </summary>
        public void Finish()
        {
            // Test whether this.Startup() has been done
            if (!m_eventProcessingEnabled) return;

            // Do not allow any further (recursive) event processing from now on.
            m_eventProcessingEnabled = false;

            // Do the state change by reusing the memory of m_previousStateConfiguration for the new (next) state.
            // At last swap the pointers of m_previousStateConfiguration and m_activeStateConfiguration to reflect the state change.
            StateConfiguration stateConfigurationNext = m_previousStateConfiguration;
            stateConfigurationNext.SetNirwana();

            // If desired: Announce the state change
            if (m_fnTraceStateChange != null)
            {
                m_fnTraceStateChange(this, m_activeStateConfiguration, stateConfigurationNext, null);
            }

            // Execute exit actions up to top level.
            // If we are already in nirwana state, there is no need to execute any actions.
            // TODO: Check if this condition is necessary / possible?
            if (!m_activeStateConfiguration.IsNirwana())
            {
                // Execute exit actions
                this.LeaveState(m_activeStateConfiguration, m_stateMachineTemplate.Root, null, null);
            }

            // Swap active state
            m_previousStateConfiguration = m_activeStateConfiguration;
            m_activeStateConfiguration = stateConfigurationNext;
        }


        /// <summary>
        /// Enqueues a trigger event and starts processing the state machine, if not already inside a processing.
        /// As a result a transition may be executed by invoking exit actions, changing the active state and invoking entry actions.
        /// </summary>
        /// <param name="triggerEvent">
        /// An <see cref="Object"/> that represents the trigger event that is sent to the state machine or <c>null</c> to trigger the evaluation of transitions having an "any" event.
        /// The <see cref="Object"/>  will be compared using <see cref="Object.Equals(Object)"/> to the value of the <see cref="Transition.TriggerEvent"/> property of the active <see cref="Transition"/> instances.
        /// </param>
        /// <returns>
        /// The number of executed state machine steps.
        /// </returns>
        public int SendTriggerEvent(object triggerEvent)
        {
            return this.SendTriggerEvent(triggerEvent, null);
        }


        /// <summary>
        /// Enqueues a trigger event together with additional parameters and starts processing the state machine, if not already inside a processing.
        /// As a result a transition may be executed by invoking exit actions, changing the active state and invoking entry actions.
        /// </summary>
        /// <param name="triggerEvent">
        /// An <see cref="Object"/> that represents the trigger event that is sent to the state machine or <c>null</c> to trigger the evaluation of transitions having an "any" event.
        /// The <see cref="Object"/>  will be compared using <see cref="Object.Equals(Object)"/> to the value of the <see cref="Transition.TriggerEvent"/> property of the active <see cref="Transition"/> instances.
        /// </param>
        /// <param name="eventArgs">
        /// An <see cref="EventArgs"/> instance that carries additional parameters accompanying the trigger event.
        /// The additional parameters may be used in the guard conditions of the active <see cref="Transition"/> instances.
        /// </param>
        /// <returns>
        /// The number of executed state machine steps.
        /// </returns>
        public int SendTriggerEvent(object triggerEvent, EventArgs eventArgs)
        {
            int stepsExecuted = -1;

            EventQueueItem eventQueueItem;
            if ((triggerEvent == null) && (eventArgs == EventArgs.Empty))
            {
                eventQueueItem = NullEvent;
            }
            else
            {
                eventQueueItem = new EventQueueItem();
                eventQueueItem.m_triggerEvent = triggerEvent;
                eventQueueItem.m_eventArgs = eventArgs;
            }
            
            m_eventQueue.Enqueue(eventQueueItem);
            
            if (m_eventProcessingEnabled)
            {
                m_eventProcessingEnabled = false;
                
                try
                {
                    stepsExecuted = 0;

                    // Test for enabled transitions until state machine has done all transitions possible
                    
                    bool transitionTestNeeded = true;
                    while (transitionTestNeeded)
                    {
                        // Process all queued events (including those sent by entry, exit or transition actions)
                        while (m_eventQueue.Count > 0)
                        {
                            EventQueueItem eventQueueItemQueued = (EventQueueItem)m_eventQueue.Dequeue();

                            // Send the event to the state machine
                            bool stateChanged = this.DispatchTriggerEvent(eventQueueItemQueued.m_triggerEvent, eventQueueItemQueued.m_eventArgs);
                            if (stateChanged)
                            {
                                stepsExecuted++;

                                if ((m_stateMachineTemplate.StateMachineOptions & StateMachineOptions.UseDoActions) != 0)
                                {
                                    this.ExecuteDoActions(m_activeStateConfiguration, m_stateMachineTemplate.Root);
                                }
                            }
                            transitionTestNeeded = stateChanged;
                        }

                        if (transitionTestNeeded)
                        {
                            // Now let's have a final look at the transitions, there
                            // may be completion transitions, which didn't have a chance up to now
                            bool stateChanged = this.DispatchTriggerEvent(null, null);
                            if (stateChanged)
                            {
                                stepsExecuted++;

                                if ((m_stateMachineTemplate.StateMachineOptions & StateMachineOptions.UseDoActions) != 0)
                                {
                                    this.ExecuteDoActions(m_activeStateConfiguration, m_stateMachineTemplate.Root);
                                }
                            }
                            transitionTestNeeded = stateChanged;
                        }
                        else
                        {
                            if ((m_stateMachineTemplate.StateMachineOptions & StateMachineOptions.UseDoActions) != 0)
                            {
                                if (stepsExecuted == 0)
                                {
                                    this.ExecuteDoActions(m_activeStateConfiguration, m_stateMachineTemplate.Root);
                                    transitionTestNeeded = true;
                                }
                            }
                        }

                        // transitionTestNeeded is true if state entry-, exit- or do-actions were executed which could have enqueued events or altered guard operands.
                        // So we have to test again for any enabled transitions.
                    }
                }
                finally
                {
                    m_eventProcessingEnabled = true;
                }
            }

            return stepsExecuted;
        }


        private bool DispatchTriggerEvent(object triggerEvent, EventArgs eventArgs)
        {
            if (m_fnTraceDispatchTriggerEvent != null)
            {
                m_fnTraceDispatchTriggerEvent(this, triggerEvent, eventArgs);
            }

            m_activeStateConfiguration.CopyTo(m_unalteredStateConfiguration);
            bool stateChanged = TestAndExecuteTransitions(m_unalteredStateConfiguration, m_stateMachineTemplate.Root, triggerEvent, eventArgs);

            return stateChanged;
        }


        private bool TestAndExecuteTransitions(StateConfiguration unalteredStateConfiguration,
                                               Region region,
                                               object triggerEvent, EventArgs eventArgs)
        {
            State state = unalteredStateConfiguration.GetState(region);

            // First check all transitions of this state
            foreach (Transition transition in state.Transitions)
            {

                // Are we in this state? (This query could be optimized by holding a cache of the transitions
                // which could get active, if the proper events or conditions arrive; cache might be filled after
                // a state transition

                // Because we allow transition guard states, we need to check from the root
                bool isSameAs = m_activeStateConfiguration.IsMatching(transition.SourceState, m_stateMachineTemplate.Root);
                if (isSameAs)
                {
                    if (m_fnTraceTestTransition != null)
                    {
                        m_fnTraceTestTransition(this, transition, triggerEvent, eventArgs);
                    }

                    // A transition is considered enabled if
                    // (1) The transition is defined with no specific event (i.e. transitions event parameter is null)
                    // (2) The transition is defined with an event, and this event has been sent (i.e. Equals signals equality)
                    if ((transition.TriggerEvent == null) ||
                        ( (triggerEvent != null) && triggerEvent.Equals(transition.TriggerEvent) ))
                    {
                        // If there is a condition with the transition, then check the condition
                        StateMachineGuardCallback guard = transition.Guard;
                        if (guard != null)
                        {
                            // If m_fnEvaluateGuard is not valid, then we will get an exception
                            // which is the desired behaviour in this situation, because the 
                            // state machine executional concept relies on this callback

                            bool permit = guard(this, triggerEvent, eventArgs);
                            if (permit)
                            {
                                // Found enabled transition
                                ExecuteTransition(transition, triggerEvent, eventArgs);
                                return true;
                            }
                        }
                        else
                        {
                            // Found enabled transition
                            ExecuteTransition(transition, triggerEvent, eventArgs);
                            return true;
                        }
                    }
                }
            }

            bool stateChanged = false;
            
            // Recurse to subregions
            foreach (Region subRegion in state.Regions)
            {
                // TestTransitions function:
                bool stateChangedSubRegion = TestAndExecuteTransitions(unalteredStateConfiguration, subRegion, triggerEvent, eventArgs);
                stateChanged = stateChanged || stateChangedSubRegion;
            }

            return stateChanged;
        }


        private void ExecuteTransition(Transition transition, object triggerEvent, EventArgs eventArgs)
        {
            // Switching to the target state will change at most the states belonging to the
            // transition's parent region.
            // So we initialize the new state with the active state, and then overwrite the
            // corresponding old state(s) with the transition's target states.
            // By the way we complete unspecified states of the transition target.

            // Do the state change by reusing the memory of m_previousStateConfiguration for the new (next) state.
            // At last swap the references of m_previousStateConfiguration and m_activeStateConfiguration to reflect the state change.

            StateConfiguration stateConfigurationNext = m_previousStateConfiguration;

            // Calculate the new (next) state based on the active state. Only the
            // sub-region of the transition's least common ancestor will change to the transition's target state.
            Region regionLCA = transition.LeastCommonAncestor;

            stateConfigurationNext.InitializeAndResolve(m_activeStateConfiguration,
                                                         transition.TargetState,
                                                         regionLCA,
                                                         m_historyStatesRepository);

            // If desired: Announce the state change
            if (m_fnTraceStateChange != null)
            {
                m_fnTraceStateChange(this, m_activeStateConfiguration, stateConfigurationNext, transition);
            }

            // Execute exit actions
            this.LeaveState(m_activeStateConfiguration, regionLCA, triggerEvent, eventArgs);

            // Execute transition actions
#if !STAMA_COMPATIBLE21
            StateMachineActionCallback action = transition.TransitionAction;
#else
            StateMachineActivityCallback action = transition.TransitionActivity;
#endif
            if (action != null)
            {
                action(this, triggerEvent, eventArgs);
            }

            // Swap active state
            m_previousStateConfiguration = m_activeStateConfiguration;
            m_activeStateConfiguration = stateConfigurationNext;

            // Execute entry actions of the new state
            this.EnterState(stateConfigurationNext, regionLCA, triggerEvent, eventArgs);
        }
        

        private void LeaveState(
            StateConfiguration stateConfiguration,
            Region region,
            object triggerEvent,
            EventArgs eventArgs)
        {
            State state = stateConfiguration.GetState(region);
            if (state == null)
            {
                // Logical error: All StateConfigurationIndex elements have to be filled with valid state references
                // Don't raise ArgumentException as this is a private method and client hasn't caused the problem.
                throw new InvalidOperationException("The given StateConfiguration doesn't have a defined value for the given region.");
            }

            // First recurse to all substates (and execute their leaving sequence)
            for (int i = state.Regions.Count - 1; i >= 0; i--)
            {
                Region subRegion = state.Regions[i];
                this.LeaveState(stateConfiguration, subRegion, triggerEvent, eventArgs);
            }
            
            // Then execute our exit action

#if !STAMA_COMPATIBLE21
            StateMachineActionCallback action = state.ExitAction;
#else
            StateMachineActivityCallback action = state.ExitActivity;
#endif
            if (action != null)
            {
                action(this, triggerEvent, eventArgs);
            }
        }


        private void EnterState(
            StateConfiguration stateConfiguration,
            Region region,
            object triggerEvent,
            EventArgs eventArgs)
        {
            State state = stateConfiguration.GetState(region);
            if (state == null)
            {
                // Logical error: All StateConfigurationIndex elements have to be filled with valid state references
                // Don't raise ArgumentException as this is a private method and client hasn't caused the problem.
                throw new InvalidOperationException("The given StateConfiguration doesn't have a defined value for the given region.");
            }

            // First execute our entry action
#if !STAMA_COMPATIBLE21
            StateMachineActionCallback action = state.EntryAction;
#else
            StateMachineActivityCallback action = state.EntryActivity;
#endif
            if (action != null)
            {
                action(this, triggerEvent, eventArgs);
            }

            // Save history for next time we enter this state
            if (region.HasHistory)
            {
                m_historyStatesRepository[region.HistoryIndex] = state;
            }

            // Then recurse to all substates (and execute their entering sequence)
            foreach (Region subRegion in state.Regions)
            {
                this.EnterState(stateConfiguration, subRegion, triggerEvent, eventArgs);
            }
        }


        private void ExecuteDoActions(StateConfiguration stateConfiguration, Region region)
        {
            State state = stateConfiguration.GetState(region);
            if (state == null)
            {
                // Logical error: All StateConfigurationIndex elements have to be filled with valid state references
                // Don't raise ArgumentException as this is a private method and client hasn't caused the problem.
                throw new InvalidOperationException("The given StateConfiguration doesn't have a defined value for the given region.");
            }

            // First execute our do action
            StateMachineDoActionCallback action = state.DoAction;
            if (action != null)
            {
                action(this);
            }

            // Recurse to all substates (and execute their do actions)
            foreach (Region subRegion in state.Regions)
            {
                this.ExecuteDoActions(stateConfiguration, subRegion);
            }
        }


        /// <summary>
        /// Gets the <see cref="StateMachineTemplate"/> instance that defines the structure for this <see cref="StateMachine"/>.
        /// </summary>
        public StateMachineTemplate Template
        {
            get
            {
                return m_stateMachineTemplate;
            }
        }


        /// <summary>
        /// Gets the active state of the <see cref="StateMachine"/> instance.
        /// </summary>
        /// <value>
        /// A <see cref="StateConfiguration"/> that contains the set of active states.
        /// In case the <see cref="StateMachineTemplate"/> has a structure with concurrent (orthogonal) states
        /// the active state is defined by more than one <see cref="State"/> instance.
        /// </value>
        public StateConfiguration ActiveStateConfiguration
        {
            get
            {
                return (StateConfiguration)m_activeStateConfiguration.Clone();
            }
        }


        /// <summary>
        /// Returns whether the <see cref="StateMachine"/> resides in the
        /// given <see cref="StateConfiguration"/>.
        /// </summary>
        /// <param name="stateConfiguration">
        /// A <see cref="StateConfiguration"/> instance that describes
        /// a (potentially partially specified) state configuration.
        /// </param>
        /// <returns>
        /// <c>true</c> if the <see cref="StateMachine"/> is in the given <paramref name="stateConfiguration"/>;
        /// otherwise, <c>false</c>.
        /// </returns>
#if !MF_FRAMEWORK
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "stateMachine.IsInState(state) sounds natural.")]
#endif
        public bool IsInState(StateConfiguration stateConfiguration)
        {
            return m_activeStateConfiguration.IsMatching(stateConfiguration);
        }


        /// <summary>
        /// Discards any pending events in the event queue of the <see cref="StateMachine"/>.
        /// </summary>
        public void ClearEventQueue()
        {
            m_eventQueue.Clear();
        }


        internal void InitHistory(Region region)
        {
            if (region.HasHistory)
            {
                m_historyStatesRepository[region.HistoryIndex] = region.InitialState;
            }

            // Recurse to all subregions
            foreach (State state in region.States)
            {
                foreach (Region subRegion in state.Regions)
                {
                    InitHistory(subRegion);
                }
            }
        }


        private class EventQueueItem
        {
            public object m_triggerEvent;
            public EventArgs m_eventArgs;
        }
    }
}

#if STAMA_COMPATIBLE21
#pragma warning restore 0618
#endif
