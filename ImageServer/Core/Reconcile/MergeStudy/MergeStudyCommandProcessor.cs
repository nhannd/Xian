#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Data;

namespace ClearCanvas.ImageServer.Core.Reconcile.MergeStudy
{
	/// <summary>
	/// A processor implementing <see cref="IReconcileProcessor"/> to handle "MergeStudy" operation
	/// </summary>
    class MergeStudyCommandProcessor : ReconcileProcessorBase, IReconcileProcessor
	{
	    public MergeStudyCommandProcessor()
            : base("Merge Study Processor")
		{

		}

		#region IReconcileProcessor Members

		public void Initialize(ReconcileStudyProcessorContext context, bool complete)
		{
			Platform.CheckForNullReference(context, "context");
			Context = context;

			ReconcileMergeToExistingStudyDescriptor desc =
				XmlUtils.Deserialize<ReconcileMergeToExistingStudyDescriptor>(Context.History.ChangeDescription);

			MergeStudyCommand command = new MergeStudyCommand(Context,
			                                                  context.History.DestStudyStorageKey == null,
			                                                  desc.Commands,
			                                                  complete);
			AddCommand(command);

			if (complete)
			{
				AddCleanupCommands();
			}
		}

		#endregion
	}
}