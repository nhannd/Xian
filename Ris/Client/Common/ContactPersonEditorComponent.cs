using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Services;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Ris.Client.Common
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
        private ContactPerson _contactPerson;
        private ContactPersonRelationshipEnumTable _relationshipTypes;
        private ContactPersonTypeEnumTable _typeTypes;
        private IAdtService _service;

        /// <summary>
        /// Constructor
        /// </summary>
        public ContactPersonEditorComponent(ContactPerson contactPerson)
        {
            _contactPerson = contactPerson;
        }

        public override void Start()
        {
            // TODO prepare the component for its live phase
            base.Start();

            _service = ApplicationContext.GetService<IAdtService>();

            _relationshipTypes = _service.GetContactPersonRelationshipEnumTable();
            _typeTypes = _service.GetContactPersonTypeEnumTable();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        #region Presentation Model

        public ContactPerson ContactPerson
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
            get { return _contactPerson.HomePhone; }
            set
            {
                _contactPerson.HomePhone = value;
                this.Modified = true;
            }
        }

        public string BusinessPhoneNumber
        {
            get { return _contactPerson.BusinessPhone; }
            set
            {
                _contactPerson.BusinessPhone = value;
                this.Modified = true;
            }
        }

        public string Type
        {
            get { return _typeTypes[_contactPerson.Type].Value; }
            set
            {
                _contactPerson.Type = _typeTypes[value].Code;
                this.Modified = true;
            }
        }

        public string[] TypeChoices
        {
            get { return _typeTypes.Values; }
        }

        public string Relationship
        {
            get { return _relationshipTypes[_contactPerson.Relationship].Value; }
            set
            {
                _contactPerson.Relationship = _relationshipTypes[value].Code;
                this.Modified = true;
            }
        }

        public string[] RelationshipChoices
        {
            get { return _relationshipTypes.Values; }
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
