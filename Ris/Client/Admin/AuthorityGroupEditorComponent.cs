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
using ClearCanvas.Ris.Application.Common.Admin.AuthenticationAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
    public class AuthorityTokenTableEntry
    {
        private AuthorityTokenSummary _summary;
        private bool _selected;
        private event EventHandler _selectedChanged;

        public AuthorityTokenTableEntry(AuthorityTokenSummary authorityTokenSummary, EventHandler onChanged)
        {
            _summary = authorityTokenSummary;
            _selected = false;
            _selectedChanged = onChanged;
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

        public AuthorityTokenSummary AuthorityTokenSummary
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
                delegate(AuthorityTokenTableEntry entry) { return entry.AuthorityTokenSummary.Name; },
                1.5f));

            this.Columns.Add(new TableColumn<AuthorityTokenTableEntry, string>(SR.ColumnAuthorityTokenDescription,
                delegate(AuthorityTokenTableEntry entry) { return entry.AuthorityTokenSummary.Description; },
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
        private EntityRef _authorityGroupRef;
        private AuthorityGroupDetail _authorityGroupDetail;
        private List<AuthorityTokenTableEntry> _authorityTokens;
        private SelectableAuthorityTokenTable _authorityTokenTable;

        /// <summary>
        /// Constructor
        /// </summary>
        public AuthorityGroupEditorComponent()
        {
            _isNew = true;
            _authorityGroupRef = null;
            _authorityTokenTable = new SelectableAuthorityTokenTable();
        }

        public AuthorityGroupEditorComponent(EntityRef authorityGroupRef)
        {
            Platform.CheckForNullReference(authorityGroupRef, "authorityGroupRef");

            _isNew = false;
            _authorityGroupRef = authorityGroupRef;
            _authorityTokenTable = new SelectableAuthorityTokenTable();
        }

        public override void Start()
        {
            Platform.GetService<IAuthenticationAdminService>(
                delegate(IAuthenticationAdminService service)
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
                        LoadAuthorityGroupForEditResponse response = service.LoadAuthorityGroupForEdit(new LoadAuthorityGroupForEditRequest(_authorityGroupRef));
                        _authorityGroupRef = response.AuthorityGroupRef;
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

        public ITable AuthorityTokenTable
        {
            get { return _authorityTokenTable; }
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
                            AddAuthorityGroupResponse response = service.AddAuthorityGroup(new AddAuthorityGroupRequest(_authorityGroupDetail));
                        }
                        else
                        {
                            UpdateAuthorityGroupResponse response = service.UpdateAuthorityGroup(new UpdateAuthorityGroupRequest(_authorityGroupRef, _authorityGroupDetail));
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
                CollectionUtils.Remove<AuthorityTokenSummary>(
                    _authorityGroupDetail.AuthorityTokens,
                    delegate(AuthorityTokenSummary summary)
                    {
                        return summary.EntityRef == changedEntry.AuthorityTokenSummary.EntityRef;
                    });
                this.Modified = true;
            }
            else
            {
                bool alreadyAdded = CollectionUtils.Contains<AuthorityTokenSummary>(
                    _authorityGroupDetail.AuthorityTokens,
                    delegate(AuthorityTokenSummary summary)
                    {
                        return summary.EntityRef == changedEntry.AuthorityTokenSummary.EntityRef;
                    });

                if (alreadyAdded == false)
                {
                    _authorityGroupDetail.AuthorityTokens.Add(changedEntry.AuthorityTokenSummary);
                    this.Modified = true;
                }
            }

        }

        #endregion

        private void InitialiseTable()
        {
            _authorityTokenTable.Items.Clear();
            _authorityTokenTable.Items.AddRange(_authorityTokens);

            foreach (AuthorityTokenSummary selectedToken in _authorityGroupDetail.AuthorityTokens)
            {
                AuthorityTokenTableEntry foundEntry = CollectionUtils.SelectFirst<AuthorityTokenTableEntry>(
                    _authorityTokens,
                    delegate(AuthorityTokenTableEntry entry)
                    {
                        return selectedToken.EntityRef == entry.AuthorityTokenSummary.EntityRef;
                    });

                if (foundEntry != null) foundEntry.Selected = true;
            }
        }
    }
}
