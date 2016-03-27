#region StateMachineTemplate.cs file
//
// StaMa state machine controller library, https://github.com/StaMa-StateMachine/StaMa
//
// Copyright (c) 2005-2016, Roland Schneider. All rights reserved.
//
#endregion

using System;
#if !MF_FRAMEWORK
using System.Collections.Generic;
using System.Collections;
#else
using System.Collections;
using Microsoft.SPOT;
#endif

#if STAMA_COMPATIBLE21
#pragma warning disable 0618
#endif

namespace StaMa
{
    /// <summary>
    /// Defines optional behavior or functionality for the state machines.
    /// </summary>
    [Flags]
    public enum StateMachineOptions
    {
        /// <summary>
        /// <para>
        /// Defines the default behavior for state machines.
        /// </para>
        /// <para>
        /// The default behavior is optimized for event driven state machines.
        /// </para>
        /// </summary>
        None = 0,

        /// <summary>
        /// <para>
        /// Defines that the execution of do-actions is enabled.
        /// </para>
        /// <para>
        /// Do-actions are executed "while" a state machine stays in a state, in particular whenever the <see cref="StateMachine.SendTriggerEvent(object,EventArgs)">StateMachine.SendTriggerEvent</see> method is called,
        /// after every individual state change or once, in case no state change occurred.
        /// </para>
        /// <para>
        /// Do-actions are specified like entry- and exit-actions through the <see cref="StateMachineTemplate.State(string,StateMachineActionCallback,StateMachineActionCallback,StateMachineDoActionCallback)">StateMachineTemplate.State</see> method.
        /// </para>
        /// <para>
        /// Do-actions can e.g. be used to run digital open or closed control loop algorithms for binary or continuous values when a state machine is regularly triggered in a cycle.
        /// Opposed to this, event driven state machines are only sporadically triggered when events occur or timers elapse, thus they don't benefit from do-actions.
        /// </para>
        /// </summary>
        UseDoActions = 1,
    }


    /// <summary>
    /// Represents a method that calculates a hash value for a <see cref="String"/>.
    /// </summary>
    /// <param name="input">
    /// The <see cref="String"/> for which to calculate the hash value.
    /// </param>
    /// <returns>
    /// A <see cref="String"/> that contains the hash value for the <paramref name="input"/> value.
    /// </returns>
    public delegate String SignatureGenerator(String input);


    /// <summary>
    /// Contains the structure and behaviour definition of a state machine.
    /// </summary>
    /// <remarks>
    /// <see cref="StateMachine"/> instances with identical behaviour may be created
    /// from an instance of this class through the <see cref="CreateStateMachine()"/> method.
    /// The <see cref="StateMachine"/> instances interpret the structure definition in
    /// the <see cref="StateMachineTemplate"/> instance during trigger event dispatching and
    /// accordingly execute state entry and exit callbacks.
    /// </remarks>
    public class StateMachineTemplate
    {
        private Region m_rootRegion;
        private RegionStack m_regionStack;
        private StateStack m_stateStack;
        private int m_historyMax;
        private int m_stateConfigurationMax;
        private int m_concurrencyDegree;
        private StateDictionary m_stateDictionary;
        private TransitionDictionary m_transitionDictionary;
        private StateMachineOptions m_stateMachineOptions;
        private static System.Text.RegularExpressions.Regex s_identifierRegEx;
        private SignatureGenerator m_serializationSignatureGenerator;
        private String m_signature;
        private bool m_signatureGenerated;

#if !MF_FRAMEWORK
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "This type has static methods only in context with s_identifierRegEx.")]
#endif
        static StateMachineTemplate()
        {
            s_identifierRegEx = new System.Text.RegularExpressions.Regex(
#if !MF_FRAMEWORK
            @"^[0-9]+$|^[_\p{Lu}\p{Ll}\p{Lt}\p{Lm}\p{Lo}\p{Nl}][_\p{Lu}\p{Ll}\p{Lt}\p{Lm}\p{Lo}\p{Nl}\p{Nd}\p{Pc}\p{Mn}\p{Mc}\p{Cf}]*$");
#else
            @"^[0-9]+$|^[_A-Za-z][_A-Za-z0-9]*$");
#endif
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachineTemplate"/> class.
        /// </summary>
        public StateMachineTemplate()
            : this(StateMachineOptions.None)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachineTemplate"/> class with specific options.
        /// </summary>
        /// <param name="stateMachineOptions">
        /// Defines the option flags for the state machine.
        /// </param>
        public StateMachineTemplate(StateMachineOptions stateMachineOptions)
        {
            m_stateMachineOptions = stateMachineOptions;

            m_rootRegion = null;

            // TBD: Think about creating these objects (m_regionStack,
            // m_stateStack, m_stateDictionary, ...) just in time and
            // release them after template has been sealed.
            m_regionStack = new RegionStack();
            m_regionStack.Push(null);

            m_stateStack = new StateStack();
            m_stateStack.Push(null);

            m_stateDictionary = new StateDictionary();
            m_transitionDictionary = new TransitionDictionary();

            m_historyMax = 0;
            m_stateConfigurationMax = 0;
            m_concurrencyDegree = 0;

            m_serializationSignatureGenerator = StandardSignatureGenerator;
        }


        /// <summary>
        /// Gets the option flags for the state machine.
        /// </summary>
        public StateMachineOptions StateMachineOptions
        {
            get
            {
                return m_stateMachineOptions;
            }
        }


        /// <summary>
        /// Gets the root <see cref="StaMa.Region"/> instance of this <see cref="StateMachineTemplate"/> instance.
        /// </summary>
        /// <value>
        /// Returns a valid <see cref="StaMa.Region"/> instance when the top level opening <see cref="StateMachineTemplate"/>.<see cref="Region"/> statement has been balanced with a
        /// <see cref="StateMachineTemplate"/>.<see cref="EndRegion"/> statement
        /// and the substructure is well-formed and valid; otherwise, <c>null</c>.
        /// </value>
        public Region Root
        {
            get
            {
                return m_rootRegion;
            }
        }


        /// <summary>
        /// Gets the length of the state array within a <see cref="StateConfiguration"/> instance.
        /// </summary>
        public int StateConfigurationMax
        {
            get
            {
                return m_stateConfigurationMax;
            }
        }


        /// <summary>
        /// Gets the maximum number of concurrent transitions possible for a single trigger event.
        /// </summary>
        public int ConcurrencyDegree
        {
            get
            {
                return m_concurrencyDegree;
            }
        }


        /// <summary>
        /// Gets the number of history storage slots necessary within a <see cref="StateMachine"/> instance.
        /// </summary>
        public int HistoryMax
        {
            get
            {
                return m_historyMax;
            }
        }


        /// <summary>
        /// Gets or sets a method that calculates a hash signature from a <see cref="String"/> that uniquely describes the state machine structure.
        /// </summary>
        /// <value>
        /// A <see cref="SignatureGenerator"/> delegate or <c>null</c> to suppress calculation of a signature when
        /// a <see cref="StateMachine"/> saves its state or resumes from a saved state.
        /// </value>
        public SignatureGenerator SerializationSignatureGenerator
        {
            get
            {
                return m_serializationSignatureGenerator;
            }
            set
            {
                if (m_signatureGenerated)
                {
                    throw new InvalidOperationException("Cannot change SerializationSignatureGenerator after signature has been generated.");
                }
                m_serializationSignatureGenerator = value;
            }
        }


