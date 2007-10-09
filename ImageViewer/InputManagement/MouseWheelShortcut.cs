using System;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.InputManagement
{
	public sealed class MouseWheelShortcut
	{
		private Modifiers _modifiers;
		private string _description;

		public MouseWheelShortcut()
			: this(false, false, false)
		{
		}

		public MouseWheelShortcut(Modifiers modifiers)
		{
			_modifiers = modifiers;
			_description = String.Format(SR.FormatMouseWheelShortcutDescription, _modifiers.ToString());
		}

		public MouseWheelShortcut(ModifierFlags modifierFlags)
			: this(new Modifiers(modifierFlags))
		{
		}

		public MouseWheelShortcut(bool control, bool alt, bool shift)
			: this(new Modifiers(control, alt, shift))
		{
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

		public override string ToString()
		{
			return _description;
		}
	}
}
