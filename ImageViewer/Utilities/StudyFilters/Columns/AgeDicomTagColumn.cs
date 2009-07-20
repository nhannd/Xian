using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	public class AgeDicomTagColumn : DicomTagColumn<DicomArray<DicomAge>>, INumericSortableColumn
	{
		public AgeDicomTagColumn(DicomTag dicomTag) : base(dicomTag) {}

		public override DicomArray<DicomAge> GetTypedValue(StudyItem item)
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

		public override int Compare(StudyItem x, StudyItem y)
		{
			return this.CompareNumerically(x, y);
		}

		public int CompareNumerically(StudyItem x, StudyItem y)
		{
			return DicomArray<DicomAge>.Compare(this.GetTypedValue(x), this.GetTypedValue(y));
		}
	}
}