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
		private Modifiers _modifiers;

		public MouseWheelControlAttribute(string wheelIncrementDelegateName, string wheelDecrementDelegateName)
			: this(wheelIncrementDelegateName, wheelDecrementDelegateName, ModifierFlags.None)
		{
		}

		public MouseWheelControlAttribute(string wheelIncrementDelegateName, string wheelDecrementDelegateName, ModifierFlags modifiers)
		{
			_wheelIncrementDelegateName = wheelIncrementDelegateName;
			_wheelDecrementDelegateName = wheelDecrementDelegateName;
			_modifiers = new Modifiers(modifiers);
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

		public Modifiers Modifiers
		{
			get { return _modifiers; }
		}

		public MouseWheelDelegatePair CreateDelegatePair(object wheelControlObject)
		{
			MouseWheelDelegate wheelIncrementDelegate = (MouseWheelDelegate)Delegate.CreateDelegate(typeof(MouseWheelDelegate), wheelControlObject, _wheelIncrementDelegateName);
			MouseWheelDelegate wheelDecrementDelegate = (MouseWheelDelegate)Delegate.CreateDelegate(typeof(MouseWheelDelegate), wheelControlObject, _wheelDecrementDelegateName);

			return new MouseWheelDelegatePair(wheelIncrementDelegate, wheelDecrementDelegate);
		}
	}
}
