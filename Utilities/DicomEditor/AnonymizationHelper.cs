using System;
using System.Collections.Generic;
using System.IO;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Common;

namespace ClearCanvas.Utilities.DicomEditor
{
	public interface IAnonymizationCallback
	{
		void ReportProgress(int percent, string message);
		
		void BeforeAnonymize(DicomAttributeCollection dataSet);
		void BeforeSave(DicomAttributeCollection dataSet);
	}

	public sealed class AnonymizationException : Exception
	{
		internal AnonymizationException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		internal AnonymizationException(string message)
			: base(message)
		{
		}
	}

	public static class AnonymizationHelper
	{
		private static bool _initialized = false;
		private static bool _canAnonymizeStudy = false;

		private static IStudyLoader GetStudyLoader()
		{
			try
			{
				StudyLoaderExtensionPoint xp = new StudyLoaderExtensionPoint();
				foreach (IStudyLoader loader in xp.CreateExtensions())
				{
					if (loader.Name == "DICOM_LOCAL")
					{
						_canAnonymizeStudy = true;
						return loader;
					}
				}

				throw new AnonymizationException("There is no local study loader installed.");
			}
			catch (NotSupportedException e)
			{
				throw new AnonymizationException("There is no local study loader installed.", e);
			}
		}

		internal static bool CanAnonymizeStudy()
		{
			if (!_initialized)
			{
				_initialized = true;
				_canAnonymizeStudy = GetStudyLoader() != null;
			}

			return _canAnonymizeStudy;
		}

