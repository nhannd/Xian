using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Validation
{
    public interface IApplicationComponentContainerValidationStrategy
    {
        /// <summary>
        /// Determines whether the specified container has validation errors, according to this strategy.
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        bool HasValidationErrors(IApplicationComponentContainer container);

        /// <summary>
        /// Displays validation errors for the specified container to the user, according to the logic
        /// encapsulated in this strategy.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="show"></param>
        void ShowValidation(IApplicationComponentContainer container, bool show);
    }
}
