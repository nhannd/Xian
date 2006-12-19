using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Validation
{
    /// <summary>
    /// Implements a validation strategy that considers all contained nodes, regardless of whether the user
    /// has visited them or not.
    /// </summary>
    public class AllNodesContainerValidationStrategy : IApplicationComponentContainerValidationStrategy
    {
        #region IApplicationComponentContainerValidationStrategy Members

        public bool HasValidationErrors(IApplicationComponentContainer container)
        {
            // true if any contained component has validation errors
            return CollectionUtils.Contains<IApplicationComponent>(container.ContainedComponents,
                delegate(IApplicationComponent c)
                {
                    container.EnsureStarted(c);
                    return c.HasValidationErrors;
                });
        }

        public void ShowValidation(IApplicationComponentContainer container, bool show)
        {
            if (show)
            {
                // propagate to each component, starting the component if not already started
                foreach (IApplicationComponent c in container.ContainedComponents)
                {
                    container.EnsureStarted(c);
                    c.ShowValidation(show);
                }

                bool visibleComponentHasErrors = CollectionUtils.Contains<IApplicationComponent>(container.VisibleComponents,
                    delegate(IApplicationComponent c) { return c.HasValidationErrors; });

                // if there are no errors on a visible component, find the first component with errors and ensure it is visible
                if (!visibleComponentHasErrors)
                {
                    IApplicationComponent firstComponentWithErrors = CollectionUtils.SelectFirst<IApplicationComponent>(
                        container.ContainedComponents,
                        delegate(IApplicationComponent c) { return c.HasValidationErrors; });

                    if (firstComponentWithErrors != null)
                        container.EnsureVisible(firstComponentWithErrors);
                }
            }
            else
            {
                // propagate to each started component
                foreach (IApplicationComponent c in container.ContainedComponents)
                {
                    if(c.IsStarted)
                        c.ShowValidation(show);
                }
            }
            
        }

        #endregion
    }
}
