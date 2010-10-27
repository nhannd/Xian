#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Core.Reconcile.ProcessAsIs
{
	/// <summary>
	/// A processor implementing <see cref="IReconcileProcessor"/> to handle "MergeStudy" operation
	/// </summary>
    internal class ReconcileProcessAsIsProcessor : ReconcileProcessorBase, IReconcileProcessor
	{
		public ReconcileProcessAsIsProcessor()
			: base("Process As Is Processor")
		{

		}

		#region IReconcileProcessor Members

		public void Initialize(ReconcileStudyProcessorContext context, bool complete)
		{
			Platform.CheckForNullReference(context, "context");
			Context = context;

			ProcessAsIsCommand command = new ProcessAsIsCommand(Context, complete);

			AddCommand(command);

            if (complete)
            {
                AddCleanupCommands();
            }
		}

		#endregion
	}
}