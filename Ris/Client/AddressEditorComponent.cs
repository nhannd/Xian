using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
    [ExtensionPoint()]
    public class AddressesEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [AssociateView(typeof(AddressesEditorComponentViewExtensionPoint))]
    public class AddressEditorComponent : ApplicationComponent
    {
        private AddressDetail _address;
        private IList<EnumValueInfo> _addressTypes;

        public AddressEditorComponent(AddressDetail address, IList<EnumValueInfo> addressTypes)
        {
            _address = address;
            _addressTypes = addressTypes;
        }

        /// <summary>
        /// Sets the subject upon which the editor acts
        /// Not for use by the view
        /// </summary>
        public AddressDetail Address
        {
            get { return _address; }
            set { _address = value; }
        }

        public override void Start()
        {
            base.Start();
        }

        public string Street
        {
            get { return _address.Street; }
            set
            {
                _address.Street = value;
                this.Modified = true;
            }
        }

        public string Unit
        {
            get { return _address.Unit; }
            set
            {
                _address.Unit = value;
                this.Modified = true;
            }
        }

        public string City
        {
            get { return _address.City; }
            set
            {
                _address.City = value;
                this.Modified = true;
            }
        }

        public string Province
        {
            get { return _address.Province; }
            set
            {
                _address.Province = value;
                this.Modified = true;
            }
        }

        public ICollection<string> ProvinceChoices
        {
            get { return AddressEditorComponentSettings.Default.ProvinceChoices; }
        }

        public string Country
        {
            get { return _address.Country; }
            set
            {
                _address.Country = value;
                this.Modified = true;
            }
        }

        public ICollection<string> CountryChoices
        {
            get { return AddressEditorComponentSettings.Default.CountryChoices; }
        }

        public string PostalCode
        {
            get { return _address.PostalCode; }
            set
            {
                _address.PostalCode = value;
                this.Modified = true;
            }
        }

        public DateTime? ValidFrom
        {
            get { return _address.ValidRangeFrom; }
            set {
                _address.ValidRangeFrom = value;
                this.Modified = true;
            }
        }

        public DateTime? ValidUntil
        {
            get { return _address.ValidRangeUntil; }
            set {
                _address.ValidRangeUntil = value;
                this.Modified = true;
            }
        }

        public string Type
        {
            get { return _address.Type.Value; }
            set
            {
                _address.Type = EnumValueUtils.MapDisplayValue(_addressTypes, value);
                this.Modified = true;
            }
        }

        public List<string> TypeChoices
        {
            get { return EnumValueUtils.GetDisplayValues(_addressTypes); }
        }

        public void Accept()
        {
            this.ExitCode = ApplicationComponentExitCode.Normal;
            Host.Exit();
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

    }
}
