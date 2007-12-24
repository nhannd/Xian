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
using System.Collections.Generic;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Tables;

using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.NoteCategoryAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
    [MenuAction("launch", "global-menus/Admin/Note Categories")]
    [ClickHandler("launch", "Launch")]
    [ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.NoteAdmin)]

    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class NoteCategorySummaryTool : Tool<IDesktopToolContext>
    {
        private IWorkspace _workspace;

        public void Launch()
        {
            if (_workspace == null)
            {
                try
                {
                    NoteCategorySummaryComponent component = new NoteCategorySummaryComponent();

                    _workspace = ApplicationComponent.LaunchAsWorkspace(
                        this.Context.DesktopWindow,
                        component,
                        SR.TitleNoteCategories);
                    _workspace.Closed += delegate { _workspace = null; };

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
    /// Extension point for views onto <see cref="NoteCategorySummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class NoteCategorySummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// NoteCategorySummaryComponent class
    /// </summary>
    [AssociateView(typeof(NoteCategorySummaryComponentViewExtensionPoint))]
    public class NoteCategorySummaryComponent : ApplicationComponent
    {
        private PatientNoteCategorySummary _selectedNoteCategory;
        private NoteCategoryTable _noteCategoryTable;
        private CrudActionModel _noteCategoryActionHandler;

        private PagingController<PatientNoteCategorySummary> _pagingController;
        private PagingActionModel<PatientNoteCategorySummary> _pagingActionHandler;

        /// <summary>
        /// Constructor
        /// </summary>
        public NoteCategorySummaryComponent()
        {
        }

        public override void Start()
        {
            _noteCategoryTable = new NoteCategoryTable();
            _noteCategoryActionHandler = new CrudActionModel();
            _noteCategoryActionHandler.Add.SetClickHandler(AddNoteCategory);
            _noteCategoryActionHandler.Edit.SetClickHandler(UpdateSelectedNoteCategory);
            _noteCategoryActionHandler.Add.Enabled = true;
            _noteCategoryActionHandler.Delete.Enabled = false;

            InitialisePaging();
            _noteCategoryActionHandler.Merge(_pagingActionHandler);

            LoadNoteCategoryTable();

            base.Start();
        }

        private void InitialisePaging()
        {
            _pagingController = new PagingController<PatientNoteCategorySummary>(
                delegate(int firstRow, int maxRows)
                {
                    ListAllNoteCategoriesResponse listResponse = null;

                    Platform.GetService<INoteCategoryAdminService>(
                        delegate(INoteCategoryAdminService service)
                        {
                            ListAllNoteCategoriesRequest listRequest = new ListAllNoteCategoriesRequest();
                            listRequest.Page.FirstRow = firstRow;
                            listRequest.Page.MaxRows = maxRows;

                            listResponse = service.ListAllNoteCategories(listRequest);
                        });

                    return listResponse.NoteCategories;
                }
            );

            _pagingActionHandler = new PagingActionModel<PatientNoteCategorySummary>(_pagingController, _noteCategoryTable, Host.DesktopWindow);
        }

        public override void Stop()
        {
            base.Stop();
        }


        #region Presentation Model

        public ITable NoteCategories
        {
            get { return _noteCategoryTable; }
        }

        public ActionModelNode NoteCategoryListActionModel
        {
            get { return _noteCategoryActionHandler; }
        }

        public ISelection SelectedNoteCategory
        {
            get { return _selectedNoteCategory == null ? Selection.Empty : new Selection(_selectedNoteCategory); }
            set
            {
                _selectedNoteCategory = (PatientNoteCategorySummary)value.Item;
                NoteCategorySelectionChanged();
            }
        }

        public void AddNoteCategory()
        {
            try
            {
                NoteCategoryEditorComponent editor = new NoteCategoryEditorComponent();
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                    this.Host.DesktopWindow, editor, SR.TitleAddNoteCategory);
                if (exitCode == ApplicationComponentExitCode.Accepted)
                {
                    _noteCategoryTable.Items.Add(_selectedNoteCategory = editor.NoteCategorySummary);
                    NoteCategorySelectionChanged();
                }

            }
            catch (Exception e)
            {
                // could not launch editor
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public void UpdateSelectedNoteCategory()
        {
            try
            {
                // can occur if user double clicks while holding control
                if (_selectedNoteCategory == null) return;

                NoteCategoryEditorComponent editor = new NoteCategoryEditorComponent(_selectedNoteCategory.NoteCategoryRef);
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                    this.Host.DesktopWindow, editor, SR.TitleUpdateNoteCategory);
                if (exitCode == ApplicationComponentExitCode.Accepted)
                {
                    _noteCategoryTable.Items.Replace(
                        delegate(PatientNoteCategorySummary s) { return s.NoteCategoryRef.Equals(editor.NoteCategorySummary.NoteCategoryRef, true); },
                        editor.NoteCategorySummary);
                }

            }
            catch (Exception e)
            {
                // could not launch editor
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        private void LoadNoteCategoryTable()
        {
            _noteCategoryTable.Items.Clear();
            _noteCategoryTable.Items.AddRange(_pagingController.GetFirst());
        }

        #endregion

        private void NoteCategorySelectionChanged()
        {
            if (_selectedNoteCategory != null)
                _noteCategoryActionHandler.Edit.Enabled = true;
            else
                _noteCategoryActionHandler.Edit.Enabled = false;

            NotifyPropertyChanged("SelectedNoteCategory");
        }

    }
}
