#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
					info.PositionPatientTopLeft = frame.ImagePlaneHelper.ConvertToPatient(new PointF(0, 0));
					info.PositionPatientTopRight = frame.ImagePlaneHelper.ConvertToPatient(new PointF(width, 0));
					info.PositionPatientBottomLeft = frame.ImagePlaneHelper.ConvertToPatient(new PointF(0, height));
					info.PositionPatientBottomRight = frame.ImagePlaneHelper.ConvertToPatient(new PointF(width, height));
					info.PositionPatientCenterOfImage = frame.ImagePlaneHelper.ConvertToPatient(new PointF(width / 2F, height / 2F));
					
					info.Normal = frame.ImagePlaneHelper.GetNormalVector();
					
					if (info.PositionPatientCenterOfImage == null || info.Normal == null)
						return null;

					// here, we want the position in the coordinate system of the image plane, 
					// without moving the origin (e.g. leave it at the patient origin).
					info.PositionImagePlaneTopLeft = frame.ImagePlaneHelper.ConvertToImagePlane(info.PositionPatientTopLeft, Vector3D.Empty);

					_sopInfoDictionary[frame.ParentImageSop.SopInstanceUID] = info;
				}
				else
				{
					info = _sopInfoDictionary[frame.ParentImageSop.SopInstanceUID];
				}

				return info;
			}
		}

		// helper method; don't want to add a new class just for this.
		public static float ComputeAngleBetweenNormals(ImageInfo info1, ImageInfo info2)
		{
			Vector3D n1 = info1.Normal.Normalize();
			Vector3D n2 = info2.Normal.Normalize();

			// the vectors are already normalized, so we don't need to divide by the magnitudes.
			float dot = n1.Dot(n2);

			if (dot < -1F)
				dot = -1F;
			if (dot > 1F)
				dot = 1F;

			return Math.Abs((float)Math.Acos(dot));
		}
	}
}
