#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Configuration.ActionModel;

namespace ClearCanvas.Desktop.View.WinForms.Configuration
{
	public partial class ActionModelConfigurationComponentControl : UserControl
	{
		private IActionModelConfigurationComponentViewModel _component;

		public ActionModelConfigurationComponentControl(IActionModelConfigurationComponentViewModel component)
		{
			InitializeComponent();

			_component = component;
			_component.SelectedNodeChanged += OnComponentSelectedNodeChanged;

			_actionModelTree.ShowToolbar = true;
			_actionModelTree.Tree = component.ActionModelTreeRoot;
			_actionModelTree.ToolbarModel = component.ToolbarActionModel;
			_actionModelTree.MenuModel = component.ContextMenuActionModel;
			_actionModelTree.ShowLines = !component.EnforceFlatActionModel;
			_actionModelTree.ShowRootLines = !component.EnforceFlatActionModel;
			_actionModelTree.ShowPlusMinus = !component.EnforceFlatActionModel;

			this.OnComponentSelectedNodeChanged(null, null);
		}

		private void PerformDispose(bool disposing)
		{
			if (_component != null)
			{
				_component.SelectedNodeChanged -= OnComponentSelectedNodeChanged;
				_component = null;
			}
		}

		public bool ShowActionPropertiesPane
		{
			get { return !_pnlSplit.Panel2Collapsed; }
			set { _pnlSplit.Panel2Collapsed = !value; }
		}

		private void OnComponentSelectedNodeChanged(object sender, EventArgs e)
		{
			AbstractActionModelTreeNode selectedNode = _component.SelectedNode;
			_pnlNodeProperties.SuspendLayout();
			try
			{
				_pnlNodeProperties.Visible = selectedNode != null;
				if (selectedNode != null)
				{
					_lblLabel.Text = selectedNode.CanonicalLabel;

					string tooltip = selectedNode.Tooltip;
					if (String.IsNullOrEmpty(tooltip))
						tooltip = selectedNode.CanonicalLabel;

					_toolTip.SetToolTip(_lblLabel, tooltip);
					_toolTip.SetToolTip(_pnlIcon, tooltip);

					if (!string.IsNullOrEmpty(selectedNode.Description))
					{
						_lblDescription.Text = selectedNode.Description;
						_lblDescription.Height = _lblDescription.GetPreferredSize(new Size(_lblDescription.Width, 100)).Height;
					}
					else
					{
						_lblDescription.Text = string.Empty;
						_lblDescription.Height = 0;
					}

					// destroy old icon
					Image image = _pnlIcon.BackgroundImage;
					_pnlIcon.BackgroundImage = null;
					if (image != null)
					{
						image.Dispose();
						image = null;
					}

					// set new icon
					IconSet iconSet = selectedNode.IconSet;
					if (iconSet != null)
					{
						try
						{
							image = iconSet.CreateIcon(IconSize.Medium, selectedNode.ResourceResolver);
						}
						catch (Exception ex)
						{
							Platform.Log(LogLevel.Debug, ex, "Icon resolution failed.");
						}
						_pnlIcon.BackgroundImage = image;
					}
					_pnlIcon.Visible = _pnlIcon.BackgroundImage != null;

					// reload properties extensions
					_lyoNodePropertiesExtensions.SuspendLayout();
					try
					{
						ArrayList oldControls = new ArrayList(_lyoNodePropertiesExtensions.Controls);
						_lyoNodePropertiesExtensions.Controls.Clear();
						foreach (Control c in oldControls)
							c.Dispose();
						foreach (IApplicationComponentView componentView in _component.SelectedNodeProperties.ComponentViews)
						{
							try
							{
								_lyoNodePropertiesExtensions.Controls.Add((Control) componentView.GuiElement);
							}
							catch (Exception ex)
							{
								Platform.Log(LogLevel.Debug, ex, "Error encountered while loading a component extension");
							}
						}
					}
					finally
					{
						_lyoNodePropertiesExtensions.ResumeLayout();
					}
					this.OnLyoNodePropertiesExtensionsSizeChanged(null, null);
				}
			}
			finally
			{
				_pnlNodeProperties.ResumeLayout(true);
			}
		}

		private void OnActionModelTreeSelectionChanged(object sender, EventArgs e)
		{
			AbstractActionModelTreeNode selectedNode = null;
			if (_actionModelTree.Selection != null)
				selectedNode = (_actionModelTree.Selection.Item as AbstractActionModelTreeNode);
			_component.SelectedNode = selectedNode;
		}

		private void OnBindingTreeViewItemDrag(object sender, ItemDragEventArgs e)
		{
			BindingTreeView bindingTreeView = sender as BindingTreeView;
			if (bindingTreeView == null)
				return;

			ISelection selection = e.Item as ISelection;
			if (selection != null && selection.Item != null)
			{
				bindingTreeView.DoDragDrop(selection.Item, DragDropEffects.All);
			}
		}

		private void OnLyoNodePropertiesExtensionsSizeChanged(object sender, EventArgs e)
		{
			foreach (Control control in _lyoNodePropertiesExtensions.Controls)
			{
				// for some reason, the controls get cut off even using the client area width...
				control.Width = _lyoNodePropertiesExtensions.ClientSize.Width - 8;
			}
		}
	}
}