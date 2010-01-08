#region License

// Copyright (c) 2010, ClearCanvas Inc.
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