using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.InputManagement
{
	public sealed class MouseWheelShortcut
	{
		private Modifiers _modifiers;

		public MouseWheelShortcut()
			: this(false, false, false)
		{ 
		}

		public MouseWheelShortcut(Modifiers modifiers)
		{
			_modifiers = modifiers;
		}

		public MouseWheelShortcut(ModifierFlags modifierFlags)
		{
			_modifiers = new Modifiers(modifierFlags);
		}

		public MouseWheelShortcut(bool control, bool alt, bool shift)
		{
			_modifiers = new Modifiers(control, alt, shift);
		}

		public Modifiers Modifiers
		{
			get { return _modifiers; }
		}

		public override bool Equals(object obj)
		{
			if (obj is MouseWheelShortcut)
			{
				MouseWheelShortcut shortcut = (MouseWheelShortcut)obj;
				return shortcut.Modifiers.Equals(this.Modifiers);
			}

			return false;
		}

		public override int GetHashCode()
		{
			return _modifiers.GetHashCode();
		}
	}
}
