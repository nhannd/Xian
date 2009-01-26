using System;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public abstract class StandardSopDataSource : SopDataSource
	{
		private readonly object _syncLock = new object();
		private volatile WeakReference[] _framePixelData;

		protected StandardSopDataSource(bool isStored)
			: base(isStored)
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
			WeakReference reference = FramePixelData[frameIndex];

			try
			{
				pixelData = reference.Target as byte[];
			}
			catch(InvalidOperationException)
			{
				pixelData = null;
				reference = new WeakReference(null);
				FramePixelData[frameIndex] = reference;
			}

			if (!reference.IsAlive || pixelData == null)
			{
				lock(_syncLock)
				{
					pixelData = CreateFrameNormalizedPixelData(frameNumber);
					reference.Target = pixelData;
				}
			}
		}

		//NOTE: no need to implement anything here, at least for pixel data, since we're using a WeakReferenceCache.
		public override void UnloadFrameData(int frameNumber)
		{
		}
	}
}
