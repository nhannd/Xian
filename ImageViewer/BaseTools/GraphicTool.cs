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
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.BaseTools
{
	/// <summary>
	/// A base class for tools that operate on <see cref="IGraphic"/>
	/// objects.
	/// </summary>
	public abstract class GraphicTool : Tool<IGraphicToolContext>
	{
		private bool _enabled = true;
		private event EventHandler _enabledChanged;

		private bool _visible = true;
		private event EventHandler _visibleChanged;

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
		/// Gets or sets a value indicating whether the tool is visible.
		/// </summary>
		public bool Visible
		{
			get { return _visible; }
			protected set
			{
				if (_visible != value)
				{
					_visible = value;
					EventsHelper.Fire(_visibleChanged, this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Occurs when the <see cref="Visible"/> property has changed.
		/// </summary>
		public event EventHandler VisibleChanged
		{
			add { _visibleChanged += value; }
			remove { _visibleChanged -= value; }
		}
	}
}
