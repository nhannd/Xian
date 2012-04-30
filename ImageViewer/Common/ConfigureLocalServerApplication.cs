#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common.DicomServer;
using ClearCanvas.ImageViewer.Common.StudyManagement;

namespace ClearCanvas.ImageViewer.Common
{
    /// <summary>
    /// Setup application for the installer to set the AE Title and port of the DICOM Server.
    /// </summary>
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    internal class ConfigureLocalServerApplication : IApplicationRoot
    {
        private class CommandLine : ClearCanvas.Common.Utilities.CommandLine
        {
            [CommandLineParameter("ae", "Sets the AE title of the local DICOM server.", Required = true)]
            public string AETitle { get; set;}

            [CommandLineParameter("host", "Sets the host name of the local DICOM server.", Required = false)]
            public string HostName { get; set; }

            [CommandLineParameter("port", "Sets the listening port of the local DICOM server.", Required = true)]
            public int Port { get; set; }

            [CommandLineParameter("filestore", "Sets the location of the file store.", Required = false)]
            public string FileStoreDirectory { get; set; }

            [CommandLineParameter("minspacepercent", "Sets the minimum used space required on the file store volume for the server to continue accepting studies.", Required = false)]
            public double? MinimumFreeSpacePercent { get; set; }
        }

        #region Implementation of IApplicationRoot

        public void RunApplication(string[] args)
        {
            var commandLine = new CommandLine();
            commandLine.Parse(args);

            try
            {
                DicomServer.DicomServer.UpdateConfiguration(new DicomServerConfiguration
                                                                {
                                                                    HostName = commandLine.HostName,
                                                                    AETitle = commandLine.AETitle,
                                                                    Port = commandLine.Port
                                                                });
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Warn, e);
            }

            try
            {
                StudyStore.UpdateConfiguration(new StorageConfiguration
                                                   {
                                                       FileStoreDirectory = commandLine.FileStoreDirectory,
                                                       MinimumFreeSpacePercent =
                                                           commandLine.MinimumFreeSpacePercent.HasValue
                                                               ? commandLine.MinimumFreeSpacePercent.Value
                                                               : StorageConfiguration.AutoMinimumFreeSpace
                                                   });
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Warn, e);
            }
        }

        #endregion
    }
}
