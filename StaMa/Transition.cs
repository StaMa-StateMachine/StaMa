#region Transition.cs file
//
// StaMa state machine controller library, https://github.com/StaMa-StateMachine/StaMa
//
// Copyright (c) 2005-2016, Roland Schneider. All rights reserved.
//
#endregion

using System;
using System.Diagnostics;

#if STAMA_COMPATIBLE21
#pragma warning disable 0618
#endif

namespace StaMa
{
    /// <summary>
    /// Represents a transition with source state, target state and information when the transition shall be executed.
    /// </summary>
    /// <remarks>
    /// Instances of this class will be created through the <see cref="StateMachineTemplate"/>.<see cref="O:StaMa.StateMachineTemplate.Transition"/> methods.
    /// </remarks>
    [DebuggerDisplay("Name={this.Name}, Rank={1 + m_parentState.m_transitions.m_transitions.IndexOf(this)}")]
    public class Transition
    {
        private State m_parentState;
        private string m_name;
        private StateConfiguration m_sourceStateConfiguration;
        private StateConfiguration m_targetStateConfiguration;
        private object m_triggerEvent;
        private StateMachineGuardCallback m_guard;
#if !STAMA_COMPATIBLE21
        private StateMachineActionCallback m_transitionAction;
#else
        private StateMachineActivityCallback m_transitionAction;
#endif
        private Region m_leastCommonAncestor;

        private string[] m_sourceStates;
        private string[] m_targetStates;


        /// <summary>
        /// Initializes a new <see cref="Transition"/> instance.
        /// </summary>
        /// <param name="parentState">
        /// The anchor <see cref="State"/> where the <see cref="Transition"/> is aggregated.
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
        /// A <see cref="System.Object"/> that represents the trigger event that will execute the transition.
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
        /// Instances of this class will be created through the <see cref="StateMachineTemplate"/>.<see cref="StateMachineTemplate.Transition(string, string, string, object)"/> method
        /// or one of its overloads.
        /// </remarks>
        internal protected Transition(
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
            if (sourceStates == null)
            {
                throw new ArgumentNullException("sourceStates");
            }

            foreach (string sourceStateName in sourceStates)
            {
                if ((sourceStateName == null) || (sourceStateName.Length == 0))
                {
                    throw new ArgumentNullException("sourceStates", "The list of source states contains invalid items.");
                }
            }

            if (targetStates == null)
            {
                throw new ArgumentNullException("targetStates");
            }

            foreach (string targetStateName in targetStates)
            {
                if ((targetStateName == null) || (targetStateName.Length == 0))
                {
                    throw new ArgumentNullException("targetStates", "The list of target states contains invalid items.");
                }
            }

            m_parentState = parentState;
            m_name = name;
            m_sourceStateConfiguration = null;
            m_targetStateConfiguration = null;
            m_sourceStates = sourceStates;
            m_targetStates = targetStates;
            m_triggerEvent = triggerEvent;
            m_guard = guard;
            m_transitionAction = transitionAction;
            m_leastCommonAncestor = null;
        }


        /// <summary>
        /// Gets name of the <see cref="Transition"/>.
        /// </summary>
        /// <value>
        /// An identifier which is unique within the embedding <see cref="StateMachineTemplate"/>.
        /// </value>
        public string Name
        {
            get
            {
                return m_name;
            }
        }


        /// <summary>
        /// Returns the name of the <see cref="Transition"/> as specified in the <see cref="StateMachineTemplate"/>.<see cref="O:StaMa.StateMachineTemplate.Transition"/> statement.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> containing the value of the <see cref="Name"/> property of the <see cref="Transition"/>.
        /// </returns>
        public override string ToString()
        {
            return this.Name;
        }


        /// <summary>
        /// Gets the anchor <see cref="State"/> where the <see cref="Transition"/> is aggregated.
        /// </summary>
        /// <remarks>
        /// Usually the <see cref="Parent"/> will be identical with the source <see cref="State"/>.
        /// In some cases a transition may be aggregated on a higher hierarchical level than the 
        /// <see cref="SourceState"/> in order to raise the <see cref="Transition"/>s priority above other
        /// transitions that are handled premptive due to their hierarchy level.
        /// </remarks>
        public State Parent
        {
            get
            {
                return m_parentState;
            }
        }


        /// <summary>
        /// Gets the <see cref="StateConfiguration"/> that represents the
        /// source state of the <see cref="Transition"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="SourceState"/> will only reference a configuration of
        /// sub-<see cref="State"/> instances of the anchor <see cref="Parent"/> state.
        /// </remarks>
        public StateConfiguration SourceState
        {
            get
            {
                return m_sourceStateConfiguration;
            }
        }


        /// <summary>
        /// Gets the <see cref="StateConfiguration"/> that represents the
        /// target state of the <see cref="Transition"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="TargetState"/> could reference any configuration of
        /// <see cref="State"/> instances within the <see cref="StateMachineTemplate"/>.
        /// </remarks>
        public StateConfiguration TargetState
        {
            get
            {
                return m_targetStateConfiguration;
            }
        }


        /// <summary>
        /// Gets the trigger event on which the <see cref="Transition"/> will react.
        /// </summary>
        /// <value>
        /// A <see cref="object"/> instance that represents the trigger event or <c>null</c> for 
        /// completion transitions.
        /// </value>
        /// <remarks>
        /// See also <see cref="StateMachine.SendTriggerEvent(object)"/>.
        /// </remarks>
        public object TriggerEvent
        {
            get
            {
                return m_triggerEvent;
            }
        }


