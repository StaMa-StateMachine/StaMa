#region StateConfiguration.cs file
//
// StaMa state machine controller library, http://stama.codeplex.com/
//
// Copyright (c) 2005-2014, Roland Schneider. All rights reserved.
//
#endregion

using System;
using System.Text;
using System.Reflection;
using System.Diagnostics;


namespace StaMa
{
    /// <summary>
    /// Specifies a state configuration in a structured way.
    /// </summary>
    /// <remarks>
    /// Every <see cref="Region"/> from the <see cref="StateMachineTemplate"/> occupies a "slot" within the
    /// <see cref="StateConfiguration"/>. These slots are filled with references to <see cref="State"/> instances or
    /// may be left unspecified during initialization of the <see cref="StateConfiguration"/>.
    /// A <see cref="StateConfiguration"/> that contains unspecified slots is partially specified but may
    /// still be compared for compliance with other <see cref="StateConfiguration"/> instances through the
    /// <see cref="StateConfiguration.IsMatching(StateConfiguration)"/> method.
    /// </remarks>
    [DebuggerDisplay("StateConfiguration {ToString()}")]
    public class StateConfiguration
    {
        private StateMachineTemplate m_stateMachineTemplate;
        private State[] m_states;


        /// <summary>
        /// Initializes a new <see cref="StateConfiguration"/> instance.
        /// </summary>
        /// <param name="stateMachineTemplate">
        /// The <see cref="StateMachineTemplate"/> to which this <see cref="StateConfiguration"/> applies.
        /// </param>
        /// <param name="stateBaseConfiguration">
        /// A plain list of <see cref="string"/>s that contain the names of the base <see cref="State"/> instances that shall compose the <see cref="StateConfiguration"/>.
        /// </param>
        /// <remarks>
        /// <para>
        /// The states referenced through <paramref name="stateBaseConfiguration"/> must be part of disjoint orthogonal sub-regions.
        /// </para>
        /// <para>
        /// In absence of orthogonal sub-regions, a <see cref="StateConfiguration"/> is sufficiently defined through a single state.
		/// Passing in parent states of a single state will not change the <see cref="StateConfiguration"/>.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// At least one of the states provided through <paramref name="stateBaseConfiguration"/> could not be found.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The states provided through <paramref name="stateBaseConfiguration"/> contains mutually exclusive states.
        /// </exception>
#if !MF_FRAMEWORK
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Method is internal.")]
#endif
        internal protected StateConfiguration(StateMachineTemplate stateMachineTemplate, string[] stateBaseConfiguration)
        {
            m_stateMachineTemplate = stateMachineTemplate;
            m_states = new State[stateMachineTemplate.StateConfigurationMax];
            // Array is initialized with null references.

            if (stateBaseConfiguration != null)
            {
                Initialize(stateBaseConfiguration);
            }
        }


        internal void Initialize(string[] stateBaseConfiguration)
        {
            this.SetNirwana();

            // For all source states do walk up the tree and set the state configuration entries
            foreach (string stateName in stateBaseConfiguration)
            {
                State state = m_stateMachineTemplate.FindState(stateName);
                if (state == null)
                {
                    // There is no state corresponding to this transi's stateId
                    throw new ArgumentOutOfRangeException("stateBaseConfiguration", FrameworkAbstractionUtils.StringFormat("Cannot find the State \"{0}\".", stateName));
                }

                while (state != null)
                {
                    Region parentRegion = state.Parent;

                    int stateConfigurationIndex = parentRegion.StateConfigurationIndex;
                    State state0 = this.GetState(parentRegion);

                    // If a previous "foreach" stateName has already entered a value,
                    // check whether that one matches with the current value.
                    if ((state0 != State.Wildcard) && (state0 != state))
                    {
                        // Mutual exclusive definition of source states
                        throw new ArgumentOutOfRangeException("stateBaseConfiguration", FrameworkAbstractionUtils.StringFormat("The list of states contains mutually exclusive states \"{0}\" and \"{1}\".", stateName, state0.Name));
                    }

                    m_states[stateConfigurationIndex] = state;

                    // Step up one level
                    state = parentRegion.Parent;
                }
            }
        }


        internal State GetState(Region region)
        {
            return m_states[region.StateConfigurationIndex];
        }


        internal State FindEnvelopeState()
        {
            return this.FindEnvelopeState(m_stateMachineTemplate.Root);
        }


        private State FindEnvelopeState(Region region)
        {
            // Find the topmost state which is not divided into concurrent states
            // Hint: Topmost may also be a very deep nested base state
            State state = this.GetState(region);
            
            if (state != State.Wildcard)
            {
                State stateFirst = null;

                foreach (Region subRegion in state.Regions)
                {
                    State stateRet = this.FindEnvelopeState(subRegion);
                    if (stateRet != State.Wildcard)
                    {
                        if (stateFirst == null)
                        {
                            // Found first concurrent, memorize it
                            stateFirst = stateRet;
                        }
                        else
                        {
                            // Found more than one concurrent, so
                            // 'this' level is not the one we are looking for
                            goto exit;
                        }
                    }
                }

                if (stateFirst != null)
                {
                    state = stateFirst;
                }
            }

            exit:
            return state;
        }


