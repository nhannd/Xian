#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.IO;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.DicomServices;
using ClearCanvas.DicomServices.Xml;

namespace ClearCanvas.Dicom.DataStore
{
	public class Study : PersistentDicomObject, IStudy, IEquatable<Study>
    {
		#region Private Fields

		private Guid _studyOid;
    	private string _studyInstanceUid;
    	private string _patientId;
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
		private string _modalitiesInStudy;
		private int _numberOfStudyRelatedInstances;
		private int _numberOfStudyRelatedSeries;
		private string _specificCharacterSet;
    	private string _procedureCodeSequenceCodeValue;
    	private string _procedureCodeSequenceCodingSchemeDesignator;
        private DateTime? _storeTime;
		private DicomUri _studyXmlUri;

		private List<ISeries> _series;
		private StudyXml _studyXml;

		#endregion //Private Fields

		protected internal Study()
        {
		}

		#region Public Properties

		#region NHibernate Persistent Properties

		protected virtual Guid StudyOid
        {
            get { return _studyOid; }
			set { _studyOid = value; }
		}

		[QueryableProperty(DicomTags.StudyInstanceUid, ReturnAlways = true)]
    	public virtual string StudyInstanceUid
    	{
    		get { return _studyInstanceUid; }
			set { SetClassMember(ref _studyInstanceUid, value); }
		}

		[QueryableProperty(DicomTags.PatientId, ReturnAlways = true)]
		public virtual string PatientId
    	{
    		get { return _patientId; }
			set { SetClassMember(ref _patientId, value); }
    	}

		[QueryableProperty(DicomTags.PatientsName, ReturnAlways = true, ReturnProperty = "PatientsNameRaw")]
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

		[QueryableProperty(DicomTags.PatientsSex)]
		public virtual string PatientsSex
    	{
    		get { return _patientsSex; }
			set { SetClassMember(ref _patientsSex, value); }
    	}

		[QueryableProperty(DicomTags.PatientsBirthDate, ReturnOnly = true)]
		public virtual string PatientsBirthDateRaw
    	{
    		get { return _patientsBirthDateRaw; }
			set { SetClassMember(ref _patientsBirthDateRaw, value); }
    	}

		[QueryableProperty(DicomTags.StudyId, ReturnAlways = true)]
		public virtual string StudyId
        {
            get { return _studyId; }
			set { SetClassMember(ref _studyId, value); }
        }

		[QueryableProperty(DicomTags.AccessionNumber, ReturnAlways = true)]
		public virtual string AccessionNumber
    	{
    		get { return _accessionNumber; }
			set { SetClassMember(ref _accessionNumber, value); }
    	}

		[QueryableProperty(DicomTags.StudyDescription)]
		public virtual string StudyDescription
    	{
    		get { return _studyDescription; }
			set { SetClassMember(ref _studyDescription, value); }
    	}

		[QueryableProperty(DicomTags.StudyDate, ReturnAlways = true, ReturnProperty = "StudyDateRaw")]
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

		[QueryableProperty(DicomTags.StudyTime, ReturnOnly = true)]
		public virtual string StudyTimeRaw
        {
            get { return _studyTimeRaw; }
			set { SetClassMember(ref _studyTimeRaw, value); }
		}

		[QueryableProperty(DicomTags.ModalitiesInStudy, IsComputed = true)]
		public virtual string ModalitiesInStudy
		{
			get { return _modalitiesInStudy; }
			set { SetClassMember(ref _modalitiesInStudy, value); }
		}

		[QueryableProperty(DicomTags.NumberOfStudyRelatedSeries, ReturnOnly = true)]
		public virtual int NumberOfStudyRelatedSeries
		{
			get { return _numberOfStudyRelatedSeries; }
			set { SetValueTypeMember(ref _numberOfStudyRelatedSeries, value); }
		}

		[QueryableProperty(DicomTags.NumberOfStudyRelatedInstances, ReturnOnly = true)]
		public virtual int NumberOfStudyRelatedInstances
		{
			get { return _numberOfStudyRelatedInstances; }
			set { SetValueTypeMember(ref _numberOfStudyRelatedInstances, value);}
		}

		[QueryableProperty(DicomTags.ProcedureCodeSequence, DicomTags.CodeValue)]
		public virtual string ProcedureCodeSequenceCodeValue
		{
			get { return _procedureCodeSequenceCodeValue; }
			set { SetClassMember(ref _procedureCodeSequenceCodeValue, value); }
		}

		[QueryableProperty(DicomTags.ProcedureCodeSequence, DicomTags.CodingSchemeDesignator)]
		public virtual string ProcedureCodeSequenceCodingSchemeDesignator
		{
			get { return _procedureCodeSequenceCodingSchemeDesignator; }
			set { SetClassMember(ref _procedureCodeSequenceCodingSchemeDesignator, value); }
		}

		[QueryableProperty(DicomTags.SpecificCharacterSet, ReturnAlways = true)]
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

		public virtual DicomUri StudyXmlUri
	    {
			get { return _studyXmlUri; }
			set { SetClassMember(ref _studyXmlUri, value); }
		}

		#endregion

		#region Non-Hibernate Properties

		public string[] SopClassesInStudy
		{
			get
			{
				List<string> sopClasses = new List<string>();
				foreach (Series series in Series)
				{
					foreach (SopInstance instance in series.GetSopInstances())
					{
						if (!sopClasses.Contains(instance.SopClassUid))
							sopClasses.Add(instance.SopClassUid);
					}
				}
				
				return sopClasses.ToArray();
			}
		}

		#endregion

		#endregion

		#region Private Properties

