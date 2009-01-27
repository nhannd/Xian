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
using System.Drawing;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Comparers
{
	/// <summary>
	/// Compares two <see cref="Frame"/>s based on slice location.
	/// </summary>
	public class SliceLocationComparer : DicomFrameComparer
	{
		/// <summary>
		/// Initializes a new instance of <see cref="SliceLocationComparer"/>.
		/// </summary>
		public SliceLocationComparer()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="SliceLocationComparer"/>.
		/// </summary>
		public SliceLocationComparer(bool reverse)
			: base(reverse)
		{
		}

		private static IEnumerable<IComparable> GetCompareValues(Frame frame)
		{
			yield return frame.FrameOfReferenceUid;

			double? normalX = null, normalY = null, normalZ = null;
			double? zImagePlane = null;

			Vector3D normal = frame.ImagePlaneHelper.GetNormalVector();
			if (normal != null)
			{
				// Return the 3 components of the image normal; if they are all equal
				// then the images are in the same plane.  We are disregarding
				// the rare case where the 2 normals being compared are the negative
				// of each other - technically, they could be considered to be in the
				// same 'plane', but for the purposes of sorting, we won't consider it.
				normalX = Math.Round(normal.X, 3, MidpointRounding.AwayFromZero);
				normalY = Math.Round(normal.Y, 3, MidpointRounding.AwayFromZero);
				normalZ = Math.Round(normal.Z, 3, MidpointRounding.AwayFromZero);
				
				Vector3D positionPatient = frame.ImagePlaneHelper.ConvertToPatient(new PointF((frame.Columns - 1) / 2F, (frame.Rows - 1) / 2F));
				if (positionPatient != null)
				{
					Vector3D positionImagePlane = frame.ImagePlaneHelper.ConvertToImagePlane(positionPatient, Vector3D.Null);

					//return only the z-component of the image plane position (where the origin remains at the patient origin).
					zImagePlane = Math.Round(positionImagePlane.Z, 3, MidpointRounding.AwayFromZero);
				}
			}

			yield return normalX;
			yield return normalY;
			yield return normalZ;
			yield return zImagePlane;

			//as a last resort.
			yield return frame.ParentImageSop.InstanceNumber;
			yield return frame.FrameNumber;
			yield return frame.AcquisitionNumber;
		}

		/// <summary>
		/// Compares two <see cref="Frame"/>s based on slice location.
		/// </summary>
		public override int Compare(Frame x, Frame y)
		{
			return Compare(GetCompareValues(x), GetCompareValues(y));
		}
	}
}
