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
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	public abstract class OverlayToolBase : Tool<IImageViewerToolContext>
	{
		private static readonly IList<OverlayToolBase> _toolRegistry = new List<OverlayToolBase>();
		private event EventHandler _checkedChanged;
		private bool _checked;

		protected OverlayToolBase()
			: this(true)
		{
		}

		protected OverlayToolBase(bool @checked)
		{
			_checked = @checked;
		}

		public override void Initialize()
		{
			base.Initialize();

			_toolRegistry.Add(this);

			this.Context.Viewer.EventBroker.DisplaySetChanged += OnDisplaySetChanged;
		}

		protected override void Dispose(bool disposing)
		{
			this.Context.Viewer.EventBroker.DisplaySetChanged -= OnDisplaySetChanged;

			_toolRegistry.Remove(this);

			base.Dispose(disposing);
		}

		//NOTE: checked is changed internally, or by the parent overlay tool.
		//So, when it is changed externally, we don't draw because presumably the parent tool will do that.
		public bool Checked
		{
			get { return _checked; }
			set
			{
				if (_checked == value)
					return;
				
				_checked = value;
				OnCheckedChanged();
			}
		}

		public event EventHandler CheckedChanged
		{
			add { _checkedChanged += value; }
			remove { _checkedChanged -= value; }
		}

		protected virtual void OnCheckedChanged()
		{
			UpdateVisibility();
			EventsHelper.Fire(_checkedChanged, this, new EventArgs());
		}

		public void ShowHide()
		{
			//NOTE: When this method is called directly, the tool was invoked directly, so we draw after.
			Checked = !Checked; //changing this updates visibility.
			Context.Viewer.PhysicalWorkspace.Draw();
		}

		private void OnDisplaySetChanged(object sender, DisplaySetChangedEventArgs e)
		{
			if (e.NewDisplaySet == null)
				return;

			CodeClock clock = new CodeClock();
			clock.Start();

			UpdateVisibility(e.NewDisplaySet, Checked);
			
			clock.Stop();
			Platform.Log(LogLevel.Debug, "{0} - UpdateVisibility took {1}", GetType().FullName, clock.Seconds);

			//The display set will be drawn externally because it just changed.
		}

		private void UpdateVisibility()
		{
			if (Context == null)
				return;

			foreach (var imageBox in Context.Viewer.PhysicalWorkspace.ImageBoxes)
			{
				if (imageBox.DisplaySet != null)
					UpdateVisibility(imageBox.DisplaySet, Checked);
			}
		}

		protected virtual void UpdateVisibility(IDisplaySet displaySet, bool visible)
		{
			foreach (var image in displaySet.PresentationImages)
				UpdateVisibility(image, visible);
		}

		protected abstract void UpdateVisibility(IPresentationImage image, bool visible);

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
