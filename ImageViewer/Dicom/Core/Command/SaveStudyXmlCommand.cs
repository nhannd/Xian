using System;
using System.IO;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageViewer.StudyManagement.Storage;

namespace ClearCanvas.ImageViewer.Dicom.Core.Command
{
    public class SaveStudyXmlCommand : CommandBase
    {
        #region Private Members

        private readonly StudyXml _studyXml;
        private readonly StudyLocation _studyStorageLocation;
        private string _backupPath;
        private bool _fileCreated;

        #endregion

        public SaveStudyXmlCommand(StudyXml studyXml, StudyLocation storageLocation)
            : base("Save Study Xml", true)
        {
            _studyXml = studyXml;
            _studyStorageLocation = storageLocation;
        }

        private void Backup()
        {
            if (File.Exists(_studyStorageLocation.GetStudyXmlPath()))
            {
                try
                {
                    _backupPath = FileUtils.Backup(_studyStorageLocation.GetStudyXmlPath(), ProcessorContext.BackupDirectory);
                }
                catch (IOException)
                {
                    _backupPath = null;
                    throw;
                }
            }
        }

        protected override void OnExecute(CommandProcessor theProcessor)
        {
            Backup();

            // TODO (Marmot) - Probably shoudl move the saving back into this class, there's a small chance of issues with the _fileCreated flag not set right when the file is created
            _studyStorageLocation.SaveStudyXml(_studyXml);
            _fileCreated = true;
        }

        protected override void OnUndo()
        {
            if (false == String.IsNullOrEmpty(_backupPath) && File.Exists(_backupPath))
            {
                // restore original file
                File.Copy(_backupPath, _studyStorageLocation.GetStudyXmlPath(), true);
                File.Delete(_backupPath);
                _backupPath = null;
            }
            else if (File.Exists(_studyStorageLocation.GetStudyXmlPath()) && _fileCreated)
                File.Delete(_studyStorageLocation.GetStudyXmlPath());
        }
    }
}
