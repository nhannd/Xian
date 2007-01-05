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
    [MenuAction("launch", "global-menus/Admin/Staff")]
    [ClickHandler("launch", "Launch")]
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class StaffSummaryTool : Tool<IDesktopToolContext>
    {
        private IWorkspace _workspace;

        public void Launch()
        {
            if (_workspace == null)
            {
                StaffSummaryComponent component = new StaffSummaryComponent();

                _workspace = ApplicationComponent.LaunchAsWorkspace(
                    this.Context.DesktopWindow,
                    component,
                    SR.TitleStaff,
                    delegate(IApplicationComponent c) { _workspace = null; });
            }
            else
            {
                _workspace.Activate();
            }
        }
    }

    /// <summary>
    /// Extension point for views onto <see cref="StaffSummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class StaffSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// StaffSummaryComponent class
    /// </summary>
    [AssociateView(typeof(StaffSummaryComponentViewExtensionPoint))]
    public class StaffSummaryComponent : ApplicationComponent
    {
        private Staff _selectedStaff;
        private StaffTable _staffTable;

        private IStaffAdminService _staffAdminService;
        private CrudActionModel _staffActionHandler;

        /// <summary>
        /// Constructor
        /// </summary>
        public StaffSummaryComponent()
        {
        }

        public override void Start()
        {
            _staffAdminService = ApplicationContext.GetService<IStaffAdminService>();
            _staffAdminService.StaffChanged += StaffChangedEventHandler;

            _staffTable = new StaffTable();
            LoadStaffTable();

            _staffActionHandler = new CrudActionModel();
            _staffActionHandler.Add.SetClickHandler(AddStaff);
            _staffActionHandler.Edit.SetClickHandler(UpdateSelectedStaff);
            _staffActionHandler.Add.Enabled = true;
            _staffActionHandler.Delete.Enabled = false;

            base.Start();
        }

        public override void Stop()
        {
            _staffAdminService.StaffChanged -= StaffChangedEventHandler;

            base.Stop();
        }

        #region Event Handler

        private void StaffChangedEventHandler(object sender, EntityChangeEventArgs e)
        {
            // check if the staff with this oid is in the list
            int index = _staffTable.Items.FindIndex(delegate(Staff s) { return e.EntityRef.RefersTo(s); });
            if (index > -1)
            {
                if (e.ChangeType == EntityChangeType.Update)
                {
                    try
                    {
                        Staff s = _staffAdminService.LoadStaff((EntityRef<Staff>)e.EntityRef, false);
                        _staffTable.Items[index] = s;
                    }
                    catch (Exception exception)
                    {
                        ExceptionHandler.Report(exception, this.Host.DesktopWindow);
                    }
                }
                else if (e.ChangeType == EntityChangeType.Delete)
                {
                    _staffTable.Items.RemoveAt(index);
                }
            }
            else
            {
                if (e.ChangeType == EntityChangeType.Create)
                {
                    try
                    {
                        Staff s = _staffAdminService.LoadStaff((EntityRef<Staff>)e.EntityRef, false);
                        if (s != null)
                            _staffTable.Items.Add(s);
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

        public ITable Staffs
        {
            get { return _staffTable; }
        }

        public ActionModelNode StaffListActionModel
        {
            get { return _staffActionHandler; }
        }

        public ISelection SelectedStaff
        {
            get { return _selectedStaff == null ? Selection.Empty : new Selection(_selectedStaff); }
            set
            {
                _selectedStaff = (Staff)value.Item;
                StaffSelectionChanged();
            }
        }

        public void AddStaff()
        {
            PractitionerStaffEditorComponent editor = new PractitionerStaffEditorComponent(true);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                this.Host.DesktopWindow, editor, SR.TitleAddStaff);
        }

        public void UpdateSelectedStaff()
        {
            // can occur if user double clicks while holding control
            if (_selectedStaff == null) return;

            PractitionerStaffEditorComponent editor = new PractitionerStaffEditorComponent(new EntityRef<Staff>(_selectedStaff));
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                this.Host.DesktopWindow, editor, SR.TitleUpdateStaff);
        }

        public void LoadStaffTable()
        {
            try
            {
                IList<Staff> staffList = _staffAdminService.GetAllStaffs();
                if (staffList != null)
                {
                    _staffTable.Items.Clear();
                    _staffTable.Items.AddRange(staffList);
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        #endregion

        private void StaffSelectionChanged()
        {
            if (_selectedStaff != null)
                _staffActionHandler.Edit.Enabled = true;
            else
                _staffActionHandler.Edit.Enabled = false;
        }
    }
}
