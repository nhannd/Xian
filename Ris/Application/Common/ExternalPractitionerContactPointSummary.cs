#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class ExternalPractitionerContactPointSummary : DataContractBase, ICloneable
	{
		public ExternalPractitionerContactPointSummary(
			EntityRef contactPointRef, 
			string name, 
			string description, 
			bool isDefaultContactPoint,
			bool isMerged,
			bool deactivated)
		{
			this.ContactPointRef = contactPointRef;
			this.Name = name;
			this.Description = description;
			this.IsDefaultContactPoint = isDefaultContactPoint;
			this.IsMerged = isMerged;
			this.Deactivated = deactivated;
		}

		public ExternalPractitionerContactPointSummary()
		{
		}

		[DataMember]
		public EntityRef ContactPointRef;

		[DataMember]
		public string Name;

		[DataMember]
		public string Description;

		[DataMember]
		public bool IsDefaultContactPoint;

		[DataMember]
		public bool IsMerged;

		[DataMember]
		public bool Deactivated;

		#region ICloneable Members

		public object Clone()
		{
			return new ExternalPractitionerContactPointSummary(
				this.ContactPointRef,
				this.Name,
				this.Description,
				this.IsDefaultContactPoint,
				this.IsMerged,
				this.Deactivated);
		}

		#endregion
	}
}
