using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

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
        private DicomServerEventManager _serverManager;
        private int _progressBytes;
        private int _totalBytes;
        private string _sopInstanceUID;

        /// <summary>
        /// Constructor
        /// </summary>
        public DicomServerTestComponent()
        {
            _serverManager = new DicomServerEventManager();
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

        #region Properties

        public string AETitle
        {
            get { return _serverManager.AETitle; }
        }

        public int Port
        {
            get { return _serverManager.Port; }
        }

        public string SaveDirectory
        {
            get { return _serverManager.SaveDirectory; }
        }

        public bool IsServerStarted
        {
            get { return _serverManager.IsServerRunning; }
        }

        public string SOPInstanceUID
        {
            get { return _sopInstanceUID; }
        }

        public int ProgressBytes
        {
            get { return _progressBytes; }
        }

        public int TotalBytes
        {
            get { return _totalBytes; }
        }

        #endregion

        public void StartServer()
        {
            try
            {
                _serverManager.StartServer();
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public void StopServer()
        {
            try
            {
                if (_serverManager.IsServerRunning)
                    _serverManager.StopServer();
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            Host.Exit();
        }
    }
}
