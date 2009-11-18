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

using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Rules;

namespace ClearCanvas.ImageServer.Core
{
	public class ApplySopRulesCommand : ServerCommand
	{
		private readonly ServerActionContext _context;
		private readonly ServerRulesEngine _engine;

		public ApplySopRulesCommand(ServerActionContext context, ServerRulesEngine engine)
			: base("Apply SOP Rules Engine and insert Archival Request", false)
		{
			_context = context;
			_engine = engine;
		}

		protected override void OnExecute(ServerCommandProcessor theProcessor)
		{
			// Run the rules engine against the object.
			_engine.Execute(_context);

			// Do insert into the archival queue.  Note that we re-run this with each object processed
			// so that the scheduled time is pushed back each time.  Note, however, if the study only 
			// has one image, we could incorrectly insert an ArchiveQueue request, since the 
			// study rules haven't been run.  We re-run the command when the study processed
			// rules are run to remove out the archivequeue request again, if it isn't needed.
			_context.CommandProcessor.AddCommand(
				new InsertArchiveQueueCommand(_context.ServerPartitionKey, _context.StudyLocationKey));
		}

		protected override void OnUndo()
		{
			
		}
	}
}