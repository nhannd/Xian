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
using ClearCanvas.Ris.Application.Common.Admin.ModalityAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
    [MenuAction("launch", "global-menus/Admin/Modality")]
    [ClickHandler("launch", "Launch")]
    [ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.ModalityAdmin)]

    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class ModalitySummaryTool : Tool<IDesktopToolContext>
    {
        private IWorkspace _workspace;

        public void Launch()
        {
            if (_workspace == null)
            {
                try
                {
                    ModalitySummaryComponent component = new ModalitySummaryComponent();

                    _workspace = ApplicationComponent.LaunchAsWorkspace(
                        this.Context.DesktopWindow,
                        component,
                        SR.TitleModalities,
                        delegate(IApplicationComponent c) { _workspace = null; });

                }
                catch (Exception e)
                {
                    // could not launch editor
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
    /// Extension point for views onto <see cref="ModalitySummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ModalitySummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ModalitySummaryComponent class
    /// </summary>
    [AssociateView(typeof(ModalitySummaryComponentViewExtensionPoint))]
    public class ModalitySummaryComponent : ApplicationComponent
    {
        private ModalitySummary _selectedModality;
        private ModalityTable _modalityTable;
        private CrudActionModel _modalityActionHandler;

        private PagingController<ModalitySummary> _pagingController;
        private PagingActionModel<ModalitySummary> _pagingActionHandler;

        /// <summary>
        /// Constructor
        /// </summary>
        public ModalitySummaryComponent()
        {
        }

        public override void Start()
        {
            _modalityTable = new ModalityTable();
            _modalityActionHandler = new CrudActionModel();
            _modalityActionHandler.Add.SetClickHandler(AddModality);
            _modalityActionHandler.Edit.SetClickHandler(UpdateSelectedModality);
            _modalityActionHandler.Add.Enabled = true;
            _modalityActionHandler.Delete.Enabled = false;

            InitialisePaging();
            _modalityActionHandler.Merge(_pagingActionHandler);

            LoadModalityTable();

            base.Start();
        }

        private void InitialisePaging()
        {
            _pagingController = new PagingController<ModalitySummary>(
                delegate(int firstRow, int maxRows)
                {
                    ListAllModalitiesResponse listResponse = null;

                    Platform.GetService<IModalityAdminService>(
                        delegate(IModalityAdminService service)
                        {
                            ListAllModalitiesRequest listRequest = new ListAllModalitiesRequest(false);
                            listRequest.PageRequest.FirstRow = firstRow;
                            listRequest.PageRequest.MaxRows = maxRows;

                            listResponse = service.ListAllModalities(listRequest);
                        });

                    return listResponse.Modalities;
                }
            );

            _pagingActionHandler = new PagingActionModel<ModalitySummary>(_pagingController, _modalityTable, Host.DesktopWindow);
        }

        public override void Stop()
        {
            base.Stop();
        }

        #region Presentation Model

        public ITable Modalities
        {
            get { return _modalityTable; }
        }

        public ActionModelNode ModalityListActionModel
        {
            get { return _modalityActionHandler; }
        }

        public ISelection SelectedModality
        {
            get { return _selectedModality == null ? Selection.Empty : new Selection(_selectedModality); }
            set
            {
                _selectedModality = (ModalitySummary)value.Item;
                ModalitySelectionChanged();
            }
        }

        public void AddModality()
        {
            try
            {
                ModalityEditorComponent editor = new ModalityEditorComponent();
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                    this.Host.DesktopWindow, editor, SR.TitleAddModality);
                if (exitCode == ApplicationComponentExitCode.Accepted)
                {
                    _modalityTable.Items.Add(_selectedModality = editor.ModalitySummary);
                    ModalitySelectionChanged();
                }

            }
            catch (Exception e)
            {
                // could not launch editor
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public void UpdateSelectedModality()
        {
            try
            {
                // can occur if user double clicks while holding control
                if (_selectedModality == null) return;

                ModalityEditorComponent editor = new ModalityEditorComponent(_selectedModality.ModalityRef);
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                    this.Host.DesktopWindow, editor, SR.TitleUpdateModality);
                if (exitCode == ApplicationComponentExitCode.Accepted)
                {
                    _modalityTable.Items.Replace(delegate(ModalitySummary m) { return m.ModalityRef.Equals(editor.ModalitySummary.ModalityRef); }, editor.ModalitySummary);
                }

            }
            catch (Exception e)
            {
                // could not launch editor
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        #endregion

        private void LoadModalityTable()
        {
            _modalityTable.Items.Clear();
            _modalityTable.Items.AddRange(_pagingController.GetFirst());
        }

        private void ModalitySelectionChanged()
        {
            if (_selectedModality != null)
                _modalityActionHandler.Edit.Enabled = true;
            else
                _modalityActionHandler.Edit.Enabled = false;

            this.NotifyPropertyChanged("SelectedModality");
        }
    }
}
