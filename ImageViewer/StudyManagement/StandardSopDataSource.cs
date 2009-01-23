using System;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public abstract class StandardSopDataSource : SopDataSource
	{
		private readonly object _syncLock = new object();
		private volatile WeakReference[] _framePixelData;

		public StandardSopDataSource()
		{
		}

		private WeakReference[] FramePixelData
		{
			get
			{
				if (_framePixelData == null)
				{
					lock(_syncLock)
					{
						if (_framePixelData == null)
						{
							_framePixelData = new WeakReference[NumberOfFrames];
							for (int i = 0; i < _framePixelData.Length; ++i)
								_framePixelData[i] = new WeakReference(null);
						}
					}
				}

				return _framePixelData;
			}
		}

		protected abstract byte[] CreateFrameNormalizedPixelData(int frameNumber);

		protected sealed override void OnGetFrameNormalizedPixelData(int frameNumber, out byte[] pixelData)
		{
			int frameIndex = frameNumber - 1;

			if (FramePixelData[frameIndex].IsAlive)
			{
				pixelData = (byte[])FramePixelData[frameIndex].Target;
			}
			else
			{
				lock(_syncLock)
				{
					pixelData = CreateFrameNormalizedPixelData(frameNumber);
					FramePixelData[frameIndex].Target = pixelData;
				}
			}
		}

		//NOTE: no need to implement anything here, at least for pixel data, since we're using a WeakReferenceCache.
		public override void UnloadFrameData(int frameNumber)
		{
		}
	}
}
