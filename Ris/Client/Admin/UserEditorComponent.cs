#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.UserAdmin;
using ClearCanvas.Ris.Client.Formatting;
using ClearCanvas.Desktop.Validation;

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
        private string _userName;
        private UserDetail _userDetail;
        private List<AuthorityGroupTableEntry> _authorityGroups;
        private SelectableAuthorityGroupTable _table;

        private UserSummary _userSummary;
		private UserSummary _affectedUserSummary;

        /// <summary>
        /// Constructor
        /// </summary>
        public UserEditorComponent()
        {
            _isNew = true;
            _userName = null;
            _table = new SelectableAuthorityGroupTable();
        }

        public UserEditorComponent(string userName)
        {
            Platform.CheckForNullReference(userName, "userName");

            _isNew = false;
            _userName = userName;
            _table = new SelectableAuthorityGroupTable();
        }

        /// <summary>
        /// Returns the user summary for use by the caller of this component
        /// </summary>
        public UserSummary UserSummary
        {
            get { return _userSummary; }
        }

		/// <summary>
		/// Returns other user that is affected by editing the user
		/// </summary>
		public UserSummary AffectedUserSummary
		{
			get { return _affectedUserSummary; }
		}
		
		public override void Start()
        {
            Platform.GetService<IUserAdminService>(
                delegate(IUserAdminService service)
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
                        LoadUserForEditResponse response = service.LoadUserForEdit(new LoadUserForEditRequest(_userName));
                        _userDetail = response.UserDetail;
                    }

                    InitialiseTable();
                });

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        #region Presentation Model

        [ValidateNotNull]
        public string UserId
        {
            get { return _userDetail.UserName; }
            set
            {
                _userDetail.UserName = value;
                this.Modified = true;
            }
        }

        public bool IsUserIdReadOnly
        {
            get { return !_isNew; }
        }

        public string StaffName
        {
            get { return _userDetail.StaffRef == null ? "" : PersonNameFormat.Format(_userDetail.StaffName); }
        }

        public DateTime? ValidFrom
        {
            get { return _userDetail.ValidFrom; }
            set
            {
                // set valid from to the start of the day
                _userDetail.ValidFrom = value == null ? value : value.Value.Date;
                this.Modified = true;
            }
        }

        public DateTime? ValidUntil
        {
            get { return _userDetail.ValidUntil; }
            set
            {
                // set valid unitl to the end of the day
                _userDetail.ValidUntil = value == null ? value : value.Value.Date.AddDays(1).AddTicks(-1);
                this.Modified = true;
             }
        }

        public bool AccountEnabled
        {
            get { return _userDetail.Enabled; }
            set
            {
                _userDetail.Enabled = value;
                this.Modified = true;
            }
        }

        public ITable Groups
        {
            get { return _table; }
        }

        public void SetStaff()
        {
            try
            {
                StaffSummaryComponent staffComponent = new StaffSummaryComponent(true);
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, staffComponent, "Select Staff");
                if (exitCode == ApplicationComponentExitCode.Accepted)
                {
                    StaffSummary staffSummary = (StaffSummary)staffComponent.SummarySelection.Item;
                    _userDetail.StaffRef = staffSummary.StaffRef;
                    _userDetail.StaffName = staffSummary.Name;
                    _userDetail.DisplayName =
                        string.Format("{0}, {1}", _userDetail.StaffName.FamilyName, _userDetail.StaffName.GivenName);

                    this.NotifyPropertyChanged("StaffName");
                    this.NotifyPropertyChanged("ClearStaffEnabled");
                    this.Modified = true;
                }

            }
            catch (Exception e)
            {
                // failed to launch StaffSummaryComponent
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public bool ClearStaffEnabled
        {
            get { return _userDetail.StaffRef != null; }
        }

        public void ClearStaff()
        {
            _userDetail.StaffRef = null;
            _userDetail.StaffName = new PersonNameDetail();
            _userDetail.DisplayName = null;

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
                Platform.GetService<IUserAdminService>(
                    delegate(IUserAdminService service)
                    {
                        if (_isNew)
                        {
                            AddUserResponse response = service.AddUser(new AddUserRequest(_userDetail));
                            _userSummary = response.UserSummary;
                        	_affectedUserSummary = response.AffectedUserSummary;
                        }
                        else
                        {
                            UpdateUserResponse response = service.UpdateUser(new UpdateUserRequest(_userDetail));
                            _userSummary = response.UserSummary;
                        	_affectedUserSummary = response.AffectedUserSummary;
                        }
                    });

                this.Exit(ApplicationComponentExitCode.Accepted);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(
                    e, 
                    SR.ExceptionSaveUser, 
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
            this.ExitCode = ApplicationComponentExitCode.None;
            this.Host.Exit();
        }

        public void OnAuthorityGroupChecked(object sender, EventArgs e)
        {
            AuthorityGroupTableEntry changedEntry = (AuthorityGroupTableEntry)sender;

            if (changedEntry.Selected == false)
            {
                // Remove the 
                CollectionUtils.Remove(_userDetail.AuthorityGroups,
                    delegate(AuthorityGroupSummary summary)
                    {
                        return summary.Name == changedEntry.AuthorityGroupSummary.Name;
                    });
                this.Modified = true;
            }
            else
            {
                bool alreadyAdded = CollectionUtils.Contains(_userDetail.AuthorityGroups,
                    delegate(AuthorityGroupSummary summary)
                    {
                        return summary.Name == changedEntry.AuthorityGroupSummary.Name;
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
                AuthorityGroupTableEntry foundEntry = CollectionUtils.SelectFirst(_authorityGroups,
                    delegate(AuthorityGroupTableEntry entry)
                    {
                        return selectedSummary.Name == entry.AuthorityGroupSummary.Name;
                    });

                if (foundEntry != null) foundEntry.Selected = true;
            }
        }
    }
}
