using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.Import;
using System.ServiceModel;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.Ris.Client.Admin
{
    [MenuAction("launch", "global-menus/Admin/Import Data")]
    [ClickHandler("launch", "Launch")]

    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class DataImportTool : Tool<IDesktopToolContext>
    {
        private IWorkspace _workspace;

        public void Launch()
        {
            if (_workspace == null)
            {
                try
                {
                    DataImportComponent component = new DataImportComponent();

                    _workspace = ApplicationComponent.LaunchAsWorkspace(
                        this.Context.DesktopWindow,
                        component,
                        SR.TitleImportData,
                        delegate(IApplicationComponent c) { _workspace = null; });

                }
                catch (Exception e)
                {
                    // failed to launch component
                    ExceptionHandler.Report(e, this.Context.DesktopWindow);
                }
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
    public class DataImportComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ImportDiagnosticServicesComponent class
    /// </summary>
    [AssociateView(typeof(DataImportComponentViewExtensionPoint))]
    public class DataImportComponent : ApplicationComponent
    {
        private string _filename;
        private int _batchSize;

        private string[] _importTypeChoices;
        private string _importType;

        /// <summary>
        /// Constructor
        /// </summary>
        public DataImportComponent()
        {
            // set default size
            _filename = "";
            _batchSize = 10;
        }

        public override void Start()
        {
            Platform.GetService<IImportService>(
                delegate(IImportService service)
                {
                    ListImportersResponse response = service.ListImporters(new ListImportersRequest());
                    response.Importers.Add(null);
                    _importTypeChoices = response.Importers.ToArray();
                });

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        #region Presentation Model

        [ValidateNotNull]
        public string FileName
        {
            get { return _filename; }
            set { _filename = value; }
        }

        [ValidateGreaterThan(0)]
        [ValidateLessThan(40, Inclusive=true)]
        public int BatchSize
        {
            get { return _batchSize; }
            set { _batchSize = value; }
        }

        [ValidateNotNull]
        public string ImportType
        {
            get { return _importType; }
            set { _importType = value; }
        }

        public string[] ImportTypeChoices
        {
            get { return _importTypeChoices; }
        }

        public void StartImport()
        {
            // validate input
            if (this.HasValidationErrors)
            {
                this.ShowValidation(true);
                return;
            }

            // create a background task to import diagnostic services
            BackgroundTask task = new BackgroundTask(
                delegate(IBackgroundTaskContext context)
                {
                    DoImport(context);
                }, true);

            try
            {
                // run task and block
                ProgressDialog.Show(task, this.Host.DesktopWindow);
            }
            catch (Exception e)
            {
                // we know that the background task has wrapped up an inner exception, so extract it
                ExceptionHandler.Report(e, "Import failed", this.Host.DesktopWindow);
            }
        }

        #endregion

        /// <summary>
        /// Validates that the file exists, and returns the total line count in the file.
        /// </summary>
        /// <returns></returns>
        private int PrescanFile()
        {
            try
            {
                int lineCount = 0;
                using (StreamReader reader = File.OpenText(_filename))
                {
                    string line = null;
                    while ((line = reader.ReadLine()) != null)
                        lineCount++;
                }
                return lineCount;
            }
            catch (Exception e)
            {
                throw new Exception("Invalid data file: " + e.Message);
            }
        }

        private delegate bool WithBatchCallback(List<string> lines);

        private bool ReadBatches(WithBatchCallback callback)
        {
            using (StreamReader reader = File.OpenText(_filename))
            {
                string line = null;
                List<string> lines = new List<string>();

                while ((line = reader.ReadLine()) != null)
                {
                    if(!string.IsNullOrEmpty(line))
                        lines.Add(line);

                    if (lines.Count == _batchSize)
                    {
                        // send batch
                        bool next = callback(lines);
                        if (!next)
                            return false;
                        lines.Clear();
                    }
                }

                if (lines.Count > 0)
                {
                    // send final batch
                    return callback(lines);
                }
                return true;
            }
        }

        private void UpdateProgress(IBackgroundTaskContext context, string status, int batch, int lineCount)
        {
            int importedRows = Math.Min(batch * _batchSize, lineCount);
            float percentage = (lineCount == 0) ? 0 : 100 * ((float)importedRows) / (float)lineCount;
            string message = string.Format("{0} - Imported {1} rows.", status, importedRows);
            context.ReportProgress(new BackgroundTaskProgress((int)percentage, message));
        }

        private void DoImport(IBackgroundTaskContext context)
        {
            int lineCount = 0;
            int batch = 0;
            try
            {
                lineCount = PrescanFile();
                bool complete = ReadBatches(
                    delegate(List<string> rows)
                    {
                        if (context.CancelRequested)
                            return false;

                        batch++;
                        try
                        {
                            Platform.GetService<IImportService>(
                                delegate(IImportService service)
                                {
                                    service.ImportData(new ImportDataRequest(_importType, rows));
                                });
                        }
                        catch (FaultException<ImportException> e)
                        {
                            // handle import exceptions so that we can add information about the 
                            // row where the exception occured to the error message
                            if (e.Detail.DataRow > -1)
                            {
                                string message = string.Format("Error importing row {0}: {1}",
                                    batch * _batchSize + e.Detail.DataRow,
                                    e.Detail.Message);

                                throw new Exception(message);
                            }
                            else
                                throw e.Detail;
                        }

                        UpdateProgress(context, "Importing", batch, lineCount);
                        
                        return true;    // get next batch
                    });

                if (complete)
                {
                    UpdateProgress(context, "Completed", batch, lineCount);
                    context.Complete(null);
                }
                else
                {
                    UpdateProgress(context, "Cancelled", batch, lineCount);
                    context.Cancel();
                }
            }
            catch (Exception e)
            {
                UpdateProgress(context, "Error", batch, lineCount);
                context.Error(e);
            }
        }
    }
}
 
