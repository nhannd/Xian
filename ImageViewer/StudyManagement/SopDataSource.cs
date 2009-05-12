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
using ClearCanvas.Dicom;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public abstract class SopDataSource : ISopDataSource
	{
		private string _studyLoaderName;
		private object _server;

		protected SopDataSource()
		{
		}

		#region ISopDataSource Members

		public virtual string StudyInstanceUid
		{
			get { return this[DicomTags.StudyInstanceUid].GetString(0, null); }
		}

		public virtual string SeriesInstanceUid
		{
			get { return this[DicomTags.SeriesInstanceUid].GetString(0, null); }
		}

		public virtual string SopInstanceUid
		{
			get { return this[DicomTags.SopInstanceUid].GetString(0, null); }
		}

		public virtual string SopClassUid
		{
			get { return this[DicomTags.SopClassUid].GetString(0, null); }
		}

		public virtual string TransferSyntaxUid
		{
			get { return this[DicomTags.TransferSyntaxUid].GetString(0, String.Empty); }
		}

		public virtual int InstanceNumber
		{
			get { return this[DicomTags.InstanceNumber].GetInt32(0, 0); }
		}

		public virtual bool IsStored
		{
			get { return !String.IsNullOrEmpty(_studyLoaderName); }
		}
		
		public string StudyLoaderName
		{
			get { return _studyLoaderName; }
			internal protected set { _studyLoaderName = value; }
		}

		public object Server
		{
			get { return _server; }
			internal protected set { _server = value; }
		}

		[Obsolete("Use ISopDataSource.GetFrameData(frameNumber).GetNormalizedPixelData() instead.")]
		public byte[] GetFrameNormalizedPixelData(int frameNumber)
		{
			return this.GetFrameData(frameNumber).GetNormalizedPixelData();
		}

		[Obsolete("Use ISopDataSource.GetFrameData(frameNumber).GetNormalizedOverlayData(overlayGroupNumber, overlayFrameNumber) instead.")]
		public byte[] GetFrameNormalizedOverlayData(int overlayNumber, int frameNumber)
		{
			return this.GetFrameData(frameNumber).GetNormalizedOverlayData(overlayNumber, frameNumber);
		}

		[Obsolete("Use ISopDataSource.GetFrameData(frameNumber).Unload() instead.")]
		public virtual void UnloadFrameData(int frameNumber)
		{
			this.GetFrameData(frameNumber).Unload();
		}

		public ISopFrameData GetFrameData(int frameNumber)
		{
			CheckIsImage();
			return OnGetFrameData(frameNumber);
		}

		#endregion

		protected int NumberOfFrames
		{
			get
			{
				CheckIsImage();
				return this[DicomTags.NumberOfFrames].GetInt32(0, 1);
			}
		}

		protected bool IsImage
		{
			get { return SopDataHelper.IsImageSop(SopClass.GetSopClass(this.SopClassUid)); }
		}

		protected void CheckIsImage()
		{
			if (!IsImage)
				throw new InvalidOperationException("This functionality cannot be used for non-images.");
		}

		protected abstract ISopFrameData OnGetFrameData(int frameNumber);

		[Obsolete("This formerly abstract method is no longer called.")]
		protected virtual void OnGetFrameNormalizedPixelData(int frameNumber, out byte[] pixelData)
		{
			pixelData = null;
		}

		[Obsolete("This formerly abstract method is no longer called.")]
		protected virtual void OnGetFrameNormalizedOverlayData(int overlayNumber, int frameNumber, out byte[] pixelData)
		{
			pixelData = null;
		}

		#region IDicomAttributeProvider Members

		public DicomAttribute this[DicomTag tag]
		{
			get { return GetDicomAttribute(tag); }
		}

		public DicomAttribute this[uint tag]
		{
			get { return GetDicomAttribute(tag); }
		}

		DicomAttribute IDicomAttributeProvider.this[DicomTag tag]
		{
			get { return GetDicomAttribute(tag); }
			set { throw new NotSupportedException("SopDataSource objects should be considered read-only."); }
		}

		DicomAttribute IDicomAttributeProvider.this[uint tag]
		{
			get { return GetDicomAttribute(tag); }
			set { throw new NotSupportedException("SopDataSource objects should be considered read-only."); }
		}

		public abstract DicomAttribute GetDicomAttribute(DicomTag tag);

		public abstract DicomAttribute GetDicomAttribute(uint tag);

		public abstract bool TryGetAttribute(DicomTag tag, out DicomAttribute attribute);

		public abstract bool TryGetAttribute(uint tag, out DicomAttribute attribute);

		#endregion

		protected virtual void Dispose(bool disposing)
		{
		}

		#region IDisposable Members

		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Error, e, "An unexpected error has occurred while disposing the sop data source.");
			}
		}

		#endregion
	}
}
