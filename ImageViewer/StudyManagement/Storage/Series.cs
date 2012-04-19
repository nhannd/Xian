#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.Common.WorkItem;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage
{
	internal class Series : ISeries
    {
		#region Private Fields

		private readonly Study _parentStudy;
		private readonly SeriesXml _seriesXml;
		private IList<ISopInstance> _sopInstances;

		#endregion

		internal Series(Study parentStudy, SeriesXml seriesXml)
        {
			_parentStudy = parentStudy;
			_seriesXml = seriesXml;
		}

        internal Study ParentStudy { get { return _parentStudy; } }

	    #region Private Members

		private IList<ISopInstance> SopInstances
		{
			get
			{
				if (_sopInstances == null)
				    _sopInstances = _seriesXml.Select(instance => (ISopInstance)new SopInstance(this, instance)).ToList();

				return _sopInstances;
			}	
		}

		private InstanceXml GetFirstSopInstanceXml()
		{
            try
            {
                return _seriesXml.First();
            }
            catch (Exception e)
            {
                string message = String.Format("There are no instances in this series ({0}).", SeriesInstanceUid);
                throw new Exception(message, e);
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

		public string StudyInstanceUid
		{
			get { return _parentStudy.StudyInstanceUid; }
		}

		public string SeriesInstanceUid
		{
			get { return _seriesXml.SeriesInstanceUid; }
		}

		public string Modality
		{
			get { return GetFirstSopInstanceXml()[DicomTags.Modality].GetString(0, ""); }
		}

		public string SeriesDescription
		{
			get { return GetFirstSopInstanceXml()[DicomTags.SeriesDescription].GetString(0, ""); }
		}

		public int SeriesNumber
		{
			get { return GetFirstSopInstanceXml()[DicomTags.SeriesNumber].GetInt32(0, 0); }
		}

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

	    public int[] BitsAllocatedInSeries
	    {
	        get 
            { 
                return SopInstances.Cast<SopInstance>()
                    .Where(s => s.BitsAllocated.HasValue)
                    .Select(s => s.BitsAllocated).Cast<int>().Distinct().ToArray(); 
            }
	    }

        public int[] BitsStoredInSeries
        {
            get
            {
                return SopInstances.Cast<SopInstance>()
                    .Where(s => s.BitsStored.HasValue)
                    .Select(s => s.BitsStored).Cast<int>().Distinct().ToArray();
            }
        }

        public string[] PhotometricInterpretationsInSeries
        {
            get
            {
                return SopInstances.Cast<SopInstance>()
                    .Where(s => !String.IsNullOrEmpty(s.PhotometricInterpretation))
                    .Select(s => s.PhotometricInterpretation).Distinct().ToArray();
            }
        }

        public string[] SourceAETitlesInSeries
        {
            get
            {
                return SopInstances.Cast<SopInstance>()
                    .Where(s => !String.IsNullOrEmpty(s.SourceAETitle))
                    .Select(s => s.SourceAETitle).Distinct().ToArray();
            }
        }

        public string[] TransferSyntaxesInSeries
        {
            get
            {
                return SopInstances.Cast<SopInstance>()
                    .Where(s => !String.IsNullOrEmpty(s.TransferSyntaxUid))
                    .Select(s => s.TransferSyntaxUid).Distinct().ToArray();
            }
        }

	    internal SeriesEntry ToStoreEntry()
        {
            var entry = new SeriesEntry
            {
                Series = new SeriesIdentifier(this)
                {
                    InstanceAvailability = "ONLINE",
                    RetrieveAeTitle = Utilities.GetLocalServerAETitle(),    
                    SpecificCharacterSet = SpecificCharacterSet
                },
                Data = new SeriesEntryData
                {
                    DeleteTime = GetScheduledDeleteTime(),
                    SourceAETitlesInSeries = SourceAETitlesInSeries
                }
            };
            return entry;
        }

        private DateTime? GetScheduledDeleteTime()
        {
            //TODO (Marmot):Fill this in when we have the request object.

            return null;
            //using (var context = new DataAccessContext())
            //{
            //    var broker = context.GetWorkItemBroker();
            //    var items = broker.GetWorkItems(WorkItemTypeEnum.SeriesDelete, null, StudyInstanceUid);
            //    if (items == null || items.Count == 0)
            //        return null;

            //    var validItems = items.Where(w => w.Status != WorkItemStatusEnum.Failed && w.Status != WorkItemStatusEnum.Canceled);
            //    var requests = validItems.Select(w => w.Request).OfType<>()
            //}
        }
    }
}
