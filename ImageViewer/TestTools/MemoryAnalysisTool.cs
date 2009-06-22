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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.TestTools
{
	[MenuAction("show", "global-menus/MenuTools/MenuUtilities/Memory Analysis", "Show", KeyStroke = XKeys.Control | XKeys.M)]
	[ButtonAction("show", "global-toolbars/ToolbarUtilities/Memory Analysis", "Show")]
	[IconSet("show", IconScheme.Colour, "Icons.MemoryAnalysisToolSmall.png", "Icons.MemoryAnalysisToolMedium.png", "")]

	[ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
	public class MemoryAnalysisTool : Tool<ClearCanvas.Desktop.IDesktopToolContext>
	{
		private static IShelf _shelf;

		public MemoryAnalysisTool()
		{
		}

		public void Show()
		{
			if (_shelf != null)
			{
				_shelf.Activate();
			}
			else
			{
				MemoryAnalysisComponent component = new MemoryAnalysisComponent(this.Context.DesktopWindow);
				_shelf = ApplicationComponent.LaunchAsShelf(this.Context.DesktopWindow, component, "Memory Analysis",
					                                   ShelfDisplayHint.DockFloat);
				_shelf.Closed += delegate { _shelf = null; };
			}
		}
	}
}