        /// <summary>
        /// Gets the <see cref="StateMachineTemplate"/> to which this <see cref="StateConfiguration"/> applies.
        /// </summary>
        public StateMachineTemplate Template
        {
            get
            {
                return m_stateMachineTemplate;
            }
        }


        /// <summary>
        /// Indicates whether a <see cref="StateConfiguration"/> is compliant with this instance.
        /// </summary>
        /// <param name="stateConfiguration">
        /// The <see cref="StateConfiguration"/> to be checked.
        /// </param>
        /// <returns>
        /// <c>true</c> if the given <see cref="StateConfiguration"/> is exactly identical or if it
        /// differs only in those slots where the other <see cref="StateConfiguration"/> isn't specified for a particular <see cref="Region"/>.
        /// <c>false</c> if there are different states for the same <see cref="Region"/> slot.
        /// </returns>
        /// <remarks>
        /// Every <see cref="Region"/> from the <see cref="StateMachineTemplate"/> occupies a "slot" within the
        /// <see cref="StateConfiguration"/>. These slots are filled with references to <see cref="State"/> instances or
        /// may be left unspecified during initialization of the <see cref="StateConfiguration"/>.
        /// A <see cref="StateConfiguration"/> that contains unspecified slots is partially specified but may
        /// still be compared for compliance with other <see cref="StateConfiguration"/> instances through the
        /// <see cref="StateConfiguration.IsMatching(StateConfiguration)"/> method.
        /// </remarks>
        public bool IsMatching(StateConfiguration stateConfiguration)
        {
            return this.IsMatching(stateConfiguration, m_stateMachineTemplate.Root);
        }


