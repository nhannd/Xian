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

#if DEBUG
#pragma warning disable 1591

using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop
{
/*
    [MenuAction("shelf1", "global-menus/Test/Shelf1", "Shelf1")]
    [MenuAction("show1", "global-menus/Test/Show Shelf1", "ShowShelf1")]
    [MenuAction("hide1", "global-menus/Test/Hide Shelf1", "HideShelf1")]

	[MenuAction("shelf2", "global-menus/Test/Shelf2", "Shelf2")]
    [MenuAction("show2", "global-menus/Test/Show Shelf2", "ShowShelf2")]
    [MenuAction("hide2", "global-menus/Test/Hide Shelf2", "HideShelf2")]

	[MenuAction("ws", "global-menus/Test/New Workspace", "NewWorkspace")]

	[MenuAction("desktop1", "global-menus/Test/Desktop1", "Desktop1")]
	[MenuAction("desktop2", "global-menus/Test/Desktop2", "Desktop2")]
*/
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class TestTool : Tool<IDesktopToolContext>
    {
        private Shelf _shelf1;
        private Shelf _shelf2;
        private int _workspaceCount;

        void Shelf1()
        {
            if (_shelf1 == null)
            {
                TestComponent component = new TestComponent("Shelf1");
                _shelf1 = ApplicationComponent.LaunchAsShelf(
                    this.Context.DesktopWindow,
                    component,
                    "Shelf 1", 
					"Shelf1",
                    ShelfDisplayHint.DockFloat | ShelfDisplayHint.DockLeft | ShelfDisplayHint.HideOnWorkspaceOpen);

                _shelf1.Closed += delegate { _shelf1 = null; };
            }
            else
            {
                _shelf1.Activate();
            }
        }

        void ShowShelf1()
        {
            if (_shelf1 != null)
                _shelf1.Show();
        }

        void HideShelf1()
        {
            if (_shelf1 != null)
                _shelf1.Hide();
        }

        void Shelf2()
        {
            if (_shelf2 == null)
            {
                TestComponent component = new TestComponent("Shelf2");
                _shelf2 = ApplicationComponent.LaunchAsShelf(
                    this.Context.DesktopWindow,
                    component,
                    "Shelf 2",
					"Shelf2",
					ShelfDisplayHint.DockFloat);

                _shelf2.Closed += delegate { _shelf2 = null; };
            }
            else
            {
                _shelf2.Activate();
            }
        }

        void ShowShelf2()
        {
            if (_shelf2 != null)
                _shelf2.Show();
        }

        void HideShelf2()
        {
            if (_shelf2 != null)
                _shelf2.Hide();
        }

        void NewWorkspace()
        {
            string name = "Workspace " + (_workspaceCount++);
            ApplicationComponent.LaunchAsWorkspace(
                this.Context.DesktopWindow,
                new TestComponent(name),
                name,
                name);
        }

		void Desktop1()
		{
			DesktopWindowCreationArgs args = new DesktopWindowCreationArgs("Desktop1", "Desktop1");
			Application.DesktopWindows.AddNew(args);
		}
	
		void Desktop2()
		{
			DesktopWindowCreationArgs args = new DesktopWindowCreationArgs("Desktop2", "Desktop2");
			Application.DesktopWindows.AddNew(args);
		}
	}
}

#endif