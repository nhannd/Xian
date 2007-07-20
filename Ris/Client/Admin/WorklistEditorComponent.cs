using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
    /// <summary>
    /// Extension point for views onto <see cref="WorklistEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class WorklistEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// WorklistEditorComponent class
    /// </summary>
    [AssociateView(typeof(WorklistEditorComponentViewExtensionPoint))]
    public class WorklistEditorComponent : ApplicationComponent
    {
        private readonly bool _isNew;

        private EntityRef _editedItemEntityRef;
        private WorklistSummary _editedItemSummary;
        private WorklistDetail _editedItemDetail;

        private List<string> _typeChoices;
        private RequestedProcedureTypeGroupSummaryTable _availableRequestedProcedureTypeGroups;
        private RequestedProcedureTypeGroupSummaryTable _selectedRequestedProcedureTypeGroups;
        private RequestedProcedureTypeGroupSummary _selectedRequestedProcedureTypeGroupSelection;
        private RequestedProcedureTypeGroupSummary _availableRequestedProcedureTypeGroupSelection;

        private event EventHandler _requestedProcedureTypeGroupSummaryTablesChanged;

        /// <summary>
        /// Constructor
        /// </summary>
        public WorklistEditorComponent()
        {
            _isNew = true;
        }

        public WorklistEditorComponent(EntityRef entityRef)
        {
            _isNew = false;
            _editedItemEntityRef = entityRef;
        }

        public override void Start()
        {
            _availableRequestedProcedureTypeGroups = new RequestedProcedureTypeGroupSummaryTable();
            _selectedRequestedProcedureTypeGroups = new RequestedProcedureTypeGroupSummaryTable();

            Platform.GetService<IWorklistAdminService>(
                delegate(IWorklistAdminService service)
                {
                    GetWorklistEditFormDataResponse formDataResponse =
                        service.GetWorklistEditFormData(new GetWorklistEditFormDataRequest());
                    _typeChoices = formDataResponse.WorklistTypes;
                    _availableRequestedProcedureTypeGroups.Items.AddRange(formDataResponse.RequestedProcedureTypeGroups);

                    if(_isNew)
                    {
                        _editedItemDetail = new WorklistDetail();
                        _editedItemDetail.WorklistType = _typeChoices[0];
                    }
                    else
                    {
                        LoadWorklistForEditResponse response =
                            service.LoadWorklistForEdit(new LoadWorklistForEditRequest(_editedItemEntityRef));

                        _editedItemDetail = response.WorklistDetail;
                        _selectedRequestedProcedureTypeGroups.Items.AddRange(_editedItemDetail.RequestedProcedureTypeGroups);
                    }

                    foreach (RequestedProcedureTypeGroupSummary selectedSummary in _selectedRequestedProcedureTypeGroups.Items)
                    {
                        _availableRequestedProcedureTypeGroups.Items.Remove(selectedSummary);
                    }

                });

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public WorklistSummary EditedWorklistSummary
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
            get { return ""; }
            set
            {
                _editedItemDetail.Description = value;
                this.Modified = true;
            }
        }

        #region Type
        public string Type
        {
            get { return _editedItemDetail.WorklistType; }
            set
            {
                _editedItemDetail.WorklistType = value;
                this.Modified = true;
            }
        }

        public IList<string> TypeChoices
        {
            get { return _typeChoices; }
        }

        public bool TypeEnabled
        {
            get { return _isNew; }
        }
        #endregion

        public ITable AvailableRequestedProcedureTypeGroups
        {
            get { return _availableRequestedProcedureTypeGroups; }
        }

        public ITable SelectedRequestedProcedureTypeGroups
        {
            get { return _selectedRequestedProcedureTypeGroups; }
        }

        public ISelection SelectedRequestedProcedureTypeGroupsSelection
        {
            get { return _selectedRequestedProcedureTypeGroupSelection == null ? Selection.Empty : new Selection(_selectedRequestedProcedureTypeGroupSelection); }
            set
            {
                _selectedRequestedProcedureTypeGroupSelection = (RequestedProcedureTypeGroupSummary)value.Item;
            }
        }

        public ISelection AvailableRequestedProcedureTypeGroupsSelection
        {
            get { return _availableRequestedProcedureTypeGroupSelection == null ? Selection.Empty : new Selection(_availableRequestedProcedureTypeGroupSelection); }
            set
            {
                _availableRequestedProcedureTypeGroupSelection = (RequestedProcedureTypeGroupSummary)value.Item;
            }
        }

        public event EventHandler RequestedProcedureTypeGroupTablesChanged
        {
            add { _requestedProcedureTypeGroupSummaryTablesChanged += value; }
            remove { _requestedProcedureTypeGroupSummaryTablesChanged -= value; }
        }

        #endregion

        public bool AcceptEnabled
        {
            get { return this.Modified; }
        }

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
                    _editedItemDetail.RequestedProcedureTypeGroups.Clear();
                    _editedItemDetail.RequestedProcedureTypeGroups.AddRange(_selectedRequestedProcedureTypeGroups.Items);

                    Platform.GetService<IWorklistAdminService>(
                        delegate(IWorklistAdminService service)
                        {
                            if (_isNew)
                            {
                                AddWorklistResponse response =
                                    service.AddWorklist(new AddWorklistRequest(_editedItemDetail));
                                _editedItemEntityRef = response.AddedWorklistSummary.EntityRef;
                                _editedItemSummary = response.AddedWorklistSummary;
                            }
                            else
                            {
                                UpdateWorklistResponse response =
                                    service.UpdateWorklist(new UpdateWorklistRequest(_editedItemEntityRef, _editedItemDetail));
                                _editedItemEntityRef = response.WorklistSummary.EntityRef;
                                _editedItemSummary = response.WorklistSummary;
                            }
                        });

                    this.Host.Exit();
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, SR.ExceptionSaveWorklist, this.Host.DesktopWindow,
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

        public bool AddSelectionEnabled
        {
            get { return _availableRequestedProcedureTypeGroupSelection != null; }
        }

        public void AddSelection(ISelection selection)
        {
            foreach (RequestedProcedureTypeGroupSummary summary in selection.Items)
            {
                _selectedRequestedProcedureTypeGroups.Items.Add(summary);
                _availableRequestedProcedureTypeGroups.Items.Remove(summary);
            }
            EventsHelper.Fire(_requestedProcedureTypeGroupSummaryTablesChanged, this, EventArgs.Empty);
            this.Modified = true;
        }

        public bool RemoveSelectionEnabled
        {
            get { return _selectedRequestedProcedureTypeGroupSelection != null; }
        }

        public void RemoveSelection(ISelection selection)
        {
            foreach (RequestedProcedureTypeGroupSummary summary in selection.Items)
            {
                _selectedRequestedProcedureTypeGroups.Items.Remove(summary);
                _availableRequestedProcedureTypeGroups.Items.Add(summary);
            }
            EventsHelper.Fire(_requestedProcedureTypeGroupSummaryTablesChanged, this, EventArgs.Empty);
            this.Modified = true;
        }
    }
}