#if !MF_FRAMEWORK
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", Justification = "No culture specific formatting.")]
#endif
        private static String StandardSignatureGenerator(String input)
        {
            return input.GetHashCode().ToString("X8");
        }


        /// <summary>
        /// Gets a hash signature for the state machine structure.
        /// </summary>
        /// <value>
        /// A <see cref="String"/> as calculated by the <see cref="SerializationSignatureGenerator"/>. Returns <see cref="String.Empty"/> if the <see cref="SerializationSignatureGenerator"/> has been set <c>null</c>.
        /// </value>
        public String Signature
        {
            get
            {
                if (m_rootRegion == null)
                {
                    throw new InvalidOperationException("Unable to get the signature before the state machine template is completely configured.");
                }
                if (!m_signatureGenerated)
                {
                    if (m_serializationSignatureGenerator != null)
                    {
                        SignatureCollector signatureCollector = new SignatureCollector();
                        m_rootRegion.PassThrough(signatureCollector);
                        m_signature = m_serializationSignatureGenerator(signatureCollector.TemplateStructure);
                    }
                    else
                    {
                        m_signature = String.Empty;
                    }
                    m_signatureGenerated = true;
                }
                return m_signature;
            }
        }


        /// <summary>
        /// Creates a <see cref="StaMa.Region"/> instance and precedes the definition of the <see cref="StaMa.Region"/>'s <see cref="StaMa.State"/>s.
        /// </summary>
        /// <param name="initialStateName">
        /// A <see cref="System.String"/> that defines the name of the <see cref="StaMa.Region"/>'s initial <see cref="StaMa.State"/>.
        /// </param>
        /// <param name="hasHistory">
        /// <c>true</c> if the <see cref="StaMa.Region"/> shall have a shallow history; otherwise, <c>false</c>.
        /// </param>
        /// <remarks>
        /// <para>
        /// This method must be used paired with an <see cref="StateMachineTemplate"/>.<see cref="EndRegion"/> statement.
        /// </para>
        /// <para>
        /// Each pair of enclosed <see cref="StateMachineTemplate"/>.<see cref="O:StaMa.StateMachineTemplate.State"/> ... <see cref="EndState"/> statements will add a <see cref="StaMa.State"/> to the <see cref="StaMa.Region"/>.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="initialStateName"/> is <c>null</c> or empty.
        /// </exception>
        /// <exception cref="StateMachineException">
        /// A syntax error was detected with this or the preceeding Region, EndRegion, State, EndState or Transition statements.
        /// </exception>
        public void Region(string initialStateName, bool hasHistory)
        {
            if (FrameworkAbstractionUtils.StringIsNullOrEmpty(initialStateName))
            {
                throw new ArgumentNullException("initialStateName");
            }

            if (this.Root != null)
            {
                throw new StateMachineException("This StateMachineTemplate is already complete, no further template element insertions are allowed.");
            }

            State parentState = m_stateStack.Peek();

            if ((parentState == null) && (hasHistory))
            {
                throw new StateMachineException("Syntax error: Activation of the history function is not appropriate for the root Region.");
            }

            // Check syntax: Not allowed to nest regions without intermediate "State".
            Region regionGrandParent = m_regionStack.Peek();
            if (regionGrandParent != null)
            {
                if (regionGrandParent.Parent == parentState)
                {
                    throw new StateMachineException("Syntax error: A \"Region()\" statement must be embedded within \"State()\"...\"EndState()\"");
                }
            }

            // Reserve storage for reference to history state in history array.
            // (class StateMachine will allocate storage for m_historyMax references)
            int historyIndex = int.MaxValue;
            if (hasHistory)
            {
                historyIndex = m_historyMax;
                m_historyMax++;
            }

            // Now ready for creating a region object
            Region region = this.InternalCreateRegion(parentState, initialStateName, historyIndex);

            // Link region to parent
            if (parentState != null)
            {
                parentState.AddRegion(region);
            }
            
            // Push this region to make it available for childs
            m_regionStack.Push(region);
        }


        /// <summary>
        /// Completes the definition of a <see cref="StaMa.Region"/>'s <see cref="StaMa.State"/>s.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method must be used paired after a <see cref="StateMachineTemplate"/>.<see cref="StateMachineTemplate.Region"/> statement to
        /// conclude the definition of the contents of a <see cref="StaMa.Region"/>.
        /// </para>
        /// <para>
        /// The syntactic validation of the <see cref="StateMachineTemplate"/> is started within this statement, if this is the
        /// paired <see cref="StateMachineTemplate"/>.EndRegion statement for the top level <see cref="StaMa.Region"/> instance within the <see cref="StateMachineTemplate"/>.
        /// If the validation is successful, the <see cref="StateMachineTemplate"/> instance is ready for creating <see cref="StateMachine"/> instances
        /// by invoking <see cref="O:StaMa.StateMachineTemplate.CreateStateMachine"/>.
        /// </para>
        /// </remarks>
        /// <exception cref="StateMachineException">
        /// A syntax error was detected with this or the preceeding Region, EndRegion, State, EndState or Transition statements.
        /// </exception>
        public void EndRegion()
        {
            if (this.Root != null)
            {
                throw new StateMachineException("This StateMachineTemplate is already complete, no further template element insertions are allowed.");
            }
            
            // Pop this region to balance stack
            Region region = m_regionStack.Pop();
            if (region == null)
            {
                // Typical reason for this: Cannot start a template with "EndRegion"
                throw new StateMachineException("Syntax error: A \"State()\" statement must be embedded within \"Region()\"...\"EndRegion()\"");
            }

            State parentState = region.Parent;
            if (parentState != m_stateStack.Peek())
            {
                // Typical reason for this: Unbalanced tree.
                throw new StateMachineException("Syntax error: Unbalanced tree.");  // TODO Check.
            }

            // Resolve initial state name to state instance and others.
            region.EndRegion();
            
            // If we are back at the root we have to do some special things
            if (parentState == null)
            {
                try
                {
                    // Lock this template, no further template element insertions allowed
                    m_rootRegion = region;

                    // Calculate the size of state configurations and
                    // set the regions m_stateConfigurationIndex
                    int stateConfigurationIndex = 0;
                    region.FixupAndCalcStateConfigMetrics(ref stateConfigurationIndex, out m_stateConfigurationMax, out m_concurrencyDegree);

                    // Iterate through all transis to fixup source and target state and
                    //         through all regions to fixup initial state id
                    region.FinalFixup(this);

                    m_regionStack = null;
                    m_stateStack = null;
                    m_transitionDictionary = null;
                }
                catch
                {
                    m_rootRegion = null;
                    throw;
                }
            }
        }


        /// <summary>
        /// Creates a <see cref="StaMa.State"/> instance and starts the definition the <see cref="StaMa.State"/>'s substructure and contents.
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="StaMa.State"/> to be created. Must be unique within the <see cref="StateMachineTemplate"/>.
        /// </param>
        /// <param name="entryAction">
        /// A <see cref="StateMachineActionCallback"/> delegate that will be called when a <see cref="StateMachine"/>
        /// enters the <see cref="StaMa.State"/>.
        /// May be <c>null</c> if no entry action is specified.
        /// </param>
        /// <param name="exitAction">
        /// A <see cref="StateMachineActionCallback"/> delegate that will be called when a <see cref="StateMachine"/>
        /// leaves the the <see cref="StaMa.State"/>.
        /// May be <c>null</c> if no exit action is specified.
        /// </param>
        /// <param name="doAction">
        /// The <see cref="StateMachineDoActionCallback"/> delegate that defines the perpetual action to be executed while the <see cref="StateMachine"/> is in this <see cref="StaMa.State"/>.
        /// May be <c>null</c> if no do action is specified.
        /// </param>
        /// <remarks>
        /// <para>
        /// This method must be used paired with an <see cref="StateMachineTemplate"/>.<see cref="EndState"/> statement.
        /// </para>
        /// <para>
        /// An enclosed <see cref="StateMachineTemplate"/>.<see cref="O:StaMa.StateMachineTemplate.Transition"/> statement adds a <see cref="StaMa.Transition"/> emanating from this <see cref="StaMa.State"/>.
        /// </para>
        /// <para>
        /// Each pair of enclosed <see cref="StateMachineTemplate"/>.<see cref="Region"/> ... <see cref="EndRegion"/> statements will add a hierarchical sub-<see cref="StaMa.Region"/> to the <see cref="StaMa.State"/>.
        /// </para>
        /// <para>
        /// The <paramref name="doAction"/> is executed whenever the <see cref="StateMachine.SendTriggerEvent(object,EventArgs)">StateMachine.SendTriggerEvent</see> method is called,
        /// therein after every individual state change or once, in case no state change occurred.
        /// </para>
        /// <para>
        /// Do-actions can e.g. be used to run digital open or closed control loop algorithms for binary or continuous values when a state machine is regularly triggered in a cycle.
        /// Opposed to this, event driven state machines are only sporadically triggered when events occur or timers elapse, thus they don't benefit from do-actions.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// The <paramref name="doAction"/> is valid, but do actions are not enabled for this <see cref="StateMachineTemplate"/>.
        /// To enable do actions, please create the <see cref="StateMachineTemplate"/> instance with the <see cref="StateMachineOptions"/>.UseDoActions flag specified at the constructor.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="name"/> is <c>null</c> or empty.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="name"/> is not a valid identifier for a <see cref="StaMa.State"/>: The identifier of a <see cref="StaMa.State"/> must start with a character followed by nonspace characters or digits.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="name"/> is not a unique: A <see cref="StaMa.State"/> with the same name already exists within the <see cref="StateMachineTemplate"/> instance.
        /// </exception>
        /// <exception cref="StateMachineException">
        /// A syntax error was detected with this or the preceeding Region, EndRegion, State, EndState or Transition statements.
        /// </exception>
        public void State(
            string name,
#if !STAMA_COMPATIBLE21
            StateMachineActionCallback entryAction,
            StateMachineActionCallback exitAction,
#else
            StateMachineActivityCallback entryAction,
            StateMachineActivityCallback exitAction,
#endif
            StateMachineDoActionCallback doAction)
        {
            if (this.Root != null)
            {
                throw new StateMachineException("This StateMachineTemplate is already complete, no further template element insertions are allowed.");
            }

            Region parentRegion = m_regionStack.Peek();
            if (parentRegion == null)
            {
                // Not allowed to start a template with "State..EndState".
                throw new StateMachineException("Syntax error: Cannot start a template with \"State..EndState\".");
            }

            State grandParentState = m_stateStack.Peek();
            if (grandParentState != null)
            {
                if (parentRegion == grandParentState.Parent)
                {
                    // Not allowed to nest states without intermediate "Region".
                    throw new StateMachineException("Syntax error: Cannot nest states without embedding \"Region(...)\".");
                }
            }

            if (((m_stateMachineOptions & StateMachineOptions.UseDoActions) == 0) && (doAction != null))
            {
                // Do actions are not supported.
                throw new ArgumentException("Do actions are not enabled for this state machine. To enable do-actions, please create the StateMachineTemplate with StateMachineOptions.UseDoActions flag specified at the constructor.", "doAction");
            }

            // Check if name is a valid identifier
            if (!IsValidIdentifier(name))
            {
                // Invalid identifier.
                throw new ArgumentOutOfRangeException("name", "The identifier of a State must start with a character followed by nonspace characters or digits.");
            }

            // Check uniqueness of state name
            if (m_stateDictionary.ContainsKey(name))
            {
                // Double defined state name.
                throw new ArgumentOutOfRangeException("name", FrameworkAbstractionUtils.StringFormat("A state with an identical name \"{0}\" is already defined.", name));
            }

            // Create new object and ...
            State state = this.InternalCreateState(parentRegion, name, entryAction, exitAction, doAction);

            // ... add it to the region's list of states
            parentRegion.AddState(state);

            // Push this state to make it available for childs
            m_stateStack.Push(state);

            // Remember this state name
            m_stateDictionary.Add(name, state);
        }