		public static void AnonymizeLocalStudy(
				string studyInstanceUid,
				string newPatientId,
				string newPatientsName,
				DateTime? newPatientsBirthDate,
				string newAccessionNumber,
				string newStudyDescription,
				DateTime? newStudyDate,
				bool preserveSex,
				bool preserveSeriesDescriptions,
				IAnonymizationCallback callback)
		{
			Platform.CheckForEmptyString(studyInstanceUid, "studyInstanceUid");
			Platform.CheckForEmptyString(newPatientId, "newPatientId");
			Platform.CheckForEmptyString(newPatientsName, "newPatientsName");

			IStudyLoader studyLoader = GetStudyLoader();

			try
			{
				Dictionary<string, string> seriesUidMap = new Dictionary<string, string>();
				Dictionary<string, string> seriesDescriptionMap = new Dictionary<string, string>();

				string tempFilePath = String.Format(".\\temp\\{0}", Path.GetRandomFileName());
				Directory.CreateDirectory(tempFilePath);
				tempFilePath = Path.GetFullPath(tempFilePath);

				string newStudyUid = DicomUid.GenerateUid().UID;

				if (callback != null)
					callback.ReportProgress(0, SR.MessageAnonymizingStudy);

				int numberOfSops = studyLoader.Start(studyInstanceUid);
				for (int i = 0; i < numberOfSops; ++i)
				{
					Sop sop = studyLoader.LoadNextImage();
					DicomFile file = (DicomFile) sop.NativeDicomObject;
					file.Load();

					if (callback != null)
						callback.BeforeAnonymize(file.DataSet);

					DicomAttribute seriesUidAttribute = file.DataSet[DicomTags.SeriesInstanceUid];
					string oldSeriesUid = seriesUidAttribute.GetString(0, "");
					if (!seriesUidMap.ContainsKey(oldSeriesUid))
					{
						seriesUidMap[oldSeriesUid] = DicomUid.GenerateUid().UID;
						seriesDescriptionMap[oldSeriesUid] = file.DataSet[DicomTags.SeriesDescription].GetString(0, "");
					}

					DateTime? oldAcquisitionDateTime = null;
					if (file.DataSet.Contains(DicomTags.AcquisitionDatetime))
						oldAcquisitionDateTime = DateTimeParser.Parse(file.DataSet[DicomTags.AcquisitionDatetime].ToString());

					string oldSex = null;
					if (file.DataSet.Contains(DicomTags.PatientsSex))
						oldSex = file.DataSet[DicomTags.PatientsSex].GetString(0, "");

					AnonymizationHelper.RemoveAllPrivateTags(file.DataSet);
					AnonymizationHelper.NullType2Tags(file.DataSet);

					file.DataSet[DicomTags.StudyInstanceUid].SetStringValue(newStudyUid);
					file.DataSet[DicomTags.SeriesInstanceUid].SetStringValue(seriesUidMap[oldSeriesUid]);
					file.DataSet[DicomTags.SopInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);

					file.DataSet[DicomTags.PatientsName].SetStringValue(newPatientsName);
					file.DataSet[DicomTags.PatientId].SetStringValue(newPatientId);

					if (newPatientsBirthDate != null)
						file.DataSet[DicomTags.PatientsBirthDate].SetStringValue(
							newPatientsBirthDate.Value.ToString(DateParser.DicomDateFormat));

					if (!String.IsNullOrEmpty(newAccessionNumber))
						file.DataSet[DicomTags.AccessionNumber].SetStringValue(newAccessionNumber);

					if (!String.IsNullOrEmpty(newStudyDescription))
						file.DataSet[DicomTags.StudyDescription].SetStringValue(newStudyDescription);

					if (preserveSeriesDescriptions)
						file.DataSet[DicomTags.SeriesDescription].SetStringValue(seriesDescriptionMap[oldSeriesUid]);

					if (preserveSex && oldSex != null)
						file.DataSet[DicomTags.PatientsSex].SetStringValue(oldSex);

					if (file.DataSet.Contains(DicomTags.StudyDate))
						file.DataSet[DicomTags.StudyDate].SetStringValue("");

					if (file.DataSet.Contains(DicomTags.SeriesDate))
						file.DataSet[DicomTags.SeriesDate].SetStringValue("");

					if (file.DataSet.Contains(DicomTags.AcquisitionDate))
						file.DataSet[DicomTags.AcquisitionDate].SetStringValue("");

					if (file.DataSet.Contains(DicomTags.AcquisitionDatetime))
						file.DataSet[DicomTags.AcquisitionDatetime].SetStringValue("");

					if (newStudyDate != null)
					{
						//set all date tags to be the same.
						DateTime date = newStudyDate.Value;
						file.DataSet[DicomTags.StudyDate].SetStringValue(date.ToString(DateParser.DicomDateFormat));
						file.DataSet[DicomTags.SeriesDate].SetStringValue(date.ToString(DateParser.DicomDateFormat));
						file.DataSet[DicomTags.AcquisitionDate].SetStringValue(date.ToString(DateParser.DicomDateFormat));
						if (oldAcquisitionDateTime != null)
						{
							TimeSpan diff = date - oldAcquisitionDateTime.Value;
							oldAcquisitionDateTime = oldAcquisitionDateTime.Value.AddDays(diff.TotalDays);
							file.DataSet[DicomTags.AcquisitionDatetime].SetStringValue(
								oldAcquisitionDateTime.Value.ToString(DateTimeParser.ToDicomString(oldAcquisitionDateTime.Value, false)));
						}
					}

					if (callback != null)
						callback.BeforeSave(file.DataSet);

					file.Save(String.Format("{0}\\{1}.dcm", tempFilePath, i));

					if (callback != null)
					{
						callback.ReportProgress((int)Math.Floor((i + 1) / (float)numberOfSops * 100),
					                 String.Format(SR.MessageAnonymizingStudy, tempFilePath));
					}
				}

				//trigger an import of the anonymized files.
				LocalDataStoreServiceClient client = new LocalDataStoreServiceClient();
				client.Open();
				try
				{
					FileImportRequest request = new FileImportRequest();
					request.BadFileBehaviour = BadFileBehaviour.Move;
					request.FileImportBehaviour = FileImportBehaviour.Move;
					List<string> filePaths = new List<string>();
					filePaths.Add(tempFilePath);
					request.FilePaths = filePaths;
					request.Recursive = true;
					client.Import(request);
					client.Close();
				}
				catch
				{
					client.Abort();
					throw;
				}
			}
			catch(Exception e)
			{
				throw new AnonymizationException("An error occurred while trying to anonymize the local study.", e);
			}
		}

