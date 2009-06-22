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
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.Iods;
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.Dicom.Iod.Macros.DocumentRelationship;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Dicom.Utilities;
using ValueType=ClearCanvas.Dicom.Iod.ValueType;

namespace ClearCanvas.ImageViewer.KeyObjects
{
	/// <summary>
	/// Represents a single key image content item being serialized in the key object document.
	/// </summary>
	/// <remarks>
	/// <para>Due to the relatively new nature of key object support in the ClearCanvas Framework, this API may be more prone to changes in the next release.</para>
	/// </remarks>
	public class KeyObjectContentItem
	{
		internal KeyObjectContentItem(Frame frame, Sop presentationStateSop)
		{
			this.Frame = frame;
			this.PresentationStateSop = presentationStateSop;

			if (presentationStateSop != null)
			{
				if (presentationStateSop.SopClassUID == SopClass.GrayscaleSoftcopyPresentationStateStorageSopClassUid)
					GrayscalePresentationStateIod = new GrayscaleSoftcopyPresentationStateIod(presentationStateSop.DataSource);
				else if (presentationStateSop.SopClassUID == SopClass.ColorSoftcopyPresentationStateStorageSopClassUid)
					ColorPresentationStateIod = new ColorSoftcopyPresentationStateIod(presentationStateSop.DataSource);
			}
		}

		/// <summary>
		/// Gets the frame that is being serialized.
		/// </summary>
		public readonly Frame Frame;

		/// <summary>
		/// Gets the presentation state SOP instance associated with this frame.
		/// </summary>
		public readonly Sop PresentationStateSop;

		/// <summary>
		/// Gets the grayscale softcopy presentation state decoded from the <see cref="PresentationStateSop"/>.
		/// </summary>
		public readonly GrayscaleSoftcopyPresentationStateIod GrayscalePresentationStateIod;

		/// <summary>
		/// Gets the color softcopy presentation state decoded from the <see cref="PresentationStateSop"/>.
		/// </summary>
		public readonly ColorSoftcopyPresentationStateIod ColorPresentationStateIod;
	}

	/// <summary>
	/// A class for deserializing a key image series into the constituent images and associated presentation states.
	/// </summary>
	/// <remarks>
	/// <para>Due to the relatively new nature of key object support in the ClearCanvas Framework, this API may be more prone to changes in the next release.</para>
	/// </remarks>
	public class KeyImageDeserializer
	{
		private readonly StudyTree _studyTree;
		private readonly KeyObjectSelectionDocumentIod _document;

		/// <summary>
		/// Constructs a new instance of <see cref="KeyImageDeserializer"/>.
		/// </summary>
		/// <remarks>
		/// <para>Due to the relatively new nature of key object support in the ClearCanvas Framework, this API may be more prone to changes in the next release.</para>
		/// </remarks>
		public KeyImageDeserializer(Sop sourceSop, StudyTree studyTree)
		{
			_studyTree = studyTree;
			_document = new KeyObjectSelectionDocumentIod(sourceSop.DataSource);
		}

		/// <summary>
		/// Constructs a new instance of <see cref="KeyImageDeserializer"/>.
		/// </summary>
		/// <remarks>
		/// <para>Due to the relatively new nature of key object support in the ClearCanvas Framework, this API may be more prone to changes in the next release.</para>
		/// </remarks>
		public KeyImageDeserializer(KeyObjectSelectionDocumentIod iod, StudyTree studyTree)
		{
			_studyTree = studyTree;
			_document = iod;
		}

		/// <summary>
		/// Deserializes the key object selection SOP instance into a list of constituent images and associated presentation states.
		/// </summary>
		public IList<KeyObjectContentItem> Deserialize()
		{
			List<KeyObjectContentItem> contentItems = new List<KeyObjectContentItem>();

			SrDocumentContentModuleIod srDocument = _document.SrDocumentContent;
			foreach (IContentSequence contentItem in srDocument.ContentSequence)
			{
				if (contentItem.RelationshipType == RelationshipType.Contains)
				{
					if (contentItem.ValueType == ValueType.Image)
					{
						try
						{
							IImageReferenceMacro imageReference = contentItem;
							string referencedSopInstanceUid = imageReference.ReferencedSopSequence.ReferencedSopInstanceUid;
							ImageSop referencedImage = FindReferencedImageSop(referencedSopInstanceUid);
							if (referencedImage == null)
							{
								Platform.Log(LogLevel.Warn, "Unable to find referenced image '{0}'.", referencedSopInstanceUid);
								continue;
							}

							Sop presentationState = null;
							if(imageReference.ReferencedSopSequence.ReferencedSopSequence != null)
							{
								presentationState = FindReferencedSop(imageReference.ReferencedSopSequence.ReferencedSopSequence.ReferencedSopInstanceUid);
							}

							string referencedFrameNumbers = imageReference.ReferencedSopSequence.ReferencedFrameNumber;
							int[] frameNumbers;
							if (DicomStringHelper.TryGetIntArray(referencedFrameNumbers, out frameNumbers) && frameNumbers.Length > 0)
							{
								foreach (int frameNumber in frameNumbers)
								{
									if (frameNumber > referencedImage.Frames.Count)
									{
										Platform.Log(LogLevel.Warn, "Unable to find referenced frame number {0} for instance '{1}'.", frameNumber, referencedSopInstanceUid);
									}
									else
									{
										KeyObjectContentItem item = new KeyObjectContentItem(referencedImage.Frames[frameNumber], presentationState);
										contentItems.Add(item);
									}
								}
							}
							else
							{
								foreach (Frame frame in referencedImage.Frames) {
									KeyObjectContentItem item = new KeyObjectContentItem(frame, presentationState);
									contentItems.Add(item);
								}
							}
						}
						catch (Exception ex)
						{
							Platform.Log(LogLevel.Warn, ex, "Error realizing key object selection content item of value type {0}.", contentItem.ValueType);
							continue;
						}
					}
					else
					{
						Platform.Log(LogLevel.Warn, "Unsupported key object selection content item of value type {0}.", contentItem.ValueType);
						continue;
					}

				}
			}

			return contentItems.AsReadOnly();
		}

		private ImageSop FindReferencedImageSop(string sopInstanceUid)
		{
			string sameStudyUid = _document.GeneralStudy.StudyInstanceUid;
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

		private Sop FindReferencedSop(string sopInstanceUid) {
			string sameStudyUid = _document.GeneralStudy.StudyInstanceUid;
			Study sameStudy = _studyTree.GetStudy(sameStudyUid);

			if (sameStudy != null) {
				foreach (Series series in sameStudy.Series) {
					Sop referencedSop = series.Sops[sopInstanceUid];
					if (referencedSop != null)
						return referencedSop;
				}
			}

			return null;
		}
	}
}
