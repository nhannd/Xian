using System;
using System.Collections.Generic;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin.AuthenticationAdmin;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Admin
{
    public class AuthorityGroupTableEntry
    {
        private AuthorityGroupSummary _summary;
        private bool _selected;
        private event EventHandler _selectedChanged;

        public AuthorityGroupTableEntry(AuthorityGroupSummary authorityGroupSummary, EventHandler onChanged)
        {
            _summary = authorityGroupSummary;
            _selected = false;
            _selectedChanged += onChanged;
        }

        public bool Selected
        {
            get { return _selected; }
            set
            {
                if (_selected != value)
                {
                    _selected = value;
                    EventsHelper.Fire(_selectedChanged, this, EventArgs.Empty);
                }
            }
        }

        public AuthorityGroupSummary AuthorityGroupSummary
        {
            get { return _summary; }
        }
    }

    public class SelectableAuthorityGroupTable : Table<AuthorityGroupTableEntry>
    {
        public SelectableAuthorityGroupTable()
        {
            this.Columns.Add(new TableColumn<AuthorityGroupTableEntry, bool>(SR.ColumnActive,
                delegate(AuthorityGroupTableEntry entry) { return entry.Selected; },
                delegate(AuthorityGroupTableEntry entry, bool value) { entry.Selected = value; },
                0.5f));

            this.Columns.Add(new TableColumn<AuthorityGroupTableEntry, string>(SR.ColumnAuthorityGroup,
                delegate(AuthorityGroupTableEntry entry) { return entry.AuthorityGroupSummary.Name; },
                2.5f));
        }
    }

    /// <summary>
    /// Extension point for views onto <see cref="UserEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class UserEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// UserEditorComponent class
    /// </summary>
    [AssociateView(typeof(UserEditorComponentViewExtensionPoint))]
    public class UserEditorComponent : ApplicationComponent
    {
        private bool _isNew;
        private EntityRef _userRef;
        private UserDetail _userDetail;
        private List<AuthorityGroupTableEntry> _authorityGroups;
        private SelectableAuthorityGroupTable _table;

        private UserSummary _userSummary;

        /// <summary>
        /// Constructor
        /// </summary>
        public UserEditorComponent()
        {
            _isNew = true;
            _userRef = null;
            _table = new SelectableAuthorityGroupTable();
        }

        public UserEditorComponent(EntityRef userRef)
        {
            Platform.CheckForNullReference(userRef, "userRef");

            _isNew = false;
            _userRef = userRef;
            _table = new SelectableAuthorityGroupTable();
        }

        /// <summary>
        /// Returns the user summary for use by the caller of this component
        /// </summary>
        public UserSummary UserSummary
        {
            get { return _userSummary; }
        }

        public override void Start()
        {
            try
            {
                Platform.GetService<IAuthenticationAdminService>(
                    delegate(IAuthenticationAdminService service)
                    {
                        ListAuthorityGroupsResponse authorityGroupsResponse = service.ListAuthorityGroups(new ListAuthorityGroupsRequest());

                        _authorityGroups = CollectionUtils.Map<AuthorityGroupSummary, AuthorityGroupTableEntry, List<AuthorityGroupTableEntry>>(
                            authorityGroupsResponse.AuthorityGroups,
                            delegate(AuthorityGroupSummary summary)
                            {                            
                                return new AuthorityGroupTableEntry(summary, this.OnAuthorityGroupChecked);
                            });

                        if (_isNew)
                        {
                            _userDetail = new UserDetail();
                        }
                        else
                        {
                            LoadUserForEditResponse response = service.LoadUserForEdit(new LoadUserForEditRequest(_userRef));
                            _userRef = response.UserRef;
                            _userDetail = response.UserDetail;
                        }

                        InitialiseTable();
                    });

            }
            catch (Exception e)
            {
                
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        #region Presentation Model

        public string UserId
        {
            get { return _userDetail.UserId; }
            set
            {
                _userDetail.UserId = value;
                this.Modified = true;
            }
        }

        public bool IsUserIdReadOnly
        {
            get { return !_isNew; }
        }

        public string StaffName
        {
            get { return _userDetail.StaffRef == null ? "" : string.Format("{0}, {1}", _userDetail.Name.FamilyName, _userDetail.Name.GivenName); }
        }

        public ITable Groups
        {
            get { return _table; }
        }

        public void SetStaff()
        {
            StaffSummaryComponent staffComponent = new StaffSummaryComponent(true);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, staffComponent, "Select Staff");
            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                StaffSummary staffSummary = (StaffSummary)staffComponent.SelectedStaff.Item;
                _userDetail.StaffRef = staffSummary.StaffRef;
                _userDetail.Name = staffSummary.PersonNameDetail;

                this.NotifyPropertyChanged("StaffName");
                this.NotifyPropertyChanged("ClearStaffEnabled");
                this.Modified = true;
            }
        }

        public bool ClearStaffEnabled
        {
            get { return _userDetail.StaffRef != null; }
        }

        public void ClearStaff()
        {
            _userDetail.StaffRef = null;
            _userDetail.Name = new PersonNameDetail();

            this.NotifyPropertyChanged("StaffName");
            this.NotifyPropertyChanged("ClearStaffEnabled");
            this.Modified = true;
        }

        public void Accept()
        {
            if (this.HasValidationErrors)
            {
                this.ShowValidation(true);
                return;
            }

            try
            {
                Platform.GetService<IAuthenticationAdminService>(
                    delegate(IAuthenticationAdminService service)
                    {
                        if (_isNew)
                        {
                            AddUserResponse response = service.AddUser(new AddUserRequest(_userDetail));
                            _userSummary = response.UserSummary;
                        }
                        else
                        {
                            UpdateUserResponse response = service.UpdateUser(new UpdateUserRequest(_userRef, _userDetail));
                            _userSummary = response.UserSummary;
                        }
                    });

                this.ExitCode = ApplicationComponentExitCode.Normal;
                this.Host.Exit();
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(
                    e, 
                    string.Format("Cannot {0} user", _isNew ? "add" : "update"), 
                    this.Host.DesktopWindow,
                    delegate()
                    {
                        this.ExitCode = ApplicationComponentExitCode.Error;
                        this.Host.Exit();
                    });
            }
        }

        public bool AcceptEnabled
        {
            get { return this.Modified && string.IsNullOrEmpty(UserId) == false; }
        }

        public event EventHandler AcceptEnabledChanged
        {
            add { this.ModifiedChanged += value; }
            remove { this.ModifiedChanged -= value; }
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            this.Host.Exit();
        }

        public void OnAuthorityGroupChecked(object sender, EventArgs e)
        {
            AuthorityGroupTableEntry changedEntry = (AuthorityGroupTableEntry)sender;

            if (changedEntry.Selected == false)
            {
                // Remove the 
                CollectionUtils.Remove<AuthorityGroupSummary>(_userDetail.AuthorityGroups,
                    delegate(AuthorityGroupSummary summary)
                    {
                        return summary.EntityRef == changedEntry.AuthorityGroupSummary.EntityRef;
                    });
                this.Modified = true;
            }
            else
            {
                bool alreadyAdded = CollectionUtils.Contains<AuthorityGroupSummary>(_userDetail.AuthorityGroups,
                    delegate(AuthorityGroupSummary summary)
                    {
                        return summary.EntityRef == changedEntry.AuthorityGroupSummary.EntityRef;
                    });
                
                if (alreadyAdded == false)
                {
                    _userDetail.AuthorityGroups.Add(changedEntry.AuthorityGroupSummary);
                    this.Modified = true;
                }
            }

        }

        #endregion

        private void InitialiseTable()
        {
            _table.Items.Clear();
            _table.Items.AddRange(_authorityGroups);

            foreach (AuthorityGroupSummary selectedSummary in _userDetail.AuthorityGroups)
            {
                AuthorityGroupTableEntry foundEntry = CollectionUtils.SelectFirst<AuthorityGroupTableEntry>(_authorityGroups,
                    delegate(AuthorityGroupTableEntry entry)
                    {
                        return selectedSummary.EntityRef == entry.AuthorityGroupSummary.EntityRef;
                    });

                if (foundEntry != null) foundEntry.Selected = true;
            }
        }

    }
}
