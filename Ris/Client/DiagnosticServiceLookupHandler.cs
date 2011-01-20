#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common.Admin.DiagnosticServiceAdmin;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Client
{
	public interface IDiagnosticServiceInteractiveLookupProvider
	{
		DiagnosticServiceSummary ResolveDiagnosticService(string query, IDesktopWindow desktopWindow);
	}

	[ExtensionPoint]
	public class DiagnosticServiceInteractiveLookupProviderExtensionPoint: ExtensionPoint<IDiagnosticServiceInteractiveLookupProvider>
	{
	}

    public class DiagnosticServiceLookupHandler : LookupHandler<TextQueryRequest, DiagnosticServiceSummary>
    {
        private readonly DesktopWindow _desktopWindow;

        public DiagnosticServiceLookupHandler(DesktopWindow desktopWindow)
			: base(DiagnosticServiceLookupSettings.Default.MinQueryStringLength, DiagnosticServiceLookupSettings.Default.QuerySpecificityThreshold)
        {
            _desktopWindow = desktopWindow;
        }

        protected override TextQueryResponse<DiagnosticServiceSummary> DoQuery(TextQueryRequest request)
        {
            TextQueryResponse<DiagnosticServiceSummary> response = null;
            Platform.GetService<IDiagnosticServiceAdminService>(
                delegate(IDiagnosticServiceAdminService service)
                {
                    response = service.TextQuery(request);
                });
            return response;
        }

        public override bool ResolveNameInteractive(string query, out DiagnosticServiceSummary diagnosticService)
        {
			diagnosticService = null;

			try
			{
				IDiagnosticServiceInteractiveLookupProvider provider = (IDiagnosticServiceInteractiveLookupProvider)
					new DiagnosticServiceInteractiveLookupProviderExtensionPoint().CreateExtension();
				diagnosticService = provider.ResolveDiagnosticService(query, _desktopWindow);

			}
			catch (NotSupportedException)
			{
				// default
				DiagnosticServiceSummaryComponent summaryComponent = new DiagnosticServiceSummaryComponent(true);
				if (!string.IsNullOrEmpty(query))
				{
					summaryComponent.Name = query;
				}

				ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
					_desktopWindow, summaryComponent, "Imaging Services");

				if (exitCode == ApplicationComponentExitCode.Accepted)
				{
					diagnosticService = (DiagnosticServiceSummary)summaryComponent.SummarySelection.Item;
				}
			}

			return (diagnosticService != null);
		}


        public override string FormatItem(DiagnosticServiceSummary item)
        {
            return item.Name;
        }
    }
}
