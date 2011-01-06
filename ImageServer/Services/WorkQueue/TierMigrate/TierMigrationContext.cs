#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
