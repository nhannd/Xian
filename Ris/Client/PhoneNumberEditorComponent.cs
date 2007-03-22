using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    [ExtensionPoint()]
    public class PhoneNumbersEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [AssociateView(typeof(PhoneNumbersEditorComponentViewExtensionPoint))]
    public class PhoneNumberEditorComponent : ApplicationComponent
    {
        private TelephoneDetail _phoneNumber;
        private IList<EnumValueInfo> _phoneTypeChoices;

        public PhoneNumberEditorComponent(TelephoneDetail phoneNumber, IList<EnumValueInfo> phoneTypeChoices)
        {
            _phoneNumber = phoneNumber;
            _phoneTypeChoices = phoneTypeChoices;
        }

        public override void Start()
        {
            base.Start();
        }

        /// <summary>
        /// Sets the subject upon which the editor acts
        /// Not for use by the view
        /// </summary>
        public TelephoneDetail PhoneNumber
        {
            get { return _phoneNumber; }
            set { _phoneNumber = value; }
        }

        public string PhoneNumberMask
        {
            get { return TextFieldMasks.TelephoneNumberLocalMask; }
        }

        public string CountryCode
        {
            get { return _phoneNumber.CountryCode; }
            set
            {
                _phoneNumber.CountryCode = value;
                this.Modified = true;
            }
        }

        public string AreaCode
        {
            get { return _phoneNumber.AreaCode; }
            set
            {
               _phoneNumber.AreaCode = value;
               this.Modified = true;
            }
        }

        public string Number
        {
            get { return _phoneNumber.Number; }
            set
            {
                _phoneNumber.Number = value;
                this.Modified = true;
            }
        }

        public string Extension
        {
            get { return _phoneNumber.Extension; }
            set
            {
                _phoneNumber.Extension = value;
                this.Modified = true;
            }
        }

        public string PhoneType
        {
            get { return _phoneNumber.Type.Value; }
            set
            {
                _phoneNumber.Type = EnumValueUtils.MapDisplayValue(_phoneTypeChoices, value);

                this.Modified = true;
            }
        }

        public List<string> PhoneTypeChoices
        {
            get { return EnumValueUtils.GetDisplayValues(_phoneTypeChoices); }
        }

        public DateTime? ValidFrom
        {
            get { return _phoneNumber.ValidRangeFrom; }
            set
            {
                _phoneNumber.ValidRangeFrom = value;
                this.Modified = true;
            }
        }

        public DateTime? ValidUntil
        {
            get { return _phoneNumber.ValidRangeUntil; }
            set
            {
                _phoneNumber.ValidRangeUntil = value;
                this.Modified = true;
            }
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
