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
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.View.WinForms
{
	public partial class TabComponentContainerControl : UserControl
	{
		TabComponentContainer _component;

		public TabComponentContainerControl(TabComponentContainer component)
		{
			Platform.CheckForNullReference(component, "component");
			
			InitializeComponent();
			_component = component;

			_tabControl.ControlLeftOffset = 3;
			_tabControl.ControlTopOffset = 3;
			_tabControl.ControlRightOffset = 3;
			_tabControl.ControlBottomOffset = 3;

			foreach (TabPage page in _component.Pages)
			{
				Crownwood.DotNetMagic.Controls.TabPage tabPageUI = new Crownwood.DotNetMagic.Controls.TabPage(page.Name);
				tabPageUI.Tag = page;

				_tabControl.TabPages.Add(tabPageUI);
			}

			_tabControl.SelectionChanged += new Crownwood.DotNetMagic.Controls.SelectTabHandler(OnControlSelectionChanged);
			_component.CurrentPageChanged += new EventHandler(OnComponentCurrentPageChanged);

            ShowPage(_component.CurrentPage);
		}

		private void OnComponentCurrentPageChanged(object sender, EventArgs e)
		{
            ShowPage(_component.CurrentPage);
		}

        private void OnControlSelectionChanged(Crownwood.DotNetMagic.Controls.TabControl sender, Crownwood.DotNetMagic.Controls.TabPage oldPage, Crownwood.DotNetMagic.Controls.TabPage newPage)
		{
			TabPage tabPage = newPage.Tag as TabPage;
			_component.CurrentPage = tabPage;
		}

        private void ShowPage(TabPage page)
        {
            // find the tab corresponding to the current page
            Crownwood.DotNetMagic.Controls.TabPage tab = CollectionUtils.SelectFirst<Crownwood.DotNetMagic.Controls.TabPage>(_tabControl.TabPages,
                delegate(Crownwood.DotNetMagic.Controls.TabPage tp) { return tp.Tag == page; });

            // if the tab's control was not yet created, create it now
            if (tab.Control == null)
            {
                tab.Control = (Control)_component.GetPageView(page).GuiElement;
            }

            // ensure the correct tab is selected (in case the current page was changed programatically)
            tab.Selected = true;
        }

    }
}
