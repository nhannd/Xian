using System.Collections.Generic;
using ClearCanvas.ImageViewer.Mathematics;
using System.Threading;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Synchronization
{
	internal class ImageInfo
	{
		public Matrix RotationMatrix;
		public Vector3D Normal;
		public Vector3D PositionPatientTopLeft;
		public Vector3D PositionPatientCenterOfImage;
		public Vector3D PositionPatientBottomRight;
		public Vector3D PositionImagePlaneTopLeft;
	}

	internal class SopInfoCache
	{
		private static readonly SopInfoCache _cache = new SopInfoCache();

		private readonly Dictionary<string, ImageInfo> _sopInfoDictionary;
		private readonly object _syncLock;

		private int _referenceCount;

		private SopInfoCache()
		{
			_referenceCount = 0;
			_syncLock = new object();
			_sopInfoDictionary = new Dictionary<string, ImageInfo>();
		}

		public static SopInfoCache Get()
		{
			lock (_cache._syncLock)
			{
				++_cache._referenceCount;
			}

			return _cache;
		}

		public void Release()
		{
			lock (_syncLock)
			{
				if (_referenceCount > 0)
					--_referenceCount;

				if (_referenceCount == 0)
					_sopInfoDictionary.Clear();
			}
		}

		public ImageInfo GetImageInformation(ImageSop sop)
		{
			lock (_syncLock)
			{
				ImageInfo info;

				if (!_sopInfoDictionary.ContainsKey(sop.SopInstanceUID))
				{
					info = new ImageInfo();
					info.PositionPatientTopLeft = ImagePositionHelper.SourceToPatientTopLeftOfImage(sop);
					info.PositionPatientCenterOfImage = ImagePositionHelper.SourceToPatientCenterOfImage(sop);
					info.PositionPatientBottomRight = ImagePositionHelper.SourceToPatientBottomRightOfImage(sop);
					info.RotationMatrix = ImagePositionHelper.GetRotationMatrix(sop);

					if (info.PositionPatientTopLeft == null || info.PositionPatientBottomRight == null || info.RotationMatrix == null)
						return null;

					info.Normal = new Vector3D(info.RotationMatrix[2, 0], info.RotationMatrix[2, 1], info.RotationMatrix[2, 2]);

					// Transform the position (patient) vector to the coordinate system of the image.
					// This way, the z-components will all be along the same vector path.
					Matrix positionPatientMatrix = new Matrix(3, 1);
					positionPatientMatrix.SetColumn(0, info.PositionPatientTopLeft.X, info.PositionPatientTopLeft.Y,
					                                info.PositionPatientTopLeft.Z);
					Matrix result = info.RotationMatrix*positionPatientMatrix;

					info.PositionImagePlaneTopLeft = new Vector3D(result[0, 0], result[1, 0], result[2, 0]);

					_sopInfoDictionary[sop.SopInstanceUID] = info;
				}
				else
				{
					info = _sopInfoDictionary[sop.SopInstanceUID];
				}

				return info;
			}
		}
	}
}
