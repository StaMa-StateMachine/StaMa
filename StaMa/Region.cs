#region Region.cs file
//
// StaMa state machine controller library, https://github.com/StaMa-StateMachine/StaMa
//
// Copyright (c) 2005-2016, Roland Schneider. All rights reserved.
//
#endregion

using System;
#if !MF_FRAMEWORK
using System.Collections.Generic;
#else
using System.Collections;
#endif
using System.Diagnostics;


namespace StaMa
{
    /// <summary>
    /// Represents a container for states and defines the initial <see cref="State"/> for the contained states.
    /// </summary>
    /// <remarks>
    /// Instances of this class will be created through the <see cref="StateMachineTemplate"/>.<see cref="StateMachineTemplate.Region"/> method.
    /// </remarks>
    [DebuggerDisplay("FullName={FullName}")]
    public class Region
    {
        private State m_parentState;
        private StateCollection m_states;
        private State m_initialState;
        private string m_initialStateName;
        private int m_historyIndex;
        private int m_stateConfigurationIndex;


        /// <summary>
        /// Initializes a new <see cref="Region"/> instance. 
        /// </summary>
        /// <param name="parentState">
        /// The <see cref="State"/> instance that contains this <see cref="Region"/>. Shall be <c>null</c> for the root <see cref="Region"/> instance.
        /// </param>
        /// <param name="initialStateName">
        /// The name of the initial <see cref="State"/> of the <see cref="Region"/>.
        /// </param>
        /// <param name="historyIndex">
        /// If the <see cref="Region"/> shall have a history, the index of the slot where
        /// this <see cref="Region"/> stores its history in the <see cref="StateMachine"/> instance.
        /// If the region has no history, <see cref="int.MaxValue">int.MaxValue</see>.
        /// </param>
        /// <param name="stateMachineTemplate">
        /// The embedding <see cref="StateMachineTemplate"/> instance.
        /// Internally used for creating the <see cref="States"/> collection.
        /// </param>
        /// <remarks>
        /// Instances of this class will be created through the <see cref="StateMachineTemplate"/>.<see cref="StateMachineTemplate.Region"/> method.
        /// </remarks>
#if !MF_FRAMEWORK
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Method is internal.")]
#endif
        internal protected Region(State parentState, string initialStateName, int historyIndex, StateMachineTemplate stateMachineTemplate)
        {
            m_parentState = parentState;
            m_states = stateMachineTemplate.InternalCreateStateCollection();
            m_initialState = null;
            m_initialStateName = initialStateName;
            m_historyIndex = historyIndex;
            m_stateConfigurationIndex = int.MaxValue;
        }


        internal void EndRegion()
        {
            foreach (State state in m_states)
            {
                if (state.Name == m_initialStateName)
                {
                    m_initialState = state;
                    break;
                }
            }
            if (m_initialState == null)
            {
                throw new StateMachineException("The value of the initialStateName parameter of the \"Region(...)\" statement doesn't any of the region's sub-states.");
            }
        }


        /// <summary>
        /// Returns the execution order of the <see cref="Region"/>.
        /// </summary>
        /// <returns>
        /// Returns the <see cref="ExecutionOrder"/> value formatted as a <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return FrameworkAbstractionUtils.Int32ToString(this.ExecutionOrder);
        }


#if !MF_FRAMEWORK
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used in DebuggerDisplayAttribute.")]
#endif
        internal string FullName
        {
            get
            {
                if (m_parentState != null)
                {
                    return m_parentState.FullName + this.Name;
                }
                else
                {
                    return this.Name;
                }
            }
        }


        internal string Name
        {
            get
            {
                return '~' + FrameworkAbstractionUtils.Int32ToString(this.ExecutionOrder);
            }
        }


        /// <summary>
        /// Gets the <see cref="State"/> that aggregates this <see cref="Region"/>.
        /// </summary>
        public State Parent
        {
            get
            {
                return m_parentState;
            }
        }


        /// <summary>
        /// Gets the list of <see cref="State"/>s of this <see cref="Region"/>.
        /// </summary>
        /// <value>
        /// A <see cref="StateCollection"/> instance. At least one item (namely the
        /// <see cref="InitialState"/>) will be present in this collection.
        /// </value>
        public StateCollection States
        {
            get
            {
                return m_states;
            }
        }


        /// <summary>
        /// The sequential index of this <see cref="Region"/> within
        /// the <see cref="State.Regions"/> collection of the parent <see cref="State"/>.
        /// </summary>
        /// <value>
        /// A <see cref="int"/> greater or equal 1.
        /// </value>
        public int ExecutionOrder
        {
            get
            {
                if (m_parentState == null)
                {
                    return 1;
                }
                else
                {
                    // TODO: Check if better make Regions inherit from LinkedList and follow Prev until we reach First.
                    int index = 1;
                    foreach (Region region in this.Parent.Regions)
                    {
                        if (region == this)
                        {
                            return index;
                        }
                        index++;
                    }
                    // Logical error
                    throw new InvalidOperationException("Internal error: Region instance not contained in regions list of parent state.");
                }
            }
        }


