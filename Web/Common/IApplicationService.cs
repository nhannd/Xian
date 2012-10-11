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
    #region Data Contracts

    [DataContract(Namespace = Namespace.Value)]
    public abstract class StartApplicationRequest : DataContractBase
	{
		[DataMember(IsRequired = true)]
		public Guid Identifier { get; set; }

		[DataMember(IsRequired = false)]
		public string Username { get; set; }

		[DataMember(IsRequired = false)]
		public string SessionId { get; set; }

		[DataMember(IsRequired = false)]
		public bool IsSessionShared { get; set; }

        [DataMember(IsRequired = false)]
        public MetaInformation MetaInformation { get; set; }
	}

    [DataContract(Namespace = Namespace.Value)]
    public class MetaInformation : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public string Language { get; set; }
    }

    [DataContract(Namespace = Namespace.Value)]
    public class StartApplicationResult : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public Guid ApplicationId { get; set; }
    }
    
	[DataContract(Namespace = Namespace.Value)]
    public class StopApplicationRequest : DataContractBase
	{
		[DataMember(IsRequired = true)]
		public Guid ApplicationId { get; set; }
	}

    [DataContract(Namespace = Namespace.Value)]
    public class GetPendingEventsRequest : DataContractBase
	{
		[DataMember(IsRequired = true)]
		public Guid ApplicationId { get; set; }

        [DataMember(IsRequired = false)]
        public int MaxWaitTime { get; set; }
	}

    [DataContract(Namespace = Namespace.Value)]
    public class GetPendingEventsResult : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public Guid ApplicationId { get; set; }

        [DataMember(IsRequired = false)]
        public EventSet EventSet { get; set; }
    }

    [DataContract(Namespace = Namespace.Value)]
    public class ProcessEventsRequest : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public Guid ApplicationId { get; set; }

        [DataMember(IsRequired = false)]
        public EventSet EventSet { get; set; }
    }

    [DataContract]
    public class InvalidOperationFault : DataContractBase
    {
    }

    [DataContract]
    public class SetPropertyRequest : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public Guid ApplicationId { get; set; }

        [DataMember]
        public string Key { get; set; }

        [DataMember]
        public string Value { get; set; }
    }

    #endregion

    [ServiceContract(Namespace = Namespace.Value)]
	[ServiceKnownType("GetKnownTypes", typeof(ServiceKnownTypeExtensionPoint))]
	public interface IApplicationService
    {
		[OperationContract(IsOneWay = false)]
        [FaultContract(typeof(SessionValidationFault))]
        [FaultContract(typeof(OutOfResourceFault))]
        StartApplicationResult StartApplication(StartApplicationRequest request);

		[OperationContract(IsOneWay = false)]
        [FaultContract(typeof(InvalidOperationFault))]
        void StopApplication(StopApplicationRequest request);
    }
}
