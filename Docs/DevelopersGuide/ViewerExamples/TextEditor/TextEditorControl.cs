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
using ClearCanvas.Desktop.View.WinForms;

namespace MyPlugin.TextEditor
{
	public partial class TextEditorControl : ApplicationComponentUserControl
	{
		private readonly TextEditorComponent _component;

		public TextEditorControl(TextEditorComponent component) : base(component)
		{
			InitializeComponent();

			_component = component;

			_txtText.WordWrap = TextEditorSettings.Default.WordWrap;

			_txtText.DataBindings.Add("Text", _component,
			                          "Text", true,
			                          DataSourceUpdateMode.OnPropertyChanged);

			_txtFilename.DataBindings.Add("Text", _component,
			                              "Filename", true,
			                              DataSourceUpdateMode.OnPropertyChanged);

			_txtWordCount.DataBindings.Add("Text", _component,
			                               "WordCount", true,
			                               DataSourceUpdateMode.OnPropertyChanged);

			_btnSave.Click += delegate(object sender, EventArgs args) { _component.Save(); };

			_btnCancel.Click += delegate(object sender, EventArgs args) { _component.Cancel(); };
		}
	}
}