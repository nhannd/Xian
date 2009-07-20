using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.Utilities;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters
{
	[ExtensionPoint]
	public sealed class StudyFilterColumnToolExtensionPoint : ExtensionPoint<ITool> {}

	public interface IStudyFilterColumnToolContext : IToolContext
	{
		StudyFilterColumn Column { get; }
	}

	public abstract class StudyFilterColumnTool : Tool<IStudyFilterColumnToolContext>
	{
		public event EventHandler VisibleChanged;

		private bool _visible = true;

		public bool Visible
		{
			get { return _visible; }
			protected set
			{
				if (_visible != value)
				{
					_visible = value;
					EventsHelper.Fire(this.VisibleChanged, this, EventArgs.Empty);
				}
			}
		}

		public StudyFilterColumn Column
		{
			get { return base.Context.Column; }
		}

		public CompositeFilterPredicate ColumnFilterRoot
		{
			get { return base.Context.Column.ColumnFilterRoot; }
		}

		public IStudyFilter StudyFilter
		{
			get { return base.Context.Column.Owner; }
		}
		
		protected virtual bool IsColumnSupported()
		{
			return true;
		}

		public override void Initialize()
		{
			base.Initialize();
			this.Visible = this.IsColumnSupported();
		}
	}
}