#if !STAMA_COMPATIBLE21
        /// <summary>
        /// Creates a <see cref="StaMa.State"/> instance and starts the definition the <see cref="StaMa.State"/>'s substructure and contents.
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="StaMa.State"/> to be created. Must be unique within the <see cref="StateMachineTemplate"/>.
        /// </param>
        /// <param name="entryAction">
        /// A <see cref="StateMachineActionCallback"/> delegate that will be called when a <see cref="StateMachine"/>
        /// enters the <see cref="StaMa.State"/>.
        /// May be <c>null</c> if no entry action is specified.
        /// </param>
        /// <param name="exitAction">
        /// A <see cref="StateMachineActionCallback"/> delegate that will be called when a <see cref="StateMachine"/>
        /// leaves the the <see cref="StaMa.State"/>.
        /// May be <c>null</c> if no exit action is specified.
        /// </param>
        /// <remarks>
        /// <para>
        /// This method must be used paired with an <see cref="StateMachineTemplate"/>.<see cref="EndState"/> statement.
        /// </para>
        /// <para>
        /// An enclosed <see cref="StateMachineTemplate"/>.<see cref="O:StaMa.StateMachineTemplate.Transition"/> statement adds a <see cref="StaMa.Transition"/> emanating from this <see cref="StaMa.State"/>.
        /// </para>
        /// <para>
        /// Each pair of enclosed <see cref="StateMachineTemplate"/>.<see cref="Region"/> ... <see cref="EndRegion"/> statements will add a hierarchical sub-<see cref="StaMa.Region"/> to the <see cref="StaMa.State"/>.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="name"/> is <c>null</c> or empty.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="name"/> is not a valid identifier for a <see cref="StaMa.State"/>: The identifier of a <see cref="StaMa.State"/> must start with a character followed by nonspace characters or digits.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="name"/> is not a unique: A <see cref="StaMa.State"/> with the same name already exists within the <see cref="StateMachineTemplate"/> instance.
        /// </exception>
        /// <exception cref="StateMachineException">
        /// A syntax error was detected with this or the preceeding Region, EndRegion, State, EndState or Transition statements.
        /// </exception>
        public void State(
            string name,
            StateMachineActionCallback entryAction,
            StateMachineActionCallback exitAction)
#else
        /// <summary>
        /// Obsolete. The term "activity" is improper and has been replaced with "action". Please use instead the State method with consistent naming.
        /// </summary>
        [Obsolete("The term \"activity\" is improper and has been replaced with \"action\". Please use instead the State method with consistent naming.")]
        public void State(
            string name,
            StateMachineActivityCallback entryAction,
            StateMachineActivityCallback exitAction)
