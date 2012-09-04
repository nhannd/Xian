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
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue
{
    /// <summary>
    /// Represents the execution context where the WorkQueue item processor is run.
    /// </summary>
    public class WorkQueueProcessorContext : ServerExecutionContext
    {
        #region Private Fields
        private readonly Model.WorkQueue _item;
        #endregion

        public WorkQueueProcessorContext(Model.WorkQueue item)
            :base(item.GetKey().Key.ToString())
        {
            _item = item;
            
        }

        protected override string GetTemporaryPath()
        {
            IList<StudyStorageLocation> storages =
                StudyStorageLocation.FindStorageLocations(StudyStorage.Load(_item.StudyStorageKey));

            if (storages == null || storages.Count == 0)
            {
                // ???
                return base.GetTemporaryPath();
            }

            ServerFilesystemInfo filesystem = FilesystemMonitor.Instance.GetFilesystemInfo(storages[0].FilesystemKey);
            if (filesystem == null)
            {
                // not ready?
                return base.GetTemporaryPath();
            }
            string basePath = GetTempPathRoot();

            if (String.IsNullOrEmpty(basePath))
            {
                basePath = Path.Combine(filesystem.Filesystem.FilesystemPath, "temp");
            }

            String tempDirectory = Path.Combine(basePath,
                                                String.Format("{0}-{1}", _item.WorkQueueTypeEnum.Lookup, _item.GetKey()));

            for (int i = 2; i < 1000; i++)
            {
                if (!Directory.Exists(tempDirectory))
                {
                    break;
                }

                tempDirectory = Path.Combine(basePath, String.Format("{0}-{1}({2})",
                                                                     _item.WorkQueueTypeEnum.Lookup, _item.GetKey(), i));
            }

            if (!Directory.Exists(tempDirectory))
                Directory.CreateDirectory(tempDirectory);

            return tempDirectory;
        }
    }
}