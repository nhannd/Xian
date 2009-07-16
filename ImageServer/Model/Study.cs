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
                if (_series == null)
                {
                    lock (SyncRoot)
                    {
                        using (IReadContext readContext = _store.OpenReadContext())
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
    }
}
