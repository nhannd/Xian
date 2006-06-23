using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public class StudyTree
	{
		private PatientCollection _patients = new PatientCollection();

		// We add these master dictionaries so we can have rapid
		// look up of study, series and sop objects without having to traverse
		// the tree.
		private StudyCollection _studies = new StudyCollection();
		private SeriesCollection _seriesCollection = new SeriesCollection();
		private SopCollection _sops = new SopCollection();

		public StudyTree()
		{
		}

		public PatientCollection Patients
		{
			get { return _patients; }
		}

		public Patient GetPatient(string patientId)
		{
			Platform.CheckForEmptyString(patientId, "patientId");

			if (!_patients.ContainsKey(patientId))
				return null;

			return this.Patients[patientId];
		}

		public Study GetStudy(string studyInstanceUID)
		{
			Platform.CheckForEmptyString(studyInstanceUID, "studyInstanceUID");

			if (!_studies.ContainsKey(studyInstanceUID))
				return null;

			return _studies[studyInstanceUID];
		}

		public Series GetSeries(string seriesInstanceUID)
		{
			Platform.CheckForEmptyString(seriesInstanceUID, "seriesInstanceUID");

			if (!_seriesCollection.ContainsKey(seriesInstanceUID))
				return null;

			return _seriesCollection[seriesInstanceUID];
		}

		public Sop GetSop(string sopInstanceUID)
		{
			Platform.CheckForEmptyString(sopInstanceUID, "sopInstanceUID");

			if (!_sops.ContainsKey(sopInstanceUID))
				return null;

			return _sops[sopInstanceUID];
		}

		public void AddImage(ImageSop image)
		{
			Platform.CheckForNullReference(image, "image");

			ImageValidator.ValidateImage(image);

			Patient patient;
			Study study;
			Series series;

			if (!_patients.ContainsKey(image.PatientId))
			{
				patient = new Patient(image.PatientId);
				_patients.Add(image.PatientId, patient);
			}
			else
			{
				patient = _patients[image.PatientId];
			}

			StudyCollection studies = patient.Studies;

			if (!studies.ContainsKey(image.StudyInstanceUID))
			{
				study = new Study(image.StudyInstanceUID, patient);
				studies.Add(image.StudyInstanceUID, study);
				_studies.Add(image.StudyInstanceUID, study);
			}
			else
			{
				study = studies[image.StudyInstanceUID];
			}

			SeriesCollection seriesCollection = study.Series;

			if (!seriesCollection.ContainsKey(image.SeriesInstanceUID))
			{
				series = new Series(image.SeriesInstanceUID, study);
				seriesCollection.Add(image.SeriesInstanceUID, series);
				_seriesCollection.Add(image.SeriesInstanceUID, series);
			}
			else
			{
				series = seriesCollection[image.SeriesInstanceUID];
			}

			SopCollection sops = series.Sops;

			if (!sops.ContainsKey(image.SopInstanceUID))
			{
				image.ParentSeries = series;
				sops.Add(image.SopInstanceUID, image);
				_sops.Add(image.SopInstanceUID, image);
			}
		}

		internal void IncrementStudyReferenceCount(string studyInstanceUID)
		{
			Platform.CheckForEmptyString(studyInstanceUID, "studyInstanceUID");

			Study study = GetStudy(studyInstanceUID);

			if (study == null)
				return;

			study.ReferenceCount++;
		}

		internal void DecrementStudyReferenceCount(string studyInstanceUID)
		{
			Platform.CheckForEmptyString(studyInstanceUID, "studyInstanceUID");

			Study study = GetStudy(studyInstanceUID);

			if (study == null)
				return;
			
			study.ReferenceCount--;

			if (study.ReferenceCount == 0)
				RemoveStudy(study);

			// A lot of memory can be freed here since image pixel data
			// is held by the ImageSop objects.
			GC.Collect();
		}

		private void RemoveStudy(Study study)
		{
			RemoveStudyFromMasterDictionaries(study);
			RemoveStudyFromTree(study);
		}

		private void RemoveStudyFromMasterDictionaries(Study study)
		{
			_studies.Remove(study.StudyInstanceUID);

			foreach (Series series in study.Series.Values)
			{
				_seriesCollection.Remove(series.SeriesInstanceUID);

				foreach (Sop sop in series.Sops.Values)
					_sops.Remove(sop.SopInstanceUID);
			}
		}

		private void RemoveStudyFromTree(Study study)
		{
			Patient parentPatient = study.ParentPatient;
			parentPatient.Studies.Remove(study.StudyInstanceUID);

			// If the parent patient has no studies left, remove it too.
			if (parentPatient.Studies.Count == 0)
				this.Patients.Remove(parentPatient.PatientId);
		}
	}
}
