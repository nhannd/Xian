using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;

namespace ClearCanvas.Ris.Client
{
	public class ExternalPractitionerContactPointMergeComponent : MergeComponentBase<ExternalPractitionerContactPointSummary>
	{
		private readonly EntityRef _practitionerRef;

		private ILookupHandler _duplicateLookupHandler;
		private ILookupHandler _originalLookupHandler;

		public ExternalPractitionerContactPointMergeComponent(EntityRef practitionerRef)
			: this(practitionerRef, null)
		{
		}

		public ExternalPractitionerContactPointMergeComponent(
			EntityRef practitionerRef,
			ExternalPractitionerContactPointSummary duplicate)
			: base(duplicate)
		{
			_practitionerRef = practitionerRef;
		}

		public override void Start()
		{
			_duplicateLookupHandler = new ExternalPractitionerContactPointLookupHandler(_practitionerRef, this.Host.DesktopWindow);
			_originalLookupHandler = new ExternalPractitionerContactPointLookupHandler(_practitionerRef, this.Host.DesktopWindow);

			base.Start();
		}

		protected override bool IsSameItem(ExternalPractitionerContactPointSummary x, ExternalPractitionerContactPointSummary y)
		{
			return x == null || y == null ? false : x.ContactPointRef.Equals(y.ContactPointRef, true);
		}

		protected override string GenerateReport(ExternalPractitionerContactPointSummary duplicate)
		{
			List<OrderSummary> affectedOrders = null;

			Platform.GetService<IExternalPractitionerAdminService>(
				delegate(IExternalPractitionerAdminService service)
				{
					LoadMergeDuplicateContactPointFormDataRequest request = new LoadMergeDuplicateContactPointFormDataRequest(duplicate);
					LoadMergeDuplicateContactPointFormDataResponse response = service.LoadMergeDuplicateContactPointFormData(request);
					affectedOrders = response.AffectedOrders;
				});

			// TODO: format reports
			return "TODO format reports";
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
						MergeDuplicateContactPointRequest request = new MergeDuplicateContactPointRequest(
							this.SelectedDuplicateSummary,
							this.SelectedOriginalSummary);

						service.MergeDuplicateContactPoint(request);
					});

				// TODO: if the Default is duplicate and deleted, make sure a new default is checked

				base.Accept();
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.ExceptionFailedToMergeDuplicateContactPoints, this.Host.DesktopWindow);
			}
		}
	}
}
