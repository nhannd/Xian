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
    /// <summary>
    /// Context object used by the <see cref="ViewerCommandProcessor"/>.
    /// </summary>
    public class ViewerCommandProcessorContext : ICommandProcessorContext
    {
        private bool _disposed;
        private DataAccessContext _context;

        public DataAccessContext DataAccessContext
        {
            get { return _context ?? (_context = new DataAccessContext(DataAccessContext.WorkItemMutex)); }
        }

        public ViewerCommandProcessorContext()
        {
            BackupDirectory = Path.GetTempPath();
        }

        public void Dispose()
        {
            if (_disposed)
                throw new InvalidOperationException("Already disposed.");
            
            _disposed = true;
            
            if (_context != null)
            {
                _context.Dispose();
                _context = null;
            }
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
            if (_context != null)
            {
                _context.Dispose();
                _context = null;
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
