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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.StaffGroupAdmin;
using ClearCanvas.Ris.Client.Formatting;
using System.Threading;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Defines an interface for providing custom editing pages to be displayed in the staff group editor.
	/// </summary>
	public interface IStaffGroupEditorPageProvider : IExtensionPageProvider<IStaffGroupEditorPage, IStaffGroupEditorContext>
	{
	}

	/// <summary>
	/// Defines an interface for providing a custom editor page with access to the editor context.
	/// </summary>
	public interface IStaffGroupEditorContext
	{
		EntityRef StaffGroupRef { get; }
	}

	/// <summary>
	/// Defines an interface to a custom staff group editor page.
	/// </summary>
	public interface IStaffGroupEditorPage : IExtensionPage
	{
		void Save();
	}

	/// <summary>
	/// Defines an extension point for adding custom pages to the staff group editor.
	/// </summary>
	public class StaffGroupEditorPageProviderExtensionPoint : ExtensionPoint<IStaffGroupEditorPageProvider>
	{
	}

	/// <summary>
	/// Allows editing of staff group information.
	/// </summary>
	public class StaffGroupEditorComponent : NavigatorComponentContainer
    {
		#region StaffGroupEditorContext

		class EditorContext : IStaffGroupEditorContext
		{
			private readonly StaffGroupEditorComponent _owner;

			public EditorContext(StaffGroupEditorComponent owner)
			{
				_owner = owner;
			}

			public EntityRef StaffGroupRef
			{
				get { return _owner._staffGroupRef; }
			}
		}

		#endregion

		class StaffTable : Table<StaffSummary>
		{
			public StaffTable()
			{
				this.Columns.Add(new TableColumn<StaffSummary, string>("Name",
					delegate(StaffSummary item) { return PersonNameFormat.Format(item.Name); }, 1.0f));
				this.Columns.Add(new TableColumn<StaffSummary, string>("Role",
					delegate(StaffSummary item) { return item.StaffType.Value; }, 0.5f));
			}
		}

		class WorklistTable : Table<WorklistSummary>
		{
			public WorklistTable()
			{
				this.Columns.Add(new TableColumn<WorklistSummary, string>("Name",
					delegate(WorklistSummary summary) { return summary.DisplayName; },
					0.5f));

				this.Columns.Add(new TableColumn<WorklistSummary, string>("Class",
					delegate(WorklistSummary summary) { return string.Concat(summary.ClassCategoryName, " - ", summary.ClassDisplayName); },
					0.5f));
			}
        }

        private EntityRef _staffGroupRef;
        private StaffGroupDetail _staffGroupDetail;

        // return value
        private StaffGroupSummary _staffGroupSummary;

    	private StaffGroupDetailsEditorComponent _detailsEditor;
    	private SelectorEditorComponent<StaffSummary, StaffTable> _staffEditor;
		private SelectorEditorComponent<WorklistSummary, WorklistTable> _worklistEditor;

		private List<IStaffGroupEditorPage> _extensionPages;

        /// <summary>
        /// Constructs an editor to edit a new staff
        /// </summary>
        public StaffGroupEditorComponent()
        {
        }

        /// <summary>
        /// Constructs an editor to edit an existing staff profile
        /// </summary>
        public StaffGroupEditorComponent(EntityRef staffGroupRef)
        {
            _staffGroupRef = staffGroupRef;
        }

		/// <summary>
		/// Gets summary of staff group that was added or edited
		/// </summary>
		public StaffGroupSummary StaffGroupSummary
        {
            get { return _staffGroupSummary; }
        }

        public override void Start()
        {
        	LoadStaffGroupEditorFormDataResponse formDataResponse = null;

            Platform.GetService<IStaffGroupAdminService>(
                delegate(IStaffGroupAdminService service)
                {
                    formDataResponse = service.LoadStaffGroupEditorFormData(
                        new LoadStaffGroupEditorFormDataRequest());

                    if (_staffGroupRef == null)
                    {
                        _staffGroupDetail = new StaffGroupDetail();
                    }
                    else
                    {
                        LoadStaffGroupForEditResponse response = service.LoadStaffGroupForEdit(new LoadStaffGroupForEditRequest(_staffGroupRef));
                        _staffGroupRef = response.StaffGroup.StaffGroupRef;
                        _staffGroupDetail = response.StaffGroup;
                    }
                });

        	_detailsEditor = new StaffGroupDetailsEditorComponent();
			_detailsEditor.StaffGroupDetail = _staffGroupDetail;

			_staffEditor = new SelectorEditorComponent<StaffSummary, StaffTable>(formDataResponse.AllStaff, _staffGroupDetail.Members,
				delegate(StaffSummary staff) { return staff.StaffRef; });

			bool isWorklistEditorReadOnly = Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Worklist) == false;
			_worklistEditor = new SelectorEditorComponent<WorklistSummary, WorklistTable>(formDataResponse.AllAdminWorklists, _staffGroupDetail.Worklists,
				delegate(WorklistSummary worklist) { return worklist.WorklistRef; },
				isWorklistEditorReadOnly);

			this.Pages.Add(new NavigatorPage("Staff Group", _detailsEditor));
			this.Pages.Add(new NavigatorPage("Staff Group/Staffs", _staffEditor));
			this.Pages.Add(new NavigatorPage(isWorklistEditorReadOnly ? "Staff Group/Worklists (read only)" : "Staff Group/Worklists", _worklistEditor));

			// instantiate all extension pages
			_extensionPages = new List<IStaffGroupEditorPage>();
			foreach (IStaffGroupEditorPageProvider pageProvider in new StaffGroupEditorPageProviderExtensionPoint().CreateExtensions())
			{
				_extensionPages.AddRange(pageProvider.GetPages(new EditorContext(this)));
			}

			// add extension pages to navigator
			// the navigator will start those components if the user goes to that page
			foreach (IStaffGroupEditorPage page in _extensionPages)
			{
				this.Pages.Add(new NavigatorPage(page.Path.LocalizedPath, page.GetComponent()));
			}

            base.Start();
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
				_extensionPages.ForEach(delegate(IStaffGroupEditorPage page) { page.Save(); });

				// Update staffs
				_staffGroupDetail.Members = new List<StaffSummary>(_staffEditor.SelectedItems);

				if (!_worklistEditor.IsReadOnly)
            		_staffGroupDetail.Worklists = new List<WorklistSummary>(_worklistEditor.SelectedItems);

                Platform.GetService<IStaffGroupAdminService>(
                    delegate(IStaffGroupAdminService service)
                    {
                        if (_staffGroupRef == null)
                        {
                            AddStaffGroupResponse response = service.AddStaffGroup(
                                new AddStaffGroupRequest(_staffGroupDetail));
                            _staffGroupRef = response.StaffGroup.StaffGroupRef;
                            _staffGroupSummary = response.StaffGroup;
                        }
                        else
                        {
                            UpdateStaffGroupResponse response = service.UpdateStaffGroup(
                                new UpdateStaffGroupRequest(_staffGroupDetail));
                            _staffGroupRef = response.StaffGroup.StaffGroupRef;
                            _staffGroupSummary = response.StaffGroup;
                        }
                    });

                this.Exit(ApplicationComponentExitCode.Accepted);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, "Unable to save Staff Group", this.Host.DesktopWindow,
                    delegate
                    {
                        this.Exit(ApplicationComponentExitCode.Error);
                    });
            }
        }
    }
}
