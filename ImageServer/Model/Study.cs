using System;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Model
{
    public partial class Study
    {
        #region Private Fields
        static private readonly IPersistentStore _store = PersistentStoreRegistry.GetDefaultStore();
        private readonly object _syncRoot = new object();
        private IList<Series> _series = null;
        private Patient _patient=null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the <see cref="Series"/> related to this study.
        /// </summary>
        public IList<Series> Series
        {
            get
            {
                lock (_syncRoot)
                {
                    if (_series == null)
                    {
                        using(IReadContext readContext = _store.OpenReadContext())
                        {
                            ISeriesEntityBroker broker = readContext.GetBroker<ISeriesEntityBroker>();
                            SeriesSelectCriteria criteria = new SeriesSelectCriteria();
                            criteria.StudyKey.EqualTo(this.GetKey());
                            _series = broker.Find(criteria);
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
                lock(_syncRoot)
                {
                    if (_patient==null)
                    {
                        _patient = Patient.Load(this.PatientKey);
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
    }
}
