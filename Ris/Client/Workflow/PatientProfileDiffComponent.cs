#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.PatientReconciliation;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Client.Workflow
{
    /// <summary>
    /// PatientProfileDiffComponent class
    /// </summary>
    public class PatientProfileDiffComponent : DHtmlComponent
	{
		#region Healthcare Context

		// Internal data contract used for jscript deserialization
		[DataContract]
		public class HealthcareContext : DataContractBase
		{
			private PatientProfileDiffComponent _owner;
			public HealthcareContext(PatientProfileDiffComponent owner)
			{
				_owner = owner;
			}

			[DataMember]
			public List<string> ProfileAuthorities
			{
				get { return _owner._profileAuthorities; }
			}

			[DataMember]
			public List<Field> Fields
			{
				get { return _owner._fields; }
			}
		}

		[DataContract]
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

			[DataMember]
            public string Heading
            {
                get { return _heading; }
            }

			[DataMember]
			public List<Value> Values
            {
                get { return _values; }
            }

			[DataMember]
			public bool IsDiscrepancy
            {
                get { return _isDiscrepant; }
            }

        }

		[DataContract]
        public class Value
        {
            private List<Segment> _segments;

            public Value(List<Segment> segments)
            {
                _segments = segments;
            }

			[DataMember]
			public List<Segment> Segments
            {
                get { return _segments; }
            }
        }

		[DataContract]
        public class Segment
        {
            private string _text;
            private bool _isDiscrepant;

            public Segment(string text, bool discrepant)
            {
                _text = text;
                _isDiscrepant = discrepant;
            }

			[DataMember]
			public string Text { get { return _text; } }

			[DataMember]
			public bool IsDiscrepant { get { return _isDiscrepant; } }
		}

		#endregion

		private List<Field> _fields;
        private EntityRef[] _profileRefs;

        private List<string> _profileAuthorities;

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

		protected override DataContractBase GetHealthcareContext()
		{
			return new HealthcareContext(this);
		}

        #region Presentation Model


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
			SetUrl(WebResourcesSettings.Default.PatientReconciliationPageUrl);
            NotifyAllPropertiesChanged();
        }

        private void AddField(string heading, PropertyDiff propertyDiff)
        {
            _fields.Add(new Field(heading, propertyDiff.IsDiscrepant, propertyDiff.AlignedLeftValue, propertyDiff.AlignedRightValue, propertyDiff.DiffMask));
        }

    }

}
