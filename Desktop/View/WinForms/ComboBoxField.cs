#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
	public partial class ComboBoxField : UserControl
	{
		class WellBehavedComboBox : ComboBox
		{
			protected override void OnCreateControl()
			{
				base.OnCreateControl();

				// if the DataSource property was set, the SelectedItem will be
				// set to the first value in the DataSource list, which means it
				// may be out of since with the value it is bound to on the app component
				// we can fix this by setting it to null here
				this.SelectedItem = null;
			}
		}

		public ComboBoxField()
		{
			InitializeComponent();
		}

		public event ListControlConvertEventHandler Format
		{
			add { _comboBox.Format += value; }
			remove { _comboBox.Format -= value; }
		}

		public string LabelText
		{
			get { return _label.Text; }
			set { _label.Text = value; }
		}

		public ComboBoxStyle DropDownStyle
		{
			get { return _comboBox.DropDownStyle; }
			set { _comboBox.DropDownStyle = value; }
		}

		public object Value
		{
			get { return _comboBox.SelectedItem; }
			set
			{
				// Conver DBNUll to null.  If this is not done and a property bound to Value is set to null,
				// the displayed value of the combo box is not updated correctly.
				if (value is DBNull)
				{
					_comboBox.SelectedItem = null;
				}
				else
				{
					_comboBox.SelectedItem = value;
				}
			}
		}

		public event EventHandler ValueChanged
		{
			// use pass through event subscription
			add { _comboBox.SelectedIndexChanged += value; }
			remove { _comboBox.SelectedIndexChanged -= value; }
		}

		public new string Text
		{
			get { return _comboBox.Text; }
			set { _comboBox.Text = value; }
		}

		public new event EventHandler TextChanged
		{
			add { _comboBox.TextChanged += value; }
			remove { _comboBox.TextChanged -= value; }
		}

		public object DataSource
		{
			get { return _comboBox.DataSource; }
			set
			{
				// Conver DBNUll to null.  If this is not done and a property bound to Value is set to null,
				// the displayed value of the combo box is not updated correctly.
				if (value is DBNull)
				{
					_comboBox.DataSource = null;
				}
				else
				{
					_comboBox.DataSource = value;
				}
			}
		}

		public string DisplayMember
		{
			get { return _comboBox.DisplayMember; }
			set { _comboBox.DisplayMember = value; }
		}
	}
}
