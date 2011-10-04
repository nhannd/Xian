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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Model
{
    public partial class Study
    {
        #region Private Fields
        static private readonly IPersistentStore _store = PersistentStoreRegistry.GetDefaultStore();
        private IDictionary<string, Series> _series = null;
        private Patient _patient=null;
        #endregion

        #region Public Properties

        public bool HasAttachment
        {
            get
            {
                if (this.Series==null)
                    return false;

                return CollectionUtils.Contains(this.Series.Values, 
                    (series)=> !String.IsNullOrEmpty(series.Modality) && (series.Modality.Equals("SR") || series.Modality.Equals("DOC")));
            }
        }

        /// <summary>
        /// Gets the <see cref="Series"/> related to this study.
        /// </summary>
        public IDictionary<string, Series> Series
        {
            get
            {
                if (_series == null)
                {
                    lock (SyncRoot)
                    {
                        using (IReadContext readContext = _store.OpenReadContext())
                        {
                            ISeriesEntityBroker broker = readContext.GetBroker<ISeriesEntityBroker>();
                            SeriesSelectCriteria criteria = new SeriesSelectCriteria();
                            criteria.StudyKey.EqualTo(this.GetKey());
                            IList<Series> list = broker.Find(criteria);

                            _series = new Dictionary<string, Series>();
                            foreach(Series theSeries in list)
                            {
                                _series.Add(theSeries.SeriesInstanceUid, theSeries);
                            }
                        }
                    }
                }

                return _series;
            }
        }

        /// <summary>
        /// Gets the <see cref="Patient"/> related to this study
        /// </summary>
        public Patient Patient
        {
            get
            {
                if (_patient==null)
                {
                    lock (SyncRoot)
                    {
                        using (IReadContext readContext = _store.OpenReadContext())
                        {
                            _patient = Model.Patient.Load(this.PatientKey);
                        }
                    }
                }
                return _patient;
            }
        }

        #endregion

        /// <summary>
        /// Find a <see cref="Study"/> with the specified study instance uid on the given partition.
        /// </summary>
        /// <param name="studyInstanceUid"></param>
        /// <param name="partition"></param>
        /// <returns></returns>
        /// 
        static public Study Find(IPersistenceContext context, String studyInstanceUid, ServerPartition partition)
        {
            IStudyEntityBroker broker = context.GetBroker<IStudyEntityBroker>();
            StudySelectCriteria criteria = new StudySelectCriteria();
            criteria.ServerPartitionKey.EqualTo(partition.GetKey());
            criteria.StudyInstanceUid.EqualTo(studyInstanceUid);
            Study study = broker.FindOne(criteria);
            return study;
           
        }

        public Patient LoadPatient(IPersistenceContext context)
        {
            if (_patient==null)
            {
                lock (SyncRoot)
                {
                    if (_patient == null)
                    {
                        _patient = Patient.Load(context, PatientKey);
                    }
                }
            }
            return _patient;
        }


        public StudyStorage LoadStudyStorage(IPersistenceContext context)
        {
            return StudyStorage.Load(this.StudyStorageKey);
        }

        static public Study Find(IPersistenceContext context, ServerEntityKey studyStorageKey)
        {
            IStudyEntityBroker broker = context.GetBroker<IStudyEntityBroker>();
            StudySelectCriteria criteria = new StudySelectCriteria();
            criteria.StudyStorageKey.EqualTo( studyStorageKey);
            return broker.FindOne(criteria);
        }
    }
}
