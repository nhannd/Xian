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
	public class AgeDicomTagColumn : DicomTagColumn<DicomArray<DicomAge>>, INumericSortableColumn
	{
		public AgeDicomTagColumn(DicomTag dicomTag) : base(dicomTag) {}

		public override DicomArray<DicomAge> GetTypedValue(IStudyItem item)
		{
			DicomAttribute attribute = item[base.Tag];

			if (attribute == null)
				return null;
			if (attribute.IsNull)
				return new DicomArray<DicomAge>();

			DicomAge?[] result;
			try
			{
				result = new DicomAge?[CountValues(attribute)];
				for (int n = 0; n < result.Length; n++)
				{
					string value;
					DicomAge age;
					if (attribute.TryGetString(n, out value) && DicomAge.TryParse(value, out age))
						result[n] = age;
				}
			}
			catch (DicomException)
			{
				return null;
			}
			return new DicomArray<DicomAge>(result);
		}

		public override bool Parse(string input, out DicomArray<DicomAge> output)
		{
			return DicomArray<DicomAge>.TryParse(input, DicomAge.TryParse, out output);
		}

		public override int Compare(IStudyItem x, IStudyItem y)
		{
			return this.CompareNumerically(x, y);
		}

		public int CompareNumerically(IStudyItem x, IStudyItem y)
		{
			return DicomArray<DicomAge>.Compare(this.GetTypedValue(x), this.GetTypedValue(y));
		}
	}
}