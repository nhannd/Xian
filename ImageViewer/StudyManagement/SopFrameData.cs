using ClearCanvas.Common;
using System;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public interface ISopFrameData : IDisposable
	{
		ISopDataSource Parent { get; }
		int FrameNumber { get; }

		byte[] GetNormalizedPixelData();
		byte[] GetNormalizedOverlayData(int overlayGroupNumber, int overlayFrameNumber);
		void Unload();
	}

	public abstract class SopFrameData : ISopFrameData
	{
		private readonly SopDataSource _parent;
		private readonly int _frameNumber;

		protected SopFrameData(int frameNumber, SopDataSource parent)
		{
			Platform.CheckForNullReference(parent, "parent");
			Platform.CheckPositive(frameNumber, "frameNumber");

			_parent = parent;
			_frameNumber = frameNumber;
		}

		public ISopDataSource Parent
		{
			get { return _parent; }
		}

		public int FrameNumber
		{
			get { return _frameNumber; }
		}

		public abstract byte[] GetNormalizedPixelData();

		public abstract byte[] GetNormalizedOverlayData(int overlayGroupNumber, int overlayFrameNumber);

		public abstract void Unload();

		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Error, e, "An unexpected error has occurred while disposing the frame data.");
			}
		}

		protected virtual void Dispose(bool disposing)
		{
		}
	}
}