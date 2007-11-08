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

using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	// TODO (Stewart): Might consider making this a single static helper class

	/// <summary>
	/// A delegate used to retrieve data of a particular type <typeparamref name="T"/> from an <see cref="ImageSop"/>.
	/// </summary>
	public delegate T SopDataRetrieverDelegate<T>(ImageSop imageSop);

	/// <summary>
	/// A Generic abstract class that defines how to get arbitrary types of data from an <see cref="ImageSop"/>.
	/// </summary>
	/// <seealso cref="ImageSop"/>
	public abstract class DicomTagRetriever<T>
	{
		private uint _dicomTag;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="dicomTag">The dicom tag whose value is to be retrieved by <see cref="GetTagValue"/>.</param>
		protected DicomTagRetriever(uint dicomTag)
		{
			Platform.CheckForNullReference(dicomTag, "dicomTag");
			_dicomTag = dicomTag;
		}

		/// <summary>
		/// Gets the dicom tag whose value is to be retrieved.
		/// </summary>
		public uint DicomTag
		{
			get { return _dicomTag; }
		}

		/// <summary>
		/// Gets the value of <see cref="DicomTag"/> as type <typeparamref name="T"/>.
		/// </summary>
		/// <param name="imageSop">The <see cref="ImageSop"/> from which to get <see cref="DicomTag"/>'s value.</param>
		public abstract T GetTagValue(ImageSop imageSop);
	}

	/// <summary>
	/// A <see cref="DicomTagRetriever{T}"/> that gets the tag value as a string.
	/// </summary>
	public class DicomTagAsStringRetriever : DicomTagRetriever<string>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="dicomTag">The dicom tag whose value is to be retrieved by <see cref="GetTagValue"/>.</param>
		public DicomTagAsStringRetriever(uint dicomTag)
			: base(dicomTag)
		{
		}

		/// <summary>
		/// Gets the value of <see cref="DicomTagRetriever{T}.DicomTag"/> as a string.
		/// </summary>
		/// <param name="imageSop">The <see cref="ImageSop"/> from which to get <see cref="DicomTagRetriever{T}.DicomTag"/>'s value.</param>
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

	/// <summary>
	/// A <see cref="DicomTagRetriever{T}"/> that gets the tag value as a double.
	/// </summary>
	public class DicomTagAsDoubleRetriever : DicomTagRetriever<double>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="dicomTag">The dicom tag whose value is to be retrieved by <see cref="GetTagValue"/>.</param>
		public DicomTagAsDoubleRetriever(uint dicomTag)
			: base(dicomTag)
		{
		}

		/// <summary>
		/// Gets the value of <see cref="DicomTagRetriever{T}.DicomTag"/> as a double.
		/// </summary>
		/// <param name="imageSop">The <see cref="ImageSop"/> from which to get <see cref="DicomTagRetriever{T}.DicomTag"/>'s value.</param>
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

	/// <summary>
	/// A <see cref="DicomTagRetriever{T}"/> that gets the tag value as a string array (e.g. as in Dicom "1\2\3").
	/// </summary>
	public class DicomTagAsRawStringArrayRetriever : DicomTagRetriever<string>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="dicomTag">The dicom tag whose value is to be retrieved by <see cref="GetTagValue"/>.</param>
		public DicomTagAsRawStringArrayRetriever(uint dicomTag)
			: base(dicomTag)
		{
		}

		/// <summary>
		/// Gets the value of <see cref="DicomTagRetriever{T}.DicomTag"/> as a string array (e.g. as in Dicom "1\2\3").
		/// </summary>
		/// <param name="imageSop">The <see cref="ImageSop"/> from which to get <see cref="DicomTagRetriever{T}.DicomTag"/>'s value.</param>
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

	/// <summary>
	/// A <see cref="DicomTagRetriever{T}"/> that gets the tag value as a string[].
	/// </summary>
	public class DicomTagAsStringArrayRetriever : DicomTagRetriever<string[]>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="dicomTag">The dicom tag whose value is to be retrieved by <see cref="GetTagValue"/>.</param>
		public DicomTagAsStringArrayRetriever(uint dicomTag)
			: base(dicomTag)
		{
		}

		/// <summary>
		/// Gets the value of <see cref="DicomTagRetriever{T}.DicomTag"/> as a string[].
		/// </summary>
		/// <param name="imageSop">The <see cref="ImageSop"/> from which to get <see cref="DicomTagRetriever{T}.DicomTag"/>'s value.</param>
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

	/// <summary>
	/// A <see cref="DicomTagRetriever{T}"/> that gets the tag value as a double[].
	/// </summary>
	public class DicomTagAsDoubleArrayRetriever : DicomTagRetriever<double[]>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="dicomTag">The dicom tag whose value is to be retrieved by <see cref="GetTagValue"/>.</param>
		public DicomTagAsDoubleArrayRetriever(uint dicomTag)
			: base(dicomTag)
		{
		}

		/// <summary>
		/// Gets the value of <see cref="DicomTagRetriever{T}.DicomTag"/> as a double[].
		/// </summary>
		/// <param name="imageSop">The <see cref="ImageSop"/> from which to get <see cref="DicomTagRetriever{T}.DicomTag"/>'s value.</param>
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