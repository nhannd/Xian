#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	public abstract class OverlayToolBase : Tool<IImageViewerToolContext>
	{
		private static readonly IList<OverlayToolBase> _toolRegistry = new List<OverlayToolBase>();
		private event EventHandler _checkedChanged;
		private bool _checked;

		protected OverlayToolBase()
		{
			_checked = true;
		}

		public override void Initialize()
		{
			base.Initialize();

			_toolRegistry.Add(this);

			this.Context.Viewer.EventBroker.ImageDrawing += OnImageDrawing;
		}

		protected override void Dispose(bool disposing)
		{
			this.Context.Viewer.EventBroker.ImageDrawing -= OnImageDrawing;

			_toolRegistry.Remove(this);

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

		public static IEnumerable<OverlayToolBase> EnumerateTools(IImageViewer imageViewer)
		{
			foreach (OverlayToolBase overlayTool in _toolRegistry)
			{
				if (overlayTool.Context != null && overlayTool.Context.Viewer == imageViewer)
					yield return overlayTool;
			}
		}
	}
}
