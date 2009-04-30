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

using System.Collections.Generic;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Tools.Synchronization
{
	public partial class StackingSynchronizationTool
	{
		private class FrameOfReferenceCalibrator
		{
			#region Plane class

			private class Plane
			{
				public Plane(string studyInstanceUid, string frameOfReferenceUid, Vector3D normal)
				{
					FrameOfReferenceUid = frameOfReferenceUid;
					StudyInstanceUid = studyInstanceUid;
					Normal = normal;
				}

				public readonly string FrameOfReferenceUid;
				public readonly string StudyInstanceUid;
				public readonly Vector3D Normal;

				public override bool Equals(object obj)
				{
					if (obj == this)
						return true;

					if (obj is Plane)
					{
						Plane other = (Plane)obj;
						return other.FrameOfReferenceUid == FrameOfReferenceUid &&
							other.StudyInstanceUid == StudyInstanceUid &&
							other.Normal.Equals(Normal);
					}

					return false;
				}

				public override int GetHashCode()
				{
					return 3 * FrameOfReferenceUid.GetHashCode() + 5 * StudyInstanceUid.GetHashCode() + 7 * Normal.GetHashCode();
				}
			}

			#endregion

			private readonly Dictionary<Plane, Dictionary<Plane, Vector3D>> _calibrationMatrix;
			private readonly float _angleTolerance;

			public FrameOfReferenceCalibrator(float angleTolerance)
			{
				_calibrationMatrix = new Dictionary<Plane, Dictionary<Plane, Vector3D>>();
				_angleTolerance = angleTolerance;
			}

			private static Plane FromDicomImagePlane(DicomImagePlane plane)
			{
				string frameOfReferenceUid = plane.FrameOfReferenceUid;
				string studyInstanceUid = plane.StudyInstanceUid;
				Vector3D normal = plane.Normal;

				return new Plane(studyInstanceUid, frameOfReferenceUid, normal);
			}

			private Dictionary<Plane, Vector3D> GetOffsetDictionary(Plane referencePlane)
			{
				if (_calibrationMatrix.ContainsKey(referencePlane))
					return _calibrationMatrix[referencePlane];
				else
					_calibrationMatrix[referencePlane] = new Dictionary<Plane, Vector3D>();

				return _calibrationMatrix[referencePlane];
			}

			private Vector3D ExtrapolateOffset(Plane referencePlane, Plane targetPlane, List<Plane> eliminatedPlanes)
			{
				Vector3D relativeOffset = null;

				foreach (Plane relatedPlane in _calibrationMatrix[referencePlane].Keys)
				{
					if (eliminatedPlanes.Contains(relatedPlane))
						continue;

					Vector3D offset = GetOffset(relatedPlane, targetPlane, eliminatedPlanes);
					if (offset != null)
					{
						//again, find the smallest of all possible offsets.
						offset += _calibrationMatrix[referencePlane][relatedPlane];
						if (relativeOffset == null || offset.Magnitude < relativeOffset.Magnitude)
							relativeOffset = offset;
					}
				}

				return relativeOffset;
			}

			private Vector3D GetOffset(Plane referencePlane, Plane targetPlane, List<Plane> eliminatedPlane)
			{
				if (referencePlane.Equals(targetPlane))
					return null;

				// This 'reference plane' has now been checked against 'target plane', so whether it 
				// has a direct dependency or not, it should not be considered again,
				// otherwise, we could end up in infinite recursion.
				eliminatedPlane.Add(referencePlane);

				if (_calibrationMatrix[referencePlane].ContainsKey(targetPlane))
					return _calibrationMatrix[referencePlane][targetPlane];
				else
					return ExtrapolateOffset(referencePlane, targetPlane, eliminatedPlane);
			}
			
			public Vector3D GetOffset(DicomImagePlane referenceImagePlane, DicomImagePlane targetImagePlane)
			{
				Vector3D offset = null;

				Plane referencePlane = FromDicomImagePlane(referenceImagePlane);
				if (_calibrationMatrix.ContainsKey(referencePlane))
				{
					Plane targetPlane = FromDicomImagePlane(targetImagePlane);
					if (!referencePlane.Equals(targetPlane))
						offset = GetOffset(referencePlane, targetPlane, new List<Plane>());
				}

				return offset;
			}
			

			public void Calibrate(DicomImagePlane referenceImagePlane, DicomImagePlane targetImagePlane)
			{
				if (!referenceImagePlane.IsInSameFrameOfReference(targetImagePlane) && referenceImagePlane.IsParallelTo(targetImagePlane, _angleTolerance))
				{
					Plane referencePlane = FromDicomImagePlane(referenceImagePlane);
					Plane targetPlane = FromDicomImagePlane(targetImagePlane);

					Dictionary<Plane, Vector3D> referenceOffsets = GetOffsetDictionary(referencePlane);
					Dictionary<Plane, Vector3D> targetOffsets = GetOffsetDictionary(targetPlane);

					Vector3D offset = targetImagePlane.PositionPatientCenterOfImage - referenceImagePlane.PositionPatientCenterOfImage;
					
					referenceOffsets[targetPlane] = offset;
					targetOffsets[referencePlane] = -offset;
				}
			}
		}
	}
}
