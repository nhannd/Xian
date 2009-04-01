using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters
{
	[ExtensionPoint]
	public sealed class StudyFilterToolExtensionPoint : ExtensionPoint<ITool> {}

	public interface IStudyFilterToolContext : IToolContext
	{
		StudyFilterComponent Component { get; }
		IDesktopWindow DesktopWindow { get; }
	}

	public abstract class StudyFilterTool : Tool<IStudyFilterToolContext>
	{
		public const string DefaultToolbarActionSite = "studyfilter-toolbar";
		public const string DefaultContextMenuActionSite = "studyfilter-context";

		protected StudyFilterComponent Component
		{
			get { return base.Context.Component; }
		}

		protected StudyFilterColumnCollection Columns
		{
			get { return base.Context.Component.Columns; }
		}

		protected StudyItemSelection Selection
		{
			get { return base.Context.Component.Selection; }
		}

		protected IDesktopWindow DesktopWindow
		{
			get { return base.Context.DesktopWindow; }
		}

		public override void Initialize()
		{
			base.Initialize();
			this.Selection.SelectionChanged += SelectionChangedEventHandler;
		}

		protected override void Dispose(bool disposing)
		{
			this.Selection.SelectionChanged -= SelectionChangedEventHandler;
			base.Dispose(disposing);
		}

		private void SelectionChangedEventHandler(object sender, EventArgs e)
		{
			this.OnSelectionChanged();
		}

		protected virtual void OnSelectionChanged() {}
	}
}