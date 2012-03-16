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
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;
using ClearCanvas.Ris.Application.Common.Admin;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
    [DataContract]
    public class PlaceOrderRequest : DataContractBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="requisition"></param>
        public PlaceOrderRequest(OrderRequisition requisition)
        {
            this.Requisition = requisition;
        }

        /// <summary>
        /// Requisition specifying details of the order to place.
        /// </summary>
        [DataMember]
        public OrderRequisition Requisition;
    }
}