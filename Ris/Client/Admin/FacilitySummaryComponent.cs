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
using System.Collections.Generic;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.FacilityAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
    [MenuAction("launch", "global-menus/Admin/Facilities")]
    [ClickHandler("launch", "Launch")]
    [ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.FacilityAdmin)]

    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class FacilitySummaryTool : Tool<IDesktopToolContext>
    {
        private IWorkspace _workspace;

        public void Launch()
        {
            if (_workspace == null)
            {
                try
                {
                    FacilitySummaryComponent component = new FacilitySummaryComponent();

                    _workspace = ApplicationComponent.LaunchAsWorkspace(
                        this.Context.DesktopWindow,
                        component,
                        SR.TitleFacilities,
                        delegate(IApplicationComponent c) { _workspace = null; });

                }
                catch (Exception e)
                {
                    // could not launch component
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
    /// Extension point for views onto <see cref="FacilitySummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class FacilitySummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// FacilitySummaryComponent class
    /// </summary>
    [AssociateView(typeof(FacilitySummaryComponentViewExtensionPoint))]
    public class FacilitySummaryComponent : ApplicationComponent
    {
        private FacilitySummary _selectedFacility;
        private FacilityTable _facilityTable;
        private CrudActionModel _facilityActionHandler;

        private PagingController<FacilitySummary> _pagingController;
        private PagingActionModel<FacilitySummary> _pagingActionHandler;

        /// <summary>
        /// Constructor
        /// </summary>
        public FacilitySummaryComponent()
        {
        }

        public override void Start()
        {
            _facilityTable = new FacilityTable();
            _facilityActionHandler = new CrudActionModel();
            _facilityActionHandler.Add.SetClickHandler(AddFacility);
            _facilityActionHandler.Edit.SetClickHandler(UpdateSelectedFacility);
            _facilityActionHandler.Add.Enabled = true;
            _facilityActionHandler.Delete.Enabled = false;

            InitialisePaging();
            _facilityActionHandler.Merge(_pagingActionHandler);

            LoadFacilityTable();

            base.Start();
        }

        private void InitialisePaging()
        {
            _pagingController = new PagingController<FacilitySummary>(
                delegate(int firstRow, int maxRows)
                {
                    ListAllFacilitiesResponse listResponse = null;

                    Platform.GetService<IFacilityAdminService>(
                        delegate(IFacilityAdminService service)
                        {
                            ListAllFacilitiesRequest listRequest = new ListAllFacilitiesRequest();
                            listRequest.PageRequest.FirstRow = firstRow;
                            listRequest.PageRequest.MaxRows = maxRows;

                            listResponse = service.ListAllFacilities(listRequest);
                        });

                    return listResponse.Facilities;
                }
            );

            _pagingActionHandler = new PagingActionModel<FacilitySummary>(_pagingController, _facilityTable, Host.DesktopWindow);
        }

        public override void Stop()
        {
            base.Stop();
        }


        #region Presentation Model

        public ITable Facilities
        {
            get { return _facilityTable; }
        }

        public ActionModelNode FacilityListActionModel
        {
            get { return _facilityActionHandler; }
        }

        public ISelection SelectedFacility
        {
            get { return _selectedFacility == null ? Selection.Empty : new Selection(_selectedFacility); }
            set
            {
                _selectedFacility = (FacilitySummary)value.Item;
                FacilitySelectionChanged();
            }
        }

        public void AddFacility()
        {
            try
            {
                FacilityEditorComponent editor = new FacilityEditorComponent();
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                    this.Host.DesktopWindow, editor, SR.TitleAddFacility);
                if (exitCode == ApplicationComponentExitCode.Accepted)
                {
                    _facilityTable.Items.Add(_selectedFacility = editor.FacilitySummary);
                    FacilitySelectionChanged();
                }

            }
            catch (Exception e)
            {
                // could not launch editor
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public void UpdateSelectedFacility()
        {
            try
            {
                // can occur if user double clicks while holding control
                if (_selectedFacility == null) return;

                FacilityEditorComponent editor = new FacilityEditorComponent(_selectedFacility.FacilityRef);
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                    this.Host.DesktopWindow, editor, SR.TitleUpdateFacility);
                if (exitCode == ApplicationComponentExitCode.Accepted)
                {
                    _facilityTable.Items.Replace(delegate(FacilitySummary f) { return f.FacilityRef.Equals(editor.FacilitySummary.FacilityRef); }, editor.FacilitySummary);
                }
            }
            catch (Exception e)
            {
                // could not launch editor
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        #endregion

        private void LoadFacilityTable()
        {
            _facilityTable.Items.Clear();
            _facilityTable.Items.AddRange(_pagingController.GetFirst());
        }

        private void FacilitySelectionChanged()
        {
            if (_selectedFacility != null)
                _facilityActionHandler.Edit.Enabled = true;
            else
                _facilityActionHandler.Edit.Enabled = false;

            NotifyPropertyChanged("SelectedFacility");
        }
    }
}
