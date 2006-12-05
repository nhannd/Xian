using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Base class for application components that act as containers for other application components.
    /// Currently this class has no content, but exists as a placeholder.  Components that intend to act
    /// primarily as containers for other component should inherit this class for future compatability.
    /// </summary>
    public abstract class ApplicationComponentContainer : ApplicationComponent
    {
        protected abstract IEnumerable<IApplicationComponent> ContainedComponents { get; }

        public override bool HasValidationErrors
        {
            get
            {
                // true if any contained component has validation errors
                return CollectionUtils.Contains<IApplicationComponent>(this.ContainedComponents,
                    delegate(IApplicationComponent c) { return c.HasValidationErrors; });
            }
        }

        public override void ShowValidation(bool show)
        {
            // propagate to each contained component
            foreach (IApplicationComponent c in this.ContainedComponents)
                c.ShowValidation(show);
        }
    }
}
