using System;
using System.Collections.Generic;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Tables;

using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.StaffAdmin;

namespace ClearCanvas.Ris.Client
{
    [MenuAction("launch", "global-menus/Admin/Staff")]
    [ClickHandler("launch", "Launch")]
    [ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.StaffAdmin)]

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
        private StaffSummary _selectedStaff;
        private StaffTable _staffTable;

        private SimpleActionModel _staffActionHandler;
        private readonly string _addStaffKey = "AddStaff";
        private readonly string _addPractionerKey = "AddPractitioner";
        private readonly string _updateStaffKey = "UpdateStaff";

        private PagingController<StaffSummary> _pagingController;

        private ListStaffRequest _listRequest;
        private string _firstName;
        private string _lastName;

        private bool _dialogMode;


        /// <summary>
        /// Constructor
        /// </summary>
        public StaffSummaryComponent()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dialogMode">Indicates whether the component will be shown in a dialog box or not</param>
        public StaffSummaryComponent(bool dialogMode)
        {
            _dialogMode = dialogMode;
        }

        public override void Start()
        {
            //_staffAdminService.StaffChanged += StaffChangedEventHandler;
            //_staffAdminService.PractitionerChanged += PractitionerChangedEventHandler;

            _staffTable = new StaffTable();

            _staffActionHandler = new SimpleActionModel(new ResourceResolver(this.GetType().Assembly));
            _staffActionHandler.AddAction(_addStaffKey, SR.TitleAddPractitioner, "Icons.AddToolSmall.png", SR.TitleAddPractitioner, AddPractitioner);
			_staffActionHandler.AddAction(_addPractionerKey, SR.TitleAddStaff, "Icons.AddToolSmall.png", SR.TitleAddStaff, AddStaff);
			_staffActionHandler.AddAction(_updateStaffKey, SR.TitleUpdateStaff, "Icons.EditToolSmall.png", SR.TitleUpdateStaff, UpdateSelectedStaff);

            InitialisePaging(_staffActionHandler);

            _listRequest = new ListStaffRequest();

            base.Start();

            // if the last name or first name properties are valued, generate an initial search
            if (!string.IsNullOrEmpty(_lastName) || !string.IsNullOrEmpty(_firstName))
            {
                Search();
            }
        }

        private void InitialisePaging(ActionModelNode actionModelNode)
        {
            _pagingController = new PagingController<StaffSummary>(
                delegate(int firstRow, int maxRows)
                {
                    IList<StaffSummary> staff = null;

                    try
                    {
                        ListStaffResponse listResponse = null;

                        Platform.GetService<IStaffAdminService>(
                            delegate(IStaffAdminService service)
                            {
                                _listRequest.PageRequest.FirstRow = firstRow;
                                _listRequest.PageRequest.MaxRows = maxRows;

                                listResponse = service.ListStaff(_listRequest);
                            });

                        staff = listResponse.Staffs;
                    }
                    catch (Exception e)
                    {
                        ExceptionHandler.Report(e, this.Host.DesktopWindow);
                    }

                    return staff;
                }
            );

            if (actionModelNode != null)
            {
                actionModelNode.Merge(new PagingActionModel<StaffSummary>(_pagingController, _staffTable));
                
            }
        }

        public override void Stop()
        {
            //_staffAdminService.StaffChanged -= StaffChangedEventHandler;
            //_staffAdminService.PractitionerChanged -= PractitionerChangedEventHandler;

            base.Stop();
        }

        #region Event Handler

        //TODO: PractitionerChangedEventHandler
        //private void PractitionerChangedEventHandler(object sender, EntityChangeEventArgs e)
        //{
        //    // check if the staff with this oid is in the list
        //    int index = _staffTable.Items.FindIndex(delegate(Staff s) { return e.EntityRef.RefersTo(s); });
        //    IPractitionerAdminService practitionerAdminService = ApplicationContext.GetService<IPractitionerAdminService>();
        //    if (index > -1)
        //    {
        //        if (e.ChangeType == EntityChangeType.Update)
        //        {
        //            try
        //            {
        //                Practitioner p = practitionerAdminService.LoadPractitioner((EntityRef<Practitioner>)e.EntityRef, false);
        //                _staffTable.Items[index] = p;
        //            }
        //            catch (Exception exception)
        //            {
        //                ExceptionHandler.Report(exception, this.Host.DesktopWindow);
        //            }
        //        }
        //        else if (e.ChangeType == EntityChangeType.Delete)
        //        {
        //            _staffTable.Items.RemoveAt(index);
        //        }
        //    }
        //    else
        //    {
        //        if (e.ChangeType == EntityChangeType.Create)
        //        {
        //            try
        //            {
        //                Practitioner p = practitionerAdminService.LoadPractitioner((EntityRef<Practitioner>)e.EntityRef, false);
        //                if (p != null)
        //                    _staffTable.Items.Add(p);
        //            }
        //            catch (Exception exception)
        //            {
        //                ExceptionHandler.Report(exception, this.Host.DesktopWindow);
        //            }
        //        }
        //    }
        //}

        //TODO: StaffChangedEventHandler
        //private void StaffChangedEventHandler(object sender, EntityChangeEventArgs e)
        //{
        //    // check if the staff with this oid is in the list
        //    int index = _staffTable.Items.FindIndex(delegate(Staff s) { return e.EntityRef.RefersTo(s); });
        //    if (index > -1)
        //    {
        //        if (e.ChangeType == EntityChangeType.Update)
        //        {
        //            try
        //            {
        //                Staff s = _staffAdminService.LoadStaff((EntityRef<Staff>)e.EntityRef, false);
        //                _staffTable.Items[index] = s;
        //            }
        //            catch (Exception exception)
        //            {
        //                ExceptionHandler.Report(exception, this.Host.DesktopWindow);
        //            }
        //        }
        //        else if (e.ChangeType == EntityChangeType.Delete)
        //        {
        //            _staffTable.Items.RemoveAt(index);
        //        }
        //    }
        //    else
        //    {
        //        if (e.ChangeType == EntityChangeType.Create)
        //        {
        //            try
        //            {
        //                Staff s = _staffAdminService.LoadStaff((EntityRef<Staff>)e.EntityRef, false);
        //                if (s != null)
        //                    _staffTable.Items.Add(s);
        //            }
        //            catch (Exception exception)
        //            {
        //                ExceptionHandler.Report(exception, this.Host.DesktopWindow);
        //            }
        //        }
        //    }
        //}

        #endregion

        #region Presentation Model

        public bool ShowAcceptCancelButtons
        {
            get { return _dialogMode; }
            set { _dialogMode = value; }
        }

        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

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
                _selectedStaff = (StaffSummary)value.Item;
                StaffSelectionChanged();
            }
        }

        public void AddStaff()
        {
            StaffEditorComponent editor = new StaffEditorComponent(true);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                this.Host.DesktopWindow, editor, SR.TitleAddStaff);
            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                _staffTable.Items.Add(editor.StaffSummary);
            }
        }

        public void AddPractitioner()
        {
            StaffEditorComponent editor = new StaffEditorComponent(false);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                this.Host.DesktopWindow, editor, SR.TitleAddPractitioner);
            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                _staffTable.Items.Add(editor.PractitionerSummary);
            }
        }

        public void UpdateSelectedStaff()
        {
            // can occur if user double clicks while holding control
            if (_selectedStaff == null) return;

            StaffEditorComponent editor;
            if (_selectedStaff.LicenseNumber == null || _selectedStaff.LicenseNumber == "")
            {
                editor = new StaffEditorComponent(_selectedStaff.StaffRef, true);
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                    this.Host.DesktopWindow, editor, SR.TitleUpdateStaff);
                if (exitCode == ApplicationComponentExitCode.Normal)
                {
                    _staffTable.Items.Replace(
                        delegate(StaffSummary s) { return s.StaffRef.Equals(editor.StaffSummary.StaffRef); },
                        editor.StaffSummary);
                }
            }
            else
            {
                editor = new StaffEditorComponent(_selectedStaff.StaffRef, false);
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                    this.Host.DesktopWindow, editor, SR.TitleUpdatePractitioner);
                if (exitCode == ApplicationComponentExitCode.Normal)
                {
                    _staffTable.Items.Replace(
                        delegate(StaffSummary s) { return s.StaffRef.Equals(editor.PractitionerSummary.StaffRef); },
                        editor.PractitionerSummary);
                }
            }
        }

        public void DoubleClickSelectedStaff()
        {
            // double-click behaviour is different depending on whether we're running as a dialog box or not
            if (_dialogMode)
                Accept();
            else
                UpdateSelectedStaff();
        }


        public void Search()
        {
            _listRequest.FirstName = _firstName;
            _listRequest.LastName = _lastName;

            _staffTable.Items.Clear();
            _staffTable.Items.AddRange(_pagingController.GetFirst());
        }

        public bool AcceptEnabled
        {
            get { return _selectedStaff != null; }
        }

        public void Accept()
        {
            this.ExitCode = ApplicationComponentExitCode.Normal;
            this.Host.Exit();
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            this.Host.Exit();
        }

        #endregion

        private void StaffSelectionChanged()
        {
            if (_selectedStaff != null)
                _staffActionHandler[_updateStaffKey].Enabled = true;
            else
                _staffActionHandler[_updateStaffKey].Enabled = false;
        }
    }
}