		public static void NullType2Tags(DicomAttributeCollection dataset)
		{
			if (dataset.Contains(DicomTags.AccessionNumber))
				dataset[DicomTags.AccessionNumber].SetStringValue("");
			if (dataset.Contains(DicomTags.InstitutionName))
				dataset[DicomTags.InstitutionName].SetStringValue("");
			if (dataset.Contains(DicomTags.InstitutionAddress))
				dataset[DicomTags.InstitutionAddress].SetStringValue("");
			if (dataset.Contains(DicomTags.ReferringPhysiciansName))
				dataset[DicomTags.ReferringPhysiciansName].SetStringValue("");
			if (dataset.Contains(DicomTags.StationName))
				dataset[DicomTags.StationName].SetStringValue("");
			if (dataset.Contains(DicomTags.StudyDescription))
				dataset[DicomTags.StudyDescription].SetStringValue("");
			if (dataset.Contains(DicomTags.InstitutionalDepartmentName))
				dataset[DicomTags.InstitutionalDepartmentName].SetStringValue("");
			if (dataset.Contains(DicomTags.PhysiciansOfRecord))
				dataset[DicomTags.PhysiciansOfRecord].SetStringValue("");
			if (dataset.Contains(DicomTags.PerformingPhysiciansName))
				dataset[DicomTags.PerformingPhysiciansName].SetStringValue("");
			if (dataset.Contains(DicomTags.NameOfPhysiciansReadingStudy))
				dataset[DicomTags.NameOfPhysiciansReadingStudy].SetStringValue("");
			if (dataset.Contains(DicomTags.OperatorsName))
				dataset[DicomTags.OperatorsName].SetStringValue("");
			if (dataset.Contains(DicomTags.AdmittingDiagnosesDescription))
				dataset[DicomTags.AdmittingDiagnosesDescription].SetStringValue("");
			if (dataset.Contains(DicomTags.DerivationDescription))
				dataset[DicomTags.DerivationDescription].SetStringValue("");
			if (dataset.Contains(DicomTags.SeriesDescription))
				dataset[DicomTags.SeriesDescription].SetStringValue("");
			if (dataset.Contains(DicomTags.PatientsName))
				dataset[DicomTags.PatientsName].SetStringValue("");
			if (dataset.Contains(DicomTags.PatientId))
				dataset[DicomTags.PatientId].SetStringValue("");
			if (dataset.Contains(DicomTags.PatientsBirthDate))
				dataset[DicomTags.PatientsBirthDate].SetStringValue("");
			if (dataset.Contains(DicomTags.PatientsBirthTime))
				dataset[DicomTags.PatientsBirthTime].SetStringValue("");
			if (dataset.Contains(DicomTags.PatientsSex))
				dataset[DicomTags.PatientsSex].SetStringValue("");
			if (dataset.Contains(DicomTags.OtherPatientIds))
				dataset[DicomTags.OtherPatientIds].SetStringValue("");
			if (dataset.Contains(DicomTags.OtherPatientNames))
				dataset[DicomTags.OtherPatientNames].SetStringValue("");
			if (dataset.Contains(DicomTags.PatientsAge))
				dataset[DicomTags.PatientsAge].SetStringValue("");
			if (dataset.Contains(DicomTags.PatientsSize))
				dataset[DicomTags.PatientsSize].SetStringValue("");
			if (dataset.Contains(DicomTags.PatientsWeight))
				dataset[DicomTags.PatientsWeight].SetStringValue("");
			if (dataset.Contains(DicomTags.EthnicGroup))
				dataset[DicomTags.EthnicGroup].SetStringValue("");
			if (dataset.Contains(DicomTags.Occupation))
				dataset[DicomTags.Occupation].SetStringValue("");
			if (dataset.Contains(DicomTags.PatientComments))
				dataset[DicomTags.PatientComments].SetStringValue("");
			if (dataset.Contains(DicomTags.AdditionalPatientHistory))
				dataset[DicomTags.AdditionalPatientHistory].SetStringValue("");
			if (dataset.Contains(DicomTags.DeviceSerialNumber))
				dataset[DicomTags.DeviceSerialNumber].SetStringValue("");
			if (dataset.Contains(DicomTags.ProtocolName))
				dataset[DicomTags.ProtocolName].SetStringValue("");
			if (dataset.Contains(DicomTags.StudyId))
				dataset[DicomTags.StudyId].SetStringValue("");
			if (dataset.Contains(DicomTags.ImageComments))
				dataset[DicomTags.ImageComments].SetStringValue("");
		}

		public static void RemoveAllPrivateTags(DicomAttributeCollection dataset)
		{
			List<DicomTag> privateTags = new List<DicomTag>();
			foreach (DicomAttribute attribute in dataset)
			{
				if (attribute.Tag.Name == "Private Tag")
					privateTags.Add(attribute.Tag);
			}

			foreach (DicomTag tag in privateTags)
				dataset[tag] = null;
		}

		public static bool IsMetainfoTag(uint attribute)
		{
			return attribute <= 267546;
		}
	}
}
