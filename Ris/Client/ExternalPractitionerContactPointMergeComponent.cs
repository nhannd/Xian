#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
				reportBuilder.AppendLine("Affected Orders");
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
