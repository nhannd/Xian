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

namespace ClearCanvas.Samples.Google.Calendar
{
    [MenuAction("apply", "global-menus/MenuTools/MenuToolsMyTools/SchedulingTool", "Apply")]
    [ButtonAction("apply", "global-toolbars/ToolbarMyTools/SchedulingTool", "Apply")]
    [Tooltip("apply", "SchedulingToolTooltip")]
    [IconSet("apply", IconScheme.Colour, "Icons.SchedulingToolSmall.png", "Icons.SchedulingToolMedium.png", "Icons.SchedulingToolLarge.png")]

    [ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
    public class SchedulingTool : Tool<ClearCanvas.Desktop.IDesktopToolContext>
    {
        private Shelf _shelf;

        /// <summary>
        /// Default constructor.  A no-args constructor is required by the
        /// framework.  Do not remove.
        /// </summary>
        public SchedulingTool()
        {
        }

        /// <summary>
        /// Called by the framework when the user clicks the "apply" menu item or toolbar button.
        /// </summary>
        public void Apply()
        {
            // check if the shelf already exists
            if (_shelf == null)
            {
                // create a new shelf that hosts the SchedulingComponent
                _shelf = ApplicationComponent.LaunchAsShelf(
                    this.Context.DesktopWindow,
                    new SchedulingComponent(),
                    SR.SchedulingTool,
                    ShelfDisplayHint.DockRight|ShelfDisplayHint.DockAutoHide,
                    delegate(IApplicationComponent c)
                    {
                        _shelf = null;  // destroy the shelf when the user closes it
                    });
            }
            else
            {
                // activate existing shelf
                _shelf.Activate();
            }
        }
    }
}
