using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
    /// <summary>
    /// Extension point for views onto <see cref="WorklistSummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class WorklistSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// WorklistSummaryComponent class
    /// </summary>
    [AssociateView(typeof(WorklistSummaryComponentViewExtensionPoint))]
    public class WorklistSummaryComponent : ApplicationComponent
    {
        private WorklistAdminSummary _selectedWorklist;
        private WorklistAdminSummaryTable _worklistAdminSummaryTable;

        private SimpleActionModel _worklistActionHandler;
        private readonly string _addWorklistKey = "AddWorklist";
        private readonly string _updateWorklistKey = "UpdateWorklist";

        private IPagingController<WorklistAdminSummary> _pagingController;

        public override void Start()
        {
            _worklistAdminSummaryTable = new WorklistAdminSummaryTable();

            _worklistActionHandler = new SimpleActionModel(new ResourceResolver(this.GetType().Assembly));
            _worklistActionHandler.AddAction(_addWorklistKey, SR.TitleAddWorklist, "Icons.AddToolSmall.png", SR.TitleAddWorklist, AddWorklist);
            _worklistActionHandler.AddAction(_updateWorklistKey, SR.TitleUpdateWorklist, "Icons.EditToolSmall.png", SR.TitleUpdateWorklist, UpdateWorklist);
            _worklistActionHandler[_addWorklistKey].Enabled = true;
            _worklistActionHandler[_updateWorklistKey].Enabled = false;

            InitialisePaging(_worklistActionHandler);

            LoadWorklistTable();

            base.Start();
        }

        private void InitialisePaging(ActionModelNode actionModelNode)
        {
            _pagingController = new PagingController<WorklistAdminSummary>(
                delegate(int firstRow, int maxRows)
                {
                    ListWorklistsResponse listResponse = null;

                    Platform.GetService<IWorklistAdminService>(
                        delegate(IWorklistAdminService service)
                        {
                            ListWorklistsRequest listRequest = new ListWorklistsRequest();
                            listRequest.PageRequest.FirstRow = firstRow;
                            listRequest.PageRequest.MaxRows = maxRows;

                            listResponse = service.ListWorklists(listRequest);
                        });

                    return listResponse.WorklistSummaries;
                }
            );

            if (actionModelNode != null)
            {
                actionModelNode.Merge(new PagingActionModel<WorklistAdminSummary>(_pagingController, _worklistAdminSummaryTable, Host.DesktopWindow));
            }
        }

        private void LoadWorklistTable()
        {
            _worklistAdminSummaryTable.Items.Clear();
            _worklistAdminSummaryTable.Items.AddRange(_pagingController.GetFirst());
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        #region Presentation Model

        public ITable Worklists
        {
            get { return _worklistAdminSummaryTable; }
        }

        public ActionModelNode WorklistActionModel
        {
            get { return _worklistActionHandler; }
        }

        public ISelection SelectedWorklist
        {
            get
            {
                return _selectedWorklist == null
                    ? Selection.Empty
                    : new Selection(_selectedWorklist);
            }
            set
            {
                _selectedWorklist = (WorklistAdminSummary) value.Item;
                SelectedWorklistChanged();
            }
        }

        private void SelectedWorklistChanged()
        {
            _worklistActionHandler[_updateWorklistKey].Enabled = 
                _selectedWorklist != null;
        }

        #endregion

        #region Action Model Handlers

        private void AddWorklist()
        {
            try
            {
                WorklistEditorComponent editor = new WorklistEditorComponent();
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleAddWorklist);

                if (exitCode == ApplicationComponentExitCode.Normal)
                {
                    LoadWorklistTable();
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public void UpdateWorklist()
        {
            try
            {
                if (_selectedWorklist == null) return;

                WorklistEditorComponent editor = new WorklistEditorComponent(_selectedWorklist.EntityRef);
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleUpdateWorklist);

                if (exitCode == ApplicationComponentExitCode.Normal)
                {
                    LoadWorklistTable();
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        #endregion
    }
}