        /// <summary>
        /// Gets the callback method that provides additional conditions
        /// for the transition.
        /// </summary>
        /// <value>
        /// A <see cref="StateMachineGuardCallback"/> delegate of <c>null</c> if no additional
        /// conditions are defined.
        /// </value>
        public StateMachineGuardCallback Guard
        {
            get
            {
                return m_guard;
            }
        }


#if !STAMA_COMPATIBLE21
        /// <summary>
        /// Gets the callback method that will be called when the transition is executed.
        /// </summary>
        /// <value>
        /// A <see cref="StateMachineActionCallback"/> delegate or
        /// <c>null</c> if no transition action is defined.
        /// </value>
        public StateMachineActionCallback TransitionAction
#else
        /// <summary>
        /// Obsolete. The term "activity" is improper and has been replaced with "action". Please use the corresponding TransitionAction property instead.
        /// </summary>
        [Obsolete("The term \"activity\" is improper and has been replaced with \"action\". Please use the corresponding TransitionAction property instead.")]
        public StateMachineActivityCallback TransitionActivity
#endif
        {
            get
            {
                return m_transitionAction;
            }
        }


        /// <summary>
        /// Gets the least common ancestor of the <see cref="SourceState"/> and
        /// the <see cref="TargetState"/> of the <see cref="Transition"/>.
        /// </summary>
        /// <value>
        /// The <see cref="Region"/> instance 'up' to which the <see cref="StateMachine"/> has to
        /// leave states and enter states during the state change initiated through this <see cref="Transition"/>.
        /// </value>
        public Region LeastCommonAncestor
        {
            get
            {
                return m_leastCommonAncestor;
            }
        }


        internal bool PassThrough(IStateMachineTemplateVisitor visitor)
        {
            bool continuePassThrough = visitor.Transition(this);
            return continuePassThrough;
        }

        internal void FinalFixup(StateMachineTemplate stateMachineTemplate)
        {
            // Make a state configuration for the source and target states
            try
            {
                m_sourceStateConfiguration = stateMachineTemplate.CreateStateConfiguration(m_sourceStates);
            }
            catch (Exception ex)
            {
                throw new StateMachineException(FrameworkAbstractionUtils.StringFormat("The state names for the source state of transition \"{0}\" do not form a valid StateConfiguration.", this.Name), ex);
            }

            try
            {
                m_targetStateConfiguration = stateMachineTemplate.CreateStateConfiguration(m_targetStates);
            }
            catch (Exception ex)
            {
                throw new StateMachineException(FrameworkAbstractionUtils.StringFormat("The state names for the target state of transition \"{0}\" do not form a valid StateConfiguration.", this.Name), ex);
            }

            // Ensure, that at least one of the transition source states is a substate of
            // the state, the transition is linked to (i.e. the transition parent).
            {
                State transiParentState = this.Parent;
                bool found = false;

                foreach (string stateName in m_sourceStates)
                {
                    State state = stateMachineTemplate.FindState(stateName);
                    if (state == null)
                    {
                        // Unknown state (stateName)
                        throw new StateMachineException(FrameworkAbstractionUtils.StringFormat("The source State \"{1}\" of Transition \"{0}\" could not be found.", this.Name, stateName));
                    }

                    while (state != null)
                    {
                        if (state == transiParentState)
                        {
                            found = true;
                            break;
                        }

                        // Step up one level
                        state = state.Parent.Parent;
                    }
                    
                    if (found)
                    {
                        break;
                    }
                }

                if (!found)
                {
                    // If we come to this place we have tested all source states
                    // and did not find any of them within the transition parent state.
                    // Prevent this semantical meaningless situation.
                    throw new StateMachineException(FrameworkAbstractionUtils.StringFormat("None of the source States of Transition \"{0}\" are within the transition source scope.", this.Name));
                }
            }

            // Now we calculate the least common ancestor (LCA) between the transition source and target
            // The transition LCA defines which states (more precise: up to which region) to leave
            // and enter during a state change.
            // To find out the transition LCA, we do the following:
            // Start from the transition's parent state and walk the tree in direction to the root.
            // While walking search for a region that contains the target enveloping state.
            // This region contains both the transition parent state and the target enveloping state.

            // Start a new C# block to make some variables local.
            {
                State stateEnvelopeTarget = m_targetStateConfiguration.FindEnvelopeState();

                m_leastCommonAncestor = null;

                // Start walking with the transi parent
                State state = this.Parent;
                while (state != null)
                {
                    Region parentRegion = state.Parent;
                    
                    // Walk up to the tree from stateEnvelopeTarget
                    State stateTarget = stateEnvelopeTarget;
                    while (stateTarget != null)
                    {
                        Region regionTarget = stateTarget.Parent;
                        if (regionTarget == parentRegion)
                        {
                            m_leastCommonAncestor = parentRegion;
                            break;
                        }

                        // Step up one level with target
                        stateTarget = regionTarget.Parent;
                    }

                    if (m_leastCommonAncestor != null)
                    {
                        break;
                    }

                    // Step up one level with source
                    state = parentRegion.Parent;
                }

                if (m_leastCommonAncestor == null)
                {
                    // Every state config joins at the root, so we never should reach this code
                    throw new InvalidOperationException("Internal error: Couldn't find the least common ancestor for the transition.");
                }
            }
        }
    }
}

#if STAMA_COMPATIBLE21
#pragma warning restore 0618
#endif
