#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms view onto <see cref="UnmergeOrderComponent"/>
	/// </summary>
	[ExtensionOf(typeof(UnmergeOrderComponentViewExtensionPoint))]
	public class UnmergeOrderComponentView : WinFormsView, IApplicationComponentView
	{
		private UnmergeOrderComponent _component;
		private UnmergeOrderComponentControl _control;


		#region IApplicationComponentView Members

		public void SetComponent(IApplicationComponent component)
		{
			_component = (UnmergeOrderComponent)component;
		}

		#endregion

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new UnmergeOrderComponentControl(_component);
				}
				return _control;
			}
		}
	}
}
