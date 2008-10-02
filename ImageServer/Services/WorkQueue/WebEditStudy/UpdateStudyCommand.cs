using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy
{
    /// <summary>
    /// Command for updating a study with rollback capability
    /// </summary>
    class UpdateStudyCommand :ServerDatabaseCommand, IDisposable
    {
        #region Private Members
        private readonly StudyUpdater _updater;
        private readonly string _oldStudyPath;
        private readonly string _backupDir = ServerPlatform.GetTempPath();
        private readonly StudyStorageLocation _studyLocation;
        private bool _undo=false;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of <see cref="UpdateStudyCommand"/> to update a study
        /// </summary>
        /// <param name="partition"></param>
        /// <param name="studyLocation"></param>
        /// <param name="_imageLevelCommands"></param>
        public UpdateStudyCommand(ServerPartition partition, 
                                  StudyStorageLocation studyLocation,
                                  IList<IImageLevelUpdateCommand> _imageLevelCommands) 
            : base("Update existing study", true)
        {
            _studyLocation = studyLocation;
            _updater = new StudyUpdater(partition, studyLocation, _imageLevelCommands);
            _updater.SopUpdating += SopUpdating;
            _updater.StudyUpdated += StudyUpdated;
            _updater.SopUpdated += SopUpdated;
            _oldStudyPath = studyLocation.GetStudyPath();
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the study path of the study after it is updated
        /// </summary>
        public String NewStudyPath
        {
            get
            {
                return _updater.NewStudyPath;
            }
        }

        /// <summary>
        /// Gets the study instance uid of the study after it is updated
        /// </summary>
        public String NewStudyInstanceUid
        {
            get
            {
                return _updater.NewStudyInstanceUid;
            }
        }

        /// <summary>
        /// Gets the name of study folder
        /// </summary>
        public String NewStudyFolder
        {
            get
            {
                return _updater.NewStudyFolder;
            }
        }

        #endregion

        #region Private Methods

        void SopUpdated(object sender, SopUpdatedEventArgs e)
        {
            Platform.Log(LogLevel.Info, "SOP {0} updated.", e.Uid);
        }

        void StudyUpdated(object sender, StudyUpdatedEventArgs e)
        {
            Platform.Log(LogLevel.Info, "Study {0} has been updated. New Study Instance Uid={1}. Location={2}",
                         e.OldStudyInstanceUid, e.StudyInstanceUid, e.Path);
        }

        void SopUpdating(object sender, SopUpdatingEventArgs e)
        {
            Platform.Log(LogLevel.Debug, "Updating {0}", e.Uid);
        }

        private void Rollback()
        {
            Platform.Log(LogLevel.Info, "Rolling back changes", _backupDir);
            DirectoryUtility.DeleteIfExists(_updater.NewStudyPath);

            if (NewStudyPath != _oldStudyPath)
            {
                DirectoryUtility.Copy(_backupDir, _oldStudyPath);
                Platform.Log(LogLevel.Info, "Original study files have been restored");
            }
            _undo = true;

        }

        private void BackupFilesystem()
        {
            Platform.Log(LogLevel.Info, "Backing up current study folder to {0}", _backupDir);
            DirectoryUtility.Copy(_oldStudyPath, _backupDir);
            Platform.Log(LogLevel.Info, "A copy of the study is stored in {0}", _backupDir);
        }

        #endregion

        #region Overriden Protected Methods

        protected override void OnExecute(IUpdateContext updateContext)
        {
            Platform.Log(LogLevel.Info, "Updating study {0}", _studyLocation.StudyInstanceUid);
            BackupFilesystem();
            _updater.Update(updateContext);
            
        }

        protected override void OnUndo()
        {
            Rollback();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            DirectoryUtility.DeleteIfExists(_backupDir);

            if (NewStudyPath != _oldStudyPath)
            {
                if (!_undo) 
                    DirectoryUtility.DeleteIfExists(_oldStudyPath);
            }

            if (_updater!=null)
                _updater.Dispose();
        }

        #endregion
    }
}