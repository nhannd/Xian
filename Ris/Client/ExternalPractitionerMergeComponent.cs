using System;
using System.Text;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	public class ExternalPractitionerMergeComponent : MergeComponentBase<ExternalPractitionerSummary>
	{
		private ILookupHandler _duplicateLookupHandler;
		private ILookupHandler _originalLookupHandler;

		public ExternalPractitionerMergeComponent()
			: this(null, null)
		{
		}

		public ExternalPractitionerMergeComponent(ExternalPractitionerSummary duplicate, ExternalPractitionerSummary original)
			: base(duplicate, original)
		{
		}

		public override void Start()
		{
			_duplicateLookupHandler = new ExternalPractitionerLookupHandler(this.Host.DesktopWindow);
			_originalLookupHandler = new ExternalPractitionerLookupHandler(this.Host.DesktopWindow);

			base.Start();
		}

		protected override bool IsSameItem(ExternalPractitionerSummary x, ExternalPractitionerSummary y)
		{
			return x == null || y == null ? false : x.PractitionerRef.Equals(y.PractitionerRef, true);
		}

		protected override string GenerateReport(ExternalPractitionerSummary duplicate, ExternalPractitionerSummary original)
		{
			List<OrderSummary> affectedOrders = null;
			List<VisitSummary> affectedVisits = null;

			Platform.GetService<IExternalPractitionerAdminService>(
				delegate(IExternalPractitionerAdminService service)
				{
					LoadMergeDuplicatePractitionerFormDataRequest request = new LoadMergeDuplicatePractitionerFormDataRequest(duplicate);
					LoadMergeDuplicatePractitionerFormDataResponse response = service.LoadMergeDuplicatePractitionerFormData(request);
					affectedOrders = response.AffectedOrders;
					affectedVisits = response.AffectedVisits;
				});

			StringBuilder reportBuilder = new StringBuilder();
			reportBuilder.AppendFormat("Replace {0}", PersonNameFormat.Format(duplicate.Name));
			reportBuilder.AppendLine();
			reportBuilder.AppendFormat("with {0}", PersonNameFormat.Format(original.Name));
			reportBuilder.AppendLine();

			reportBuilder.AppendLine();
			if (affectedOrders.Count == 0)
			{
				reportBuilder.AppendLine("No affected orders");
			}
			else
			{
				reportBuilder.AppendLine("Affected Orders accession number");
				CollectionUtils.ForEach(affectedOrders, delegate(OrderSummary o) { reportBuilder.AppendLine(o.AccessionNumber); });
			}

			reportBuilder.AppendLine();
			if (affectedVisits.Count == 0)
			{
				reportBuilder.AppendLine("No affected visits");
			}
			else
			{
				reportBuilder.AppendLine("Affected Visit Id");
				CollectionUtils.ForEach(affectedVisits, delegate(VisitSummary v) { reportBuilder.AppendLine(MrnFormat.Format(v.VisitNumber)); });
			}

			return reportBuilder.ToString();
		}

		public override ILookupHandler DuplicateLookupHandler
		{
			get { return _duplicateLookupHandler; }
		}

		public override ILookupHandler OriginalLookupHandler
		{
			get { return _originalLookupHandler; }
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
				Platform.GetService<IExternalPractitionerAdminService>(
					delegate(IExternalPractitionerAdminService service)
					{
						MergeDuplicatePractitionerRequest request = new MergeDuplicatePractitionerRequest(
							this.SelectedDuplicateSummary,
							this.SelectedOriginalSummary);

						service.MergeDuplicatePractitioner(request);

						service.DeletePractitioner(new DeletePractitionerRequest(this.SelectedDuplicateSummary));
					});

				base.Accept();
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.ExceptionFailedToMergeDuplicatePractitioners, this.Host.DesktopWindow);
			}
		}
	}
}
