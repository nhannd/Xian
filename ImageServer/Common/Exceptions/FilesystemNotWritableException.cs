#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.ImageServer.Common.Exceptions
{
    /// <summary>
    /// Represents the exception thrown when the study is online but the filesystem is missing or not writable.
    /// </summary>
    public class FilesystemNotWritableException : SopInstanceProcessingException
    {

        public string Path { get; set; }
        public string Reason { get; set; }

        public FilesystemNotWritableException()
            : base("Study is online but the filesystem is no longer writable.")
        {
        }

        public FilesystemNotWritableException(string path) 
            : base(String.Format("Filesystem is not writable: {0}", path))
        {
            Path = path;
        }

        public override string ToString()
        {
            return string.Format("{0} : {1}", Path, Reason);
        }
    }
}