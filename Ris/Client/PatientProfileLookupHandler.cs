#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client
{
	public class PatientProfileLookupHandler : LookupHandler<TextQueryRequest, PatientProfileSummary>
	{
		private readonly DesktopWindow _desktopWindow;

		public PatientProfileLookupHandler(DesktopWindow desktopWindow)
			: base(PatientProfileLookupSettings.Default.MinQueryStringLength, PatientProfileLookupSettings.Default.QuerySpecificityThreshold)
		{
			_desktopWindow = desktopWindow;
		}

		protected override TextQueryResponse<PatientProfileSummary> DoQuery(TextQueryRequest request)
		{
			TextQueryResponse<PatientProfileSummary> response = null;

			Platform.GetService(
				delegate(IRegistrationWorkflowService service)
				{
					response = service.PatientProfileTextQuery(request);
				});

			return response;
		}

		public override bool ResolveNameInteractive(string query, out PatientProfileSummary result)
		{
			result = null;

			var summaryComponent = new PatientProfileSummaryComponent(true);
			if (!string.IsNullOrEmpty(query))
			{
				summaryComponent.SearchString = query;
			}

			var exitCode = ApplicationComponent.LaunchAsDialog(_desktopWindow, summaryComponent, "Patients");
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				result = (PatientProfileSummary)summaryComponent.SummarySelection.Item;
			}

			return (result != null);
		}


		public override string FormatItem(PatientProfileSummary item)
		{
			return item == null ? null : 
				string.Format("{0} {1}", item.Mrn.Id, Formatting.PersonNameFormat.Format(item.Name));
		}
	}
}
