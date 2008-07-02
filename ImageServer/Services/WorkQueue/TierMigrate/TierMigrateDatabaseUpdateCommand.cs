using System.Collections.Generic;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
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
            // update StorageFilesystem
            if (Context != null)
            {
                IStorageFilesystemEntityBroker broker = updateContext.GetBroker<IStorageFilesystemEntityBroker>();

                StorageFilesystemSelectCriteria searchCriteria = new StorageFilesystemSelectCriteria();
                searchCriteria.StudyStorageKey.EqualTo(Context.OriginalStudyLocation.GetKey());
                searchCriteria.FilesystemKey.EqualTo(Context.OriginalStudyLocation.FilesystemKey);
                IList<StorageFilesystem> storageFilesystems = broker.Find(searchCriteria);

                Debug.Assert(storageFilesystems.Count == 1);

                StorageFilesystem storageFilesystem = storageFilesystems[0];
                storageFilesystem.FilesystemKey = Context.Destination.Filesystem.GetKey();

                broker.Update(storageFilesystem);


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
