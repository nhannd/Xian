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
using ClearCanvas.ImageViewer;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A tree representation of the DICOM patient, study, series, SOP hierarchy.
	/// </summary>
	public sealed class StudyTree
	{
		// We add these master dictionaries so we can have rapid
		// look up of study, series and sop objects without having to traverse
		// the tree.
		private PatientCollection _patients;
		private Dictionary<string, Study> _studies;
		private Dictionary<string, Series> _series;
		private Dictionary<string, Sop> _sops;

		internal StudyTree()
		{
			_patients = new PatientCollection();
			_studies = new Dictionary<string, Study>();
			_series = new Dictionary<string, Series>();
			_sops = new Dictionary<string, Sop>();
		}

		/// <summary>
		/// Gets the collection of <see cref="Patient"/> objects that belong
		/// to this <see cref="StudyTree"/>.
		/// </summary>
		public PatientCollection Patients
		{
			get { return _patients; }
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

			if (!_studies.ContainsKey(studyInstanceUID))
				return null;

			return _studies[studyInstanceUID];
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

			if (!_series.ContainsKey(seriesInstanceUID))
				return null;

			return _series[seriesInstanceUID];
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

			if (!_sops.ContainsKey(sopInstanceUID))
				return null;

			return _sops[sopInstanceUID];
		}

		#region Private methods

		/// <summary>
		/// Adds an <see cref="ImageSop"/> to the <see cref="StudyTree"/>.
		/// </summary>
		internal void AddImage(ImageSop image)
		{
			Platform.CheckForNullReference(image, "image");

			image.Validate();

			image.IncrementReferenceCount();

			if (_sops.ContainsKey(image.SopInstanceUID))
			{
				image.DecrementReferenceCount();
				return;
			}

			ImageSop cachedSop = SopCache.Add(image);
			if (image != cachedSop) //there was already one in the cache.
			{
				image.DecrementReferenceCount();
				cachedSop.IncrementReferenceCount();
			}

			image = new ImageSopProxy(cachedSop);
			AddPatient(image);
			AddStudy(image);
			AddSeries(image);
			_sops[image.SopInstanceUID] = image;
		}

		/// <summary>
		/// Adds the contents of a <see cref="KeyObjectSelectionSop"/> to the <see cref="StudyTree"/>.
		/// </summary>
		/// <remarks>
		/// The visualization mode of a key object selection document in the <see cref="ImageViewerComponent"/>
		/// is to realize each individual content item of the selection as separate SOPs and add each of these
		/// to the study tree under a series prototyped by the key object selection document.
		/// </remarks>
		/// <param name="keyObjectSelection"></param>
		internal void AddKeyObjectSelection(KeyObjectSelectionSop keyObjectSelection)
		{
			Platform.CheckForNullReference(keyObjectSelection, "keyObjectSelection");

			foreach (KeyObjectSop keyObjectSop in keyObjectSelection.CreateImages())
			{
				AddImage(keyObjectSop);
			}
		}

		private void AddPatient(ImageSop sop)
		{
			if (_patients[sop.PatientId] != null)
				return;

			Patient patient = new Patient();
			patient.SetSop(sop);

			_patients.Add(patient);
		}

		private void AddStudy(ImageSop sop)
		{
			if (_studies.ContainsKey(sop.StudyInstanceUID))
				return;

			Patient patient = _patients[sop.PatientId];
			Study study = new Study(patient);
			study.SetSop(sop);
			patient.Studies.Add(study);

			_studies[study.StudyInstanceUID] = study;
		}

		private void AddSeries(ImageSop sop)
		{
			Series series;
			if (_series.ContainsKey(sop.SeriesInstanceUID))
			{
				series = _series[sop.SeriesInstanceUID];
			}
			else
			{
				Study study = _studies[sop.StudyInstanceUID];
				series = new Series(study);
				series.SetSop(sop);
				study.Series.Add(series);

				_series[series.SeriesInstanceUID] = series;
			}

			sop.ParentSeries = series;
			series.Sops.Add(sop);
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
				if (_sops != null)
				{
					foreach (Sop sop in _sops.Values)
						sop.DecrementReferenceCount();

					_sops.Clear();
					_sops = null;
				}

				if (_series != null)
				{
					_series.Clear();
					_series = null;
				}

				if (_studies != null)
				{
					_studies.Clear();
					_studies = null;
				}

				if (_patients != null)
				{
					_patients.Clear();
					_patients = null;
				}
			}
		}

		#endregion
	}
}
