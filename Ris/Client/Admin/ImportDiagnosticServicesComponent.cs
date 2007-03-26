using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise;
using ClearCanvas.Ris.Application.Common.Admin.DiagnosticServiceAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
    [MenuAction("launch", "global-menus/Tools/Import Diagnostic Services")]
    [ClickHandler("launch", "Launch")]
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class ImportDiagnosticServicesTool : Tool<IDesktopToolContext>
    {
        private IWorkspace _workspace;

        public void Launch()
        {
            if (_workspace == null)
            {
                ImportDiagnosticServicesComponent component = new ImportDiagnosticServicesComponent();

                _workspace = ApplicationComponent.LaunchAsWorkspace(
                    this.Context.DesktopWindow,
                    component,
                    SR.TitleImportDiagnosticServices,
                    delegate(IApplicationComponent c) { _workspace = null; });
            }
            else
            {
                _workspace.Activate();
            }
        }
    }

    /// <summary>
    /// Extension point for views onto <see cref="ImportDiagnosticServicesComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ImportDiagnosticServicesComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ImportDiagnosticServicesComponent class
    /// </summary>
    [AssociateView(typeof(ImportDiagnosticServicesComponentViewExtensionPoint))]
    public class ImportDiagnosticServicesComponent : ApplicationComponent
    {
        private string _filename;
        private int _batchSize;
        private List<string[]> _rows;

        /// <summary>
        /// Constructor
        /// </summary>
        public ImportDiagnosticServicesComponent()
        {
            // set default size
            _filename = "";
            _batchSize = 10;
        }

        public override void Start()
        {
            _rows = new List<string[]>();

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        #region Presentation Model

        public string FileName
        {
            get { return _filename; }
            set { _filename = value; }
        }

        public int BatchSize
        {
            get { return _batchSize; }
            set { _batchSize = value; }
        }

        public int NumberOfBatches
        {
            get { return (_batchSize == 0) ? 0 : (int) Math.Ceiling((decimal)_rows.Count / (decimal)_batchSize); }
        }

        #endregion

        public void OpenFile()
        {
            if (_filename == null)
                return;

            _rows.Clear();
            using (StreamReader reader = File.OpenText(_filename))
            {
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] row = line.Split(new string[] { "," }, StringSplitOptions.None);
                    _rows.Add(row);
                }
            }
        }

        public void StartImport()
        {
            // Start a background task to import diagnostic services
            BackgroundTask task = new BackgroundTask(delegate(IBackgroundTaskContext context) { ImportDiagnosticService(context); }, true);

            try
            {
                ProgressDialog.Show(task, this.Host.DesktopWindow);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        private void ImportDiagnosticService(IBackgroundTaskContext context)
        {
            if (NumberOfBatches <= 0)
            {
                context.Complete(null);
                return;
            }

            List<string[]> rowsToImport = new List<string[]>();
            int startRow = 0, endRow = 0;

            // do some background processing
            for (int batch = 1; batch <= NumberOfBatches; batch++)
            {
                int percentage = batch * 100 / NumberOfBatches;

                if (context.CancelRequested)
                {
                    context.ReportProgress(new BackgroundTaskProgress(percentage, String.Format("Cancelled after the {0} rows", endRow + 1)));
                    context.Cancel();
                    return;
                }

                // Parse out the next few rows to import
                startRow = (batch - 1) * _batchSize;
                endRow = Math.Min(batch * _batchSize, _rows.Count);
                rowsToImport.Clear();
                for (int i = startRow; i < endRow; i++)
                {
                    rowsToImport.Add(_rows[i]);
                }

                if (rowsToImport.Count > 0)
                {
                    try
                    {
                        Platform.GetService<IDiagnosticServiceAdminService>(
                            delegate(IDiagnosticServiceAdminService service)
                            {
                                service.BatchImport(new BatchImportRequest(rowsToImport));
                            });
                    }
                    catch (Exception e)
                    {
                        String message = String.Format("Error importing batch {0} of {1} between rows {2}~{3}", batch, NumberOfBatches, startRow, endRow);
                        context.Error(new Exception(message, e));
                        return;
                    }
                }

                context.ReportProgress(new BackgroundTaskProgress(percentage, String.Format("Importing batch {0} of {1}", batch, NumberOfBatches)));
            }

            context.ReportProgress(new BackgroundTaskProgress(100, String.Format("Importing {0} rows completed", _rows.Count)));

            context.Complete(null);
        }
    }
}
 