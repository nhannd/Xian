using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.RequestedProcedureTypeGroupAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
    internal class RequestedProcedureTypeSummaryTable : Table<RequestedProcedureTypeSummary>
    {
        private readonly int columnSortIndex = 0;

        internal RequestedProcedureTypeSummaryTable()
        {
            this.Columns.Add(new TableColumn<RequestedProcedureTypeSummary, int>("ID",
                delegate(RequestedProcedureTypeSummary rpt)
                {
                    int id;
                    return int.TryParse(rpt.Id, out id) ? id : -1 ;
                },
                0.5f));

            this.Columns.Add(new TableColumn<RequestedProcedureTypeSummary, string>("Name",
                delegate(RequestedProcedureTypeSummary rpt) { return rpt.Name; },
                0.5f));

            this.Sort(new TableSortParams(this.Columns[columnSortIndex], true));
        }
    }

    /// <summary>
    /// Extension point for views onto <see cref="RequestedProcedureTypeGroupEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class RequestedProcedureTypeGroupEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// RequestedProcedureTypeGroupEditorComponent class
    /// </summary>
    [AssociateView(typeof(RequestedProcedureTypeGroupEditorComponentViewExtensionPoint))]
    public class RequestedProcedureTypeGroupEditorComponent : ApplicationComponent
    {
        private readonly bool _isNew;

        private EntityRef _editedItemEntityRef;
        private RequestedProcedureTypeGroupDetail _editedItemDetail;
        private RequestedProcedureTypeGroupSummary _editedItemSummary;

        private List<EnumValueInfo> _requestedProcedureTypeGroupCategoryChoices;
        private RequestedProcedureTypeSummaryTable _availableRequestedProcedureTypes;
        private RequestedProcedureTypeSummaryTable _selectedRequestedProcedureTypes;

        public RequestedProcedureTypeGroupEditorComponent()
        {
            _isNew = true;
        }

        public RequestedProcedureTypeGroupEditorComponent(EntityRef entityRef)
        {
            _editedItemEntityRef = entityRef;
            _isNew = false;
        }

        public override void Start()
        {
            _availableRequestedProcedureTypes = new RequestedProcedureTypeSummaryTable();
            _selectedRequestedProcedureTypes = new RequestedProcedureTypeSummaryTable();

            Platform.GetService<IRequestedProcedureTypeGroupAdminService>(
                delegate(IRequestedProcedureTypeGroupAdminService service)
                    {
                        GetRequestedProcedureTypeGroupEditFormDataResponse formDataResponse =
                            service.GetRequestedProcedureTypeGroupEditFormData(new GetRequestedProcedureTypeGroupEditFormDataRequest());
                        _requestedProcedureTypeGroupCategoryChoices = formDataResponse.Categories;
                        _availableRequestedProcedureTypes.Items.AddRange(formDataResponse.RequestedProcedureTypes);

                        if (_isNew)
                        {
                            _editedItemDetail = new RequestedProcedureTypeGroupDetail();
                            _editedItemDetail.Category = _requestedProcedureTypeGroupCategoryChoices[0];
                        }
                        else
                        {
                            LoadRequestedProcedureTypeGroupForEditResponse response =
                                service.LoadRequestedProcedureTypeGroupForEdit(
                                    new LoadRequestedProcedureTypeGroupForEditRequest(_editedItemEntityRef));

                            _editedItemDetail = response.Detail;
                            _selectedRequestedProcedureTypes.Items.AddRange(_editedItemDetail.RequestedProcedureTypes);
                        }

                        foreach(RequestedProcedureTypeSummary selectedSummary in _selectedRequestedProcedureTypes.Items)
                        {
                            _availableRequestedProcedureTypes.Items.Remove(selectedSummary);
                        }
                    });

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public RequestedProcedureTypeGroupSummary EditedRequestedProcedureTypeGroupSummary
        {
            get { return _editedItemSummary; }
        }

        #region Presentation Model

        public string Name
        {
            get { return _editedItemDetail.Name; }
            set
            {
                _editedItemDetail.Name = value;
                this.Modified = true;
            }
        }

        public string Description
        {
            get { return _editedItemDetail.Description; }
            set
            {
                _editedItemDetail.Description = value;
                this.Modified = true;
            }
        }

        #region Category
        public string Category
        {
            get { return _editedItemDetail.Category == null ? string.Empty : _editedItemDetail.Category.Value; }
            set
            {
                _editedItemDetail.Category = EnumValueUtils.MapDisplayValue(_requestedProcedureTypeGroupCategoryChoices, value);
                this.Modified = true;
            }
        }

        public IList<string> CategoryChoices
        {
            get { return EnumValueUtils.GetDisplayValues(_requestedProcedureTypeGroupCategoryChoices); }
        }
        #endregion

        public ITable AvailableRequestedProcedureTypes
        {
            get { return _availableRequestedProcedureTypes; }
        }

        public ITable SelectedRequestedProcedureTypes
        {
            get { return _selectedRequestedProcedureTypes; }
        }

        #endregion

        public void Accept()
        {
            if (this.HasValidationErrors)
            {
                this.ShowValidation(true);
            }
            else
            {
                try
                {
                    _editedItemDetail.RequestedProcedureTypes.Clear();
                    _editedItemDetail.RequestedProcedureTypes.AddRange(_selectedRequestedProcedureTypes.Items);

                    Platform.GetService<IRequestedProcedureTypeGroupAdminService>(
                        delegate(IRequestedProcedureTypeGroupAdminService service)
                        {
                            if (_isNew)
                            {
                                AddRequestedProcedureTypeGroupResponse response = 
                                    service.AddRequestedProcedureTypeGroup(new AddRequestedProcedureTypeGroupRequest(_editedItemDetail));
                                _editedItemEntityRef = response.AddedRequestedProcedureTypeGroupSummary.EntityRef;
                                _editedItemSummary = response.AddedRequestedProcedureTypeGroupSummary;
                            }
                            else
                            {
                                UpdateRequestedProcedureTypeGroupResponse response = 
                                    service.UpdateRequestedProcedureTypeGroup(new UpdateRequestedProcedureTypeGroupRequest(_editedItemEntityRef, _editedItemDetail));
                                _editedItemEntityRef = response.UpdatedRequestedProcedureTypeGroupSummary.EntityRef;
                                _editedItemSummary = response.UpdatedRequestedProcedureTypeGroupSummary;
                            }
                        });

                    this.Host.Exit();
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, SR.ExceptionSaveRequestedProcedureTypeGroup, this.Host.DesktopWindow,
                        delegate()
                        {
                            this.ExitCode = ApplicationComponentExitCode.Error;
                            this.Host.Exit();
                        });
                }
            }
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            Host.Exit();
        }

        public void ItemsAddedOrRemoved()
        {
            this.Modified = true;
        }
    }
}
