#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using System.ServiceModel;
using System;

namespace ClearCanvas.Web.Common
{
	[DataContract(Namespace = Namespace.Value)]
	public abstract class StartApplicationRequest
	{
		[DataMember(IsRequired = true)]
		public Guid Identifier { get; set; }

		[DataMember(IsRequired = false)]
		public string Username { get; set; }

		[DataMember(IsRequired = false)]
		public string SessionId { get; set; }

		[DataMember(IsRequired = false)]
		public bool IsSessionShared { get; set; }
	}

    [DataContract(Namespace = Namespace.Value)]
    public class StartApplicationRequestResponse
    {
        [DataMember(IsRequired = true)]
        public Guid AppIdentifier { get; set; }

    }
    
	[DataContract(Namespace = Namespace.Value)]
	public class StopApplicationRequest
	{
		[DataMember(IsRequired = true)]
		public Guid ApplicationId { get; set; }
	}

    [DataContract(Namespace = Namespace.Value)]
    public class GetPendingEventRequest
	{
		[DataMember(IsRequired = true)]
		public Guid ApplicationId { get; set; }

        [DataMember(IsRequired = false)]
        public int MaxWaitTime { get; set; }
	}

    [ServiceContract(Namespace = Namespace.Value)]
	[ServiceKnownType("GetKnownTypes", typeof(ServiceKnownTypeExtensionPoint))]
	public interface IApplicationService
    {
		[OperationContract(IsOneWay = false)]
        [FaultContract(typeof(SessionValidationFault))]
        StartApplicationRequestResponse StartApplication(StartApplicationRequest request);

		[OperationContract(IsOneWay = false)]
        [FaultContract(typeof(InvalidOperationFault))]
        void StopApplication(StopApplicationRequest request);

        [OperationContract(IsOneWay = false)]
        [FaultContract(typeof(InvalidOperationFault))]
        ProcessMessagesResult ProcessMessages(MessageSet messages);
        
        [OperationContract(IsOneWay = false)]
        [FaultContract(typeof(InvalidOperationFault))]
        EventSet GetPendingEvent(GetPendingEventRequest request);

        [OperationContract(IsOneWay = true)]
        void ReportPerformance(PerformanceData data);

        [OperationContract(IsOneWay = false)]
        void SetProperty(SetPropertyRequest request);
    }

    [DataContract]
    public class InvalidOperationFault
    {
    }

    [DataContract]
    public class SetPropertyRequest
    {
        [DataMember(IsRequired = true)]
        public Guid ApplicationId { get; set; }

        [DataMember]
        public string Key { get; set; }

        [DataMember]
        public string Value { get; set; }
    }
}
