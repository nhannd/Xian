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

namespace ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin
{
	[DataContract]
	public class AddExternalPractitionerRequest : DataContractBase
	{
		public AddExternalPractitionerRequest(ExternalPractitionerDetail pracDetail, bool markVerified)
		{
			this.PractitionerDetail = pracDetail;
			this.MarkVerified = markVerified;
		}

		[DataMember]
		public ExternalPractitionerDetail PractitionerDetail;

		[DataMember]
		public bool MarkVerified;
	}
}
