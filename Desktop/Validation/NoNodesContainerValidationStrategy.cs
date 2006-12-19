using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Validation
{
    /// <summary>
    /// Implements a validation strategy that does not consider any nodes.  This is effectively equivalent
    /// to having no validation at all.  The container is always considered valid, regardless of the validity
    /// of contained nodes.
    /// </summary>
    public class NoNodesContainerValidationStrategy : IApplicationComponentContainerValidationStrategy
    {
        #region IApplicationComponentContainerValidationStrategy Members

        public bool HasValidationErrors(IApplicationComponentContainer container)
        {
            return false;
        }

        public void ShowValidation(IApplicationComponentContainer container, bool show)
        {
            // do nothing
        }

        #endregion
    }
}