        /// <summary>
        /// The <see cref="State"/> that will be made active when the <see cref="Region"/> is entered
        /// the first time.
        /// </summary>
        public State InitialState
        {
            get
            {
                return m_initialState;
            }
        }


        /// <summary>
        /// The slot within a <see cref="StateConfiguration"/> that will be used by
        /// the active state of this <see cref="Region"/>.
        /// </summary>
        public int StateConfigurationIndex
        {
            get
            {
                return m_stateConfigurationIndex;
            }
        }


        /// <summary>
        /// Indicates whether the recently active <see cref="State"/> will be made
        /// active on rentry of the <see cref="Region"/>.
        /// </summary>
        /// <value>
        /// <c>true</c> if the recently active <see cref="State"/> will be made
        /// active on rentry; <c>false</c> if the <see cref="InitialState"/> will be made active.
        /// </value>
        public bool HasHistory
        {
            get
            {
                return (m_historyIndex != int.MaxValue);
            }
        }


        /// <summary>
        /// Gets the index of the slot where this <see cref="Region"/> stores its
        /// history in the <see cref="StateMachine"/> instance.
        /// </summary>
        /// <value>
        /// A <see cref="uint"/> if the <see cref="Region"/> has a history; otherwise, <see cref="uint.MaxValue">uint.MaxValue</see>.
        /// </value>
        public int HistoryIndex
        {
            get
            {
                return m_historyIndex;
            }
        }


        internal bool PassThrough(IStateMachineTemplateVisitor visitor)
        {
            bool continuePassThrough = visitor.Region(this);
            if (!continuePassThrough)
            {
                return false;
            }

            // Iterate through states.
            foreach (State state in this.States)
            {
                continuePassThrough = state.PassThrough(visitor);
                if (!continuePassThrough)
                {
                    return false;
                }
            }

            continuePassThrough = visitor.EndRegion(this);
            if (!continuePassThrough)
            {
                return false;
            }
            
            return true;
        }


        internal void AddState(State state)
        {
            m_states.AddInternal(state);
        }


        internal void FixupAndCalcStateConfigMetrics(ref int stateConfigurationIndex, out int stateConfigMax, out int stateBasConfigMax)
        {
            // Set the slot for the active state reference.
            // For every region we have to reserve one slot in
            // the active state array.
            m_stateConfigurationIndex = stateConfigurationIndex;
            stateConfigurationIndex = stateConfigurationIndex + 1; // Increase value (children and siblings will start on a new index)

            int stateConfigLocalMax = 0;
            stateBasConfigMax = 0; // Return the maximum of concurrent transitions for all substates

            foreach (State state in this.States)
            {
                int stateConfigSize = 0; // Size of pointers needed for this state and its offspring in a state config array
                int stateBasConfigSize = 0; // Size of pointers needed for this state in a base state configuration array

                state.FixupAndCalcStateConfigMetrics(stateConfigurationIndex, out stateConfigSize, out stateBasConfigSize);

                if (stateConfigSize > stateConfigLocalMax) stateConfigLocalMax = stateConfigSize; // Find the maximum needed over all states in this region
                if (stateBasConfigSize > stateBasConfigMax) stateBasConfigMax = stateBasConfigSize;
            }

            stateConfigMax = stateConfigLocalMax + 1; // Return the maximum of the offspring plus one for ourselves
            stateConfigurationIndex += stateConfigLocalMax; // Shift the index to reserve space for this region's offspring
        }


        internal void FinalFixup(StateMachineTemplate stateMachineTemplate)
        {
            // Recurse to sub items
            foreach (State state in this.States)
            {
                foreach (Transition transition in state.Transitions)
                {
                    transition.FinalFixup(stateMachineTemplate);
                }

                foreach (Region subRegion in state.Regions)
                {
                    subRegion.FinalFixup(stateMachineTemplate);
                }
            }
        }
    }


