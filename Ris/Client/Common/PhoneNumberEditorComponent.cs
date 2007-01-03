using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;
using ClearCanvas.Ris.Services;

namespace ClearCanvas.Ris.Client.Common
{
    [ExtensionPoint()]
    public class PhoneNumbersEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [AssociateView(typeof(PhoneNumbersEditorComponentViewExtensionPoint))]
    public class PhoneNumberEditorComponent : ApplicationComponent
    {
        private TelephoneNumber _phoneNumber;
        private IPatientAdminService _patientAdminService;
        private TelephoneEquipmentEnumTable _phoneEquipments;
        private TelephoneUseEnumTable _phoneUses;

        private string _phoneNumberMask;

        public PhoneNumberEditorComponent(TelephoneNumber phoneNumber)
        {
            _phoneNumber = phoneNumber;
        }

        public override void Start()
        {
            base.Start();
            _patientAdminService = ApplicationContext.GetService<IPatientAdminService>();
            _phoneEquipments = _patientAdminService.GetTelephoneEquipmentEnumTable();
            _phoneUses = _patientAdminService.GetTelephoneUseEnumTable();

            _phoneNumberMask = "000-0000";
        }

        /// <summary>
        /// Sets the subject upon which the editor acts
        /// Not for use by the view
        /// </summary>
        public TelephoneNumber PhoneNumber
        {
            get { return _phoneNumber; }
            set { _phoneNumber = value; }
        }

        public string PhoneNumberMask
        {
            get { return _phoneNumberMask; }
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
            // We will display only allow "Home", "Work" and "Mobile" in the dropdown
            get
            {
                if (_phoneNumber.Use == TelephoneUse.PRN && _phoneNumber.Equipment == TelephoneEquipment.CP)
                    return SR.PhoneNumberMobile;
                else
                    return _phoneUses[_phoneNumber.Use].Value;
            }
            set
            {
                if (value == SR.PhoneNumberMobile)
                {
                    _phoneNumber.Use = TelephoneUse.PRN;
                    _phoneNumber.Equipment = TelephoneEquipment.CP;
                }
                else
                {
                    _phoneNumber.Equipment = TelephoneEquipment.PH;
                    if (value == _phoneUses[TelephoneUse.WPN].Value)
                        _phoneNumber.Use = TelephoneUse.WPN;
                    else
                        _phoneNumber.Use = TelephoneUse.PRN;
                }

                this.Modified = true;
            }
        }

        public string[] PhoneTypeChoices
        {
            get
            {
                return new string[] { _phoneUses[TelephoneUse.PRN].Value
                                    , _phoneUses[TelephoneUse.WPN].Value
                                    , SR.PhoneNumberMobile };
            }
        }

        public DateTime? ValidFrom
        {
            get { return _phoneNumber.ValidRange.From; }
            set
            {
                _phoneNumber.ValidRange.From = value;
                this.Modified = true;
            }
        }

        public DateTime? ValidUntil
        {
            get { return _phoneNumber.ValidRange.Until; }
            set
            {
                _phoneNumber.ValidRange.Until = value;
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
