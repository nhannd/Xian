using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Healthcare;
using ClearCanvas.Common.Utilities;

using System.IO;
using ClearCanvas.Ris.Services;
using ClearCanvas.Enterprise;
using ClearCanvas.Common.Scripting;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client.Adt
{
    /// <summary>
    /// Extension point for views onto <see cref="PatientPreviewComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class PatientPreviewComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [ExtensionPoint]
    public class PatientPreviewToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    public interface IPatientPreviewToolContext : IToolContext
    {
        IDesktopWindow DesktopWindow { get; }
    }

    /// <summary>
    /// PatientPreviewComponent class
    /// </summary>
    [AssociateView(typeof(PatientPreviewComponentViewExtensionPoint))]
    public class PatientPreviewComponent : ApplicationComponent
    {
        class PatientPreviewToolContext : ToolContext, IPatientPreviewToolContext
        {
            private PatientPreviewComponent _component;
            public PatientPreviewToolContext(PatientPreviewComponent component)
            {
                _component = component;
            }

            public IDesktopWindow DesktopWindow
            {
                get { return _component.Host.DesktopWindow; }
            }
        }

        private PatientProfile _subject;
        private Table<Address> _addresses;
        private Table<TelephoneNumber> _phoneNumbers;
        private bool _showHeader;

        private IAdtService _adtService;
        private AddressTypeEnumTable _addressTypes;
        private TelephoneEquipmentEnumTable _phoneEquipments;
        private TelephoneUseEnumTable _phoneUses;
        private SexEnumTable _sexChoices;

        private ToolSet _toolSet;

        /// <summary>
        /// Constructor
        /// </summary>
        public PatientPreviewComponent()
            :this(true)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public PatientPreviewComponent(bool showHeader)
        {
            _adtService = ApplicationContext.GetService<IAdtService>();
            _addressTypes = _adtService.GetAddressTypeEnumTable();
            _phoneEquipments = _adtService.GetTelephoneEquipmentEnumTable();
            _phoneUses = _adtService.GetTelephoneUseEnumTable();
            _sexChoices = _adtService.GetSexEnumTable();

            _showHeader = showHeader;

            string displayValue = EnumUtil.GetDisplayValue(TelephoneEquipment.CP);
            TelephoneEquipment e = (TelephoneEquipment)EnumUtil.GetCode(typeof(TelephoneEquipment), displayValue);

            _addresses = new Table<Address>();
            _addresses.Columns.Add(new TableColumn<Address, string>("Type",
                delegate(Address a) { return _addressTypes[a.Type].Value; }, 1.0f));
            _addresses.Columns.Add(new TableColumn<Address, string>("Address",
                delegate(Address a) { return a.Format(); }, 3.0f));
            _addresses.Columns.Add(new TableColumn<Address, string>("Valid From",
                delegate(Address a) { return a.ValidRange == null ? null : Format.Date(a.ValidRange.From); }, 1.0f));
            _addresses.Columns.Add(new TableColumn<Address, string>("Valid Until",
                delegate(Address a) { return a.ValidRange == null ? null : Format.Date(a.ValidRange.Until); }, 1.0f));


            _phoneNumbers = new Table<TelephoneNumber>();
            _phoneNumbers.Columns.Add(new TableColumn<TelephoneNumber, string>("Type",
                delegate(TelephoneNumber t)
                {
                    return string.Format("{0} {1}",
                        _phoneUses[t.Use].Value,
                        t.Equipment == TelephoneEquipment.CP ? _phoneEquipments[t.Equipment].Value : "");
                }, 1.0f));
            _phoneNumbers.Columns.Add(new TableColumn<TelephoneNumber, string>("Number",
                delegate(TelephoneNumber t) { return t.Format(); }, 3.0f));
            _phoneNumbers.Columns.Add(new TableColumn<TelephoneNumber, string>("Valid From",
                delegate(TelephoneNumber t) { return ""; }, 1.0f));
            _phoneNumbers.Columns.Add(new TableColumn<TelephoneNumber, string>("Valid Until",
                delegate(TelephoneNumber t) { return ""; }, 1.0f));
        }

        public PatientProfile Subject
        {
            get { return _subject; }
            set
            {
                if (_subject != value)
                {
                    _addresses.Items.Clear();
                    _phoneNumbers.Items.Clear();

                    _subject = value;
                    if (_subject != null)
                    {
                        _adtService.LoadPatientProfileDetails(_subject);

                        _addresses.Items.AddRange(_subject.Addresses);
                        _phoneNumbers.Items.AddRange(_subject.TelephoneNumbers);
                    }

                    NotifyAllPropertiesChanged();
                }
            }
        }

        public override void Start()
        {
            _toolSet = new ToolSet(new PatientPreviewToolExtensionPoint(), new PatientPreviewToolContext(this));
            
            base.Start();
        }

        public override void Stop()
        {
            _toolSet.Dispose();

            base.Stop();
        }

        #region Presentation Model

        public bool ShowHeader
        {
            get { return _showHeader; }
            set { _showHeader = value; }
        }

        public string Name
        {
            get { return _subject.Name.Format(); }
        }

        public string DateOfBirth
        {
            get { return _subject.DateOfBirth.ToShortDateString(); }
        }

        public string MrnId
        {
            get { return _subject.MRN.Id; }
        }

        public string MrnAuthority
        {
            get { return _subject.MRN.AssigningAuthority; }
        }

        public string Healthcard
        {
            get { return _subject.Healthcard.Id; }
        }

        public string Sex
        {
            get { return _sexChoices[_subject.Sex].Value; }
        }

        public string CurrentHomeAddress
        {
            get
            {
                Address address = _subject.CurrentHomeAddress;
                return (address == null) ? "Unknown" : address.Format();
            }
        }

        public string CurrentHomePhone
        {
            get
            {
                TelephoneNumber phone = _subject.CurrentHomePhone;
                return (phone == null) ? "Unknown" : phone.Format();
            }
        }

        public ITable Addresses
        {
            get { return _addresses; }
        }

        public ITable PhoneNumbers
        {
            get { return _phoneNumbers; }
        }

        public ActionModelNode MenuModel
        {
            get
            {
                return ActionModelRoot.CreateModel(this.GetType().FullName, "patientpreview-menu", _toolSet.Actions);
            }
        }

        #endregion
    }
}
