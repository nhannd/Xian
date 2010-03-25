using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

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
	[Tooltip("edit", "Verify External Practitioner Information")]
	[IconSet("apply", IconScheme.Colour, "Icons.CheckInToolSmall.png", "Icons.CheckInToolMedium.png", "Icons.CheckInToolLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.ExternalPractitioner)]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.ExternalPractitioner.Update)]
	[ExtensionOf(typeof(ExternalPractitionerItemToolExtensionPoint))]
	public class ExternalPractitionerVerifyTool : ExternalPractitionerTool
	{
		public void Apply()
		{
		}
	}

	[MenuAction("apply", "folderexplorer-items-contextmenu/Edit External Practitioner", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Edit External Practitioner", "Apply")]
	[Tooltip("edit", "Edit External Practitioner Information")]
	[IconSet("edit", IconScheme.Colour, "Icons.EditToolSmall.png", "Icons.EditToolMedium.png", "Icons.EditToolLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.ExternalPractitioner)]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.ExternalPractitioner.Update)]
	[ExtensionOf(typeof(ExternalPractitionerItemToolExtensionPoint))]
	public class ExternalPractitionerEditTool : ExternalPractitionerTool
	{
		public void Apply()
		{
			var item = CollectionUtils.FirstElement(this.Context.SelectedItems);

			var editor = new ExternalPractitionerEditorComponent(item.PractitionerRef);
			var exitCode = ApplicationComponent.LaunchAsDialog(
				this.Context.DesktopWindow, editor, SR.TitleUpdateExternalPractitioner + " - " + Formatting.PersonNameFormat.Format(item.Name));
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				DocumentManager.InvalidateFolder(typeof(VerifiedTodayFolder));
			}
		}
	}

	[MenuAction("apply", "folderexplorer-items-contextmenu/Merge External Practitioners", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Merge External Practitioners", "Apply")]
	[Tooltip("edit", "Merge Duplicate External Practitioners")]
	[IconSet("edit", IconScheme.Colour, "Icons.MergeToolSmall.png", "Icons.MergeToolMedium.png", "Icons.MergeToolLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.ExternalPractitioner)]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.ExternalPractitioner.Merge)]
	[ExtensionOf(typeof(ExternalPractitionerItemToolExtensionPoint))]
	public class ExternalPractitionerMergeTool : ExternalPractitionerTool
	{
		public void Apply()
		{
			var item = CollectionUtils.FirstElement(this.Context.SelectedItems);

			var editor = new ExternalPractitionerMergeNavigatorComponent(item.PractitionerRef);

			var title = SR.TitleMergePractitioner + " - " + Formatting.PersonNameFormat.Format(item.Name);
			var creationArg = new DialogBoxCreationArgs(editor, title, null, DialogSizeHint.Large);

			var exitCode = ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, creationArg);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				DocumentManager.InvalidateFolder(typeof(VerifiedTodayFolder));
			}
		}
	}
}
