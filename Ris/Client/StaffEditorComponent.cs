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
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.StaffAdmin;
using System.Threading;
using AuthorityTokens=ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Defines an interface for providing custom editing pages to be displayed in the staff editor.
    /// </summary>
    public interface IStaffEditorPageProvider
    {
        IStaffEditorPage[] GetEditorPages(IStaffEditorContext context);
    }

    /// <summary>
    /// Defines an interface for providing a custom editor page with access to the editor
    /// context.
    /// </summary>
    public interface IStaffEditorContext
    {
        EntityRef StaffRef { get; }

        IDictionary<string, string> StaffExtendedProperties { get; }
    }

    /// <summary>
    /// Defines an interface to a custom staff editor page.
    /// </summary>
    public interface IStaffEditorPage
    {
        Path Path { get; }
        IApplicationComponent GetComponent();

        void Save();
    }

    /// <summary>
    /// Defines an extension point for adding custom pages to the staff editor.
    /// </summary>
    public class StaffEditorPageProviderExtensionPoint : ExtensionPoint<IStaffEditorPageProvider>
    {
    }



    /// <summary>
    /// Allows editing of staff person information.
    /// </summary>
    public class StaffEditorComponent : NavigatorComponentContainer
    {
        #region StaffEditorContext

        class EditorContext : IStaffEditorContext
        {
            private readonly StaffEditorComponent _owner;

            public EditorContext(StaffEditorComponent owner)
            {
                _owner = owner;
            }

            public EntityRef StaffRef
            {
                get { return _owner._staffRef; }
            }

            public IDictionary<string, string> StaffExtendedProperties
            {
                get { return _owner._staffDetail.ExtendedProperties; }
            }
        }

        #endregion


        private EntityRef _staffRef;
        private StaffDetail _staffDetail;

        // return values for staff
        private StaffSummary _staffSummary;

        private readonly bool _isNew;

        private StaffDetailsEditorComponent _detailsEditor;
        private StaffStaffGroupEditorComponent _groupsEditor;

        private List<IStaffEditorPage> _extensionPages;

        /// <summary>
        /// Constructs an editor to edit a new staff
        /// </summary>
        public StaffEditorComponent()
        {
            _isNew = true;
        }

        /// <summary>
        /// Constructs an editor to edit an existing staff profile
        /// </summary>
        /// <param name="reference"></param>
        public StaffEditorComponent(EntityRef reference)
        {
            _isNew = false;
            _staffRef = reference;
        }

        /// <summary>
        /// Gets summary of staff that was added or edited
        /// </summary>
        public StaffSummary StaffSummary
        {
            get { return _staffSummary; }
        }

        public override void Start()
        {
            Platform.GetService<IStaffAdminService>(
                delegate(IStaffAdminService service)
                {
                    LoadStaffEditorFormDataResponse formDataResponse = service.LoadStaffEditorFormData(new LoadStaffEditorFormDataRequest());

                    this.ValidationStrategy = new AllComponentsValidationStrategy();

                    if (_isNew)
                    {
                        _staffDetail = new StaffDetail();
                        _staffDetail.StaffType = formDataResponse.StaffTypeChoices[0];
                    }
                    else
                    {
                        LoadStaffForEditResponse response = service.LoadStaffForEdit(new LoadStaffForEditRequest(_staffRef));
                        _staffRef = response.StaffRef;
                        _staffDetail = response.StaffDetail;
                    }

                    this.Pages.Add(new NavigatorPage("Staff", _detailsEditor = new StaffDetailsEditorComponent(_isNew, formDataResponse.StaffTypeChoices)));
                    _detailsEditor.StaffDetail = _staffDetail;

					// allow modification of groups only iff the user has StaffGroup admin permissions
					if(Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Admin.Data.StaffGroup))
					{
						this.Pages.Add(new NavigatorPage("Staff/Groups", _groupsEditor = new StaffStaffGroupEditorComponent(_staffDetail.Groups, formDataResponse.StaffGroupChoices)));
					}
                });

            // instantiate all extension pages
            _extensionPages = new List<IStaffEditorPage>();
            foreach (IStaffEditorPageProvider pageProvider in new StaffEditorPageProviderExtensionPoint().CreateExtensions())
            {
                _extensionPages.AddRange(pageProvider.GetEditorPages(new EditorContext(this)));
            }

            // add extension pages to navigator
            // the navigator will start those components if the user goes to that page
            foreach (IStaffEditorPage page in _extensionPages)
            {
                this.Pages.Add(new NavigatorPage(page.Path.LocalizedPath, page.GetComponent()));
            }

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public override void Accept()
        {
            if (this.HasValidationErrors)
            {
                this.ShowValidation(true);
                return;
            }

            try
            {
                // give extension pages a chance to save data prior to commit
                _extensionPages.ForEach(delegate (IStaffEditorPage page) { page.Save(); });

                // update groups
				if(_groupsEditor != null)
				{
					_staffDetail.Groups = new List<StaffGroupSummary>(_groupsEditor.SelectedItems);
				}

                Platform.GetService<IStaffAdminService>(
                    delegate(IStaffAdminService service)
                    {
                        if (_isNew)
                        {
                            AddStaffResponse response = service.AddStaff(new AddStaffRequest(_staffDetail));
                            _staffRef = response.Staff.StaffRef;
                            _staffSummary = response.Staff;
                        }
                        else
                        {
                            UpdateStaffResponse response = service.UpdateStaff(new UpdateStaffRequest(_staffRef, _staffDetail));
                            _staffRef = response.Staff.StaffRef;
                            _staffSummary = response.Staff;
                        }
                    });

                this.Exit(ApplicationComponentExitCode.Accepted);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, SR.ExceptionSaveStaff, this.Host.DesktopWindow,
                    delegate()
                    {
                        this.ExitCode = ApplicationComponentExitCode.Error;
                        this.Host.Exit();
                    });
            }
        }

        public override void Cancel()
        {
            base.Cancel();
        }
    }
}