        internal bool IsMatching(StateConfiguration stateConfigurationRhs, Region subRegionToCompare)
        {
            bool isSameAs;
            State stateLhs = this.GetState(subRegionToCompare);
            State stateRhs = stateConfigurationRhs.GetState(subRegionToCompare);

            if ((stateLhs != State.Wildcard) && (stateRhs != State.Wildcard))
            {
                if (stateLhs == stateRhs)
                {
                    isSameAs = true;

                    // Bear in mind that stateLhs and stateRhs are identical,
                    // thus it doesn't matter which of two state's Regions list we take for checking the subregions
                    foreach (Region subRegion in stateLhs.Regions)
                    {
                        isSameAs = this.IsMatching(stateConfigurationRhs, subRegion);
                        if (!isSameAs)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    isSameAs = false;
                }
            }
            else
            {
                isSameAs = true;
            }

            return isSameAs;
        }


        /// <summary>
        /// Returns a human readable representation for the <see cref="StateConfiguration"/>.
        /// </summary>
        /// <returns>
        /// <para>
        /// In case of a flat state machine this method simply returns the single state name that was passed in at the constructor.
        /// </para>
        /// <para>
        /// In case of a hierarchical state machine the returned <see cref="String"/> is a hierachically organized concatenation of the state names passed in at the constructor.
        /// </para>
        /// <para>
        /// In Extended Backus-Naur Form (EBNF) the resulting string rule can be written as
        /// </para>
        /// <para>
        /// StateConfiguration = StateName | StateName , "(" , ConcurrentStateConfigurations, ")" ;
        /// </para>
        /// <para>
        /// ConcurrentStateConfigurations = StateConfiguration | StateConfiguration , "," , ConcurrentStateConfigurations ;
        /// </para>
        /// <para>
        /// Expressed in words, the method returns the top level state name followed by an opening bracket followed by a list of sub state names separated with commas followed by a closing bracket.
        /// </para>
        /// <para>
        /// Examples: "StateA", "StateA(SubRegionStateB)" or "StateA(NestedRegion1StateB,NestedRegion2StateC)"
        /// </para>
        /// </returns>
        public override string ToString() 
        {
            // Gracefully handle partially initialized instance in case ToString is invoked via DebuggerDisplayAttribute.
            if ((m_states == null) || (m_stateMachineTemplate == null))
            {
                return String.Empty;
            }

            DefaultTextWriter textWriter = new DefaultTextWriter();
            this.PassThrough(textWriter);
            return textWriter.ToString();
        }


        internal void InitializeAndResolve(
            StateConfiguration stateConfigurationBase,
            StateConfiguration stateConfigurationTarget,
            Region regionStart,
            State[] historyStatesRepository)
        {
            if (stateConfigurationBase != null)
            {
                // Copy from stateConfigurationBase into this.
                Array.Copy(stateConfigurationBase.m_states, m_states, m_states.Length);
                //stateConfigurationBase.m_states.CopyTo(this.m_states, 0);
            }
            else
            {
                // Initialize this items with null.
                Array.Clear(m_states, 0, m_states.Length);
            }

            this.InitializeAndResolve(stateConfigurationTarget, regionStart, historyStatesRepository);
        }


        private void InitializeAndResolve(
            StateConfiguration stateConfigurationTarget,
            Region region,
            State[] historyStatesRepository)
        {
            // Copy the model's states to the target. If the target is a wildcard, then complete the
            // information by looking in the history states or setting it to the initial state of this region

            State state = stateConfigurationTarget.GetState(region);
            if (state == State.Wildcard)
            {
                if (region.HasHistory)
                {
                    // Use previously stored history state
                    state = historyStatesRepository[region.HistoryIndex];
                }
                else
                {
                    // Use initial state
                    state = region.InitialState;
                }
            }

            if (state == null)
            {
                // Logical error: All StateConfigurationIndex elements have to be filled with valid state references
                throw new InvalidOperationException("Internal error: Either state configuration or initial state/history state not properly initialized.");
            }

            m_states[region.StateConfigurationIndex] = state;
            
            // Recurse to substates
            foreach (Region subRegion in state.Regions)
            {
                this.InitializeAndResolve(stateConfigurationTarget, subRegion, historyStatesRepository);
            }
        }


        internal bool IsNirwana()
        {
            return (m_states[0] == State.Wildcard);
        }


        internal void SetNirwana()
        {
            for (int i = 0; i < m_states.Length; i++)
            {
                m_states[i] = State.Wildcard;
            }
        }


        internal void CopyTo(StateConfiguration target)
        {
            if (target.m_stateMachineTemplate != m_stateMachineTemplate)
            {
                throw new ArgumentException("Cannot copy StateConfiguration between different StateMachineTemplate instances.", "target");
            }
            m_states.CopyTo(target.m_states, 0);
        }


        /// <summary>
        /// Creates a new <see cref="StateConfiguration"/> with identical content.
        /// </summary>
        /// <returns>
        /// A new <see cref="StateConfiguration"/> instance.
        /// </returns>
        public object Clone()
        {
            StateConfiguration stateConfig = m_stateMachineTemplate.InternalCreateStateConfiguration(null);
            m_states.CopyTo(stateConfig.m_states, 0);
            return stateConfig;
        }

        
        /// <summary>
        /// Allows to collect the names of the contained <see cref="State"/> instances in a structured way.
        /// </summary>
        /// <param name="visitor">
        /// A <see cref="IStateConfigurationVisitor"/> instance.
        /// </param>
        public void PassThrough(IStateConfigurationVisitor visitor)
        {
            this.PassThrough(visitor, m_stateMachineTemplate.Root);
        }


        private void PassThrough(IStateConfigurationVisitor visitor, Region region)
        {
            State state = this.GetState(region);
            if (state == null)
            {
                // Typically uninitialized StateConfiguration
                return;
            }

            if (state != State.Wildcard)
            {
#if !STAMA_COMPATIBLE21
                visitor.State(state);
#else
#pragma warning disable 0618
                visitor.State(state.Name);
#pragma warning restore 0618
#endif
                // Are there sub-states to traverse?
                if (state.Regions.Count > 0)
                {
                    visitor.BeginSubStates();

                    bool separatorNecessary = false;
                    foreach (Region subRegion in state.Regions)
                    {
                        // Regions shall be separated by "Next", if more than one region.
                        if (separatorNecessary)
                        {
                            visitor.NextSubState();
                        }
                        else
                        {
                            // For the first item do not emit a separator but
                            // take care of following items.
                            separatorNecessary = true;
                        }

                        // Recurse to subregions
                        this.PassThrough(visitor, subRegion);
                    }

                    visitor.EndSubStates();
                }
            }
            else
            {
                visitor.StateAny();
            }
        }


        private class DefaultTextWriter : IStateConfigurationVisitor
        {
            StringBuilder text = new StringBuilder(256);

            public override string ToString()
            {
                return text.ToString();
            }

#if !STAMA_COMPATIBLE21
#if !MF_FRAMEWORK
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "StaMa.State.Name is guaranteed to be valid through the StaMa.State constructor.")]
#endif
            public void State(State state)
            {
                text.Append(state.Name);
            }
#else
            public void State(string stateName)
            {
                text.Append(stateName);
            }
#endif

            public void StateAny()
            {
                text.Append("*");
            }

            public void BeginSubStates()
            {
                text.Append("(");
            }

            public void EndSubStates()
            {
                text.Append(")");
            }

            public void NextSubState()
            {
                text.Append(",");
            }
        }
    }
}
