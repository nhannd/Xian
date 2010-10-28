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
using ClearCanvas.Common.Utilities;

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
			var cp = (ExternalPractitionerContactPointDetail)p;
			return ExternalPractitionerContactPointFormat.Format(cp);
		}

		protected override bool IsSameItem(ExternalPractitionerContactPointDetail x, ExternalPractitionerContactPointDetail y)
		{
			return x == null || y == null ? false : x.ContactPointRef.Equals(y.ContactPointRef, true);
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
				var cost = CalculateMergeCost(this.SelectedOriginalSummary, this.SelectedDuplicateSummary);

                var msg = string.Format("Merge operation will affect {0} orders.", cost);

				if (cost > ExternalPractitionerMergeSettings.Default.MergeCostUserWarningThreshold)
				{
					msg += "\n\nPerforming this operation during regular working hours may adversely affect the system performance for other users.";
					msg += "\nIt is recommended that you do not proceed with the operation unless you are certain it will not impact other users.";
				}
				msg += "\n\nPress 'Cancel' to cancel the operation.\nPress 'OK' to continue. The merge operation cannot be undone.";
				var action = this.Host.ShowMessageBox(msg, MessageBoxActions.OkCancel);
				if (action == DialogBoxAction.Cancel)
				{
					return;
				}

				// perform the merge
				var affectedOrders = PerformMergeOperation(this.SelectedOriginalSummary, this.SelectedDuplicateSummary);
				if(affectedOrders.Count > 0)
				{
					ShowReport(affectedOrders);
				}

				base.Accept();
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.ExceptionFailedToMergeDuplicateContactPoints, this.Host.DesktopWindow);
			}
		}

		private long CalculateMergeCost(ExternalPractitionerContactPointDetail original, ExternalPractitionerContactPointDetail duplicate)
		{
			return ShowProgress("Calculating number of records affected...",
					  service => service.MergeDuplicateContactPoint(new MergeDuplicateContactPointRequest(
										original.ContactPointRef, duplicate.ContactPointRef) { EstimateCostOnly = true }).CostEstimate);
		}

		private IList<string> PerformMergeOperation(ExternalPractitionerContactPointDetail original, ExternalPractitionerContactPointDetail duplicate)
		{
			var response = ShowProgress("Performing merge operation...",
					  service => service.MergeDuplicateContactPoint(new MergeDuplicateContactPointRequest(
										original.ContactPointRef, duplicate.ContactPointRef)));

			return CollectionUtils.Map(response.AffectedOrders, (MergeDuplicateContactPointResponse.AffectedOrder o) => AccessionFormat.Format(o.AccessionNumber));
		}

		private T ShowProgress<T>(
			string message,
			Converter<IExternalPractitionerAdminService, T> action)
		{
			var result = default(T);
			var task = new BackgroundTask(
				delegate(IBackgroundTaskContext context)
				{
					context.ReportProgress(new BackgroundTaskProgress(0, message));

					try
					{
						Platform.GetService<IExternalPractitionerAdminService>(service =>
									{
										result = action(service);
									});
					}
					catch (Exception e)
					{
						context.Error(e);
					}
				}, false);

			ProgressDialog.Show(task, this.Host.DesktopWindow, true, ProgressBarStyle.Marquee);
			return result;
		}

		private void ShowReport(IList<string> affectedOrders)
		{
			var reportBuilder = new StringBuilder();
			reportBuilder.Append("Merge succeeded. ");
			reportBuilder.AppendLine("The following orders have result recipients that were modified to use the replacement contact point.");
			foreach (var order in affectedOrders)
			{
				reportBuilder.AppendLine(order);
			}
			var reportComponent = new MergeOutcomeReportComponent(reportBuilder.ToString());
			LaunchAsDialog(this.Host.DesktopWindow, reportComponent, "Merge Outcome");
		}

	}
}
