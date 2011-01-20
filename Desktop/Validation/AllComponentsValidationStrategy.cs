#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Validation
{
    /// <summary>
    /// Implements a validation strategy that considers all contained nodes, regardless of whether the user
    /// has visited them or not.
    /// </summary>
    public class AllComponentsValidationStrategy : IApplicationComponentContainerValidationStrategy
    {
		/// <summary>
		/// Constructor.
		/// </summary>
		public AllComponentsValidationStrategy()
		{
		}

    	#region IApplicationComponentContainerValidationStrategy Members

    	/// <summary>
    	/// Determines whether the specified container has validation errors, according to this strategy.
    	/// </summary>
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

    	/// <summary>
    	/// Displays validation errors for the specified container to the user, according to the logic
    	/// encapsulated in this strategy.
    	/// </summary>
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

                bool visibleComponentHasErrors = CollectionUtils.Contains(container.VisibleComponents,
                    delegate(IApplicationComponent c) { return c.HasValidationErrors; });

                // if there are no errors on a visible component, find the first component with errors and ensure it is visible
                if (!visibleComponentHasErrors)
                {
                    IApplicationComponent firstComponentWithErrors = CollectionUtils.SelectFirst(
                        container.ContainedComponents,
                        delegate(IApplicationComponent c) { return c.HasValidationErrors; });

                    if (firstComponentWithErrors != null)
                    {
                        container.EnsureVisible(firstComponentWithErrors);

                        // bug #1644 : call ShowValidation after this component is already visible,
                        // to ensure that the error icons actually show up
                        firstComponentWithErrors.ShowValidation(show);
                    }
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
