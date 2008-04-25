using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare
{
    public abstract class OrderNotebox : Notebox
    {
        public IOrderNoteboxItemBroker GetBroker(INoteboxQueryContext nqc)
        {
            return nqc.GetBroker<IOrderNoteboxItemBroker>();
        }

    }

    [ExtensionOf(typeof(NoteboxExtensionPoint))]
    public class OrderNoteInbox : OrderNotebox
    {
        public override NoteboxItemSearchCriteria[] GetInvariantCriteria(INoteboxQueryContext wqc)
        {
            return new NoteboxItemSearchCriteria[]{};
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

    [ExtensionOf(typeof(NoteboxExtensionPoint))]
    public class OrderNoteSentItems : OrderNotebox
    {
        public override NoteboxItemSearchCriteria[] GetInvariantCriteria(INoteboxQueryContext wqc)
        {
            return new NoteboxItemSearchCriteria[] { };
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
