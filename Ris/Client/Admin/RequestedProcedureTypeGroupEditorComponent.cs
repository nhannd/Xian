using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin.RequestedProcedureTypeGroupAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
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

        private List<RequestedProcedureTypeSummary> _requestedProcedureTypeChoices;

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
            Platform.GetService<IRequestedProcedureTypeGroupAdminService>(
                delegate(IRequestedProcedureTypeGroupAdminService service)
                    {
                        GetRequestedProcedureTypeGroupEditFormDataResponse formDataResponse =
                            service.GetRequestedProcedureTypeGroupEditFormData(new GetRequestedProcedureTypeGroupEditFormDataRequest());
                        _requestedProcedureTypeChoices = formDataResponse.RequestedProcedureTypes;

                        if (_isNew)
                        {
                            _editedItemDetail = new RequestedProcedureTypeGroupDetail();
                        }
                        else
                        {
                            LoadRequestedProcedureTypeGroupForEditResponse response =
                                service.LoadRequestedProcedureTypeGroupForEdit(
                                    new LoadRequestedProcedureTypeGroupForEditRequest(_editedItemEntityRef));

                            _editedItemDetail = response.Detail;
                        }
                    });

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        #region Presentation Model

        public string Foo
        {
            get { return _editedItemDetail.Foo; }
            set 
            { 
                _editedItemDetail.Foo = value;
                this.Modified = true;
            }
        }

        #endregion

        public RequestedProcedureTypeGroupSummary EditedRequestedProcedureTypeGroupSummary
        {
            get { return _editedItemSummary; }
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

        public bool AcceptEnabled
        {
            get { return this.Modified; }
        }

        public event EventHandler AcceptEnabledChanged
        {
            add { this.ModifiedChanged += value; }
            remove { this.ModifiedChanged -= value; }
        }

    }
}
