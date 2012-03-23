#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.ImageViewer.StudyManagement.Storage;

namespace ClearCanvas.ImageViewer.Dicom.Core.Command
{
    public class ViewerCommandProcessorContext : ICommandProcessorContext
    {

        public DataAccessContext DataAccessContext { get; private set; }
        public ViewerCommandProcessorContext()
        {
            DataAccessContext = new DataAccessContext();
            BackupDirectory = Path.GetTempPath();
        }

        public void Dispose()
        {
            DataAccessContext.Dispose();
            DataAccessContext = null;
        }

        public void PreExecute(ICommand command)
        {
            var dataAccessComand = command as DataAccessCommand;
            if (dataAccessComand != null)
                dataAccessComand.DataAccessContext = DataAccessContext;
        }

        public void Commit()
        {
            if (DataAccessContext == null) 
                throw new ApplicationException("Unable to commit, no DataAccessContext.");

            DataAccessContext.Commit();
        }

        public void Rollback()
        {
            if (DataAccessContext != null)
            {
                DataAccessContext.Dispose();
                DataAccessContext = null;
            }
        }

        public string TempDirectory
        {
            get { return Path.GetTempPath(); }
        }

        public string BackupDirectory
        {
            get;
            set;
        }
    }
}
