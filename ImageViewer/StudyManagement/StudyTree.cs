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
using ClearCanvas.Common;
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
		private StudyCollection _studies;
		private SeriesCollection _series;
		private SopCollection _sops;

		internal StudyTree()
		{
			_patients = new PatientCollection();
			_studies = new StudyCollection();
			_series = new SeriesCollection();
			_sops = new SopCollection();
		}

		/// <summary>
		/// Gets the collection of <see cref="Patient"/> objects that belong
		/// to this <see cref="StudyTree"/>.
		/// </summary>
		public PatientCollection Patients
		{
			get { return _patients; }
		}

		private StudyCollection Studies
		{
			get { return _studies; }
		}

		private SeriesCollection Series
		{
			get { return _series; }
		}

		private SopCollection Sops
		{
			get { return _sops; }
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

			if (!this.Series.ContainsKey(seriesInstanceUID))
				return null;

			return this.Series[seriesInstanceUID];
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

		#region Private methods

		/// <summary>
		/// Adds an <see cref="ImageSop"/> to the <see cref="StudyTree"/>.
		/// </summary>
		internal void AddImage(ImageSop image)
		{
			Platform.CheckForNullReference(image, "image");

			image.Validate();

			image.IncrementReferenceCount();

			if (this.Sops.ContainsKey(image.SopInstanceUID))
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
			this.Sops[image.SopInstanceUID] = image;
		}

		private void AddPatient(ImageSop sop)
		{
			if (_patients.ContainsKey(sop.PatientId))
				return;

			Patient patient = new Patient();
			patient.SetSop(sop);

			_patients[sop.PatientId] = patient;
		}

		private void AddStudy(ImageSop sop)
		{
			if (_studies.ContainsKey(sop.StudyInstanceUID))
				return;

			Patient patient = _patients[sop.PatientId];
			Study study = new Study(patient);
			study.SetSop(sop);
			patient.Studies[study.StudyInstanceUID] = study;

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
				study.Series[series.SeriesInstanceUID] = series;

				_series[series.SeriesInstanceUID] = series;
			}

			sop.ParentSeries = series;
			series.Sops[sop.SopInstanceUID] = sop;
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
