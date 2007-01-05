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
    [MenuAction("launch", "global-menus/Admin/Practitioner")]
    [ClickHandler("launch", "Launch")]
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class PractitionerSummaryTool : Tool<IDesktopToolContext>
    {
        private IWorkspace _workspace;

        public void Launch()
        {
            if (_workspace == null)
            {
                PractitionerSummaryComponent component = new PractitionerSummaryComponent();

                _workspace = ApplicationComponent.LaunchAsWorkspace(
                    this.Context.DesktopWindow,
                    component,
                    SR.TitlePractitioner,
                    delegate(IApplicationComponent c) { _workspace = null; });
            }
            else
            {
                _workspace.Activate();
            }
        }
    }

    /// <summary>
    /// Extension point for views onto <see cref="PractitionerSummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class PractitionerSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// PractitionerSummaryComponent class
    /// </summary>
    [AssociateView(typeof(PractitionerSummaryComponentViewExtensionPoint))]
    public class PractitionerSummaryComponent : ApplicationComponent
    {
        private Practitioner _selectedPractitioner;
        private PractitionerTable _practitionerTable;

        private IPractitionerAdminService _practitionerAdminService;
        private CrudActionModel _practitionerActionHandler;

        /// <summary>
        /// Constructor
        /// </summary>
        public PractitionerSummaryComponent()
        {
        }

        public override void Start()
        {
            _practitionerAdminService = ApplicationContext.GetService<IPractitionerAdminService>();
            _practitionerAdminService.PractitionerChanged += PractitionerChangedEventHandler;

            _practitionerTable = new PractitionerTable();
            LoadPractitionerTable();

            _practitionerActionHandler = new CrudActionModel();
            _practitionerActionHandler.Add.SetClickHandler(AddPractitioner);
            _practitionerActionHandler.Edit.SetClickHandler(UpdateSelectedPractitioner);
            _practitionerActionHandler.Add.Enabled = true;
            _practitionerActionHandler.Delete.Enabled = false;

            base.Start();
        }

        public override void Stop()
        {
            _practitionerAdminService.PractitionerChanged -= PractitionerChangedEventHandler;

            base.Stop();
        }

        #region Event Handler

        private void PractitionerChangedEventHandler(object sender, EntityChangeEventArgs e)
        {
            // check if the practitioner with this oid is in the list
            int index = _practitionerTable.Items.FindIndex(delegate(Practitioner s) { return e.EntityRef.RefersTo(s); });
            if (index > -1)
            {
                if (e.ChangeType == EntityChangeType.Update)
                {
                    try
                    {
                        Practitioner s = _practitionerAdminService.LoadPractitioner((EntityRef<Practitioner>)e.EntityRef, false);
                        _practitionerTable.Items[index] = s;
                    }
                    catch (Exception exception)
                    {
                        ExceptionHandler.Report(exception, this.Host.DesktopWindow);
                    }
                }
                else if (e.ChangeType == EntityChangeType.Delete)
                {
                    _practitionerTable.Items.RemoveAt(index);
                }
            }
            else
            {
                if (e.ChangeType == EntityChangeType.Create)
                {
                    try
                    {
                        Practitioner s = _practitionerAdminService.LoadPractitioner((EntityRef<Practitioner>)e.EntityRef, false);
                        if (s != null)
                            _practitionerTable.Items.Add(s);
                    }
                    catch (Exception exception)
                    {
                        ExceptionHandler.Report(exception, this.Host.DesktopWindow);
                    }
                }
            }
        }

        #endregion

        #region Presentation Model

        public ITable Practitioners
        {
            get { return _practitionerTable; }
        }

        public ActionModelNode PractitionerListActionModel
        {
            get { return _practitionerActionHandler; }
        }

        public ISelection SelectedPractitioner
        {
            get { return _selectedPractitioner == null ? Selection.Empty : new Selection(_selectedPractitioner); }
            set
            {
                _selectedPractitioner = (Practitioner)value.Item;
                PractitionerSelectionChanged();
            }
        }

        public void AddPractitioner()
        {
            PractitionerStaffEditorComponent editor = new PractitionerStaffEditorComponent(false);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                this.Host.DesktopWindow, editor, SR.TitleAddPractitioner);
        }

        public void UpdateSelectedPractitioner()
        {
            // can occur if user double clicks while holding control
            if (_selectedPractitioner == null) return;

            PractitionerStaffEditorComponent editor = new PractitionerStaffEditorComponent(new EntityRef<Practitioner>(_selectedPractitioner));
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                this.Host.DesktopWindow, editor, SR.TitleUpdatePractitioner);
        }

        public void LoadPractitionerTable()
        {
            try
            {
                IList<Practitioner> practitionerList = _practitionerAdminService.GetAllPractitioners();
                if (practitionerList != null)
                {
                    _practitionerTable.Items.Clear();
                    _practitionerTable.Items.AddRange(practitionerList);
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        #endregion

        private void PractitionerSelectionChanged()
        {
            if (_selectedPractitioner != null)
                _practitionerActionHandler.Edit.Enabled = true;
            else
                _practitionerActionHandler.Edit.Enabled = false;
        }
    }
}
