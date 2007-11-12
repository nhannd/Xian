#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
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

                if (exitCode == ApplicationComponentExitCode.Accepted)
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

                if (exitCode == ApplicationComponentExitCode.Accepted)
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
