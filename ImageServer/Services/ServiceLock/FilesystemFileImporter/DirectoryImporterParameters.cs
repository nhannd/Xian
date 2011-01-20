#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;

namespace ClearCanvas.ImageServer.Services.ServiceLock.FilesystemFileImporter
{
    internal class DirectoryImporterParameters
    {
        public String PartitionAE;
        public DirectoryInfo Directory;
        public int MaxImages;
        public int Delay;
        public string Filter;
    }
}