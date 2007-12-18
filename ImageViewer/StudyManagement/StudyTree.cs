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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A tree representation of the DICOM patient, study, series, SOP hierarchy.
	/// </summary>
	public sealed class StudyTree
	{
		private PatientCollection _patients;

		// We add these master dictionaries so we can have rapid
		// look up of study, series and sop objects without having to traverse
		// the tree.
		private StudyCollection _studies;
		private SeriesCollection _series;
		private SopCollection _sops;

		private static readonly Dictionary<string,ReferenceCountedObjectWrapper<Sop>> _sopCache = new Dictionary<string, ReferenceCountedObjectWrapper<Sop>>();

		internal StudyTree()
		{
		}

#if UNIT_TESTS

		internal Dictionary<string, ReferenceCountedObjectWrapper<Sop>> SopCache
		{
			get { return _sopCache; }
		}

#endif

		/// <summary>
		/// Gets the collection of <see cref="Patient"/> objects that belong
		/// to this <see cref="StudyTree"/>.
		/// </summary>
		public PatientCollection Patients
		{
			get 
			{ 
				if (_patients == null)
					_patients = new PatientCollection();

				return _patients; 
			}
		}

		private StudyCollection Studies
		{
			get
			{
				if (_studies == null)
					_studies = new StudyCollection();

				return _studies;
			}
		}

		private SeriesCollection SeriesCollection
		{
			get
			{
				if (_series == null)
					_series = new SeriesCollection();

				return _series;
			}
		}

		private SopCollection Sops
		{
			get
			{
				if (_sops == null)
					_sops = new SopCollection();

				return _sops;
			}
		}

		/// <summary>
		/// Gets a <see cref="Patient"/> with the specified patient ID.
		/// </summary>
		/// <param name="patientId"></param>
		/// <returns>The <see cref="Patient"/> or <b>null</b> if the patient ID
		/// cannot be found.</returns>
		public Patient GetPatient(string patientId)
		{
			Platform.CheckForEmptyString(patientId, "patientId");

			if (!_patients.ContainsKey(patientId))
				return null;

			return this.Patients[patientId];
		}

		/// <summary>
		/// Gets a <see cref="Study"/> with the specified Study Instance UID.
		/// </summary>
		/// <param name="studyInstanceUID"></param>
		/// <returns>The <see cref="Study"/> or <b>null</b> if the Study Instance UID
		/// cannot be found.</returns>
		public Study GetStudy(string studyInstanceUID)
		{
			Platform.CheckForEmptyString(studyInstanceUID, "studyInstanceUID");

			if (!this.Studies.ContainsKey(studyInstanceUID))
				return null;

			return this.Studies[studyInstanceUID];
		}

		/// <summary>
		/// Gets a <see cref="Series"/> with the specified Series Instance UID.
		/// </summary>
		/// <param name="seriesInstanceUID"></param>
		/// <returns>The <see cref="Series"/> or <b>null</b> if the Series Instance UID
		/// cannot be found.</returns>
		public Series GetSeries(string seriesInstanceUID)
		{
			Platform.CheckForEmptyString(seriesInstanceUID, "seriesInstanceUID");

			if (!this.SeriesCollection.ContainsKey(seriesInstanceUID))
				return null;

			return this.SeriesCollection[seriesInstanceUID];
		}

		/// <summary>
		/// Gets a <see cref="Sop"/> with the specified SOP Instance UID.
		/// </summary>
		/// <param name="sopInstanceUID"></param>
		/// <returns>The <see cref="Sop"/> or <b>null</b> if the SOP Instance UID
		/// cannot be found.</returns>
		public Sop GetSop(string sopInstanceUID)
		{
			Platform.CheckForEmptyString(sopInstanceUID, "sopInstanceUID");

			if (!this.Sops.ContainsKey(sopInstanceUID))
				return null;

			return this.Sops[sopInstanceUID];
		}

		internal void AddImage(ImageSop image)
		{
			Platform.CheckForNullReference(image, "image");

			image.Validate();

			Patient patient = AddPatient(image.PatientId);
			Study study = AddStudy(image.StudyInstanceUID, patient);
			Series series = AddSeries(image.SeriesInstanceUID, study);

			AddImage(image, series);
		}

		#region Private methods

		private Patient AddPatient(string patientID)
		{
			Patient patient;
			if (!this.Patients.ContainsKey(patientID))
			{
				patient = new Patient();
				this.Patients.Add(patientID, patient);
			}
			else
			{
				patient = _patients[patientID];
			}
			return patient;
		}

		private Study AddStudy(string studyInstanceUID, Patient patient)
		{
			Study study;
			StudyCollection studies = patient.Studies;

			if (!studies.ContainsKey(studyInstanceUID))
			{
				// If this ever happens, it means that the study
				// belongs to more than one patient, which means
				// something has gone terribly wrong on the modality
				if (this.Studies.ContainsKey(studyInstanceUID))
					throw new Exception("Study belongs to more than one patient.");

				study = new Study(patient);
				studies.Add(studyInstanceUID, study);
				this.Studies.Add(studyInstanceUID, study);
			}
			else
			{
				study = studies[studyInstanceUID];
			}
			return study;
		}

		private Series AddSeries(string seriesInstanceUID, Study study)
		{
			Series series;
			SeriesCollection seriesCollection = study.Series;

			if (!seriesCollection.ContainsKey(seriesInstanceUID))
			{
				// If this ever happens, it means that the series
				// belongs to more than one study, which means
				// something has gone terribly wrong on the modality
				if (this.SeriesCollection.ContainsKey(seriesInstanceUID))
					throw new Exception("Series belongs to more than one study.");

				series = new Series(study);
				seriesCollection.Add(seriesInstanceUID, series);
				this.SeriesCollection.Add(seriesInstanceUID, series);
			}
			else
			{
				series = seriesCollection[seriesInstanceUID];
			}

			return series;
		}

		private void AddImage(ImageSop image, Series series)
		{
			SopCollection sops = series.Sops;

			if (!sops.ContainsKey(image.SopInstanceUID))
			{
				// If this ever happens, it means that the SOP
				// belongs to more than one series, which means
				// something has gone terribly wrong on the modality
				if (this.Sops.ContainsKey(image.SopInstanceUID))
					throw new Exception("SOP belongs to more than one series.");

				// Try and add the image to the cache.  If it already exists
				// it won't be added
				if (!_sopCache.ContainsKey(image.SopInstanceUID))
					_sopCache[image.SopInstanceUID] = new ReferenceCountedObjectWrapper<Sop>(image);
				
				_sopCache[image.SopInstanceUID].IncrementReferenceCount();
				
				// Create a proxy for actual image in the cache.  This allows
				// for the sharing of images so we never have to keep copies
				// of the image.  Clients use the proxy instead of the real image.
				ImageSopProxy imageSopProxy = new ImageSopProxy((ImageSop)_sopCache[image.SopInstanceUID].Item);
				imageSopProxy.ParentSeries = series;

				// Propagate the image proxy up the tree so the parent nodes
				// can access the tags in the image
				series.SetSop(imageSopProxy);

				// Add the proxy the SOP collection
				sops.Add(imageSopProxy.SopInstanceUID, imageSopProxy);
				this.Sops.Add(imageSopProxy.SopInstanceUID, imageSopProxy);
			}
		}

		#endregion

		#region Disposal

		#region IDisposable Members

		/// <summary>
		/// Releases all resources used by this <see cref="StudyTree"/>.
		/// </summary>
		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				// shouldn't throw anything from inside Dispose()
				Platform.Log(LogLevel.Error, e);
			}
		}

		#endregion

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern
		/// </summary>
		/// <param name="disposing">True if this object is being disposed, false if it is being finalized</param>
		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				foreach (Sop sop in _sops.Values)
				{
					sop.ParentSeries.SetSop(null);

					if (_sopCache.ContainsKey(sop.SopInstanceUID))
					{
						_sopCache[sop.SopInstanceUID].DecrementReferenceCount();
						if (!_sopCache[sop.SopInstanceUID].IsReferenceCountAboveZero())
							_sopCache.Remove(sop.SopInstanceUID);
					}
				}

				if (_patients != null)
				{
					_patients.Clear();
					_patients = null;
				}

				if (_studies != null)
				{
					_studies.Clear();
					_studies = null;
				}

				if (_series != null)
				{
					_series.Clear();
					_series = null;
				}

				if (_sops != null)
				{
					_sops.Clear();
					_sops = null;
				}
			}
		}

		#endregion
	}
}
