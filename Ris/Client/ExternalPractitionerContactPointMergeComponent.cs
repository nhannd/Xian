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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	public class ExternalPractitionerContactPointMergeComponent : MergeComponentBase<ExternalPractitionerContactPointDetail>
	{
		public ExternalPractitionerContactPointMergeComponent(
			EntityRef practitionerRef,
			IList<ExternalPractitionerContactPointDetail> contactPoints)
			: base(contactPoints)
		{
		}

		public override object FormatItem(object p)
		{
			var cp = (ExternalPractitionerContactPointDetail) p;
			return ExternalPractitionerContactPointFormat.Format(cp);
		}

		protected override bool IsSameItem(ExternalPractitionerContactPointDetail x, ExternalPractitionerContactPointDetail y)
		{
			return x == null || y == null ? false : x.ContactPointRef.Equals(y.ContactPointRef, true);
		}

		protected override string GenerateReport(ExternalPractitionerContactPointDetail duplicate, ExternalPractitionerContactPointDetail original)
		{
			List<OrderSummary> affectedOrders = null;

			Platform.GetService<IExternalPractitionerAdminService>(service =>
			{
				var response = service.LoadMergeDuplicateContactPointFormData(
					new LoadMergeDuplicateContactPointFormDataRequest(duplicate.GetSummary()));
				affectedOrders = response.AffectedOrders;
			});

			var reportBuilder = new StringBuilder();
			reportBuilder.AppendFormat("Replace {0} ({1})", duplicate.Name, duplicate.Description);
			reportBuilder.AppendLine();
			reportBuilder.AppendFormat("with {0} ({1})", original.Name, original.Description);
			reportBuilder.AppendLine();

			reportBuilder.AppendLine();
			if (affectedOrders.Count == 0)
			{
				reportBuilder.AppendLine("No affected orders");
			}
			else
			{
				reportBuilder.AppendLine("The following orders have result recipients that will be modified to use the replacement contact point.");
				affectedOrders.ForEach(order => reportBuilder.AppendLine(AccessionFormat.Format(order.AccessionNumber)));
			}

			return reportBuilder.ToString();
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
				Platform.GetService<IExternalPractitionerAdminService>(service => 
					service.MergeDuplicateContactPoint(
						new MergeDuplicateContactPointRequest(this.SelectedDuplicateSummary.GetSummary(), this.SelectedOriginalSummary.GetSummary())));

				base.Accept();
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.ExceptionFailedToMergeDuplicateContactPoints, this.Host.DesktopWindow);
			}
		}
	}
}
