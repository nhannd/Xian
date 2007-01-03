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
    [MenuAction("launch", "global-menus/Admin/Facilities")]
    [ClickHandler("launch", "Launch")]
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class FacilitySummaryTool : Tool<IDesktopToolContext>
    {
        private IWorkspace _workspace;

        public void Launch()
        {
            if (_workspace == null)
            {
                FacilitySummaryComponent component = new FacilitySummaryComponent();

                _workspace = ApplicationComponent.LaunchAsWorkspace(
                    this.Context.DesktopWindow,
                    component,
                    SR.TitleFacilities,
                    delegate(IApplicationComponent c) { _workspace = null; });
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
        private Facility _selectedFacility;
        private FacilityTable _facilityTable;

        private IFacilityAdminService _facilityAdminService;
        private CrudActionModel _facilityActionHandler;

        /// <summary>
        /// Constructor
        /// </summary>
        public FacilitySummaryComponent()
        {
        }

        public override void Start()
        {
            _facilityAdminService = ApplicationContext.GetService<IFacilityAdminService>();
            _facilityAdminService.FacilityChanged += FacilityChangedEventHandler;

            _facilityTable = new FacilityTable();
            LoadFacilityTable();

            _facilityActionHandler = new CrudActionModel();
            _facilityActionHandler.Add.SetClickHandler(AddFacility);
            _facilityActionHandler.Edit.SetClickHandler(UpdateSelectedFacility);
            _facilityActionHandler.Add.Enabled = true;
            _facilityActionHandler.Delete.Enabled = false;

            base.Start();
        }

        public override void Stop()
        {
            _facilityAdminService.FacilityChanged -= FacilityChangedEventHandler; 
            
            base.Stop();
        }

        private void FacilityChangedEventHandler(object sender, EntityChangeEventArgs e)
        {
            // check if the facility with this oid is in the list
            int index = _facilityTable.Items.FindIndex(delegate(Facility f) { return e.EntityRef.RefersTo(f); });
            if (index > -1)
            {
                if (e.ChangeType == EntityChangeType.Update)
                {
                    Facility f = _facilityAdminService.LoadFacility((EntityRef<Facility>)e.EntityRef);
                    _facilityTable.Items[index] = f;
                }
                else if (e.ChangeType == EntityChangeType.Delete)
                {
                    _facilityTable.Items.RemoveAt(index);
                }
            }
            else
            {
                if (e.ChangeType == EntityChangeType.Create)
                {
                    Facility f = _facilityAdminService.LoadFacility((EntityRef<Facility>)e.EntityRef);
                    if (f != null)
                        _facilityTable.Items.Add(f);
                }
            }
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
                _selectedFacility = (Facility)value.Item;
                FacilitySelectionChanged();
            }
        }

        public void AddFacility()
        {
            FacilityEditorComponent editor = new FacilityEditorComponent();
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                this.Host.DesktopWindow, editor, SR.TitleAddFacility);

            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                _facilityTable.Items.Add(editor.Facility);
            }
        }

        public void UpdateSelectedFacility()
        {
            // can occur if user double clicks while holding control
            if (_selectedFacility == null) return;

            FacilityEditorComponent editor = new FacilityEditorComponent(new EntityRef<Facility>(_selectedFacility));
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                this.Host.DesktopWindow, editor, SR.TitleUpdateFacility);
        }

        public void LoadFacilityTable()
        {
            try
            {
                IList<Facility> facilityList = _facilityAdminService.GetAllFacilities();

                if (facilityList != null)
                {
                    _facilityTable.Items.Clear();
                    _facilityTable.Items.AddRange(facilityList);
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        #endregion

        private void FacilitySelectionChanged()
        {
            if (_selectedFacility != null)
                _facilityActionHandler.Edit.Enabled = true;
            else
                _facilityActionHandler.Edit.Enabled = false;
        }
    }
}
