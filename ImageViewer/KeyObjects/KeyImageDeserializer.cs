#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
