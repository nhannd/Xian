using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.PatientReconciliation;

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
            private bool _isDiscrepant;

            public Field(string heading, bool isDiscrepant, string leftValue, string rightValue, string diffMask)
            {
                _heading = heading;
                _isDiscrepant = isDiscrepant;
                BuildValues(leftValue, rightValue, diffMask);
            }

            private void BuildValues(string leftValue, string rightValue, string diffMask)
            {
                _values = new List<Value>();
                List<String> ls1 = new List<String>();
                List<String> ls2 = new List<String>();

                if (!_isDiscrepant)
                {
                    ls1.Add(leftValue);
                    _values.Add(new Value(ls1));
                    ls2.Add(rightValue);
                    _values.Add(new Value(ls2));
                    return;
                }

                char[] dm = diffMask.ToCharArray();
                char[] al = leftValue.ToCharArray();
                char[] ar = rightValue.ToCharArray();
                char diagCh = '|';
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
                        sb1.Append(leftValue.Substring(i - n1, 1));
                    else
                        n1 += 1;
                    if (ar[i].Equals(' '))
                        sb2.Append(rightValue.Substring(i - n2, 1));
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
                get { return _isDiscrepant; }
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
        private EntityRef[] _profileRefs;

        private IList<string> _profileAuthorities;

        /// <summary>
        /// Constructor
        /// </summary>
        public PatientProfileDiffComponent()
        {
            _fields = new List<Field>();
        }

        public EntityRef[] ProfilesToCompare
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

            if (_profileRefs != null && _profileRefs.Length == 2)
            {
                Platform.GetService<IPatientReconciliationService>(
                    delegate(IPatientReconciliationService service)
                    {
                        PatientProfileDiff diffData = 
                            service.LoadPatientProfileDiff(new LoadPatientProfileDiffRequest(_profileRefs[0], _profileRefs[1])).ProfileDiff;

                        _profileAuthorities = new List<string>(new string[] { diffData.LeftProfileAssigningAuthority, diffData.RightProfileAssigningAuthority });

                        AddField(SR.ColumnHealthcardNumber, diffData.Healthcard);
                        AddField(SR.ColumnFamilyName, diffData.FamilyName);
                        AddField(SR.ColumnGivenName, diffData.GivenName);
                        AddField(SR.ColumnMiddleName, diffData.MiddleName);
                        AddField(SR.ColumnDateOfBirth, diffData.DateOfBirth);
                        AddField(SR.ColumnSex, diffData.Sex);

                        AddField(SR.ColumnHomePhone, diffData.HomePhone);
                        AddField(SR.ColumnWorkPhone, diffData.WorkPhone);
                        AddField(SR.ColumnHomeAddress, diffData.HomeAddress);
                        AddField(SR.ColumnWorkAddress, diffData.WorkAddress);
                    });

            }

            NotifyAllPropertiesChanged();
        }

        private void AddField(string heading, PropertyDiff propertyDiff)
        {
            _fields.Add(new Field(heading, propertyDiff.IsDiscrepant, propertyDiff.AlignedLeftValue, propertyDiff.AlignedRightValue, propertyDiff.DiffMask));
        }

    }

}
