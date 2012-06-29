#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
    [DataContract]
    public class CancelOrderRequest : DataContractBase
    {
        public CancelOrderRequest(EntityRef orderRef, EnumValueInfo cancelReason)
        {
            this.OrderRef = orderRef;
            this.CancelReason = cancelReason;
        }

        /// <summary>
        /// Order to cancel.
        /// </summary>
        [DataMember]
        public EntityRef OrderRef;

        /// <summary>
        /// Reason the order is being cancelled.
        /// </summary>
        [DataMember]
        public EnumValueInfo CancelReason;
    }
}
