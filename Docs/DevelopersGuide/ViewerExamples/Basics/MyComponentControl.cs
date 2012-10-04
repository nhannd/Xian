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
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MyPlugin.Basics
{
	public partial class MyComponentControl : UserControl
	{
		private readonly MyComponent _component;

		public MyComponentControl(MyComponent component)
		{
			InitializeComponent();

			_component = component;
		}
	}
}
