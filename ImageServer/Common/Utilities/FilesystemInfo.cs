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

        #endregion Private members

        #region Public properties

        [DataMember(IsRequired=true)]
        public string Path { get; set; }

        [DataMember]
        public bool Exists { get; set; }

        [DataMember]
        public ulong SizeInKB { get; set; }

        [DataMember]
        public ulong FreeSizeInKB { get; set; }

        #endregion Public properties
    }

}
