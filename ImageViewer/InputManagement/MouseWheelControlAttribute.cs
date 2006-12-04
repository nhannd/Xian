using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.InputManagement
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class MouseWheelControlAttribute : Attribute
	{
		private string _wheelIncrementDelegateName;
		private string _wheelDecrementDelegateName;
		private MouseWheelShortcut _shortcut;

		public MouseWheelControlAttribute(string wheelIncrementDelegateName, string wheelDecrementDelegateName)
			: this(wheelIncrementDelegateName, wheelDecrementDelegateName, ModifierFlags.None)
		{
		}

		public MouseWheelControlAttribute(string wheelIncrementDelegateName, string wheelDecrementDelegateName, ModifierFlags modifiers)
		{
			_wheelIncrementDelegateName = wheelIncrementDelegateName;
			_wheelDecrementDelegateName = wheelDecrementDelegateName;
			_shortcut = new MouseWheelShortcut(modifiers);
		}

		private MouseWheelControlAttribute()
		{ 
		}

		public string WheelIncrementDelegateName
		{
			get { return _wheelIncrementDelegateName; }	
		}

		public string WheelDecrementDelegateName
		{
			get { return _wheelDecrementDelegateName; }
		}

		public MouseWheelShortcut Shortcut
		{
			get { return _shortcut; }
		}
	}
}
