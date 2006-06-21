using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using ClearCanvas.Common;

namespace ClearCanvas.Common.Application.Actions
{
    /// <summary>
    /// A specialization of the <see cref="ResourceResolver"/> class for use in resolving action path
    /// resource keys.  This class attempts to resolve resources by first searching the assembly
    /// that contains the type of the action target object, and secondarily searching the Model assembly.
    /// </summary>
    internal class ActionResourceResolver : ResourceResolver
    {
        /// <summary>
        /// Constructs an instance of this object for the specified action target.
        /// </summary>
        /// <param name="actionTarget">The action target for which resources will be resolved</param>
        internal ActionResourceResolver(object actionTarget)
            :base(new Assembly[] { actionTarget.GetType().Assembly, typeof(WorkstationModel).Assembly } )
        {
        }
    }
}
