using System;
using System.Collections.Generic;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin.AuthenticationAdmin;

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

        public bool IsUserIdEditable
        {
            get { return _isNew; }
        }

        public string FamilyName
        {
            get { return _userDetail.UserName.FamilyName; }
            set
            {
                _userDetail.UserName.FamilyName = value;
                this.Modified = true;
            }
        }

        public string GivenName
        {
            get { return _userDetail.UserName.GivenName; }
            set
            {
                _userDetail.UserName.GivenName = value;
                this.Modified = true;
            }
        }

        public string Prefix
        {
            get { return _userDetail.UserName.Prefix; }
            set
            {
                _userDetail.UserName.Prefix = value;
                this.Modified = true;
            }
        }

        public string Suffix
        {
            get { return _userDetail.UserName.Suffix; }
            set
            {
                _userDetail.UserName.Suffix = value;
                this.Modified = true;
            }
        }

        public string MiddleName
        {
            get { return _userDetail.UserName.MiddleName; }
            set
            {
                _userDetail.UserName.MiddleName = value;
                this.Modified = true;
            }
        }

        public string Degree
        {
            get { return _userDetail.UserName.Degree; }
            set
            {
                _userDetail.UserName.Degree = value;
                this.Modified = true;
            }
        }

        public ITable Groups
        {
            get { return _table; }
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
                        }
                        else
                        {
                            UpdateUserResponse response = service.UpdateUser(new UpdateUserRequest(_userRef, _userDetail));
                        }
                    });

                this.ExitCode = ApplicationComponentExitCode.Normal;
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
                this.ExitCode = ApplicationComponentExitCode.Error;
            }

            this.Host.Exit();
        }

        public bool AcceptEnabled
        {
            get { return this.Modified; }
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
