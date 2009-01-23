using System.Collections.Generic;
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.Dicom.Iod.Macros.PresentationStateRelationship;

namespace ClearCanvas.Dicom.Iod
{
	public class SeriesReferenceDictionary
	{
		private readonly Dictionary<string, ImageSopInstanceReferenceDictionary> _dictionary = new Dictionary<string, ImageSopInstanceReferenceDictionary>();

		public SeriesReferenceDictionary(IEnumerable<IReferencedSeriesSequence> seriesReferences)
		{
			foreach (IReferencedSeriesSequence seriesReference in seriesReferences)
			{
				ImageSopInstanceReferenceDictionary imageSopDictionary = null;
				ImageSopInstanceReferenceMacro[] imageSopReferences = seriesReference.ReferencedImageSequence;

				if (imageSopReferences != null && imageSopReferences.Length > 0)
				{
					imageSopDictionary = new ImageSopInstanceReferenceDictionary(imageSopReferences);
				}

				_dictionary.Add(seriesReference.SeriesInstanceUid, imageSopDictionary);
			}
		}

		public bool ReferencesSeries(string seriesInstanceUid)
		{
			if (_dictionary.ContainsKey(seriesInstanceUid))
				return true;
			return false;
		}

		public bool ReferencesSop(string seriesInstanceUid, string sopInstanceUid)
		{
			if (_dictionary.ContainsKey(seriesInstanceUid))
			{
				ImageSopInstanceReferenceDictionary sopDictionary = _dictionary[seriesInstanceUid];
				if (sopDictionary == null || sopDictionary.ReferencesSop(sopInstanceUid))
					return true;
			}
			return false;
		}

		public bool ReferencesAllFrames(string seriesInstanceUid, string sopInstanceUid)
		{
			if (_dictionary.ContainsKey(seriesInstanceUid))
			{
				ImageSopInstanceReferenceDictionary sopDictionary = _dictionary[seriesInstanceUid];
				if (sopDictionary == null || sopDictionary.ReferencesAllFrames(sopInstanceUid))
					return true;
			}
			return false;
		}

		public bool ReferencesAllSegments(string seriesInstanceUid, string sopInstanceUid)
		{
			if (_dictionary.ContainsKey(seriesInstanceUid))
			{
				ImageSopInstanceReferenceDictionary sopDictionary = _dictionary[seriesInstanceUid];
				if (sopDictionary == null || sopDictionary.ReferencesAllSegments(sopInstanceUid))
					return true;
			}
			return false;
		}

		public bool ReferencesFrame(string seriesInstanceUid, string sopInstanceUid, int frameNumber)
		{
			if (_dictionary.ContainsKey(seriesInstanceUid))
			{
				ImageSopInstanceReferenceDictionary sopDictionary = _dictionary[seriesInstanceUid];
				if (sopDictionary == null || sopDictionary.ReferencesFrame(sopInstanceUid, frameNumber))
					return true;
			}
			return false;
		}

		public bool ReferencesSegment(string seriesInstanceUid, string sopInstanceUid, uint segmentNumber)
		{
			if (_dictionary.ContainsKey(seriesInstanceUid))
			{
				ImageSopInstanceReferenceDictionary sopDictionary = _dictionary[seriesInstanceUid];
				if (sopDictionary == null || sopDictionary.ReferencesSegment(sopInstanceUid, segmentNumber))
					return true;
			}
			return false;
		}
	}
}