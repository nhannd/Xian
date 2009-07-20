using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	public class TextDicomTagColumn : DicomTagColumn<string>, ILexicalSortableColumn
	{
		public TextDicomTagColumn(DicomTag dicomTag) : base(dicomTag) { }

		public override string GetTypedValue(StudyItem item)
		{
			DicomAttribute attribute = item[base.Tag];

			if (attribute == null)
				return null;
			if (attribute.IsNull)
				return string.Empty;

			return attribute.ToString();
		}

		public override bool Parse(string input, out string output)
		{
			output = input;
			return true;
		}

		public override int Compare(StudyItem x, StudyItem y)
		{
			return this.CompareLexically(x, y);
		}

		public int CompareLexically(StudyItem x, StudyItem y)
		{
			return this.GetTypedValue(x).CompareTo(this.GetTypedValue(y));
		}
	}
}