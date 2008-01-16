#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.Ris.Application.Common.Admin.ProcedureTypeGroupAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
    /// <summary>
    /// Extension point for views onto <see cref="ProcedureTypeGroupSummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ProcedureTypeGroupSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ProcedureTypeGroupSummaryComponent class
    /// </summary>
    [AssociateView(typeof(ProcedureTypeGroupSummaryComponentViewExtensionPoint))]
    public class ProcedureTypeGroupSummaryComponent : ApplicationComponent
    {
        private ProcedureTypeGroupSummary _selectedProcedureTypeGroup;
        private ProcedureTypeGroupSummaryTable _procedureTypeGroupSummaryTable;

        private SimpleActionModel _procedureTypeGroupActionHandler;
        private readonly string _addProcedureTypeGroupKey = "AddProcedureTypeGroup";
        private readonly string _updateProcedureTypeGroupKey = "UpdateProcedureTypeGroup";

        private IPagingController<ProcedureTypeGroupSummary> _pagingController;

        public override void Start()
        {
            _procedureTypeGroupSummaryTable = new ProcedureTypeGroupSummaryTable();

            _procedureTypeGroupActionHandler = new SimpleActionModel(new ResourceResolver(this.GetType().Assembly));
            _procedureTypeGroupActionHandler.AddAction(_addProcedureTypeGroupKey, SR.TitleAddProcedureTypeGroup, "Icons.AddToolSmall.png", SR.TitleAddProcedureTypeGroup, AddProcedureTypeGroup);
            _procedureTypeGroupActionHandler.AddAction(_updateProcedureTypeGroupKey, SR.TitleUpdateProcedureTypeGroup, "Icons.EditToolSmall.png", SR.TitleUpdateProcedureTypeGroup, UpdateProcedureTypeGroup);
            _procedureTypeGroupActionHandler[_addProcedureTypeGroupKey].Enabled = true;
            _procedureTypeGroupActionHandler[_updateProcedureTypeGroupKey].Enabled = false;

            InitialisePaging(_procedureTypeGroupActionHandler);

            LoadProcedureTypeGroupTable();

            base.Start();
        }

        private void InitialisePaging(ActionModelNode actionModelNode)
        {
            _pagingController = new PagingController<ProcedureTypeGroupSummary>(
                delegate(int firstRow, int maxRows)
                {
                    ListProcedureTypeGroupsResponse listResponse = null;

                    Platform.GetService<IProcedureTypeGroupAdminService>(
                        delegate(IProcedureTypeGroupAdminService service)
                        {
                            ListProcedureTypeGroupsRequest listRequest = new ListProcedureTypeGroupsRequest();
                            listRequest.Page.FirstRow = firstRow;
                            listRequest.Page.MaxRows = maxRows;

                            listResponse = service.ListProcedureTypeGroups(listRequest);
                        });

                    return listResponse.Items;
                }
            );

            if (actionModelNode != null)
            {
                actionModelNode.Merge(new PagingActionModel<ProcedureTypeGroupSummary>(_pagingController, _procedureTypeGroupSummaryTable, Host.DesktopWindow));
            }
        }


        private void LoadProcedureTypeGroupTable()
        {
            _procedureTypeGroupSummaryTable.Items.Clear();
            _procedureTypeGroupSummaryTable.Items.AddRange(_pagingController.GetFirst());
        }

        public override void Stop()
        {
            base.Stop();
        }

        #region Presentation Model

        public ITable ProcedureTypeGroups
        {
            get { return _procedureTypeGroupSummaryTable; }
        }

        public ActionModelNode ProcedureTypeGroupListActionModel
        {
            get { return _procedureTypeGroupActionHandler; }
        }

        public ISelection SelectedProcedureTypeGroup
        {
            get 
            {
                return _selectedProcedureTypeGroup == null
                        ? Selection.Empty
                        : new Selection(_selectedProcedureTypeGroup);  
            }
            set
            {
                _selectedProcedureTypeGroup = (ProcedureTypeGroupSummary) value.Item;
                ProcedureTypeGroupChanged();
            }
        }

        private void ProcedureTypeGroupChanged()
        {
            _procedureTypeGroupActionHandler[_updateProcedureTypeGroupKey].Enabled =
                _selectedProcedureTypeGroup != null;
        }

        #endregion

        #region Action Model Handlers

        private void AddProcedureTypeGroup()
        {
            try
            {
                ProcedureTypeGroupEditorComponent editor = new ProcedureTypeGroupEditorComponent();
                ApplicationComponentExitCode exitCode =  ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleAddProcedureTypeGroup);

                if (exitCode == ApplicationComponentExitCode.Accepted)
                {
                    LoadProcedureTypeGroupTable();
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }
        
        public void UpdateProcedureTypeGroup()
        {
            try
            {
                if (_selectedProcedureTypeGroup == null) return;

                ProcedureTypeGroupEditorComponent editor = new ProcedureTypeGroupEditorComponent(_selectedProcedureTypeGroup.EntityRef);
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleUpdateProcedureTypeGroup);

                if (exitCode == ApplicationComponentExitCode.Accepted)
                {
                    LoadProcedureTypeGroupTable();
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
