using System.Collections.Generic;
using ClearCanvas.ImageViewer.Mathematics;
using System.Threading;
using ClearCanvas.ImageViewer.StudyManagement;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Tools.Synchronization
{
	internal class ImageInfo
	{
		public Vector3D Normal;
		public Vector3D PositionPatientTopLeft;
		public Vector3D PositionPatientTopRight;
		public Vector3D PositionPatientBottomLeft;
		public Vector3D PositionPatientBottomRight;
		public Vector3D PositionPatientCenterOfImage;
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

		public ImageInfo GetImageInformation(Frame frame)
		{
			lock (_syncLock)
			{
				ImageInfo info;

				if (!_sopInfoDictionary.ContainsKey(frame.ParentImageSop.SopInstanceUID))
				{
					int height = frame.Rows - 1;
					int width = frame.Columns - 1;

					info = new ImageInfo();
					info.PositionPatientTopLeft = frame.ImagePlaneHelper.ConvertToPatient(PointF.Empty);
					info.PositionPatientTopRight = frame.ImagePlaneHelper.ConvertToPatient(new PointF(width, 0));
					info.PositionPatientBottomLeft = frame.ImagePlaneHelper.ConvertToPatient(new PointF(0, height));
					info.PositionPatientBottomRight = frame.ImagePlaneHelper.ConvertToPatient(new PointF(width, height));
					info.PositionPatientCenterOfImage = frame.ImagePlaneHelper.ConvertToPatient(new PointF(width / 2F, height / 2F));
					
					info.Normal = frame.ImagePlaneHelper.GetNormalVector();
					
					if (info.PositionPatientCenterOfImage == null || info.Normal == null)
						return null;

					//force the normal vector to be a unit vector, just in case of bad/imprecise data.
					info.Normal = info.Normal.Normalize();

					// here, we want the position in the coordinate system of the image plane, without moving the origin.
					info.PositionImagePlaneTopLeft = frame.ImagePlaneHelper.ConvertToImage(info.PositionPatientTopLeft, Vector3D.Empty);

					_sopInfoDictionary[frame.ParentImageSop.SopInstanceUID] = info;
				}
				else
				{
					info = _sopInfoDictionary[frame.ParentImageSop.SopInstanceUID];
				}

				return info;
			}
		}
	}
}
