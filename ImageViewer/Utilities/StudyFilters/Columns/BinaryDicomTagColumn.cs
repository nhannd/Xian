#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	public class BinaryDicomTagColumn : DicomTagColumn<DicomObject>, IGenericSortableColumn
	{
		public BinaryDicomTagColumn(DicomTag dicomTag) : base(dicomTag) {}

		public override DicomObject GetTypedValue(IStudyItem item)
		{
			DicomAttribute attribute = item[base.Tag];
			if (attribute == null)
				return new DicomObject(-1, base.VR);
			if (attribute.IsNull)
				return new DicomObject(0, base.VR);
			return new DicomObject(attribute.Count, base.VR);
		}

		public override bool Parse(string input, out DicomObject output)
		{
			output = DicomObject.Empty;
			return false;
		}

		public override int Compare(IStudyItem x, IStudyItem y)
		{
			return this.GetTypedValue(x).CompareTo(this.GetTypedValue(y));
		}
	}
}