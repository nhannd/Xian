using System.Collections.Generic;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Services.WorkQueue.TierMigrate
{
    internal class TierMigrateDatabaseUpdateCommand : ServerDatabaseCommand
    {
        #region Private Memeber
        private TierMigrationContext _context = null;
        #endregion

        public TierMigrateDatabaseUpdateCommand(TierMigrationContext context)
            : base("TierMigrateDatabaseUpdateCommand", true)
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

        protected override void OnExecute(IUpdateContext updateContext)
        {
            // update FilesystemStudyStorage
            if (Context != null)
            {
                IFilesystemStudyStorageEntityBroker broker = updateContext.GetBroker<IFilesystemStudyStorageEntityBroker>();

                FilesystemStudyStorageSelectCriteria searchCriteria = new FilesystemStudyStorageSelectCriteria();
                searchCriteria.StudyStorageKey.EqualTo(Context.OriginalStudyLocation.GetKey());
                searchCriteria.FilesystemKey.EqualTo(Context.OriginalStudyLocation.FilesystemKey);
                IList<FilesystemStudyStorage> storageFilesystems = broker.Find(searchCriteria);

                Debug.Assert(storageFilesystems.Count == 1);

                FilesystemStudyStorage filesystemStudyStorage = storageFilesystems[0];
                filesystemStudyStorage.FilesystemKey = Context.Destination.Filesystem.GetKey();

                broker.Update(filesystemStudyStorage);


                IFilesystemQueueEntityBroker fsQueueBroker = updateContext.GetBroker<IFilesystemQueueEntityBroker>();                
                FilesystemQueueSelectCriteria fsQueueSearchCriteria = new FilesystemQueueSelectCriteria();
                fsQueueSearchCriteria.StudyStorageKey.EqualTo(Context.OriginalStudyLocation.GetKey());
                fsQueueSearchCriteria.FilesystemKey.EqualTo(Context.OriginalStudyLocation.FilesystemKey);

                FilesystemQueueUpdateColumns fsQueueUpdateColumns = new FilesystemQueueUpdateColumns();
                fsQueueUpdateColumns.FilesystemKey = Context.Destination.Filesystem.GetKey();

                fsQueueBroker.Update(fsQueueSearchCriteria, fsQueueUpdateColumns);
                
                // insert new migration scheduling entry for the new filesystem
                FilesystemQueueUpdateColumns columns = new FilesystemQueueUpdateColumns();
                columns.FilesystemKey = Context.Destination.Filesystem.GetKey();
                columns.FilesystemQueueTypeEnum = FilesystemQueueTypeEnum.TierMigrate;
                columns.ScheduledTime = Platform.Time;
                columns.StudyStorageKey = Context.OriginalStudyLocation.GetKey();
                fsQueueBroker.Insert(columns);
            }
        }
    }
}
