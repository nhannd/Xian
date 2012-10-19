#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Common;

namespace ClearCanvas.ImageServer.Core.ModelExtensions
{

    public static class StudyStorageLocationExtensions
    {
        /// <summary>
        /// Returns the path to the Reconcile folder for the Partition in the same filesystem <see cref="location"/> where study is stored.
        /// This is usually \\filesystemPath\PartitionFolder\RECONCILE
        /// </summary>
        /// <returns></returns>
        public static string GetReconcileRootPath(this StudyStorageLocation location)
        {
            string path = Path.Combine(location.FilesystemPath, location.PartitionFolder);
            path = Path.Combine(path, ServerPlatform.ReconcileStorageFolder);
            return path;
        }
    }
}
