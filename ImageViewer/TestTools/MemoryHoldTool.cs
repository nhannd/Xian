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
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.TestTools
{
	[MenuAction("apply", "global-menus/MenuTools/MenuToolsMyTools/Consume Memory", "Apply", KeyStroke = XKeys.Control | XKeys.M)]
	[ButtonAction("apply", "global-toolbars/ToolbarMyTools/Consume Memory", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.MemoryHoldToolSmall.png", "Icons.MemoryHoldToolMedium.png", "")]
	[TooltipValueObserver("apply", "Tooltip", "TooltipChanged")]

	[ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
	public class MemoryHoldTool : Tool<ClearCanvas.Desktop.IDesktopToolContext>
	{
		private string _tooltip;
		private event EventHandler _tooltipChanged;
		private uint _usage;
		private static uint _increment;
		private List<object> _memoryHold;

		public MemoryHoldTool()
		{
			_memoryHold = new List<object>();
			_tooltip = "Memory Usage: 0";
		}

		public event EventHandler TooltipChanged
		{
			add { _tooltipChanged += value; }
			remove { _tooltipChanged -= value; }
		}

		/// <summary>
		/// Called by the framework to initialize this tool.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();

			_increment = 1000000;
			this.Usage = 0;
		}

		public string Tooltip
		{
			get { return _tooltip; }
		}

		private uint Usage
		{
			get { return _usage; }
			set 
			{
				if (_usage == value)
					return;

				_usage = value;
				_tooltip = String.Format("Memory Usage: {0}", _usage);
				EventsHelper.Fire(_tooltipChanged, this, EventArgs.Empty);
			}
		}

		public void Apply()
		{
			try
			{
				byte[] array = new byte[_increment];
				_memoryHold.Add(array);
				Usage += _increment;
			}
			catch (Exception e)
			{
			}
		}
	}
}
