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

using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Core.Reconcile.CreateStudy;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Core.Reconcile
{
	/// <summary>
	/// Command to update the study history record
	/// </summary>
	class UpdateHistoryCommand : ServerDatabaseCommand<ReconcileStudyProcessorContext>
	{
	    private readonly UidMapper _uidMap;
	    private readonly StudyReconcileDescriptor _changeLog;
        
        public UpdateHistoryCommand(ReconcileStudyProcessorContext context, UidMapper uidMap)
			: base("UpdateHistoryCommand", true, context)
	    {
	        _uidMap = uidMap;
	        _changeLog = XmlUtils.Deserialize<StudyReconcileDescriptor>(context.History.ChangeDescription);
	    }

        
	    protected override void OnExecute(ServerCommandProcessor theProcessor, IUpdateContext updateContext)
		{
			IStudyHistoryEntityBroker historyUpdateBroker = updateContext.GetBroker<IStudyHistoryEntityBroker>();
			StudyHistoryUpdateColumns parms = new StudyHistoryUpdateColumns();
			parms.DestStudyStorageKey = Context.DestStorageLocation.Key;

            if (_uidMap!=null)
            {
                _changeLog.SeriesMappings = new System.Collections.Generic.List<SeriesMapping>(_uidMap.GetSeriesMappings());
                parms.ChangeDescription = XmlUtils.SerializeAsXmlDoc(_changeLog);
            }

			historyUpdateBroker.Update(Context.History.Key, parms);
		}
	}
}