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

	/// <summary>
	/// Provides utilities for user name resolution.
	/// </summary>
	public class UserLookupHandler : ILookupHandler
	{
		private ISuggestionProvider _suggestionProvider;
		private readonly IDesktopWindow _desktopWindow;


		public UserLookupHandler(IDesktopWindow desktopWindow)
		{
			_desktopWindow = desktopWindow;
		}

		private static string FormatItem(UserLookupData user)
		{
			return user.UserName;
		}

		private static IList<UserLookupData> ListUsers(string query)
		{
			var users = new List<UserLookupData>();
			Platform.GetService<IUserAdminService>(
				service =>
				{
					var request = new ListUsersRequest {UserName = query};
					var response = service.ListUsers(request);
					users = CollectionUtils.Map(response.Users, (UserSummary summary) => new UserLookupData(summary.UserName));
				});

			return users;
		}

		#region ILookupHandler Members

		bool ILookupHandler.Resolve(string query, bool interactive, out object result)
		{
			result = null;

			var userComponent = new UserSummaryComponent(true);
			var exitCode = ApplicationComponent.LaunchAsDialog(
				_desktopWindow, userComponent, SR.TitleUser);

			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				var summary = (UserSummary) userComponent.SummarySelection.Item;
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
				{
					_suggestionProvider = new DefaultSuggestionProvider<UserLookupData>(ListUsers, FormatItem);
				}

				return _suggestionProvider;
			}
		}

		#endregion
	}
}
