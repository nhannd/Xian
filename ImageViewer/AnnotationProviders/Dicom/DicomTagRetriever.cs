using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	public abstract class DicomTagRetriever<T>
	{
		private uint _dicomTag;

		public DicomTagRetriever(uint dicomTag)
		{
			Platform.CheckForNullReference(dicomTag, "dicomTag");
			_dicomTag = dicomTag;
		}

		public uint DicomTag
		{
			get { return _dicomTag; }
		}

		public abstract T GetTagValue(ImageSop imageSop);
	}


	public class DicomTagAsStringRetriever : DicomTagRetriever<string>
	{
		public DicomTagAsStringRetriever(uint dicomTag)
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
		public DicomTagAsDoubleRetriever(uint dicomTag)
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
		public DicomTagAsRawStringArrayRetriever(uint dicomTag)
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
		public DicomTagAsStringArrayRetriever(uint dicomTag)
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
		public DicomTagAsDoubleArrayRetriever(uint dicomTag)
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
