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
        protected static readonly HqlSelect SelectNoteReadAcknowledged = new HqlSelect("nr.IsAcknowledged");
        protected static readonly HqlSelect SelectOrder = new HqlSelect("o");
        protected static readonly HqlSelect SelectPatient = new HqlSelect("p");
        protected static readonly HqlSelect SelectPatientProfile = new HqlSelect("pp");

        protected static readonly HqlJoin JoinOrder = new HqlJoin("n.Order", "o");
        protected static readonly HqlJoin JoinPatient = new HqlJoin("o.Patient", "p");
        protected static readonly HqlJoin JoinPatientProfile = new HqlJoin("p.Profiles", "pp");
        protected static readonly HqlJoin JoinNoteReads = new HqlJoin("n.ReadActivities", "nr");
        protected static readonly HqlJoin FetchJoinNoteReads = new HqlJoin("n.ReadActivities", "nr", HqlJoinMode.Inner, true);
        protected static readonly HqlJoin FetchJoinNoteReadsRecipStaff = new HqlJoin("n.ReadActivities.Recipient.Staff", null, HqlJoinMode.Inner, true);
        protected static readonly HqlJoin FetchJoinNoteReadsRecipGroup = new HqlJoin("n.ReadActivities.Recipient.Group", null, HqlJoinMode.Inner, true);
        protected static readonly HqlJoin FetchJoinNoteReadsAckStaff = new HqlJoin("n.ReadActivities.AcknowledgedBy.Staff", null, HqlJoinMode.Inner, true);

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

        private static readonly HqlJoin[] SentJoins
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
            HqlProjectionQuery query = BuildInboxQuery(nqc, false);
            return DoQuery(query);
        }

        public int CountInboxItems(Notebox notebox, INoteboxQueryContext nqc)
        {
            HqlProjectionQuery query = BuildInboxQuery(nqc, true);
            return DoQueryCount(query);
        }

        public IList GetSentItems(Notebox notebox, INoteboxQueryContext nqc)
        {
            HqlProjectionQuery query = BuildSentQuery(nqc, false);
            List<OrderNoteboxItem> results = DoQuery(query);

            // because the "sent items" query involves a fetch join to a child collection,
            // it needs to be filtered for unique items
            return CollectionUtils.Unique(results, new SentItemEqualityComparer());
        }

        public int CountSentItems(Notebox notebox, INoteboxQueryContext nqc)
        {
            HqlProjectionQuery query = BuildSentQuery(nqc, true);
            return DoQueryCount(query);
        }

        #endregion

        #region Helpers

        private HqlProjectionQuery BuildInboxQuery(INoteboxQueryContext nqc, bool countQuery)
        {
            HqlProjectionQuery query = new HqlProjectionQuery(new HqlFrom(typeof(OrderNote).Name, "n", InboxJoins));
            query.SelectDistinct = true;    // need this in case note was sent to staff and staffgroup containing same staff
            if (countQuery)
                query.Selects.AddRange(CountProjection);
            else
                query.Selects.AddRange(InboxItemProjection);

            HqlOr or = new HqlOr();
            or.Conditions.Add(new HqlCondition("nr.Recipient.Staff = ?", nqc.Staff));
            or.Conditions.Add(new HqlCondition("nr.Recipient.Group in (select elements(s.Groups) from Staff s where s = ?)", nqc.Staff));
            query.Conditions.Add(or);
            query.Conditions.Add(new HqlCondition("nr.IsAcknowledged = ?", false));

            query.Conditions.Add(ConditionConstrainPatientProfile);

            // add paging if not a count query
            if (!countQuery)
            {
                query.Page = nqc.Page;
            }

            return query;
        }

        private HqlProjectionQuery BuildSentQuery(INoteboxQueryContext nqc, bool countQuery)
        {
            HqlProjectionQuery query = new HqlProjectionQuery(new HqlFrom(typeof(OrderNote).Name, "n", SentJoins));
            if (countQuery)
            {
                query.Selects.AddRange(CountProjection);
            }
            else
            {
                query.Selects.AddRange(SentItemProjection);

                // add fetch join for note-reads and related entities
                // (we don't do this in the 'count' query because it would throw off the numbers)
                query.Froms[0].Joins.Add(FetchJoinNoteReads);
                //query.Froms[0].Joins.Add(FetchJoinNoteReadsRecipStaff);
                //query.Froms[0].Joins.Add(FetchJoinNoteReadsRecipGroup);
                //query.Froms[0].Joins.Add(FetchJoinNoteReadsAckStaff);
            }

            query.Conditions.Add(new HqlCondition("n.Author = ?", nqc.Staff));
            //query.Conditions.Add(new HqlCondition("where ? = all elements(nr.IsAcknowledged)", true));

            query.Conditions.Add(ConditionConstrainPatientProfile);

            // add paging if not a count query
            if (!countQuery)
            {
                query.Page = nqc.Page;
            }

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
