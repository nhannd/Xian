using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.Samples.Google.Calendar.Mail
{
    /// <summary>
    /// Extension point for views onto <see cref="EmailComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class EmailComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// EmailComponent class
    /// </summary>
    [AssociateView(typeof(EmailComponentViewExtensionPoint))]
    public class EmailComponent : ApplicationComponent
    {
        private string _emailAddress;

        /// <summary>
        /// Constructor
        /// </summary>
        public EmailComponent()
        {
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        /// <summary>
        /// Gets or sets the email address field.
        /// </summary>
        [ValidateRegex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")]
        public string EmailAddress
        {
            get { return _emailAddress; }
            set { _emailAddress = value; }
        }

        /// <summary>
        /// Called when user presses OK.
        /// </summary>
        public void Accept()
        {
            // check for validation errors
            if (this.HasValidationErrors)
            {
                this.ShowValidation(true);
                return;
            }

            // exit normally
            this.Exit(ApplicationComponentExitCode.Normal);
        }

        /// <summary>
        /// Called when user presses Cancel.
        /// </summary>
        public void Cancel()
        {
            this.Exit(ApplicationComponentExitCode.Cancelled);
        }
    }
}
