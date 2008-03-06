using System;
using System.ComponentModel;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.ImageViewer.Clipboard
{
	public abstract class ClipboardTool : Tool<IClipboardToolContext>
	{
		private bool _enabled = false;
		private event EventHandler _enabledChanged;
		private bool _applyOnlyToSelected = true;

		public override void Initialize()
		{
			this.Context.ClipboardItems.ListChanged += OnClipboardItemsChanged;
			this.Context.SelectedClipboardItemsChanged += OnSelectionChanged;
			base.Initialize();
		}

		public bool ApplyOnlyToSelected
		{
			get { return _applyOnlyToSelected; }
			set { _applyOnlyToSelected = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the tool is enabled.
		/// </summary>
		public bool Enabled
		{
			get { return _enabled; }
			protected set
			{
				if (_enabled != value)
				{
					_enabled = value;
					EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Occurs when the <see cref="Enabled"/> property has changed.
		/// </summary>
		public event EventHandler EnabledChanged
		{
			add { _enabledChanged += value; }
			remove { _enabledChanged -= value; }
		}

		private void OnClipboardItemsChanged(object sender, ListChangedEventArgs e)
		{
			if (!_applyOnlyToSelected)
			{
				if (this.Context.ClipboardItems.Count != 0)
					this.Enabled = true;
				else
					this.Enabled = false;
			}
		}

		private void OnSelectionChanged(object sender, EventArgs e)
		{
			if (_applyOnlyToSelected)
			{
				if (this.Context.SelectedClipboardItems.Count != 0)
					this.Enabled = true;
				else
					this.Enabled = false;
			}
		}
	}
}
