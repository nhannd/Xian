using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Utilities.DicomEditor;

namespace ClearCanvas.Utilities.DicomEditor.Tools
{
	/// <summary>
	/// Base class for <see cref="DicomEditorComponent"/> tools.
	/// </summary>
	public abstract class DicomEditorTool : Tool<DicomEditorComponent.DicomEditorToolContext>
	{
		private readonly bool _isLocalOnly;
		private event EventHandler _enabledChanged;
		private bool _enabled;

		/// <summary>
		/// Constructs a new <see cref="DicomEditorTool"/> for all types of DICOM sources.
		/// </summary>
		protected DicomEditorTool() : this(false) {}

		/// <summary>
		/// Constructs a new <see cref="DicomEditorTool"/> for the specified types of DICOM sources.
		/// </summary>
		/// <param name="isLocalOnly">Specifies that the tool should only be enabled for locally-available DICOM files (i.e. not streamed).</param>
		protected DicomEditorTool(bool isLocalOnly)
		{
			_isLocalOnly = isLocalOnly;
		}

		/// <summary>
		/// Gets if the tool is only enabled for locally-available DICOM files (i.e. not streamed).
		/// </summary>
		public bool IsLocalOnly
		{
			get { return _isLocalOnly; }
		}

		/// <summary>
		/// Gets or sets if the tool is currently enabled.
		/// </summary>
		/// <remarks>
		/// Note that the tool cannot be enabled if it was constructed as a local-file only tool and the currently loaded file is not local.
		/// </remarks>
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
		/// Notifies that the <see cref="Enabled"/> state of this tool has changed.
		/// </summary>
		public event EventHandler EnabledChanged
		{
			add { _enabledChanged += value; }
			remove { _enabledChanged -= value; }
		}

		/// <summary>
		/// Called by the framework to allow the tool to initialize itself.
		/// </summary>
		/// <remarks>
		public override void Initialize()
		{
			base.Initialize();

			this.Enabled = true;
			this.Context.TagEdited += OnTagEdited;
			this.Context.DisplayedDumpChanged += OnDisplayedDumpChanged;
			this.Context.SelectedTagChanged += OnSelectedTagChanged;
			this.Context.IsLocalFileChanged += OnIsLocalFileChanged;
		}

		/// <summary>
		/// Disposes of this object; override this method to do any necessary cleanup.
		/// </summary>
		/// <param name="disposing">True if this object is being disposed, false if it is being finalized.</param>
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