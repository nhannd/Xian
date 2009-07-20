using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	public class IntegerDicomTagColumn : MultiValuedDicomTagColumn<int>, INumericSortableColumn
	{
		public IntegerDicomTagColumn(DicomTag dicomTag) : base(dicomTag) {}

		public override DicomArray<int> GetTypedValue(DicomAttribute attribute)
		{
			if (attribute == null)
				return null;
			if (attribute.IsNull)
				return new DicomArray<int>();

			int?[] result;
			try
			{
				result = new int?[CountValues(attribute)];
				for (int n = 0; n < result.Length; n++)
				{
					int value;
					if (attribute.TryGetInt32(n, out value))
						result[n] = value;
				}
			}
			catch (DicomException)
			{
				return null;
			}
			return new DicomArray<int>(result);
		}

		public override int Compare(StudyItem x, StudyItem y)
		{
			return this.CompareNumerically(x, y);
		}

		public int CompareNumerically(StudyItem x, StudyItem y)
		{
			return DicomArray<int>.Compare(this.GetTypedValue(x), this.GetTypedValue(y));
		}
	}
}