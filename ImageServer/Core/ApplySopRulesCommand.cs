#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.ImageServer.Common.Command;
using ClearCanvas.ImageServer.Rules;

namespace ClearCanvas.ImageServer.Core
{
	public class ApplySopRulesCommand : CommandBase
	{
		private readonly ServerActionContext _context;
		private readonly ServerRulesEngine _engine;

		public ApplySopRulesCommand(ServerActionContext context, ServerRulesEngine engine)
			: base("Apply SOP Rules Engine and insert Archival Request", false)
		{
			_context = context;
			_engine = engine;
		}

		protected override void OnExecute(CommandProcessor theProcessor)
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