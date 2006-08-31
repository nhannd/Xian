using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// A specialization of the <see cref="ResourceResolver"/> class for use in resolving resources
    /// related to actions.
    /// </summary>
    internal class ActionResourceResolver : ResourceResolver
    {
        /// <summary>
        /// Constructs an instance of this object for the specified action target. The class of the target
        /// object determines the primary assembly that will be used to resolve resources.
        /// </summary>
        /// <param name="actionTarget">The action target for which resources will be resolved</param>
        internal ActionResourceResolver(object actionTarget)
            :base(new Assembly[] { actionTarget.GetType().Assembly, typeof(Application).Assembly } )
        {
        }
    }
}
