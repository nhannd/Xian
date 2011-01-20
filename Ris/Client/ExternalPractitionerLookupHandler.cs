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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
    public class ExternalPractitionerLookupHandler : LookupHandler<TextQueryRequest, ExternalPractitionerSummary>
    {
        private readonly DesktopWindow _desktopWindow;

        public ExternalPractitionerLookupHandler(DesktopWindow desktopWindow)
        {
            _desktopWindow = desktopWindow;
        }

        protected override TextQueryResponse<ExternalPractitionerSummary> DoQuery(TextQueryRequest request)
        {
            TextQueryResponse<ExternalPractitionerSummary> response = null;
            Platform.GetService<IExternalPractitionerAdminService>(
                delegate(IExternalPractitionerAdminService service)
                {
                    response = service.TextQuery(request);
                });
            return response;
        }
        
        public override bool ResolveNameInteractive(string query, out ExternalPractitionerSummary result)
        {
            result = null;

            ExternalPractitionerSummaryComponent component = new ExternalPractitionerSummaryComponent(true);
            if (!string.IsNullOrEmpty(query))
            {
                string[] names = query.Split(',');
                if (names.Length > 0)
                    component.LastName = names[0].Trim();
                if (names.Length > 1)
                    component.FirstName = names[1].Trim();
            }

            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                _desktopWindow, component, SR.TitleExternalPractitioner);

            if (exitCode == ApplicationComponentExitCode.Accepted)
            {
                result = (ExternalPractitionerSummary)component.SummarySelection.Item;
            }

            return (result != null);
        }

        public override string FormatItem(ExternalPractitionerSummary item)
        {
            return PersonNameFormat.Format(item.Name);
        }
    }
}
