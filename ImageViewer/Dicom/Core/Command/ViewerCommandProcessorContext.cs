#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.ImageViewer.StudyManagement.Storage;

namespace ClearCanvas.ImageViewer.Dicom.Core.Command
{
    public class ViewerCommandProcessorContext : ICommandProcessorContext
    {
        private DataAccessScope _scope;

        public ViewerCommandProcessorContext()
        {
            _scope = new DataAccessScope();
        }

        public void Dispose()
        {
            _scope.Dispose();
            _scope = null;
        }

        public void PreExecute(ICommand command)
        {
            // No need for this in the viewer, we just use the scope internally for now
        }

        public void Commit()
        {
            _scope.SubmitChanges();
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string TempDirectory
        {
            get { throw new NotImplementedException(); }
        }

        public string BackupDirectory
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }
}
