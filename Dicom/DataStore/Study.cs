#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibernate.Collection;
using Iesi.Collections;
using System.Collections.ObjectModel;
using ClearCanvas.Common;

namespace ClearCanvas.Dicom.DataStore
{
	public class Study : PersistentDicomObject, IStudy, IEquatable<Study>
    {
        public delegate void InitializeAssociatedCollectionCallback(object domainObject, PersistentCollection associatedCollection);
        public InitializeAssociatedCollectionCallback InitializeAssociatedCollection;

		#region Private Fields

		private Guid _studyOid;
    	private string _studyInstanceUid;
    	private PatientId _patientId;
    	private PersonName _patientsName;
    	private string _patientsNameRaw;
    	private string _patientsSex;
    	private string _patientsBirthDateRaw;
    	private string _studyId;
    	private string _accessionNumber;
    	private string _studyDescription;
    	private DateTime? _studyDate;
		private string _studyDateRaw;
    	private string _studyTimeRaw;
    	private string _specificCharacterSet;
    	private string _procedureCodeSequenceCodeValue;
    	private string _procedureCodeSequenceCodingSchemeDesignator;
    	private DateTime? _storeTime;
    	private readonly ISet _internalSeries;

		#endregion //Private Fields
		
        public Study()
        {
        	_internalSeries = new HybridSet();
        }

		#region NHibernate Persistent Properties

		protected virtual Guid StudyOid
        {
            get { return _studyOid; }
			set { _studyOid = value; }
		}

    	public virtual string StudyInstanceUid
    	{
    		get { return _studyInstanceUid; }
			set { SetClassMember(ref _studyInstanceUid, value); }
		}

    	public virtual PatientId PatientId
    	{
    		get { return _patientId; }
			set { SetClassMember(ref _patientId, value); }
    	}

    	public virtual PersonName PatientsName
    	{
    		get { return _patientsName; }
			set { SetClassMember(ref _patientsName, value); }
    	}

    	public virtual string PatientsNameRaw
    	{
    		get { return _patientsNameRaw; }
			set { SetClassMember(ref _patientsNameRaw, value); }
    	}

    	public virtual string PatientsSex
    	{
    		get { return _patientsSex; }
			set { SetClassMember(ref _patientsSex, value); }
    	}

    	public virtual string PatientsBirthDateRaw
    	{
    		get { return _patientsBirthDateRaw; }
			set { SetClassMember(ref _patientsBirthDateRaw, value); }
    	}

    	public virtual string StudyId
        {
            get { return _studyId; }
			set { SetClassMember(ref _studyId, value); }
        }

    	public virtual string AccessionNumber
    	{
    		get { return _accessionNumber; }
			set { SetClassMember(ref _accessionNumber, value); }
    	}

    	public virtual string StudyDescription
    	{
    		get { return _studyDescription; }
			set { SetClassMember(ref _studyDescription, value); }
    	}

    	public virtual DateTime? StudyDate
    	{
    		get { return _studyDate; }
			set { SetNullableTypeMember(ref _studyDate, value); }
    	}

    	public virtual string StudyDateRaw
    	{
    		get { return _studyDateRaw; }
			set { SetClassMember(ref _studyDateRaw, value); }
    	}

    	public virtual string StudyTimeRaw
        {
            get { return _studyTimeRaw; }
			set { SetClassMember(ref _studyTimeRaw, value); }
		}

		public virtual string ProcedureCodeSequenceCodeValue
		{
			get { return _procedureCodeSequenceCodeValue; }
			set { SetClassMember(ref _procedureCodeSequenceCodeValue, value); }
		}

		public virtual string ProcedureCodeSequenceCodingSchemeDesignator
		{
			get { return _procedureCodeSequenceCodingSchemeDesignator; }
			set { SetClassMember(ref _procedureCodeSequenceCodingSchemeDesignator, value); }
		}

		public virtual string SpecificCharacterSet
    	{
    		get { return _specificCharacterSet; }
			set { SetClassMember(ref _specificCharacterSet, value); }
    	}

