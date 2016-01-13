#region StateMachine.cs file
//
// StaMa state machine controller library, http://stama.codeplex.com/
//
// Copyright (c) 2005-2014, Roland Schneider. All rights reserved.
//
#endregion

using System;
using System.Runtime.InteropServices;

#if !MF_FRAMEWORK
using System.Runtime.Serialization;
#endif

namespace StaMa
{
    /// <summary>
    /// The exception that is thrown during creation of the state machine template when creation rules are violated.
    /// </summary>
#if STAMA_COMPATIBLE21
    [Serializable]
    [ComVisible(true)]
#endif
    public class StateMachineException : InvalidOperationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachineException"/> class.
        /// </summary>
        public StateMachineException()
            : base()
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachineException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">
        /// The message that describes the error.
        /// </param>
        public StateMachineException(string message)
            : base(message)
        {
        }

#if STAMA_COMPATIBLE21
#if !MF_FRAMEWORK
        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachineException"/> class with serialized data.
        /// </summary>
        /// <param name="info">
        /// The object that holds the serialized object data.
        /// </param>
        /// <param name="context">
        /// The contextual information about the source or destination.
        /// </param>
        protected StateMachineException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachineException"/> class with
        /// a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">
        /// The error message that explains the reason for the exception
        /// </param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception.
        /// If the <paramref name="innerException"/> is not <c>null</c>, the
        /// current exception is raised in a catch block that handles the inner exception.
        /// </param>
        public StateMachineException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
