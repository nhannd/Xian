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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.StaffAdmin;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Provides utilities for staff name resolution.
	/// </summary>
	public class StaffLookupHandler : LookupHandler<StaffTextQueryRequest, StaffSummary>
	{
		private readonly DesktopWindow _desktopWindow;
		private readonly string[] _staffTypesFilter;

		public StaffLookupHandler(DesktopWindow desktopWindow)
			: this(desktopWindow, new string[] { })
		{
		}

		public StaffLookupHandler(DesktopWindow desktopWindow, string[] staffTypesFilter)
		{
			_desktopWindow = desktopWindow;
			_staffTypesFilter = staffTypesFilter;
		}

		protected override TextQueryResponse<StaffSummary> DoQuery(StaffTextQueryRequest request)
		{
			if (_staffTypesFilter != null && _staffTypesFilter.Length > 0)
			{
				request.StaffTypesFilter = _staffTypesFilter;
			}

			TextQueryResponse<StaffSummary> response = null;
			Platform.GetService<IStaffAdminService>(
				delegate(IStaffAdminService service)
				{
					response = service.TextQuery(request);
				});
			return response;
		}

		/// <summary>
		/// Shows a dialog to allow user to resolve the specified query to a single staff.
		/// The query may consist of part of the surname,
		/// optionally followed by a comma and then part of the given name (e.g. "sm, b" for smith, bill).
		/// The method returns true if the name is successfully resolved, or false otherwise.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public override bool ResolveNameInteractive(string query, out StaffSummary result)
		{
			result = null;

			StaffSummaryComponent staffComponent = new StaffSummaryComponent(true, _staffTypesFilter);
			if (!string.IsNullOrEmpty(query))
			{
				string[] names = query.Split(',');
				if (names.Length > 0)
					staffComponent.LastName = names[0].Trim();
				if (names.Length > 1)
					staffComponent.FirstName = names[1].Trim();
			}

			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
				_desktopWindow, staffComponent, SR.TitleStaff);

			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				result = (StaffSummary)staffComponent.SummarySelection.Item;
			}

			return (result != null);
		}


		public override string FormatItem(StaffSummary item)
		{
			return PersonNameFormat.Format(item.Name);
		}
	}
}
