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
	/// A class for deserializing a key image series into the constituent images and associated presentation states.
	/// </summary>
	/// <remarks>
	/// <para>Due to the relatively new nature of key object support in the ClearCanvas Framework, this API may be more prone to changes in the next release.</para>
	/// </remarks>
	public class KeyImageDeserializer
	{
		private readonly KeyObjectSelectionDocumentIod _document;

		/// <summary>
		/// Constructs a new instance of <see cref="KeyImageDeserializer"/>.
		/// </summary>
		/// <remarks>
		/// <para>Due to the relatively new nature of key object support in the ClearCanvas Framework, this API may be more prone to changes in the next release.</para>
		/// </remarks>
		public KeyImageDeserializer(Sop sourceSop)
		{
			_document = new KeyObjectSelectionDocumentIod(sourceSop.DataSource);
		}

		/// <summary>
		/// Constructs a new instance of <see cref="KeyImageDeserializer"/>.
		/// </summary>
		/// <remarks>
		/// <para>Due to the relatively new nature of key object support in the ClearCanvas Framework, this API may be more prone to changes in the next release.</para>
		/// </remarks>
		public KeyImageDeserializer(KeyObjectSelectionDocumentIod iod)
		{
			_document = iod;
		}

		/// <summary>
		/// Deserializes the key object selection SOP instance into a list of constituent images and associated presentation states.
		/// </summary>
		public IList<IKeyObjectContentItem> Deserialize()
		{
			List<IKeyObjectContentItem> contentItems = new List<IKeyObjectContentItem>();

			SrDocumentContentModuleIod srDocument = _document.SrDocumentContent;
			foreach (IContentSequence contentItem in srDocument.ContentSequence)
			{
				if (contentItem.RelationshipType == RelationshipType.Contains)
				{
					if (contentItem.ValueType == ValueType.Image)
					{
						IImageReferenceMacro imageReference = contentItem;
						string referencedSopInstanceUid = imageReference.ReferencedSopSequence.ReferencedSopInstanceUid;
						string presentationStateSopInstanceUid = null;

						if(imageReference.ReferencedSopSequence.ReferencedSopSequence != null)
						{
							presentationStateSopInstanceUid = imageReference.ReferencedSopSequence.ReferencedSopSequence.ReferencedSopInstanceUid;
						}

						string referencedFrameNumbers = imageReference.ReferencedSopSequence.ReferencedFrameNumber;
						int[] frameNumbers;
						if (!string.IsNullOrEmpty(referencedFrameNumbers)
							&& DicomStringHelper.TryGetIntArray(referencedFrameNumbers, out frameNumbers) && frameNumbers.Length > 0)
						{
							foreach (int frameNumber in frameNumbers)
							{
								KeyImageContentItem item = new KeyImageContentItem(referencedSopInstanceUid, frameNumber, presentationStateSopInstanceUid, _document);
								contentItems.Add(item);
							}
						}
						else
						{
							KeyImageContentItem item = new KeyImageContentItem(referencedSopInstanceUid, presentationStateSopInstanceUid, _document);
							contentItems.Add(item);
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
	}
}
