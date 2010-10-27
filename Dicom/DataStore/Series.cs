#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities.Xml;

namespace ClearCanvas.Dicom.DataStore
{
	internal class Series : ISeries
    {
		#region Private Fields

		private readonly Study _parentStudy;
		private readonly SeriesXml _seriesXml;
		private List<ISopInstance> _sopInstances;

		#endregion

		internal Series(Study parentStudy, SeriesXml seriesXml)
        {
			_parentStudy = parentStudy;
			_seriesXml = seriesXml;
		}

		#region Private Members

		private List<ISopInstance> SopInstances
		{
			get
			{
				if (_sopInstances == null)
				{
					_sopInstances = new List<ISopInstance>();
					foreach (InstanceXml instanceXml in _seriesXml)
						_sopInstances.Add(new SopInstance(this, instanceXml));
				}

				return _sopInstances;
			}	
		}

		private InstanceXml GetFirstSopInstanceXml()
		{
			using (IEnumerator<InstanceXml> iterator = _seriesXml.GetEnumerator())
			{
				if (!iterator.MoveNext())
				{
					string message = String.Format("There are no instances in this series ({0}).", SeriesInstanceUid);
					throw new DataStoreException(message);
				}

				return iterator.Current;
			}
		}

		#endregion

		#region ISeries Members

		public IStudy GetParentStudy()
		{
			return _parentStudy;
		}

		public string SpecificCharacterSet
		{
			get { return GetFirstSopInstanceXml()[DicomTags.SpecificCharacterSet].ToString(); }
		}

		[QueryableProperty(DicomTags.StudyInstanceUid, IsHigherLevelUnique = true)]
		public string StudyInstanceUid
		{
			get { return _parentStudy.StudyInstanceUid; }
		}

		[QueryableProperty(DicomTags.SeriesInstanceUid, IsUnique = true, PostFilterOnly = true)]
		public string SeriesInstanceUid
		{
			get { return _seriesXml.SeriesInstanceUid; }
		}

		[QueryableProperty(DicomTags.Modality, PostFilterOnly = true)]
		public string Modality
		{
			get
			{
				return GetFirstSopInstanceXml()[DicomTags.Modality].GetString(0, "");
			}
		}

		[QueryableProperty(DicomTags.SeriesDescription, PostFilterOnly = true)]
		public string SeriesDescription
		{
			get
			{
				return GetFirstSopInstanceXml()[DicomTags.SeriesDescription].GetString(0, "");
			}
		}

		[QueryableProperty(DicomTags.SeriesNumber, IsRequired = true, PostFilterOnly = true)]
		public int SeriesNumber
		{
			get
			{
				return GetFirstSopInstanceXml()[DicomTags.SeriesNumber].GetInt32(0, 0);
			}
		}

		[QueryableProperty(DicomTags.NumberOfSeriesRelatedInstances, PostFilterOnly = true)]
		public int NumberOfSeriesRelatedInstances
		{
			get { return SopInstances.Count; }
		}

		int? ISeriesData.NumberOfSeriesRelatedInstances
		{
			get { return NumberOfSeriesRelatedInstances; }
		}

    	public IEnumerable<ISopInstance> GetSopInstances()
        {
    		return SopInstances;
        }

        #endregion
	}
}
