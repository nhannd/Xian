#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.ImageServer.Core.Edit
{
	public class PatientInfo : IEquatable<PatientInfo>
	{
		public PatientInfo()
		{
		}

		public PatientInfo(PatientInfo other)
		{
			Name = other.Name;
			PatientId = other.PatientId;
			IssuerOfPatientId = other.IssuerOfPatientId;
		}

		public string Name { get; set; }

		public string PatientId { get; set; }

		public string IssuerOfPatientId { get; set; }

		#region IEquatable<PatientInfo> Members

		public bool Equals(PatientInfo other)
		{
			PersonName name = new PersonName(Name);
			PersonName otherName = new PersonName(other.Name);
			return name.Equals(otherName) && String.Equals(PatientId, other.PatientId, StringComparison.InvariantCultureIgnoreCase);
		}

		#endregion
	}
}