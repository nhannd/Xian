#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
