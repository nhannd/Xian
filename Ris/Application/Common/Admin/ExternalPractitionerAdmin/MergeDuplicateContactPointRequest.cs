#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin
{
	[DataContract]
	public class MergeDuplicateContactPointRequest : DataContractBase
	{
		public MergeDuplicateContactPointRequest(
			EntityRef retained,
			EntityRef replaced)
		{
			this.RetainedContactPointRef = retained;
			this.ReplacedContactPointRef = replaced;
		}

		[DataMember]
		public EntityRef RetainedContactPointRef;

		[DataMember]
		public EntityRef ReplacedContactPointRef;

		/// <summary>
		/// If true, no merge will actually be performed.  Instead, the server will return some estimated
		/// measures of the cost of the merge operation if it were to be performed.
		/// </summary>
		[DataMember]
		public bool EstimateCostOnly;
	}
}
