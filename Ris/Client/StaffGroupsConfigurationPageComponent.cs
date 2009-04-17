using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.Ris.Application.Common.Admin.StaffGroupAdmin;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.StaffAdmin;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
	[ExtensionOf(typeof(ConfigurationPageProviderExtensionPoint))]
	public class StaffGroupsConfigurationPageProvider : IConfigurationPageProvider
	{
		public IEnumerable<IConfigurationPage> GetPages()
		{
			// only display this page if current user has associated staff
			return LoginSession.Current.Staff == null ? new IConfigurationPage[0]
				: new IConfigurationPage[] { new StaffGroupConfigurationPageComponent() };
		}
	}

	/// <summary>
	/// Allows a user to modify his membership to elective Staff Groups.
	/// </summary>
	public class StaffGroupConfigurationPageComponent : StaffStaffGroupEditorComponent, IConfigurationPage
	{
		private StaffDetail _staffDetail;

		public StaffGroupConfigurationPageComponent()
		{
		}

		public override void Start()
		{
			base.Start();

			// load group choices
			List<StaffGroupSummary> groupChoices = null;
			Platform.GetService<IStaffGroupAdminService>(
				delegate(IStaffGroupAdminService service)
				{
					ListStaffGroupsRequest request = new ListStaffGroupsRequest();
					request.ElectiveGroupsOnly = true;
					groupChoices = service.ListStaffGroups(request).StaffGroups;
				});

			// load user's staff groups
			Platform.GetService<IStaffAdminService>(
				delegate(IStaffAdminService service)
				{
					_staffDetail = service.LoadStaffForEdit(
						new LoadStaffForEditRequest(LoginSession.Current.Staff.StaffRef)).StaffDetail;
				});

			// initialize lists with elective groups only
			Initialize(
				CollectionUtils.Select(_staffDetail.Groups, delegate(StaffGroupSummary g) { return g.IsElective; }),
				groupChoices);
			
		}

		#region IConfigurationPage Members

		string IConfigurationPage.GetPath()
		{
			return "Staff Groups";
		}

		IApplicationComponent IConfigurationPage.GetComponent()
		{
			return this;
		}

		void IConfigurationPage.SaveConfiguration()
		{
			_staffDetail.Groups = new List<StaffGroupSummary>(this.SelectedItems);

			Platform.GetService<IStaffAdminService>(
				delegate(IStaffAdminService service)
				{
					UpdateStaffRequest request = new UpdateStaffRequest(_staffDetail);

					// even if current user has StaffGroupAdmin token, we don't want to update non-elective groups in this context
					request.UpdateNonElectiveGroups = false;
					service.UpdateStaff(request);
				});

			// reset modified flag
			this.Modified = false;
		}

		#endregion
	}
}
