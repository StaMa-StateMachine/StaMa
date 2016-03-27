#region State.cs file
//
// StaMa state machine controller library, https://github.com/StaMa-StateMachine/StaMa
//
// Copyright (c) 2005-2016, Roland Schneider. All rights reserved.
//
#endregion

using System;
using System.Text;
using System.Diagnostics;
#if !MF_FRAMEWORK
using System.Collections.Generic;
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
    /// Represents a state with entry and exit actions. Hierarchical sub-states are aggregated through the <see cref="Regions"/> collection.
    /// </summary>
    /// <remarks>
    /// Instances of this class will be created through the <see cref="StateMachineTemplate"/>.<see cref="O:StaMa.StateMachineTemplate.State"/> methods.
    /// </remarks>
    [DebuggerDisplay("FullName={FullName}")]
    public class State
    {
        private static State s_wildcardState = new State();

        internal static State Wildcard
        {
            get
            {
                return s_wildcardState;
            }
        }

        private StateMachineTemplate m_stateMachineTemplate;
        private Region m_parentRegion;
        private RegionCollection m_regions;
        private TransitionCollection m_transitions;
        private string m_name;
#if !STAMA_COMPATIBLE21
        private StateMachineActionCallback m_entryAction;
        private StateMachineActionCallback m_exitAction;
#else
        private StateMachineActivityCallback m_entryAction;
        private StateMachineActivityCallback m_exitAction;
#endif
        private StateMachineDoActionCallback m_doAction;

        private State()
        {
        }


        /// <summary>
        /// Initializes a new <see cref="State"/> instance.
        /// </summary>
        /// <param name="parentRegion">
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
        /// The <see cref="StateMachineDoActionCallback"/> delegate that defines the perpetual action to be executed while the <see cref="StateMachine"/> is in this <see cref="State"/>.
        /// May be <c>null</c> if no do action is specified.
        /// </param>
        /// <param name="stateMachineTemplate">
        /// The embedding <see cref="StateMachineTemplate"/> instance.
        /// Internally used for creating the <see cref="Transitions"/> and <see cref="Regions"/> collections.
        /// </param>
        /// <remarks>
        /// Instances of this class will be created through the <see cref="StateMachineTemplate"/>.<see cref="O:StaMa.StateMachineTemplate.State"/> methods.
        /// </remarks>
#if !MF_FRAMEWORK
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Method is internal.")]
#endif
        internal protected State(
            Region parentRegion,
            string name,
#if !STAMA_COMPATIBLE21
            StateMachineActionCallback entryAction,
            StateMachineActionCallback exitAction,
#else
            StateMachineActivityCallback entryAction,
            StateMachineActivityCallback exitAction,
#endif
            StateMachineDoActionCallback doAction,
            StateMachineTemplate stateMachineTemplate)
        {
            m_stateMachineTemplate = stateMachineTemplate;
            m_parentRegion = parentRegion;
            m_regions = m_stateMachineTemplate.InternalCreateRegionCollection();
            m_transitions = m_stateMachineTemplate.InternalCreateTransitionCollection();
            m_name = name;
            m_entryAction = entryAction;
            m_exitAction = exitAction;
            m_doAction = doAction;
        }


        /// <summary>
        /// Gets the <see cref="Region"/> that aggregates this <see cref="State"/>.
        /// </summary>
        public Region Parent
        {
            get
            {
                return m_parentRegion;
            }
        }


        /// <summary>
        /// Gets the list of sub-<see cref="Region"/> instances.
        /// </summary>
        /// <value>
        /// A <see cref="RegionCollection"/> instance. The <see cref="RegionCollection.Count"/> property
        /// will be <c>0</c> if there are no sub-<see cref="Region"/> instances.
        /// </value>
        public RegionCollection Regions
        {
            get
            {
                return m_regions;
            }
        }


        /// <summary>
        /// Gets the list of <see cref="Transition"/> instances that use this <see cref="State"/> as an anchor.
        /// </summary>
        /// <value>
        /// A <see cref="TransitionCollection"/> instance. The <see cref="TransitionCollection.Count"/> property
        /// will be 0 if there are no <see cref="Transition"/> instances.
        /// </value>
        public TransitionCollection Transitions
        {
            get
            {
                return m_transitions;
            }
        }


        /// <summary>
        /// Gets name of the <see cref="StaMa.State"/>.
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


#if !MF_FRAMEWORK
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used in DebuggerDisplayAttribute.")]
#endif
        internal string FullName
        {
            get
            {
                if (m_parentRegion != null)
                {
                    return m_parentRegion.FullName + '.' + m_name;
                }
                else
                {
                    // State.WildCard
                    return String.Empty;
                }
            }
        }


        /// <summary>
        /// Returns the name of the <see cref="State"/> as specified in the <see cref="StateMachineTemplate"/>.<see cref="O:StaMa.StateMachineTemplate.State"/> statement.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> containing the value of the <see cref="Name"/> property of the <see cref="State"/>.
        /// </returns>
        public override string ToString()
        {
            return m_name;
        }


#if !STAMA_COMPATIBLE21
        /// <summary>
        /// Gets the callback that will be called when a <see cref="StateMachine"/> enters
        /// the <see cref="State"/>.
        /// </summary>
        /// <value>
        /// A <see cref="StateMachineActionCallback"/> delegate or <c>null</c> if
        /// no entry action is defined.
        /// </value>
        public StateMachineActionCallback EntryAction
#else
        /// <summary>
        /// Obsolete. The term "activity" is improper and has been replaced with "action". Please use the corresponding EntryAction property instead.
        /// </summary>
        [Obsolete("The term \"activity\" is improper and has been replaced with \"action\". Please use the corresponding EntryAction property instead.")]
        public StateMachineActivityCallback EntryActivity
#endif
        {
            get
            {
                return m_entryAction;
            }
        }


#if !STAMA_COMPATIBLE21
        /// <summary>
        /// Gets the callback that will be called when a <see cref="StateMachine"/> leaves
        /// the <see cref="State"/>.
        /// </summary>
        /// <value>
        /// A <see cref="StateMachineActionCallback"/> delegate or <c>null</c> if
        /// no exit action is defined.
        /// </value>
        public StateMachineActionCallback ExitAction
#else
        /// <summary>
        /// Obsolete. The term "activity" is improper and has been replaced with "action". Please use the corresponding ExitAction property instead.
        /// </summary>
        [Obsolete("The term \"activity\" is improper and has been replaced with \"action\". Please use the corresponding ExitAction property instead.")]
        public StateMachineActivityCallback ExitActivity
#endif
        {
            get
            {
                return m_exitAction;
            }
        }


        /// <summary>
        /// Gets the <see cref="StateMachineDoActionCallback"/> delegate that defines the perpetual action to be executed while the <see cref="StateMachine"/> is in this <see cref="State"/>.
        /// </summary>
        /// <value>
        /// A <see cref="StateMachineDoActionCallback"/> delegate or <c>null</c> if no do action is defined.
        /// </value>
        /// <remarks>
        /// This callback will be executed from within <see cref="StateMachine.SendTriggerEvent(object,EventArgs)"/> or <see cref="StateMachine.SendTriggerEvent(object)"/>.
        /// after every individual state change or one time in case no state change occurred in the invocation.
        /// </remarks>
        public StateMachineDoActionCallback DoAction
        {
            get
            {
                return m_doAction;
            }
        }


        internal void AddRegion(Region region)
        {
            m_regions.AddInternal(region);
        }


        internal bool PassThrough(IStateMachineTemplateVisitor visitor)
        {
            bool continuePassThrough = visitor.State(this);
            if (!continuePassThrough)
            {
                return false;
            }

            // Iterate through transitions.
            foreach  (Transition transition in this.Transitions)
            {
                continuePassThrough = transition.PassThrough(visitor);
                if (!continuePassThrough)
                {
                    return false;
                }
            }

            // Iterate through sub-regions.
            foreach  (Region subRegion in this.Regions)
            {
                continuePassThrough = subRegion.PassThrough(visitor);
                if (!continuePassThrough)
                {
                    return false;
                }
            }

            continuePassThrough = visitor.EndState(this);
            if (!continuePassThrough)
            {
                return false;
            }
            
            return true;
        }


        internal void AddTransition(Transition transition)
        {
            m_transitions.AddInternal(transition);
        }


        internal void FixupAndCalcStateConfigMetrics(int stateConfigurationIndex, out int stateConfigSize, out int stateBasConfigSize)
        {
            stateConfigSize = 0; // Space needed in a state configuration by all sub-regions. If no sub-regions, no space needed.

            if (this.Regions.Count > 0)
            {
                stateBasConfigSize = 0; // Descriptive comment: see error handling block below.

                foreach (Region subRegion in this.Regions)
                {
                    int stateConfigMax = 0;
                    int stateBasConfigMax = 0;

                    subRegion.FixupAndCalcStateConfigMetrics(ref stateConfigurationIndex, out stateConfigMax, out stateBasConfigMax);

                    stateConfigSize += stateConfigMax;
                    stateBasConfigSize += stateBasConfigMax;
                }

                if ( !(stateBasConfigSize >= 1))
                {
                    // TODO: Clarify what happens if Region doesn't contain a State?
                    // Assume that each (sub-)region has at least one state.
                    // Through recursion we will reach this function with this.Regions.Count == 0, which will return a value == 1.
                    // So after iteration there *must* be a value >= 1. If not so, we have a serious problem.
                    throw new InvalidOperationException("Internal error: FixupAndCalcStateConfigMetrics returned zero length for state configuration.");
                }
            }
            else
            {
                stateBasConfigSize = 1; // Space needed in a base state configuration.
                // If no sub-regions, then return 1 else accumulate over all sub-regions (see below)
            }
        }
    }