#if !MF_FRAMEWORK
    /// <summary>
    /// Represents a list of <see cref="State"/> instances.
    /// </summary>
    public class StateCollection : ICollection<State>
    {
        private List<State> m_states;


        /// <summary>
        /// Initializes a new <see cref="StateCollection"/> instance.
        /// </summary>
        internal protected StateCollection()
        {
            m_states = new List<State>();
        }


        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="StateCollection"/>. 
        /// </summary>
        /// <returns>
        /// A <see cref="System.Collections.IEnumerator"/> for the <see cref="StateCollection"/>.
        /// </returns>
        public System.Collections.IEnumerator GetEnumerator()
        {
            return ((System.Collections.IEnumerable)m_states).GetEnumerator();
        }


        /// <summary>
        /// Gets the <see cref="State"/> with a specific name within the <see cref="Region"/>.
        /// </summary>
        /// <param name="stateName">
        /// The name of the <see cref="State"/>.
        /// </param>
        /// <returns>
        /// The <see cref="State"/> with the specified name, or <c>null</c> if there is no such <see cref="State"/>.
        /// </returns>
        public State this[string stateName]
        {
            get
            {
                foreach (State state in m_states)
                {
                    if (stateName == state.Name)
                    {
                        return state;
                    }
                }
                return null;
            }
        }


        /// <summary>
        /// Gets the <see cref="State"/> at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the <see cref="State"/> to get.
        /// </param>
        /// <returns>
        /// The <see cref="State"/> at the index.
        /// </returns>
        public State this[int index]
        {
            get
            {
                return m_states[index];
            }
        }


        /// <summary>
        /// Gets the number of elements contained in the <see cref="StateCollection"/>. 
        /// </summary>
        public int Count
        {
            get
            {
                return m_states.Count;
            }
        }
        

        internal void AddInternal(State state)
        {
            m_states.Add(state);
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Instances of this class will be created through a factory, client will not subclass.")]
        void ICollection<State>.Add(State item)
        {
            throw new NotSupportedException("Collection is read-only.");
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Instances of this class will be created through a factory, client will not subclass.")]
        void ICollection<State>.Clear()
        {
            throw new NotSupportedException("Collection is read-only.");
        }


        /// <summary>
        /// Determines whether the <see cref="ICollection{State}"/> contains a specific value.
        /// </summary>
        public bool Contains(State item)
        {
            return m_states.Contains(item);
        }


        /// <summary>
        /// Copies the elements of the <see cref="ICollection{State}"/> to an <see cref="Array"/>, starting at a particular <see cref="Array"/> index.
        /// </summary>
        public void CopyTo(State[] array, int arrayIndex)
        {
            m_states.CopyTo(array, arrayIndex);
        }


        /// <summary>
        /// Gets a value indicating whether the <see cref="ICollection{State}"/> is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Instances of this class will be created through a factory, client will not subclass.")]
        bool ICollection<State>.Remove(State item)
        {
            throw new NotSupportedException("Collection is read-only.");
        }


        IEnumerator<State> IEnumerable<State>.GetEnumerator()
        {
            return m_states.GetEnumerator();
        }
    }
#else
    /// <summary>
    /// Represents a list of <see cref="State"/> instances.
    /// </summary>
    public class StateCollection
    {
        private ArrayList m_states;


        /// <summary>
        /// Initializes a new <see cref="StateCollection"/> instance.
        /// </summary>
        internal protected StateCollection()
        {
            m_states = new ArrayList();
        }


        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="StateCollection"/>. 
        /// </summary>
        /// <returns>
        /// A <see cref="System.Collections.IEnumerator"/> for the <see cref="StateCollection"/>.
        /// </returns>
        public System.Collections.IEnumerator GetEnumerator()
        {
            return ((System.Collections.IEnumerable)m_states).GetEnumerator();
        }


        /// <summary>
        /// Gets the <see cref="State"/> with a specific name within the <see cref="Region"/>.
        /// </summary>
        /// <param name="stateName">
        /// The name of the <see cref="State"/>.
        /// </param>
        /// <returns>
        /// The <see cref="State"/> with the specified name, or <c>null</c> if there is no such <see cref="State"/>.
        /// </returns>
        public State this[string stateName]
        {
            get
            {
                foreach (State state in m_states)
                {
                    if (stateName == state.Name)
                    {
                        return state;
                    }
                }
                return null;
            }
        }


        /// <summary>
        /// Gets the <see cref="State"/> at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the <see cref="State"/> to get.
        /// </param>
        /// <returns>
        /// The <see cref="State"/> at the index.
        /// </returns>
        public State this[int index]
        {
            get
            {
                if (index >= m_states.Count)
                {
                    throw new ArgumentOutOfRangeException("index", "Index was out of range. Must be non-negative and less than the size of the collection.");
                }
                return (State)m_states[index];
            }
        }


        /// <summary>
        /// Gets the number of elements contained in the <see cref="StateCollection"/>. 
        /// </summary>
        public int Count
        {
            get
            {
                return m_states.Count;
            }
        }


        internal void AddInternal(State state)
        {
            m_states.Add(state);
        }
    }
#endif
}
