using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

using ClearCanvas.Dicom;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    [MenuAction("launch", "global-menus/Tools/DICOM Server")]
    [ClickHandler("launch", "Launch")]
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class DICOMServerTestTool : Tool<IDesktopToolContext>
    {
        public void Launch()
        {
            ApplicationComponent.LaunchAsDialog(
                this.Context.DesktopWindow,
                new DicomServerTestComponent(),
                "DICOM Server");
        }
    }

    /// <summary>
    /// Extension point for views onto <see cref="DicomServerTestComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class DicomServerTestComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// DicomServerTestComponent class
    /// </summary>
    [AssociateView(typeof(DicomServerTestComponentViewExtensionPoint))]
    public class DicomServerTestComponent : ApplicationComponent
    {
        private ClearCanvas.Dicom.Network.DicomServer _dicomServer;
        private string _aeTitle;
        private int _port;
        private bool _isServerStarted;
        
        /// <summary>
        /// Constructor
        /// </summary>
        public DicomServerTestComponent()
        {
            _aeTitle = "JLNETTEST";
            _port = 4000;
            _isServerStarted = false;
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public void StartServer()
        {
            ApplicationEntity myOwnAEParameters = new ApplicationEntity(new HostName("localhost"),
                new AETitle(_aeTitle), new ListeningPort(_port));

            _dicomServer = new ClearCanvas.Dicom.Network.DicomServer(myOwnAEParameters, @"..\studies\");
            _dicomServer.FindScpEvent += OnFindScpEvent;
            try
            {
                _dicomServer.Start();
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }

            _isServerStarted = true;
        }

        public void StopServer()
        {
            if (_dicomServer != null)
            {
                _dicomServer.FindScpEvent -= OnFindScpEvent;
                _dicomServer.Stop();
                _dicomServer = null;
            }
            _isServerStarted = false;
        }

        public string AETitle
        {
            get { return _aeTitle; }
            set { _aeTitle = value; }
        }

        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        public bool IsServerStarted
        {
            get { return _isServerStarted; }
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            Host.Exit();
        }

        private void OnFindScpEvent(object sender, FindScpEventArgs e)
        {
            if (e == null)
                return;

            try
            {
                e.QueryResults = DataAccessLayer.GetIDataStoreReader().StudyQuery(e.QueryKey);
            }
            catch (System.Data.SqlClient.SqlException exception)
            {
                ExceptionHandler.Report(exception, "MessageUnableToConnectToDataStore", this.Host.DesktopWindow);
            }
            catch (Exception exception)
            {
                ExceptionHandler.Report(exception, this.Host.DesktopWindow);
            }
        }
    }
}
