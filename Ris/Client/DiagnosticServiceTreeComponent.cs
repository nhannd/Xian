using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Trees;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="DiagnosticServiceTreeComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class DiagnosticServiceTreeComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// DiagnosticServiceTreeComponent class
    /// </summary>
    [AssociateView(typeof(DiagnosticServiceTreeComponentViewExtensionPoint))]
    public class DiagnosticServiceTreeComponent : ApplicationComponent
    {
        private Tree<DiagnosticServiceTreeItem> _diagnosticServiceTree;
        private DiagnosticServiceTreeItem _selectedDiagnosticServiceTreeItem;
        private DiagnosticServiceDetail _selectedDiagnosticServiceDetail;

        private Table<RequestedProcedureTypeDetail> _diagnosticServiceBreakdown;
        private bool _acceptEnabled;
        private event EventHandler _selectedDiagnosticServiceTreeItemChanged;

        /// <summary>
        /// Constructor
        /// </summary>
        public DiagnosticServiceTreeComponent()
        {
            _diagnosticServiceBreakdown = new Table<RequestedProcedureTypeDetail>();
            _diagnosticServiceBreakdown.Columns.Add(
                new TableColumn<RequestedProcedureTypeDetail, string>("Procedure Name",
                           delegate(RequestedProcedureTypeDetail rp) { return rp.Name; }));
        }

        public DiagnosticServiceDetail SelectedDiagnosticServiceDetail
        {
            get { return _selectedDiagnosticServiceDetail; }
        }

        public override void Start()
        {
            Platform.GetService<IOrderEntryService>(
                delegate(IOrderEntryService service)
                {
                    _diagnosticServiceTree = ExpandDiagnosticServiceTree(null);
                });


            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        #region Presentation Model

        public ITree DiagnosticServiceTree
        {
            get { return _diagnosticServiceTree; }
        }

        public ISelection SelectedDiagnosticServiceTreeItem
        {
            get { return new Selection(_selectedDiagnosticServiceTreeItem); }
            set
            {
                _selectedDiagnosticServiceTreeItem = value.Item as DiagnosticServiceTreeItem;
                UpdateDiagnosticServiceBreakdown();
            }
        }

        public ITable DiagnosticServiceBreakdown
        {
            get { return _diagnosticServiceBreakdown; }
        }

        public bool AcceptEnabled
        {
            get { return _selectedDiagnosticServiceDetail != null; }
        }

        public void Accept()
        {
            this.Exit(ApplicationComponentExitCode.Accepted);
        }

        public void Cancel()
        {
            this.Exit(ApplicationComponentExitCode.None);
        }

        #endregion


        private Tree<DiagnosticServiceTreeItem> ExpandDiagnosticServiceTree(DiagnosticServiceTreeItem item)
        {
            Tree<DiagnosticServiceTreeItem> subtree = null;

            try
            {
                Platform.GetService<IOrderEntryService>(
                    delegate(IOrderEntryService service)
                    {
                        EntityRef nodeRef = item == null ? null : item.NodeRef;
                        GetDiagnosticServiceSubTreeResponse response = service.GetDiagnosticServiceSubTree(new GetDiagnosticServiceSubTreeRequest(nodeRef));

                        TreeItemBinding<DiagnosticServiceTreeItem> binding = new TreeItemBinding<DiagnosticServiceTreeItem>(
                                delegate(DiagnosticServiceTreeItem ds) { return ds.Description; },
                                ExpandDiagnosticServiceTree);
                        binding.CanHaveSubTreeHandler = delegate(DiagnosticServiceTreeItem ds) { return ds.DiagnosticService == null; };
                        subtree = new Tree<DiagnosticServiceTreeItem>(binding, response.DiagnosticServiceSubTree);
                    });
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, SR.ExceptionCannotExpandDiagnositicServiceTree, this.Host.DesktopWindow,
                    delegate
                    {
                        this.ExitCode = ApplicationComponentExitCode.Error;
                        this.Host.Exit();
                    });
            }

            return subtree;
        }

        private void UpdateDiagnosticServiceBreakdown()
        {
            _selectedDiagnosticServiceDetail = null;
            _diagnosticServiceBreakdown.Items.Clear();


            if (_selectedDiagnosticServiceTreeItem != null && _selectedDiagnosticServiceTreeItem.DiagnosticService != null)
            {
                try
                {
                    Platform.GetService<IOrderEntryService>(
                        delegate(IOrderEntryService service)
                        {
                            LoadDiagnosticServiceBreakdownResponse response = service.LoadDiagnosticServiceBreakdown(new LoadDiagnosticServiceBreakdownRequest(_selectedDiagnosticServiceTreeItem.DiagnosticService.DiagnosticServiceRef));
                            _selectedDiagnosticServiceDetail = response.DiagnosticServiceDetail;

                            _diagnosticServiceBreakdown.Items.AddRange(_selectedDiagnosticServiceDetail.RequestedProcedureTypes);
                        });
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, SR.ExceptionCannotUpdateDiagnosticServiceBreakdown,
                                            this.Host.DesktopWindow);
                }
            }

            NotifyPropertyChanged("AcceptEnabled");
        }

    }
}
