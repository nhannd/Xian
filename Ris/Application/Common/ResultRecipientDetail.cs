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

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class ResultRecipientDetail : DataContractBase
	{
		public ResultRecipientDetail()
		{
		}

		public ResultRecipientDetail(ExternalPractitionerSummary practitioner, ExternalPractitionerContactPointDetail contactPoint, EnumValueInfo preferredCommunicationMode)
		{
			this.Practitioner = practitioner;
			this.ContactPoint = contactPoint;
			this.PreferredCommunicationMode = preferredCommunicationMode;
		}

		[DataMember]
		public ExternalPractitionerSummary Practitioner;

		[DataMember]
		public ExternalPractitionerContactPointDetail ContactPoint;

		[DataMember]
		public EnumValueInfo PreferredCommunicationMode;
	}
}