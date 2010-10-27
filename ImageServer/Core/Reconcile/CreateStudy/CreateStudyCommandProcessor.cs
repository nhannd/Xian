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
using ClearCanvas.ImageServer.Core.Reconcile.MergeStudy;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Reconcile.CreateStudy
{
    /// <summary>
	/// A processor implementing <see cref="IReconcileProcessor"/> to handle "CreateStudy" operation
	/// </summary>
	class ReconcileCreateStudyProcessor : ReconcileProcessorBase, IReconcileProcessor
	{
		#region Private Members

        #endregion

		#region Constructors
		/// <summary>
		/// Create an instance of <see cref="ReconcileCreateStudyProcessor"/>
		/// </summary>
		public ReconcileCreateStudyProcessor()
			: base("Create Study")
		{

		}

		#endregion

		#region IReconcileProcessor Members


		public void Initialize(ReconcileStudyProcessorContext context, bool complete)
		{
			Platform.CheckForNullReference(context, "context");
			Context = context;

			ReconcileCreateStudyDescriptor desc = XmlUtils.Deserialize<ReconcileCreateStudyDescriptor>(Context.History.ChangeDescription);
			

			if (Context.History.DestStudyStorageKey == null)
			{
				CreateStudyCommand command = new CreateStudyCommand(Context, desc.Commands, complete);
				AddCommand(command);
			}
			else
			{
				MergeStudyCommand command = new MergeStudyCommand(Context, false, desc.Commands, complete);
				AddCommand(command);
			}

            if (complete)
            {
                AddCleanupCommands();
            }
		}

        #endregion      
	}
}