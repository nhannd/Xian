#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="StackTabComponentContainer"/>
    /// </summary>
    public partial class StackTabComponentContainerControl : CustomUserControl
    {
        private StackTabComponentContainer _component;

        public StackTabComponentContainerControl(StackTabComponentContainer component)
        {
            InitializeComponent();
            _component = component;

            CreateStackTabs();
        }

        private void CreateStackTabs()
        {
            _stackTabControl.RootDirection = Crownwood.DotNetMagic.Common.LayoutDirection.Vertical;

            foreach (TabPage page in _component.Pages)
            {
                StackTab stackTab = CreateStackTab(page.Name, _component.StackStyle);

                Crownwood.DotNetMagic.Controls.TabGroupLeaf tgl = _stackTabControl.RootSequence.AddNewLeaf() as Crownwood.DotNetMagic.Controls.TabGroupLeaf;
                tgl.MinimumSize = stackTab.MinimumRequestedSize;

                // Prevent user from resizing
                tgl.ResizeBarLock = _component.StackStyle == StackStyle.ShowMultiple ? false : true;

                Crownwood.DotNetMagic.Controls.TabPage tabPageUI = new Crownwood.DotNetMagic.Controls.TabPage(page.Name, stackTab);
                tabPageUI.Tag = page;
                tgl.TabPages.Add(tabPageUI);
            }

            // Set the sizing spaces between groups
            _stackTabControl.ResizeBarVector = _component.StackStyle == StackStyle.ShowMultiple ? 1 : 0;

            // The space of each tab group can only be set after each tab group is created
            // Open up only the first tab group and close all others
            for (int i = 0; i < _stackTabControl.RootSequence.Count; i++)
            {
                Crownwood.DotNetMagic.Controls.TabGroupLeaf tgl = _stackTabControl.RootSequence[i] as Crownwood.DotNetMagic.Controls.TabGroupLeaf;

                if (_component.StackStyle == StackStyle.ShowMultiple && _component.OpenAllTabsInitially)
                {
                    OpenTabGroup(tgl, (decimal)100 / _stackTabControl.RootSequence.Count);
                }
                else
                {
                    if (tgl == _stackTabControl.RootSequence[0])
                        OpenTabGroup(tgl, 100);
                    else
                        CloseTabGroup(tgl);
                }
            }

            // Reflect spacing changes immediately
            _stackTabControl.RootSequence.Reposition();
        }

        #region Event Handlers

        private void OnArrowClick(object sender, System.EventArgs e)
        {
            // Remember which title bar sent message
            Crownwood.DotNetMagic.Controls.TitleBar tbClick = sender as Crownwood.DotNetMagic.Controls.TitleBar;
            List<Crownwood.DotNetMagic.Controls.TabGroupLeaf> openedTabGroup = new List<Crownwood.DotNetMagic.Controls.TabGroupLeaf>();
            Crownwood.DotNetMagic.Controls.TabGroupLeaf selectedTabGroup = null;
            Crownwood.DotNetMagic.Controls.TabGroupLeaf tgl = _stackTabControl.FirstLeaf();
            while (tgl != null)
            {
                // Extract the StackTabTitleBar instance from page
                Crownwood.DotNetMagic.Controls.TitleBar tb = (tgl.TabPages[0].Control as StackTab).TitleBar;

                // Is the source of the click?
                if (tb == tbClick)
                {
                    selectedTabGroup = tgl;
                    tb.Active = true;

                    // Add to openedTabGroup because we want to open this
                    if (tgl.Space == 0)
                        openedTabGroup.Add(tgl);
                }
                else
                {
                    // Remember which TabGroup is opened
                    if (tgl.Space > 0)
                        openedTabGroup.Add(tgl);
                }

                // reset all TabGroups to close
                CloseTabGroup(tgl);

                tgl = _stackTabControl.NextLeaf(tgl);
            }

            // Open each TabGroup with evenly distributed space
            Decimal tabGroupSpace = openedTabGroup.Count == 0 ? 100 : (decimal)100 / openedTabGroup.Count;
            Decimal spaceRemained = 100;
            foreach (Crownwood.DotNetMagic.Controls.TabGroupLeaf leaf in openedTabGroup)
            {
                OpenTabGroup(leaf, tabGroupSpace);
                spaceRemained -= tabGroupSpace;
            }

            if (openedTabGroup.Count == 0)
            {
                // We need at least one TabGroup open, so keep the current TabGroup open if it was the only one opened
                OpenTabGroup(selectedTabGroup, spaceRemained);
            }
            else
            {
                openedTabGroup[0].Space += spaceRemained;
            }

            // Reflect changes immediately
            _stackTabControl.RootSequence.Reposition();
        }

        private void OnTitleClick(object sender, System.EventArgs e)
        {
            // Remember which title bar sent message
            Crownwood.DotNetMagic.Controls.TitleBar tbClick = sender as Crownwood.DotNetMagic.Controls.TitleBar;
            Crownwood.DotNetMagic.Controls.TabGroupLeaf tgl = _stackTabControl.FirstLeaf();
            while (tgl != null)
            {
                // Extract the StackTabTitleBar instance from page
                Crownwood.DotNetMagic.Controls.TitleBar tb = (tgl.TabPages[0].Control as StackTab).TitleBar;

                // Is the source of the click?
                if (tb == tbClick)
                {
                    // open the tab
                    tb.Active = true;
                    OpenTabGroup(tgl, 100);
                }
                else
                {
                    // close the tab
                    tb.Active = false;
                    CloseTabGroup(tgl);
                }

                // Move on to the next tab group
                tgl = _stackTabControl.NextLeaf(tgl);
            }

            // Reflect changes immediately
            _stackTabControl.RootSequence.Reposition();
        }

        #endregion

        #region Private Helpers

        private StackTab CreateStackTab(string name, StackStyle stackStyle)
        {
            StackTab stackTab = null;

            if (stackStyle == StackStyle.ShowMultiple)
            {
                stackTab = new StackTab(string.Empty, name, string.Empty,
                    Crownwood.DotNetMagic.Controls.ArrowButton.UpArrow, new EventHandler(OnArrowClick));
            }
            else
            {
                stackTab = new StackTab(string.Empty, name, string.Empty,
                    Crownwood.DotNetMagic.Controls.ArrowButton.None, new EventHandler(OnTitleClick));

                stackTab.TitleBar.ActAsButton = Crownwood.DotNetMagic.Controls.ActAsButton.WholeControl;

                // Customize titlebar colours
                //stackTab.TitleBar.BackColor = Color.SlateBlue;
                //stackTab.TitleBar.ForeColor = Color.SteelBlue;
                //stackTab.TitleBar.InactiveBackColor = ControlPaint.Light(Color.SlateBlue);
                //stackTab.TitleBar.InactiveForeColor = ControlPaint.LightLight(Color.SlateBlue);
                //stackTab.TitleBar.GradientActiveColor = Color.DarkSlateBlue;
                //stackTab.TitleBar.GradientInactiveColor = ControlPaint.Light(Color.DarkSlateBlue);
                //stackTab.TitleBar.MouseOverColor = Color.SkyBlue;
            }

            return stackTab;
        }

        private void OpenTabGroup(Crownwood.DotNetMagic.Controls.TabGroupLeaf tgl, decimal space)
        {
            Crownwood.DotNetMagic.Controls.TabPage tabPageUI = tgl.TabPages[0] as Crownwood.DotNetMagic.Controls.TabPage;
            TabPage page = tabPageUI.Tag as TabPage;
            StackTab stackTab = tabPageUI.Control as StackTab;

            if (page.Component.IsStarted == false)
                page.Component.Start();

            if (stackTab.ApplicationComponentControl == null)
            {
                stackTab.ApplicationComponentControl = (Control)_component.GetPageView(page).GuiElement;
                stackTab.ApplicationComponentControl.Dock = DockStyle.Fill;
            }

            if (_component.StackStyle == StackStyle.ShowMultiple)
                stackTab.TitleBar.ArrowButton = Crownwood.DotNetMagic.Controls.ArrowButton.UpArrow;

            _component.CurrentPage = page;
            tabPageUI.Select();
            tgl.Space = space;
        }

        private void CloseTabGroup(Crownwood.DotNetMagic.Controls.TabGroupLeaf tgl)
        {
            StackTab stackTab = tgl.TabPages[0].Control as StackTab;

            if (_component.StackStyle == StackStyle.ShowMultiple)
                stackTab.TitleBar.ArrowButton = Crownwood.DotNetMagic.Controls.ArrowButton.DownArrow;

            tgl.Space = 0;
        }

        #endregion
    }
}
