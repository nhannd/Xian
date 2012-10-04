#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Windows.Forms;

namespace MyPlugin.TextEditor
{
	public partial class TextEditorConfigControl : UserControl
	{
		private TextEditorConfigComponent _component;

		public TextEditorConfigControl(TextEditorConfigComponent component)
		{
			InitializeComponent();

			_component = component;

			_chkWordWrap.DataBindings.Add("Checked", _component,
			                              "WordWrap", false,
			                              DataSourceUpdateMode.OnPropertyChanged);
		}
	}
}