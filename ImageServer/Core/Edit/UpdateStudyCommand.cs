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
using System.Diagnostics;
using System.IO;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Diagnostics;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Rules;

namespace ClearCanvas.ImageServer.Core.Edit
{
	/// <summary>
	/// <see cref="ServerDatabaseCommand"/> for updating a study.
	/// </summary>
	/// <remarks>
	/// 
	/// </remarks>
	public class UpdateStudyCommand : ServerDatabaseCommand, IDisposable
	{
		#region Private Members
		private readonly List<InstanceInfo> _updatedSopList = new List<InstanceInfo>();
		private readonly StudyStorageLocation _oldStudyLocation;
		private string _oldStudyPath;
		private string _oldStudyInstanceUid;
		private string _newStudyFolder;
		private string _newStudyInstanceUid;
		private string _oldStudyFolder;
		private bool _initialized=false;

		private PatientInfo _oldPatientInfo;
		private PatientInfo _newPatientInfo;

		private readonly IList<BaseImageLevelUpdateCommand> _commands;
		private string _newStudyPath;
		private string _backupDir;
		private readonly ServerPartition _partition;
		private Study _study;
		private Patient _curPatient;
		private Patient _newPatient;
		private StudyStorage _storage;

		private readonly UpdateStudyStatistics _statistics;
		private int _totalSopCount;
		private bool _restored;
		private bool _deleteOriginalFolder;

		private bool _patientInfoIsNotChanged;
		private readonly ServerRulesEngine _rulesEngine;
		#endregion

		#region Constructors
		public UpdateStudyCommand(ServerPartition partition, 
		                          StudyStorageLocation studyLocation,
		                          IList<BaseImageLevelUpdateCommand> imageLevelCommands) 
			: base("Update existing study", true)
		{
			_partition = partition;
			_oldStudyLocation = studyLocation;
			_commands = imageLevelCommands;
			_statistics = new UpdateStudyStatistics(_oldStudyLocation.StudyInstanceUid);
			_rulesEngine = null;
		}

		public UpdateStudyCommand(ServerPartition partition,
								  StudyStorageLocation studyLocation,
								  IList<BaseImageLevelUpdateCommand> imageLevelCommands,
								  ServerRulesEngine rulesEngine)
			: base("Update existing study", true)
		{
			_partition = partition;
			_oldStudyLocation = studyLocation;
			_commands = imageLevelCommands;
			_statistics = new UpdateStudyStatistics(_oldStudyLocation.StudyInstanceUid);
			_rulesEngine = rulesEngine;
		}

		#endregion

		#region Properties
		public new UpdateStudyStatistics Statistics
		{
			get { return _statistics; }
		}

		public string NewStudyPath
		{
			get { return _newStudyPath; }
			set { _newStudyPath = value; }
		}

		#endregion

		#region Protected Method
		protected override void OnExecute(ServerCommandProcessor theProcessor, IUpdateContext updateContext)
		{
			Statistics.ProcessTime.Start();
            
			Initialize();

			PrintUpdateCommands();

			if (RequiresRollback)
				BackupFilesystem();

			UpdateFilesystem();

			UpdateDatabase();

			Statistics.ProcessTime.End();
		}

		private void CleaupBackupFiles()
		{
			DirectoryUtility.DeleteIfExists(_backupDir);
		}

		protected override void OnUndo()
		{
			RestoreFilesystem();

			// db rollback is done by the processor
			CleaupBackupFiles();

			_restored = true;
		}


		#endregion

