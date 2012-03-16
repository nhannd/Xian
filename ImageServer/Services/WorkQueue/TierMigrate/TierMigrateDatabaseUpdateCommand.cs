#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.WorkQueue.TierMigrate
{
    internal class TierMigrateDatabaseUpdateCommand : ServerDatabaseCommand
    {
        #region Private Memeber
        private TierMigrationContext _context = null;
        #endregion

        public TierMigrateDatabaseUpdateCommand(TierMigrationContext context)
            : base("TierMigrateDatabaseUpdateCommand")
        {
            Context = context;
        }

        public TierMigrationContext Context
        {
            get
            {
                return _context;
            }
            set
            {
                _context = value;
            }
        }

        protected override void OnExecute(CommandProcessor theProcessor, IUpdateContext updateContext)
        {
            // update FilesystemStudyStorage
            if (Context != null)
            {
                Platform.Log(LogLevel.Info, "Updating database...");
                IFilesystemStudyStorageEntityBroker broker = updateContext.GetBroker<IFilesystemStudyStorageEntityBroker>();

                FilesystemStudyStorageSelectCriteria searchCriteria = new FilesystemStudyStorageSelectCriteria();
                searchCriteria.StudyStorageKey.EqualTo(Context.OriginalStudyLocation.GetKey());
                searchCriteria.FilesystemKey.EqualTo(Context.OriginalStudyLocation.FilesystemKey);
                FilesystemStudyStorage filesystemStudyStorage = broker.FindOne(searchCriteria);
                Debug.Assert(filesystemStudyStorage != null);

                // Update Filesystem for the StudyStorage entry
                filesystemStudyStorage.FilesystemKey = Context.Destination.Filesystem.GetKey();
                broker.Update(filesystemStudyStorage);


                // Update Filesystem for the remaining FilesystemQueue entries
                IFilesystemQueueEntityBroker fsQueueBroker = updateContext.GetBroker<IFilesystemQueueEntityBroker>();                
                FilesystemQueueSelectCriteria fsQueueSearchCriteria = new FilesystemQueueSelectCriteria();
                fsQueueSearchCriteria.StudyStorageKey.EqualTo(Context.OriginalStudyLocation.GetKey());
                fsQueueSearchCriteria.FilesystemKey.EqualTo(Context.OriginalStudyLocation.FilesystemKey);

                FilesystemQueueUpdateColumns fsQueueUpdateColumns = new FilesystemQueueUpdateColumns();
                fsQueueUpdateColumns.FilesystemKey = Context.Destination.Filesystem.GetKey();
                fsQueueBroker.Update(fsQueueSearchCriteria, fsQueueUpdateColumns);
                
                // Insert or update Filesystem Queue table.
                IInsertFilesystemQueue insertFilesystemQueueBroker = updateContext.GetBroker<IInsertFilesystemQueue>();
                FilesystemQueueInsertParameters parms = new FilesystemQueueInsertParameters();
                parms.FilesystemKey = Context.Destination.Filesystem.GetKey();
                parms.FilesystemQueueTypeEnum = FilesystemQueueTypeEnum.TierMigrate;
                parms.ScheduledTime = Platform.Time;
                parms.StudyStorageKey = Context.OriginalStudyLocation.GetKey();
                insertFilesystemQueueBroker.Execute(parms);

                Platform.Log(LogLevel.Info, "Database is updated.");
                
            }
        }
    }
}
