using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	public class BinaryDicomTagColumn : DicomTagColumn<DicomObject>, IGenericSortableColumn
	{
		public BinaryDicomTagColumn(DicomTag dicomTag) : base(dicomTag) {}

		public override DicomObject GetTypedValue(StudyItem item)
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

		public override int Compare(StudyItem x, StudyItem y)
		{
			return this.GetTypedValue(x).CompareTo(this.GetTypedValue(y));
		}
	}
}