using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.ImageViewer.Clipboard
{
	/// <summary>
	/// Base class for clipboard tools.
	/// </summary>
	/// <remarks>
	/// Though not required, it is recommended that clipboard tools 
	/// derive from this class.
	/// </remarks>
	public abstract class ClipboardTool : Tool<IClipboardToolContext>
	{
		private bool _enabled = false;
		private event EventHandler _enabledChanged;

    	/// <summary>
    	/// Called by the framework to allow the tool to initialize itself.
    	/// </summary>
    	/// <remarks>
    	/// This method will be called after <see cref="ITool.SetContext"/> has been called, 
    	/// which guarantees that the tool will have access to its context when this method is called.
    	/// </remarks>
		public override void Initialize()
		{
			base.Initialize();

			UpdateEnabled();
			
			this.Context.ClipboardItemsChanged += OnClipboardItemsChanged;
			this.Context.SelectedClipboardItemsChanged += OnSelectionChanged;
		}

		/// <summary>
		/// Disposes of this object; override this method to do any necessary cleanup.
		/// </summary>
		/// <param name="disposing">True if this object is being disposed, false if it is being finalized.</param>
		protected override void Dispose(bool disposing)
		{
			this.Context.ClipboardItemsChanged -= OnClipboardItemsChanged;
			this.Context.SelectedClipboardItemsChanged -= OnSelectionChanged;

			base.Dispose(disposing);
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

		/// <summary>
		/// Called when the selection has changed.
		/// </summary>
		/// <remarks>
		/// By default, this method sets the <see cref="Enabled"/> property based
		/// on whether or not anything is currently selected.  If you want to change
		/// this behaviour you should override this method (and not call the base method).
		/// </remarks>
		protected virtual void OnSelectionChanged()
		{
			UpdateEnabled();
		}

		/// <summary>
		/// Called when the items on the clipboard have changed.
		/// </summary>
		protected virtual void OnClipboardItemsChanged()
		{
		}

		private void OnClipboardItemsChanged(object sender, EventArgs e)
		{
			OnClipboardItemsChanged();
		}

		private void OnSelectionChanged(object sender, EventArgs e)
		{
			OnSelectionChanged();
		}

		private void UpdateEnabled()
		{
			if (this.Context.SelectedClipboardItems.Count != 0)
				this.Enabled = true;
			else
				this.Enabled = false;
		}
	}
}
