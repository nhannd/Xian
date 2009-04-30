#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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