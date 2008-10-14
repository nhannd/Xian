using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Utilities.DicomEditor.Tools
{
	public abstract class DicomEditorTool : Tool<DicomEditorComponent.DicomEditorToolContext>
	{
		private readonly bool _isLocalOnly;
		private event EventHandler _enabledChanged;
		private bool _enabled;

		protected DicomEditorTool() : this(false) {}

		protected DicomEditorTool(bool isLocalOnly)
		{
			_isLocalOnly = isLocalOnly;
		}

		public bool IsLocalOnly
		{
			get { return _isLocalOnly; }
		}

		public virtual bool Enabled
		{
			get { return _enabled; }
			protected set
			{
				if (_isLocalOnly)
					value = value & this.Context.IsLocalFile;

				if (_enabled != value)
				{
					_enabled = value;
					EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Notifies that the Enabled state of this tool has changed.
		/// </summary>
		public event EventHandler EnabledChanged
		{
			add { _enabledChanged += value; }
			remove { _enabledChanged -= value; }
		}

		public override void Initialize()
		{
			base.Initialize();

			this.Enabled = true;
			this.Context.TagEdited += OnTagEdited;
			this.Context.DisplayedDumpChanged += OnDisplayedDumpChanged;
			this.Context.SelectedTagChanged += OnSelectedTagChanged;
			this.Context.IsLocalFileChanged += OnIsLocalFileChanged;
		}

		protected override void Dispose(bool disposing)
		{
			this.Context.TagEdited -= OnTagEdited;
			this.Context.DisplayedDumpChanged -= OnDisplayedDumpChanged;
			this.Context.SelectedTagChanged -= OnSelectedTagChanged;
			this.Context.IsLocalFileChanged -= OnIsLocalFileChanged;

			base.Dispose(disposing);
		}

		protected virtual void OnDisplayedDumpChanged(object sender, DisplayedDumpChangedEventArgs e) {}

		protected virtual void OnTagEdited(object sender, EventArgs e) {}

		protected virtual void OnIsLocalFileChanged(object sender, EventArgs e) {}

		protected virtual void OnSelectedTagChanged(object sender, EventArgs e) {}
	}
}