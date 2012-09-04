#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
	public class ModalityAssembler
	{
		public ModalitySummary CreateModalitySummary(Modality modality)
		{
			return new ModalitySummary(
				modality.GetRef(),
				modality.Id,
				modality.Name,
				modality.AETitle,
				EnumUtils.GetEnumValueInfo(modality.DicomModality),
				modality.Deactivated);
		}

		public ModalityDetail CreateModalityDetail(Modality modality)
		{
			return new ModalityDetail(
				modality.GetRef(),
				modality.Id,
				modality.Name,
				modality.AETitle,
				EnumUtils.GetEnumValueInfo(modality.DicomModality),
				modality.Deactivated);
		}

		public void UpdateModality(ModalityDetail detail, Modality modality, IPersistenceContext context)
		{
			modality.Id = detail.Id;
			modality.Name = detail.Name;
			modality.AETitle = detail.AETitle;
			modality.DicomModality = EnumUtils.GetEnumValue<DicomModalityEnum>(detail.DicomModality, context);
			modality.Deactivated = detail.Deactivated;
		}
	}
}
