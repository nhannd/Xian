#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;
using ClearCanvas.Enterprise.Common.Admin.UserAdmin;

namespace ClearCanvas.Enterprise.Desktop
{
    public class AuthorityGroupTableEntry
    {
        private readonly AuthorityGroupSummary _summary;
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
        private readonly bool _isNew;
        private readonly string _userName;
		private readonly SelectableAuthorityGroupTable _table;

		private UserDetail _userDetail;
        private List<AuthorityGroupTableEntry> _authorityGroups;

        private UserSummary _userSummary;

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

		public override void Start()
        {
			// load all auth groups
			Platform.GetService<IAuthorityGroupAdminService>(
				delegate(IAuthorityGroupAdminService service)
				{
					ListAuthorityGroupsResponse authorityGroupsResponse = service.ListAuthorityGroups(new ListAuthorityGroupsRequest());

					_authorityGroups = CollectionUtils.Map<AuthorityGroupSummary, AuthorityGroupTableEntry>(
						authorityGroupsResponse.AuthorityGroups,
						delegate(AuthorityGroupSummary summary)
						{
							return new AuthorityGroupTableEntry(summary, this.OnAuthorityGroupChecked);
						});
				});

			// load user
            Platform.GetService<IUserAdminService>(
                delegate(IUserAdminService service)
                {
                    if (_isNew)
                    {
                        _userDetail = new UserDetail();
                    }
                    else
                    {
                        LoadUserForEditResponse response = service.LoadUserForEdit(new LoadUserForEditRequest(_userName));
                        _userDetail = response.UserDetail;
                    }
                });

			InitialiseTable();

            base.Start();
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

		public string DisplayName
		{
			get { return _userDetail.DisplayName; }
			set
			{
				_userDetail.DisplayName = value;
				this.Modified = true;
			}
		}

        public bool IsUserIdReadOnly
        {
            get { return !_isNew; }
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

        public void Accept()
        {
            if (this.HasValidationErrors)
            {
                this.ShowValidation(true);
                return;
            }

            try
            {
				// add or update the user account
                Platform.GetService<IUserAdminService>(
                    delegate(IUserAdminService service)
                    {
                        if (_isNew)
                        {
                            AddUserResponse response = service.AddUser(new AddUserRequest(_userDetail));
                            _userSummary = response.UserSummary;
                        }
                        else
                        {
                            UpdateUserResponse response = service.UpdateUser(new UpdateUserRequest(_userDetail));
                            _userSummary = response.UserSummary;
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
                    delegate
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
