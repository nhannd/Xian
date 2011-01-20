#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom.Iod.Iods;
using ValueType=ClearCanvas.Dicom.Iod.ValueType;

namespace ClearCanvas.ImageViewer.KeyObjects
{
	/// <summary>
	/// A single key object content item in a key object document.
	/// </summary>
	/// <remarks>
	/// <para>Due to the relatively new nature of key object support in the ClearCanvas Framework, this API may be more prone to changes in the next release.</para>
	/// </remarks>
	public interface IKeyObjectContentItem
	{
		ValueType ValueType { get; }
		KeyObjectSelectionDocumentIod Source { get; }
	}

	/// <summary>
	/// A single key image content item in a key object document.
	/// </summary>
	/// <remarks>
	/// <para>Due to the relatively new nature of key object support in the ClearCanvas Framework, this API may be more prone to changes in the next release.</para>
	/// </remarks>
	public class KeyImageContentItem : IKeyObjectContentItem
	{
		public readonly string ReferencedImageSopInstanceUid;
		public readonly string PresentationStateSopInstanceUid;

		public readonly int? FrameNumber;

		private readonly KeyObjectSelectionDocumentIod _source;

		public KeyImageContentItem(string imageSopInstanceUid, string presentationSopInstanceUid, KeyObjectSelectionDocumentIod source)
		{
			this.ReferencedImageSopInstanceUid = imageSopInstanceUid;
			this.PresentationStateSopInstanceUid = presentationSopInstanceUid;
			this.FrameNumber = null;
			this._source = source;
		}

		public KeyImageContentItem(string imageSopInstanceUid, int frameNumber, string presentationSopInstanceUid, KeyObjectSelectionDocumentIod source)
			: this(imageSopInstanceUid, presentationSopInstanceUid, source)
		{
			this.FrameNumber = frameNumber;
		}

		ValueType IKeyObjectContentItem.ValueType
		{
			get { return ValueType.Image; }
		}

		public KeyObjectSelectionDocumentIod Source
		{
			get { return _source; }
		}
	}
}