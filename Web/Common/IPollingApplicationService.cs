#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.ServiceModel;

namespace ClearCanvas.Web.Common
{
    [ServiceContract(Namespace = Namespace.Value)]
    public interface IPollingApplicationService : IApplicationService
    {
        [OperationContract(IsOneWay = false)]
        [FaultContract(typeof(InvalidOperationFault))]
        ProcessMessagesResult ProcessMessages(MessageSet messages);

        [OperationContract(IsOneWay = false)]
        [FaultContract(typeof(InvalidOperationFault))]
        GetPendingEventsResult GetPendingEvents(GetPendingEventsRequest request);
    }
}