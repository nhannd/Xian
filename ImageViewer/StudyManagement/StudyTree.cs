using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Imaging;

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
		private StudyCollection _studies = new StudyCollection();
		private SeriesCollection _seriesCollection = new SeriesCollection();
		private SopCollection _sops = new SopCollection();

		private static SopCache _sopCache = new SopCache();

		internal StudyTree()
		{
		}

#if UNIT_TESTS

		internal SopCache SopCache
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
				if (_seriesCollection == null)
					_seriesCollection = new SeriesCollection();

				return _seriesCollection;
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

			ImageValidator.ValidateImage(image);

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
				_sopCache.Add(image as ICacheableSop);

				// Get the image from the cache
				ImageSop cachedSop = _sopCache[image.SopInstanceUID] as ImageSop;

				// Create a proxy for actual image in the cache.  This allows
				// for the sharing of images so we never have to keep copies
				// of the image.  Clients use the proxy instead of the real image.
				ImageSopProxy imageSopProxy = new ImageSopProxy(cachedSop);
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
				Platform.Log(e);
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
					_sopCache.Remove(sop.SopInstanceUID);

				_sops.Clear();
			}
		}

		#endregion
	}
}
