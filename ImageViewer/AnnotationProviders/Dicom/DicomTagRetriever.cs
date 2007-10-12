#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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

			return DicomStringHelper.GetStringArray(value);
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

			return DicomStringHelper.GetDoubleArray(value);
		}
	}
}
