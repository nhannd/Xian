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
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	[ExtensionOf(typeof(BrokerExtensionPoint))]
	public class OrderNoteboxItemBroker : Broker, IOrderNoteboxItemBroker
	{
		#region Hql Constants

		protected static readonly HqlSelect SelectNote = new HqlSelect("n");
		protected static readonly HqlSelect SelectNoteAuthor = new HqlSelect("s");
		protected static readonly HqlSelect SelectNotePostingAcknowledged = new HqlSelect("np.IsAcknowledged");
		protected static readonly HqlSelect SelectNoteFullyAcknowledged = new HqlSelect("n.IsFullyAcknowledged");
		protected static readonly HqlSelect SelectOrder = new HqlSelect("o");
		protected static readonly HqlSelect SelectOrderingFacilityInformationAuthority = new HqlSelect("ia");
		protected static readonly HqlSelect SelectOrderDiagnosticServiceName = new HqlSelect("ds.Name");
		protected static readonly HqlSelect SelectPatient = new HqlSelect("p");
		protected static readonly HqlSelect SelectPatientProfile = new HqlSelect("pp");

		protected static readonly HqlJoin JoinNote = new HqlJoin("np.Note", "n", HqlJoinMode.Left);
		protected static readonly HqlJoin JoinOrder = new HqlJoin("n.Order", "o", HqlJoinMode.Left);
		protected static readonly HqlJoin JoinPatient = new HqlJoin("o.Patient", "p", HqlJoinMode.Left);
		protected static readonly HqlJoin JoinPatientProfile = new HqlJoin("p.Profiles", "pp");
		protected static readonly HqlJoin JoinStaff = new HqlJoin("n.Author", "s", HqlJoinMode.Left);
		protected static readonly HqlJoin JoinOrderingFacility = new HqlJoin("o.OrderingFacility", "f", HqlJoinMode.Left);
		protected static readonly HqlJoin JoinInformationAuthority = new HqlJoin("f.InformationAuthority", "ia", HqlJoinMode.Left);
		protected static readonly HqlJoin JoinOrderingDiagnosticService = new HqlJoin("o.DiagnosticService", "ds", HqlJoinMode.Left);

		//protected static readonly HqlCondition ConditionMostRecentNote = new HqlCondition(
		//    "(n.PostTime = (select max(n2.PostTime) from OrderNote n2 join n2.Postings np2 where np2 = np and n2.Order = n.Order and n2.Category = n.Category))");

		/// <summary>
		/// This class is used internally by this broker only.
		/// </summary>
		class NoteboxItem
		{
			public NoteboxItem(Note note, Order order, Patient patient, Staff author,
				string diagnosticServiceName, bool isAcknowledged, InformationAuthorityEnum informationAuthority)
			{
				this.Note = note;
				this.Order = order;
				this.Patient = patient;
				this.Author = author;
				this.DiagnosticServiceName = diagnosticServiceName;
				this.NotePostingAcknowledged = isAcknowledged;
				this.OrderingFacilityInformationAuthority = informationAuthority;
			}

			public Note Note { get; private set; }

			public Order Order { get; private set; }

			public Patient Patient { get; private set; }

			public Staff Author { get; private set; }

			public string DiagnosticServiceName { get; private set; }

			public bool NotePostingAcknowledged { get; private set; }

			public InformationAuthorityEnum OrderingFacilityInformationAuthority { get; private set; }
		}

		private static readonly HqlSelect[] InboxItemProjection
			= {
				SelectNote,
				SelectOrder,
				SelectPatient,
				SelectNoteAuthor,
				SelectOrderDiagnosticServiceName,
				SelectNotePostingAcknowledged,
				SelectOrderingFacilityInformationAuthority
			  };

		private static readonly HqlSelect[] SentItemProjection
			= {
				SelectNote,
				SelectOrder,
				SelectPatient,
				SelectNoteAuthor,
				SelectOrderDiagnosticServiceName,
				SelectNoteFullyAcknowledged,
				SelectOrderingFacilityInformationAuthority
			  };

		private static readonly HqlSelect[] CountProjection
			= {
				  new HqlSelect("count(*)")
			  };

		private static readonly HqlJoin[] InboxItemsJoins
			= {
				JoinNote,
				JoinOrder,
				JoinOrderingDiagnosticService,
				JoinOrderingFacility,
				JoinInformationAuthority,
				JoinPatient,
				JoinStaff
			  };

		private static readonly HqlJoin[] SentItemsJoins
			= {
				JoinOrder,
				JoinOrderingDiagnosticService,
				JoinOrderingFacility,
				JoinInformationAuthority,
				JoinPatient,
				JoinStaff
			  };

		private static readonly HqlSort[] InboxItemOrdering
			= {
				new HqlSort("np.CreationTime", false, 1)
			  };

		private static readonly HqlSort[] SentItemOrdering
			= {
				new HqlSort("n.PostTime", false, 1)
			  };

		#endregion

		#region IOrderNoteboxItemBroker Members

		public IList GetInboxItems(Notebox notebox, INoteboxQueryContext nqc)
		{
			var query = BuildInboxQuery(notebox, nqc, false);
			return DoQuery(query);
		}

		public int CountInboxItems(Notebox notebox, INoteboxQueryContext nqc)
		{
			var query = BuildInboxQuery(notebox, nqc, true);
			return DoQueryCount(query);
		}

		public IList GetSentItems(Notebox notebox, INoteboxQueryContext nqc)
		{
			var query = BuildSentQuery(notebox, nqc, false);
			return DoQuery(query);
		}

		public int CountSentItems(Notebox notebox, INoteboxQueryContext nqc)
		{
			var query = BuildSentQuery(notebox, nqc, true);
			return DoQueryCount(query);
		}

		#endregion

		#region Helpers

		private static HqlProjectionQuery GetBaseQuery(INoteboxQueryContext nqc, bool countQuery, 
			string entityAlias, Type fromEntityType, HqlSelect[] itemProjection, HqlJoin[] itemJoins)
		{
			var joins = countQuery ? new HqlJoin[] { } : itemJoins;
			var query = new HqlProjectionQuery(new HqlFrom(fromEntityType.Name, entityAlias, joins));

			if (countQuery)
			{
				query.Selects.AddRange(CountProjection);
			}
			else
			{
				query.Selects.AddRange(itemProjection);

				// need this in case orderNote was sent to staff and staffgroup containing same staff
				//query.SelectDistinct = true;

				// add paging if not a count query
				query.Page = nqc.Page;
			}

			return query;
		}

		private static HqlProjectionQuery BuildInboxQuery(Notebox notebox, INoteboxQueryContext nqc, bool countQuery)
		{
			var query = GetBaseQuery(nqc, countQuery, "np", typeof(NotePosting), InboxItemProjection, InboxItemsJoins);

			var or = new HqlOr();
			foreach (var criteria in notebox.GetInvariantCriteria(nqc))
			{
				var and = new HqlAnd();
				and.Conditions.Add(new HqlCondition("np.IsAcknowledged = ?", criteria.IsAcknowledged));

				if (criteria.SentToMe)
					and.Conditions.Add(new HqlCondition("np.Recipient = ?", nqc.Staff));
				if (criteria.SentToGroupIncludingMe)
					and.Conditions.Add(new HqlCondition("np.Recipient = ?", nqc.StaffGroup));
				
				or.Conditions.Add(and);
			}
			query.Conditions.Add(or);
			//query.Conditions.Add(ConditionMostRecentNote);

			if (!countQuery)
				query.Sorts.AddRange(InboxItemOrdering);

			return query;
		}

		private static HqlProjectionQuery BuildSentQuery(Notebox notebox, INoteboxQueryContext nqc, bool countQuery)
		{
			var query = GetBaseQuery(nqc, countQuery, "n", typeof(Note), SentItemProjection, SentItemsJoins);

			var or = new HqlOr();
			foreach (var criteria in notebox.GetInvariantCriteria(nqc))
			{
				var and = new HqlAnd();

				// for sent items, IsAcknowledged means fully acknowledged (all readers have acknowledged)
				and.Conditions.Add(new HqlCondition("n.IsFullyAcknowledged = ?", criteria.IsAcknowledged));
				and.Conditions.Add(new HqlCondition("n.HasPostings = ?", true));

				if (criteria.SentByMe)
					and.Conditions.Add(new HqlCondition("n.Author = ?", nqc.Staff));

				or.Conditions.Add(and);
			}
			query.Conditions.Add(or);

			if (!countQuery)
				query.Sorts.AddRange(SentItemOrdering);

			return query;
		}

		protected List<OrderNoteboxItem> DoQuery(HqlProjectionQuery query)
		{
			var list = ExecuteHql<object[]>(query);
			var results = new List<NoteboxItem>();
			foreach (var tuple in list)
			{
				var item = (NoteboxItem)Activator.CreateInstance(typeof(NoteboxItem), tuple);
				results.Add(item);
			}

			return BuildOrderNoteboxItems(results);
		}

		protected int DoQueryCount(HqlQuery query)
		{
			return (int)ExecuteHqlUnique<long>(query);
		}

		private List<OrderNoteboxItem> BuildOrderNoteboxItems(List<NoteboxItem> inboxItems)
		{
			if (inboxItems.Count == 0)
				return new List<OrderNoteboxItem>();

			// Get all the patients for all the items
			var patients = CollectionUtils.Unique(CollectionUtils.Map<NoteboxItem, Patient>(inboxItems, item => item.Patient));
			var patientQuery = new HqlProjectionQuery(new HqlFrom(typeof(PatientProfile).Name, "pp"));
			var patientCriteria = new PatientProfileSearchCriteria();
			patientCriteria.Patient.In(patients);
			patientQuery.Conditions.AddRange(HqlCondition.FromSearchCriteria("pp", patientCriteria));
			var profiles = ExecuteHql<PatientProfile>(patientQuery);

			// Have to manually get the postings (and later their recipients) to work around a Hibernate fetch="subselect" issue.
			// The subselect somehow removed the "top(100)" and "order by" clause.  Making the query slow.
			// Load all the postings for all the notes.  There may be more than one postings per orderNote.
			// Therefore it is inappropriate to just use the postings in the base query.
			var notes = CollectionUtils.Unique(CollectionUtils.Map<NoteboxItem, Note>(inboxItems, item => item.Note));
			var postingQuery = new HqlProjectionQuery(new HqlFrom(typeof(NotePosting).Name, "np"));
			var postingCriteria = new NotePostingSearchCriteria();
			postingCriteria.Note.In(notes);
			postingQuery.Conditions.AddRange(HqlCondition.FromSearchCriteria("np", postingCriteria));
			postingQuery.Froms[0].Joins.Add(new HqlJoin("np.Recipient", null, HqlJoinMode.Left, true));
			var postings = ExecuteHql<NotePosting>(postingQuery);

			// Build order notebox items
			var orderNoteboxItems = CollectionUtils.Map(inboxItems,
				delegate(NoteboxItem item)
				{
					// Find the appropriate patient profile based on OrderingFacility
					var profile = CollectionUtils.SelectFirst(profiles,
						pp => pp.Patient.Equals(item.Patient)
							&& pp.Mrn.AssigningAuthority.Code.Equals(item.OrderingFacilityInformationAuthority.Code));

					// Find all the recipients
					var postingsForThisNote = CollectionUtils.Select(postings, np => np.Note.Equals(item.Note));
					var recipients = CollectionUtils.Map<NotePosting, object>(postingsForThisNote,
						posting => posting is StaffNotePosting
							? (object)((StaffNotePosting)posting).Recipient
							: (object)((GroupNotePosting)posting).Recipient);

					return new OrderNoteboxItem(item.Note, item.Order, item.Patient, profile, item.Author, recipients,
						item.DiagnosticServiceName, item.NotePostingAcknowledged);
				});

			return orderNoteboxItems;
		}

		#endregion
	}
}
