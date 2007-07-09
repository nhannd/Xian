using System;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.Admin.RequestedProcedureTypeGroupAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
    /// <summary>
    /// Extension point for views onto <see cref="RequestedProcedureTypeGroupSummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class RequestedProcedureTypeGroupSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// RequestedProcedureTypeGroupSummaryComponent class
    /// </summary>
    [AssociateView(typeof(RequestedProcedureTypeGroupSummaryComponentViewExtensionPoint))]
    public class RequestedProcedureTypeGroupSummaryComponent : ApplicationComponent
    {
        private RequestedProcedureTypeGroupSummary _selectedRequestedProcedureTypeGroup;
        private RequestedProcedureTypeGroupSummaryTable _requestedProcedureTypeGroupSummaryTable;

        private SimpleActionModel _requestedProcedureTypeGroupActionHandler;
        private readonly string _addRequestedProcedureTypeGroupKey = "AddRequestedProcedureTypeGroup";
        private readonly string _updateRequestedProcedureTypeGroupKey = "UpdateRequestedProcedureTypeGroup";

        private IPagingController<RequestedProcedureTypeGroupSummary> _pagingController;

        public override void Start()
        {
            _requestedProcedureTypeGroupSummaryTable = new RequestedProcedureTypeGroupSummaryTable();

            _requestedProcedureTypeGroupActionHandler = new SimpleActionModel(new ResourceResolver(this.GetType().Assembly));
            _requestedProcedureTypeGroupActionHandler.AddAction(_addRequestedProcedureTypeGroupKey, SR.TitleAddRequestedProcedureTypeGroup, "Icons.AddToolSmall.png", SR.TitleAddRequestedProcedureTypeGroup, AddRequestedProcedureTypeGroup);
            _requestedProcedureTypeGroupActionHandler.AddAction(_updateRequestedProcedureTypeGroupKey, SR.TitleUpdateRequestedProcedureTypeGroup, "Icons.EditToolSmall.png", SR.TitleUpdateRequestedProcedureTypeGroup, UpdateRequestedProcedureTypeGroup);
            _requestedProcedureTypeGroupActionHandler[_addRequestedProcedureTypeGroupKey].Enabled = true;
            _requestedProcedureTypeGroupActionHandler[_updateRequestedProcedureTypeGroupKey].Enabled = false;

            InitialisePaging(_requestedProcedureTypeGroupActionHandler);

            LoadRequestedProcedureTypeGroupTable();

            base.Start();
        }

        private void InitialisePaging(ActionModelNode actionModelNode)
        {
            _pagingController = new PagingController<RequestedProcedureTypeGroupSummary>(
                delegate(int firstRow, int maxRows)
                {
                    ListRequestedProcedureTypeGroupsResponse listResponse = null;

                    Platform.GetService<IRequestedProcedureTypeGroupAdminService>(
                        delegate(IRequestedProcedureTypeGroupAdminService service)
                        {
                            ListRequestedProcedureTypeGroupsRequest listRequest = new ListRequestedProcedureTypeGroupsRequest();
                            listRequest.PageRequest.FirstRow = firstRow;
                            listRequest.PageRequest.MaxRows = maxRows;

                            listResponse = service.ListRequestedProcedureTypeGroups(listRequest);
                        });

                    return listResponse.Items;
                }
            );

            if (actionModelNode != null)
            {
                actionModelNode.Merge(new PagingActionModel<RequestedProcedureTypeGroupSummary>(_pagingController, _requestedProcedureTypeGroupSummaryTable, Host.DesktopWindow));
            }
        }


        private void LoadRequestedProcedureTypeGroupTable()
        {
            _requestedProcedureTypeGroupSummaryTable.Items.Clear();
            _requestedProcedureTypeGroupSummaryTable.Items.AddRange(_pagingController.GetFirst());
        }

        public override void Stop()
        {
            base.Stop();
        }

        #region Presentation Model

        public ITable RequestedProcedureTypeGroups
        {
            get { return _requestedProcedureTypeGroupSummaryTable; }
        }

        public ActionModelNode RequestedProcedureTypeGroupListActionModel
        {
            get { return _requestedProcedureTypeGroupActionHandler; }
        }

        public ISelection SelectedRequestedProcedureTypeGroup
        {
            get 
            {
                return _selectedRequestedProcedureTypeGroup == null
                        ? Selection.Empty
                        : new Selection(_selectedRequestedProcedureTypeGroup);  
            }
            set
            {
                _selectedRequestedProcedureTypeGroup = (RequestedProcedureTypeGroupSummary) value.Item;
                RequestedProcedureTypeGroupChanged();
            }
        }

        private void RequestedProcedureTypeGroupChanged()
        {
            _requestedProcedureTypeGroupActionHandler[_updateRequestedProcedureTypeGroupKey].Enabled =
                _selectedRequestedProcedureTypeGroup != null;
        }

        #endregion

        #region Action Model Handlers

        private void AddRequestedProcedureTypeGroup()
        {
            try
            {
                RequestedProcedureTypeGroupEditorComponent editor = new RequestedProcedureTypeGroupEditorComponent();
                ApplicationComponentExitCode exitCode =  ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleAddRequestedProcedureTypeGroup);

                if (exitCode == ApplicationComponentExitCode.Normal)
                {
                    LoadRequestedProcedureTypeGroupTable();
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }
        
        public void UpdateRequestedProcedureTypeGroup()
        {
            try
            {
                if (_selectedRequestedProcedureTypeGroup == null) return;

                RequestedProcedureTypeGroupEditorComponent editor = new RequestedProcedureTypeGroupEditorComponent(_selectedRequestedProcedureTypeGroup.EntityRef);
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleUpdateRequestedProcedureTypeGroup);

                if (exitCode == ApplicationComponentExitCode.Normal)
                {
                    LoadRequestedProcedureTypeGroupTable();
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
