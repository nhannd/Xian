#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.Dicom.ServiceModel.Query
{
	//NOTE: internal for now because we don't actually implement IPatientRootQuery anywhere.

	[DataContract(Namespace = QueryNamespace.Value)]
	internal class PatientRootStudyIdentifier : StudyIdentifier
	{
		public PatientRootStudyIdentifier()
		{
		}

		public PatientRootStudyIdentifier(IStudyIdentifier other)
			: base(other)
		{
		}

		public PatientRootStudyIdentifier(IStudyData other)
			: base(other)
		{
		}

		public PatientRootStudyIdentifier(IStudyData other, IIdentifier identifier)
			: base(other, identifier)
		{
		}

		public PatientRootStudyIdentifier(DicomAttributeCollection attributes)
			: base(attributes)
		{
		}
	}
}
