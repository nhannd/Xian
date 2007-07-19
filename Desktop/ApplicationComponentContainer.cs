using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Abstract base class for application components that act as containers for other application components.
    /// </summary>
    public abstract class ApplicationComponentContainer : ApplicationComponent, IApplicationComponentContainer
    {
        private IApplicationComponentContainerValidationStrategy _validationStrategy;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ApplicationComponentContainer()
        {
            _validationStrategy = new NoNodesContainerValidationStrategy();
        }

        /// <summary>
        /// Gets or sets the validation strategy that determines how this container responds
        /// to validation requests.
        /// </summary>
        public IApplicationComponentContainerValidationStrategy ValidationStrategy
        {
            get { return _validationStrategy; }
            set { _validationStrategy = value; }
        }

        /// <summary>
        /// Gets a value indicating whether there are any data validation errors.
        /// </summary>
        /// <remarks>
        /// The default implementation of this property delegates to the <see cref="ValidationStrategy"/> object.
        /// Invoking this property may cause any unstarted components in the container to be started,
        /// which means that it may throw exceptions.
        /// </remarks>
        public override bool HasValidationErrors
        {
            get
            {
                return _validationStrategy.HasValidationErrors(this);
            }
        }

        /// <summary>
        /// Sets the <see cref="ValidationVisible"/> property and raises the <see cref="ValidationVisibleChanged"/> event.
        /// </summary>
        /// <remarks>
        /// The default implementation of this property delegates to the <see cref="ValidationStrategy"/> object.
        /// Invoking this property may cause any unstarted components in the container to be started,
        /// which means that it may throw exceptions.
        /// </remarks>
        /// <param name="show"></param>
        public override void ShowValidation(bool show)
        {
            _validationStrategy.ShowValidation(this, show);
        }

        #region IApplicationComponentContainer Members

        /// <summary>
        /// Gets an enumeration of the contained components.
        /// </summary>
        public abstract IEnumerable<IApplicationComponent> ContainedComponents { get; }

        /// <summary>
        /// Gets an enumeration of the components that are currently visible.
        /// </summary>
        public abstract IEnumerable<IApplicationComponent> VisibleComponents { get; }

        /// <summary>
        /// Ensures that the specified component is visible.
        /// </summary>
        /// <param name="component"></param>
        public abstract void EnsureVisible(IApplicationComponent component);

        /// <summary>
        /// Ensures that the specified component has been started.
        /// </summary>
        /// <param name="component"></param>
        public abstract void EnsureStarted(IApplicationComponent component);

        #endregion
    }
}
