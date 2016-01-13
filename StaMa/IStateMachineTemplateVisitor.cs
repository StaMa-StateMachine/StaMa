#region IStateMachineTemplateVisitor.cs file
//
// StaMa state machine controller library, http://stama.codeplex.com/
//
// Copyright (c) 2005-2014, Roland Schneider. All rights reserved.
//
#endregion

using System;

namespace StaMa
{
    /// <summary>
    /// Defines the visitor interface to be used for passing through the
    /// regions, states and transitions of a <see cref="StaMa.StateMachineTemplate"/>.
    /// </summary>
    /// <remarks>
    /// This interface has to be implemented by a client object if the
    /// <see cref="StateMachineTemplate.PassThrough">PassThrough()</see> method is used for
    /// traversing through the <see cref="StaMa.Region"/>, <see cref="StaMa.State"/> and
    /// <see cref="StaMa.Transition"/> nodes of a
    /// <see cref="StaMa.StateMachineTemplate"/> object.
    /// The client object's methods <see cref="IStateMachineTemplateVisitor.Region">Region()</see>,
    /// <see cref="IStateMachineTemplateVisitor.EndRegion">EndRegion()</see>,
    /// <see cref="IStateMachineTemplateVisitor.State">State()</see>, 
    /// <see cref="IStateMachineTemplateVisitor.EndState">EndState()</see> and
    /// <see cref="IStateMachineTemplateVisitor.Transition">Transition()</see> will be
    /// called subsequently during traversal of the node tree.
    /// </remarks>
    /// <seealso cref="StaMa.Region"/>
    /// <seealso cref="StaMa.State"/>
    /// <seealso cref="StaMa.Transition"/>
    public interface IStateMachineTemplateVisitor
    {
        /// <summary>
        /// This method will be called when a <see cref="StaMa.Region"/> object is
        /// encountered while traversing a <see cref="StaMa.StateMachineTemplate"/> with
        /// the <see cref="StateMachineTemplate.PassThrough">PassThrough()</see> method.
        /// </summary>
        /// <remarks>
        /// This method will be called before traversing the sub-objects of
        /// the <see cref="StaMa.Region"/>. Balanced with this method call there
        /// will be a <see cref="IStateMachineTemplateVisitor.EndRegion">EndRegion()</see> method
        /// call after traversing the sub-objects of the <see cref="StaMa.Region"/>.
        /// Between the <see cref="IStateMachineTemplateVisitor.Region">Region()</see> and
        /// <see cref="IStateMachineTemplateVisitor.EndRegion">EndRegion()</see> method calls there
        /// may be <see cref="IStateMachineTemplateVisitor.State">State()</see>,
        /// <see cref="IStateMachineTemplateVisitor.EndState">EndState()</see> method calls for
        /// the sub-objects of the <see cref="StaMa.Region"/>.
        /// </remarks>
        /// <example>
        /// See <see cref="StateMachineTemplate.PassThrough">PassThrough()</see>.
        /// </example>
        /// <param name="region">
        /// The <see cref="StaMa.Region"/> object that is encountered.
        /// </param>
        /// <returns>
        /// <see cref="System.Boolean">true</see> if traversal shall continue; 
        /// <see cref="System.Boolean">false</see> to stop traversal.
        /// </returns>
        /// <seealso cref="StaMa.Region">Region Class</seealso>
        /// <seealso cref="IStateMachineTemplateVisitor.EndRegion">EndRegion() Method</seealso>
        /// <seealso cref="IStateMachineTemplateVisitor.State">State() Method</seealso>
        /// <seealso cref="IStateMachineTemplateVisitor.EndState">EndState() Method</seealso>
        /// <seealso cref="IStateMachineTemplateVisitor.Transition">Transition() Method</seealso>
        bool Region(Region region);

        /// <summary>
        /// This method will be called after a <see cref="StaMa.Region"/> object was
        /// encountered while traversing a <see cref="StateMachineTemplate"/> with
        /// the <see cref="StateMachineTemplate.PassThrough">PassThrough()</see> method.
        /// </summary>
        /// <remarks>
        /// This method will be called after traversing the sub-objects of
        /// the <see cref="StaMa.Region"/>. Balanced with this method call there
        /// was a <see cref="IStateMachineTemplateVisitor.Region">Region()</see> method
        /// call before traversing the sub-objects of the <see cref="StaMa.Region"/>.
        /// </remarks>
        /// <example>
        /// See <see cref="StateMachineTemplate.PassThrough">PassThrough()</see>.
        /// </example>
        /// <param name="region">
        /// The <see cref="StaMa.Region"/> object that was encountered.
        /// </param>
        /// <returns>
        /// <see cref="System.Boolean">true</see> if traversal shall continue; 
        /// <see cref="System.Boolean">false</see> to stop traversal.
        /// </returns>
        /// <seealso cref="StaMa.Region">Region Class</seealso>
        /// <seealso cref="IStateMachineTemplateVisitor.Region">Region() Method</seealso>
        /// <seealso cref="IStateMachineTemplateVisitor.State">State() Method</seealso>
        /// <seealso cref="IStateMachineTemplateVisitor.EndState">EndState() Method</seealso>
        /// <seealso cref="IStateMachineTemplateVisitor.Transition">Transition() Method</seealso>
        bool EndRegion(Region region);

