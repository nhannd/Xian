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
