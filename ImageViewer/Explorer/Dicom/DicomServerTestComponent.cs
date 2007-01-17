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
        private DicomServerTestComponent _component;
        public void Launch()
        {
            if (_component == null)
            {
                _component = new DicomServerTestComponent();
                ApplicationComponent.LaunchAsShelf(
                        this.Context.DesktopWindow,
                        _component,
                        "DICOM Server",
                        ShelfDisplayHint.DockRight,
                        delegate(IApplicationComponent component) { _component = null; });
            }
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
        private string _saveDirectory;
        private int _port;
        private bool _isServerStarted;
        
        /// <summary>
        /// Constructor
        /// </summary>
        public DicomServerTestComponent()
        {
            DicomServerTree dicomServerTree = new DicomServerTree();
            if (dicomServerTree.CurrentServer != null)
            {
                DicomServer server = (DicomServer)dicomServerTree.CurrentServer;
                _aeTitle = server.DicomAE.AE;
                _port = server.DicomAE.Port;
            }
            else
            {
                _aeTitle = "STORESCP";
                _port = 4000;
            }

            _saveDirectory = ".\\dicom";
            _isServerStarted = false;
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Stop()
        {
            StopServer();
            base.Stop();
        }

        public void StartServer()
        {
            ApplicationEntity myOwnAEParameters = new ApplicationEntity(new HostName("localhost"),
                new AETitle(_aeTitle), new ListeningPort(_port));

            _dicomServer = new ClearCanvas.Dicom.Network.DicomServer(myOwnAEParameters, _saveDirectory);
            _dicomServer.FindScpEvent += OnFindScpEvent;
            _dicomServer.StoreScpEvent += OnStoreScpEvent;

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
                _dicomServer.StoreScpEvent -= OnStoreScpEvent;
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

        public string SaveDirectory
        {
            get { return _saveDirectory; }
            set { _saveDirectory = value; }
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

        private void OnStoreScpEvent(object sender, StoreScpEventArgs e)
        {
            if (e == null)
                return;

            try
            {
                // need to extract the transfersyntax from ImageDataSet and store in a separate MetaInfo variable as part of the InsertSopInstance argument
                DcmMetaInfo metaInfo = new DcmMetaInfo();
                string xfer = TransferSyntaxHelper.GetString(e.ImageDataSet.getOriginalXfer());
                if (xfer != null)
                    metaInfo.putAndInsertString(new DcmTag(Dcm.TransferSyntaxUID), xfer);

                IDicomPersistentStore store = DataAccessLayer.GetIDicomPersistentStore();
                if (store != null)
                {
                    store.InsertSopInstance(metaInfo, e.ImageDataSet, e.FileName);
                    store.Flush();
                }
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
