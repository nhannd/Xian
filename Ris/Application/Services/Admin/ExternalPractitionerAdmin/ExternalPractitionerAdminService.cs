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
		private delegate TOutput Converter<TInput1, TInput2, TOutput>(TInput1 input1, TInput2 input2);

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

			var response = new LoadExternalPractitionerForEditResponse { PractitionerDetail = assembler.CreateExternalPractitionerDetail(practitioner, this.PersistenceContext) };

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
			var dest = PersistenceContext.Load<ExternalPractitionerContactPoint>(request.RetainedContactPointRef, EntityLoadFlags.Proxy);
			var src = PersistenceContext.Load<ExternalPractitionerContactPoint>(request.ReplacedContactPointRef, EntityLoadFlags.Proxy);

			// sanity check
			if (!Equals(dest.Practitioner, src.Practitioner))
				throw new RequestValidationException("Only contact points belonging to the same practitioner can be merged.");

			// if we are only doing a cost estimate, exit here without modifying any data
			if (request.EstimateCostOnly)
			{
				// compute cost estimate
				var cost = EstimateMergeCost(src);
				return new MergeDuplicateContactPointResponse(cost);
			}

			// merge contact points
			MergeContactPointRecords(dest, src);

			// update affected orders
			var replacements = new Dictionary<ExternalPractitionerContactPoint, ExternalPractitionerContactPoint> { { src, dest } };
			IList<Order> affectedOrders;
			DoContactPointReplacements(replacements, out affectedOrders);

			// if user has verify permission, re-verify the dest
			if (Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Admin.Data.ExternalPractitionerVerification))
				dest.Practitioner.MarkVerified();

			PersistenceContext.SynchState();

			var assembler = new ExternalPractitionerAssembler();
			return new MergeDuplicateContactPointResponse(
				assembler.CreateExternalPractitionerContactPointSummary(dest),
					CollectionUtils.Map(affectedOrders,
						(Order o) => new MergeDuplicateContactPointResponse.AffectedOrder
									{
										AccessionNumber = o.AccessionNumber,
										Status = EnumUtils.GetEnumValueInfo(o.Status, PersistenceContext)
									}));
		}

		[ReadOperation]
		public LoadMergeDuplicateContactPointFormDataResponse LoadMergeDuplicateContactPointFormData(LoadMergeDuplicateContactPointFormDataRequest request)
		{
			return new LoadMergeDuplicateContactPointFormDataResponse();
		}

		[UpdateOperation]
		public MergeExternalPractitionerResponse MergeExternalPractitioner(MergeExternalPractitionerRequest request)
		{
			// unpack the request, loading all required entities
			var left = PersistenceContext.Load<ExternalPractitioner>(request.LeftPractitionerRef, EntityLoadFlags.Proxy);
			var right = PersistenceContext.Load<ExternalPractitioner>(request.RightPractitionerRef, EntityLoadFlags.Proxy);

			var nameAssembler = new PersonNameAssembler();
			var name = new PersonName();
			nameAssembler.UpdatePersonName(request.Name, name);

			var defaultContactPoint = request.DefaultContactPointRef != null ?
				PersistenceContext.Load<ExternalPractitionerContactPoint>(request.DefaultContactPointRef) : null;

			var deactivatedContactPoints = request.DeactivatedContactPointRefs == null ? new List<ExternalPractitionerContactPoint>() :
				CollectionUtils.Map(request.DeactivatedContactPointRefs,
				(EntityRef cpRef) => PersistenceContext.Load<ExternalPractitionerContactPoint>(cpRef));

			var cpReplacements = CollectionUtils.Map(request.ContactPointReplacements ?? (new Dictionary<EntityRef, EntityRef>()),
				kvp => new KeyValuePair<ExternalPractitionerContactPoint, ExternalPractitionerContactPoint>(
							PersistenceContext.Load<ExternalPractitionerContactPoint>(kvp.Key, EntityLoadFlags.Proxy),
							PersistenceContext.Load<ExternalPractitionerContactPoint>(kvp.Value, EntityLoadFlags.Proxy)));

			// compute cost estimate and determine the optimal merge direction
			ExternalPractitioner src, dest;
			var cost = EstimateMergeCost(right, left, deactivatedContactPoints, out dest);
			src = dest.Equals(left) ? right : left;

			// if we are only doing a cost estimate, exit here without modifying any data
			if (request.EstimateCostOnly)
				return new MergeExternalPractitionerResponse(cost);

			// merge the practitioners
			MergePractitionerRecords(dest, src,
				name,
				request.LicenseNumber,
				request.BillingNumber,
				request.ExtendedProperties,
				defaultContactPoint,
				deactivatedContactPoints);

			// update all references
			IList<Order> affectedOrders1;
			IList<Visit> affectedVisits;
			UpdateReferencingOrdersAndVisits(dest, src, out affectedOrders1, out affectedVisits);

			// do contact point replacements in result recipients
			IList<Order> affectedOrders2;
			DoContactPointReplacements(cpReplacements, out affectedOrders2);

			// if user has verify permission, re-verify the dest
			if (Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Admin.Data.ExternalPractitionerVerification))
				dest.MarkVerified();

			PersistenceContext.SynchState();

			var affectedOrders = CollectionUtils.Unique(CollectionUtils.Concat(affectedOrders1, affectedOrders2));
			var assembler = new ExternalPractitionerAssembler();
			return new MergeExternalPractitionerResponse(
				assembler.CreateExternalPractitionerSummary(dest, this.PersistenceContext),
					CollectionUtils.Map(affectedOrders,
						(Order o) => new MergeExternalPractitionerResponse.AffectedOrder
						{
							AccessionNumber = o.AccessionNumber,
							Status = EnumUtils.GetEnumValueInfo(o.Status, PersistenceContext)
						}),
					CollectionUtils.Map(affectedVisits,
						(Visit v) => new MergeExternalPractitionerResponse.AffectedVisit
						{
							VisitNumber = new CompositeIdentifierDetail(v.VisitNumber.Id, EnumUtils.GetEnumValueInfo(v.VisitNumber.AssigningAuthority)),
							Status = EnumUtils.GetEnumValueInfo(v.Status, PersistenceContext)
						})
						);
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

		private long EstimateMergeCost(ExternalPractitioner right, ExternalPractitioner left,
			ICollection<ExternalPractitionerContactPoint> deactivatedContactPoints,
			out ExternalPractitioner optimalDest)
		{
			// TODO: optimize this - if right side returns < N, don't even bother with left side (where N is a small number, maybe 5 or 10)
			var rightOrderCount = QueryOrders<long>(right, PersistenceContext.GetBroker<IOrderBroker>().Count);
			var rightVisitCount = QueryVisits<long>(right, PersistenceContext.GetBroker<IVisitBroker>().CountByVisitPractitioner);
			var leftOrderCount = QueryOrders<long>(left, PersistenceContext.GetBroker<IOrderBroker>().Count);
			var leftVisitCount = QueryVisits<long>(left, PersistenceContext.GetBroker<IVisitBroker>().CountByVisitPractitioner);

			// total number of references
			var r = rightOrderCount + rightVisitCount;
			var l = leftOrderCount + leftVisitCount;

			// determine the optimal merge direction
			// choose the side with the higher number of total references to be the dest
			optimalDest = (r > l) ? right : left;

			// minimum number of practitioner references that must be updated
			var pracCount = Math.Min(r, l);

			// number of contact point references that must be updated
			var cpCount = deactivatedContactPoints.Count == 0 ? 0
				:QueryOrders<long>(deactivatedContactPoints, PersistenceContext.GetBroker<IOrderBroker>().CountByResultRecipient);

			// the cost is measured in terms of the total number of references that must be updated 
			return pracCount + cpCount;
		}

		private long EstimateMergeCost(ExternalPractitionerContactPoint target)
		{
			// number of contact point referneces that must be updated
			return QueryOrders<long>(new[] { target }, PersistenceContext.GetBroker<IOrderBroker>().CountByResultRecipient);
		}

		private void MergePractitionerRecords(
			ExternalPractitioner dest,
			ExternalPractitioner src,
			PersonName name,
			string licenseNumber,
			string billingNumber,
			IDictionary<string, string> extendedProperties,
			ExternalPractitionerContactPoint defaultContactPoint,
			ICollection<ExternalPractitionerContactPoint> deactivatedContactPoints)
		{
			// update properties on dest record
			dest.Name = name;
			dest.LicenseNumber = licenseNumber;
			dest.BillingNumber = billingNumber;

			ExtendedPropertyUtils.Update(dest.ExtendedProperties, extendedProperties);

			// move all contact points to dest
			foreach (var cp in src.ContactPoints)
			{
				cp.Practitioner = dest;
			}
			dest.ContactPoints.AddAll(src.ContactPoints);

			// set contact point default/deactivated status appropriately
			foreach (var cp in dest.ContactPoints)
			{
				cp.IsDefaultContactPoint = cp.Equals(defaultContactPoint);
				cp.Deactivated = deactivatedContactPoints.Contains(cp);
			}

			// mark both src and dest as edited
			src.MarkEdited();
			dest.MarkEdited();

			// de-activate source
			src.Deactivated = true;
		}

		private void MergeContactPointRecords(ExternalPractitionerContactPoint dest, ExternalPractitionerContactPoint src)
		{
			// merge the value collections so that the history of src is preserved on dest
			// JR: the collection merging was added in #6823 - I'm reverting it because I don't think it was a good idea
			// since this is more a "replace" operation rather than a true merge operation
			//MergeValueCollection(dest.TelephoneNumbers, src.TelephoneNumbers, (x, y) => x.IsSameNumber(y), x => x.ValidRange);
			//MergeValueCollection(dest.Addresses, src.Addresses, (x, y) => x.IsSameAddress(y), x => x.ValidRange);
			//MergeValueCollection(dest.EmailAddresses, src.EmailAddresses, (x, y) => x.IsSameEmailAddress(y), x => x.ValidRange);

			if (src.IsDefaultContactPoint)
			{
				src.IsDefaultContactPoint = false;
				dest.IsDefaultContactPoint = true;
			}

			// de-activate the src
			src.Deactivated = true;

			// mark the practitioner as being edited (same practitioner for both)
			src.Practitioner.MarkEdited();
		}

		private void UpdateReferencingOrdersAndVisits(ExternalPractitioner dest, ExternalPractitioner src, out IList<Order> affectedOrders, out IList<Visit> affectedVisits)
		{
			// move all referenced orders to the dest
			var orders = QueryOrders<IList<Order>>(src, PersistenceContext.GetBroker<IOrderBroker>().Find);
			foreach (var order in orders)
			{
				order.OrderingPractitioner = dest;
			}

			// move all referenced visits to the dest
			var visits = QueryVisits<IList<Visit>>(src, PersistenceContext.GetBroker<IVisitBroker>().FindByVisitPractitioner);
			foreach (var visit in visits)
			{
				foreach (var visitPractitioner in visit.Practitioners)
				{
					if (visitPractitioner.Practitioner.Equals(src))
						visitPractitioner.Practitioner = dest;
				}
			}

			affectedOrders = CollectionUtils.Unique(orders);
			affectedVisits = CollectionUtils.Unique(visits);
		}

		private void DoContactPointReplacements(IDictionary<ExternalPractitionerContactPoint, ExternalPractitionerContactPoint> replacements, out IList<Order> affectedOrders)
		{
			if(replacements.Count == 0)
			{
				affectedOrders = new List<Order>();
				return;
			}

			// find all orders that require a cp replacement
			var orders = QueryOrders<IList<Order>>(replacements.Keys, PersistenceContext.GetBroker<IOrderBroker>().FindByResultRecipient);

			// do replacements
			foreach (var order in orders)
			{
				foreach (var kvp in replacements)
				{
					foreach (var rr in order.ResultRecipients)
					{
						if (rr.PractitionerContactPoint.Equals(kvp.Key))
							rr.PractitionerContactPoint = kvp.Value;
					}
				}
			}

			// return list of affected orders
			affectedOrders = CollectionUtils.Unique(orders);
		}


		private static T QueryOrders<T>(ExternalPractitioner practitioner, Converter<OrderSearchCriteria, T> queryAction)
		{
			var ordersWhere = new OrderSearchCriteria();
			ordersWhere.OrderingPractitioner.EqualTo(practitioner);
			return queryAction(ordersWhere);
		}

		private static T QueryVisits<T>(ExternalPractitioner practitioner, Converter<VisitSearchCriteria, VisitPractitionerSearchCriteria, T> queryAction)
		{
			var visitsWhere = new VisitPractitionerSearchCriteria();
			visitsWhere.Practitioner.EqualTo(practitioner);
			return queryAction(new VisitSearchCriteria(), visitsWhere);
		}

		private static T QueryOrders<T>(ICollection<ExternalPractitionerContactPoint> contactPoints, Converter<OrderSearchCriteria, ResultRecipientSearchCriteria, T> queryAction)
		{
			return QueryOrders(contactPoints, false, queryAction);
		}

		private static T QueryOrders<T>(ICollection<ExternalPractitionerContactPoint> contactPoints, bool activeOnly, Converter<OrderSearchCriteria, ResultRecipientSearchCriteria, T> queryAction)
		{
			var recipientCriteria = new ResultRecipientSearchCriteria();
			recipientCriteria.PractitionerContactPoint.In(contactPoints);

			var orderCriteria = new OrderSearchCriteria();
			if(activeOnly)
			{
				// Active order search criteria
				orderCriteria.Status.In(new[] { OrderStatus.SC, OrderStatus.IP });
			}

			return queryAction(orderCriteria, recipientCriteria);
		}

		private static void MergeValueCollection<T>(ICollection<T> dest, IEnumerable<T> src,
			Converter<T, T, bool> isSameFunction,
			Converter<T, DateTimeRange> rangeFunction)
			where T : ICloneable
		{
			foreach (var srcItem in src)
			{
				if (CollectionUtils.Contains(dest, destItem => isSameFunction(destItem, srcItem)))
					continue;

				var copy = (T)srcItem.Clone();
				var validRange = rangeFunction(copy);
				validRange.Until = validRange.Until ?? Platform.Time;
				dest.Add(copy);
			}
		}

		private void EnsureNoDeactivatedContactPointsWithActiveOrders(List<ExternalPractitionerContactPointDetail> details)
		{
			var broker = this.PersistenceContext.GetBroker<IExternalPractitionerContactPointBroker>();
			var contactPointsWithOrders = CollectionUtils.Select(
				details,
				detail =>
				{
					if (detail.ContactPointRef == null)
						return false;  // a new contact point will not have associated orders.q

					var contactPoint = broker.Load(detail.ContactPointRef);
					return contactPoint.Deactivated == false
						&& detail.Deactivated
						&& QueryOrders<long>(new[] { contactPoint }, true, PersistenceContext.GetBroker<IOrderBroker>().CountByResultRecipient) > 0;
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
