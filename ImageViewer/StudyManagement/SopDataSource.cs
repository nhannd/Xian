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
	/// <summary>
	/// Abstract base implementation of an <see cref="ISopDataSource"/>.
	/// </summary>
	public abstract class SopDataSource : ISopDataSource
	{
		private string _studyLoaderName;
		private object _server;

		/// <summary>
		/// Constructs a new <see cref="SopDataSource"/>.
		/// </summary>
		protected SopDataSource()
		{
		}

		#region ISopDataSource Members

		/// <summary>
		/// Gets the study instance UID.
		/// </summary>
		public virtual string StudyInstanceUid
		{
			get { return this[DicomTags.StudyInstanceUid].GetString(0, null); }
		}

		/// <summary>
		/// Gets the series instance UID.
		/// </summary>
		public virtual string SeriesInstanceUid
		{
			get { return this[DicomTags.SeriesInstanceUid].GetString(0, null); }
		}

		/// <summary>
		/// Gets the SOP instance UID.
		/// </summary>
		public virtual string SopInstanceUid
		{
			get { return this[DicomTags.SopInstanceUid].GetString(0, null); }
		}

		/// <summary>
		/// Gets the SOP class UID.
		/// </summary>
		public virtual string SopClassUid
		{
			get { return this[DicomTags.SopClassUid].GetString(0, null); }
		}

		/// <summary>
		/// Gets the transfer syntax UID.
		/// </summary>
		public virtual string TransferSyntaxUid
		{
			get { return this[DicomTags.TransferSyntaxUid].GetString(0, String.Empty); }
		}

		/// <summary>
		/// Gets the instance number.
		/// </summary>
		public virtual int InstanceNumber
		{
			get { return this[DicomTags.InstanceNumber].GetInt32(0, 0); }
		}

		/// <summary>
		/// Gets a value indicating whether or not the SOP instance is 'stored',
		/// for example in the local data store or on a remote PACS server.
		/// </summary>
		/// <remarks>
		/// This would normally be used to determine whether an <see cref="ISopDataSource">data source</see>
		/// is one that is generated and only exists in memory, as the treatment of such a sop
		/// might be different in some cases.
		/// </remarks>
		public virtual bool IsStored
		{
			get { return !String.IsNullOrEmpty(_studyLoaderName); }
		}

		/// <summary>
		/// Gets the name of the <see cref="IStudyLoader"/> that created this data source.
		/// </summary>
		public string StudyLoaderName
		{
			get { return _studyLoaderName; }
			internal protected set { _studyLoaderName = value; }
		}

		/// <summary>
		/// Gets an object representing the source server where this data source was loaded from.
		/// </summary>
		/// <remarks>
		/// When non-null, this object is most likely an <see cref="ApplicationEntity"/>, but is not guaranteed to be one.
		/// </remarks>
		public object Server
		{
			get { return _server; }
			internal protected set { _server = value; }
		}

		/// <summary>
		/// Gets a value indicating whether or not the SOP instance is an image.
		/// </summary>
		public abstract bool IsImage { get; }

		/// <summary>
		/// Gets the number of frames in this SOP instance.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if the SOP instance is not an image.</exception>
		public int NumberOfFrames
		{
			get
			{
				CheckIsImage();
				return this[DicomTags.NumberOfFrames].GetInt32(0, 1);
			}
		}

		/// <summary>
		/// Gets the data for a particular frame in the SOP instance, if it is an image.
		/// </summary>
		/// <param name="frameNumber">The 1-based number of the frame for which the data is to be retrieved.</param>
		/// <returns>An <see cref="ISopFrameData"/> containing frame-specific data.</returns>
		/// <exception cref="InvalidOperationException">Thrown if this <see cref="ISopDataSource"/> is not an image
		/// (e.g. <see cref="ISopDataSource.IsImage"/> returns false).</exception>
		ISopFrameData ISopDataSource.GetFrameData(int frameNumber)
		{
			CheckIsImage();
			return GetFrameData(frameNumber);
		}

		#endregion

		/// <summary>
		/// Throws an <see cref="InvalidOperationException"/> if the SOP instance is not an image.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if the SOP instance is not an image.</exception>
		protected void CheckIsImage()
		{
			if (!IsImage)
				throw new InvalidOperationException("This functionality cannot be used for non-images.");
		}

		/// <summary>
		/// Gets the data for a particular frame in the SOP instance.
		/// </summary>
		/// <param name="frameNumber">The 1-based number of the frame for which the data is to be retrieved.</param>
		/// <returns>An <see cref="ISopFrameData"/> containing frame-specific data.</returns>
		protected abstract ISopFrameData GetFrameData(int frameNumber);

		/// <summary>
		/// Gets the <see cref="DicomAttribute"/> for the given tag.
		/// </summary>
		public abstract DicomAttribute this[DicomTag tag] { get; }

		/// <summary>
		/// Gets the <see cref="DicomAttribute"/> for the given tag.
		/// </summary>
		public abstract DicomAttribute this[uint tag] { get; }

		#region IDicomAttributeProvider Members

		/// <summary>
		/// Gets the <see cref="DicomAttribute"/> for the given tag.
		/// </summary>
		/// <exception cref="NotSupportedException">If the setter is accessed.</exception>
		DicomAttribute IDicomAttributeProvider.this[DicomTag tag]
		{
			get { return this[tag]; }
			set { throw new NotSupportedException("SopDataSource objects should be considered read-only."); }
		}

		/// <summary>
		/// Gets the <see cref="DicomAttribute"/> for the given tag.
		/// </summary>
		/// <exception cref="NotSupportedException">If the setter is accessed.</exception>
		DicomAttribute IDicomAttributeProvider.this[uint tag]
		{
			get { return this[tag]; }
			set { throw new NotSupportedException("SopDataSource objects should be considered read-only."); }
		}

		/// <summary>
		/// Attempts to get the attribute specified by <paramref name="tag"/>.
		/// </summary>
		public abstract bool TryGetAttribute(DicomTag tag, out DicomAttribute attribute);

		/// <summary>
		/// Attempts to get the attribute specified by <paramref name="tag"/>.
		/// </summary>
		public abstract bool TryGetAttribute(uint tag, out DicomAttribute attribute);

		#endregion

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern.
		/// </summary>
		/// <param name="disposing">A value indicating whether or not the object is being disposed.</param>
		protected virtual void Dispose(bool disposing)
		{
		}

		#region IDisposable Members

		/// <summary>
		/// Releases resources owned by this <see cref="SopDataSource"/>.
		/// </summary>
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
