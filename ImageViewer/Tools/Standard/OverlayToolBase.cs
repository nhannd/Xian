using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	public abstract class OverlayToolBase : Tool<IImageViewerToolContext>
	{
		private event EventHandler _checkedChanged;
		private bool _checked;

		protected OverlayToolBase()
		{
			_checked = true;
		}

		public override void Initialize()
		{
			base.Initialize();

			this.Context.Viewer.EventBroker.ImageDrawing += OnImageDrawing;
		}

		protected override void Dispose(bool disposing)
		{
			this.Context.Viewer.EventBroker.ImageDrawing -= OnImageDrawing;

			base.Dispose(disposing);
		}

		public bool Checked
		{
			get { return _checked; }
			set
			{
				if (_checked != value)
				{
					_checked = value;
					OnCheckedChanged();
				}
			}
		}

		protected virtual void OnCheckedChanged()
		{
			EventsHelper.Fire(_checkedChanged, this, new EventArgs());
		}

		public event EventHandler CheckedChanged
		{
			add { _checkedChanged += value; }
			remove { _checkedChanged -= value; }
		}

		public void ShowHide()
		{
			this.Checked = !this.Checked;
			this.Context.Viewer.PhysicalWorkspace.Draw();
		}

		protected abstract void UpdateVisibility(IPresentationImage image, bool visible);

		private void OnImageDrawing(object sender, ImageDrawingEventArgs e)
		{
			UpdateVisibility(e.PresentationImage, Checked);
		}
	}
}
