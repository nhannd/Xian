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

namespace ClearCanvas.ImageServer.Services.Dicom
{
    /// <summary>
    /// Class used to pass parameters to the ImageServer SCP extensions.
    /// </summary>
    public class DicomScpContext
    {
        #region Constructors
        public DicomScpContext(ServerPartition partition)
        {
            _partition = partition;
        }
        #endregion

        #region Private Members
        private ServerPartition _partition;
        #endregion

        #region Properties
        public ServerPartition Partition
        {
            get { return _partition; }
			set { _partition = value; }
        }
        #endregion
    }
}