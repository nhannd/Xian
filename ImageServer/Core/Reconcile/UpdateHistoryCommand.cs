#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.Command;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Core.Reconcile.CreateStudy;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Core.Reconcile
{
    
	/// <summary>
	/// Command to update the study history record
	/// </summary>
	public class UpdateHistorySeriesMappingCommand : ServerDatabaseCommand
    {
        private readonly UidMapper _map;
        private readonly StudyHistory _studyHistory;
	    private readonly StudyStorageLocation _destStudy;

	    public UpdateHistorySeriesMappingCommand(StudyHistory studyHistory, StudyStorageLocation destStudy, UidMapper map) 
            : base("Update Study History Series Mapping")
        {
            _map = map;
            _studyHistory = studyHistory;
            _destStudy = destStudy;
        }

        protected override void OnExecute(CommandProcessor theProcessor, IUpdateContext updateContext)
        {
            IStudyHistoryEntityBroker historyUpdateBroker = updateContext.GetBroker<IStudyHistoryEntityBroker>();
        	StudyHistoryUpdateColumns parms = new StudyHistoryUpdateColumns {DestStudyStorageKey = _destStudy.Key};

        	if (_map != null)
            {
                // replace the mapping in the history
                StudyReconcileDescriptor changeLog = XmlUtils.Deserialize<StudyReconcileDescriptor>(_studyHistory.ChangeDescription);
                changeLog.SeriesMappings = new System.Collections.Generic.List<SeriesMapping>(_map.GetSeriesMappings());
                parms.ChangeDescription = XmlUtils.SerializeAsXmlDoc(changeLog);
            }

            historyUpdateBroker.Update(_studyHistory.Key, parms);
        }
    }
}