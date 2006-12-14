using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Healthcare;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise;
using ClearCanvas.Ris.Services;

namespace ClearCanvas.Ris.Client.Adt
{
    /// <summary>
    /// Extension point for views onto <see cref="PatientProfileDiffComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class PatientProfileDiffComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// PatientProfileDiffComponent class
    /// </summary>
    [AssociateView(typeof(PatientProfileDiffComponentViewExtensionPoint))]
    public class PatientProfileDiffComponent : ApplicationComponent
    {
        public class Field
        {
            private string _heading;
            private IList<string> _values;
            private bool _discrepancy;

            public Field(string heading, IList<string> values, bool discrepancy)
            {
                _heading = heading;
                _values = values;
                _discrepancy = discrepancy;
            }

            public string Heading
            {
                get { return _heading; }
            }

            public IList<string> Values
            {
                get { return _values; }
            }

            public bool IsDiscrepancy
            {
                get { return _discrepancy; }
            }
        }

        private List<Field> _fields;
        private EntityRef<PatientProfile>[] _profileRefs;
        private IAdtService _adtService;
        private SexEnumTable _sexEnumTable;

        private IList<string> _profileAuthorities;

        /// <summary>
        /// Constructor
        /// </summary>
        public PatientProfileDiffComponent()
        {
            _fields = new List<Field>();
        }

        public EntityRef<PatientProfile>[] ProfilesToCompare
        {
            get { return _profileRefs; }
            set
            {
                _profileRefs = value;

                if (this.IsStarted)
                {
                    Refresh();
                }
            }
        }

        public override void Start()
        {
            _adtService = ApplicationContext.GetService<IAdtService>();
            _sexEnumTable = _adtService.GetSexEnumTable();

            Refresh();

            base.Start();
        }

        public override void Stop()
        {
            // This is a good place to do any clean up
            base.Stop();
        }

        #region Presentation Model

        public IList<string> ProfileAuthorities
        {
            get { return _profileAuthorities; }
        }

        public IList<Field> Fields
        {
            get { return _fields; }
        }

        #endregion


        private void Refresh()
        {
            _fields.Clear();

            if (_profileRefs != null && _profileRefs.Length > 1)
            {
                PatientProfileDiff diffData = _adtService.LoadPatientProfileDiff(_profileRefs, PatientProfileDiscrepancy.All);

                _profileAuthorities = CollectionUtils.Map<PatientProfile, string, List<string>>(diffData.Profiles, delegate(PatientProfile p) { return p.Mrn.AssigningAuthority; });

                AddField(SR.ColumnHealthcardNumber, PatientProfileDiscrepancy.Healthcard, diffData, delegate(PatientProfile p) { return p.Healthcard.Format(); });
                AddField(SR.ColumnFamilyName, PatientProfileDiscrepancy.FamilyName, diffData, delegate(PatientProfile p) { return p.Name.GivenName; });
                AddField(SR.ColumnGivenName, PatientProfileDiscrepancy.GivenName, diffData, delegate(PatientProfile p) { return p.Name.FamilyName; });
                AddField(SR.ColumnMiddleName, PatientProfileDiscrepancy.MiddleName, diffData, delegate(PatientProfile p) { return p.Name.MiddleName; });
                AddField(SR.ColumnDateOfBirth, PatientProfileDiscrepancy.DateOfBirth, diffData, delegate(PatientProfile p) { return Format.Date(p.DateOfBirth); });
                AddField(SR.ColumnSex, PatientProfileDiscrepancy.Sex, diffData, delegate(PatientProfile p) { return _sexEnumTable[p.Sex].Value; });

                AddField(SR.ColumnHomePhone, PatientProfileDiscrepancy.HomePhone, diffData,
                    delegate(PatientProfile p) { return p.CurrentHomePhone == null ? "Unknown" : p.CurrentHomePhone.Format(); });
                AddField(SR.ColumnWorkPhone, PatientProfileDiscrepancy.WorkPhone, diffData,
                    delegate(PatientProfile p) { return p.CurrentWorkPhone == null ? "Unknown" : p.CurrentWorkPhone.Format(); });
                AddField(SR.ColumnHomeAddress, PatientProfileDiscrepancy.HomeAddress, diffData,
                    delegate(PatientProfile p) { return p.CurrentHomeAddress == null ? "Unknown" : p.CurrentHomeAddress.Format(); });
                AddField(SR.ColumnWorkAddress, PatientProfileDiscrepancy.WorkAddress, diffData,
                    delegate(PatientProfile p) { return p.CurrentWorkAddress == null ? "Unknown" : p.CurrentWorkAddress.Format(); });
            }

            NotifyAllPropertiesChanged();
        }

        private void AddField(string heading, PatientProfileDiscrepancy test, PatientProfileDiff data, Converter<PatientProfile, string> fieldValueGetter)
        {
            List<string> values = CollectionUtils.Map<PatientProfile, string, List<string>>(data.Profiles, fieldValueGetter);
            _fields.Add(new Field(heading, values, (test & data.Discrepancies) != 0));
        }

    }
}
