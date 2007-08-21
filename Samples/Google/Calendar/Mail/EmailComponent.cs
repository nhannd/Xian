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
            // TODO prepare the component for its live phase
            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        [ValidateRegex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")]
        public string EmailAddress
        {
            get { return _emailAddress; }
            set { _emailAddress = value; }
        }


        public void Accept()
        {
            if (this.HasValidationErrors)
            {
                this.ShowValidation(true);
                return;
            }

            this.Exit(ApplicationComponentExitCode.Normal);
        }

        public void Cancel()
        {
            this.Exit(ApplicationComponentExitCode.Cancelled);
        }
    }
}
