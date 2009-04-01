using System.IO;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters
{
	public class StudyItem
	{
		private readonly FileInfo _file;
		private readonly DicomFile _dcf;

		public StudyItem(string filename)
		{
			_file = new FileInfo(filename);
			_dcf = new DicomFile(filename);
			_dcf.Load(DicomReadOptions.StorePixelDataReferences);
		}

		public FileInfo File
		{
			get { return _file; }
		}

		public string this[uint tag]
		{
			get
			{
				DicomAttribute attribute;
				if (!_dcf.DataSet.TryGetAttribute(tag, out attribute))
				{
					if (!_dcf.MetaInfo.TryGetAttribute(tag, out attribute))
						return SR.LabelNonExistentValue;
				}

				if (attribute.IsEmpty)
					return SR.LabelNonExistentValue;
				if (attribute is DicomAttributeOB || attribute is DicomAttributeOF || attribute is DicomAttributeOW || attribute is DicomAttributeSQ)
					return SR.LabelBinaryTagValue;
				if (attribute is DicomAttributeUN)
					return SR.LabelVRUnknown;
				if (attribute.IsNull)
					return SR.LabelNullTagValue;

				return attribute.ToString();
			}
		}
	}
}