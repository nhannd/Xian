using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.TierMigrate
{
    class TierMigrationContext
    {
        #region Private members
        private StudyStorageLocation _origLocation;
        private ServerFilesystemInfo _destination;
        #endregion Private members
        
        /// <summary>
        /// The original location of the study being migrated.
        /// </summary>
        public StudyStorageLocation OriginalStudyLocation
        {
            get
            {
                return _origLocation;
            }
            set
            {
                _origLocation = value;
            }
        }

        public ServerFilesystemInfo Destination
        {
            get { return _destination; }
            set { _destination = value; }
        }


    }
}