#endif
        {
            this.State(name, entryAction, exitAction, null);
        }



        /// <summary>
        /// Creates a <see cref="StaMa.State"/> instance and starts the definition the <see cref="StaMa.State"/>'s substructure and contents.
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="StaMa.State"/> to be created. Must be unique within the <see cref="StateMachineTemplate"/>.
        /// </param>
        /// <remarks>
        /// <para>
        /// This method must be used paired with an <see cref="StateMachineTemplate"/>.<see cref="EndState"/> statement.
        /// </para>
        /// <para>
        /// An enclosed <see cref="StateMachineTemplate"/>.<see cref="O:StaMa.StateMachineTemplate.Transition"/> statement adds a <see cref="StaMa.Transition"/> emanating from this <see cref="StaMa.State"/>.
        /// </para>
        /// <para>
        /// Each pair of enclosed <see cref="StateMachineTemplate"/>.<see cref="Region"/> ... <see cref="EndRegion"/> statements will add a hierarchical sub-<see cref="StaMa.Region"/> to the <see cref="StaMa.State"/>.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="name"/> is <c>null</c> or empty.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="name"/> is not a valid identifier for a <see cref="StaMa.State"/>: The identifier of a <see cref="StaMa.State"/> must start with a character followed by nonspace characters or digits.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="name"/> is not a unique: A <see cref="StaMa.State"/> with the same name already exists within the <see cref="StateMachineTemplate"/> instance.
        /// </exception>
        /// <exception cref="StateMachineException">
        /// A syntax error was detected with this or the preceeding Region, EndRegion, State, EndState or Transition statements.
        /// </exception>
        public void State(string name)
        {
            this.State(name, null, null, null);
        }


        /// <summary>
        /// Completes the definition of a <see cref="StaMa.State"/>'s substructure and contents.
        /// </summary>
        /// <remarks>
        /// This method must be used paired after a <see cref="StateMachineTemplate"/>.<see cref="O:StaMa.StateMachineTemplate.State"/> statement to
        /// conclude the definition of the contents of a <see cref="StaMa.State"/>.
        /// </remarks>
        /// <exception cref="StateMachineException">
        /// A syntax error was detected with this or the preceeding Region, EndRegion, State, EndState or Transition statements.
        /// </exception>
        public void EndState()
        {
            if (this.Root != null)
            {
                throw new StateMachineException("This StateMachineTemplate is already complete, no further template element insertions are allowed.");
            }

            // Pop this state to balance stack
            State state = m_stateStack.Pop();
            if (state == null)
            {
                // Not allowed to start a template with "EndState".
                throw new StateMachineException("Syntax error: \"State()\"...\"EndState()\" statements are unbalanced.");
            }

            Region parentRegion = m_regionStack.Peek();
            if (parentRegion == null)
            {
                // Unbalanced tree.
                throw new StateMachineException("Syntax error: \"State()\"...\"EndState()\" statements are unbalanced.");
            }
        }


        /// <summary>
        /// Creates a <see cref="StaMa.Transition"/> that starts at the <see cref="StaMa.State"/> defined by the enclosing <see cref="StateMachineTemplate"/>.<see cref="O:StaMa.StateMachineTemplate.State"/>...<see cref="EndState"/> statements.
        /// </summary>
        /// <param name="name">
        /// A name that identifies the <see cref="StaMa.Transition"/> for debugging and tracing purposes. Must be unique within the <see cref="StateMachineTemplate"/>.
        /// </param>
        /// <param name="targetState">
        /// The name of a <see cref="StaMa.State"/> instance that defines the target-<see cref="StaMa.State"/> of the <see cref="StaMa.Transition"/>.
        /// </param>
        /// <param name="triggerEvent">
        /// A <see cref="System.Object"/> that represents the trigger event that will execute the transition or <c>null</c> to indicate an "any" transition.
        /// See also <see cref="StateMachine.SendTriggerEvent(object)"/>.
        /// </param>
        /// <remarks>
        /// <para>
        /// The <paramref name="targetState"/> may reference any <see cref="StaMa.State"/> within this <see cref="StateMachineTemplate"/>.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="name"/> is <c>null</c> or empty.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="name"/> is not a valid identifier for a <see cref="StaMa.Transition"/>: The identifier of a <see cref="StaMa.Transition"/> must start with a character followed by nonspace characters or digits.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="name"/> is not a unique: A <see cref="StaMa.Transition"/> with the same name already exists within the <see cref="StateMachineTemplate"/> instance.
        /// </exception>
        /// <exception cref="StateMachineException">
        /// A syntax error was detected with this or the preceeding Region, EndRegion, State, EndState or Transition statements.
        /// </exception>
        public void Transition(string name,
            string targetState,
            object triggerEvent)
        {
            this.TransitionInternal(name,
                null, new string[] { targetState },
                triggerEvent,
                null, null);
        }


        /// <summary>
        /// Creates a fork <see cref="StaMa.Transition"/> that starts at the <see cref="StaMa.State"/> defined by the enclosing <see cref="StateMachineTemplate"/>.<see cref="O:StaMa.StateMachineTemplate.State"/>...<see cref="EndState"/> statements.
        /// </summary>
        /// <param name="name">
        /// A name that identifies the <see cref="StaMa.Transition"/> for debugging and tracing purposes. Must be unique within the <see cref="StateMachineTemplate"/>.
        /// </param>
        /// <param name="targetStates">
        /// A list of names of <see cref="StaMa.State"/> instances that define the target-<see cref="StaMa.State"/> instances of the <see cref="StaMa.Transition"/>.
        /// The <paramref name="targetStates"/> collection must form a valid <see cref="StaMa.State"/> configuration within this <see cref="StateMachineTemplate"/>.
        /// </param>
        /// <param name="triggerEvent">
        /// A <see cref="System.Object"/> that represents the trigger event that will execute the transition or <c>null</c> to indicate an "any" transition.
        /// See also <see cref="StateMachine.SendTriggerEvent(object)"/>.
        /// </param>
        /// <remarks>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="name"/> is <c>null</c> or empty.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="name"/> is not a valid identifier for a <see cref="StaMa.Transition"/>: The identifier of a <see cref="StaMa.Transition"/> must start with a character followed by nonspace characters or digits.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="name"/> is not a unique: A <see cref="StaMa.Transition"/> with the same name already exists within the <see cref="StateMachineTemplate"/> instance.
        /// </exception>
        /// <exception cref="StateMachineException">
        /// A syntax error was detected with this or the preceeding Region, EndRegion, State, EndState or Transition statements.
        /// </exception>
        public void Transition(string name,
            string[] targetStates,
            object triggerEvent)
        {
            this.TransitionInternal(name,
                null, targetStates,
                triggerEvent,
                null, null);
        }


        /// <summary>
        /// Creates a <see cref="StaMa.Transition"/> from the <see cref="StaMa.State"/> defined by the enclosing <see cref="StateMachineTemplate"/>.<see cref="O:StaMa.StateMachineTemplate.State"/> ... <see cref="EndState"/> statements.
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="StaMa.Transition"/> to identify the <see cref="StaMa.Transition"/> for debugging and tracing purposes. Must be unique within the <see cref="StateMachineTemplate"/>.
        /// </param>
        /// <param name="sourceState">
        /// The name of a <see cref="StaMa.State"/> instance that defines the <see cref="StaMa.Transition"/> source-<see cref="StaMa.State"/>.
        /// </param>
        /// <param name="targetState">
        /// The name of a <see cref="StaMa.State"/> instance that defines the <see cref="StaMa.Transition"/> target-<see cref="StaMa.State"/>.
        /// </param>
        /// <param name="triggerEvent">
        /// A <see cref="System.Object"/> that represents the trigger event that will execute the transition or <c>null</c> to indicate an "any" transition.
        /// See also <see cref="StateMachine.SendTriggerEvent(object)"/>.
        /// </param>
        /// <remarks>
        /// <para>
        /// The <paramref name="sourceState"/> may only reference a <see cref="StaMa.State"/> that is a sub-<see cref="StaMa.State"/> of the anchor-<see cref="StaMa.State"/>.
        /// </para>
        /// <para>
        /// The <paramref name="targetState"/> may reference any <see cref="StaMa.State"/> within this <see cref="StateMachineTemplate"/>.
        /// </para>
        /// <para>
        /// Usually the <paramref name="sourceState"/> will be identical with the name of the <see cref="StaMa.State"/> defined
        /// through the enclosing <see cref="StateMachineTemplate"/>.<see cref="O:StaMa.StateMachineTemplate.State"/> ... <see cref="EndState"/> statements.
        /// </para>
        /// <para>
        /// In some cases it may be useful to define a <see cref="StaMa.Transition"/> on a higher hierarchical level in order to raise the <see cref="StaMa.Transition"/>'s priority above other
        /// transitions that would otherwise be handled premptive due to their hierarchy level.
        /// For such cases the <paramref name="sourceState"/> will reference a <see cref="StaMa.State"/>
        /// that is hierachically nested within the immediate enclosing <see cref="StateMachineTemplate"/>.<see cref="O:StaMa.StateMachineTemplate.State"/> ... <see cref="EndState"/> statement pair.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="name"/> is <c>null</c> or empty.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="name"/> is not a valid identifier for a <see cref="StaMa.Transition"/>: The identifier of a <see cref="StaMa.Transition"/> must start with a character followed by nonspace characters or digits.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="name"/> is not a unique: A <see cref="StaMa.Transition"/> with the same name already exists within the <see cref="StateMachineTemplate"/> instance.
        /// </exception>
        /// <exception cref="StateMachineException">
        /// A syntax error was detected with this or the preceeding Region, EndRegion, State, EndState or Transition statements.
        /// </exception>
        public void Transition(string name,
            string sourceState,
            string targetState,
            object triggerEvent)
        {
            this.TransitionInternal(name,
                new string[] { sourceState }, new string[] { targetState },
                triggerEvent,
                null, null);
        }


        /// <summary>
        /// Defines a transition emanating from state defined through
        /// the enclosing <see cref="State(string)"/>...<see cref="EndState"/> calls.
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="StaMa.Transition"/> to be created. Must be unique within the <see cref="StateMachineTemplate"/>.
        /// </param>
        /// <param name="sourceStates">
        /// A list of names of the transition source states.
        /// </param>
        /// <param name="targetState">
        /// A state name if the transition target is a single state.
        /// </param>
        /// <param name="triggerEvent">
        /// A <see cref="System.Object"/> that represents the trigger event that will execute the transition or <c>null</c> to indicate an "any" transition.
        /// See also <see cref="StateMachine.SendTriggerEvent(object)"/>.
        /// </param>
        /// <remarks>
        /// <para>
        /// The source state may only reference a state or state configuration of sub-states of the anchor state.
        /// </para>
        /// <para>
        /// The target state may reference any state or state configuration within the state machine template.
        /// </para>
        /// <para>
        /// Usually the source state will be identical with the name of the <see cref="StaMa.State"/> defined
        /// through the enclosing <see cref="State(string)"/> call. In some cases it may be useful to
        /// define a transition on a higher hierarchical level in order to raise its priority above other
        /// transitions that are handled premptive due to their hierarchy level. For such cases
        /// the <paramref name="sourceStates"/> may reference a set of <see cref="StaMa.State"/> instances on a hierarchical lower
        /// layer than the immediate <see cref="StaMa.State"/> parent.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="name"/> is <c>null</c> or empty.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="name"/> is not a valid identifier for a <see cref="StaMa.Transition"/>: The identifier of a <see cref="StaMa.Transition"/> must start with a character followed by nonspace characters or digits.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="name"/> is not a unique: A <see cref="StaMa.Transition"/> with the same name already exists within the <see cref="StateMachineTemplate"/> instance.
        /// </exception>
        /// <exception cref="StateMachineException">
        /// A syntax error was detected with this or the preceeding Region, EndRegion, State, EndState or Transition statements.
        /// </exception>
        public void Transition(string name,
            string[] sourceStates,
            string targetState,
            object triggerEvent)
        {
            this.TransitionInternal(name,
                sourceStates, new string[] { targetState },
                triggerEvent,
                null, null);
        }


        /// <summary>
        /// Defines a transition emanating from state defined through
        /// the enclosing <see cref="State(string)"/>...<see cref="EndState"/> calls.
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="StaMa.Transition"/> to be created. Must be unique within the <see cref="StateMachineTemplate"/>.
        /// </param>
        /// <param name="sourceState">
        /// A state name if the transition source is a single state.
        /// </param>
        /// <param name="targetStates">
        /// A list of names of the transition target states.
        /// </param>
        /// <param name="triggerEvent">
        /// A <see cref="System.Object"/> that represents the trigger event that will execute the transition or <c>null</c> to indicate an "any" transition.
        /// See also <see cref="StateMachine.SendTriggerEvent(object)"/>.
        /// </param>
        /// <remarks>
        /// <para>
        /// The source state may only reference a state or state configuration of sub-states of the anchor state.
        /// </para>
        /// <para>
        /// The target state may reference any state or state configuration within the state machine template.
        /// </para>
        /// <para>
        /// Usually the source state will be identical with the name of the <see cref="StaMa.State"/> defined
        /// through the enclosing <see cref="State(string)"/> call. In some cases it may be useful to
        /// define a transition on a higher hierarchical level in order to raise its priority above other
        /// transitions that are handled premptive due to their hierarchy level. For such cases
        /// the <paramref name="sourceState"/> may reference a <see cref="StaMa.State"/> on a hierarchical lower
        /// layer than the immediate <see cref="StaMa.State"/> parent.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="name"/> is <c>null</c> or empty.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="name"/> is not a valid identifier for a <see cref="StaMa.Transition"/>: The identifier of a <see cref="StaMa.Transition"/> must start with a character followed by nonspace characters or digits.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="name"/> is not a unique: A <see cref="StaMa.Transition"/> with the same name already exists within the <see cref="StateMachineTemplate"/> instance.
        /// </exception>
        /// <exception cref="StateMachineException">
        /// A syntax error was detected with this or the preceeding Region, EndRegion, State, EndState or Transition statements.
        /// </exception>
        public void Transition(string name,
            string sourceState,
            string[] targetStates,
            object triggerEvent)
        {
            this.TransitionInternal(name,
                new string[] { sourceState }, targetStates,
                triggerEvent,
                null, null);
        }


        /// <summary>
        /// Defines a transition emanating from state defined through
        /// the enclosing <see cref="State(string)"/>...<see cref="EndState"/> calls.
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="StaMa.Transition"/> to be created. Must be unique within the <see cref="StateMachineTemplate"/>.
        /// </param>
        /// <param name="sourceStates">
        /// A list of names of the transition source states.
        /// </param>
        /// <param name="targetStates">
        /// A list of names of the transition target states.
        /// </param>
        /// <param name="triggerEvent">
        /// A <see cref="System.Object"/> that represents the trigger event that will execute the transition or <c>null</c> to indicate an "any" transition.
        /// See also <see cref="StateMachine.SendTriggerEvent(object)"/>.
        /// </param>
        /// <remarks>
        /// <para>
        /// The source state may only reference a state or state configuration of sub-states of the anchor state.
        /// </para>
        /// <para>
        /// The target state may reference any state or state configuration within the state machine template.
        /// </para>
        /// <para>
        /// Usually the source state will be identical with the name of the <see cref="StaMa.State"/> defined
        /// through the enclosing <see cref="State(string)"/> call. In some cases it may be useful to
        /// define a transition on a higher hierarchical level in order to raise its priority above other
        /// transitions that are handled premptive due to their hierarchy level. For such cases
        /// the <paramref name="sourceStates"/> may reference a set of <see cref="StaMa.State"/> instances on a hierarchical lower
        /// layer than the immediate <see cref="StaMa.State"/> parent.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="name"/> is <c>null</c> or empty.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="name"/> is not a valid identifier for a <see cref="StaMa.Transition"/>: The identifier of a <see cref="StaMa.Transition"/> must start with a character followed by nonspace characters or digits.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="name"/> is not a unique: A <see cref="StaMa.Transition"/> with the same name already exists within the <see cref="StateMachineTemplate"/> instance.
        /// </exception>
        /// <exception cref="StateMachineException">
        /// A syntax error was detected with this or the preceeding Region, EndRegion, State, EndState or Transition statements.
        /// </exception>
        public void Transition(string name,
            string[] sourceStates,
            string[] targetStates,
            object triggerEvent)
        {
            this.TransitionInternal(name,
                sourceStates, targetStates,
                triggerEvent,
                null, null);
        }


