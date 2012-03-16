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
    public class AcknowledgeAndPostResponse : DataContractBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="orderNote"></param>
        public AcknowledgeAndPostResponse(OrderNoteDetail orderNote)
        {
            OrderNote = orderNote;
        }

        /// <summary>
        /// Returns the order note that was created as a result of the request.
        /// </summary>
        [DataMember]
        public OrderNoteDetail OrderNote;
    }
}