		#region Private Methods
		private void Initialize()
		{
			_backupDir = ExecutionContext.BackupDirectory;

			_oldStudyPath = _oldStudyLocation.GetStudyPath();
			_oldStudyInstanceUid = _oldStudyLocation.StudyInstanceUid;
			_oldStudyFolder = _oldStudyLocation.StudyFolder;
			_newStudyFolder = _oldStudyFolder;
			_newStudyInstanceUid = _oldStudyInstanceUid;

			_study = _oldStudyLocation.LoadStudy(UpdateContext);
			_totalSopCount = _study.NumberOfStudyRelatedInstances;
			_curPatient = _study.LoadPatient(UpdateContext);
			_oldPatientInfo = new PatientInfo();
			_oldPatientInfo.Name = _curPatient.PatientsName;
			_oldPatientInfo.PatientId = _curPatient.PatientId;
			_oldPatientInfo.IssuerOfPatientId = _curPatient.IssuerOfPatientId;

			_newPatientInfo = new PatientInfo(_oldPatientInfo);
			Debug.Assert(_newPatientInfo.Equals(_oldPatientInfo));

			foreach (BaseImageLevelUpdateCommand command in _commands)
			{
				ImageLevelUpdateEntry imageLevelUpdate = command.UpdateEntry;
				if (imageLevelUpdate != null)
				{
					if (imageLevelUpdate.TagPath.Tag.TagValue == DicomTags.StudyDate)
					{
						// Update the folder name if the system is not currently using receiving date as the study folder
						if (!ImageServerCommonConfiguration.UseReceiveDateAsStudyFolder)
							_newStudyFolder = imageLevelUpdate.GetStringValue();
					}
					else if (imageLevelUpdate.TagPath.Tag.TagValue == DicomTags.StudyInstanceUid)
					{
						_newStudyInstanceUid = imageLevelUpdate.GetStringValue();
					}
					else if (imageLevelUpdate.TagPath.Tag.TagValue == DicomTags.PatientId)
					{
						_newPatientInfo.PatientId = imageLevelUpdate.GetStringValue();
					}
					else if (imageLevelUpdate.TagPath.Tag.TagValue == DicomTags.IssuerOfPatientId)
					{
						_newPatientInfo.IssuerOfPatientId = imageLevelUpdate.GetStringValue();
					}
					else if (imageLevelUpdate.TagPath.Tag.TagValue == DicomTags.PatientsName)
					{
						_newPatientInfo.Name = imageLevelUpdate.GetStringValue();
					}
				}
			}


			Platform.CheckForNullReference(_newStudyInstanceUid, "_newStudyInstanceUid");

			if (String.IsNullOrEmpty(_newStudyFolder))
			{
				_newStudyFolder = ImageServerCommonConfiguration.DefaultStudyRootFolder;
			}

			_newStudyPath = Path.Combine(_oldStudyLocation.FilesystemPath, _partition.PartitionFolder);
			_newStudyPath = Path.Combine(_newStudyPath, _newStudyFolder);
			_newStudyPath = Path.Combine(_newStudyPath, _newStudyInstanceUid);

			_newPatient = FindPatient(_newPatientInfo);
			_patientInfoIsNotChanged = _newPatientInfo.Equals(_oldPatientInfo);

			Statistics.InstanceCount = _study.NumberOfStudyRelatedInstances;
			Statistics.StudySize = (ulong) _oldStudyLocation.LoadStudyXml().GetStudySize();

			// The study path will be changed. We will need to delete the original folder at the end.
			// May be too simple to test if two paths are the same. But let's assume it is good enough for 99% of the time.
			_deleteOriginalFolder = NewStudyPath != _oldStudyPath; 
			_initialized = true;
		}

		private Patient FindPatient(PatientInfo patientInfo)
		{
			IPatientEntityBroker patientFindBroker = UpdateContext.GetBroker<IPatientEntityBroker>();
			PatientSelectCriteria criteria = new PatientSelectCriteria();
			criteria.PatientId.EqualTo(patientInfo.PatientId);
			criteria.PatientsName.EqualTo(patientInfo.Name);

			return patientFindBroker.FindOne(criteria);
		}

		private void PrintUpdateCommands()
		{
			StringBuilder log = new StringBuilder();
			log.AppendLine(String.Format("Study to be updated:"));
			log.AppendLine(String.Format("\tServer Partition: {0}", _partition.AeTitle));
			log.AppendLine(String.Format("\tStorage GUID: {0}", _oldStudyLocation.GetKey().Key));
			log.AppendLine(String.Format("\tPatient ID: {0}", _study.PatientId));
			log.AppendLine(String.Format("\tPatient Name: {0}", _study.PatientsName));
			log.AppendLine(String.Format("\tAccession #: {0}", _study.AccessionNumber));
			log.AppendLine(String.Format("\tStudy ID : {0}", _study.StudyId));
			log.AppendLine(String.Format("\tStudy Date : {0}", _study.StudyDate));
			log.AppendLine(String.Format("\tStudy Instance Uid: {0}", _study.StudyInstanceUid));
			log.AppendLine(String.Format("\tInstance Count: {0}", _study.NumberOfStudyRelatedInstances));
			log.AppendLine(String.Format("\tCurrent location: {0}", _oldStudyPath));
			log.AppendLine();
			log.AppendLine("Changes to be applied:");
			foreach (BaseImageLevelUpdateCommand cmd in _commands)
			{
				log.AppendLine(String.Format("\t{0}", cmd));
			}
            
			log.AppendLine(String.Format("\tNew location: {0}", NewStudyPath));
			Platform.Log(LogLevel.Info, log);
		}

