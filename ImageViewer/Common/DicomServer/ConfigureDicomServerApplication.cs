#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Common.DicomServer
{
    /// <summary>
    /// Setup application for the installer to set the AE Title and port of the DICOM Server.
    /// </summary>
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    internal class ConfigureDicomServerApplication : IApplicationRoot
    {
        private class CommandLine : ClearCanvas.Common.Utilities.CommandLine
        {
            [CommandLineParameter("ae", "Sets the AE title of dicom server.")]
            public string AETitle { get; set;}

            [CommandLineParameter("port", "Sets the listening port of the dicom server.")]
            public int Port { get; set; }
        }

        #region Implementation of IApplicationRoot

        public void RunApplication(string[] args)
        {
            var commandLine = new CommandLine();
            commandLine.Parse(args);

            Platform.GetService<IDicomServerConfiguration>(
                s => s.UpdateConfiguration(new UpdateDicomServerConfigurationRequest
                                          {
                                              Configuration = new DicomServerConfiguration
                                                      {AETitle = commandLine.AETitle, Port = commandLine.Port}
                                          }));
        }

        #endregion
    }
}
