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
