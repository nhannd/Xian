using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="ContactPersonEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ContactPersonEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ContactPersonEditorComponent class
    /// </summary>
    [AssociateView(typeof(ContactPersonEditorComponentViewExtensionPoint))]
    public class ContactPersonEditorComponent : ApplicationComponent
    {
        private ContactPersonDetail _contactPerson;
        private IList<EnumValueInfo> _contactTypeChoices;
        private IList<EnumValueInfo> _contactRelationshipChoices;

        /// <summary>
        /// Constructor
        /// </summary>
        public ContactPersonEditorComponent(ContactPersonDetail contactPerson, IList<EnumValueInfo> contactTypeChoices, IList<EnumValueInfo> contactRelationshipChoices)
        {
            _contactPerson = contactPerson;
            _contactTypeChoices = contactTypeChoices;
            _contactRelationshipChoices = contactRelationshipChoices;
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

        public ContactPersonDetail ContactPerson
        {
            get { return _contactPerson; }
            set { _contactPerson = value; }
        }

        public string Name
        {
            get { return _contactPerson.Name; }
            set
            {
                _contactPerson.Name = value;
                this.Modified = true;
            }
        }

        public string Address
        {
            get { return _contactPerson.Address; }
            set
            {
                _contactPerson.Address = value;
                this.Modified = true;
            }
        }

        public string HomePhoneNumber
        {
            get { return _contactPerson.HomePhoneNumber; }
            set
            {
                _contactPerson.HomePhoneNumber = value;
                this.Modified = true;
            }
        }

        public string BusinessPhoneNumber
        {
            get { return _contactPerson.BusinessPhoneNumber; }
            set
            {
                _contactPerson.BusinessPhoneNumber = value;
                this.Modified = true;
            }
        }

        public string PhoneNumberMask
        {
            get { return TextFieldMasks.TelphoneNumberFullMask; }
        }

        public string Type
        {
            get { return _contactPerson.Type.Value; }
            set
            {
                _contactPerson.Type = EnumValueUtils.MapDisplayValue(_contactTypeChoices, value);
                this.Modified = true;
            }
        }

        public List<string> TypeChoices
        {
            get { return EnumValueUtils.GetDisplayValues(_contactTypeChoices); }
        }

        public string Relationship
        {
            get { return _contactPerson.Relationship.Value; }
            set
            {
                _contactPerson.Relationship = EnumValueUtils.MapDisplayValue(_contactRelationshipChoices, value);
                this.Modified = true;
            }
        }

        public List<string> RelationshipChoices
        {
            get { return EnumValueUtils.GetDisplayValues(_contactRelationshipChoices); }
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
