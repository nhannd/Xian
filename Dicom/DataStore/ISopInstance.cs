#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.Dicom.DataStore
{
    public interface ISopInstance : ISopInstanceData
    {
		ISeries GetParentSeries();
		DicomUri GetLocationUri();

		string SpecificCharacterSet { get; }
		string TransferSyntaxUid { get; }

    	bool IsStoredTag(uint tag);
		bool IsStoredTag(DicomTag tag);

		DicomAttribute this[DicomTag tag] { get; }
		DicomAttribute this[uint tag] { get; }
    }
}