#if !STAMA_COMPATIBLE21
        /// <summary>
        /// Creates a <see cref="StaMa.Transition"/> that starts at the <see cref="StaMa.State"/> defined by the enclosing <see cref="StateMachineTemplate"/>.<see cref="O:StaMa.StateMachineTemplate.State"/>...<see cref="EndState"/> statements.
        /// </summary>
        /// <param name="name">
        /// A name that identifies the <see cref="StaMa.Transition"/> for debugging and tracing purposes. Must be unique within the <see cref="StateMachineTemplate"/>.
        /// </param>
        /// <param name="targetState">
        /// The name of a <see cref="StaMa.State"/> instance that defines the target-<see cref="StaMa.State"/> of the <see cref="StaMa.Transition"/>.
        /// </param>
        /// <param name="triggerEvent">
        /// A <see cref="System.Object"/> that represents the trigger event that will execute the transition or <c>null</c> to indicate an "any" transition.
        /// See also <see cref="StateMachine.SendTriggerEvent(object)"/>.
        /// </param>
        /// <param name="guard">
        /// A <see cref="StateMachineGuardCallback"/> delegate that provides additional conditions
        /// for the transition; otherwise, <c>null</c> if no addititional conditions are neccessary.
        /// </param>
        /// <param name="transitionAction">
        /// A <see cref="StateMachineActionCallback"/> delegate that will be called when
        /// the transition is executed.
        /// May be <c>null</c> if no transition action is required.
        /// </param>
        /// <remarks>
        /// <para>
        /// The <paramref name="targetState"/> may reference any <see cref="StaMa.State"/> within this <see cref="StateMachineTemplate"/>.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="name"/> is <c>null</c> or empty.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="name"/> is not a valid identifier for a <see cref="StaMa.Transition"/>: The identifier of a <see cref="StaMa.Transition"/> must start with a character followed by nonspace characters or digits.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="name"/> is not a unique: A <see cref="StaMa.Transition"/> with the same name already exists within the <see cref="StateMachineTemplate"/> instance.
        /// </exception>
        /// <exception cref="StateMachineException">
        /// A syntax error was detected with this or the preceeding Region, EndRegion, State, EndState or Transition statements.
        /// </exception>
        public void Transition(
            string name,
            string targetState,
            object triggerEvent,
            StateMachineGuardCallback guard,
            StateMachineActionCallback transitionAction)
        {
            this.TransitionInternal(name, 
                            null, new string[] { targetState },
                            triggerEvent,
                            guard, transitionAction);
        }


        /// <summary>
        /// Creates a <see cref="StaMa.Transition"/> that starts at the <see cref="StaMa.State"/> defined by the enclosing <see cref="StateMachineTemplate"/>.<see cref="O:StaMa.StateMachineTemplate.State"/>...<see cref="EndState"/> statements.
        /// </summary>
        /// <param name="name">
        /// A name that identifies the <see cref="StaMa.Transition"/> for debugging and tracing purposes. Must be unique within the <see cref="StateMachineTemplate"/>.
        /// </param>
        /// <param name="targetStates">
        /// A list of names of <see cref="StaMa.State"/> instances that define the target-<see cref="StaMa.State"/> instances of the <see cref="StaMa.Transition"/>.
        /// The <paramref name="targetStates"/> collection must form a valid <see cref="StaMa.State"/> configuration within this <see cref="StateMachineTemplate"/>.
        /// </param>
        /// <param name="triggerEvent">
        /// A <see cref="System.Object"/> that represents the trigger event that will execute the transition or <c>null</c> to indicate an "any" transition.
        /// See also <see cref="StateMachine.SendTriggerEvent(object)"/>.
        /// </param>
        /// <param name="guard">
        /// A <see cref="StateMachineGuardCallback"/> delegate that provides additional conditions
        /// for the transition; otherwise, <c>null</c> if no addititional conditions are neccessary.
        /// </param>
        /// <param name="transitionAction">
        /// A <see cref="StateMachineActionCallback"/> delegate that will be called when
        /// the transition is executed.
        /// May be <c>null</c> if no transition action is required.
        /// </param>
        /// <remarks>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="name"/> is <c>null</c> or empty.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="name"/> is not a valid identifier for a <see cref="StaMa.Transition"/>: The identifier of a <see cref="StaMa.Transition"/> must start with a character followed by nonspace characters or digits.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="name"/> is not a unique: A <see cref="StaMa.Transition"/> with the same name already exists within the <see cref="StateMachineTemplate"/> instance.
        /// </exception>
        /// <exception cref="StateMachineException">
        /// A syntax error was detected with this or the preceeding Region, EndRegion, State, EndState or Transition statements.
        /// </exception>
        public void Transition(
            string name,
            string[] targetStates,
            object triggerEvent,
            StateMachineGuardCallback guard,
            StateMachineActionCallback transitionAction)
        {
            this.TransitionInternal(name,
                            null, targetStates,
                            triggerEvent,
                            guard, transitionAction);
        }


        /// <summary>
        /// Defines a transition emanating from state defined through
        /// the enclosing <see cref="State(string)"/>...<see cref="EndState"/> calls.
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="StaMa.Transition"/> to be created. Must be unique within the <see cref="StateMachineTemplate"/>.
        /// </param>
        /// <param name="sourceState">
        /// A state name if the transition source is a single state.
        /// </param>
        /// <param name="targetState">
        /// A state name if the transition target is a single state.
        /// </param>
        /// <param name="triggerEvent">
        /// A <see cref="System.Object"/> that represents the trigger event that will execute the transition or <c>null</c> to indicate an "any" transition.
        /// See also <see cref="StateMachine.SendTriggerEvent(object)"/>.
        /// </param>
        /// <param name="guard">
        /// A <see cref="StateMachineGuardCallback"/> delegate that provides additional conditions
        /// for the transition; otherwise, <c>null</c> if no addititional conditions are neccessary.
        /// </param>
        /// <param name="transitionAction">
        /// A <see cref="StateMachineActionCallback"/> delegate that will be called when
        /// the transition is executed.
        /// May be <c>null</c> if no transition action is required.
        /// </param>
        /// <remarks>
        /// <para>
        /// The source state may only reference a state or state configuration of sub-states of the anchor state.
        /// </para>
        /// <para>
        /// The target state may reference any state or state configuration within the state machine template.
        /// </para>
        /// <para>
        /// Usually the source state will be identical with the name of the <see cref="StaMa.State"/> defined
        /// through the enclosing <see cref="State(string)"/> call. In some cases it may be useful to
        /// define a transition on a higher hierarchical level in order to raise its priority above other
        /// transitions that are handled premptive due to their hierarchy level. For such cases
        /// the <paramref name="sourceState"/> may reference a <see cref="StaMa.State"/> on a hierarchical lower
        /// layer than the immediate <see cref="StaMa.State"/> parent.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="name"/> is <c>null</c> or empty.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="name"/> is not a valid identifier for a <see cref="StaMa.Transition"/>: The identifier of a <see cref="StaMa.Transition"/> must start with a character followed by nonspace characters or digits.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="name"/> is not a unique: A <see cref="StaMa.Transition"/> with the same name already exists within the <see cref="StateMachineTemplate"/> instance.
        /// </exception>
        /// <exception cref="StateMachineException">
        /// A syntax error was detected with this or the preceeding Region, EndRegion, State, EndState or Transition statements.
        /// </exception>
        public void Transition(
            string name,
            string sourceState,
            string targetState,
            object triggerEvent,
            StateMachineGuardCallback guard,
            StateMachineActionCallback transitionAction)
#else
        /// <summary>
        /// Obsolete. The term "activity" is improper and has been replaced with "action". Please use instead the Transition method with consistent naming.
        /// </summary>
        [Obsolete("The term \"activity\" is improper and has been replaced with \"action\". Please use instead the Transition method with consistent naming.")]
        public void Transition(
            string name,
            string sourceState,
            string targetState,
            object triggerEvent,
            StateMachineGuardCallback guard,
            StateMachineActivityCallback transitionAction)
#endif
        {
            this.TransitionInternal(name, 
                            new string[] { sourceState }, new string[] { targetState },
                            triggerEvent,
                            guard, transitionAction);
        }


#if !STAMA_COMPATIBLE21
        /// <summary>
        /// Defines a transition emanating from state defined through
        /// the enclosing <see cref="State(string)"/>...<see cref="EndState"/> calls.
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="StaMa.Transition"/> to be created. Must be unique within the <see cref="StateMachineTemplate"/>.
        /// </param>
        /// <param name="sourceState">
        /// A state name if the transition source is a single state.
        /// </param>
        /// <param name="targetStates">
        /// A list of names of the transition target states.
        /// </param>
        /// <param name="triggerEvent">
        /// A <see cref="System.Object"/> that represents the trigger event that will execute the transition or <c>null</c> to indicate an "any" transition.
        /// See also <see cref="StateMachine.SendTriggerEvent(object)"/>.
        /// </param>
        /// <param name="guard">
        /// A <see cref="StateMachineGuardCallback"/> delegate that provides additional conditions
        /// for the transition; otherwise, <c>null</c> if no addititional conditions are neccessary.
        /// </param>
        /// <param name="transitionAction">
        /// A <see cref="StateMachineActionCallback"/> delegate that will be called when
        /// the transition is executed.
        /// May be <c>null</c> if no transition action is required.
        /// </param>
        /// <remarks>
        /// <para>
        /// The source state may only reference a state or state configuration of sub-states of the anchor state.
        /// </para>
        /// <para>
        /// The target state may reference any state or state configuration within the state machine template.
        /// </para>
        /// <para>
        /// Usually the source state will be identical with the name of the <see cref="StaMa.State"/> defined
        /// through the enclosing <see cref="State(string)"/> call. In some cases it may be useful to
        /// define a transition on a higher hierarchical level in order to raise its priority above other
        /// transitions that are handled premptive due to their hierarchy level. For such cases
        /// the <paramref name="sourceState"/> may reference a <see cref="StaMa.State"/> on a hierarchical lower
        /// layer than the immediate <see cref="StaMa.State"/> parent.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="name"/> is <c>null</c> or empty.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="name"/> is not a valid identifier for a <see cref="StaMa.Transition"/>: The identifier of a <see cref="StaMa.Transition"/> must start with a character followed by nonspace characters or digits.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="name"/> is not a unique: A <see cref="StaMa.Transition"/> with the same name already exists within the <see cref="StateMachineTemplate"/> instance.
        /// </exception>
        /// <exception cref="StateMachineException">
        /// A syntax error was detected with this or the preceeding Region, EndRegion, State, EndState or Transition statements.
        /// </exception>
        public void Transition(
            string name,
            string sourceState,
            string[] targetStates,
            object triggerEvent,
            StateMachineGuardCallback guard,
            StateMachineActionCallback transitionAction)
