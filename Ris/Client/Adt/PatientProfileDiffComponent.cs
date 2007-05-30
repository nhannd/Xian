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
    public class PatientProfileDiffComponent : HtmlApplicationComponent
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
                List<Segment> leftSegments = new List<Segment>();
                List<Segment> rightSegments = new List<Segment>();

                if (!_isDiscrepant)
                {
                    leftSegments.Add(new Segment(leftValue, false));
                    _values.Add(new Value(leftSegments));
                    rightSegments.Add(new Segment(rightValue, false));
                    _values.Add(new Value(rightSegments));
                    return;
                }

                char maskChar = diffMask[0];
                StringBuilder leftSegment = new StringBuilder();
                StringBuilder rightSegment = new StringBuilder();
                for (int i = 0; i < diffMask.Length; i++)
                {
                    if (!diffMask[i].Equals(maskChar))
                    {
                        leftSegments.Add(new Segment(leftSegment.ToString(), maskChar == ' '));
                        rightSegments.Add(new Segment(rightSegment.ToString(), maskChar == ' '));
                        maskChar = diffMask[i];
                        leftSegment = new StringBuilder();
                        rightSegment = new StringBuilder();
                    }
                    leftSegment.Append(leftValue[i]);
                    rightSegment.Append(rightValue[i]);
                }
                leftSegments.Add(new Segment(leftSegment.ToString(), maskChar == ' '));
                rightSegments.Add(new Segment(rightSegment.ToString(), maskChar == ' '));

                _values.Add(new Value(leftSegments));
                _values.Add(new Value(rightSegments));
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
            private List<Segment> _segments;

            public Value(List<Segment> segments)
            {
                _segments = segments;
            }

            public List<Segment> Segments
            {
                get { return _segments; }
            }
        }

        public class Segment
        {
            private string _text;
            private bool _isDiscrepant;

            public Segment(string text, bool discrepant)
            {
                _text = text;
                _isDiscrepant = discrepant;
            }

            public string Text { get { return _text; } }
            public bool IsDiscrepant { get { return _isDiscrepant; } }
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
