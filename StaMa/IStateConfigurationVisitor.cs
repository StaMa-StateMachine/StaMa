#region StateConfiguration.cs file
//
// StaMa state machine controller library, https://github.com/StaMa-StateMachine/StaMa
//
// Copyright (c) 2005-2016, Roland Schneider. All rights reserved.
//
#endregion

using System;


namespace StaMa
{
    /// <summary>
    /// Defines a set of methods which allow for composing a human readable representation of a <see cref="StateConfiguration"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The members of this interface are successively called when passing the interface to the
    /// <see cref="StateConfiguration"/>.<see cref="StateConfiguration.PassThrough(IStateConfigurationVisitor)"/> method.
    /// </para>
    /// <para>
    /// An implementation of this interface creates the result of the <see cref="StateConfiguration.ToString()"/> method.
    /// </para>
    /// </remarks>
    public interface IStateConfigurationVisitor
    {
#if !STAMA_COMPATIBLE21
        /// <summary>
        /// Will be called for every well-defined state of a <see cref="StateConfiguration"/>.
        /// </summary>
        /// <param name="state">
        /// The <see cref="StaMa.State"/> instance that is visited.
        /// </param>
        void State(State state);
#else
        /// <summary>
        /// Obsolete. The IStateConfigurationVisitor.State(string stateName) method has been replaced through the more comprehensive State(StaMa.State state) method. The stateName can be obtained from the StaMa.State instance through the StaMa.State.Name property.
        /// </summary>
        [Obsolete("The IStateConfigurationVisitor.State(string stateName) method has been replaced through the more versatile State(StaMa.State state) method. The stateName can be obtained from the StaMa.State instance through the StaMa.State.Name property.")]
        void State(string stateName);
#endif

        /// <summary>
        /// Will be called when the active state in a <see cref="StateConfiguration"/> is not specified for a particular <see cref="Region"/>.
        /// </summary>
        void StateAny();

        /// <summary>
        /// Will be called before enumerating the active states of the sub-regions of a composite state.
        /// </summary>
        void BeginSubStates();

        /// <summary>
        /// Will be called after the active states of the sub-regions of a composite state were enumerated.
        /// </summary>
        void EndSubStates();

        /// <summary>
        /// Will be called after enumerating the active state of the sub-region of a composite state.
        /// </summary>
        /// <remarks>
        /// This method will not be called for the last sub-region of a composite state.
        /// </remarks>
        void NextSubState();
    }
}
