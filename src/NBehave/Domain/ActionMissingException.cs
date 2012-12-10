// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionMissingException.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the ActionMissingException type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace NBehave.Domain
{
    public class ActionMissingException : Exception
    {
        public ActionMissingException()
        {
        }

        public ActionMissingException(string message)
            : base(message)
        {
        }

        public ActionMissingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public ActionMissingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}