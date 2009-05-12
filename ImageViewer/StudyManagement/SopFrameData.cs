using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public interface ISopFrameData
	{
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

		public SopDataSource Parent
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
	}
}