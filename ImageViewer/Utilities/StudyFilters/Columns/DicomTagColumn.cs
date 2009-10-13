using System;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	public abstract class DicomTagColumn<T> : StudyFilterColumnBase<T>
	{
		private readonly string _tagName;

		protected readonly uint Tag;
		protected readonly string VR;

		protected DicomTagColumn(DicomTag dicomTag)
		{
			this.Tag = dicomTag.TagValue;
			this.VR = dicomTag.VR.Name;

			uint tagGroup = (this.Tag >> 16) & 0x0000FFFF;
			uint tagElement = this.Tag & 0x0000FFFF;

			if (DicomTagDictionary.GetDicomTag(dicomTag.TagValue) == null)
				_tagName = string.Format(SR.FormatUnknownDicomTag, tagGroup, tagElement);
			else
				_tagName = string.Format(SR.FormatDicomTag, tagGroup, tagElement, dicomTag.Name);
		}

		public override string Name
		{
			get { return _tagName; }
		}

		public override string Key
		{
			get { return this.Tag.ToString("x8"); }
		}

		protected static int CountValues(DicomAttribute attribute)
		{
			return (int) Math.Min(50, attribute.Count);
		}
	}
}