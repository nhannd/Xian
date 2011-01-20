#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.View.WinForms
{
	public partial class DropListPickerField : UserControl
	{
	    private event EventHandler _checkedItemsChanged;
	    private event ListControlConvertEventHandler _format;

		private readonly CheckedListBox _listBox;

		private readonly ArrayList _items;
        private readonly ArrayList _checkedItems;

	    private object _nullItem;

		public DropListPickerField()
		{
			InitializeComponent();

			_listBox = new CheckedListBox();
			_listBox.BorderStyle = BorderStyle.None;
			_listBox.CheckOnClick = true;
			_listBox.SelectionMode = SelectionMode.One;
            _listBox.Format += new ListControlConvertEventHandler(_listBox_Format);
		    _listBox.FormattingEnabled = true;
			
			// We don't sort the list automatically, because we want the "Clear" item at the top.
			_listBox.Sorted = false;
			_listBox.ItemCheck += new ItemCheckEventHandler(OnListItemCheck);

			_contextMenuStrip.Items.Add(new ToolStripControlHost(_listBox));

            _items = new ArrayList();
            _checkedItems = new ArrayList();
		}

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public IList Items
        {
            get { return _items; }
            set
            {
                if (!TestEqual(_items, value))
                {
                    SetItems(value);
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] 
		[Browsable(false)]
		public IList CheckedItems
		{
			get
			{
				return _checkedItems;
			}
			set
			{
                if (!TestEqual(_checkedItems, value))
                {
                    SetCheckedItems(value);
                }
			}
		}

        public event EventHandler CheckedItemsChanged
        {
            add { _checkedItemsChanged += value; }
            remove { _checkedItemsChanged -= value; }
        }


        /// <summary>
        /// Gets or sets an object that will used as a "null" value.  If set,
        /// this object will appear at the top of the list, and clicking it will
        /// have the effect of clearing the list.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public object NullItem
	    {
            get { return _nullItem; }
            set { _nullItem = value; }
	    }

		public string LabelText
		{
			get { return _label.Text; }
			set { _label.Text = value; }
		}

        /// <summary>
        /// Occurs to allow formatting of the item for display in the user-interface.
        /// </summary>
        public event ListControlConvertEventHandler Format
        {
            add { _format += value; }
            remove { _format -= value; }
        }
        
        private void SetItems(IList items)
		{

		    _listBox.Items.Clear();

            // add null item if defined
            if (_nullItem != null)
                _listBox.Items.Add(_nullItem);

            _items.Clear();
            if (items != null)
                _items.AddRange(items);
            _listBox.Items.AddRange(_items.ToArray());

            // clear the checked items
		    this.CheckedItems = new ArrayList();
		}

		private void SetCheckedItems(IList checkedItems)
		{
			_checkedItems.Clear();

            if(checkedItems != null)
                _checkedItems.AddRange(checkedItems);

			for (int index = 0; index < _listBox.Items.Count; ++index)
			{
				object item = _listBox.Items[index];
                if (item.Equals(_nullItem))
                    continue;

				_listBox.SetItemChecked(index, _checkedItems.Contains(item));
			}

			UpdateText();

            EventsHelper.Fire(_checkedItemsChanged, this, EventArgs.Empty);
		}

		private void OnListItemCheck(object sender, ItemCheckEventArgs e)
		{
            object item = _listBox.Items[e.Index];
            if (item.Equals(_nullItem)) // "Clear" item
			{
				e.NewValue = CheckState.Unchecked;
			    this.CheckedItems = new ArrayList();
				return;
			}

			if (e.NewValue == CheckState.Unchecked)
			{
    			_checkedItems.Remove(item);
			}
			else
			{
				if (!_checkedItems.Contains(item))
					_checkedItems.Add(item);
			}

			UpdateText();

            EventsHelper.Fire(_checkedItemsChanged, this, EventArgs.Empty);
        }

		private void UpdateText()
		{
			string newText = "";
			foreach (object item in _checkedItems)
			{
				newText += FormatItem(item) + @", ";
			}

			if (newText.Length > 0)
				newText = newText.Remove(newText.Length - 2);

			this._itemString.Text = newText;
		}

        private void _showItemListButton_Click(object sender, EventArgs e)
        {
            _contextMenuStrip.Show(_showItemListButton, new Point(0, _showItemListButton.Height), ToolStripDropDownDirection.BelowRight);
        }

        private void _listBox_Format(object sender, ListControlConvertEventArgs e)
        {
            e.Value = FormatItem(e.ListItem);
        }

        private bool TestEqual(IList x, IList y)
        {
            return CollectionUtils.Equal(x, y, false);
        }

        private string FormatItem(object item)
        {
            ListControlConvertEventArgs args = new ListControlConvertEventArgs(item, typeof(string), item);
            EventsHelper.Fire(_format, this, args);

            return args.Value.ToString();
        }
	}
}
