using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Services.LocalDataStore;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	internal class FileImportInformation
	{
		private string _sourceFile;
		private BadFileBehaviour _badFileBehaviour;

		private string _storedFile;
		private Exception _error;

		private string _studyInstanceUid;
		private string _seriesInstanceUid;
		private string _sopInstanceUid;

		private string _patientId;
		private string _studyDescription;
		private string _studyId;

		public FileImportInformation(string sourceFile)
			: this(sourceFile, BadFileBehaviour.Ignore)
		{
		}

		public FileImportInformation(string sourceFile, BadFileBehaviour badFileBehaviour)
		{
			_sourceFile = sourceFile;
			_badFileBehaviour = badFileBehaviour;
		}
		
		private FileImportInformation()
		{ 
		}

		public string SourceFile
		{
			get { return _sourceFile; }
		}

		public BadFileBehaviour BadFileBehaviour
		{
			get { return _badFileBehaviour; }
		}

		public string StoredFile
		{
			get { return _storedFile; }
			set { _storedFile = value; }
		}

		public Exception Error
		{
			get { return _error; }
			set { _error = value; }
		}

		public bool Failed
		{
			get { return _error != null; }
		}

		public string PatientId
		{
			get { return _patientId; }
			set { _patientId = value; }
		}

		public string StudyInstanceUid
		{
			get { return _studyInstanceUid; }
			set { _studyInstanceUid = value; }
		}

		public string SeriesInstanceUid
		{
			get { return _seriesInstanceUid; }
			set { _seriesInstanceUid = value; }
		}

		public string SopInstanceUid
		{
			get { return _sopInstanceUid; }
			set { _sopInstanceUid = value; }
		}

		public string StudyDescription
		{
			get { return _studyDescription; }
			set { _studyDescription = value; }
		}

		public string StudyId
		{
			get { return _studyId; }
			set { _studyId = value; }
		}
	}
	
	internal class DicomFileImporter : DicomImageStoreBase
	{
		private object _syncLock = new object();
		
		public void InsertSopInstance(FileImportInformation fileImportInformation)
		{
			DcmFileFormat file = new DcmFileFormat();

			try
			{
				OFCondition condition = file.loadFile(fileImportInformation.SourceFile);
				if (!condition.good())
				{
					file.Dispose();
					file = null;

					ProcessBadFile(fileImportInformation);

					fileImportInformation.Error = new Exception(String.Format("The file format is not recognized ({0}).", fileImportInformation.SourceFile));
					return;
				}

				DcmMetaInfo metaInfo = file.getMetaInfo();
				DcmDataset dataset = file.getDataset();

				if (!ConfirmProcessableFile(metaInfo, dataset))
				{
					file.Dispose();
					file = null;

					ProcessBadFile(fileImportInformation);

					fileImportInformation.Error	= 
						new Exception(String.Format("Although the file format is recognized, it is not appropriate for insertion into the datastore ({0}).", fileImportInformation.SourceFile));
					return;
				}

				StringBuilder studyInstanceUid = new StringBuilder(128);
				StringBuilder seriesInstanceUid = new StringBuilder(128);
				StringBuilder sopInstanceUid = new StringBuilder(128);

				dataset.findAndGetOFString(Dcm.StudyInstanceUID, studyInstanceUid);
				dataset.findAndGetOFString(Dcm.SeriesInstanceUID, seriesInstanceUid);
				dataset.findAndGetOFString(Dcm.SOPInstanceUID, sopInstanceUid);

				string storedFile = String.Format("{0}{1}\\{2}\\{3}.dcm", LocalDataStoreService.Instance.StorageFolder,
																			studyInstanceUid.ToString(),
																			seriesInstanceUid.ToString(),
																			sopInstanceUid.ToString());
				//!! lock the uri!!
				//UriBuilder storedFileUri = new UriBuilder();
				//storedFileUri.Scheme = "file";
				//storedFileUri.Path = storedFile;

				if (System.IO.File.Exists(storedFile))
					System.IO.File.Delete(storedFile);

				string directoryName = System.IO.Path.GetDirectoryName(storedFile);
				if (!System.IO.Directory.Exists(directoryName))
					System.IO.Directory.CreateDirectory(directoryName);

				//System.IO.File.Move(fileImportInformation.SourceFile, storedFile);
				System.IO.File.Copy(fileImportInformation.SourceFile, storedFile);

				lock (_syncLock)
				{
					ISopInstance newSop = GetSopInstance(metaInfo, dataset, fileImportInformation.StoredFile);
					IStudy study = GetStudy(metaInfo, dataset);
					ISeries series = GetSeries(metaInfo, dataset);

					series.AddSopInstance(newSop);
					study.AddSeries(series);
				}

				fileImportInformation.StoredFile = storedFile;
				fileImportInformation.StudyInstanceUid = studyInstanceUid.ToString();
				fileImportInformation.SeriesInstanceUid = seriesInstanceUid.ToString();
				fileImportInformation.SopInstanceUid = sopInstanceUid.ToString();

				StringBuilder tagRetrievalBuilder = new StringBuilder(1024);

				dataset.findAndGetOFString(Dcm.StudyID, tagRetrievalBuilder);
				fileImportInformation.StudyId = tagRetrievalBuilder.ToString();

				dataset.findAndGetOFString(Dcm.PatientId, tagRetrievalBuilder);
				fileImportInformation.PatientId = tagRetrievalBuilder.ToString();

				dataset.findAndGetOFString(Dcm.StudyDescription, tagRetrievalBuilder);
				fileImportInformation.StudyDescription = tagRetrievalBuilder.ToString();

				// keep the file object alive until the end of this scope block
				// otherwise, it'll be GC'd and metaInfo and dataset will be gone
				// as well, even though they are needed in the InsertSopInstance
				// and sub methods
				//GC.KeepAlive(file);
			}
			catch (Exception e)
			{
				fileImportInformation.Error = e;
			}
			finally
			{
				file.Dispose();
				file = null;
			}
		}

		private void ProcessBadFile(FileImportInformation fileImportInformation)
		{
			if (fileImportInformation.BadFileBehaviour == BadFileBehaviour.Move)
			{
				string storedFile = String.Format("{0}{1}", LocalDataStoreService.Instance.BadFileFolder, System.IO.Path.GetRandomFileName());
				System.IO.File.Move(fileImportInformation.SourceFile, storedFile);
				fileImportInformation.StoredFile = storedFile;
			}
			else if (fileImportInformation.BadFileBehaviour == BadFileBehaviour.Delete)
			{
				System.IO.File.Delete(fileImportInformation.SourceFile);
			}
		}

		private bool ConfirmProcessableFile(DcmMetaInfo metaInfo, DcmDataset dataset)
		{
			StringBuilder stringValue = new StringBuilder(1024);
			OFCondition cond;
			cond = metaInfo.findAndGetOFString(Dcm.MediaStorageSOPClassUID, stringValue);
			if (cond.good())
			{
				// we want to skip Media Storage Directory Storage (DICOMDIR directories)
				if ("1.2.840.10008.1.3.10" == stringValue.ToString())
					return false;
			}

			return true;
		}

		public void Flush()
		{
			lock (_syncLock)
			{
				foreach (KeyValuePair<string, Study> pair in this.StudyCache)
				{
					DataAccessLayer.GetIDataStoreWriter().StoreStudy(pair.Value);
				}

				this.SeriesCache.Clear();
				this.StudyCache.Clear();
			}
		}

		public int GetCachedStudiesCount()
		{
			lock (_syncLock)
			{
				return this.StudyCache.Count;
			}
		}

		private IStudy GetStudy(DcmMetaInfo metaInfo, DcmDataset sopInstanceDataset)
		{
			lock (_syncLock)
			{
				return base.GetStudy(metaInfo, sopInstanceDataset, typeof(DataAccessLayer));
			}
		}
	}
}
