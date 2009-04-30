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

using System.Collections.Generic;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
    /// <summary>
    /// Used to create/update/delete file system entries in the database.
    /// </summary>
    ///
    public class FileSystemDataAdapter : BaseAdaptor<Filesystem, IFilesystemEntityBroker, FilesystemSelectCriteria,FilesystemUpdateColumns>
    {
        #region Public methods

        /// <summary>
        /// Gets a list of all file systems.
        /// </summary>
        /// <returns></returns>
        public IList<Filesystem> GetAllFileSystems()
        {
            return Get();
        }

        public IList<Filesystem> GetFileSystems(FilesystemSelectCriteria criteria)
        {
            return Get(criteria);
        }

        /// <summary>
        /// Creats a new file system.
        /// </summary>
        /// <param name="filesystem"></param>
        public bool AddFileSystem(Filesystem filesystem)
        {
            bool ok;

            // This filesystem update must be used, because the stored procedure does some 
            // additional work on insert.
            using (IUpdateContext ctx = PersistentStore.OpenUpdateContext(UpdateContextSyncMode.Flush))
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

                Filesystem newFilesystem = insert.FindOne(parms);

				ok = newFilesystem != null;

                if (ok)
                    ctx.Commit();
            }

            return ok;
        }


        public bool Update(Filesystem filesystem)
        {

            FilesystemUpdateColumns parms = new FilesystemUpdateColumns();
            parms.Description = filesystem.Description;
            parms.Enabled = filesystem.Enabled;
            parms.FilesystemPath = filesystem.FilesystemPath;
            parms.ReadOnly = filesystem.ReadOnly;
            parms.FilesystemTierEnum = filesystem.FilesystemTierEnum;
            parms.WriteOnly = filesystem.WriteOnly;
            parms.HighWatermark = filesystem.HighWatermark;
            parms.LowWatermark = filesystem.LowWatermark;


            return Update(filesystem.Key, parms);
        }

        public IList<FilesystemTierEnum> GetFileSystemTiers()
        {
            return FilesystemTierEnum.GetAll();
        }

        #endregion Public methods
    }
}
