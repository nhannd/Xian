using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="EmailAddressEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class EmailAddressEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// EmailAddressEditorComponent class
    /// </summary>
    [AssociateView(typeof(EmailAddressEditorComponentViewExtensionPoint))]
    public class EmailAddressEditorComponent : ApplicationComponent
    {
        private EmailAddressDetail _emailAddress;

        /// <summary>
        /// Constructor
        /// </summary>
        public EmailAddressEditorComponent(EmailAddressDetail emailAddress)
        {
            _emailAddress = emailAddress;
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

        #region Presentation Model

        public EmailAddressDetail EmailAddress
        {
            get { return _emailAddress; }
            set { _emailAddress = value; }
        }

        public string Address
        {
            get { return _emailAddress.Address; }
            set
            {
                _emailAddress.Address = value;
                this.Modified = true;
            }
        }


        public DateTime? ValidFrom
        {
            get { return _emailAddress.ValidRangeFrom; }
            set
            {
                _emailAddress.ValidRangeFrom = value;
                this.Modified = true;
            }
        }

        public DateTime? ValidUntil
        {
            get { return _emailAddress.ValidRangeUntil; }
            set
            {
                _emailAddress.ValidRangeUntil = value;
                this.Modified = true;
            }
        }

        public void Accept()
        {
            if (this.HasValidationErrors)
            {
                this.ShowValidation(true);
            }
            else
            {
                this.ExitCode = ApplicationComponentExitCode.Normal;
                Host.Exit();
            }
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            Host.Exit();
        }

        public bool AcceptEnabled
        {
            get { return this.Modified; }
        }

        public event EventHandler AcceptEnabledChanged
        {
            add { this.ModifiedChanged += value; }
            remove { this.ModifiedChanged -= value; }
        }

        #endregion
    }
}
