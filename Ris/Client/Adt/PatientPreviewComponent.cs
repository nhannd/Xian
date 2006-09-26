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

namespace ClearCanvas.Ris.Client.Adt
{
    /// <summary>
    /// Extension point for views onto <see cref="PatientPreviewComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class PatientPreviewComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// PatientPreviewComponent class
    /// </summary>
    [AssociateView(typeof(PatientPreviewComponentViewExtensionPoint))]
    public class PatientPreviewComponent : ApplicationComponent
    {

        private PatientProfile _subject;
        private TableData<Address> _addresses;
        private TableData<TelephoneNumber> _phoneNumbers;

        private IAdtService _adtService;

        /// <summary>
        /// Constructor
        /// </summary>
        public PatientPreviewComponent()
        {
        }

        public PatientProfile Subject
        {
            get { return _subject; }
            set
            {
                if (_subject != value)
                {
                    _subject = value;
                    _adtService.LoadPatientProfileDetails(_subject);

                    _addresses.Clear();
                    _addresses.AddRange(_subject.Addresses);

                    _phoneNumbers.Clear();
                    _phoneNumbers.AddRange(_subject.TelephoneNumbers);

                    NotifyAllPropertiesChanged();
                }
            }
        }

        public override void Start()
        {
            _adtService = ApplicationContext.GetService<IAdtService>();

            _addresses = new TableData<Address>();
            _addresses.Columns.Add(new TableColumn<Address, string>("Type", delegate(Address a) { return _adtService.AddressTypeEnumTable[a.Type].Value; }));
            _addresses.Columns.Add(new TableColumn<Address, string>("Address", delegate(Address a) { return a.Format(); }));
            _addresses.Columns.Add(new TableColumn<Address, string>("Valid From", delegate(Address a) { return Format.Date(a.ValidFrom); }));
            _addresses.Columns.Add(new TableColumn<Address, string>("Valid Until", delegate(Address a) { return Format.Date(a.ValidUntil); }));


            _phoneNumbers = new TableData<TelephoneNumber>();
            _phoneNumbers.Columns.Add(new TableColumn<TelephoneNumber, string>("Use", delegate(TelephoneNumber t) { return _adtService.TelephoneUseEnumTable[t.Use].Value; }));
            _phoneNumbers.Columns.Add(new TableColumn<TelephoneNumber, string>("Cell", delegate(TelephoneNumber t) { return t.Equipment == TelephoneEquipment.CP ? "Yes" : "No"; }));
            _phoneNumbers.Columns.Add(new TableColumn<TelephoneNumber, string>("Number", delegate(TelephoneNumber t) { return t.Format(); }));

            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
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

        public string Mrn
        {
            get { return _subject.MRN.Id; }
        }

        public string Healthcard
        {
            get { return _subject.Healthcard.Id; }
        }

        public string Sex
        {
            get { return _adtService.SexEnumTable[_subject.Sex].Value; }
        }

        public ITableData Addresses
        {
            get { return _addresses; }
        }

        public ITableData PhoneNumbers
        {
            get { return _phoneNumbers; }
        }


        #endregion
    }
}
