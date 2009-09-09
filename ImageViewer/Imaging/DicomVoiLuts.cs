#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Defines a property to get a collection of DICOM-defined VOI LUTs from the image header and/or any associated presentation state.
	/// </summary>
	public interface IDicomVoiLutsProvider
	{
		/// <summary>
		/// Gets a collection of DICOM-defined VOI LUTs from the image header and/or any associated presentation state.
		/// </summary>
		IDicomVoiLuts DicomVoiLuts { get; }
	}

	/// <summary>
	/// Defines properties to get the various DICOM-defined VOI LUTs from the image header and/or any associated presentation state.
	/// </summary>
	public interface IDicomVoiLuts
	{
		/// <summary>
		/// Gets a list of linear VOI LUTs (i.e. value windows) defined in the image header.
		/// </summary>
		IList<VoiWindow> ImageVoiLinearLuts { get; }

		/// <summary>
		/// Gets a list of data VOI LUTs defined in the image header.
		/// </summary>
		IList<VoiDataLut> ImageVoiDataLuts { get; }

		/// <summary>
		/// Gets the SOP instance UID of the image.
		/// </summary>
		string ImageSopInstanceUid { get; }

		/// <summary>
		/// Gets the frame number of the frame associated with the image.
		/// </summary>
		int ImageSopFrameNumber { get; }

		/// <summary>
		/// Gets a list of linear VOI LUTs (i.e. value windows) defined in the presentation state.
		/// </summary>
		IList<VoiWindow> PresentationVoiLinearLuts { get; }

		/// <summary>
		/// Gets a list of data VOI LUTs defined in the presentation state.
		/// </summary>
		IList<VoiDataLut> PresentationVoiDataLuts { get; }

		/// <summary>
		/// Gets the SOP instance UID of the presentation state.
		/// </summary>
		string PresentationStateSopInstanceUid { get; }
	}

	internal sealed class DicomVoiLuts : IDicomVoiLuts
	{
		private readonly IImageSopProvider _image;

		internal DicomVoiLuts(IImageSopProvider image)
		{
			_image = image;
		}

		#region Presentation Luts

		[CloneIgnore]
		private readonly List<VoiWindow> _presentationVoiLinearLuts = new List<VoiWindow>();

		[CloneIgnore]
		private readonly List<VoiDataLut> _presentationVoiDataLuts = new List<VoiDataLut>();

		[CloneIgnore]
		private string _sourcePresentationSopUid = "";

		public string PresentationStateSopInstanceUid
		{
			get { return _sourcePresentationSopUid; }
		}

		public IList<VoiWindow> PresentationVoiLinearLuts
		{
			get { return _presentationVoiLinearLuts.AsReadOnly(); }
		}

		public IList<VoiDataLut> PresentationVoiDataLuts
		{
			get { return _presentationVoiDataLuts.AsReadOnly(); }
		}

		internal void ReinitializePresentationLuts(string sourceSopUid)
		{
			_sourcePresentationSopUid = sourceSopUid;
			_presentationVoiLinearLuts.Clear();
			_presentationVoiDataLuts.Clear();
		}

		internal void AddPresentationLinearLut(double width, double center, string explanation)
		{
			_presentationVoiLinearLuts.Add(new VoiWindow(width, center, explanation));
		}

		internal void AddPresentationDataLut(VoiDataLut dataLut)
		{
			_presentationVoiDataLuts.Add(dataLut);
		}

		#endregion

		#region Image Luts

		public string ImageSopInstanceUid
		{
			get { return _image.ImageSop.SopInstanceUid; }
		}

		public int ImageSopFrameNumber
		{
			get { return _image.Frame.FrameNumber; }
		}

		public IList<VoiWindow> ImageVoiLinearLuts
		{
			get { return new List<VoiWindow>(VoiWindow.GetWindows(_image.ImageSop.DataSource)).AsReadOnly(); }
		}

		public IList<VoiDataLut> ImageVoiDataLuts
		{
			get { return _image.ImageSop.VoiDataLuts; }
		}

		#endregion
	}
}