		private void RestoreFilesystem()
		{
			if (!RequiresRollback || !_initialized)
				return;

			if (_backupDir != null)
			{
				if (NewStudyPath == _oldStudyPath)
				{
					// Study folder was not changed. Files were overwritten.

					// restore header
					Platform.Log(LogLevel.Info, "Restoring old study header...");

					FileUtils.Copy(Path.Combine(_backupDir, _study.StudyInstanceUid + ".xml"), _oldStudyLocation.GetStudyXmlPath(), true);
					FileUtils.Copy(Path.Combine(_backupDir, _study.StudyInstanceUid + ".xml.gz"), _oldStudyLocation.GetCompressedStudyXmlPath(), true);

					// restore updated SOPs
					Platform.Log(LogLevel.Info, "Restoring old study folder... {0} sop need to be restored", _updatedSopList.Count);
					int restoredCount = 0;
					foreach (InstanceInfo sop in _updatedSopList)
					{
						string backupSopPath = Path.Combine(_backupDir, sop.SopInstanceUid + ".dcm");

						FileUtils.Copy(backupSopPath,_oldStudyLocation.GetSopInstancePath(sop.SeriesInstanceUid, sop.SopInstanceUid), true);

						restoredCount++;
						Platform.Log(ServerPlatform.InstanceLogLevel, "Restored SOP {0} [{1} of {2}]", sop.SopInstanceUid, restoredCount, _updatedSopList.Count);

						SimulateErrors();
					}

					if (restoredCount > 0)
						Platform.Log(LogLevel.Info, "{0} SOP(s) have been restored.", restoredCount);

				}
				else
				{
					// Different study folder was used. Original folder must be kept around 
					// because we are rolling back.
					_deleteOriginalFolder = false;
				}
			}

            
		}

		private static void SimulateErrors()
		{
			RandomError.Generate(Settings.SimulateEditError, "Update study errors");
		}

		private void UpdateEntity(ServerEntity entity)
		{
			EntityDicomMap entityMap = EntityDicomMapManager.Get(entity.GetType());

			foreach (BaseImageLevelUpdateCommand command in _commands)
			{
				ImageLevelUpdateEntry entry = command.UpdateEntry;
				if (entityMap.ContainsKey(entry.TagPath.Tag))
				{
					string value = entry.GetStringValue();
					DicomTag tag = entry.TagPath.Tag;
					if (tag.TagValue == DicomTags.PatientsSex)
					{
						// Valid Patient's Sex value : "M", "F" or "O"
						if (!String.IsNullOrEmpty(value) && !value.ToUpper().Equals("M") && !value.ToUpper().Equals("F"))
							value = "O";
					}

					if (!entityMap.Populate(entity, entry.TagPath.Tag, value))
						throw new ApplicationException(String.Format("Unable to update {0}. See log file for details.", entity.Name));
				}
			}
		}        

		private void LoadEntities()
		{
			_storage = StudyStorage.Load(_oldStudyLocation.GetKey());
			_study = _storage.LoadStudy(UpdateContext);
		}

		private void UpdateDatabase()
		{
			LoadEntities();

			UpdateEntity(_study);
			UpdateEntity(_curPatient);
			UpdateEntity(_storage);

			// Update the Study table
			IStudyEntityBroker studyUpdateBroker = UpdateContext.GetBroker<IStudyEntityBroker>();
			studyUpdateBroker.Update(_study);
            
			// Update the StudyStorage table
			IStudyStorageEntityBroker storageUpdateBroker = UpdateContext.GetBroker<IStudyStorageEntityBroker>();
			storageUpdateBroker.Update(_storage);

			// Update the FilesystemStudyStorage table
			IFilesystemStudyStorageEntityBroker filesystemStorageBroker = UpdateContext.GetBroker<IFilesystemStudyStorageEntityBroker>();
			FilesystemStudyStorageSelectCriteria criteria = new FilesystemStudyStorageSelectCriteria();
			criteria.FilesystemKey.Equals(_oldStudyLocation.FilesystemKey);
			criteria.StudyStorageKey.EqualTo(_oldStudyLocation.GetKey());
			FilesystemStudyStorageUpdateColumns columns = new FilesystemStudyStorageUpdateColumns();
			columns.StudyFolder = _newStudyFolder;
			filesystemStorageBroker.Update(criteria, columns);

			// Update Patient level info. Different cases can occur here: 
			//      A) Patient demographic info is not changed ==> update the current patient
			//      B) New patient demographics matches (another) existing patient in the datbase 
			//              ==> Transfer the study to that patient. This means the study count on both patients must be updated.
			//                  The current patient should also be deleted if there's no more study attached to it after the transfer.
			//      C) New patient demographics doesn't match any patient in the database
			//              ==> A new patient should be created for this study. The study count on the current patient should be updated
			//                  and the patient should also be deleted if this is the only study attached to it.
			if (_patientInfoIsNotChanged)
			{
				UpdateCurrentPatient();
			}
			else 
			{
				if (_newPatient == null) 
				{
					// No matching patient in the database. We should create a new patient for this study
					_newPatient = CreateNewPatient(_newPatientInfo); 
				}
				else
				{
					// There's already patient in the database with the new patient demographics
					// The study should be attached to that patient.
					TransferStudy(_study, _oldPatientInfo, _newPatient);
				}
			}


			Rearchive();
		}

