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

		public byte[] GetFrameNormalizedPixelData(int frameNumber)
		{
			CheckIsImage();
			byte[] pixelData;
			OnGetFrameNormalizedPixelData(frameNumber, out pixelData);
			return pixelData;
		}

		public virtual void UnloadFrameData(int frameNumber)
		{
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

		protected abstract void OnGetFrameNormalizedPixelData(int frameNumber, out byte[] pixelData);

		#region IDicomAttributeProvider Members

		public DicomAttribute this[DicomTag tag]
		{
			get { return this[tag.TagValue]; }
			set { this[tag.TagValue] = value; }
		}

		public DicomAttribute this[uint tag]
		{
			get { return GetDicomAttribute(tag); }
			set
			{
				throw new NotSupportedException("SopDataSource objects should be considered read-only.");
			}
		}

		public abstract DicomAttribute GetDicomAttribute(uint tag);

		public bool TryGetAttribute(DicomTag tag, out DicomAttribute attribute)
		{
			return TryGetAttribute(tag.TagValue, out attribute);
		}

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
