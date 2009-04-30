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

using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
    public partial class TabGroupComponentContainerControl : UserControl
    {
        TabGroupComponentContainer _component;

        public TabGroupComponentContainerControl(TabGroupComponentContainer component)
        {
            InitializeComponent();
            _component = component;

            CreateTabGroups();
        }

        private void CreateTabGroups()
        {
            _tabbedGroupsControl.PageChanged += new Crownwood.DotNetMagic.Controls.TabbedGroups.PageChangeHandler(OnControlPageChanged);

            _tabbedGroupsControl.RootDirection = _component.LayoutDirection == LayoutDirection.Vertical ?
                Crownwood.DotNetMagic.Common.LayoutDirection.Vertical :
                Crownwood.DotNetMagic.Common.LayoutDirection.Horizontal;

            foreach (TabGroup tabGroup in _component.TabGroups)
            {
                Crownwood.DotNetMagic.Controls.TabGroupLeaf tgl = _tabbedGroupsControl.RootSequence.AddNewLeaf() as Crownwood.DotNetMagic.Controls.TabGroupLeaf;

                foreach (TabPage page in tabGroup.Component.Pages)
                {
                    Crownwood.DotNetMagic.Controls.TabPage tabPageUI = new Crownwood.DotNetMagic.Controls.TabPage(page.Name);
                    tabPageUI.Tag = page;
                    tgl.TabPages.Add(tabPageUI);
                }
            }

            // The weight can only be set after each leaf is created
            // Ask control to reposition children according to new spacing
            for (int i = 0; i < _component.TabGroups.Count; i++)
            {
                Crownwood.DotNetMagic.Controls.TabGroupLeaf tgl = _tabbedGroupsControl.RootSequence[i] as Crownwood.DotNetMagic.Controls.TabGroupLeaf;
                tgl.Space = (decimal)(_component.TabGroups[i].Weight * 100);
            }

            _tabbedGroupsControl.RootSequence.Reposition();
        }

        private void OnControlPageChanged(Crownwood.DotNetMagic.Controls.TabbedGroups tg, Crownwood.DotNetMagic.Controls.TabPage selectedPage)
        {
            if (selectedPage != null)
            {
                TabPage tabPage = selectedPage.Tag as TabPage;

                if (tabPage.Component.IsStarted == false)
                    tabPage.Component.Start();

                if (selectedPage.Control == null)
                {
                    TabGroup tabGroup = _component.GetTabGroup(tabPage);
                    selectedPage.Control = (Control)tabGroup.Component.GetPageView(tabPage).GuiElement;
                }
            }
        }

        private void OnTabControlCreated(Crownwood.DotNetMagic.Controls.TabbedGroups tg, Crownwood.DotNetMagic.Controls.TabControl tc)
        {
            // Place a thin border between edge of the tab control and inside contents
            tc.ControlTopOffset = 3;
            tc.ControlBottomOffset = 3;
            tc.ControlLeftOffset = 3;
            tc.ControlRightOffset = 3;
            tc.Style = Crownwood.DotNetMagic.Common.VisualStyle.IDE2005;
        }
    }
}
