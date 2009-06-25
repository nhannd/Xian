using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common.Admin.UserAdmin;
using ClearCanvas.Enterprise.Desktop;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// This class is created so it can be shared between model and the view.  This way the summary object does not have to be exposed to the view.
	/// </summary>
	public class UserLookupData
	{
		private readonly string _userName;

		public UserLookupData(string userName)
		{
			_userName = userName;
		}

		public string UserName
		{
			get { return _userName; }
		}
	}

	public class UserLookupHandler : ILookupHandler
	{
		class UserSuggestionProvider : SuggestionProviderBase<UserLookupData>
		{
			protected override IList<UserLookupData> GetShortList(string query)
			{
				List<UserLookupData> users = new List<UserLookupData>();
				Platform.GetService<IUserAdminService>(
					delegate(IUserAdminService service)
					{
						ListUsersRequest request = new ListUsersRequest();
						request.UserName = query;
						ListUsersResponse response = service.ListUsers(request);
						users = CollectionUtils.Map<UserSummary, UserLookupData>(response.Users,
							delegate(UserSummary summary) { return new UserLookupData(summary.UserName); });
					});

				// sort results in the way that they will be formatted for the suggest box
				users.Sort(
					delegate(UserLookupData x, UserLookupData y)
					{
						return UserLookupHandler.FormatItem(x).CompareTo(UserLookupHandler.FormatItem(y));
					});

				return users;
			}

			protected override string FormatItem(UserLookupData item)
			{
				return UserLookupHandler.FormatItem(item);
			}
		}


		private UserSuggestionProvider _suggestionProvider;
		private readonly IDesktopWindow _desktopWindow;


		public UserLookupHandler(IDesktopWindow desktopWindow)
		{
			_desktopWindow = desktopWindow;
		}

		private static string FormatItem(UserLookupData user)
		{
			return user.UserName;
		}

		#region ILookupHandler Members

		bool ILookupHandler.Resolve(string query, bool interactive, out object result)
		{
			result = null;

			UserSummaryComponent userComponent = new UserSummaryComponent(true);
			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
				_desktopWindow, userComponent, SR.TitleUser);

			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				UserSummary summary = (UserSummary) userComponent.SummarySelection.Item;
				result = new UserLookupData(summary.UserName);
			}

			return (result != null);
		}

		string ILookupHandler.FormatItem(object item)
		{
			return FormatItem((UserLookupData)item);
		}

		ISuggestionProvider ILookupHandler.SuggestionProvider
		{
			get
			{
				if (_suggestionProvider == null)
					_suggestionProvider = new UserSuggestionProvider();

				return _suggestionProvider;
			}
		}

		#endregion
	}
}
