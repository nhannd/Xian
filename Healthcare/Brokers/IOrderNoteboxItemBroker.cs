using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Brokers
{
    public interface IOrderNoteboxItemBroker : IPersistenceBroker
    {
        IList GetSentItems(Notebox notebox, INoteboxQueryContext nqc);

        IList GetInboxItems(Notebox notebox, INoteboxQueryContext nqc);

        int CountSentItems(Notebox notebox, INoteboxQueryContext nqc);

        int CountInboxItems(Notebox notebox, INoteboxQueryContext nqc);
    }
}
