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
            private List<Value> _values;
            private bool _discrepancy;

            public Field(string heading, IList<string> fieldValues, bool discrepancy)
            {
                _heading = heading;
                _discrepancy = discrepancy;
                BuildValues(fieldValues);
            }

            private void BuildValues(IList<string> fieldValues)
            {
                _values = new List<Value>();
                if (fieldValues == null || fieldValues.Count < 2)
                {
                    return;
                }
                List<String> ls1 = new List<String>();
                if (!_discrepancy)
                {
                    ls1.Add(fieldValues[0]);
                    _values.Add(new Value(ls1));
                    ls1.Clear();
                    ls1.Add(fieldValues[1]);
                    _values.Add(new Value(ls1));
                    return;
                }
                StringDiff diff = new StringDiff(fieldValues[0].ToLower(), fieldValues[1].ToLower());
                char[] dm = diff.DiffMask.ToCharArray();
                char[] al = diff.AlignedLeft.ToCharArray();
                char[] ar = diff.AlignedRight.ToCharArray();
                char diagCh = '|';
                List<String> ls2 = new List<String>();
                StringBuilder sb1 = new StringBuilder();
                StringBuilder sb2 = new StringBuilder();
                int n1 = 0;
                int n2 = 0;
                for (int i = 0; i < dm.Length; i++)
                {
                    if (!dm[i].Equals(diagCh))
                    {
                        ls1.Add(sb1.ToString());
                        ls2.Add(sb2.ToString());
                        diagCh = dm[i];
                        sb1 = new StringBuilder();
                        sb2 = new StringBuilder();
                    }
                    if (al[i].Equals(' '))
                        sb1.Append(fieldValues[0].Substring(i - n1, 1));
                    else
                        n1 += 1;
                    if (ar[i].Equals(' '))
                        sb2.Append(fieldValues[1].Substring(i - n2, 1));
                    else
                        n2 += 1;
                }
                ls1.Add(sb1.ToString());
                ls2.Add(sb2.ToString());
                _values.Add(new Value(ls1));
                _values.Add(new Value(ls2));
                return;
            }

            public string Heading
            {
                get { return _heading; }
            }

            public List<Value> Values
            {
                get { return _values; }
            }

            public bool IsDiscrepancy
            {
                get { return _discrepancy; }
            }

        }

        public class Value
        {
            private List<string> _segments;

            public Value(List<string> segments)
            {
                _segments = segments;
            }

            public List<string> Segments
            {
                get { return _segments; }
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

                AddField(SR.ColumnHealthcardNumber, PatientProfileDiscrepancy.Healthcard, diffData, delegate(PatientProfile p) { return Format.Custom(p.Healthcard); });
                AddField(SR.ColumnFamilyName, PatientProfileDiscrepancy.FamilyName, diffData, delegate(PatientProfile p) { return p.Name.GivenName; });
                AddField(SR.ColumnGivenName, PatientProfileDiscrepancy.GivenName, diffData, delegate(PatientProfile p) { return p.Name.FamilyName; });
                AddField(SR.ColumnMiddleName, PatientProfileDiscrepancy.MiddleName, diffData, delegate(PatientProfile p) { return p.Name.MiddleName; });
                AddField(SR.ColumnDateOfBirth, PatientProfileDiscrepancy.DateOfBirth, diffData, delegate(PatientProfile p) { return Format.Date(p.DateOfBirth); });
                AddField(SR.ColumnSex, PatientProfileDiscrepancy.Sex, diffData, delegate(PatientProfile p) { return _sexEnumTable[p.Sex].Value; });

                AddField(SR.ColumnHomePhone, PatientProfileDiscrepancy.HomePhone, diffData,
                    delegate(PatientProfile p) { return p.CurrentHomePhone == null ? "" : Format.Custom(p.CurrentHomePhone); });
                AddField(SR.ColumnWorkPhone, PatientProfileDiscrepancy.WorkPhone, diffData,
                    delegate(PatientProfile p) { return p.CurrentWorkPhone == null ? "" : Format.Custom(p.CurrentWorkPhone); });
                AddField(SR.ColumnHomeAddress, PatientProfileDiscrepancy.HomeAddress, diffData,
                    delegate(PatientProfile p) { return p.CurrentHomeAddress == null ? "" : Format.Custom(p.CurrentHomeAddress); });
                AddField(SR.ColumnWorkAddress, PatientProfileDiscrepancy.WorkAddress, diffData,
                    delegate(PatientProfile p) { return p.CurrentWorkAddress == null ? "" : Format.Custom(p.CurrentWorkAddress); });
            }

            NotifyAllPropertiesChanged();
        }

        private void AddField(string heading, PatientProfileDiscrepancy test, PatientProfileDiff data, Converter<PatientProfile, string> fieldValueGetter)
        {
            List<string> values = CollectionUtils.Map<PatientProfile, string, List<string>>(data.Profiles, fieldValueGetter);
            _fields.Add(new Field(heading, values, (test & data.Discrepancies) != 0));
        }

    }

    /// <summary>
    /// Computes the difference between two strings.  The speed and memory requirements are 
    /// to be O(n2) for this algorithm, so it should not be used on very long strings.
    /// 
    /// Adapted from an algorithm presented here in Javascript:
    /// http://www.csse.monash.edu.au/~lloyd/tildeAlgDS/Dynamic/Edit/
    /// </summary>
    public class StringDiff
    {
        private string _alignedLeft;
        private string _alignedRight;
        private string _diffMask;

        public StringDiff(string left, string right)
        {
            string[] results = ComputeDiff(left, right);
            _alignedLeft = results[0];
            _alignedRight = results[2];
            _diffMask = results[1];
        }

        public string AlignedLeft
        {
            get { return _alignedLeft; }
        }

        public string AlignedRight
        {
            get { return _alignedRight; }
        }

        public string DiffMask
        {
            get { return _diffMask; }
        }

        private string[] ComputeDiff(string s1, string s2)
        {
            int[,] m = new int[s1.Length + 1, s2.Length + 1];

            m[0, 0] = 0; // boundary conditions

            for (int j = 1; j <= s2.Length; j++)
                m[0, j] = m[0, j - 1] - 0 + 1; // boundary conditions

            for (int i = 1; i <= s1.Length; i++)                            // outer loop
            {
                m[i, 0] = m[i - 1, 0] - 0 + 1; // boundary conditions

                for (int j = 1; j <= s2.Length; j++)                         // inner loop
                {
                    int diag = m[i - 1, j - 1];
                    if (s1[i - 1] != s2[j - 1]) diag++;

                    m[i, j] = Math.Min(diag,               // match or change
                           Math.Min(m[i - 1, j] - 0 + 1,    // deletion
                                     m[i, j - 1] - 0 + 1)); // insertion
                }//for j
            }//for i

            return traceBack("", "", "", m, s1.Length, s2.Length, s1, s2);
        }

        private string[] traceBack(string row1, string row2, string row3, int[,] m, int i, int j, string s1, string s2)
        {
            // recover the alignment of s1 and s2
            if (i > 0 && j > 0)
            {
                int diag = m[i - 1, j - 1];
                char diagCh = '|';

                if (s1[i - 1] != s2[j - 1]) { diag++; diagCh = ' '; }

                if (m[i, j] == diag) //LAllison comp sci monash uni au
                    return traceBack(' ' + row1, diagCh + row2, ' ' + row3,
                              m, i - 1, j - 1, s1, s2);    // change or match
                else if (m[i, j] == m[i - 1, j] - 0 + 1) // delete
                    return traceBack(' ' + row1, ' ' + row2, '-' + row3,
                              m, i - 1, j, s1, s2);
                else
                    return traceBack('-' + row1, ' ' + row2, ' ' + row3,
                              m, i, j - 1, s1, s2);      // insertion
            }
            else if (i > 0)
                return traceBack(' ' + row1, ' ' + row2, '-' + row3, m, i - 1, j, s1, s2);
            else if (j > 0)
                return traceBack('-' + row1, ' ' + row2, ' ' + row3, m, i, j - 1, s1, s2);
            else // i==0 and j==0
            {
                return new string[] { row1, row2, row3 };
            }
        }//traceBack
    }
}
