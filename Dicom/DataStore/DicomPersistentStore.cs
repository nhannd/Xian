#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using ClearCanvas.Common;

namespace ClearCanvas.Dicom.DataStore
{
	public sealed partial class DataAccessLayer
	{
		private sealed class DicomPersistentStore : IDicomPersistentStore
		{
			private readonly Dictionary<string, Study> _existingStudyCache = new Dictionary<string, Study>();
			private readonly Dictionary<string, Study> _studiesToUpdateInXml = new Dictionary<string, Study>();
			private readonly Dictionary<string, Study> _studiesToUpdateInDatastore = new Dictionary<string, Study>();

			private IDataStoreReader _dataStoreReader;
			private IDicomPersistentStoreValidator _validator;

			public DicomPersistentStore()
			{
			}

			private void ClearCache()
			{
				_existingStudyCache.Clear();
				_studiesToUpdateInXml.Clear();
				_studiesToUpdateInDatastore.Clear();
			}

			private IDataStoreReader GetIDataStoreReader()
			{
				if (_dataStoreReader == null)
					_dataStoreReader = DataAccessLayer.GetIDataStoreReader();

				return _dataStoreReader;
			}

			private IDicomPersistentStoreValidator GetValidator()
			{
				if (_validator == null)
					_validator = GetIDicomPersistentStoreValidator();

				return _validator;
			}

			private Study GetStudy(string studyInstanceUid)
			{
				if (_existingStudyCache.ContainsKey(studyInstanceUid))
					return _existingStudyCache[studyInstanceUid];

				//everything gets added for xml update
				if (_studiesToUpdateInXml.ContainsKey(studyInstanceUid))
					return _studiesToUpdateInXml[studyInstanceUid];

				Study existingStudy = (Study)GetIDataStoreReader().GetStudy(studyInstanceUid);
				if (existingStudy != null)
					_existingStudyCache[existingStudy.StudyInstanceUid] = existingStudy;

				return existingStudy;
			}

			#region IDicomPersistentStore Members

			public void UpdateSopInstance(DicomFile file)
			{
				GetValidator().Validate(file);

				string studyInstanceUid = file.DataSet[DicomTags.StudyInstanceUid];
				Study study = GetStudy(studyInstanceUid);

				bool newStudy = false;
				bool studyDirty = false;

				if (study == null)
				{
					study = new Study();
					study.StoreTime = DateTime.Now;

					string studyStoragePath = GetStudyStorageLocator().GetStudyStorageDirectory(studyInstanceUid);
					if (String.IsNullOrEmpty(studyStoragePath))
						throw new DataStoreException("The study storage path is empty.");

					studyStoragePath = Path.GetFullPath(studyStoragePath);
					if (!Directory.Exists(studyStoragePath))
						throw new DirectoryNotFoundException(String.Format("The specified directory does not exist ({0}).", studyStoragePath));

					UriBuilder uriBuilder = new UriBuilder();
					uriBuilder.Scheme = "file";
					uriBuilder.Path = Path.Combine(studyStoragePath, "studyXml.xml");

					study.StudyXmlUri = new DicomUri(uriBuilder.Uri);
					newStudy = true;
				}

				EventHandler studyDirtyDelegate = delegate { studyDirty = true; };
				study.Changed += studyDirtyDelegate;

				try
				{
					study.Update(file);
				}
				finally
				{
					study.Changed -= studyDirtyDelegate;
				}

				if (studyDirty || newStudy)
					_studiesToUpdateInDatastore[study.StudyInstanceUid] = study;

				_studiesToUpdateInXml[study.StudyInstanceUid] = study;
			}

			public void Commit()
			{
				try
				{
					using (IDataStoreWriter writer = GetIDataStoreWriter())
					{
						foreach (Study study in _studiesToUpdateInXml.Values)
							study.Flush();

						writer.StoreStudies(_studiesToUpdateInDatastore.Values);
					}
				}
				catch (Exception e)
				{
					throw new DataStoreException("Failed to commit files to data store.", e);
				}
				finally
				{
					ClearCache();
				}
			}

			#endregion

			private void DisposeReader()
			{
				try
				{
					if (_dataStoreReader != null)
						_dataStoreReader.Dispose();
				}
				finally
				{
					_dataStoreReader = null;
				}
			}

			#region IDisposable Members

			public void Dispose()
			{
				try
				{

					DisposeReader();
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e);
				}

				GC.SuppressFinalize(this);
			}

			#endregion
		}
	}
}