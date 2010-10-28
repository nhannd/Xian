#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
	public class StopApplicationRequest
	{
		[DataMember(IsRequired = true)]
		public Guid ApplicationId { get; set; }
	}

	//[DataContract(Namespace = Namespace.Value)]
	//public class RestoreApplicationRequest
	//{
	//    [DataMember(IsRequired = true)]
	//    public Guid ApplicationIdentifer { get; set; }
	//}

    [ServiceContract(Namespace = Namespace.Value, CallbackContract = typeof(IApplicationServiceCallback), SessionMode = SessionMode.Required)]
	[ServiceKnownType("GetKnownTypes", typeof(ServiceKnownTypeExtensionPoint))]
	public interface IApplicationService
    {
		[OperationContract(IsOneWay = false)]
        [FaultContract(typeof(SessionValidationFault))]
		void StartApplication(StartApplicationRequest request);

		//[OperationContract(IsOneWay = true)]
		//void RestoreApplication(RestoreApplicationRequest request);

		[OperationContract(IsOneWay = true)]
		void StopApplication(StopApplicationRequest request);

        //messages must be process synchronously, so IsOneWay has to be false
        [OperationContract(IsOneWay = false)]
        void ProcessMessages(MessageSet messages);

        [OperationContract(IsOneWay = true)]
        void ReportPerformance(PerformanceData data);
        
    }
}
