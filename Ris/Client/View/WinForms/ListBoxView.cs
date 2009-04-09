using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// ListBox-view that binds to an instance of an <see cref="IBindingList"/>, which acts as data-source.
	/// Also has built-in drag & drop support, delegating drop decisions to the underlying <see cref="ListBoxWithDragSupport"/>.
	/// </summary>
	public partial class ListBoxView : UserControl
	{
		private ActionModelNode _toolbarModel;
		private ActionModelNode _menuModel;

		private bool _isLoaded = false;

		public ListBoxView()
		{
			InitializeComponent();
		}

		#region Public Members

		/// <summary>
		/// Gets or sets the data source for the underlying <see cref="ListBoxWithDragSupport"/>.
		/// </summary>
		public IBindingList DataSource
		{
			get { return (IBindingList) _listBox.DataSource; }
			set { _listBox.DataSource = value; }
		}

		/// <summary>
		/// Gets or sets the display member of the ListBox item.
		/// </summary>
		public string DisplayMember
		{
			get { return _listBox.DisplayMember; }
			set { _listBox.DisplayMember = value; }
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ActionModelNode ToolbarModel
		{
			get { return _toolbarModel; }
			set
			{
				_toolbarModel = value;
				if (_isLoaded) 
					InitializeToolStrip();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ActionModelNode MenuModel
		{
			get { return _menuModel; }
			set
			{
				_menuModel = value;
				if (_isLoaded) 
					InitializeMenu();
			}
		}

		/// <summary>
		/// Gets or sets the current selected index.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int SelectedIndex
		{
			get { return _listBox.SelectedIndex; }
			set { _listBox.SelectedIndex = value; }
		}

		/// <summary>
		/// Notifies that the selection has changed
		/// </summary>
		public event EventHandler SelectedIndexChanged
		{
			add { _listBox.SelectedIndexChanged += value; }
			remove { _listBox.SelectedIndexChanged -= value; }
		}

		/// <summary>
		/// Notifies that an item is dragged and dropped on another item.
		/// </summary>
		public event EventHandler<ListBoxItemDroppedEventArgs> ItemDropped
		{
			add { _listBox.ItemDropped += value; }
			remove { _listBox.ItemDropped -= value; }
		}

		#endregion

		#region Design Time Properties and Events

		[DefaultValue(true)]
		public bool ShowToolbar
		{
			get { return _toolStrip.Visible; }
			set { _toolStrip.Visible = value; }
		}

		#endregion

		private void ListBoxView_Load(object sender, EventArgs e)
		{
			InitializeMenu();
			InitializeToolStrip();
			_isLoaded = true;
		}

		private void _contextMenu_Opening(object sender, CancelEventArgs e)
		{
			Point pt = _listBox.PointToClient(ListBox.MousePosition);
			int itemIndex = _listBox.IndexFromPoint(pt);
			if (itemIndex != -1)
				_listBox.SetSelected(itemIndex, true);
		}

		private void InitializeToolStrip()
		{
			ToolStripBuilder.Clear(_toolStrip.Items);
			if (_toolbarModel != null)
			{
				ToolStripBuilder.BuildToolbar(_toolStrip.Items, _toolbarModel.ChildNodes);
			}
		}

		private void InitializeMenu()
		{
			ToolStripBuilder.Clear(_contextMenu.Items);
			if (_menuModel != null)
			{
				ToolStripBuilder.BuildMenu(_contextMenu.Items, _menuModel.ChildNodes);
			}
		}

	}
}
