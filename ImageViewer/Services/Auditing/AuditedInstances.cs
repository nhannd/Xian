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
using System.IO;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Audit;

namespace ClearCanvas.ImageViewer.Services.Auditing
{
	/// <summary>
	/// Represents a collection of DICOM instances and, optionally, the associated file paths that are the subject of an audit event.
	/// </summary>
	public sealed class AuditedInstances
	{
		private readonly Dictionary<Patient, Dictionary<Study, List<string>>> _studies = new Dictionary<Patient, Dictionary<Study, List<string>>>();

		/// <summary>
		/// Constructs a new, empty collection.
		/// </summary>
		public AuditedInstances() {}

		/// <summary>
		/// Constructs a new collection and adds files on the indicated paths, automatically parsing for patient and study instance information.
		/// </summary>
		/// <param name="recursive">True if the paths should be processed recursively; False otherwise.</param>
		/// <param name="paths">The file paths on which to search for files.</param>
		public AuditedInstances(bool recursive, params string[] paths)
		{
			if (paths != null)
			{
				foreach (string path in paths)
					this.AddPath(path, recursive);
			}
		}

		/// <summary>
		/// Recursively searches the given path for DICOM object files, automatically parsing for patient and study instance information.
		/// </summary>
		/// <param name="path">The file path on which to search for files.</param>
		public void AddPath(string path)
		{
			this.AddPath(path, true);
		}

		/// <summary>
		/// Searches the given path for DICOM object files, automatically parsing for patient and study instance information.
		/// </summary>
		/// <param name="path">The file path on which to search for files.</param>
		/// <param name="recursive">True if the paths should be processed recursively; False otherwise.</param>
		public void AddPath(string path, bool recursive)
		{
			if (File.Exists(path))
			{
				try
				{
					DicomFile dcf = new DicomFile(path);
					dcf.Load(DicomReadOptions.Default | DicomReadOptions.DoNotStorePixelDataInDataSet);

					List<string> s = InternalAddStudy(dcf.DataSet[DicomTags.PatientId].ToString(), dcf.DataSet[DicomTags.PatientsName].ToString(), dcf.DataSet[DicomTags.StudyInstanceUid].ToString());
					s.Add(path);
				}
				catch (DicomException) {}
			}
			else if (recursive && Directory.Exists(path))
			{
				foreach (string subpaths in Directory.GetFileSystemEntries(path))
					this.AddPath(subpaths);
			}
		}

		/// <summary>
		/// Adds a given instance to the collection without any patient information.
		/// </summary>
		/// <remarks>
		/// This overload should be used only when patient information is not available, or is not required by
		/// the type of auditable event. In most cases, the <see cref="AddInstance(string,string,string)"/>
		/// overload is preferred over this one. Furthermore, if the type of auditable event requires some knowledge
		/// of a source or destination media, then the <see cref="AddInstance(string,string,string,string)"/>
		/// overload should be used instead.
		/// </remarks>
		/// <param name="studyInstanceUid">The study instance UID of the instance.</param>
		public void AddInstance(string studyInstanceUid)
		{
			InternalAddStudy(string.Empty, string.Empty, studyInstanceUid);
		}

		/// <summary>
		/// Adds a given instance to the collection.
		/// </summary>
		/// <remarks>
		/// If the type of auditable event requires some knowledge
		/// of a source or destination media, then the <see cref="AddInstance(string,string,string,string)"/>
		/// overload should be used instead.
		/// </remarks>
		/// <param name="patientId">The patient ID of the instance.</param>
		/// <param name="patientName">The patient's name of the instance.</param>
		/// <param name="studyInstanceUid">The study instance UID of the instance.</param>
		public void AddInstance(string patientId, string patientName, string studyInstanceUid)
		{
			InternalAddStudy(patientId, patientName, studyInstanceUid);
		}

		/// <summary>
		/// Adds a given instance to the collection.
		/// </summary>
		/// <remarks>
		/// This overload is used when the type of audtable event requires knowledge of a source or
		/// destination media, such as for Data Import and Data Export events. Otherwise, the
		/// <see cref="AddInstance(string,string,string)"/> overload is a perfectly acceptable
		/// substitute, especially when such source path information is not available.
		/// </remarks>
		/// <param name="patientId">The patient ID of the instance.</param>
		/// <param name="patientName">The patient's name of the instance.</param>
		/// <param name="studyInstanceUid">The study instance UID of the instance.</param>
		/// <param name="filename">The filename or path associated with the specified instance.</param>
		public void AddInstance(string patientId, string patientName, string studyInstanceUid, string filename)
		{
			List<string> s = InternalAddStudy(patientId, patientName, studyInstanceUid);
			s.Add(filename);
		}

