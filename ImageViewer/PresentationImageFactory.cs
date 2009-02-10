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

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Iod.Iods;
using ClearCanvas.ImageViewer.KeyObjects;
using ClearCanvas.ImageViewer.PresentationStates;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A factory class which creates <see cref="IPresentationImage"/>s.
	/// </summary>
	public class PresentationImageFactory
	{
		private readonly StudyTree _studyTree;

		public PresentationImageFactory(StudyTree studyTree)
		{
			_studyTree = studyTree;
		}

		public StudyTree StudyTree
		{
			get { return _studyTree; }
		}

		public virtual List<IPresentationImage> CreateImages(Sop sop)
		{
			if (sop is ImageSop)
			{
				return CreateImages((ImageSop) sop);
			}
			else if (sop.SopClassUID == SopClass.KeyObjectSelectionDocumentStorageUid)
			{
				return CreateImages(new KeyObjectSelectionDocumentIod(sop.DataSource));
			}

			return new List<IPresentationImage>();
		}

		protected virtual List<IPresentationImage> CreateImages(KeyObjectSelectionDocumentIod keyObjectDocument)
		{
			List<IPresentationImage> images = new List<IPresentationImage>();
			IList<KeyObjectContentItem> content = new KeyImageDeserializer(keyObjectDocument, _studyTree).Deserialize();
			foreach (KeyObjectContentItem item in content)
			{
				IPresentationImage image = Create(item.Frame);
				if (item.PresentationStateSop != null && image is IDicomSoftcopyPresentationStateProvider)
				{
					try
					{
						IDicomSoftcopyPresentationStateProvider presentationStateProvider = (IDicomSoftcopyPresentationStateProvider) image;
						presentationStateProvider.PresentationState = DicomSoftcopyPresentationState.Load(item.PresentationStateSop.DataSource);
					}
					catch (Exception ex)
					{
						Platform.Log(LogLevel.Warn, ex, SR.MessagePresentationStateReadFailure);
					}
				}
				images.Add(image);
			}
			return images;
		}



		protected virtual List<IPresentationImage> CreateImages(ImageSop imageSop)
		{
			return Create(imageSop);
		}

		/// <summary>
		/// Creates an appropriate subclass of <see cref="BasicPresentationImage"/>
		/// for each <see cref="Frame"/> in the input <see cref="ImageSop"/>.
		/// </summary>
		public static List<IPresentationImage> Create(ImageSop imageSop)
		{
			return CollectionUtils.Map<Frame, IPresentationImage>(imageSop.Frames,
			                                                      delegate(Frame frame) { return Create(frame); });
		}

		/// <summary>
		/// Creates an appropriate subclass of <see cref="BasicPresentationImage"/>
		/// based on the <see cref="Frame"/>'s photometric interpretation.
		/// </summary>
		public static IPresentationImage Create(Frame frame)
		{
			if (frame.PhotometricInterpretation == PhotometricInterpretation.Unknown)
			{
				throw new Exception("Photometric interpretation is unknown.");
			}
			else if (!frame.PhotometricInterpretation.IsColor)
			{
				return new DicomGrayscalePresentationImage(frame);
			}
			else
			{
				return new DicomColorPresentationImage(frame);
			}
		}
	}
}