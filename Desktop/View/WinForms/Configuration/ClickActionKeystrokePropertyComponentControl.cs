#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Windows.Forms;
using ClearCanvas.Desktop.Configuration.ActionModel;

namespace ClearCanvas.Desktop.View.WinForms.Configuration
{
	public partial class ClickActionKeystrokePropertyComponentControl : UserControl
	{
		private static readonly IList<XKeys> _invalidKeyStrokes;
		private readonly ClickActionKeystrokePropertyComponent _component;

		public ClickActionKeystrokePropertyComponentControl(ClickActionKeystrokePropertyComponent component)
		{
			InitializeComponent();

			_component = component;

			_keyStrokeCaptureBox.DataBindings.Add("KeyStroke", component, "KeyStroke", false, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void _keyStrokeCaptureBox_ValidateKeyStroke(object sender, ValidateKeyStrokeEventArgs e)
		{
			e.IsValid = e.IsValid && !_invalidKeyStrokes.Contains(e.KeyStroke) && _component.IsValidKeyStroke(e.KeyStroke);
		}

		static ClickActionKeystrokePropertyComponentControl()
		{
			// these invalid key strokes are specific to the way we listen for keyboard events using the WinForms toolkit
			// they may be different depending on platform and toolkit, which is why this logic is not implemented model-side.
			var invalidKeyStrokes = new List<XKeys>();
			invalidKeyStrokes.Add(XKeys.Control | XKeys.Alt | XKeys.Delete);
			invalidKeyStrokes.Add(XKeys.Control | XKeys.Shift | XKeys.Escape);
			invalidKeyStrokes.Add(XKeys.Control | XKeys.Escape);
			invalidKeyStrokes.Add(XKeys.Alt | XKeys.PrintScreen);
			invalidKeyStrokes.Add(XKeys.Alt | XKeys.Tab);
			invalidKeyStrokes.Add(XKeys.PrintScreen);
			invalidKeyStrokes.Add(XKeys.LeftWinKey);
			invalidKeyStrokes.Add(XKeys.RightWinKey);
			_invalidKeyStrokes = invalidKeyStrokes.AsReadOnly();
		}
	}
}