		private void Rearchive()
		{
			Platform.Log(LogLevel.Info, "Scheduling/Updating study archive..");
			_storage.Archive(UpdateContext);
		}

		private Patient CreateNewPatient(PatientInfo patientInfo)
		{
			Platform.Log(LogLevel.Info, "Creating new patient {0}", patientInfo.PatientId);
			ICreatePatientForStudy createStudyBroker = UpdateContext.GetBroker<ICreatePatientForStudy>();
			CreatePatientForStudyParameters parms = new CreatePatientForStudyParameters();
			parms.IssuerOfPatientId = patientInfo.IssuerOfPatientId;
			parms.PatientId = patientInfo.PatientId;
			parms.PatientsName = patientInfo.Name;
			parms.SpecificCharacterSet = _curPatient.SpecificCharacterSet; // assume it's the same
			parms.StudyKey = _study.GetKey();
			Patient newPatient = createStudyBroker.FindOne(parms);
			if (newPatient==null)
				throw new ApplicationException("Unable to create patient for the study");

			return newPatient;
		}

		private void UpdateCurrentPatient()
		{
			Platform.Log(LogLevel.Info, "Update current patient record...");
			IPatientEntityBroker patientUpdateBroker = UpdateContext.GetBroker<IPatientEntityBroker>();
			patientUpdateBroker.Update(_curPatient);
		}

		private void TransferStudy(Study study, PatientInfo oldPatient, Patient newPatient)
		{
			Platform.Log(LogLevel.Info, "Transferring study from {0} [ID={1}] to {2} [ID={3}]",
			             oldPatient.Name, oldPatient.PatientId, newPatient.PatientsName, newPatient.PatientId);

			IAttachStudyToPatient attachStudyToPatientBroker = UpdateContext.GetBroker<IAttachStudyToPatient>();
			AttachStudyToPatientParamaters parms = new AttachStudyToPatientParamaters();
			parms.StudyKey = study.GetKey();
			parms.NewPatientKey = newPatient.GetKey();
			attachStudyToPatientBroker.Execute(parms);
		}

		private void UpdateFilesystem()
		{
			Platform.Log(LogLevel.Info, "Updating filesystem...");
			StudyXml studyXml = _oldStudyLocation.LoadStudyXml();
			StudyXmlOutputSettings outputSettings = ImageServerCommonConfiguration.DefaultStudyXmlOutputSettings;

			StudyXml newStudyXml = new StudyXml();
			foreach (SeriesXml seriesXml in studyXml)
			{
				foreach (InstanceXml instanceXml in seriesXml)
				{
					string path = Path.Combine(_oldStudyPath, seriesXml.SeriesInstanceUid);
					path = Path.Combine(path, instanceXml.SopInstanceUid);
					path += ".dcm";

					//create backup
					try
					{
						DicomFile file = new DicomFile(path);
						file.Load();

						InstanceInfo instance = new InstanceInfo();
						instance.SeriesInstanceUid = file.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
						instance.SopInstanceUid = file.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);
                        
						foreach (BaseImageLevelUpdateCommand command in _commands)
						{
							command.File = file;
							command.Apply(file);
						}

						SaveFile(file);


						_updatedSopList.Add(instance);

						long fileSize = 0;
						if (File.Exists(file.Filename))
						{
							FileInfo finfo = new FileInfo(file.Filename);

							fileSize = finfo.Length;
						}
						newStudyXml.AddFile(file, fileSize, outputSettings);

						Platform.Log(ServerPlatform.InstanceLogLevel, "SOP {0} updated [{1} of {2}].", instance.SopInstanceUid, _updatedSopList.Count, _totalSopCount);

						SimulateErrors();
					}
					catch (Exception)
					{
						File.Delete(Path.Combine(_backupDir, instanceXml.SopInstanceUid) + ".bak"); //dont' need to restore this file
						throw;
					}

				}
			}

