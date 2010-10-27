#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Represents the collection of <see cref="WorkspaceDialogBox"/> objects for a given workspace.
	/// </summary>
	internal class WorkspaceDialogBoxCollection : DesktopObjectCollection<WorkspaceDialogBox>
	{
		private readonly Workspace _owner;

		/// <summary>
		/// Constructor.
		/// </summary>
		internal WorkspaceDialogBoxCollection(Workspace owner)
		{
			_owner = owner;
		}

		/// <summary>
		/// Creates a new dialog box with the specified arguments.
		/// </summary>
		internal WorkspaceDialogBox AddNew(DialogBoxCreationArgs args)
		{
			var dialogBox = CreateDialogBox(args);
			Open(dialogBox);
			return dialogBox;
		}

		/// <summary>
		/// Creates a new <see cref="WorkspaceDialogBox"/>.
		/// </summary>
		private WorkspaceDialogBox CreateDialogBox(DialogBoxCreationArgs args)
		{
			var factory = CollectionUtils.FirstElement<IWorkspaceDialogBoxFactory>(
				(new WorkspaceDialogBoxFactoryExtensionPoint()).CreateExtensions()) ?? new DefaultWorkspaceDialogBoxFactory();

			return factory.CreateWorkspaceDialogBox(args, _owner);
		}
	}
}
