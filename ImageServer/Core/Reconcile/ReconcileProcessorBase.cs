#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Command;
using ClearCanvas.ImageServer.Core.Command;
using ClearCanvas.ImageServer.Core.Reconcile.CreateStudy;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Reconcile
{
    internal abstract class ReconcileProcessorBase : ServerCommandProcessor
    {
        private ReconcileStudyProcessorContext _context;

        /// <summary>
        /// Create an instance of <see cref="ReconcileCreateStudyProcessor"/>
        /// </summary>
        protected ReconcileProcessorBase(string name)
            :base(name)
        {
        }

        protected internal ReconcileStudyProcessorContext Context
        {
            get { return _context; }
            set { _context = value; }
        }



        public string Name
        {
            get { return Description; }
        }

        protected void AddCleanupCommands()
        {
            string[] paths = GetFoldersToRemove();
            foreach(string path in paths)
            {
                AddCommand(new DeleteDirectoryCommand(path, false, true));
            }
        }

        protected string[] GetFoldersToRemove()
        {
            List<string> folders = new List<string>();

            Model.WorkQueue workqueueItem = Context.WorkQueueItem;

            // Path = \\Filesystem Path\Reconcile\GroupID\StudyInstanceUid\*.dcm
            if (!String.IsNullOrEmpty(workqueueItem.GroupID))
            {
                StudyStorageLocation storageLocation = Context.WorkQueueItemStudyStorage;
                string path = Path.Combine(storageLocation.FilesystemPath, storageLocation.PartitionFolder);
                path = Path.Combine(path, ServerPlatform.ReconcileStorageFolder);
                path = Path.Combine(path, workqueueItem.GroupID);
                
                string studyFolderPath = Path.Combine(path, storageLocation.StudyInstanceUid);

                folders.Add(studyFolderPath);
                folders.Add(path);
            }
            else
            {
                #region BACKWARD-COMPATIBLE CODE
                string path = Context.ReconcileWorkQueueData.StoragePath;
                folders.Add(path);
                #endregion
            }

            return folders.ToArray();
        }
    }
}