#else
        /// <summary>
        /// Obsolete. The term "activity" is improper and has been replaced with "action". Please use instead the Transition method with consistent naming.
        /// </summary>
        [Obsolete("The term \"activity\" is improper and has been replaced with \"action\". Please use instead the Transition method with consistent naming.")]
        public void Transition(
            string name,
            string sourceState,
            string[] targetStates,
            object triggerEvent,
            StateMachineGuardCallback guard,
            StateMachineActivityCallback transitionAction)
#endif
        {
            this.TransitionInternal(name, 
                new string[] { sourceState }, targetStates,
                triggerEvent,
                guard, transitionAction);
        }


#if !STAMA_COMPATIBLE21
        /// <summary>
        /// Defines a transition emanating from state defined through
        /// the enclosing <see cref="State(string)"/>...<see cref="EndState"/> calls.
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="StaMa.Transition"/> to be created. Must be unique within the <see cref="StateMachineTemplate"/>.
        /// </param>
        /// <param name="sourceStates">
        /// A list of names of the transition source states.
        /// </param>
        /// <param name="targetState">
        /// A state name if the transition target is a single state.
        /// </param>
        /// <param name="triggerEvent">
        /// A <see cref="System.Object"/> that represents the trigger event that will execute the transition or <c>null</c> to indicate an "any" transition.
        /// See also <see cref="StateMachine.SendTriggerEvent(object)"/>.
        /// </param>
        /// <param name="guard">
        /// A <see cref="StateMachineGuardCallback"/> delegate that provides additional conditions
        /// for the transition; otherwise, <c>null</c> if no addititional conditions are neccessary.
        /// </param>
        /// <param name="transitionAction">
        /// A <see cref="StateMachineActionCallback"/> delegate that will be called when
        /// the transition is executed.
        /// May be <c>null</c> if no transition action is required.
        /// </param>
        /// <remarks>
        /// <para>
        /// The source state may only reference a state or state configuration of sub-states of the anchor state.
        /// </para>
        /// <para>
        /// The target state may reference any state or state configuration within the state machine template.
        /// </para>
        /// <para>
        /// Usually the source state will be identical with the name of the <see cref="StaMa.State"/> defined
        /// through the enclosing <see cref="State(string)"/> call. In some cases it may be useful to
        /// define a transition on a higher hierarchical level in order to raise its priority above other
        /// transitions that are handled premptive due to their hierarchy level. For such cases
        /// the <paramref name="sourceStates"/> may reference a set of <see cref="StaMa.State"/> instances on a hierarchical lower
        /// layer than the immediate <see cref="StaMa.State"/> parent.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="name"/> is <c>null</c> or empty.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="name"/> is not a valid identifier for a <see cref="StaMa.Transition"/>: The identifier of a <see cref="StaMa.Transition"/> must start with a character followed by nonspace characters or digits.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="name"/> is not a unique: A <see cref="StaMa.Transition"/> with the same name already exists within the <see cref="StateMachineTemplate"/> instance.
        /// </exception>
        /// <exception cref="StateMachineException">
        /// A syntax error was detected with this or the preceeding Region, EndRegion, State, EndState or Transition statements.
        /// </exception>
        public void Transition(string name,
            string[] sourceStates,
            string targetState,
            object triggerEvent,
            StateMachineGuardCallback guard,
            StateMachineActionCallback transitionAction)
#else
        /// <summary>
        /// Obsolete. The term "activity" is improper and has been replaced with "action". Please use instead the Transition method with consistent naming.
        /// </summary>
        [Obsolete("The term \"activity\" is improper and has been replaced with \"action\". Please use instead the Transition method with consistent naming.")]
        public void Transition(string name,
            string[] sourceStates,
            string targetState,
            object triggerEvent,
            StateMachineGuardCallback guard,
            StateMachineActivityCallback transitionAction)
#endif
        {
            this.TransitionInternal(name, 
                sourceStates, new string[] { targetState },
                triggerEvent,
                guard, transitionAction);
        }


#if !STAMA_COMPATIBLE21
        /// <summary>
        /// Defines a transition emanating from state defined through
        /// the enclosing <see cref="State(string)"/>...<see cref="EndState"/> calls.
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="StaMa.Transition"/> to be created. Must be unique within the <see cref="StateMachineTemplate"/>.
        /// </param>
        /// <param name="sourceStates">
        /// A list of names of the transition source states.
        /// </param>
        /// <param name="targetStates">
        /// A list of names of the transition target states.
        /// </param>
        /// <param name="triggerEvent">
        /// A <see cref="System.Object"/> that represents the trigger event that will execute the transition or <c>null</c> to indicate an "any" transition.
        /// See also <see cref="StateMachine.SendTriggerEvent(object)"/>.
        /// </param>
        /// <param name="guard">
        /// A <see cref="StateMachineGuardCallback"/> delegate that provides additional conditions
        /// for the transition; otherwise, <c>null</c> if no addititional conditions are neccessary.
        /// </param>
        /// <param name="transitionAction">
        /// A <see cref="StateMachineActionCallback"/> delegate that will be called when
        /// the transition is executed.
        /// May be <c>null</c> if no transition action is required.
        /// </param>
        /// <remarks>
        /// <para>
        /// The source state may only reference a state or state configuration of sub-states of the anchor state.
        /// </para>
        /// <para>
        /// The target state may reference any state or state configuration within the state machine template.
        /// </para>
        /// <para>
        /// Usually the source state will be identical with the name of the <see cref="StaMa.State"/> defined
        /// through the enclosing <see cref="State(string)"/> call. In some cases it may be useful to
        /// define a transition on a higher hierarchical level in order to raise its priority above other
        /// transitions that are handled premptive due to their hierarchy level. For such cases
        /// the <paramref name="sourceStates"/> may reference a set of <see cref="StaMa.State"/> instances on a hierarchical lower
        /// layer than the immediate <see cref="StaMa.State"/> parent.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="name"/> is <c>null</c> or empty.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="name"/> is not a valid identifier for a <see cref="StaMa.Transition"/>: The identifier of a <see cref="StaMa.Transition"/> must start with a character followed by nonspace characters or digits.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="name"/> is not a unique: A <see cref="StaMa.Transition"/> with the same name already exists within the <see cref="StateMachineTemplate"/> instance.
        /// </exception>
        /// <exception cref="StateMachineException">
        /// A syntax error was detected with this or the preceeding Region, EndRegion, State, EndState or Transition statements.
        /// </exception>
        public void Transition(string name,
                               string[] sourceStates,
                               string[] targetStates,
                               object triggerEvent,
                               StateMachineGuardCallback guard,
                               StateMachineActionCallback transitionAction)
#else
        /// <summary>
        /// Obsolete. The term "activity" is improper and has been replaced with "action". Please use instead the Transition method with consistent naming.
        /// </summary>
        [Obsolete("The term \"activity\" is improper and has been replaced with \"action\". Please use instead the Transition method with consistent naming.")]
        public void Transition(string name,
                               string[] sourceStates,
                               string[] targetStates,
                               object triggerEvent,
                               StateMachineGuardCallback guard,
                               StateMachineActivityCallback transitionAction)
#endif
        {
            if (sourceStates == null)
            {
                throw new ArgumentNullException("sourceStates");
            }
            // Validity of other arguments will be checked within Transition ctor.

            this.TransitionInternal(name,
                sourceStates, targetStates,
                triggerEvent,
                guard, transitionAction);
        }

        
#if !STAMA_COMPATIBLE21
        private void TransitionInternal(string name,
                               string[] sourceStates,
                               string[] targetStates,
                               object triggerEvent,
                               StateMachineGuardCallback guard,
                               StateMachineActionCallback transitionAction)
#else
        private void TransitionInternal(string name,
                               string[] sourceStates,
                               string[] targetStates,
                               object triggerEvent,
                               StateMachineGuardCallback guard,
                               StateMachineActivityCallback transitionAction)
