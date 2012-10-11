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
    public interface IApplicationServiceCallback
    {
        [OperationContract(IsOneWay = false)]
        void ProcessEvents(ProcessEventsRequest request);
    }

    [ServiceContract(Namespace = Namespace.Value, CallbackContract = typeof(IApplicationServiceCallback))]
    public interface IDuplexApplicationService : IApplicationService
    {
        [OperationContract(IsOneWay = false)]
        [FaultContract(typeof(InvalidOperationFault))]
        void ProcessMessages(MessageSet messages);
    }
}