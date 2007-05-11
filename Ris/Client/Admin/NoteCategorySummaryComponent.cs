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

        private PagingController<NoteCategorySummary> _pagingController;
        private PagingActionModel<NoteCategorySummary> _pagingActionHandler;

        private ListAllNoteCategoriesRequest _listRequest;

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

            InitialisePaging();
            _noteCategoryActionHandler.Merge(_pagingActionHandler);

            base.Start();
        }

        private void InitialisePaging()
        {
            _pagingController = new PagingController<NoteCategorySummary>(
                delegate(int firstRow, int maxRows)
                {
                    ListAllNoteCategoriesResponse listResponse = null;

                    try
                    {
                        Platform.GetService<INoteCategoryAdminService>(
                            delegate(INoteCategoryAdminService service)
                            {
                                ListAllNoteCategoriesRequest listRequest = _listRequest;
                                listRequest.PageRequest.FirstRow = firstRow;
                                listRequest.PageRequest.MaxRows = maxRows;

                                listResponse = service.ListAllNoteCategories(listRequest);
                            });
                    }
                    catch (Exception e)
                    {
                        ExceptionHandler.Report(e, this.Host.DesktopWindow);
                    }

                    return listResponse.NoteCategories;
                }
            );

            _pagingActionHandler = new PagingActionModel<NoteCategorySummary>(_pagingController, _noteCategoryTable);
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
            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                _noteCategoryTable.Items.Add(_selectedNoteCategory = editor.NoteCategorySummary);
                NoteCategorySelectionChanged();
            }
        }

        public void UpdateSelectedNoteCategory()
        {
            // can occur if user double clicks while holding control
            if (_selectedNoteCategory == null) return;

            NoteCategoryEditorComponent editor = new NoteCategoryEditorComponent(_selectedNoteCategory.NoteCategoryRef);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                this.Host.DesktopWindow, editor, SR.TitleUpdateNoteCategory);
            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                _noteCategoryTable.Items.Replace(
                    delegate(NoteCategorySummary s) { return s.NoteCategoryRef.Equals(editor.NoteCategorySummary.NoteCategoryRef); },
                    editor.NoteCategorySummary);
            }
        }

        public void LoadNoteCategoryTable()
        {
            _listRequest = new ListAllNoteCategoriesRequest();
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
