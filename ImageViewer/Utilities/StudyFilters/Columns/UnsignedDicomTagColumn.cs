using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	public class UnsignedDicomTagColumn : MultiValuedDicomTagColumn<uint>, INumericSortableColumn
	{
		public UnsignedDicomTagColumn(DicomTag dicomTag) : base(dicomTag) {}

		public override DicomArray<uint> GetTypedValue(DicomAttribute attribute)
		{
			if (attribute == null)
				return null;
			if (attribute.IsNull)
				return new DicomArray<uint>();

			uint?[] result;
			try
			{
				result = new uint?[CountValues(attribute)];
				for (int n = 0; n < result.Length; n++)
				{
					uint value;
					if (attribute.TryGetUInt32(n, out value))
						result[n] = value;
				}
			}
			catch (DicomException)
			{
				return null;
			}
			return new DicomArray<uint>(result);
		}

		public override int Compare(StudyItem x, StudyItem y)
		{
			return this.CompareNumerically(x, y);
		}

		public int CompareNumerically(StudyItem x, StudyItem y)
		{
			return DicomArray<uint>.Compare(this.GetTypedValue(x), this.GetTypedValue(y));
		}
	}
}