using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Tables;

using ClearCanvas.Enterprise;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.NoteCategoryAdmin;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Admin
{
    [MenuAction("launch", "global-menus/Admin/NoteCategories")]
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
                NoteCategorySummaryComponent component = new NoteCategorySummaryComponent();

                _workspace = ApplicationComponent.LaunchAsWorkspace(
                    this.Context.DesktopWindow,
                    component,
                    SR.TitleNoteCategories,
                    delegate(IApplicationComponent c) { _workspace = null; });
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
        private NoteCategorySummary _selectedNoteCategory;
        private NoteCategoryTable _noteCategoryTable;
        private CrudActionModel _noteCategoryActionHandler;

        /// <summary>
        /// Constructor
        /// </summary>
        public NoteCategorySummaryComponent()
        {
        }

        public override void Start()
        {
            //_noteCategoryAdminService.NoteCategoryChanged += NoteCategoryChangedEventHandler;

            _noteCategoryTable = new NoteCategoryTable();
            _noteCategoryActionHandler = new CrudActionModel();
            _noteCategoryActionHandler.Add.SetClickHandler(AddNoteCategory);
            _noteCategoryActionHandler.Edit.SetClickHandler(UpdateSelectedNoteCategory);
            _noteCategoryActionHandler.Add.Enabled = true;
            _noteCategoryActionHandler.Delete.Enabled = false;

            base.Start();
        }

        public override void Stop()
        {
            //_noteCategoryAdminService.NoteCategoryChanged -= NoteCategoryChangedEventHandler;
            
            base.Stop();
        }

        //TODO: NoteCategoryChangedEventHandler
        //private void NoteCategoryChangedEventHandler(object sender, EntityChangeEventArgs e)
        //{
        //}

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
                _selectedNoteCategory = (NoteCategorySummary)value.Item;
                NoteCategorySelectionChanged();
            }
        }

        public void AddNoteCategory()
        {
            NoteCategoryEditorComponent editor = new NoteCategoryEditorComponent();
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                this.Host.DesktopWindow, editor, SR.TitleAddNoteCategory);
        }

        public void UpdateSelectedNoteCategory()
        {
            // can occur if user double clicks while holding control
            if (_selectedNoteCategory == null) return;

            NoteCategoryEditorComponent editor = new NoteCategoryEditorComponent(_selectedNoteCategory.NoteCategoryRef);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                this.Host.DesktopWindow, editor, SR.TitleUpdateNoteCategory);
        }

        public void LoadNoteCategoryTable()
        {
            try
            {
                Platform.GetService<INoteCategoryAdminService>(
                    delegate(INoteCategoryAdminService service)
                    {
                        ListAllNoteCategoriesResponse response = service.ListAllNoteCategories(new ListAllNoteCategoriesRequest());
                        if (response.NoteCategories != null)
                        {
                            _noteCategoryTable.Items.Clear();
                            _noteCategoryTable.Items.AddRange(response.NoteCategories);
                        }
                    });
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        #endregion

        private void NoteCategorySelectionChanged()
        {
            if (_selectedNoteCategory != null)
                _noteCategoryActionHandler.Edit.Enabled = true;
            else
                _noteCategoryActionHandler.Edit.Enabled = false;
        }

    }
}
