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
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Extended.Common.OrderNotes
{
    [DataContract]
    public class AcknowledgeAndPostRequest : DataContractBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="orderRef"></param>
        /// <param name="orderNotesToAcknowledge"></param>
        /// <param name="orderNote"></param>
        public AcknowledgeAndPostRequest(EntityRef orderRef, List<EntityRef> orderNotesToAcknowledge, OrderNoteDetail orderNote)
        {
            OrderRef = orderRef;
            OrderNotesToAcknowledge = orderNotesToAcknowledge;
            OrderNote = orderNote;
        }

        /// <summary>
        /// Specifies the order to which this request pertains.
        /// </summary>
        [DataMember]
        public EntityRef OrderRef;

        /// <summary>
        /// Specifies a list of order notes to acknowledge prior replying. Optional.
        /// </summary>
        [DataMember]
        public List<EntityRef> OrderNotesToAcknowledge;

        /// <summary>
        /// Specifies a reply order note. Optional.
        /// </summary>
        [DataMember]
        public OrderNoteDetail OrderNote;

    }
}
