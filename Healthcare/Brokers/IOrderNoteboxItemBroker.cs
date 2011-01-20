#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
