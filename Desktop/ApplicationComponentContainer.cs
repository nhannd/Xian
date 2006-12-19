using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Base class for application components that act as containers for other application components.
    /// </summary>
    public abstract class ApplicationComponentContainer : ApplicationComponent, IApplicationComponentContainer
    {
        private IApplicationComponentContainerValidationStrategy _validationStrategy;

        /// <summary>
        /// Constructor
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
        /// The behaviour of this property depends on the <see cref="ValidationStrategy"/> property
        /// </summary>
        public override bool HasValidationErrors
        {
            get
            {
                return _validationStrategy.HasValidationErrors(this);
            }
        }

        /// <summary>
        /// The behaviour of this property depends on the <see cref="ValidationStrategy"/> property
        /// </summary>
        /// <param name="show"></param>
        public override void ShowValidation(bool show)
        {
            _validationStrategy.ShowValidation(this, show);
        }

        #region IApplicationComponentContainer Members

        public abstract IEnumerable<IApplicationComponent> ContainedComponents { get; }

        public abstract IEnumerable<IApplicationComponent> VisibleComponents { get; }

        public abstract void EnsureVisible(IApplicationComponent component);

        public abstract void EnsureStarted(IApplicationComponent component);

        #endregion
    }
}