        /// <summary>
        /// This method will be called when a <see cref="StaMa.State"/> object is
        /// encountered while traversing a <see cref="StaMa.StateMachineTemplate"/> with
        /// the <see cref="StateMachineTemplate.PassThrough">PassThrough()</see> method.
        /// </summary>
        /// <remarks>
        /// This method will be called before traversing the sub-objects of
        /// the <see cref="StaMa.State"/>. Balanced with this method call there
        /// will be a <see cref="IStateMachineTemplateVisitor.EndState">EndState()</see> method
        /// call after traversing the sub-objects of the <see cref="StaMa.State"/>.
        /// Between the <see cref="IStateMachineTemplateVisitor.State">State()</see> and
        /// <see cref="IStateMachineTemplateVisitor.EndState">EndState()</see> method calls there
        /// may be <see cref="IStateMachineTemplateVisitor.Transition">Transition()</see>, 
        /// <see cref="IStateMachineTemplateVisitor.Region">Region()</see> and
        /// <see cref="IStateMachineTemplateVisitor.EndRegion">EndRegion()</see>  method calls for
        /// the sub-objects of the <see cref="StaMa.State"/>.
        /// </remarks>
        /// <example>
        /// See <see cref="StateMachineTemplate.PassThrough">PassThrough()</see>.
        /// </example>
        /// <param name="state">
        /// The <see cref="StaMa.State"/> object that is encountered.
        /// </param>
        /// <returns>
        /// <see cref="System.Boolean">true</see> if traversal shall continue; 
        /// <see cref="System.Boolean">false</see> to stop traversal.
        /// </returns>
        /// <seealso cref="StaMa.State">State Class</seealso>
        /// <seealso cref="IStateMachineTemplateVisitor.Region">Region() Method</seealso>
        /// <seealso cref="IStateMachineTemplateVisitor.EndRegion">EndRegion() Method</seealso>
        /// <seealso cref="IStateMachineTemplateVisitor.EndState">EndState() Method</seealso>
        /// <seealso cref="IStateMachineTemplateVisitor.Transition">Transition() Method</seealso>
        bool State(State state);

        /// <summary>
        /// This method will be called after a <see cref="StaMa.State"/> object was
        /// encountered while traversing a <see cref="StaMa.StateMachineTemplate"/> with
        /// the <see cref="StateMachineTemplate.PassThrough">PassThrough()</see> method.
        /// </summary>
        /// <remarks>
        /// This method will be called after traversing the sub-objects of
        /// the <see cref="StaMa.State"/>. Balanced with this method call there
        /// was a <see cref="IStateMachineTemplateVisitor.State">State()</see> method
        /// call before traversing the sub-objects of the <see cref="StaMa.State"/>.
        /// </remarks>
        /// <example>
        /// See <see cref="StateMachineTemplate.PassThrough">PassThrough()</see>.
        /// </example>
        /// <param name="state">
        /// The <see cref="StaMa.State"/> object that was encountered.
        /// </param>
        /// <returns>
        /// <see cref="System.Boolean">true</see> if traversal shall continue; 
        /// <see cref="System.Boolean">false</see> to stop traversal.
        /// </returns>
        /// <seealso cref="StaMa.State">State Class</seealso>
        /// <seealso cref="IStateMachineTemplateVisitor.Region">Region() Method</seealso>
        /// <seealso cref="IStateMachineTemplateVisitor.EndRegion">EndRegion() Method</seealso>
        /// <seealso cref="IStateMachineTemplateVisitor.State">State() Method</seealso>
        /// <seealso cref="IStateMachineTemplateVisitor.Transition">Transition() Method</seealso>
        bool EndState(State state);

        /// <summary>
        /// This method will be called when a <see cref="StaMa.Transition"/> object is
        /// encountered while traversing a <see cref="StaMa.StateMachineTemplate">StateMachineTemplate</see> with
        /// the <see cref="StateMachineTemplate.PassThrough">PassThrough()</see> method.
        /// </summary>
        /// <remarks>
        /// Because there are no sub-objects in a <see cref="StaMa.Transition"/>, there
        /// is only a single method call, as opposed to the
        /// <see cref="IStateMachineTemplateVisitor.Region">Region()</see> and
        /// <see cref="IStateMachineTemplateVisitor.State">State()</see> methods, which are
        /// always called paired with a <see cref="IStateMachineTemplateVisitor.EndRegion">EndRegion()</see>
        /// and <see cref="IStateMachineTemplateVisitor.EndState">EndState()</see> call.
        /// </remarks>
        /// <example>
        /// See <see cref="StateMachineTemplate.PassThrough">PassThrough()</see>.
        /// </example>
        /// <param name="transition">
        /// The <see cref="StaMa.Transition"/> object that is encountered.
        /// </param>
        /// <returns>
        /// <see cref="System.Boolean">true</see> if traversal shall continue; 
        /// <see cref="System.Boolean">false</see> to stop traversal.
        /// </returns>
        /// <seealso cref="StaMa.Transition">Transition Class</seealso>
        /// <seealso cref="IStateMachineTemplateVisitor.Region">Region() Method</seealso>
        /// <seealso cref="IStateMachineTemplateVisitor.EndRegion">EndRegion() Method</seealso>
        /// <seealso cref="IStateMachineTemplateVisitor.State">State() Method</seealso>
        /// <seealso cref="IStateMachineTemplateVisitor.EndState">EndState() Method</seealso>
        bool Transition(Transition transition);
    }
}
