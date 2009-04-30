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

using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Imaging
{
	public interface IDicomVoiLutsProvider {
		IDicomVoiLuts DicomVoiLuts { get; }
	}

	public interface IDicomVoiLuts
	{
		IList<Window> ImageVoiLinearLuts { get; }
		IList<string> ImageVoiLinearLutExplanations { get; }
		IList<VoiDataLut> ImageVoiDataLuts { get; }
		string ImageSopInstanceUid { get; }
		int ImageSopFrameNumber { get; }

		IList<Window> PresentationVoiLinearLuts { get; }
		IList<string> PresentationVoiLinearLutExplanations { get; }
		IList<VoiDataLut> PresentationVoiDataLuts { get; }
		string PresentationStateSopInstanceUid { get; }
	}

	internal sealed class DicomVoiLuts : IDicomVoiLuts {
		private readonly IImageSopProvider _image;

		internal DicomVoiLuts(IImageSopProvider image)
		{
			_image = image;
		}

		#region Presentation Luts

		[CloneIgnore]
		private readonly List<Window> _presentationVoiLinearLuts = new List<Window>();

		[CloneIgnore]
		private readonly List<string> _presentationVoiLinearLutExplanations = new List<string>();

		[CloneIgnore]
		private readonly List<VoiDataLut> _presentationVoiDataLuts = new List<VoiDataLut>();

		[CloneIgnore]
		private string _sourcePresentationSopUid = "";

		public string PresentationStateSopInstanceUid
		{
			get { return _sourcePresentationSopUid; }
		}

		public IList<Window> PresentationVoiLinearLuts
		{
			get { return _presentationVoiLinearLuts.AsReadOnly(); }
		}

		public IList<string> PresentationVoiLinearLutExplanations
		{
			get { return _presentationVoiLinearLutExplanations.AsReadOnly(); }
		}

		public IList<VoiDataLut> PresentationVoiDataLuts {
			get { return _presentationVoiDataLuts.AsReadOnly(); }
		}

		internal void ReinitializePresentationLuts(string sourceSopUid)
		{
			_sourcePresentationSopUid = sourceSopUid;
			_presentationVoiLinearLuts.Clear();
			_presentationVoiLinearLutExplanations.Clear();
			_presentationVoiDataLuts.Clear();
		}

		internal void AddPresentationLinearLut(double width, double center, string explanation)
		{
			_presentationVoiLinearLuts.Add(new Window(width, center));
			_presentationVoiLinearLutExplanations.Add(explanation);
		}

		internal void AddPresentationDataLut(VoiDataLut dataLut)
		{
			_presentationVoiDataLuts.Add(dataLut);
		}

		#endregion

		#region Image Luts

		public string ImageSopInstanceUid
		{
			get { return _image.ImageSop.SopInstanceUID; }
		}

		public int ImageSopFrameNumber
		{
			get { return _image.Frame.FrameNumber; }
		}

		public IList<Window> ImageVoiLinearLuts {
			get
			{
				Window[] windows = _image.Frame.WindowCenterAndWidth;
				if (windows == null || windows.Length == 0)
					return new List<Window>().AsReadOnly();
				return new List<Window>(windows).AsReadOnly();
			}
		}

		public IList<string> ImageVoiLinearLutExplanations {
			get
			{
				string[] explanations = _image.Frame.WindowCenterAndWidthExplanation;
				if (explanations == null || explanations.Length == 0)
					return new List<string>().AsReadOnly();
				return new List<string>(explanations).AsReadOnly();
			}
		}

		public IList<VoiDataLut> ImageVoiDataLuts {
			get { return _image.ImageSop.VoiDataLuts; }
		}

		#endregion
	}
}