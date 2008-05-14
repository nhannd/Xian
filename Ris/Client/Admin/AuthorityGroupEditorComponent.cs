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
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin.UserAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
    public class AuthorityTokenTableEntry
    {
        private readonly AuthorityTokenSummary _summary;
        private bool _selected;
        private event EventHandler _selectedChanged;
    	private readonly Path _path;

        public AuthorityTokenTableEntry(AuthorityTokenSummary authorityTokenSummary, EventHandler onChanged)
        {
            _summary = authorityTokenSummary;
            _selected = false;
            _selectedChanged = onChanged;
			_path = new Path(_summary.Name, new ResourceResolver(this.GetType().Assembly));
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

    	public string Name
    	{
			get { return _summary.Name; }
    	}

    	public string Description
    	{
			get { return _summary.Description; }
    	}

    	public Path Path
    	{
			get { return _path; }
    	}

    	internal AuthorityTokenSummary Summary
    	{
			get { return _summary; }
    	}
    }

    public class SelectableAuthorityTokenTable : Table<AuthorityTokenTableEntry>
    {
        public SelectableAuthorityTokenTable ()
    	{
            this.Columns.Add(new TableColumn<AuthorityTokenTableEntry, bool>(SR.ColumnActive,
                delegate(AuthorityTokenTableEntry entry) { return entry.Selected; },
                delegate(AuthorityTokenTableEntry entry, bool value) { entry.Selected = value; },
                0.5f));

            this.Columns.Add(new TableColumn<AuthorityTokenTableEntry, string>(SR.ColumnAuthorityTokenName,
                delegate(AuthorityTokenTableEntry entry) { return entry.Name; },
                1.5f));

            this.Columns.Add(new TableColumn<AuthorityTokenTableEntry, string>(SR.ColumnAuthorityTokenDescription,
                delegate(AuthorityTokenTableEntry entry) { return entry.Description; },
                3.5f));
	    }
    }

    /// <summary>
    /// Extension point for views onto <see cref="AuthorityGroupEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class AuthorityGroupEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// AuthorityGroupEditorComponent class
    /// </summary>
    [AssociateView(typeof(AuthorityGroupEditorComponentViewExtensionPoint))]
    public class AuthorityGroupEditorComponent : ApplicationComponent
    {
        private bool _isNew;
        private string _authorityGroupName;
        private AuthorityGroupDetail _authorityGroupDetail;
		private List<AuthorityTokenTableEntry> _authorityTokens;

        /// <summary>
        /// Constructor
        /// </summary>
        public AuthorityGroupEditorComponent()
        {
            _isNew = true;
        }

        public AuthorityGroupEditorComponent(string authorityGroupName)
        {
            Platform.CheckForNullReference(authorityGroupName, "authorityGroupName");

            _isNew = false;
            _authorityGroupName = authorityGroupName;
        }

        public override void Start()
        {
            Platform.GetService<IUserAdminService>(
                delegate(IUserAdminService service)
                {
                    ListAuthorityTokensResponse authorityTokenResponse = service.ListAuthorityTokens(new ListAuthorityTokensRequest());

                    _authorityTokens = CollectionUtils.Map<AuthorityTokenSummary, AuthorityTokenTableEntry, List<AuthorityTokenTableEntry>>(
                        authorityTokenResponse.AuthorityTokens,
                        delegate(AuthorityTokenSummary summary)
                        {
                            return new AuthorityTokenTableEntry(summary, this.OnAuthorityTokenChecked);
                        });

                    if (_isNew)
                    {
                        _authorityGroupDetail = new AuthorityGroupDetail();
                    }
                    else
                    {
                        LoadAuthorityGroupForEditResponse response = service.LoadAuthorityGroupForEdit(new LoadAuthorityGroupForEditRequest(_authorityGroupName));
                        _authorityGroupDetail = response.AuthorityGroupDetail;
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

        public string Name
        {
            get { return _authorityGroupDetail.Name; }
            set
            {
                _authorityGroupDetail.Name = value;
                this.Modified = true;
            }
        }

        public List<AuthorityTokenTableEntry> AuthorityTokens
        {
            get { return _authorityTokens; }
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
                            AddAuthorityGroupResponse response = service.AddAuthorityGroup(new AddAuthorityGroupRequest(_authorityGroupDetail));
                        }
                        else
                        {
                            UpdateAuthorityGroupResponse response = service.UpdateAuthorityGroup(new UpdateAuthorityGroupRequest(_authorityGroupDetail));
                        }
                    });

                this.Exit(ApplicationComponentExitCode.Accepted);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, SR.ExceptionSaveAuthorityGroup, this.Host.DesktopWindow,
                    delegate()
                    {
                        this.ExitCode = ApplicationComponentExitCode.Error;
                        this.Host.Exit();
                    });
            }
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
            this.ExitCode = ApplicationComponentExitCode.None;
            this.Host.Exit();
        }

        public void OnAuthorityTokenChecked(object sender, EventArgs e)
        {
            AuthorityTokenTableEntry changedEntry = (AuthorityTokenTableEntry)sender;

            if (changedEntry.Selected == false)
            {
                CollectionUtils.Remove(
                    _authorityGroupDetail.AuthorityTokens,
                    delegate(AuthorityTokenSummary summary)
                    {
                        return summary.Name == changedEntry.Summary.Name;
                    });
                this.Modified = true;
            }
            else
            {
                bool alreadyAdded = CollectionUtils.Contains(
                    _authorityGroupDetail.AuthorityTokens,
                    delegate(AuthorityTokenSummary summary)
                    {
						return summary.Name == changedEntry.Summary.Name;
                    });

                if (alreadyAdded == false)
                {
					_authorityGroupDetail.AuthorityTokens.Add(changedEntry.Summary);
                    this.Modified = true;
                }
            }

        }

        #endregion

        private void InitialiseTable()
        {
            foreach (AuthorityTokenSummary selectedToken in _authorityGroupDetail.AuthorityTokens)
            {
                AuthorityTokenTableEntry foundEntry = CollectionUtils.SelectFirst(
                    _authorityTokens,
                    delegate(AuthorityTokenTableEntry entry)
                    {
                        return selectedToken.Name == entry.Summary.Name;
                    });

                if (foundEntry != null) foundEntry.Selected = true;
            }
        }
    }
}
