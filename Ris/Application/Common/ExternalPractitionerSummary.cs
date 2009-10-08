#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class ExternalPractitionerSummary : DataContractBase, ICloneable, IEquatable<ExternalPractitionerSummary>
	{
		public ExternalPractitionerSummary(EntityRef pracRef, PersonNameDetail personNameDetail, string licenseNumber, string billingNumber,
			bool deactivated)
		{
			this.PractitionerRef = pracRef;
			this.Name = personNameDetail;
			this.LicenseNumber = licenseNumber;
			this.BillingNumber = billingNumber;
			this.Deactivated = deactivated;
		}

		public ExternalPractitionerSummary()
		{
		}

		[DataMember]
		public EntityRef PractitionerRef;

		[DataMember]
		public PersonNameDetail Name;

		[DataMember]
		public string LicenseNumber;

		[DataMember]
		public string BillingNumber;

		[DataMember]
		public bool Deactivated;

		public bool Equals(ExternalPractitionerSummary externalPractitionerSummary)
		{
			return externalPractitionerSummary != null && Equals(PractitionerRef, externalPractitionerSummary.PractitionerRef);
		}

		public override bool Equals(object obj)
		{
			return ReferenceEquals(this, obj) || Equals(obj as ExternalPractitionerSummary);
		}

		public override int GetHashCode()
		{
			return PractitionerRef.GetHashCode();
		}

		#region ICloneable Members

		public object Clone()
		{
			return new ExternalPractitionerSummary(
				this.PractitionerRef,
				(PersonNameDetail)this.Name.Clone(),
				this.LicenseNumber,
				this.BillingNumber,
				this.Deactivated);
		}

		#endregion
	}
}
