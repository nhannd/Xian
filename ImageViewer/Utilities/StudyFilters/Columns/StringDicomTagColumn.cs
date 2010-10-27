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
	public class StringDicomTagColumn : DicomTagColumn<DicomObjectArray<string>>, ILexicalSortableColumn
	{
		public StringDicomTagColumn(DicomTag dicomTag) : base(dicomTag) {}

		public override DicomObjectArray<string> GetTypedValue(IStudyItem item)
		{
			DicomAttribute attribute = item[base.Tag];

			if (attribute == null)
				return null;
			if (attribute.IsNull)
				return new DicomObjectArray<string>();
			if (attribute is DicomAttributeSingleValueText)
				return new DicomObjectArray<string>(attribute.ToString());
			if (!(attribute is DicomAttributeMultiValueText))
				return new DicomObjectArray<string>(string.Format(SR.LabelVRIncorrect, attribute.Tag.VR.Name, base.VR));

			string[] result;
			result = new string[CountValues(attribute)];
			for (int n = 0; n < result.Length; n++)
				result[n] = attribute.GetString(n, string.Empty);
			return new DicomObjectArray<string>(result);
		}

		public override bool Parse(string input, out DicomObjectArray<string> output)
		{
			return DicomObjectArray<string>.TryParse(
				input,
				delegate(string s, out string result)
					{
						result = s;
						return true;
					},
				out output);
		}

		public override int Compare(IStudyItem x, IStudyItem y)
		{
			return this.CompareLexically(x, y);
		}

		public int CompareLexically(IStudyItem x, IStudyItem y)
		{
			return DicomObjectArray<string>.Compare(this.GetTypedValue(x), this.GetTypedValue(y));
		}
	}
}