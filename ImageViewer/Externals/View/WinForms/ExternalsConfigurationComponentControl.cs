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
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Externals.Config;
using ClearCanvas.ImageViewer.Externals.View.WinForms.Properties;

namespace ClearCanvas.ImageViewer.Externals.View.WinForms
{
	public partial class ExternalsConfigurationComponentControl : UserControl
	{
		private ExternalsConfigurationComponent _component;
		private IList<ExternalType> _externalTypes;

		public ExternalsConfigurationComponentControl(ExternalsConfigurationComponent component)
		{
			InitializeComponent();

			_component = component;

			SortedList<string, ExternalType> externalTypes = new SortedList<string, ExternalType>();
			ExternalFactoryExtensionPoint extensionPoint = new ExternalFactoryExtensionPoint();
			foreach (IExternalFactory externalFactory in extensionPoint.CreateExtensions())
			{
				ExternalType externalType = new ExternalType(externalFactory);
				externalTypes.Add(externalType.ToString(), externalType);
			}
			_externalTypes = externalTypes.Values;

			foreach (ExternalType externalType in _externalTypes)
			{
				ToolStripMenuItem item = new ToolStripMenuItem(externalType.ToString());
				item.Tag = externalType;
				item.Click += _mnuAdd_Click;
				_mnuExternalTypes.Items.Add(item);
			}

			ResetExternalList();
		}

		private void ResetExternalList()
		{
			_listExternals.Items.Clear();
			foreach (IExternal external in _component.Externals)
			{
				ListViewItem lvi = CreateItem(external);
				_listExternals.Items.Add(lvi);
				lvi.Selected = true;
			}
		}

		private ListViewItem SelectedItem
		{
			get
			{
				if (_listExternals.SelectedItems != null && _listExternals.SelectedItems.Count > 0)
				{
					return _listExternals.SelectedItems[0];
				}
				return null;
			}
		}

		private ListViewItem CreateItem(IExternal external)
		{
			ListViewItem item = new ListViewItem();
			item.Text = external.Label;
			item.SubItems.Add(external.ToString());
			item.Tag = external;
			return item;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			colLabel.Width = _listExternals.Width/2;
			colDescription.Width = _listExternals.Width/2;
		}

		private void _listExternals_SelectedIndexChanged(object sender, EventArgs e)
		{
			_btnEdit.Enabled = _btnRemove.Enabled = this.SelectedItem != null;
		}

		private void _listExternals_AfterLabelEdit(object sender, LabelEditEventArgs e)
		{
			if (e.Label == null)
				return;

			ListViewItem item = _listExternals.Items[e.Item];
			if (item != null)
			{
				IExternal launcher = item.Tag as IExternal;
				if (launcher != null)
				{
					launcher.Label = e.Label;
					_component.FlagModified();
				}
			}
		}

		private void _btnRemove_Click(object sender, EventArgs e)
		{
			ListViewItem _selectedItem = this.SelectedItem;
			if (_selectedItem != null)
			{
				_listExternals.Items.Remove(_selectedItem);
				_component.Externals.Remove(_selectedItem.Tag as IExternal);
			}
			_component.FlagModified();
		}

		private void _mnuAdd_Click(object sender, EventArgs e)
		{
			try
			{
				ExternalType externalType = (ExternalType) ((ToolStripMenuItem) sender).Tag;
				IExternal external = externalType.CreateExternal();

				IExternalPropertiesComponent component = externalType.CreateExternalPropertiesComponent();
				component.Load(external);

				SimpleComponentContainer container = new SimpleComponentContainer(component);
				if (_component.DesktopWindow.ShowDialogBox(container, Resources.TitleNewExternal) == DialogBoxAction.Ok)
				{
					component.Update(external);
					_component.Externals.Add(external);
					ListViewItem lvi = CreateItem(external);
					_listExternals.Items.Add(lvi);
					lvi.Selected = true;
					_component.FlagModified();
				}
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Error, ex, "An error occured while adding an external application definition.");
				MessageBox.Show(this, Resources.MessageErrorAddingExternal);
			}
		}

		private void _btnAdd_Click(object sender, EventArgs e)
		{
			_mnuExternalTypes.Show(_btnAdd, new Point(0, _btnAdd.Height));
		}

		private void _btnEdit_Click(object sender, EventArgs e)
		{
			if (this.SelectedItem != null)
			{
				try
				{
					IExternal external = this.SelectedItem.Tag as IExternal;
					if (external != null)
					{
						foreach (ExternalType externalType in _externalTypes)
						{
							if (externalType.SupportsExternal(external))
							{
								IExternalPropertiesComponent component = externalType.CreateExternalPropertiesComponent();
								component.Load(external);

								SimpleComponentContainer container = new SimpleComponentContainer(component);
								if (_component.DesktopWindow.ShowDialogBox(container, string.Format(Resources.TitleEditProperties, external.Label)) == DialogBoxAction.Ok)
								{
									component.Update(external);
									ResetExternalList();
									foreach (ListViewItem item in _listExternals.Items)
									{
										if (item.Tag == external)
										{
											item.Selected = true;
											break;
										}
									}
									_component.FlagModified();
								}
								break;
							}
						}
					}
				}
				catch (Exception ex)
				{
					Platform.Log(LogLevel.Error, ex, "An error occured while editing an external application definition.");
					MessageBox.Show(this, Resources.MessageErrorEditingExternal);
				}
			}
		}

		private void _listExternals_DoubleClick(object sender, EventArgs e)
		{
			_btnEdit.PerformClick();
		}

		private class ExternalType
		{
			private readonly IExternalFactory _externalFactory;

			internal ExternalType(IExternalFactory externalFactory)
			{
				_externalFactory = externalFactory;
			}

			public bool SupportsExternal(IExternal external)
			{
				if (external == null)
					return false;
				return _externalFactory.ExternalType.IsAssignableFrom(external.GetType());
			}

			public IExternal CreateExternal()
			{
				return _externalFactory.CreateNew();
			}

			public IExternalPropertiesComponent CreateExternalPropertiesComponent()
			{
				return _externalFactory.CreatePropertiesComponent();
			}

			public override string ToString()
			{
				return _externalFactory.Description;
			}
		}
	}
}