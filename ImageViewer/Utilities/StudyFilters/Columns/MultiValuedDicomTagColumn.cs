using System;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	public abstract class MultiValuedDicomTagColumn<T> : DicomTagColumn<DicomArray<T>>
		where T : struct, IComparable<T>, IEquatable<T>
	{
		private readonly DicomAttribute _testAttribute;

		protected MultiValuedDicomTagColumn(DicomTag dicomTag) : base(dicomTag)
		{
			_testAttribute = dicomTag.CreateDicomAttribute();
		}

		public abstract DicomArray<T> GetTypedValue(DicomAttribute attribute);

		public override sealed DicomArray<T> GetTypedValue(StudyItem item)
		{
			return GetTypedValue(item[base.Tag]);
		}

		public override sealed bool Parse(string input, out DicomArray<T> output)
		{
			try
			{
				_testAttribute.SetStringValue(input);
				output = GetTypedValue(_testAttribute);
				return true;
			}
			catch (Exception)
			{
				output = null;
				return false;
			}
		}
	}
}