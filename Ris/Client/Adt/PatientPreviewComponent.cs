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

        private IAdtService _adtService;

        private ToolSet _toolSet;

        /// <summary>
        /// Constructor
        /// </summary>
        public PatientPreviewComponent()
        {
            _adtService = ApplicationContext.GetService<IAdtService>();

            _addresses = new Table<Address>();
            _addresses.Columns.Add(new TableColumn<Address, string>("Type", delegate(Address a) { return _adtService.AddressTypeEnumTable[a.Type].Value; }));
            _addresses.Columns.Add(new TableColumn<Address, string>("Address", delegate(Address a) { return a.Format(); }));
            _addresses.Columns.Add(new TableColumn<Address, string>("Valid From", delegate(Address a) { return a.ValidRange == null ? null : Format.Date(a.ValidRange.From); }));
            _addresses.Columns.Add(new TableColumn<Address, string>("Valid Until", delegate(Address a) { return a.ValidRange == null ? null : Format.Date(a.ValidRange.Until); }));


            _phoneNumbers = new Table<TelephoneNumber>();
            _phoneNumbers.Columns.Add(new TableColumn<TelephoneNumber, string>("Type",
                delegate(TelephoneNumber t)
                {
                    return string.Format("{0} {1}",
                        _adtService.TelephoneUseEnumTable[t.Use].Value,
                        t.Equipment == TelephoneEquipment.CP ? _adtService.TelephoneEquipmentEnumTable[t.Equipment].Value : "");
                }));
            _phoneNumbers.Columns.Add(new TableColumn<TelephoneNumber, string>("Number", delegate(TelephoneNumber t) { return t.Format(); }));
            _phoneNumbers.Columns.Add(new TableColumn<TelephoneNumber, string>("Valid From", delegate(TelephoneNumber t) { return ""; }));
            _phoneNumbers.Columns.Add(new TableColumn<TelephoneNumber, string>("Valid Until", delegate(TelephoneNumber t) { return ""; }));
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
            get { return _adtService.SexEnumTable[_subject.Sex].Value; }
        }

        public string CurrentHomeAddress
        {
            get
            {
                Address address = _subject.CurrentHomeAddress;
                return (address == null) ? null : address.Format();
            }
        }

        public string CurrentHomePhone
        {
            get
            {
                TelephoneNumber phone = _subject.CurrentHomePhone;
                return (phone == null) ? null : phone.Format();
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
