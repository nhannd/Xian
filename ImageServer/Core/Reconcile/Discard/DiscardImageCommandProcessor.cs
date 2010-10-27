#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Core.Reconcile.Discard
{
	/// <summary>
	/// A processor implementing <see cref="IReconcileProcessor"/> to handle "Discard" operation
	/// </summary>
    class DiscardImageCommandProcessor : ReconcileProcessorBase, IReconcileProcessor
	{
		public DiscardImageCommandProcessor()
			: base("Discard Image Processor")
		{

		}
		#region IReconcileProcessor Members

		public void Initialize(ReconcileStudyProcessorContext context, bool complete)
		{
			Platform.CheckForNullReference(context, "context");
            Context = context;

			DiscardImagesCommand discard = new DiscardImagesCommand(context);

			AddCommand(discard);

            if (complete)
            {
                AddCleanupCommands();
            }
            
		}

		#endregion
	}
}