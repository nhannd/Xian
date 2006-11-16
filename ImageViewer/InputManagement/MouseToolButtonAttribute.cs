using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.InputManagement
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class MouseToolButtonAttribute : Attribute
	{
		private XMouseButtons _mouseButton;
		private bool _initiallyActive;

		public MouseToolButtonAttribute(XMouseButtons mouseButton, bool initiallyActive)
		{
			_mouseButton = mouseButton;
			_initiallyActive = initiallyActive;
		}

		private MouseToolButtonAttribute()
		{ 
		}

		public XMouseButtons MouseButton
		{
			get { return _mouseButton; }
		}

		public bool InitiallyActive
		{
			get { return _initiallyActive; }
		}
	}
}