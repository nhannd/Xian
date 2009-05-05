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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Admin.UserAdmin;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.StaffAdmin;

namespace ClearCanvas.Ris.Client
{
	[MenuAction("launch", "global-menus/MenuTools/Staff Profile", "Launch")]
	[ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.StaffProfile.Update)]
	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class StaffEditorTool : Tool<IDesktopToolContext>
	{
		public void Launch()
		{
			try
			{
				StaffEditorComponent component = new StaffEditorComponent(LoginSession.Current.Staff.StaffRef);

				ApplicationComponent.LaunchAsDialog(
					this.Context.DesktopWindow,
					component,
					SR.TitleStaff);
			}
			catch (Exception e)
			{
				// could not launch component
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}
	}

	/// <summary>
	/// Defines an interface for providing custom editing pages to be displayed in the staff editor.
	/// </summary>
	public interface IStaffEditorPageProvider : IExtensionPageProvider<IStaffEditorPage, IStaffEditorContext>
	{
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
	public interface IStaffEditorPage : IExtensionPage
	{
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
		private string _staffUserName;

		// return values for staff
		private StaffSummary _staffSummary;

		private readonly bool _isNew;

		private EmailAddressesSummaryComponent _emailAddressesSummary;
		private PhoneNumbersSummaryComponent _phoneNumbersSummary;
		private AddressesSummaryComponent _addressesSummary;

		private StaffDetailsEditorComponent _detailsEditor;
		private StaffStaffGroupEditorComponent _nonElectiveGroupsEditor;
		private StaffStaffGroupEditorComponent _electiveGroupsEditor;

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
						_staffRef = response.StaffDetail.StaffRef;
						_staffDetail = response.StaffDetail;
					}

					_staffUserName = _staffDetail.UserName;

					this.Pages.Add(new NavigatorPage("Staff", _detailsEditor = new StaffDetailsEditorComponent(formDataResponse.StaffTypeChoices, formDataResponse.SexChoices)));
					this.Pages.Add(new NavigatorPage("Staff/Phone Numbers", _phoneNumbersSummary = new PhoneNumbersSummaryComponent(formDataResponse.PhoneTypeChoices)));
					this.Pages.Add(new NavigatorPage("Staff/Addresses", _addressesSummary = new AddressesSummaryComponent(formDataResponse.AddressTypeChoices)));
					this.Pages.Add(new NavigatorPage("Staff/Email Addresses", _emailAddressesSummary = new EmailAddressesSummaryComponent()));

					_addressesSummary.SetModifiedOnListChange = true;
					_phoneNumbersSummary.SetModifiedOnListChange = true;
					_emailAddressesSummary.SetModifiedOnListChange = true;

					_detailsEditor.StaffDetail = _staffDetail;
					_phoneNumbersSummary.Subject = _staffDetail.TelephoneNumbers;
					_addressesSummary.Subject = _staffDetail.Addresses;
					_emailAddressesSummary.Subject = _staffDetail.EmailAddresses;

					// allow modification of non-elective groups only iff the user has StaffGroup admin permissions
					bool nonElectiveGroupsReadOnly = Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.StaffGroup) == false;
					string nonElectiveGroupsPageTitle = nonElectiveGroupsReadOnly ? "Staff/Groups/Non-elective (read only)" : "Staff/Groups/Non-elective";
					this.Pages.Add(new NavigatorPage(nonElectiveGroupsPageTitle, _nonElectiveGroupsEditor = new StaffNonElectiveStaffGroupEditorComponent(_staffDetail.Groups, formDataResponse.StaffGroupChoices, nonElectiveGroupsReadOnly)));
					this.Pages.Add(new NavigatorPage("Staff/Groups/Elective", _electiveGroupsEditor = new StaffElectiveStaffGroupEditorComponent(_staffDetail.Groups, formDataResponse.StaffGroupChoices, false)));
				});

			// instantiate all extension pages
			_extensionPages = new List<IStaffEditorPage>();
			foreach (IStaffEditorPageProvider pageProvider in new StaffEditorPageProviderExtensionPoint().CreateExtensions())
			{
				_extensionPages.AddRange(pageProvider.GetPages(new EditorContext(this)));
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
				_extensionPages.ForEach(delegate(IStaffEditorPage page) { page.Save(); });

				// update non-elective groups
				_staffDetail.Groups = _nonElectiveGroupsEditor != null 
					? new List<StaffGroupSummary>(_nonElectiveGroupsEditor.SelectedItems)
					: new List<StaffGroupSummary>();

				// update elective groups
				if (_electiveGroupsEditor!= null)
				{
					_staffDetail.Groups.AddRange(_electiveGroupsEditor.SelectedItems);
				}

				// add or update staff
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
							UpdateStaffResponse response = service.UpdateStaff(new UpdateStaffRequest(_staffDetail));
							_staffRef = response.Staff.StaffRef;
							_staffSummary = response.Staff;
						}
					});

				// if necessary, update associated user account
				if(_staffUserName != _staffDetail.UserName)
				{
					UpdateUserAccount(_staffDetail.UserName, _staffSummary);
				}

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

		internal static void UpdateUserAccount(string userName, StaffSummary staff)
		{
			if(!string.IsNullOrEmpty(userName))
			{
				Platform.GetService<IUserAdminService>(
					delegate(IUserAdminService service)
					{
						// check if the user account exists
						ListUsersRequest request = new ListUsersRequest();
						request.UserName = userName;
						request.ExactMatchOnly = true;
						UserSummary user = CollectionUtils.FirstElement(service.ListUsers(request).Users);

						if(user != null)
						{
							// modify the display name on the user account
							UserDetail detail = service.LoadUserForEdit(
								new LoadUserForEditRequest(userName)).UserDetail;
							detail.DisplayName = (staff == null) ? null : staff.Name.ToString();

							service.UpdateUser(new UpdateUserRequest(detail));
						}
					});
			}
		}
	}
}
