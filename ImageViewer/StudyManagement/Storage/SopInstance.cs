#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Xml;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage
{
    internal class SopInstance : ISopInstance
    {
		private readonly Series _parentSeries;
		private readonly InstanceXml _xml;
		private readonly DicomAttributeCollection _metaInfo;

		internal SopInstance(Series parentSeries, InstanceXml instanceXml)
		{
			_parentSeries = parentSeries;
			_xml = instanceXml;
			_metaInfo = new DicomAttributeCollection();

			if (instanceXml.TransferSyntax != null)
			{
				string transferSyntax = instanceXml.TransferSyntax.UidString;
				if (!String.IsNullOrEmpty(transferSyntax))
					_metaInfo[DicomTags.TransferSyntaxUid].SetString(0, transferSyntax);
			}

			if (instanceXml.SopClass != null)
			{
				string sopClass = instanceXml.SopClass.Uid;
				if (!String.IsNullOrEmpty(sopClass))
					_metaInfo[DicomTags.SopClassUid].SetString(0, sopClass);
			}
		}

		#region ISopInstance Members

		public ISeries GetParentSeries()
		{
			return _parentSeries;
		}

		public string SpecificCharacterSet
		{
			get { return _xml[DicomTags.SpecificCharacterSet].ToString(); }
		}

		public string StudyInstanceUid
		{
			get { return _parentSeries.GetParentStudy().StudyInstanceUid; }
		}

		public string SeriesInstanceUid
		{
			get { return _parentSeries.SeriesInstanceUid; }
		}

		public string SopInstanceUid
		{
			get { return _xml.SopInstanceUid; }
		}

		public int InstanceNumber
		{
			get { return _xml[DicomTags.InstanceNumber].GetInt32(0, 0); }
		}

		public string SopClassUid
		{
			get
			{
				if (_xml.SopClass == null)
					return ""; //shouldn't happen.

				return _xml.SopClass.Uid;
			}
		}

		public string TransferSyntaxUid
		{
			get { return _xml.TransferSyntax.UidString; }
		}

		public string GetLocationUri()
		{
		    return _parentSeries.ParentStudy.StudyLocation.GetSopInstancePath(SeriesInstanceUid, SopInstanceUid);
		}

		public bool IsStoredTag(uint tag)
		{
			DicomTag dicomTag = DicomTagDictionary.GetDicomTag(tag);
			if (dicomTag == null)
				return false;

			return IsStoredTag(dicomTag);
		}

		public bool IsStoredTag(DicomTag tag)
		{
			Platform.CheckForNullReference(tag, "tag");

			if (_metaInfo.Contains(tag))
				return true;

			//if it's meta info, just defer to the file.
			if (tag.TagValue <= 0x0002FFFF)
				return false;

			if (_xml.IsTagExcluded(tag.TagValue))
				return false;

			if (tag.VR == DicomVr.SQvr)
			{
				var items = _xml[tag].Values as DicomSequenceItem[];
				if (items != null)
				{
					foreach (DicomSequenceItem item in items)
					{
						if (item is InstanceXmlDicomSequenceItem)
						{
							if (((InstanceXmlDicomSequenceItem)item).HasExcludedTags(true))
								return false;
						}
					}
				}
			}

			bool isBinary = tag.VR == DicomVr.OBvr || tag.VR == DicomVr.OWvr || tag.VR == DicomVr.OFvr;
			//these tags are not stored in the xml.
			if (isBinary || tag.IsPrivate || tag.VR == DicomVr.UNvr)
				return false;

			return true;
		}

    	public DicomAttribute this[DicomTag tag]
    	{
			get
			{
				DicomAttribute attribute;
				if (_metaInfo.TryGetAttribute(tag, out attribute))
					return attribute;

				return _xml[tag];
			}
    	}

		public DicomAttribute this[uint tag]
		{
			get
			{
				DicomAttribute attribute;
				if (_metaInfo.TryGetAttribute(tag, out attribute))
					return attribute;

				return _xml[tag];
			}
		}

		#endregion
	}
}
