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
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
	public class ExternalPractitionerContactPointLookupHandler : ILookupHandler
	{
		private readonly EntityRef _practitionerRef;
		private readonly IList<ExternalPractitionerContactPointDetail> _contactPoints;
		private readonly IDesktopWindow _desktopWindow;
		private ISuggestionProvider _suggestionProvider;

		public ExternalPractitionerContactPointLookupHandler(
			EntityRef practitionerRef,
			IList<ExternalPractitionerContactPointDetail> contactPoints,
			IDesktopWindow desktopWindow)
		{
			_practitionerRef = practitionerRef;
			_contactPoints = contactPoints;
			_desktopWindow = desktopWindow;
		}

		private static string FormatItem(ExternalPractitionerContactPointDetail cp)
		{
			return string.Format("{0} ({1})", cp.Name, cp.Description);
		}

		#region ILookupHandler Members

		bool ILookupHandler.Resolve(string query, bool interactive, out object result)
		{
			result = null;

			ExternalPractitionerDetail practitionerDetail = null;
			Platform.GetService<IExternalPractitionerAdminService>(
				delegate(IExternalPractitionerAdminService service)
				{
					LoadExternalPractitionerForEditResponse response = service.LoadExternalPractitionerForEdit(new LoadExternalPractitionerForEditRequest(_practitionerRef));
					practitionerDetail = response.PractitionerDetail;
				});

			ExternalPractitionerContactPointSummaryComponent component = new ExternalPractitionerContactPointSummaryComponent(_practitionerRef);

			practitionerDetail.ContactPoints.ForEach(delegate(ExternalPractitionerContactPointDetail p)
													  {
														  component.Subject.Add(p);
													  });

			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
				_desktopWindow, component, SR.TitleContactPoints);

			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				result = component.SummarySelection.Item;
			}

			return (result != null);
		
		}

		string ILookupHandler.FormatItem(object item)
		{
			return FormatItem((ExternalPractitionerContactPointDetail)item);
		}

		ISuggestionProvider ILookupHandler.SuggestionProvider
		{
			get
			{
				if (_suggestionProvider == null)
				{
					var sorted = CollectionUtils.Sort(_contactPoints, (x, y) => FormatItem(x).CompareTo(FormatItem(y)));
					_suggestionProvider = new DefaultSuggestionProvider<ExternalPractitionerContactPointDetail>(sorted, FormatItem);
				}
				return _suggestionProvider;
			}
		}

		#endregion
	}
}
