#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A delegate used to retrieve data from an <see cref="ImageSop"/>.
	/// </summary>
	public delegate T SopDataRetrieverDelegate<T>(ImageSop imageSop);

	/// <summary>
	/// A helper factory for constructing a delegate to return a specific <see cref="DicomTag"/>'s
	/// value from an <see cref="ImageSop"/>.
	/// </summary>
	/// <remarks>
	/// Note that the <see cref="SopDataRetrieverDelegate{T}"/>s returned by this factory
	/// simply return the default value for the return type when the tag has no value or
	/// does not exist.  Therefore, you should only use <see cref="SopDataRetrieverDelegate{T}"/>s
	/// for cases when the existence of the tag is unimportant.
	/// </remarks>
	public static class SopDataRetrieverFactory
	{
		/// <summary>
		/// Returns a delegate that will get the value of <paramref name="dicomTag"/> (at position 0),
		/// from an <see cref="ImageSop"/> as a <see cref="string"/>.
		/// </summary>
		public static SopDataRetrieverDelegate<string> GetStringRetriever(uint dicomTag)
		{
			return GetStringRetriever(dicomTag, 0);
		}

		/// <summary>
		/// Returns a delegate that will get the value of <paramref name="dicomTag"/> (at <paramref name="position"/>),
		/// from an <see cref="ImageSop"/> as a <see cref="string"/>.
		/// </summary>
		public static SopDataRetrieverDelegate<string> GetStringRetriever(uint dicomTag, uint position)
		{
			return delegate(ImageSop imageSop)
				{
					bool tagExists;
					string value;
					imageSop.GetTag(dicomTag, out value, position, out tagExists);
					return value ?? "";
				};
		}

		/// <summary>
		/// Returns a delegate that will get the value of <paramref name="dicomTag"/> (all positions),
		/// from an <see cref="ImageSop"/> as an array of <see cref="string"/>s.
		/// </summary>
		public static SopDataRetrieverDelegate<string[]> GetStringArrayRetriever(uint dicomTag)
		{
			return delegate(ImageSop imageSop)
				{
					bool tagExists;
					string value;
					imageSop.GetTagAsDicomStringArray(dicomTag, out value, out tagExists);
					return DicomStringHelper.GetStringArray(value ?? "");
				};
		}

		/// <summary>
		/// Returns a delegate that will get the value of <paramref name="dicomTag"/> (at position 0),
		/// from an <see cref="ImageSop"/> as an <see cref="int"/>.
		/// </summary>
		public static SopDataRetrieverDelegate<int> GetIntRetriever(uint dicomTag)
		{
			return GetIntRetriever(dicomTag, 0);
		}

		/// <summary>
		/// Returns a delegate that will get the value of <paramref name="dicomTag"/> (at <paramref name="position"/>),
		/// from an <see cref="ImageSop"/> as an <see cref="int"/>.
		/// </summary>
		public static SopDataRetrieverDelegate<int> GetIntRetriever(uint dicomTag, uint position)
		{
			return delegate(ImageSop imageSop)
				{
					bool tagExists;
					int value;
					imageSop.GetTag(dicomTag, out value, position, out tagExists);
					return value;
				};
		}

		/// <summary>
		/// Returns a delegate that will get the value of <paramref name="dicomTag"/> (all positions),
		/// from an <see cref="ImageSop"/> as an array of <see cref="int"/>s.
		/// </summary>
		public static SopDataRetrieverDelegate<int[]> GetIntArrayRetriever(uint dicomTag)
		{
			return delegate(ImageSop imageSop)
				{
					bool tagExists;
					string value;
					imageSop.GetTagAsDicomStringArray(dicomTag, out value, out tagExists);
					
					int[] values;
					if (!DicomStringHelper.TryGetIntArray(value ?? "", out values))
						values = new int[]{};

					return values;
				};
		}

		/// <summary>
		/// Returns a delegate that will get the value of <paramref name="dicomTag"/> (at position 0),
		/// from an <see cref="ImageSop"/> as a <see cref="double"/>.
		/// </summary>
		public static SopDataRetrieverDelegate<double> GetDoubleRetriever(uint dicomTag)
		{
			return GetDoubleRetriever(dicomTag, 0);
		}

		/// <summary>
		/// Returns a delegate that will get the value of <paramref name="dicomTag"/> (at <paramref name="position"/>),
		/// from an <see cref="ImageSop"/> as a <see cref="double"/>.
		/// </summary>
		public static SopDataRetrieverDelegate<double> GetDoubleRetriever(uint dicomTag, uint position)
		{
			return delegate(ImageSop imageSop)
				{
					bool tagExists;
					double value;
					imageSop.GetTag(dicomTag, out value, position, out tagExists);
					return value;
				};
		}

		/// <summary>
		/// Returns a delegate that will get the value of <paramref name="dicomTag"/> (all positions),
		/// from an <see cref="ImageSop"/> as an array of <see cref="double"/>s.
		/// </summary>
		public static SopDataRetrieverDelegate<double[]> GetDoubleArrayRetriever(uint dicomTag)
		{
			return delegate(ImageSop imageSop)
				{
					bool tagExists;
					string value;
					imageSop.GetTagAsDicomStringArray(dicomTag, out value, out tagExists);
					double[] values;
					if (!DicomStringHelper.TryGetDoubleArray(value ?? "", out values))
						values = new double[] { };

					return values;

				};
		}
	}
}