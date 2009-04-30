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
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop.Configuration.Tools
{
	/// <summary>
	/// A tool that, when executed, will show a configuration dialog.
	/// </summary>
	[MenuAction("show", "global-menus/MenuTools/MenuOptions", "Show", KeyStroke = XKeys.Control | XKeys.O)]
	[Tooltip("show", "MenuOptions")]
	[IconSet("show", IconScheme.Colour, "Icons.OptionsToolSmall.png", "Icons.OptionsToolMedium.png", "Icons.OptionsToolLarge.png")]
	[EnabledStateObserver("show", "Enabled", "EnabledChanged")]
	[GroupHint("show", "Application.Options")]

	[ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
	public class OptionsTool : Tool<ClearCanvas.Desktop.IDesktopToolContext>
	{
		private bool _enabled;
		private event EventHandler _enabledChanged;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public OptionsTool()
		{
			_enabled = true;
		}

		/// <summary>
		/// Called to determine whether this tool is enabled/disabled in the UI.
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
		/// Notifies that the Enabled state of this tool has changed.
		/// </summary>
		public event EventHandler EnabledChanged
		{
			add { _enabledChanged += value; }
			remove { _enabledChanged -= value; }
		}

		/// <summary>
		/// Called by the framework when the user clicks the "Options" menu item.
		/// </summary>
		public void Show()
		{
			try
			{
				ConfigurationDialog.Show(this.Context.DesktopWindow);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}
	}
}
