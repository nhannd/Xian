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
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Iod.Iods;
using ClearCanvas.ImageViewer.KeyObjects;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.PresentationStates;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Defines the factory methods for creating <see cref="IPresentationImage"/>s.
	/// </summary>
	public interface IPresentationImageFactory
	{
		/// <summary>
		/// Sets the <see cref="StudyTree"/> to be used by the <see cref="IPresentationImageFactory"/> when resolving referenced SOPs.
		/// </summary>
		/// <param name="studyTree">The <see cref="StudyTree"/> to be used for resolving referenced SOPs.</param>
		void SetStudyTree(StudyTree studyTree);

		/// <summary>
		/// Creates the presentation images for a given image SOP.
		/// </summary>
		/// <param name="sop">The image SOP from which presentation images are to be created.</param>
		/// <returns>A list of created presentation images.</returns>
		List<IPresentationImage> CreateImages(Sop sop);

		/// <summary>
		/// Creates the presentation image for a given image frame.
		/// </summary>
		/// <param name="frame">The image frame from which a presentation image is to be created.</param>
		/// <returns>The created presentation image.</returns>
		IPresentationImage CreateImage(Frame frame);
	}

	/// <summary>
	/// A factory class which creates <see cref="IPresentationImage"/>s.
	/// </summary>
	public class PresentationImageFactory : IPresentationImageFactory
	{
		private static readonly PresentationImageFactory _defaultInstance = new PresentationImageFactory();

		private StudyTree _studyTree;

		/// <summary>
		/// Constructs a <see cref="PresentationImageFactory"/>.
		/// </summary>
		public PresentationImageFactory()
		{
		}

		public PresentationState DefaultPresentationState {get; set; }

		/// <summary>
		/// Gets the <see cref="StudyTree"/> used by the factory to resolve referenced SOPs.
		/// </summary>
		protected StudyTree StudyTree
		{
			get { return _studyTree; }
		}

		#region IPresentationImageFactory Members

		void IPresentationImageFactory.SetStudyTree(StudyTree studyTree)
		{
			_studyTree = studyTree;
		}

		IPresentationImage IPresentationImageFactory.CreateImage(Frame frame)
		{
			return Create(frame);
		}

		#endregion

		/// <summary>
		/// Creates the presentation image for a given image frame.
		/// </summary>
		/// <param name="frame">The image frame from which a presentation image is to be created.</param>
		/// <returns>The created presentation image.</returns>
		protected virtual IPresentationImage CreateImage(Frame frame)
		{
			if (frame.PhotometricInterpretation == PhotometricInterpretation.Unknown)
				throw new Exception("Photometric interpretation is unknown.");

			IDicomPresentationImage image;

			if (!frame.PhotometricInterpretation.IsColor)
				image = new DicomGrayscalePresentationImage(frame);
			else
				image = new DicomColorPresentationImage(frame);

			if (image.PresentationState == null || Equals(image.PresentationState, PresentationState.DicomDefault))
				image.PresentationState = DefaultPresentationState;

			return image;
		}

		/// <summary>
		/// Creates the presentation images for a given image SOP.
		/// </summary>
		/// <param name="imageSop">The image SOP from which presentation images are to be created.</param>
		/// <returns>A list of created presentation images.</returns>
		protected virtual List<IPresentationImage> CreateImages(ImageSop imageSop)
		{
			return CollectionUtils.Map(imageSop.Frames, (Frame frame) => CreateImage(frame));
		}

		/// <summary>
		/// Creates the presentation images for a given image SOP.
		/// </summary>
		/// <param name="sop">The image SOP from which presentation images are to be created.</param>
		/// <returns>A list of created presentation images.</returns>
		public virtual List<IPresentationImage> CreateImages(Sop sop)
		{
			if (sop.IsImage)
				return CreateImages((ImageSop)sop);
			
			if (sop.SopClassUid == SopClass.KeyObjectSelectionDocumentStorageUid)
				return CreateImages(new KeyObjectSelectionDocumentIod(sop.DataSource));

			return new List<IPresentationImage>();
		}

		/// <summary>
		/// Creates the presentation images for a given key object selection document.
		/// </summary>
		/// <param name="keyObjectDocument">The key object selection document from which presentation images are to be created.</param>
		/// <returns>A list of created presentation images.</returns>
		protected virtual List<IPresentationImage> CreateImages(KeyObjectSelectionDocumentIod keyObjectDocument)
		{
			List<IPresentationImage> images = new List<IPresentationImage>();
			if (_studyTree == null)
			{
				Platform.Log(LogLevel.Warn, "Key object document cannot be used to create images because there is no study tree to build from.");
			}
			else
			{
				IList<IKeyObjectContentItem> content = new KeyImageDeserializer(keyObjectDocument).Deserialize();
				foreach (IKeyObjectContentItem item in content)
				{
					if (item is KeyImageContentItem)
						images.AddRange(CreateImages((KeyImageContentItem) item));
					else
						Platform.Log(LogLevel.Warn, "Unsupported key object content value type");
				}
			}

			return images;
		}

		protected virtual List<IPresentationImage> CreateImages(KeyImageContentItem item)
		{
			List<IPresentationImage> images = new List<IPresentationImage>();

			ImageSop imageSop = FindReferencedImageSop(item.ReferencedImageSopInstanceUid, item.Source.GeneralStudy.StudyInstanceUid);
			if (imageSop != null)
			{

				int frameNumber = item.FrameNumber.GetValueOrDefault(-1);
				if (item.FrameNumber.HasValue)
				{
					if (frameNumber >= 0 && frameNumber < imageSop.Frames.Count)
					{
						images.Add(Create(imageSop.Frames[frameNumber]));
					}
					else
					{
						Platform.Log(LogLevel.Error, "The referenced key image {0} does not have a frame {1} (referenced in Key Object Selection {2})", item.ReferencedImageSopInstanceUid, frameNumber, item.Source.SopCommon.SopInstanceUid);
						images.Add(new KeyObjectPlaceholderImage(SR.MessageReferencedKeyImageFrameNotFound));
					}
				}
				else
				{
					foreach (Frame frame in imageSop.Frames)
					{
						images.Add(Create(frame));
					}
				}

				Sop presentationStateSop = FindReferencedSop(item.PresentationStateSopInstanceUid, item.Source.GeneralStudy.StudyInstanceUid);
				if (presentationStateSop != null)
				{
					foreach (IPresentationImage image in images)
					{
						if (image is IPresentationStateProvider)
						{
							try
							{
								IPresentationStateProvider presentationStateProvider = (IPresentationStateProvider)image;
								presentationStateProvider.PresentationState = DicomSoftcopyPresentationState.Load(presentationStateSop.DataSource);
							}
							catch (Exception ex)
							{
								Platform.Log(LogLevel.Warn, ex, SR.MessagePresentationStateReadFailure);
							}
						}
					}
				}
			}
			else
			{
				Platform.Log(LogLevel.Warn, "The referenced key image {0} is not loaded as part of the current study (referenced in Key Object Selection {1})", item.ReferencedImageSopInstanceUid, item.Source.SopCommon.SopInstanceUid);
				images.Add(new KeyObjectPlaceholderImage(SR.MessageReferencedKeyImageFromOtherStudy));
			}

			return images;
		}

		/// <summary>
		/// Creates an appropriate subclass of <see cref="BasicPresentationImage"/>
		/// for each <see cref="Frame"/> in the input <see cref="ImageSop"/>.
		/// </summary>
		public static List<IPresentationImage> Create(ImageSop imageSop)
		{
			return _defaultInstance.CreateImages(imageSop);
		}

		/// <summary>
		/// Creates an appropriate subclass of <see cref="BasicPresentationImage"/>
		/// based on the <see cref="Frame"/>'s photometric interpretation.
		/// </summary>
		public static IPresentationImage Create(Frame frame)
		{
			return _defaultInstance.CreateImage(frame);
		}

		#region Private KO Helpers

		private ImageSop FindReferencedImageSop(string sopInstanceUid, string studyInstanceUid)
		{
			if (string.IsNullOrEmpty(sopInstanceUid))
				return null;

			string sameStudyUid = studyInstanceUid;
			Study sameStudy = _studyTree.GetStudy(sameStudyUid);

			if (sameStudy != null)
			{
				foreach (Series series in sameStudy.Series)
				{
					Sop referencedSop = series.Sops[sopInstanceUid];
					if (referencedSop != null)
						return referencedSop as ImageSop;
				}
			}

			return null;
		}

		private Sop FindReferencedSop(string sopInstanceUid, string studyInstanceUid)
		{
			if (string.IsNullOrEmpty(sopInstanceUid))
				return null;

			string sameStudyUid = studyInstanceUid;
			Study sameStudy = _studyTree.GetStudy(sameStudyUid);

			if (sameStudy != null)
			{
				foreach (Series series in sameStudy.Series)
				{
					Sop referencedSop = series.Sops[sopInstanceUid];
					if (referencedSop != null)
						return referencedSop;
				}
			}

			return null;
		}

		#endregion
	}
}