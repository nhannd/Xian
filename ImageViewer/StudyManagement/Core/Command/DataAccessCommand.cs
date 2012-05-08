#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.ImageViewer.StudyManagement.Storage;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Command
{
    public abstract class DataAccessCommand : CommandBase
    {
        public DataAccessContext DataAccessContext { get; set; }

        protected DataAccessCommand(string description) : base(description, true)
        {
        }

        protected override void OnUndo()
        {
            // Handle automatically via a rollback
        }
    }
}
