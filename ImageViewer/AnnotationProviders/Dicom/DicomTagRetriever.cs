using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	public abstract class DicomTagRetriever<T>
	{
		private DcmTagKey _dicomTag;

		public DicomTagRetriever(DcmTagKey dicomTag)
		{
			Platform.CheckForNullReference(dicomTag, "dicomTag");
			_dicomTag = dicomTag;
		}

		public DcmTagKey DicomTag
		{
			get { return _dicomTag; }
		}

		public abstract T GetTagValue(ImageSop imageSop);
	}


	public class DicomTagAsStringRetriever : DicomTagRetriever<string>
	{
		public DicomTagAsStringRetriever(DcmTagKey dicomTag)
			: base(dicomTag)
		{
		}

		public override string GetTagValue(ImageSop imageSop)
		{
			string value;
			bool tagExists;
			imageSop.GetTag(this.DicomTag, out value, out tagExists);
			if (!tagExists)
				value = "";

			return value;
		}
	}

	public class DicomTagAsDoubleRetriever : DicomTagRetriever<double>
	{
		public DicomTagAsDoubleRetriever(DcmTagKey dicomTag)
			: base(dicomTag)
		{
		}

		public override double GetTagValue(ImageSop imageSop)
		{
			double value;
			bool tagExists;
			imageSop.GetTag(this.DicomTag, out value, out tagExists);
			if (!tagExists)
				value = double.NaN;

			return value;
		}
	}


	public class DicomTagAsRawStringArrayRetriever : DicomTagRetriever<string>
	{
		public DicomTagAsRawStringArrayRetriever(DcmTagKey dicomTag)
			: base(dicomTag)
		{
		}

		public override string GetTagValue(ImageSop imageSop)
		{
			string value;
			bool tagExists;
			imageSop.GetTagArray(this.DicomTag, out value, out tagExists);
			if (!tagExists)
				value = "";

			return value;
		}
	}

	public class DicomTagAsStringArrayRetriever : DicomTagRetriever<string[]>
	{
		public DicomTagAsStringArrayRetriever(DcmTagKey dicomTag)
			: base(dicomTag)
		{
		}

		public override string[] GetTagValue(ImageSop imageSop)
		{
			string value;
			bool tagExists;
			imageSop.GetTagArray(this.DicomTag, out value, out tagExists);
			if (tagExists)
				value = "";

			return VMStringConverter.ToStringArray(value);
		}
	}

	public class DicomTagAsDoubleArrayRetriever : DicomTagRetriever<double[]>
	{
		public DicomTagAsDoubleArrayRetriever(DcmTagKey dicomTag)
			: base(dicomTag)
		{
		}

		public override double[] GetTagValue(ImageSop imageSop)
		{
			string value;
			bool tagExists;
			imageSop.GetTagArray(this.DicomTag, out value, out tagExists);
			if (tagExists)
				value = "";

			return VMStringConverter.ToDoubleArray(value);
		}
	}
}
