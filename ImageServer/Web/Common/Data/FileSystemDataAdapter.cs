using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Criteria;
using ClearCanvas.ImageServer.Model.EnumBrokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Model.SelectBrokers;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
    /// <summary>
    /// Used to create/update/delete file system entries in the database.
    /// </summary>
    ///
    public class FileSystemDataAdapter
    {
        #region Private Members
        private IPersistentStore _store = PersistentStoreRegistry.GetDefaultStore();
        #endregion Private Members

        #region Public methods
        /// <summary>
        /// Gets a list of all file systems.
        /// </summary>
        /// <returns></returns>
        public IList<Filesystem> GetAllFileSystems()
        {
            IList<Filesystem> list = null;
            using (IReadContext ctx = _store.OpenReadContext())
            {
                ISelectFilesystem find = ctx.GetBroker<ISelectFilesystem>();

                FilesystemSelectCriteria criteria = new FilesystemSelectCriteria();
                list = find.Find(criteria);
            }

            return list;

        }

        public IList<Filesystem> GetFileSystems(FilesystemSelectCriteria criteria)
        {
            IList<Filesystem> list = null;
            using (IReadContext ctx = _store.OpenReadContext())
            {
                ISelectFilesystem select = ctx.GetBroker<ISelectFilesystem>();
                list = select.Find(criteria);
            }

            return list;

        }



        /// <summary>
        /// Creats a new file system.
        /// </summary>
        /// <param name="filesystem"></param>
        public bool AddFileSystem(Filesystem filesystem)
        {
            bool ok = false;
            IList<Filesystem> list = null;

            using (IUpdateContext ctx = _store.OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IInsertFilesystem insert = ctx.GetBroker<IInsertFilesystem>();
                FilesystemInsertParameters parms = new FilesystemInsertParameters();
                parms.Description = filesystem.Description;
                parms.Enabled = filesystem.Enabled;
                parms.FilesystemPath = filesystem.FilesystemPath;
                parms.ReadOnly = filesystem.ReadOnly;
                parms.TypeEnum = filesystem.FilesystemTierEnum;
                parms.WriteOnly = filesystem.WriteOnly;
                parms.HighWatermark = filesystem.HighWatermark;
                parms.LowWatermark = filesystem.LowWatermark;

                list = insert.Execute(parms);

                ok = list != null && list.Count > 0;

                if (ok)
                    ctx.Commit();
            }

           
            return ok;
        }



        public bool Update(Filesystem filesystem)
        {
            bool ok = false;

            using (IUpdateContext ctx = _store.OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IUpdateFilesystem update = ctx.GetBroker<IUpdateFilesystem>();
                FilesystemUpdateParameters parms = new FilesystemUpdateParameters();
                parms.FileSystemKey = filesystem.GetKey();
                parms.Description = filesystem.Description;
                parms.Enabled = filesystem.Enabled;
                parms.FilesystemPath = filesystem.FilesystemPath;
                parms.ReadOnly = filesystem.ReadOnly;
                parms.FilesystemTierEnum = filesystem.FilesystemTierEnum;
                parms.WriteOnly = filesystem.WriteOnly;
                parms.HighWatermark = filesystem.HighWatermark;
                parms.LowWatermark = filesystem.LowWatermark;

                ok = update.Execute(parms);
                if (ok)
                    ctx.Commit();
            }

            return ok;

        }

        public IList<FilesystemTierEnum> GetFileSystemTiers()
        {
            IList<FilesystemTierEnum> result = null;
            using (IReadContext ctx = _store.OpenReadContext())
            {
                IFilesystemTierEnum select = ctx.GetBroker<IFilesystemTierEnum>();
                result = select.Execute();
            }

            return result;
        }



        #endregion Public methods
    }
}