		public virtual DateTime? StoreTime
        {
            get { return _storeTime; }
			set { SetNullableTypeMember(ref _storeTime, value); }
        }

        protected virtual ISet InternalSeries
        {
            get { return _internalSeries; }
		}

		#endregion

		public virtual ISet Series
        {
            get
            {
                if (null != InitializeAssociatedCollection)
                    InitializeAssociatedCollection(this, _internalSeries as PersistentCollection);

                return _internalSeries;
            }
        }

    	#region IEquatable<Study> Members

    	public bool Equals(Study other)
    	{
			return (this.StudyInstanceUid == other.StudyInstanceUid);
    	}

    	#endregion

    	public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

			if (obj is Study)
				return this.Equals((Study) obj);

			return false;
        }
		
        public override int GetHashCode()
        {
            int accumulator = 0;
            foreach (char character in this.StudyInstanceUid)
            {
                if ('.' != character)
                    accumulator += Convert.ToInt32(character);
                else
                    accumulator -= 23;
            }
            return accumulator;
        }

		#region IStudy Members

        public IEnumerable<ISopInstance> GetSopInstances()
        {
            foreach (ISeries series in this.Series)
            {
                foreach (ISopInstance sopInstance in series.GetSopInstances())
                {
                	yield return sopInstance;
                }
            }
        }

        public Uid GetStudyInstanceUid()
        {
            return new Uid(this.StudyInstanceUid);
        }

        public IEnumerable<ISeries> GetSeries()
        {
            foreach (ISeries series in this.Series)
            {
                yield return series;
            }
        }

		public int GetNumberOfSeries()
		{
			return this.Series.Count;
		}

		public int GetNumberOfSopInstances()
		{
			int returnValue = 0;

			foreach (ISeries series in this.Series)
				returnValue += series.GetNumberOfSopInstances();

			return returnValue;
		}
		
		#endregion

		#region Helper Methods

		public void Update(DicomAttributeCollection metaInfo, DicomAttributeCollection sopInstanceDataset)
		{
			DicomAttribute attribute = sopInstanceDataset[DicomTags.StudyInstanceUid];
			if (!String.IsNullOrEmpty(StudyInstanceUid) && StudyInstanceUid != attribute.ToString())
			{
				throw new InvalidOperationException(SR.ExceptionCanOnlyUpdateExistingStudyWithSameStudyUid);
			}
			else
			{
				this.StudyInstanceUid = attribute.ToString();
			}

			Platform.CheckForEmptyString(StudyInstanceUid, "StudyInstanceUid");

			attribute = sopInstanceDataset[DicomTags.PatientId];
			PatientId = new PatientId(attribute.ToString());

			attribute = sopInstanceDataset[DicomTags.PatientsName];
			PatientsName = new PersonName(attribute.ToString());
			PatientsNameRaw = DicomImplementation.CharacterParser.EncodeAsIsomorphicString(PatientsName, sopInstanceDataset.SpecificCharacterSet);

			attribute = sopInstanceDataset[DicomTags.PatientsSex];
			PatientsSex = attribute.ToString();

			attribute = sopInstanceDataset[DicomTags.PatientsBirthDate];
			PatientsBirthDateRaw = attribute.ToString();

			attribute = sopInstanceDataset[DicomTags.StudyId];
			StudyId = attribute.ToString();

			attribute = sopInstanceDataset[DicomTags.AccessionNumber];
			AccessionNumber = attribute.ToString();

			attribute = sopInstanceDataset[DicomTags.StudyDescription];
			StudyDescription = attribute.ToString();

			attribute = sopInstanceDataset[DicomTags.StudyDate];
			StudyDateRaw = attribute.ToString();
			StudyDate = DateParser.Parse(StudyDateRaw);

			attribute = sopInstanceDataset[DicomTags.StudyTime];
			StudyTimeRaw = attribute.ToString();

			//
			// TODO: get ProcedureCodeSequence.CodeValue and ProcedureCodeSequence.SchemeDesignator
			//

			attribute = sopInstanceDataset[DicomTags.SpecificCharacterSet];
			SpecificCharacterSet = attribute.ToString();
		}

    	#endregion
	}
}
