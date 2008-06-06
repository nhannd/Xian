using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    [ExtensionOf(typeof(BrokerExtensionPoint))]
    public class OrderNoteboxItemBroker : Broker, IOrderNoteboxItemBroker
    {
        #region Hql Constants

		protected static readonly HqlSelect SelectNote = new HqlSelect("n");
		protected static readonly HqlSelect SelectNotePostingAcknowledged = new HqlSelect("np.IsAcknowledged");
        protected static readonly HqlSelect SelectNoteFullyAcknowledged = new HqlSelect("n.IsFullyAcknowledged");
        protected static readonly HqlSelect SelectOrder = new HqlSelect("o");
        protected static readonly HqlSelect SelectPatient = new HqlSelect("p");
        protected static readonly HqlSelect SelectPatientProfile = new HqlSelect("pp");

        protected static readonly HqlJoin JoinOrder = new HqlJoin("n.Order", "o");
        protected static readonly HqlJoin JoinPatient = new HqlJoin("o.Patient", "p");
        protected static readonly HqlJoin JoinPatientProfile = new HqlJoin("p.Profiles", "pp");
        protected static readonly HqlJoin JoinNotePostings = new HqlJoin("n.Postings", "np");



        protected static readonly HqlCondition ConditionConstrainPatientProfile =
            new HqlCondition("pp.Mrn.AssigningAuthority = o.OrderingFacility.InformationAuthority");

        private static readonly HqlSelect[] InboxItemProjection
            = {
				SelectNote,
                SelectOrder,
                SelectPatient,
                SelectPatientProfile,
                SelectNotePostingAcknowledged
              };

        private static readonly HqlSelect[] SentItemProjection
            = {
				SelectNote,
                SelectOrder,
                SelectPatient,
                SelectPatientProfile,
                SelectNoteFullyAcknowledged
              };

        private static readonly HqlSelect[] CountProjection
            = {
                  new HqlSelect("count(*)")
              };

        private static readonly HqlJoin[] InboxJoins
            = {
                JoinNotePostings,
                JoinOrder,
                JoinPatient,
                JoinPatientProfile,
              };

        private static readonly HqlJoin[] SentItemJoins
            = {
                JoinOrder,
                JoinPatient,
                JoinPatientProfile,
              };

        private static readonly HqlSort[] InboxItemOrdering
           = {
                    new HqlSort("np.IsAcknowledged", true, 0),
                    new HqlSort("n.PostTime", false, 1)
              };

        private static readonly HqlSort[] SentItemOrdering
           = {
                    new HqlSort("n.IsFullyAcknowledged", true, 0),
                    new HqlSort("n.PostTime", false, 1)
              };

        #endregion

        #region IOrderNoteboxItemBroker Members

        public IList GetInboxItems(Notebox notebox, INoteboxQueryContext nqc)
        {
            HqlProjectionQuery query = BuildInboxQuery(notebox, nqc, false);
            return DoQuery(query);
        }

        public int CountInboxItems(Notebox notebox, INoteboxQueryContext nqc)
        {
            HqlProjectionQuery query = BuildInboxQuery(notebox, nqc, true);
            return DoQueryCount(query);
        }

        public IList GetSentItems(Notebox notebox, INoteboxQueryContext nqc)
        {
            HqlProjectionQuery query = BuildSentQuery(notebox, nqc, false);
            return DoQuery(query);
        }

        public int CountSentItems(Notebox notebox, INoteboxQueryContext nqc)
        {
            HqlProjectionQuery query = BuildSentQuery(notebox, nqc, true);
            return DoQueryCount(query);
        }

        #endregion

        #region Helpers

        private HqlProjectionQuery GetBaseQuery(INoteboxQueryContext nqc, bool countQuery, HqlSelect[] itemProjection, HqlJoin[] joins)
        {
            HqlProjectionQuery query = new HqlProjectionQuery(new HqlFrom(typeof(OrderNote).Name, "n", joins));
            if (countQuery)
            {
                query.Selects.AddRange(CountProjection);
            }
            else
            {
                query.Selects.AddRange(itemProjection);

                // need this in case note was sent to staff and staffgroup containing same staff
                //query.SelectDistinct = true;

                // add paging if not a count query
                query.Page = nqc.Page;
            }

            // constrain patient profile by OrderingFacility (this is not ideal, but it is the easiest way to constrain the patient profile)
            query.Conditions.Add(ConditionConstrainPatientProfile);

            return query;
        }


        private HqlProjectionQuery BuildInboxQuery(Notebox notebox, INoteboxQueryContext nqc, bool countQuery)
        {
            HqlProjectionQuery query = GetBaseQuery(nqc, countQuery, InboxItemProjection, InboxJoins);

            HqlOr or = new HqlOr();
            foreach (NoteboxItemSearchCriteria criteria in notebox.GetInvariantCriteria(nqc))
            {
                HqlAnd and = new HqlAnd();
                and.Conditions.Add(new HqlCondition("np.IsAcknowledged = ?", criteria.IsAcknowledged));
                if(criteria.SentToMe)
					and.Conditions.Add(new HqlCondition("np = (select np1 from StaffNotePosting np1 where np1 = np and np1.Recipient = ?)", nqc.Staff));
                if(criteria.SentToGroupIncludingMe)
					and.Conditions.Add(new HqlCondition("np = (select np1 from GroupNotePosting np1 where np1 = np and np1.Recipient in (select elements(s.Groups) from Staff s where s = ?))", nqc.Staff));
                
                or.Conditions.Add(and);
            }
            query.Conditions.Add(or);

			if(!countQuery)
	            query.Sorts.AddRange(InboxItemOrdering);

            return query;
        }

        private HqlProjectionQuery BuildSentQuery(Notebox notebox, INoteboxQueryContext nqc, bool countQuery)
        {
            HqlProjectionQuery query = GetBaseQuery(nqc, countQuery, SentItemProjection, SentItemJoins);

            HqlOr or = new HqlOr();
            foreach (NoteboxItemSearchCriteria criteria in notebox.GetInvariantCriteria(nqc))
            {
                HqlAnd and = new HqlAnd();

                // for sent items, IsAcknowledged means fully acknowledged (all readers have acknowledged)
                and.Conditions.Add(new HqlCondition("n.IsFullyAcknowledged = ?", criteria.IsAcknowledged));

                if (criteria.SentByMe)
                    and.Conditions.Add(new HqlCondition("n.Author = ?", nqc.Staff));

                or.Conditions.Add(and);
            }
            query.Conditions.Add(or);
			query.Conditions.Add(new HqlCondition("size(n.Postings) > 0"));

			if(!countQuery)
	            query.Sorts.AddRange(SentItemOrdering);

            return query;
        }

        protected List<OrderNoteboxItem> DoQuery(HqlProjectionQuery query)
        {
            IList<object[]> list = ExecuteHql<object[]>(query);
            List<OrderNoteboxItem> results = new List<OrderNoteboxItem>();
            foreach (object[] tuple in list)
            {
                OrderNoteboxItem item = (OrderNoteboxItem)Activator.CreateInstance(typeof(OrderNoteboxItem), tuple);
                results.Add(item);
            }

            return results;
        }

        protected int DoQueryCount(HqlQuery query)
        {
            return (int)ExecuteHqlUnique<long>(query);
        }

        #endregion
    }
}
