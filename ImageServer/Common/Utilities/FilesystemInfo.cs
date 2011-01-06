#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;

namespace ClearCanvas.ImageServer.Common.Utilities
{
    /// <summary>
    /// Contains information about a filesystem folder.
    /// </summary>
    /// <remarks>
    /// </remarks>
    [DataContract] 
    public class FilesystemInfo
    {
        #region Private members
        private string _path;
        private bool _exists;
        private ulong _sizeInKb;
        private ulong _freeSizeInKb;

        #endregion Private members

        #region Public properties
        [DataMember(IsRequired=true)]
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        [DataMember]
        public bool Exists
        {
            get { return _exists; }
            set { _exists = value; }
        }

        [DataMember]
        public ulong SizeInKB
        {
            get { return _sizeInKb; }
            set { _sizeInKb = value; }
        }

        [DataMember]
        public ulong FreeSizeInKB
        {
            get { return _freeSizeInKb; }
            set { _freeSizeInKb = value; }
        }

        #endregion Public properties
    }

}