		private List<string> InternalAddStudy(string patientId, string patientName, string studyInstanceUid)
		{
			Patient p = new Patient(patientId, patientName);
			if (!_studies.ContainsKey(p))
			{
				_studies.Add(p, new Dictionary<Study, List<string>>());
			}

			Dictionary<Study, List<string>> studies = _studies[p];
			Study s = new Study(studyInstanceUid);
			if (!studies.ContainsKey(s))
				studies.Add(s, new List<string>());
			return studies[s];
		}

		/// <summary>
		/// Enumerates the unique patients in the collection.
		/// </summary>
		internal IEnumerable<AuditPatientParticipantObject> EnumeratePatients()
		{
			foreach (Patient patient in _studies.Keys)
				yield return patient;
		}

		/// <summary>
		/// Enumerates the unique studies in the collection.
		/// </summary>
		internal IEnumerable<AuditStudyParticipantObject> EnumerateStudies()
		{
			foreach (Patient patient in _studies.Keys)
				foreach (Study study in _studies[patient].Keys)
					yield return study;
		}

		/// <summary>
		/// Enumerates the unique studies for a particular <paramref name="patient"/> in the collection.
		/// </summary>
		internal IEnumerable<AuditStudyParticipantObject> EnumerateStudies(AuditPatientParticipantObject patient)
		{
			return EnumerateStudies(patient.PatientId, patient.PatientsName);
		}

		/// <summary>
		/// Enumerates the unique studies for a particular patient in the collection.
		/// </summary>
		/// <param name="patientId">The patient ID of the patient.</param>
		/// <param name="patientName">The patient's name of the patient.</param>
		internal IEnumerable<AuditStudyParticipantObject> EnumerateStudies(string patientId, string patientName)
		{
			Patient patient = new Patient(patientId, patientName);
			if (_studies.ContainsKey(patient))
				foreach (Study study in _studies[patient].Keys)
					yield return study;
		}

		/// <summary>
		/// Enumerates all the file paths in the collection.
		/// </summary>
		internal IEnumerable<string> EnumerateFiles()
		{
			foreach (Patient patient in _studies.Keys)
				foreach (Study study in _studies[patient].Keys)
					foreach (string file in _studies[patient][study])
						yield return file;
		}

		/// <summary>
		/// Enumerates the unique file system volumnes in the collection.
		/// </summary>
		/// <remarks>
		/// This enumeration returns the volume labels of each device; If the label is empty,
		/// then the name of the device is returned instead.
		/// </remarks>
		internal IEnumerable<string> EnumerateFileVolumes()
		{
			Dictionary<string, string> drives = new Dictionary<string, string>();
			foreach (string s in this.EnumerateFiles())
			{
				string root = Path.GetPathRoot(s).ToUpperInvariant();
				if (root.Length > 0 && root[0] >= 'A' && root[0] <= 'Z')
				{
					DriveInfo drive = new DriveInfo(root[0].ToString());
					if (!drives.ContainsKey(drive.Name))
						drives.Add(drive.Name, string.IsNullOrEmpty(drive.VolumeLabel) ? drive.Name : drive.VolumeLabel);
				}
			}
			return drives.Values;
		}

		private class Study
		{
			public readonly string InstanceUid;

			public Study(string instanceUid)
			{
				this.InstanceUid = instanceUid;
			}

			public override bool Equals(object obj)
			{
				if (obj is Study)
				{
					Study s = (Study) obj;
					return this.InstanceUid == s.InstanceUid;
				}
				return base.Equals(obj);
			}

			public override int GetHashCode()
			{
				return this.InstanceUid.GetHashCode() ^ 0x471A0695;
			}

			public static implicit operator AuditStudyParticipantObject(Study study)
			{
				AuditStudyParticipantObject auditObject = new AuditStudyParticipantObject(study.InstanceUid);
				return auditObject;
			}
		}

		private class Patient
		{
			public readonly string PatientId;
			public readonly string PatientName;

			public Patient(string patientId, string patientName)
			{
				this.PatientId = patientId;
				this.PatientName = patientName;
			}

			public override bool Equals(object obj)
			{
				if (obj is Patient)
				{
					Patient p = (Patient) obj;
					return this.PatientId == p.PatientId && this.PatientName == p.PatientName;
				}
				return base.Equals(obj);
			}

			public override int GetHashCode()
			{
				return this.PatientId.GetHashCode() ^ this.PatientName.GetHashCode() ^ 0x517F0000;
			}

			public static implicit operator AuditPatientParticipantObject(Patient patient)
			{
				AuditPatientParticipantObject auditObject = new AuditPatientParticipantObject(patient.PatientName, patient.PatientId);
				return auditObject;
			}
		}
	}
}