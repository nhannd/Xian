using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Tables;

using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Client.Common;
using ClearCanvas.Ris.Services;

namespace ClearCanvas.Ris.Client.Admin
{
    [MenuAction("launch", "global-menus/Admin/Modality")]
    [ClickHandler("launch", "Launch")]
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
        private Modality _selectedModality;
        private ModalityTable _modalityTable;

        private IModalityAdminService _modalityAdminService;
        private CrudActionModel _modalityActionHandler;

        /// <summary>
        /// Constructor
        /// </summary>
        public ModalitySummaryComponent()
        {
        }

        public override void Start()
        {
            _modalityAdminService = ApplicationContext.GetService<IModalityAdminService>();
            _modalityAdminService.ModalityChanged += ModalityChangedEventHandler;

            _modalityTable = new ModalityTable();
            LoadModalityTable();

            _modalityActionHandler = new CrudActionModel();
            _modalityActionHandler.Add.SetClickHandler(AddModality);
            _modalityActionHandler.Edit.SetClickHandler(UpdateSelectedModality);
            _modalityActionHandler.Add.Enabled = true;
            _modalityActionHandler.Delete.Enabled = false;

            base.Start();
        }

        public override void Stop()
        {
            _modalityAdminService.ModalityChanged -= ModalityChangedEventHandler;

            base.Stop();
        }

        private void ModalityChangedEventHandler(object sender, EntityChangeEventArgs e)
        {
            // check if the modality with this oid is in the list
            int index = _modalityTable.Items.FindIndex(delegate(Modality m) { return e.EntityRef.RefersTo(m); });
            if (index > -1)
            {
                if (e.ChangeType == EntityChangeType.Update)
                {
                    Modality m = _modalityAdminService.LoadModality((EntityRef<Modality>)e.EntityRef);
                    _modalityTable.Items[index] = m;
                }
                else if (e.ChangeType == EntityChangeType.Delete)
                {
                    _modalityTable.Items.RemoveAt(index);
                }
            }
            else
            {
                if (e.ChangeType == EntityChangeType.Create)
                {
                    Modality m = _modalityAdminService.LoadModality((EntityRef<Modality>)e.EntityRef);
                    if (m != null)
                        _modalityTable.Items.Add(m);
                }
            }
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
                _selectedModality = (Modality)value.Item;
                ModalitySelectionChanged();
            }
        }

        public void AddModality()
        {
            ModalityEditorComponent editor = new ModalityEditorComponent();
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                this.Host.DesktopWindow, editor, SR.TitleAddModality);
        }

        public void UpdateSelectedModality()
        {
            // can occur if user double clicks while holding control
            if (_selectedModality == null) return;

            ModalityEditorComponent editor = new ModalityEditorComponent(new EntityRef<Modality>(_selectedModality));
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                this.Host.DesktopWindow, editor, SR.TitleUpdateModality);
        }

        public void LoadModalityTable()
        {
            try
            {
                IList<Modality> modalityList = _modalityAdminService.GetAllModalities();
                if (modalityList != null)
                {
                    _modalityTable.Items.Clear();
                    _modalityTable.Items.AddRange(modalityList);
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        #endregion

        private void ModalitySelectionChanged()
        {
            if (_selectedModality != null)
                _modalityActionHandler.Edit.Enabled = true;
            else
                _modalityActionHandler.Edit.Enabled = false;
        }
    }
}
