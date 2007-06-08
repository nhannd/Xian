using System;
using System.Collections.Generic;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Tables;

using ClearCanvas.Ris.Application.Common.Admin;
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
                ModalitySummaryComponent component = new ModalitySummaryComponent();

                _workspace = ApplicationComponent.LaunchAsWorkspace(
                    this.Context.DesktopWindow,
                    component,
                    SR.TitleModalities,
                    delegate(IApplicationComponent c) { _workspace = null; });
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

        private ListAllModalitiesRequest _listRequest;

        /// <summary>
        /// Constructor
        /// </summary>
        public ModalitySummaryComponent()
        {
        }

        public override void Start()
        {
            //_modalityAdminService.ModalityChanged += ModalityChangedEventHandler;

            _modalityTable = new ModalityTable();
            _modalityActionHandler = new CrudActionModel();
            _modalityActionHandler.Add.SetClickHandler(AddModality);
            _modalityActionHandler.Edit.SetClickHandler(UpdateSelectedModality);
            _modalityActionHandler.Add.Enabled = true;
            _modalityActionHandler.Delete.Enabled = false;

            InitialisePaging();
            _modalityActionHandler.Merge(_pagingActionHandler);

            base.Start();
        }

        private void InitialisePaging()
        {
            _pagingController = new PagingController<ModalitySummary>(
                delegate(int firstRow, int maxRows)
                {
                    IList<ModalitySummary> modalities = null;

                    try
                    {
                        ListAllModalitiesResponse listResponse = null;

                        Platform.GetService<IModalityAdminService>(
                            delegate(IModalityAdminService service)
                            {
                                ListAllModalitiesRequest listRequest = _listRequest;
                                listRequest.PageRequest.FirstRow = firstRow;
                                listRequest.PageRequest.MaxRows = maxRows;

                                listResponse = service.ListAllModalities(listRequest);
                            });

                        modalities = listResponse.Modalities;
                    }
                    catch (Exception e)
                    {
                        ExceptionHandler.Report(e, this.Host.DesktopWindow);
                    }

                    return modalities;
                }
            );

            _pagingActionHandler = new PagingActionModel<ModalitySummary>(_pagingController, _modalityTable);
        }

        public override void Stop()
        {
            //_modalityAdminService.ModalityChanged -= ModalityChangedEventHandler;

            base.Stop();
        }

        //TODO: ModalityChangedEventHandler
        //private void ModalityChangedEventHandler(object sender, EntityChangeEventArgs e)
        //{
        //    // check if the modality with this oid is in the list
        //    int index = _modalityTable.Items.FindIndex(delegate(Modality m) { return e.EntityRef.RefersTo(m); });
        //    if (index > -1)
        //    {
        //        if (e.ChangeType == EntityChangeType.Update)
        //        {
        //            Modality m = _modalityAdminService.LoadModality((EntityRef<Modality>)e.EntityRef);
        //            _modalityTable.Items[index] = m;
        //        }
        //        else if (e.ChangeType == EntityChangeType.Delete)
        //        {
        //            _modalityTable.Items.RemoveAt(index);
        //        }
        //    }
        //    else
        //    {
        //        if (e.ChangeType == EntityChangeType.Create)
        //        {
        //            Modality m = _modalityAdminService.LoadModality((EntityRef<Modality>)e.EntityRef);
        //            if (m != null)
        //                _modalityTable.Items.Add(m);
        //        }
        //    }
        //}

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
            ModalityEditorComponent editor = new ModalityEditorComponent();
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                this.Host.DesktopWindow, editor, SR.TitleAddModality);
            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                _modalityTable.Items.Add(_selectedModality = editor.ModalitySummary);
                ModalitySelectionChanged();
            }
        }

        public void UpdateSelectedModality()
        {
            // can occur if user double clicks while holding control
            if (_selectedModality == null) return;

            ModalityEditorComponent editor = new ModalityEditorComponent(_selectedModality.ModalityRef);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                this.Host.DesktopWindow, editor, SR.TitleUpdateModality);
            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                _modalityTable.Items.Replace(delegate(ModalitySummary m) { return m.ModalityRef.Equals(editor.ModalitySummary.ModalityRef); }, editor.ModalitySummary);
            }
        }

        public void LoadModalityTable()
        {
            _listRequest = new ListAllModalitiesRequest(false);
            _modalityTable.Items.Clear();
            _modalityTable.Items.AddRange(_pagingController.GetFirst());
        }

        #endregion

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
