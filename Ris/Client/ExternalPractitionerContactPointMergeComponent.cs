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

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	public class ExternalPractitionerContactPointMergeComponent : MergeComponentBase<ExternalPractitionerContactPointDetail>
	{
		private readonly EntityRef _practitionerRef;
		private readonly IList<ExternalPractitionerContactPointDetail> _contactPoints;

		private ILookupHandler _duplicateLookupHandler;
		private ILookupHandler _originalLookupHandler;

		public ExternalPractitionerContactPointMergeComponent(
			EntityRef practitionerRef,
			IList<ExternalPractitionerContactPointDetail> contactPoints)
			: this(practitionerRef, contactPoints, null, null)
		{
		}

		public ExternalPractitionerContactPointMergeComponent(
			EntityRef practitionerRef,
			IList<ExternalPractitionerContactPointDetail> contactPoints,
			ExternalPractitionerContactPointDetail duplicate,
			ExternalPractitionerContactPointDetail original)
			: base(duplicate, original)
		{
			_practitionerRef = practitionerRef;
			_contactPoints = contactPoints;
		}

		public override void Start()
		{
			_duplicateLookupHandler = new ExternalPractitionerContactPointLookupHandler(_practitionerRef, _contactPoints, this.Host.DesktopWindow);
			_originalLookupHandler = new ExternalPractitionerContactPointLookupHandler(_practitionerRef, _contactPoints, this.Host.DesktopWindow);

			base.Start();
		}

		protected override bool IsSameItem(ExternalPractitionerContactPointDetail x, ExternalPractitionerContactPointDetail y)
		{
			return x == null || y == null ? false : x.ContactPointRef.Equals(y.ContactPointRef, true);
		}

		protected override string GenerateReport(ExternalPractitionerContactPointDetail duplicate, ExternalPractitionerContactPointDetail original)
		{
			List<OrderSummary> affectedOrders = null;

			Platform.GetService<IExternalPractitionerAdminService>(
				delegate(IExternalPractitionerAdminService service)
				{
					LoadMergeDuplicateContactPointFormDataRequest request = new LoadMergeDuplicateContactPointFormDataRequest(duplicate.GetSummary());
					LoadMergeDuplicateContactPointFormDataResponse response = service.LoadMergeDuplicateContactPointFormData(request);
					affectedOrders = response.AffectedOrders;
				});

			StringBuilder reportBuilder = new StringBuilder();
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
				CollectionUtils.ForEach(affectedOrders, delegate(OrderSummary o) { reportBuilder.AppendLine(AccessionFormat.Format(o.AccessionNumber)); });
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
						MergeDuplicateContactPointRequest request = new MergeDuplicateContactPointRequest(
							this.SelectedDuplicateSummary.GetSummary(),
							this.SelectedOriginalSummary.GetSummary());

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
