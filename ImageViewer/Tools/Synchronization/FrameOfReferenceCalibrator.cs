using System.Collections.Generic;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Tools.Synchronization
{
	public partial class StackingSynchronizationTool
	{
		private class FrameOfReferenceCalibrator
		{
			#region Key class

			private class Key
			{
				public Key(string studyInstanceUid, string frameOfReferenceUid, Vector3D normal)
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

					if (obj is Key)
					{
						Key other = (Key)obj;
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

			private readonly Dictionary<Key, Dictionary<Key, Vector3D>> _calibrationMap;
			private readonly float _angleTolerance;

			public FrameOfReferenceCalibrator(float angleTolerance)
			{
				_calibrationMap = new Dictionary<Key, Dictionary<Key, Vector3D>>();
				_angleTolerance = angleTolerance;
			}

			private static Key CreateKey(DicomImagePlane plane)
			{
				string frameOfReferenceUid = plane.FrameOfReferenceUid;
				string studyInstanceUid = plane.StudyInstanceUid;
				Vector3D normal = plane.Normal;

				return new Key(studyInstanceUid, frameOfReferenceUid, normal);
			}

			private Dictionary<Key, Vector3D> GetOffsetDictionary(Key referenceKey)
			{
				if (_calibrationMap.ContainsKey(referenceKey))
					return _calibrationMap[referenceKey];
				else
					_calibrationMap[referenceKey] = new Dictionary<Key, Vector3D>();

				return _calibrationMap[referenceKey];
			}

			private Vector3D ExtrapolateOffset(Key referenceKey, Key targetKey, List<Key> eliminatedKeys)
			{
				Vector3D relativeOffset = null;

				foreach (Key relatedKey in _calibrationMap[referenceKey].Keys)
				{
					if (eliminatedKeys.Contains(relatedKey))
						continue;

					Vector3D offset = GetOffset(relatedKey, targetKey, eliminatedKeys);
					if (offset != null)
					{
						//again, find the smallest of all possible offsets.
						offset += _calibrationMap[referenceKey][relatedKey];
						if (relativeOffset == null || offset.Magnitude < relativeOffset.Magnitude)
							relativeOffset = offset;
					}
				}

				return relativeOffset;
			}

			private Vector3D GetOffset(Key referenceKey, Key targetKey, List<Key> eliminatedKeys)
			{
				if (referenceKey.Equals(targetKey))
					return null;

				// This 'reference key' has now been checked against 'targetKey', so whether it 
				// has a direct dependency or not, it should not be considered again,
				// otherwise, we could end up in infinite recursion.
				eliminatedKeys.Add(referenceKey);

				if (_calibrationMap[referenceKey].ContainsKey(targetKey))
					return _calibrationMap[referenceKey][targetKey];
				else
					return ExtrapolateOffset(referenceKey, targetKey, eliminatedKeys);
			}

			public Vector3D GetOffset(DicomImagePlane referencePlane, DicomImagePlane targetPlane)
			{
				Vector3D offset = null;

				Key referenceKey = CreateKey(referencePlane);
				if (_calibrationMap.ContainsKey(referenceKey))
				{
					Key targetKey = CreateKey(targetPlane);
					if (!referenceKey.Equals(targetKey))
						offset = GetOffset(referenceKey, targetKey, new List<Key>());
				}

				return offset;
			}
			

			public void Calibrate(DicomImagePlane referencePlane, DicomImagePlane targetPlane)
			{
				if (!referencePlane.IsInSameFrameOfReference(targetPlane) && referencePlane.IsParallelTo(targetPlane, _angleTolerance))
				{
					Key referenceKey = CreateKey(referencePlane);
					Key targetKey = CreateKey(targetPlane);

					Dictionary<Key, Vector3D> referenceOffsets = GetOffsetDictionary(referenceKey);
					Dictionary<Key, Vector3D> targetOffsets = GetOffsetDictionary(targetKey);

					Vector3D offset = targetPlane.PositionPatientCenterOfImage - referencePlane.PositionPatientCenterOfImage;
					
					referenceOffsets[targetKey] = offset;
					targetOffsets[referenceKey] = -offset;
				}
			}
		}
	}
}
