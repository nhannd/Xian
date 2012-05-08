#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.Common.StudyManagement.Rules;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Storage;

namespace ClearCanvas.ImageViewer.Dicom.Core
{
    /// <summary>
    /// Reapply Rules Utility class.
    /// </summary>
    public class ReapplyRulesUtility
    {
        #region Private members

        private event EventHandler<ReindexUtility.StudyEventArgs> _studyProcessedEvent;
        private readonly object _syncLock = new object();
        private readonly ReapplyRulesRequest _request;
        private RuleData _rule;
        #endregion

        #region Public Events

        public class StudyEventArgs : EventArgs
        {
            public string StudyInstanceUid;
        }    
    
        public event EventHandler<ReindexUtility.StudyEventArgs> StudyProcessedEvent
        {
            add
            {
                lock (_syncLock)
                {
                    _studyProcessedEvent += value;
                }
            }
            remove
            {
                lock (_syncLock)
                {
                    _studyProcessedEvent -= value;
                }
            }
        }

        #endregion

        #region Public Properties

        public int DatabaseStudiesToScan { get; private set; }
     
        public List<long> StudyOidList { get; private set; }

        #endregion

        #region Constructors

        public ReapplyRulesUtility(ReapplyRulesRequest request)
        {
            _request = request;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initialize the Reindex.  Determine the number of studies in the database and the number of folders on disk to be used
        /// for progress.
        /// </summary>
        public void Initialize()
        {
            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var broker = context.GetStudyBroker();

                StudyOidList = broker.GetStudyOids();

                var ruleBroker = context.GetRuleBroker();
                var rule = ruleBroker.GetRule(_request.RuleId);
                if (rule != null)
                    _rule = rule.RuleData;
            }

            DatabaseStudiesToScan = StudyOidList.Count;           
        }

        /// <summary>
        /// Reapply the rules.
        /// </summary>
        public void Process()
        {            
            ProcessStudiesInDatabase();            
        }

        #endregion

        #region Private Methods
    
        private void ProcessStudiesInDatabase()
        {
            var ep = new RulesEngineExtensionPoint();
            var ruleExtensions = ep.CreateExtensions();
 

            foreach (long oid in StudyOidList)
            {
                try
                {
                    using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
                    {
                        var broker = context.GetStudyBroker();

                        var study = broker.GetStudy(oid);

                     
                        StudyEntry studyEntry = study.ToStoreEntry();

                        foreach (IRulesEngine engine in ruleExtensions)
                        {
                            engine.ApplyStudyToRule(_request.RulesEngineContext, studyEntry,_rule);
                        }


                        EventsHelper.Fire(_studyProcessedEvent, this, new ReindexUtility.StudyEventArgs { StudyInstanceUid = study.StudyInstanceUid });
                    }                    
                }
                catch (Exception x)
                {
                    Platform.Log(LogLevel.Warn, "Unexpected exception attempting to reindex StudyOid {0}: {1}", oid, x.Message);
                }
            }
        }

        #endregion
    }
}