			if (_updatedSopList.Count != _totalSopCount)
			{
				Platform.Log(LogLevel.Warn, "Inconsistent data: expected {0} instances to be updated / Found {1}.", _totalSopCount, _updatedSopList.Count);
			}

			Platform.Log(LogLevel.Info, "Generating new study header...");
			string newStudyXmlPath = Path.Combine(NewStudyPath, _newStudyInstanceUid + ".xml");
			string gzipStudyXmlPath = Path.Combine(NewStudyPath, _newStudyInstanceUid + ".xml.gz");
			using (FileStream xmlStream = FileStreamOpener.OpenForSoleUpdate(newStudyXmlPath, FileMode.Create),
			                  gzipStream = FileStreamOpener.OpenForSoleUpdate(gzipStudyXmlPath, FileMode.Create))
			{
				StudyXmlIo.WriteXmlAndGzip(newStudyXml.GetMemento(outputSettings), xmlStream, gzipStream);
				xmlStream.Close();
				gzipStream.Close();
			}
		}

		private void SaveFile(DicomFile file)
		{
			String seriesInstanceUid = file.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
			String sopInstanceUid = file.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);

			String destPath = _oldStudyLocation.FilesystemPath;
			String extension = ".dcm";

			using (ServerCommandProcessor filesystemUpdateProcessor = new ServerCommandProcessor("Update Study"))
			{
				filesystemUpdateProcessor.AddCommand(new CreateDirectoryCommand(destPath));

				destPath = Path.Combine(destPath, _partition.PartitionFolder);
				filesystemUpdateProcessor.AddCommand(new CreateDirectoryCommand(destPath));

				destPath = Path.Combine(destPath, _newStudyFolder);
				filesystemUpdateProcessor.AddCommand(new CreateDirectoryCommand(destPath));

				destPath = Path.Combine(destPath, _newStudyInstanceUid);
				filesystemUpdateProcessor.AddCommand(new CreateDirectoryCommand(destPath));

				destPath = Path.Combine(destPath, seriesInstanceUid);
				filesystemUpdateProcessor.AddCommand(new CreateDirectoryCommand(destPath));

				destPath = Path.Combine(destPath, sopInstanceUid);
				destPath += extension;

				if (File.Exists(destPath))
				{
					// overwrite it
				}

				SaveDicomFileCommand saveCommand = new SaveDicomFileCommand(destPath, file, false, true);
				filesystemUpdateProcessor.AddCommand(saveCommand);

				if (_rulesEngine != null)
				{
					ServerActionContext context = new ServerActionContext(file, _oldStudyLocation.FilesystemKey, _partition, _oldStudyLocation.Key);
					context.CommandProcessor = filesystemUpdateProcessor;
					_rulesEngine.Execute(context);
				}

				if (!filesystemUpdateProcessor.Execute())
				{
					throw new ApplicationException(String.Format("Unable to update image {0} : {1}", file.Filename, filesystemUpdateProcessor.FailureReason));
				}
			}
            
		}

		private void BackupFilesystem()
		{
			Platform.Log(LogLevel.Info, "Backing up current study folder to {0}", _backupDir);
			StudyXml studyXml = _oldStudyLocation.LoadStudyXml();
			FileUtils.Copy(_oldStudyLocation.GetStudyXmlPath(), Path.Combine(_backupDir, _study.StudyInstanceUid + ".xml"), true);
			FileUtils.Copy(_oldStudyLocation.GetCompressedStudyXmlPath(), Path.Combine(_backupDir, _study.StudyInstanceUid + ".xml.gz"), true);

			foreach(SeriesXml seriesXml in studyXml)
			{
				foreach(InstanceXml instanceXml in seriesXml)
				{
					string existingFile = _oldStudyLocation.GetSopInstancePath(seriesXml.SeriesInstanceUid, instanceXml.SopInstanceUid);

					FileInfo backupPath = new FileInfo(Path.Combine(_backupDir, instanceXml.SopInstanceUid + ".dcm"));
					FileUtils.Copy(existingFile, backupPath.FullName, true );
				}
			}

			Platform.Log(LogLevel.Info, "A copy of {0} has been saved in {1}.", _oldStudyInstanceUid, _backupDir);
		}


		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			if (RollBackRequested)
			{
				if (_restored)
					CleaupBackupFiles();
			}
			else
			{
				if (NewStudyPath != _oldStudyPath && _deleteOriginalFolder)
				{
					Platform.Log(LogLevel.Info, "Removing old study folder...");
					DirectoryUtility.DeleteIfExists(_oldStudyPath, true);
				}
                
				CleaupBackupFiles();
			}
		}

		#endregion
	}
}