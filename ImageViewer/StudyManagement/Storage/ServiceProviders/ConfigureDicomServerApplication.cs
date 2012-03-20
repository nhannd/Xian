using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common.DicomServer;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.ServiceProviders
{
    /// <summary>
    /// Setup application for the installer to set the AE Title and port of the DICOM Server.
    /// </summary>
    internal class ConfigureDicomServerApplication : IApplicationRoot
    {
        private class CommandLine : ClearCanvas.Common.Utilities.CommandLine
        {
            public CommandLine():base()
            {
            }

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
                                              Configuration = new Common.DicomServer.DicomServerConfiguration
                                                      {AETitle = commandLine.AETitle, Port = commandLine.Port}
                                          }));
        }

        #endregion
    }
}
