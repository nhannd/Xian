#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if	UNIT_TESTS
#pragma warning disable 1591,0419,1574,1587

using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion.Tests
{
	public enum Modality
	{
		CT,
		MR,
		PT,
		SC
	}

	public class ModalityConverter
	{
		public static string ToSopClassUid(Modality modality)
		{
			switch (modality)
			{
				case Modality.CT:
					return SopClass.CtImageStorageUid;
				case Modality.MR:
					return SopClass.MrImageStorageUid;
				case Modality.PT:
					return SopClass.PositronEmissionTomographyImageStorageUid;
				case Modality.SC:
				default:
					return SopClass.SecondaryCaptureImageStorageUid;
			}
		}
	}
}

#endif