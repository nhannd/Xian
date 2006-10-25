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
    public class PatientProfilePreviewComponent : ApplicationComponent
    {
        class PatientPreviewToolContext : ToolContext, IPatientPreviewToolContext
        {
            private PatientProfilePreviewComponent _component;
            public PatientPreviewToolContext(PatientProfilePreviewComponent component)
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
        public PatientProfilePreviewComponent()
            :this(true)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public PatientProfilePreviewComponent(bool showHeader)
        {
            _showHeader = showHeader;
        }

        public PatientProfile Subject
        {
            get { return _subject; }
            set
            {
                if (_subject != value)
                {
                    _subject = value;
                    if (this.IsStarted)
                    {
                        UpdateDisplay();
                    }
                }
            }
        }

        public override void Start()
        {
            _adtService = ApplicationContext.GetService<IAdtService>();

            _addressTypes = _adtService.GetAddressTypeEnumTable();
            _phoneEquipments = _adtService.GetTelephoneEquipmentEnumTable();
            _phoneUses = _adtService.GetTelephoneUseEnumTable();
            _sexChoices = _adtService.GetSexEnumTable();

            _addresses = new Table<Address>();
            _addresses.Columns.Add(new TableColumn<Address, string>("Type",
                delegate(Address a) { return _addressTypes[a.Type].Value; }, 1.0f));
            _addresses.Columns.Add(new TableColumn<Address, string>("Address",
                delegate(Address a) { return a.Format(); }, 3.0f));
            _addresses.Columns.Add(new TableColumn<Address, string>("Expired On",
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
            _phoneNumbers.Columns.Add(new TableColumn<TelephoneNumber, string>("Expired On",
                delegate(TelephoneNumber t) { return t.ValidRange == null ? null : Format.Date(t.ValidRange.Until); }, 1.0f));

            _toolSet = new ToolSet(new PatientPreviewToolExtensionPoint(), new PatientPreviewToolContext(this));

            UpdateDisplay();
            
            base.Start();
        }

        public override void Stop()
        {
            _toolSet.Dispose();

            base.Stop();
        }

        private void UpdateDisplay()
        {
            _addresses.Items.Clear();
            _phoneNumbers.Items.Clear();

            if (_subject != null)
            {
                if (!_subject.IsNew)
                {
                    _adtService.LoadPatientProfileDetails(_subject);
                }

                _addresses.Items.AddRange(_subject.Addresses);
                _addresses.Items.Remove(_subject.CurrentHomeAddress);
                //_addresses.Items.Remove(_subject.CurrentWorkAddress);
                
                _phoneNumbers.Items.AddRange(_subject.TelephoneNumbers);
                _phoneNumbers.Items.Remove(_subject.CurrentHomePhone);
                //_phoneNumbers.Items.Remove(_subject.CurrentWorkPhone);
            }

            NotifyAllPropertiesChanged();
        }

        #region Presentation Model

        public bool ShowHeader
        {
            get { return _showHeader; }
            set { _showHeader = value; }
        }

        public bool HasUnreconciledMatches
        {
            get
            {
                bool hasMatches = false;
                IList<PatientProfileMatch> matches = _adtService.FindPatientReconciliationMatches(_subject);
                hasMatches = matches.Count > 0 ? true : false;
                return hasMatches;
            }
        }

        public string Name
        {
            get { return _subject.Name.Format(); }
        }

        public string DateOfBirth
        {
            get { return ClearCanvas.Desktop.Format.Date(_subject.DateOfBirth); }
        }

        public string Mrn
        {
            get { return _subject.MRN.Format(); }
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
