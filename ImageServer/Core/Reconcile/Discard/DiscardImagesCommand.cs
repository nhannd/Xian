#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Command;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Reconcile.Discard
{
	/// <summary>
	/// Command for discarding images that need to be reconciled.
	/// </summary>
	class DiscardImagesCommand : ReconcileCommandBase
	{
		#region Constructors

		public DiscardImagesCommand(ReconcileStudyProcessorContext context)
			: base("Discard image", false, context)
		{
		}

		#endregion

		#region Overidden Protected Methods

		protected override void OnExecute(CommandProcessor theProcessor)
		{
			Platform.CheckForNullReference(Context, "Context");
			Platform.CheckForNullReference(Context.ReconcileWorkQueueData, "Context.ReconcileWorkQueueData");

			foreach (WorkQueueUid uid in Context.WorkQueueUidList)
			{

			    string imagePath = GetReconcileUidPath(uid);

				try
				{
					using (
						ServerCommandProcessor processor = new ServerCommandProcessor(String.Format("Deleting {0}", uid.SopInstanceUid)))
					{
						FileDeleteCommand deleteFile = new FileDeleteCommand(imagePath, true);
						DeleteWorkQueueUidCommand deleteUid = new DeleteWorkQueueUidCommand(uid);
						processor.AddCommand(deleteFile);
						processor.AddCommand(deleteUid);
						Platform.Log(ServerPlatform.InstanceLogLevel, deleteFile.ToString());
						if (!processor.Execute())
						{
							throw new Exception(String.Format("Unable to discard image {0}", uid.SopInstanceUid));
						}
					}
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e, "Unexpected exception discarding file: {0}", imagePath);
					SopInstanceProcessor.FailUid(uid, true);
				}
			}
		}

		protected override void OnUndo()
		{

		}

		#endregion
	}
}