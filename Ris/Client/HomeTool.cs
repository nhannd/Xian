using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// A tool for launching a home page.  A home page consists of a set of folder systems and a preview component.
	/// </summary>
	/// <remarks>
	/// Subclasses of this class should specify a <see cref="MenuActionAttribute"/> attribute with the Launch method as the clickHandler
	/// </remarks>
	public abstract class HomeTool : Tool<IDesktopToolContext> 
	{
		private IWorkspace _workspace;

		/// <summary>
		/// Title displayed when the home page is active
		/// </summary>
		public abstract string Title { get; }

		/// <summary>
		/// Creates the preview component.
		/// </summary>
		/// <returns></returns>
		protected abstract IApplicationComponent CreateComponent();

		/// <summary>
		/// Default clickHandler implementation for <see cref="MenuAction"/> and/or <see cref="ButtonAction"/> attributes.
		/// These attributes must be specified on subclasses.
		/// </summary>
		public void Launch()
		{
			if (_workspace == null)
			{
				try
				{
					_workspace = ApplicationComponent.LaunchAsWorkspace(
						this.Context.DesktopWindow,
						CreateComponent(),
						this.Title);
					_workspace.Closed += delegate { _workspace = null; };
				}
				catch (Exception e)
				{
					// could not launch component
					ExceptionHandler.Report(e, this.Context.DesktopWindow);
				}
			}
			else
			{
				_workspace.Activate();
			}
		}
	}

	/// <summary>
	/// A tool for launching a home page with a <see cref="WorklistItemPreviewComponent"/> as the preview component
	/// </summary>
	/// <typeparam name="TFolderSystemToolExtensionPoint">Specifies the extension point used to create the set of folder systems</typeparam>
	public abstract class WorklistPreviewHomeTool<TFolderSystemToolExtensionPoint> : HomeTool
		where TFolderSystemToolExtensionPoint : ExtensionPoint<IFolderSystem>, new()
	{
		protected override IApplicationComponent  CreateComponent()
		{
			WorklistItemPreviewComponent previewComponent = new WorklistItemPreviewComponent();
			return new HomePageContainer(new TFolderSystemToolExtensionPoint(), previewComponent);
		}
	}
}