#endif
        {
            if (this.Root != null)
            {
                throw new StateMachineException("This StateMachineTemplate is already complete, no further template element insertions are allowed.");
            }

            State parentState = m_stateStack.Peek();
            if (parentState == null)
            {
                // Not allowed to start template with "Transition"
                throw new StateMachineException("Syntax error: A \"Transition()\" statement must be embedded into a \"State()\"...\"EndState()\" sequence.");
            }

            if (parentState.Parent != m_regionStack.Peek())
            {
                // Not allowed to use "Transition" within "Region...EndRegion".
                throw new StateMachineException("Syntax error: A \"Transition()\" statement must be embedded into a \"State()\"...\"EndState()\" sequence.");
            }

            // Check if name is a valid identifier
            if (!IsValidIdentifier(name))
            {
                // Invalid identifier.
                throw new ArgumentOutOfRangeException("name", FrameworkAbstractionUtils.StringFormat("The Transition name \"{0}\" is invalid. A Transition name must start with a character followed by nonspace characters or digits.", name));
            }

            // Check uniqueness of transition name
            if (m_transitionDictionary.ContainsKey(name))
            {
                // Double defined transition name.
                throw new ArgumentOutOfRangeException("name", FrameworkAbstractionUtils.StringFormat("A Transition with an identical name \"{0}\" is already defined.", name));
            }

            // Infer source state from parent state in case it was not explicitly specified
            if (sourceStates == null)
            {
                sourceStates = new string[] { parentState.Name };
            }

            // Create new object and ...
            Transition transition = this.InternalCreateTransition(parentState, name,
                                                    sourceStates, targetStates,
                                                    triggerEvent, guard, transitionAction);

            // ... add it to the states's list of transis
            parentState.AddTransition(transition);

            // Remember this transition name
            m_transitionDictionary.Add(name, transition);
        }


        /// <summary>
        /// Creates a <see cref="StateMachine"/> instance from the <see cref="StateMachineTemplate"/>.
        /// </summary>
        /// <returns>
        /// A new <see cref="StateMachine"/> instance.
        /// </returns>
        /// <remarks>
        /// The <see cref="StateMachine"/> is a lightweight object composed of very few members.
        /// State names and all other runtime meta information are used per reference from the <see cref="StateMachineTemplate"/>.
        /// A <see cref="StateMachine"/> contains essentially the following data:
        /// <list type="bullet">
        /// <item> 
        /// <description>
        /// A reference to a <see cref="StaMa.State"/> that is currently the active state for this state machine.
        /// In case of orthogonal regions the active state for each orthogonal region is referenced.
        /// The active states are accessible through the <see cref="StateMachine.ActiveStateConfiguration"/> property.
        /// </description> 
        /// </item>
        /// <item> 
        /// <description>
        /// A reference to a <see cref="StaMa.State"/> for each region to keep the history.
        /// </description> 
        /// </item> 
        /// <item> 
        /// <description>
        /// A queue that intermediately caches events sent to the state machine during execution of actions.
        /// </description> 
        /// </item> 
        /// </list>
        /// <para>
        /// All further structural information needed e.g. for transition
        /// evaluation is taken from the <see cref="StateMachineTemplate"/> instance.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// A <see cref="StateMachine"/> instance cannot be created when the <see cref="StateMachineTemplate"/> is not complete.
        /// The <see cref="StateMachineTemplate.Root"/> property is not initialized to a valid <see cref="StaMa.Region"/> instance.
        /// </exception>
        public StateMachine CreateStateMachine()
        {
            return this.CreateStateMachine(null);
        }


        /// <summary>
        /// Creates a <see cref="StateMachine"/> instance from the <see cref="StateMachineTemplate"/>.
        /// </summary>
        /// <param name="context">
        /// An <see cref="object"/> that may be used to transport additional context information
        /// to the <see cref="StateMachineActionCallback"/>. The given value will be accessible through
        /// the <see cref="StateMachine.Context"/> property.
        /// </param>
        /// <returns>
        /// A new <see cref="StateMachine"/> instance.
        /// </returns>
        /// <remarks>
        /// The <see cref="StateMachine"/> is a lightweight object composed of very few members.
        /// State names and all other runtime meta information are used per reference from the <see cref="StateMachineTemplate"/>.
        /// A <see cref="StateMachine"/> contains essentially the following data:
        /// <list type="bullet">
        /// <item> 
        /// <description>
        /// A reference to a <see cref="StaMa.State"/> that is currently the active state for this state machine.
        /// In case of orthogonal regions the active state for each orthogonal region is referenced.
        /// The active states are accessible through the <see cref="StateMachine.ActiveStateConfiguration"/> property.
        /// </description> 
        /// </item>
        /// <item> 
        /// <description>
        /// A reference to a <see cref="StaMa.State"/> for each region to keep the history.
        /// </description> 
        /// </item> 
        /// <item> 
        /// <description>
        /// A queue that intermediately caches events sent to the state machine during execution of actions.
        /// </description> 
        /// </item> 
        /// </list>
        /// <para>
        /// All further structural information needed e.g. for transition
        /// evaluation is taken from the <see cref="StateMachineTemplate"/> instance.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// A <see cref="StateMachine"/> instance cannot be created when the <see cref="StateMachineTemplate"/> is not complete.
        /// The <see cref="StateMachineTemplate.Root"/> property is not initialized to a valid <see cref="StaMa.Region"/> instance.
        /// </exception>
        public StateMachine CreateStateMachine(object context)
        {
            if (m_rootRegion == null)
            {
                throw new InvalidOperationException("A StateMachine instance cannot be created when the StateMachineTemplate is not complete. StateMachineTemplate.Root is not initialized to a valid Region.");
            }
            StateMachine stateMachine = this.InternalCreateStateMachine(context);
            return stateMachine;
        }


        /// <summary>
        /// Creates a new <see cref="StateConfiguration"/> instance from a state name.
        /// </summary>
        /// <param name="stateName">
        /// A <see cref="string"/> that references a <see cref="StaMa.State"/> within
        /// this <see cref="StateMachineTemplate"/>.
        /// </param>
        /// <returns>
        /// A <see cref="StateConfiguration"/> instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="stateName"/> is <c>null</c> or empty.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// A <see cref="StateConfiguration"/> instance cannot be created when the <see cref="StateMachineTemplate"/> is not complete.
        /// The <see cref="StateMachineTemplate.Root"/> property is not initialized to a valid <see cref="StaMa.Region"/> instance.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <see cref="StaMa.State"/> instance provided through <paramref name="stateName"/> could not be found.
        /// </exception>
        public StateConfiguration CreateStateConfiguration(string stateName)
        {
            if (FrameworkAbstractionUtils.StringIsNullOrEmpty(stateName))
            {
                throw new ArgumentNullException("stateName");
            }
            return this.InternalCreateStateConfiguration(new string[] { stateName });
        }


        /// <summary>
        /// Creates a new <see cref="StateConfiguration"/> instance from a list of state names.
        /// </summary>
        /// <param name="stateBaseConfiguration">
        /// A list of <see cref="string"/> that reference <see cref="StaMa.State"/> instances within this <see cref="StateMachineTemplate"/>.
        /// </param>
        /// <returns>
        /// A <see cref="StateConfiguration"/> instance.
        /// </returns>
        /// <remarks>
        /// <para>
        /// The <see cref="StaMa.State"/> instances referenced through <paramref name="stateBaseConfiguration"/> must be part of disjoint orthogonal sub-regions.
        /// </para>
        /// <para>
        /// In absence of orthogonal sub-regions, a <see cref="StateConfiguration"/> is sufficiently defined through a single state.
        /// Passing in the parent states of the single state will not further improve or change the <see cref="StateConfiguration"/>.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="stateBaseConfiguration"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// A <see cref="StateConfiguration"/> instance cannot be created when the <see cref="StateMachineTemplate"/> is not complete.
        /// The <see cref="StateMachineTemplate.Root"/> property is not initialized to a valid <see cref="StaMa.Region"/> instance.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// At least one of the <see cref="StaMa.State"/> instances provided through <paramref name="stateBaseConfiguration"/> could not be found.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <see cref="StaMa.State"/> instances provided through <paramref name="stateBaseConfiguration"/> contain mutually exclusive <see cref="StaMa.State"/> instances,
        /// e.g. <see cref="StaMa.State"/> instances from the same <see cref="StaMa.Region"/> instance.
        /// </exception>
        public StateConfiguration CreateStateConfiguration(string[] stateBaseConfiguration)
        {
            if (stateBaseConfiguration == null)
            {
                throw new ArgumentNullException("stateBaseConfiguration");
            }
            if (m_rootRegion == null)
            {
                throw new InvalidOperationException("A StateConfiguration instance cannot be created when the StateMachineTemplate is not complete. StateMachineTemplate.Root is not initialized to a valid Region.");
            }
            return this.InternalCreateStateConfiguration(stateBaseConfiguration);
        }


        /// <summary>
        /// Provides the accept method of a visitor pattern that traverses
        /// the <see cref="StaMa.Region"/>, <see cref="StaMa.State"/> and <see cref="StaMa.Transition"/> instances
        /// of this <see cref="StateMachineTemplate"/>.
        /// </summary>
        /// <param name="visitor">
        /// A <see cref="IStateMachineTemplateVisitor"/> instance.
        /// </param>
        public void PassThrough(IStateMachineTemplateVisitor visitor)
        {
            if (visitor == null)
            {
                throw new ArgumentNullException("visitor");
            }
            this.Root.PassThrough(visitor);
        }


        /// <summary>
        /// Checks whether the given <paramref name="name"/> is a valid identifier
        /// or a cardinal number.
        /// </summary>
        /// <param name="name">
        /// The <see cref="string"/> to be checked.
        /// </param>
        /// <returns>
        /// <c>true</c> if the given <paramref name="name"/> is a valid name for
        /// a <see cref="StaMa.State"/> or <see cref="StaMa.Transition"/>.
        /// </returns>
        /// <remarks>
        /// Valid identifiers must start with a letter, digit or underscore.
        /// <para>
        /// If the identifier starts with a number, the whole identifier must be a
        /// cardinal number.
        /// </para>
        /// <para>
        /// If the identifier starts with a letter, any combination of letters,
        /// digits or underscores may follow.
        /// </para>
        /// <para>
        /// If the identifier starts with an underscore, the first letter in the
        /// string that is not an underscore must be a letter and the rest of the
        /// identifier may be any combination of letters, digits or underscores.
        /// </para>
        /// </remarks>
        public static bool IsValidIdentifier(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            return StateMachineTemplate.s_identifierRegEx.IsMatch(name);
        }


        internal State FindState(string name)
        {
            State state;
            m_stateDictionary.TryGetValue(name, out state);
            return state;
        }


        /// <summary>
        /// Creates a new state instance.
        /// </summary>
        /// <param name="parentRegion">
        /// The region that contains the state.
        /// </param>
        /// <param name="name">
        /// The name of the <see cref="StaMa.State"/> to be created. Must be unique within the <see cref="StateMachineTemplate"/>.
        /// </param>
        /// <param name="entryAction">
        /// A <see cref="StateMachineActionCallback"/> delegate that will be called when a <see cref="StateMachine"/>
        /// enters the the <see cref="StaMa.State"/>.
        /// May be <c>null</c> if no entry action is specified.
        /// </param>
        /// <param name="exitAction">
        /// A <see cref="StateMachineActionCallback"/> delegate that will be called when a <see cref="StateMachine"/>
        /// leaves the the <see cref="StaMa.State"/>.
        /// May be <c>null</c> if no exit action is specified.
        /// </param>
        /// <param name="doAction">
        /// The <see cref="StateMachineDoActionCallback"/> delegate that defines the perpetual action to be executed while the <see cref="StateMachine"/> is in this <see cref="StaMa.State"/>.
        /// May be <c>null</c> if no do action is specified.
        /// </param>
        /// <returns>
        /// A <see cref="StaMa.State"/> instance.
        /// </returns>
        protected virtual State InternalCreateState(
                Region parentRegion,
                string name,
#if !STAMA_COMPATIBLE21
                StateMachineActionCallback entryAction,
                StateMachineActionCallback exitAction,
#else
                StateMachineActivityCallback entryAction,
                StateMachineActivityCallback exitAction,
#endif
                StateMachineDoActionCallback doAction)
        {
            return new State(parentRegion, name, entryAction, exitAction, doAction, this);
        }


        /// <summary>
        /// Creates a new state collection instance.
        /// </summary>
        /// <returns>
        /// A <see cref="StateCollection"/> instance.
        /// </returns>
        internal protected virtual StateCollection InternalCreateStateCollection()
        {
            return new StateCollection();
        }


        /// <summary>
        /// Creates a new region instance.
        /// </summary>
        /// <param name="parentState">
        /// The state that contains the region.
        /// </param>
        /// <param name="initialStateName">
        /// A <see cref="System.String"/> that contains the name of the <see cref="StaMa.Region"/>'s initial <see cref="StaMa.State"/>.
        /// </param>
        /// <param name="historyIndex">
        /// If the region has a history, the index of the slot where
        /// this region stores its history in the <see cref="StateMachine"/> instance history.
        /// If the region has no history, <see cref="int.MaxValue"/>.
        /// </param>
        /// <returns>
        /// A <see cref="Region"/> instance.
        /// </returns>
        protected virtual Region InternalCreateRegion(
                State parentState,
                string initialStateName,
                int historyIndex)
        {
            return new Region(parentState, initialStateName, historyIndex, this);
        }


        /// <summary>
        /// Creates a new region collection instance.
        /// </summary>
        /// <returns>
        /// A <see cref="RegionCollection"/> instance.
        /// </returns>
        internal protected virtual RegionCollection InternalCreateRegionCollection()
        {
            return new RegionCollection();
        }


        /// <summary>
        /// Creates a new transition instance.
        /// </summary>
        /// <param name="parentState">
        /// The state that contains the transition.
        /// </param>
        /// <param name="name">
        /// The name of the <see cref="StaMa.Transition"/> to be created. Must be unique within the <see cref="StateMachineTemplate"/>.
        /// </param>
        /// <param name="sourceStates">
        /// A list of names of the transition source states.
        /// </param>
        /// <param name="targetStates">
        /// A list of names of the transition target states.
        /// </param>
        /// <param name="triggerEvent">
        /// A <see cref="System.Object"/> that represents the trigger event that will execute the transition or <c>null</c> to indicate an "any" transition.
        /// See also <see cref="StateMachine.SendTriggerEvent(object)"/>.
        /// </param>
        /// <param name="guard">
        /// A <see cref="StateMachineGuardCallback"/> delegate that provides additional conditions
        /// for the transition; otherwise, <c>null</c> if no addititional conditions are neccessary.
        /// </param>
        /// <param name="transitionAction">
        /// A <see cref="StateMachineActionCallback"/> delegate that will be called when
        /// the transition is executed.
        /// May be <c>null</c> if no transition action is required.
        /// </param>
        /// <returns>
        /// A <see cref="StaMa.Transition"/> instance.
        /// </returns>
        protected virtual Transition InternalCreateTransition(
            State parentState,
            string name,
            string[] sourceStates,
            string[] targetStates,
            object triggerEvent,
            StateMachineGuardCallback guard,
#if !STAMA_COMPATIBLE21
            StateMachineActionCallback transitionAction)
