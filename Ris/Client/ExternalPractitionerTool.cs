#region License

// Copyright (c) 2010, ClearCanvas Inc.
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

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	public abstract class ExternalPractitionerTool :  Tool<IExternalPractitionerItemToolContext>
	{
		public virtual bool Enabled
		{
			get { return this.Context.SelectedItems.Count == 1; }
		}

		public event EventHandler EnabledChanged
		{
			add { this.Context.SelectionChanged += value; }
			remove { this.Context.SelectionChanged -= value; }
		}
	}

	[MenuAction("apply", "folderexplorer-items-contextmenu/Verify", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Verify", "Apply")]
	[Tooltip("apply", "Verify External Practitioner Information")]
	[IconSet("apply", IconScheme.Colour, "Icons.VerifyPractitionerToolSmall.png", "Icons.VerifyPractitionerToolMedium.png", "Icons.VerifyPractitionerToolLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.ExternalPractitioner)]
	// Bug #7342: disabling this tool because it has been deemed too accessible and error prone, but leaving the code here just in case
	//[ExtensionOf(typeof(ExternalPractitionerItemToolExtensionPoint))]
	public class ExternalPractitionerVerifyTool : ExternalPractitionerTool
	{
		public override bool Enabled
		{
			get
			{
				var item = (ExternalPractitionerSummary)this.Context.Selection.Item;
				return base.Enabled && item.IsVerified == false;
			}
		}

		public void Apply()
		{
			try
			{
				var item = (ExternalPractitionerSummary)this.Context.Selection.Item;

				Platform.GetService(
					delegate(IExternalPractitionerAdminService service)
						{
							var editRequest = new LoadExternalPractitionerForEditRequest(item.PractitionerRef);
							var editResponse = service.LoadExternalPractitionerForEdit(editRequest);

							var updateRequest = new UpdateExternalPractitionerRequest(editResponse.PractitionerDetail, true);
							service.UpdateExternalPractitioner(updateRequest);
						});

				DocumentManager.InvalidateFolder(typeof(UnverifiedFolder));
				DocumentManager.InvalidateFolder(typeof(VerifiedTodayFolder));
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}
	}

	[MenuAction("apply", "folderexplorer-items-contextmenu/Edit External Practitioner", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Edit External Practitioner", "Apply")]
	[Tooltip("apply", "Edit External Practitioner Information")]
	[IconSet("apply", IconScheme.Colour, "Icons.EditToolSmall.png", "Icons.EditToolSmall.png", "Icons.EditToolLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.ExternalPractitioner)]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.ExternalPractitioner.Update)]
	[ExtensionOf(typeof(ExternalPractitionerItemToolExtensionPoint))]
	public class ExternalPractitionerEditTool : ExternalPractitionerTool
	{
		public void Apply()
		{
			var item = (ExternalPractitionerSummary)this.Context.Selection.Item;

			var editor = new ExternalPractitionerEditorComponent(item.PractitionerRef);
			var exitCode = ApplicationComponent.LaunchAsDialog(
				this.Context.DesktopWindow, editor, SR.TitleUpdateExternalPractitioner + " - " + Formatting.PersonNameFormat.Format(item.Name));
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				DocumentManager.InvalidateFolder(typeof(UnverifiedFolder));
				DocumentManager.InvalidateFolder(typeof(VerifiedTodayFolder));
			}
		}
	}

	[MenuAction("apply", "folderexplorer-items-contextmenu/Merge External Practitioners", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Merge External Practitioners", "Apply")]
	[Tooltip("apply", "Merge Duplicate External Practitioners")]
	[IconSet("apply", IconScheme.Colour, "Icons.MergePersonToolSmall.png", "Icons.MergePersonToolSmall.png", "Icons.MergePersonToolMedium.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.ExternalPractitioner)]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.ExternalPractitioner.Merge)]
	[ExtensionOf(typeof(ExternalPractitionerItemToolExtensionPoint))]
	public class ExternalPractitionerMergeTool : ExternalPractitionerTool
	{
		public override bool Enabled
		{
			get { return this.Context.SelectedItems.Count == 1 || this.Context.SelectedItems.Count == 2; }
		}

		public void Apply()
		{
			var firstItem = CollectionUtils.FirstElement(this.Context.SelectedItems);
			var secondItem = this.Context.SelectedItems.Count > 1 ? CollectionUtils.LastElement(this.Context.SelectedItems) : null;
			var editor = new ExternalPractitionerMergeNavigatorComponent(firstItem.PractitionerRef, secondItem == null ? null : secondItem.PractitionerRef);

			var title = SR.TitleMergePractitioner + " - " + Formatting.PersonNameFormat.Format(firstItem.Name);
			var creationArg = new DialogBoxCreationArgs(editor, title, null, DialogSizeHint.Large);

			var exitCode = ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, creationArg);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				DocumentManager.InvalidateFolder(typeof(UnverifiedFolder));
			}
		}
	}
}