		private StudyXml StudyXml
		{
			get
			{
				LoadStudyXml();
			    return _studyXml;
			}	
		}

		private List<ISeries> Series
        {
            get
            {
				if (_series == null)
				{
					_series = new List<ISeries>();
					foreach (SeriesXml seriesXml in StudyXml)
						_series.Add(new Series(this, seriesXml));
				}

				return _series;
            }
		}

		#endregion

		#region IEquatable<Study> Members

		public bool Equals(Study other)
    	{
			if (other == null)
				return false;

			return (StudyInstanceUid == other.StudyInstanceUid);
    	}

    	#endregion

    	public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

			if (obj is Study)
				return Equals((Study) obj);

			return false;
        }
		
        public override int GetHashCode()
        {
            int accumulator = 0;
            foreach (char character in StudyInstanceUid)
            {
                if ('.' != character)
                    accumulator += Convert.ToInt32(character);
                else
                    accumulator -= 23;
            }
            return accumulator;
        }

		#region IStudy Members

		string IStudy.StudyDate
		{
			get { return _studyDateRaw; }
		}

		string IStudy.StudyTime
		{
			get { return _studyTimeRaw; }
		}

		public DateTime? GetStoreTime()
		{
			return _storeTime;
		}

		public IEnumerable<ISeries> GetSeries()
		{
			foreach (ISeries series in Series)
				yield return series;
		}

		public IEnumerable<ISopInstance> GetSopInstances()
        {
            foreach (ISeries series in Series)
            {
                foreach (ISopInstance sopInstance in series.GetSopInstances())
                {
                	yield return sopInstance;
                }
            }
        }

		#endregion

		private void LoadStudyXml()
		{
			if (_studyXml == null)
			{
				if (StudyXmlUri == null)
					throw new InvalidOperationException("The study xml location must be set.");

				XmlDocument doc = new XmlDocument();
				_studyXml = new StudyXml(StudyInstanceUid);

				if (File.Exists(StudyXmlUri.LocalDiskPath))
				{
					using (FileStream stream = new FileStream(StudyXmlUri.LocalDiskPath, FileMode.Open, FileAccess.Read))
					{
						StudyXmlIo.Read(doc, stream);
						_studyXml.SetMemento(doc);
					}
				}
			}
		}

		#region Helper Methods

		private int ComputeNumberOfSopInstances()
		{
			int count = 0;
			foreach (Series series in Series)
			{
				foreach (SopInstance sop in series.GetSopInstances())
				{
					++count;
				}
			}

			return count;
		}

		private string[] ComputeModalitiesInStudy()
		{
			List<string> modalities = new List<string>();
			foreach (Series series in Series)
			{
				if (!modalities.Contains(series.Modality))
					modalities.Add(series.Modality);
			}

			return modalities.ToArray();
		}

		internal void Initialize(DicomFile file)
		{
			DicomAttributeCollection sopInstanceDataset = file.DataSet;
			
			DicomAttribute attribute = sopInstanceDataset[DicomTags.StudyInstanceUid];
			string datasetStudyUid = attribute.ToString();
			if (!String.IsNullOrEmpty(StudyInstanceUid) && StudyInstanceUid != datasetStudyUid)
			{
				string message = String.Format("The study uid in the data set does not match this study's uid ({0} != {1}).", 
													datasetStudyUid, StudyInstanceUid);

				throw new InvalidOperationException(message);
			}
			else
			{
				StudyInstanceUid = attribute.ToString();
			}

			Platform.CheckForEmptyString(StudyInstanceUid, "StudyInstanceUid");

			attribute = sopInstanceDataset[DicomTags.PatientId];
			PatientId = attribute.ToString();

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

			if (sopInstanceDataset.Contains(DicomTags.ProcedureCodeSequence))
			{
				attribute = sopInstanceDataset[DicomTags.ProcedureCodeSequence];
				if (!attribute.IsEmpty && !attribute.IsNull)
				{
					DicomSequenceItem sequence = ((DicomSequenceItem[]) attribute.Values)[0];
					ProcedureCodeSequenceCodeValue = sequence[DicomTags.CodeValue].ToString();
					ProcedureCodeSequenceCodingSchemeDesignator = sequence[DicomTags.CodingSchemeDesignator].ToString();
				}
			}

			attribute = sopInstanceDataset[DicomTags.SpecificCharacterSet];
			SpecificCharacterSet = attribute.ToString();
		}

		internal void Update(DicomFile file)
		{
			Initialize(file);

			//NOTE: important!  These need the study xml to exist first, so they must be set here.
			//The Initialize method is used by the validator to validate the lengths of all the column values.
			//Currently, this will cause the ModalitiesInStudy value to be excluded from validation, but
			//it will likely never be an issue since the column length is 256 characters.
			ModalitiesInStudy = DicomStringHelper.GetDicomStringArray(ComputeModalitiesInStudy());
			NumberOfStudyRelatedInstances = ComputeNumberOfSopInstances();
			NumberOfStudyRelatedSeries = Series.Count;

			StudyXml.AddFile(file);
		}

		internal void Flush()
		{
			StudyXmlOutputSettings settings = new StudyXmlOutputSettings();
			settings.IncludePrivateValues = false;
			settings.IncludeUnknownTags = false;
			settings.IncludeSourceFileName = true;

			//Ensure the existing stuff is loaded.
			LoadStudyXml();

			using (FileStream stream = new FileStream(StudyXmlUri.LocalDiskPath, FileMode.Create, FileAccess.Write))
			{
				StudyXmlIo.Write(StudyXml.GetMemento(settings), stream);
				stream.Close();
			}
		}

		#endregion
	}
}
