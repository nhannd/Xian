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
using System.Security.Permissions;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Alerts;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;
using AuthorityTokens = ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Application.Services.Admin.ExternalPractitionerAdmin
{
	[ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
	[ServiceImplementsContract(typeof(IExternalPractitionerAdminService))]
	public class ExternalPractitionerAdminService : ApplicationServiceBase, IExternalPractitionerAdminService
	{
		#region IExternalPractitionerAdminService Members

		[ReadOperation]
		// note: this operation is not protected with ClearCanvas.Ris.Application.Common.AuthorityTokens.ExternalPractitionerAdmin
		// because it is used in non-admin situations - perhaps we need to create a separate operation???
		public ListExternalPractitionersResponse ListExternalPractitioners(ListExternalPractitionersRequest request)
		{
			var assembler = new ExternalPractitionerAssembler();

			var criteria = new ExternalPractitionerSearchCriteria();

			if (request.SortByLastVerifiedTime)
			{
				if (request.SortAscending)
					criteria.LastVerifiedTime.SortAsc(0);
				else
					criteria.LastVerifiedTime.SortDesc(0);
			}
			else if (request.SortByLastEditedTime)
			{
				if (request.SortAscending)
					criteria.LastEditedTime.SortAsc(0);
				else
					criteria.LastEditedTime.SortDesc(0);
			}
			else
			{
				criteria.Name.FamilyName.SortAsc(0);
			}

			if (!string.IsNullOrEmpty(request.FirstName))
				criteria.Name.GivenName.StartsWith(request.FirstName);
			if (!string.IsNullOrEmpty(request.LastName))
				criteria.Name.FamilyName.StartsWith(request.LastName);

			switch (request.VerifiedState)
			{
				case VerifiedState.Verified:
					criteria.IsVerified.EqualTo(true);
					break;
				case VerifiedState.NotVerified:
					criteria.IsVerified.EqualTo(false);
					break;
			}

			if (request.LastVerifiedRangeFrom != null && request.LastVerifiedRangeUntil != null)
				criteria.LastVerifiedTime.Between(request.LastVerifiedRangeFrom, request.LastVerifiedRangeUntil);
			else if (request.LastVerifiedRangeFrom != null)
				criteria.LastVerifiedTime.MoreThanOrEqualTo(request.LastVerifiedRangeFrom);
			else if (request.LastVerifiedRangeUntil != null)
				criteria.LastVerifiedTime.LessThanOrEqualTo(request.LastVerifiedRangeUntil);

			if (!request.IncludeDeactivated)
				criteria.Deactivated.EqualTo(false);

			var results = new List<ExternalPractitionerSummary>();
			if (request.QueryItems)
				results = CollectionUtils.Map<ExternalPractitioner, ExternalPractitionerSummary, List<ExternalPractitionerSummary>>(
					PersistenceContext.GetBroker<IExternalPractitionerBroker>().Find(criteria, request.Page),
					s => assembler.CreateExternalPractitionerSummary(s, PersistenceContext));

			var itemCount = -1;
			if (request.QueryCount)
				itemCount = (int)PersistenceContext.GetBroker<IExternalPractitionerBroker>().Count(criteria);

			return new ListExternalPractitionersResponse(results, itemCount);
		}

		[ReadOperation]
		//[PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.ExternalPractitionerAdmin)]
		public LoadExternalPractitionerForEditResponse LoadExternalPractitionerForEdit(LoadExternalPractitionerForEditRequest request)
		{
			// note that the version of the ExternalPractitionerRef is intentionally ignored here (default behaviour of ReadOperation)
			var practitioner = PersistenceContext.Load<ExternalPractitioner>(request.PractitionerRef);
			var assembler = new ExternalPractitionerAssembler();

			var response = new LoadExternalPractitionerForEditResponse
				{ PractitionerDetail = assembler.CreateExternalPractitionerDetail(practitioner, this.PersistenceContext) };

			if (request.IncludeAlerts)
			{
				var alerts = new List<AlertNotification>();
				alerts.AddRange(AlertHelper.Instance.Test(practitioner, this.PersistenceContext));

				var alertAssembler = new AlertAssembler();
				response.Alerts = CollectionUtils.Map<AlertNotification, AlertNotificationDetail>(alerts, alertAssembler.CreateAlertNotification);
			}

			return response;
		}

		[ReadOperation]
		public LoadExternalPractitionerEditorFormDataResponse LoadExternalPractitionerEditorFormData(LoadExternalPractitionerEditorFormDataRequest request)
		{
			return new LoadExternalPractitionerEditorFormDataResponse(
				EnumUtils.GetEnumValueList<AddressTypeEnum>(PersistenceContext),
				(new SimplifiedPhoneTypeAssembler()).GetPractitionerPhoneTypeChoices(),
				EnumUtils.GetEnumValueList<ResultCommunicationModeEnum>(PersistenceContext));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.ExternalPractitioner)]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.ExternalPractitioner.Create)]
		public AddExternalPractitionerResponse AddExternalPractitioner(AddExternalPractitionerRequest request)
		{
			var prac = new ExternalPractitioner();

			var assembler = new ExternalPractitionerAssembler();
			assembler.UpdateExternalPractitioner(request.PractitionerDetail, prac, PersistenceContext);

			prac.MarkEdited();
			var userCanVerify = Thread.CurrentPrincipal.IsInRole(Common.AuthorityTokens.Admin.Data.ExternalPractitionerVerification);
			if (request.MarkVerified && userCanVerify)
				prac.MarkVerified();

			PersistenceContext.Lock(prac, DirtyState.New);

			// ensure the new prac is assigned an OID before using it in the return value
			PersistenceContext.SynchState();

			return new AddExternalPractitionerResponse(assembler.CreateExternalPractitionerSummary(prac, PersistenceContext));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.ExternalPractitioner)]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.ExternalPractitioner.Update)]
		public UpdateExternalPractitionerResponse UpdateExternalPractitioner(UpdateExternalPractitionerRequest request)
		{
			var prac = PersistenceContext.Load<ExternalPractitioner>(request.PractitionerDetail.PractitionerRef, EntityLoadFlags.CheckVersion);

			EnsureNoDeactivatedContactPointsWithActiveOrders(request.PractitionerDetail.ContactPoints);

			var assembler = new ExternalPractitionerAssembler();
			assembler.UpdateExternalPractitioner(request.PractitionerDetail, prac, PersistenceContext);

			prac.MarkEdited();
			var userCanVerify = Thread.CurrentPrincipal.IsInRole(Common.AuthorityTokens.Admin.Data.ExternalPractitionerVerification);
			if (request.MarkVerified && userCanVerify)
				prac.MarkVerified();

			return new UpdateExternalPractitionerResponse(assembler.CreateExternalPractitionerSummary(prac, PersistenceContext));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.ExternalPractitioner)]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.ExternalPractitioner.Merge)]
		public DeleteExternalPractitionerResponse DeleteExternalPractitioner(DeleteExternalPractitionerRequest request)
		{
			try
			{
				var broker = PersistenceContext.GetBroker<IExternalPractitionerBroker>();
				var practitioner = broker.Load(request.PractitionerRef, EntityLoadFlags.Proxy);
				broker.Delete(practitioner);
				PersistenceContext.SynchState();
				return new DeleteExternalPractitionerResponse();
			}
			catch (PersistenceException)
			{
				throw new RequestValidationException(string.Format(SR.ExceptionFailedToDelete, TerminologyTranslator.Translate(typeof(ExternalPractitioner))));
			}
		}

		[ReadOperation]
		public TextQueryResponse<ExternalPractitionerSummary> TextQuery(TextQueryRequest request)
		{
			var broker = PersistenceContext.GetBroker<IExternalPractitionerBroker>();
			var assembler = new ExternalPractitionerAssembler();

			var helper = new TextQueryHelper<ExternalPractitioner, ExternalPractitionerSearchCriteria, ExternalPractitionerSummary>(
					delegate
					{
						var rawQuery = request.TextQuery;

						var criteria = new List<ExternalPractitionerSearchCriteria>();

						// build criteria against names
						var names = TextQueryHelper.ParsePersonNames(rawQuery);
						criteria.AddRange(CollectionUtils.Map(names,
							delegate(PersonName n)
							{
								var sc = new ExternalPractitionerSearchCriteria();
								sc.Name.FamilyName.StartsWith(n.FamilyName);
								if (n.GivenName != null)
									sc.Name.GivenName.StartsWith(n.GivenName);
								return sc;
							}));

						// build criteria against identifiers
						var ids = TextQueryHelper.ParseIdentifiers(rawQuery);
						criteria.AddRange(CollectionUtils.Map(ids,
									 delegate(string word)
									 {
										 var c = new ExternalPractitionerSearchCriteria();
										 c.LicenseNumber.StartsWith(word);
										 return c;
									 }));

						return criteria.ToArray();
					},
					prac => assembler.CreateExternalPractitionerSummary(prac, PersistenceContext),
					(criteria, threshold) => broker.Count(criteria) <= threshold,
					broker.Find);

			return helper.Query(request);
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.ExternalPractitioner)]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.ExternalPractitioner.Merge)]
		public MergeDuplicateContactPointResponse MergeDuplicateContactPoint(MergeDuplicateContactPointRequest request)
		{
			var broker = PersistenceContext.GetBroker<IExternalPractitionerContactPointBroker>();
			var duplicate = broker.Load(request.Duplicate.ContactPointRef, EntityLoadFlags.Proxy);
			var original = broker.Load(request.Original.ContactPointRef, EntityLoadFlags.Proxy);

			// Change reference of result recipient to the new contact point
			foreach (var order in GetActiveOrdersForContactPoint(duplicate))
			{
				foreach (var resultRecipient in order.ResultRecipients)
				{
					if (resultRecipient.PractitionerContactPoint == duplicate)
						resultRecipient.PractitionerContactPoint = original;
				}
			}

			if (duplicate.IsDefaultContactPoint)
			{
				duplicate.IsDefaultContactPoint = false;
				original.IsDefaultContactPoint = true;
			}

			foreach (var telephoneNumber in duplicate.TelephoneNumbers)
			{
				if (CollectionUtils.Contains(original.TelephoneNumbers, originalsPhoneNumber => originalsPhoneNumber.IsSameNumber(telephoneNumber)))
					continue;

				var copy = (TelephoneNumber)telephoneNumber.Clone();
				copy.ValidRange.Until = copy.ValidRange.Until ?? Platform.Time;
				original.TelephoneNumbers.Add(copy);
			}

			foreach (var address in duplicate.Addresses)
			{
				if (CollectionUtils.Contains(original.Addresses, originalAddress => originalAddress.IsSameAddress(address)))
					continue;

				var copy = (Address)address.Clone();
				copy.ValidRange.Until = copy.ValidRange.Until ?? Platform.Time;
				original.Addresses.Add(copy);
			}

			foreach (var emailAddress in duplicate.EmailAddresses)
			{
				if (CollectionUtils.Contains(original.EmailAddresses, originalsEmailAddress => originalsEmailAddress.IsSameEmailAddress(emailAddress)))
					continue;

				var copy = (EmailAddress)emailAddress.Clone();
				copy.ValidRange.Until = copy.ValidRange.Until ?? Platform.Time;
				original.EmailAddresses.Add(copy);
			}

			duplicate.Deactivated = true;

			this.PersistenceContext.SynchState();

			duplicate.Practitioner.MarkEdited();

			var assembler = new ExternalPractitionerAssembler();
			return new MergeDuplicateContactPointResponse(assembler.CreateExternalPractitionerContactPointSummary(original));
		}

		//[ReadOperation]
		//public LoadMergeDuplicatePractitionerFormDataResponse LoadMergeDuplicatePractitionerFormData(LoadMergeDuplicatePractitionerFormDataRequest request)
		//{
		//    var practitioner = this.PersistenceContext.Load<ExternalPractitioner>(request.Practitioner.PractitionerRef, EntityLoadFlags.Proxy);

		//    var orderAssembler = new OrderAssembler();
		//    var affectedOrders = CollectionUtils.Map<Order, OrderSummary>(
		//        this.PersistenceContext.GetBroker<IOrderBroker>().FindByOrderingPractitioner(practitioner),
		//        o => orderAssembler.CreateOrderSummary(o, this.PersistenceContext));

		//    var visitAssembler = new VisitAssembler();
		//    var affectedVisits = CollectionUtils.Map<Visit, VisitSummary>(
		//        this.PersistenceContext.GetBroker<IVisitBroker>().FindByPractitioner(practitioner),
		//        v => visitAssembler.CreateVisitSummary(v, this.PersistenceContext));

		//    return new LoadMergeDuplicatePractitionerFormDataResponse(affectedOrders, affectedVisits);
		//}

		[ReadOperation]
		public LoadMergeDuplicateContactPointFormDataResponse LoadMergeDuplicateContactPointFormData(LoadMergeDuplicateContactPointFormDataRequest request)
		{
			var broker = this.PersistenceContext.GetBroker<IExternalPractitionerContactPointBroker>();
			var contactPoint = broker.Load(request.ContactPoint.ContactPointRef, EntityLoadFlags.Proxy);

			var orderAssembler = new OrderAssembler();
			var affectedOrderSummaries = CollectionUtils.Map<Order, OrderSummary>(
				GetActiveOrdersForContactPoint(contactPoint),
				order => orderAssembler.CreateOrderSummary(order, this.PersistenceContext));

			return new LoadMergeDuplicateContactPointFormDataResponse(affectedOrderSummaries);
		}

		[UpdateOperation]
		public MergeExternalPractitionerResponse MergeExternalPractitioner(MergeExternalPractitionerRequest request)
		{
			var prac1 = PersistenceContext.Load<ExternalPractitioner>(request.LeftPractitionerRef, EntityLoadFlags.Proxy);
			var prac2 = PersistenceContext.Load<ExternalPractitioner>(request.RightPractitionerRef, EntityLoadFlags.Proxy);

			// determine the optimal merge direction
			var n1 = CountAffectedOrdersAndVisits(prac1);
			var n2 = CountAffectedOrdersAndVisits(prac2);

			// merge practitioner in the optimal direction
			var result = (n1 > n2) ? MergePractitionerHelper(prac1, prac2, request) :
				MergePractitionerHelper(prac2, prac1, request);

			// do contact point replacements
			if(request.ContactPointReplacements != null)
			{
				foreach (var contactPointReplacement in request.ContactPointReplacements)
				{
					var deactivatedContactPoint = PersistenceContext.Load<ExternalPractitionerContactPoint>(
						contactPointReplacement.DeactivatedContactPointRef, EntityLoadFlags.Proxy);
					var replacementContactPoint = PersistenceContext.Load<ExternalPractitionerContactPoint>(
						contactPointReplacement.ReplacementContactPointRef, EntityLoadFlags.Proxy);

					foreach (var affectedOrder in GetActiveOrdersForContactPoint(deactivatedContactPoint))
					{
						foreach (var rr in affectedOrder.ResultRecipients)
						{
							if (rr.PractitionerContactPoint.Equals(deactivatedContactPoint))
								rr.PractitionerContactPoint = replacementContactPoint;
						}
					}
				}
			}

			var assembler = new ExternalPractitionerAssembler();
			return new MergeExternalPractitionerResponse(assembler.CreateExternalPractitionerSummary(result, this.PersistenceContext));
		}

		[ReadOperation]
		public LoadMergeExternalPractitionerFormDataResponse LoadMergeExternalPractitionerFormData(LoadMergeExternalPractitionerFormDataRequest request)
		{
			var response = new LoadMergeExternalPractitionerFormDataResponse();

			if (request.PractitionerRef != null)
			{
				var broker = PersistenceContext.GetBroker<IExternalPractitionerBroker>();
				var practitioner = PersistenceContext.Load<ExternalPractitioner>(request.PractitionerRef);
				var duplicates = broker.GetMergeCandidates(practitioner);

				var assembler = new ExternalPractitionerAssembler();
				response.Duplicates = CollectionUtils.Map<ExternalPractitioner, ExternalPractitionerSummary>(duplicates,
					item => assembler.CreateExternalPractitionerSummary(item, this.PersistenceContext));
			}

			return response;
		}

		#endregion

		private long CountAffectedOrdersAndVisits(ExternalPractitioner practitioner)
		{
			var ordersWhere = new OrderSearchCriteria();
			ordersWhere.OrderingPractitioner.EqualTo(practitioner);
			var orderCount = PersistenceContext.GetBroker<IOrderBroker>().Count(ordersWhere);

			var visitCount = PersistenceContext.GetBroker<IVisitBroker>().CountByVisitPractitioner(practitioner);

			return orderCount + visitCount;
		}

		private ExternalPractitioner MergePractitionerHelper(ExternalPractitioner dest, ExternalPractitioner src, MergeExternalPractitionerRequest details)
		{
			// Update the dest details
			var nameAssembler = new PersonNameAssembler();
			nameAssembler.UpdatePersonName(details.Name, dest.Name);

			dest.LicenseNumber = details.LicenseNumber;
			dest.BillingNumber = details.BillingNumber;

			ExtendedPropertyUtils.Update(dest.ExtendedProperties, details.ExtendedProperties);

			// move all contact points to dest practitioner,
			foreach (var cp in src.ContactPoints)
			{
				cp.Practitioner = dest;
			}
			dest.ContactPoints.AddAll(src.ContactPoints);

			// set "default" and "deactivated" status appropriately
			var defaultContactPoint = details.DefaultContactPointRef != null ?
				PersistenceContext.Load<ExternalPractitionerContactPoint>(details.DefaultContactPointRef) : null;

			var deactivatedContactPoints = details.DeactivatedContactPointRefs == null ? new List<ExternalPractitionerContactPoint>() :
				CollectionUtils.Map(details.DeactivatedContactPointRefs,
				(EntityRef cpRef) => PersistenceContext.Load<ExternalPractitionerContactPoint>(cpRef));

			foreach (var cp in dest.ContactPoints)
			{
				cp.IsDefaultContactPoint = cp.Equals(defaultContactPoint);
				cp.Deactivated = deactivatedContactPoints.Contains(cp);
			}

			// update edited/verified status of dest
			dest.MarkEdited();
			if (Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Admin.Data.ExternalPractitionerVerification))
				dest.MarkVerified();

			// move all referenced orders to the dest
			var ordersWhere = new OrderSearchCriteria();
			ordersWhere.OrderingPractitioner.EqualTo(src);
			var orderBroker = PersistenceContext.GetBroker<IOrderBroker>();
			var orders = orderBroker.Find(ordersWhere);
			foreach (var order in orders)
			{
				order.OrderingPractitioner = dest;
			}

			// move all referenced visits to the dest
			var visitBroker = PersistenceContext.GetBroker<IVisitBroker>();
			var visits = visitBroker.FindByVisitPractitioner(src);
			foreach (var visit in visits)
			{
				foreach (var visitPractitioner in visit.Practitioners)
				{
					if (visitPractitioner.Practitioner.Equals(src))
						visitPractitioner.Practitioner = dest;
				}
			}

			// at this point, there should not be any references left to the source practitioner, so we can safely delete it
			// Deactivate the src
			src.Deactivated = true;
			src.MarkEdited();
			//src.ContactPoints.Clear();
			//PersistenceContext.GetBroker<IExternalPractitionerBroker>().Delete(src);

			return dest;
		}

		private IList<Order> GetActiveOrdersForContactPoint(ExternalPractitionerContactPoint contactPoint)
		{
			var recipientCriteria = new ResultRecipientSearchCriteria();
			recipientCriteria.PractitionerContactPoint.EqualTo(contactPoint);

			// Active order search criteria
			var orderCriteria = new OrderSearchCriteria();
			orderCriteria.Status.In(new[] { OrderStatus.SC, OrderStatus.IP });

			return PersistenceContext.GetBroker<IOrderBroker>().FindByResultRecipient(orderCriteria, recipientCriteria);
		}

		private void EnsureNoDeactivatedContactPointsWithActiveOrders(List<ExternalPractitionerContactPointDetail> details)
		{
			var broker = this.PersistenceContext.GetBroker<IExternalPractitionerContactPointBroker>();
			var contactPointsWithOrders = CollectionUtils.Select(
				details,
				detail =>
				{
					if(detail.ContactPointRef == null)
						return false;  // a new contact point will not have associated orders.q

					var contactPoint = broker.Load(detail.ContactPointRef);
					return contactPoint.Deactivated == false
						&& detail.Deactivated
						&& GetActiveOrdersForContactPoint(contactPoint).Count > 0;
				});

			if (contactPointsWithOrders.Count == 0)
				return;

			var contactPointNames = CollectionUtils.Map<ExternalPractitionerContactPointDetail, string>(
				contactPointsWithOrders, 
				contactPoint => contactPoint.Name);
			var contactPointNameList = string.Join(", ", contactPointNames.ToArray());

			throw new RequestValidationException(string.Format(SR.ExceptionContactPointsHaveActiveOrders, contactPointNameList));
		}
	}
}
