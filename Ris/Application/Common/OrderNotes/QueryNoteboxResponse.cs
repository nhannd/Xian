#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Serialization;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.OrderNotes
{
    [DataContract]
    public class QueryNoteboxResponse : DataContractBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="noteboxItems"></param>
        /// <param name="itemCount"></param>
        public QueryNoteboxResponse(List<OrderNoteboxItemSummary> noteboxItems, int itemCount)
        {
            NoteboxItems = noteboxItems;
            ItemCount = itemCount;
        }

        /// <summary>
        /// List of notebox items.
        /// </summary>
        [DataMember]
        public List<OrderNoteboxItemSummary> NoteboxItems;

        /// <summary>
        /// Count of total items in the notebox.
        /// </summary>
        [DataMember]
        public int ItemCount;
    }
}
