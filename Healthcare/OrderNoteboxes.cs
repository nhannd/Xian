#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
    public class OrderNotePersonalInbox : OrderNotebox
    {
        public override NoteboxItemSearchCriteria[] GetInvariantCriteria(INoteboxQueryContext wqc)
        {
            NoteboxItemSearchCriteria where = new NoteboxItemSearchCriteria();
            where.IsAcknowledged = false;
            where.SentToMe = true;

            return new NoteboxItemSearchCriteria[]{ where };
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
	/// Defines the order-note "Inbox".
	/// </summary>
	[ExtensionOf(typeof(NoteboxExtensionPoint))]
	public class OrderNoteGroupInbox : OrderNotebox
	{
		public override NoteboxItemSearchCriteria[] GetInvariantCriteria(INoteboxQueryContext wqc)
		{
			NoteboxItemSearchCriteria where = new NoteboxItemSearchCriteria();
			where.IsAcknowledged = false;
			where.SentToGroupIncludingMe = true;

			return new NoteboxItemSearchCriteria[] { where };
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
