using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare
{
    /// <summary>
    /// Abstract base class for order-noteboxes.
    /// </summary>
    public abstract class OrderNotebox : Notebox
    {
        /// <summary>
        /// Helper method to get a broker.
        /// </summary>
        /// <param name="nqc"></param>
        /// <returns></returns>
        protected IOrderNoteboxItemBroker GetBroker(INoteboxQueryContext nqc)
        {
            return nqc.GetBroker<IOrderNoteboxItemBroker>();
        }
    }

    /// <summary>
    /// Defines the order-note "Inbox".
    /// </summary>
    [ExtensionOf(typeof(NoteboxExtensionPoint))]
    public class OrderNoteInbox : OrderNotebox
    {
        public override NoteboxItemSearchCriteria[] GetInvariantCriteria(INoteboxQueryContext wqc)
        {
            NoteboxItemSearchCriteria where1 = new NoteboxItemSearchCriteria();
            where1.IsAcknowledged = false;
            where1.SentToGroupIncludingMe = true;

            NoteboxItemSearchCriteria where2 = new NoteboxItemSearchCriteria();
            where2.IsAcknowledged = false;
            where2.SentToMe = true;


            return new NoteboxItemSearchCriteria[]{ where1, where2 };
        }

        public override IList GetItems(INoteboxQueryContext nqc)
        {
            return GetBroker(nqc).GetInboxItems(this, nqc);
        }

        public override int GetItemCount(INoteboxQueryContext nqc)
        {
            return GetBroker(nqc).CountInboxItems(this, nqc);
        }
    }

    /// <summary>
    /// Defines the order-note "Sent Items" box.
    /// </summary>
    [ExtensionOf(typeof(NoteboxExtensionPoint))]
    public class OrderNoteSentItems : OrderNotebox
    {
        public override NoteboxItemSearchCriteria[] GetInvariantCriteria(INoteboxQueryContext wqc)
        {
            NoteboxItemSearchCriteria where = new NoteboxItemSearchCriteria();
            where.IsAcknowledged = false;
            where.SentByMe = true;

            return new NoteboxItemSearchCriteria[] { where };
        }

        public override IList GetItems(INoteboxQueryContext nqc)
        {
            return GetBroker(nqc).GetSentItems(this, nqc);
        }

        public override int GetItemCount(INoteboxQueryContext nqc)
        {
            return GetBroker(nqc).CountSentItems(this, nqc);
        }
    }
}
