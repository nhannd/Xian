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
        protected static readonly HqlSelect SelectNoteReadAcknowledged = new HqlSelect("np.IsAcknowledged");
        protected static readonly HqlSelect SelectOrder = new HqlSelect("o");
        protected static readonly HqlSelect SelectPatient = new HqlSelect("p");
        protected static readonly HqlSelect SelectPatientProfile = new HqlSelect("pp");

        protected static readonly HqlJoin JoinOrder = new HqlJoin("n.Order", "o");
        protected static readonly HqlJoin JoinPatient = new HqlJoin("o.Patient", "p");
        protected static readonly HqlJoin JoinPatientProfile = new HqlJoin("p.Profiles", "pp");
        protected static readonly HqlJoin JoinNoteReads = new HqlJoin("n.Postings", "np");
        protected static readonly HqlJoin FetchJoinNoteReads = new HqlJoin("n.Postings", "np", HqlJoinMode.Inner, true);

        protected static readonly HqlCondition ConditionConstrainPatientProfile =
            new HqlCondition("pp.Mrn.AssigningAuthority = o.OrderingFacility.InformationAuthority");

        private static readonly HqlSelect[] InboxItemProjection
            = {
                SelectNote,
                SelectOrder,
                SelectPatient,
                SelectPatientProfile,
                SelectNoteReadAcknowledged
              };

        private static readonly HqlSelect[] SentItemProjection
            = {
                SelectNote,
                SelectOrder,
                SelectPatient,
                SelectPatientProfile
              };

        private static readonly HqlSelect[] CountProjection
            = {
                  new HqlSelect("count(distinct n)")    // count distinct n (in case note sent to staff and staffgroup containing same staff)
              };

        private static readonly HqlJoin[] InboxJoins
            = {
                JoinNoteReads,
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

        #endregion

        #region SentItemEqualityComparer class

        class SentItemEqualityComparer : IEqualityComparer<OrderNoteboxItem>
        {

            #region IEqualityComparer Members

            public bool Equals(OrderNoteboxItem x, OrderNoteboxItem y)
            {
                return x.OrderNoteRef.Equals(y.OrderNoteRef, true);
            }
            public int GetHashCode(OrderNoteboxItem obj)
            {
                return obj.OrderNoteRef.GetHashCode();
            }

            #endregion
        }

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
            List<OrderNoteboxItem> results = DoQuery(query);

            // because the "sent items" query involves a fetch join to a child collection,
            // it needs to be filtered for unique items
            return CollectionUtils.Unique(results, new SentItemEqualityComparer());
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
                query.SelectDistinct = true;

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
                    and.Conditions.Add(new HqlCondition("np.Recipient.Staff = ?", nqc.Staff));
                if(criteria.SentToGroupIncludingMe)
                    and.Conditions.Add(new HqlCondition("np.Recipient.Group in (select elements(s.Groups) from Staff s where s = ?)", nqc.Staff));
                
                or.Conditions.Add(and);
            }
            query.Conditions.Add(or);

            return query;
        }

        private HqlProjectionQuery BuildSentQuery(Notebox notebox, INoteboxQueryContext nqc, bool countQuery)
        {
            HqlProjectionQuery query = GetBaseQuery(nqc, countQuery, SentItemProjection, SentItemJoins);

            // potential optimization by fetch joining the note-reads
            // not sure if this actually improves performance or not
            //if (!countQuery)
            //    query.Froms[0].Joins.Add(FetchJoinNoteReads);

            HqlOr or = new HqlOr();
            foreach (NoteboxItemSearchCriteria criteria in notebox.GetInvariantCriteria(nqc))
            {
                HqlAnd and = new HqlAnd();

                // for sent items, IsAcknowledged means fully acknowledged (all readers have acknowledged)
                if(criteria.IsAcknowledged)
                {
                    // condition is that *all* nr are acknowledged
                    and.Conditions.Add(new HqlCondition("not exists (select np1.IsAcknowledged from NotePosting np1 where np1.Note = n and np1.IsAcknowledged = ?)", false));
                }
                else
                {
                    // condition is that *any* nr is not acknowledged
                    and.Conditions.Add(new HqlCondition("exists (select np1.IsAcknowledged from NotePosting np1 where np1.Note = n and np1.IsAcknowledged = ?)", false));
                }

                if (criteria.SentByMe)
                    and.Conditions.Add(new HqlCondition("n.Author = ?", nqc.Staff));

                or.Conditions.Add(and);
            }
            query.Conditions.Add(or);

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