#else
            StateMachineActivityCallback transitionAction)
#endif
        {
            return new Transition(parentState, name, sourceStates, targetStates, triggerEvent, guard, transitionAction);
        }


        /// <summary>
        /// Creates a new transition collection instance.
        /// </summary>
        /// <returns>
        /// A <see cref="TransitionCollection"/> instance.
        /// </returns>
        internal protected virtual TransitionCollection InternalCreateTransitionCollection()
        {
            return new TransitionCollection();
        }


        /// <summary>
        /// Creates a new state configuration instance based on a state machine template.
        /// </summary>
        /// <param name="stateBaseConfiguration">
        /// A list of <see cref="string"/> that reference <see cref="StaMa.State"/> instances within
        /// this <see cref="StateMachineTemplate"/>.
        /// </param>
        /// <returns>
        /// A <see cref="StateConfiguration"/> instance.
        /// </returns>
        internal protected virtual StateConfiguration InternalCreateStateConfiguration(
                string[] stateBaseConfiguration)
        {
            return new StateConfiguration(this, stateBaseConfiguration);
        }


        /// <summary>
        /// Creates a new <see cref="StateMachine"/> instance based on a <see cref="StateMachineTemplate"/>.
        /// </summary>
        /// <param name="context">
        /// A <see cref="object"/> that may be used to transport additional context information
        /// to the <see cref="StateMachineActionCallback"/>. The given value will be accessible through
        /// the <see cref="StateMachine.Context"/> property.
        /// </param>
        /// <returns>
        /// A <see cref="StateMachine"/> instance.
        /// </returns>
        protected virtual StateMachine InternalCreateStateMachine(
            object context)
        {
            return new StateMachine(this, context);
        }



#if !MF_FRAMEWORK
        private class RegionStack : Stack<Region>
        {
        }

        private class StateStack : Stack<State>
        {
        }

        private class StateDictionary : Dictionary<string, State>
        {
        }

        private class TransitionDictionary : Dictionary<string, Transition>
        {
        }
#else
        private class RegionStack
        {
            private Stack m_stack;


            public RegionStack()
            {
                m_stack = new Stack();
            }


            public Region Peek()
            {
                return (Region)m_stack.Peek();
            }


            public Region Pop()
            {
                return (Region)m_stack.Pop();
            }


            public void Push(Region item)
            {
                m_stack.Push(item);
            }
        }


        private class StateStack
        {
            private Stack m_stack;


            public StateStack()
            {
                m_stack = new Stack();
            }


            public State Peek()
            {
                return (State)m_stack.Peek();
            }


            public State Pop()
            {
                return (State)m_stack.Pop();
            }


            public void Push(State item)
            {
                m_stack.Push(item);
            }
        }


        private class StateDictionary
        {
            private Hashtable m_dictionary;

            
            public StateDictionary()
            {
                m_dictionary = new Hashtable();
            }


            public void Add(String key, State value)
            {
                m_dictionary.Add(key, value);
            }


            public bool ContainsKey(String key)
            {
                return m_dictionary.Contains(key);
            }


            public bool TryGetValue(String key, out State state)
            {
                state = (State)m_dictionary[key];
                return (state != null);
            }
        }


        private class TransitionDictionary
        {
            private Hashtable m_dictionary;


            public TransitionDictionary()
            {
                m_dictionary = new Hashtable();
            }


            public void Add(String key, Transition value)
            {
                m_dictionary.Add(key, value);
            }


            public bool ContainsKey(String key)
            {
                return m_dictionary.Contains(key);
            }


            public bool TryGetValue(String key, out Transition transition)
            {
                transition = (Transition)m_dictionary[key];
                return (transition != null);
            }
        }
#endif


        private class SignatureCollector : IStateMachineTemplateVisitor
        {
            private System.Text.StringBuilder m_templateStructure;

            public SignatureCollector()
            {
                m_templateStructure = new System.Text.StringBuilder();
            }

            public String TemplateStructure
            {
                get
                {
                    return m_templateStructure.ToString();
                }
            }

#if !MF_FRAMEWORK
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "SignatureCollector is a private class.")]
#endif
            public bool State(State state)
            {
                m_templateStructure.Append(state.Name);
                return true;
            }

            public bool EndState(State state)
            {
                m_templateStructure.Append(',');
                return true;
            }

#if !MF_FRAMEWORK
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "SignatureCollector is a private class.")]
#endif
            public bool Region(Region region)
            {
                m_templateStructure.Append("(" + region.Name + "" + (region.HasHistory ? '#' : '-') + '{');
                return true;
            }

            public bool EndRegion(Region region)
            {
                m_templateStructure.Append("})");
                return true;
            }

            public bool Transition(Transition transition)
            {
                return true;
            }
        }
    }
}

#if STAMA_COMPATIBLE21
#pragma warning restore 0618
#endif
