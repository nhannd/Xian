using System.Collections.Generic;
using ClearCanvas.Dicom.Iod.Macros;

namespace ClearCanvas.Dicom.Iod
{
	public class ImageSopInstanceReferenceDictionary
	{
		private readonly Dictionary<string, IList<int>> _frameDictionary = new Dictionary<string, IList<int>>();
		private readonly Dictionary<string, IList<uint>> _segmentDictionary = new Dictionary<string, IList<uint>>();

		public ImageSopInstanceReferenceDictionary(IEnumerable<ImageSopInstanceReferenceMacro> imageSopReferences)
		{
			foreach (ImageSopInstanceReferenceMacro imageSopReference in imageSopReferences)
			{
				DicomAttributeIS frames = imageSopReference.ReferencedFrameNumber;
				List<int> frameList = null;
				if (!frames.IsNull && !frames.IsEmpty && frames.Count > 0)
				{
					frameList = new List<int>();
					for (int n = 0; n < frames.Count; n++)
						frameList.Add(frames.GetInt32(n, -1));
				}
				_frameDictionary.Add(imageSopReference.ReferencedSopInstanceUid, frameList);

				DicomAttributeUS segments = imageSopReference.ReferencedSegmentNumber;
				List<uint> segmentList = null;
				if (!segments.IsNull && !segments.IsEmpty && segments.Count > 0)
				{
					segmentList = new List<uint>();
					for (int n = 0; n < segments.Count; n++)
						segmentList.Add(segments.GetUInt32(n, 0));
				}
				_segmentDictionary.Add(imageSopReference.ReferencedSopInstanceUid, segmentList);
			}
		}

		public bool ReferencesSop(string imageSopInstanceUid)
		{
			return ReferencesAny(imageSopInstanceUid);
		}

		public bool ReferencesAny(string imageSopInstanceUid)
		{
			if (_frameDictionary.ContainsKey(imageSopInstanceUid))
				return true;
			return false;
		}

		public bool ReferencesAllFrames(string imageSopInstanceUid)
		{
			if (_frameDictionary.ContainsKey(imageSopInstanceUid))
			{
				IList<int> frames = _frameDictionary[imageSopInstanceUid];
				if (frames == null)
					return true;
			}
			return false;
		}

		public bool ReferencesAllSegments(string imageSopInstanceUid)
		{
			if (_segmentDictionary.ContainsKey(imageSopInstanceUid))
			{
				IList<uint> segments = _segmentDictionary[imageSopInstanceUid];
				if (segments == null)
					return true;
			}
			return false;
		}

		public bool ReferencesFrame(string imageSopInstanceUid, int frameNumber)
		{
			if (_frameDictionary.ContainsKey(imageSopInstanceUid))
			{
				IList<int> frames = _frameDictionary[imageSopInstanceUid];
				if (frames == null || frames.Contains(frameNumber))
					return true;
			}
			return false;
		}

		public bool ReferencesSegment(string imageSopInstanceUid, uint segmentNumber)
		{
			if (_segmentDictionary.ContainsKey(imageSopInstanceUid))
			{
				IList<uint> segments = _segmentDictionary[imageSopInstanceUid];
				if (segments == null || segments.Contains(segmentNumber))
					return true;
			}
			return false;
		}
	}
}