#if !MF_FRAMEWORK
    /// <summary>
    /// Represents a list of <see cref="Region"/> instances.
    /// </summary>
    public class RegionCollection : ICollection<Region>
    {
        private List<Region> m_regions;

        /// <summary>
        /// Initializes a new <see cref="RegionCollection"/> instance.
        /// </summary>
        internal protected RegionCollection()
        {
            m_regions = new List<Region>();
        }


        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="RegionCollection"/>. 
        /// </summary>
        /// <returns>
        /// A <see cref="System.Collections.IEnumerator"/> for the <see cref="RegionCollection"/>.
        /// </returns>
        public System.Collections.IEnumerator GetEnumerator()
        {
            return ((System.Collections.IEnumerable)m_regions).GetEnumerator();
        }


        /// <summary>
        /// Gets the <see cref="Region"/> at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the <see cref="Region"/> to get.
        /// </param>
        /// <returns>
        /// The <see cref="Region"/> at the index.
        /// </returns>
        public Region this[int index]
        {
            get
            {
                return m_regions[index];
            }
        }


        /// <summary>
        /// Gets the number of elements contained in the <see cref="RegionCollection"/>. 
        /// </summary>
        public int Count
        {
            get
            {
                return m_regions.Count;
            }
        }
        

        internal void AddInternal(Region region)
        {
            m_regions.Add(region);
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Instances of this class will be created through a factory, client will not subclass.")]
        void ICollection<Region>.Add(Region item)
        {
            throw new NotSupportedException("Collection is read-only.");
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Instances of this class will be created through a factory, client will not subclass.")]
        void ICollection<Region>.Clear()
        {
            throw new NotSupportedException("Collection is read-only.");
        }


        /// <summary>
        /// Determines whether the <see cref="ICollection{Region}"/> contains a specific value.
        /// </summary>
        public bool Contains(Region item)
        {
            return m_regions.Contains(item);
        }


        /// <summary>
        /// Copies the elements of the <see cref="ICollection{Region}"/> to an <see cref="Array"/>, starting at a particular <see cref="Array"/> index.
        /// </summary>
        public void CopyTo(Region[] array, int arrayIndex)
        {
            m_regions.CopyTo(array, arrayIndex);
        }


        /// <summary>
        /// Gets a value indicating whether the <see cref="ICollection{Region}"/> is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Instances of this class will be created through a factory, client will not subclass.")]
        bool ICollection<Region>.Remove(Region item)
        {
            throw new NotSupportedException("Collection is read-only.");
        }


        IEnumerator<Region> IEnumerable<Region>.GetEnumerator()
        {
            return m_regions.GetEnumerator();
        }
    }
#else
    /// <summary>
    /// Represents a list of <see cref="Region"/> instances.
    /// </summary>
    public class RegionCollection
    {
        private ArrayList m_regions;

        /// <summary>
        /// Initializes a new <see cref="RegionCollection"/> instance.
        /// </summary>
        internal protected RegionCollection()
        {
            m_regions = new ArrayList();
        }


        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="RegionCollection"/>. 
        /// </summary>
        /// <returns>
        /// A <see cref="System.Collections.IEnumerator"/> for the <see cref="RegionCollection"/>.
        /// </returns>
        public System.Collections.IEnumerator GetEnumerator()
        {
            return ((System.Collections.IEnumerable)m_regions).GetEnumerator();
        }


        /// <summary>
        /// Gets the <see cref="Region"/> at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the <see cref="Region"/> to get.
        /// </param>
        /// <returns>
        /// The <see cref="Region"/> at the index.
        /// </returns>
        public Region this[int index]
        {
            get
            {
                if (index >= m_regions.Count)
                {
                    throw new ArgumentOutOfRangeException("index", "Index was out of range. Must be non-negative and less than the size of the collection.");
                }
                return (Region)m_regions[index];
            }
        }


        /// <summary>
        /// Gets the number of elements contained in the <see cref="RegionCollection"/>. 
        /// </summary>
        public int Count
        {
            get
            {
                return m_regions.Count;
            }
        }
        

        internal void AddInternal(Region region)
        {
            m_regions.Add(region);
        }


        internal void AddRange(RegionCollection regions)
        {
            if (m_regions.Capacity < m_regions.Count + regions.Count)
            {
                m_regions.Capacity = System.Math.Max(m_regions.Capacity, m_regions.Count + regions.Count);
            }
            foreach (Region r in regions)
            {
                m_regions.Add(r);
            }
        }
    }
#endif



#if !MF_FRAMEWORK
    /// <summary>
    /// Represents a list of <see cref="Transition"/> instances.
    /// </summary>
    public class TransitionCollection : ICollection<Transition>
    {
        private List<Transition> m_transitions;


        /// <summary>
        /// Initializes a new <see cref="TransitionCollection"/> instance.
        /// </summary>
        internal protected TransitionCollection()
        {
            m_transitions = new List<Transition>();
        }


        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="TransitionCollection"/>. 
        /// </summary>
        /// <returns>
        /// A <see cref="System.Collections.IEnumerator"/> for the <see cref="TransitionCollection"/>.
        /// </returns>
        public System.Collections.IEnumerator GetEnumerator()
        {
            return ((System.Collections.IEnumerable)m_transitions).GetEnumerator();
        }


        /// <summary>
        /// Gets the <see cref="Transition"/> at the specified index.
        /// </summary>
        /// <param name="priority">
        /// The zero-based index of the <see cref="Transition"/> to get.
        /// </param>
        /// <returns>
        /// The <see cref="Transition"/> at the index.
        /// </returns>
        public Transition this[int priority]
        {
            get
            {
                return m_transitions[priority];
            }
        }


        /// <summary>
        /// Gets the number of elements contained in the <see cref="RegionCollection"/>. 
        /// </summary>
        public int Count
        {
            get
            {
                return m_transitions.Count;
            }
        }
        

        internal void AddInternal(Transition transition)
        {
            m_transitions.Add(transition);
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Instances of this class will be created through a factory, client will not subclass.")]
        void ICollection<Transition>.Add(Transition item)
        {
            throw new NotSupportedException("Collection is read-only.");
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Instances of this class will be created through a factory, client will not subclass.")]
        void ICollection<Transition>.Clear()
        {
            throw new NotSupportedException("Collection is read-only.");
        }


        /// <summary>
        /// Determines whether the <see cref="ICollection{Transition}"/> contains a specific value.
        /// </summary>
        public bool Contains(Transition item)
        {
            return m_transitions.Contains(item);
        }


        /// <summary>
        /// Copies the elements of the <see cref="ICollection{Transition}"/> to an <see cref="Array"/>, starting at a particular <see cref="Array"/> index.
        /// </summary>
        public void CopyTo(Transition[] array, int arrayIndex)
        {
            m_transitions.CopyTo(array, arrayIndex);
        }


        /// <summary>
        /// Gets a value indicating whether the <see cref="ICollection{Transition}"/> is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Instances of this class will be created through a factory, client will not subclass.")]
        bool ICollection<Transition>.Remove(Transition item)
        {
            throw new NotSupportedException("Collection is read-only.");
        }


        IEnumerator<Transition> IEnumerable<Transition>.GetEnumerator()
        {
            return m_transitions.GetEnumerator();
        }
    }
#else
    /// <summary>
    /// Represents a list of <see cref="Transition"/> instances.
    /// </summary>
    public class TransitionCollection
    {
        private ArrayList m_transitions;


        /// <summary>
        /// Initializes a new <see cref="TransitionCollection"/> instance.
        /// </summary>
        internal protected TransitionCollection()
        {
            m_transitions = new ArrayList();
        }


        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="TransitionCollection"/>. 
        /// </summary>
        /// <returns>
        /// A <see cref="System.Collections.IEnumerator"/> for the <see cref="TransitionCollection"/>.
        /// </returns>
        public System.Collections.IEnumerator GetEnumerator()
        {
            return ((System.Collections.IEnumerable)m_transitions).GetEnumerator();
        }


        /// <summary>
        /// Gets the <see cref="Transition"/> at the specified index.
        /// </summary>
        /// <param name="priority">
        /// The zero-based index of the <see cref="Transition"/> to get.
        /// </param>
        /// <returns>
        /// The <see cref="Transition"/> at the index.
        /// </returns>
        public Transition this[int priority]
        {
            get
            {
                if (priority >= m_transitions.Count)
                {
                    throw new ArgumentOutOfRangeException("index", "Index was out of range. Must be non-negative and less than the size of the collection.");
                }
                return (Transition)m_transitions[priority];
            }
        }


        /// <summary>
        /// Gets the number of elements contained in the <see cref="RegionCollection"/>. 
        /// </summary>
        public int Count
        {
            get
            {
                return m_transitions.Count;
            }
        }


        internal void AddInternal(Transition transition)
        {
            m_transitions.Add(transition);
        }
    }
#endif
}

#if STAMA_COMPATIBLE21
#pragma warning restore 0618
#endif
