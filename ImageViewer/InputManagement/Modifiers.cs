using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.InputManagement
{
	public sealed class Modifiers
	{
		private ModifierFlags _modifierFlags;

		public Modifiers(bool control, bool alt, bool shift)
		{
			_modifierFlags = ModifierFlags.None;

			_modifierFlags |= (control) ? ModifierFlags.Control : ModifierFlags.None;
			_modifierFlags |= (alt) ? ModifierFlags.Alt : ModifierFlags.None;
			_modifierFlags |= (shift) ? ModifierFlags.Shift : ModifierFlags.None;
		}

		public Modifiers(ModifierFlags modifierFlags)
		{
			_modifierFlags = modifierFlags;
		}

		private Modifiers()
		{ 
		}

		public bool Control
		{
			get { return ((_modifierFlags & ModifierFlags.Control) == ModifierFlags.Control); } 
		}
		
		public bool Alt
		{
			get { return ((_modifierFlags & ModifierFlags.Alt) == ModifierFlags.Alt); }
		}

		public bool Shift
		{
			get { return ((_modifierFlags & ModifierFlags.Shift) == ModifierFlags.Shift); } 
		}

		public ModifierFlags ModifierFlags
		{
			get { return _modifierFlags; }
		}

		public override bool Equals(object obj)
		{
			if (obj is Modifiers)
			{
				Modifiers modifiers = (Modifiers)obj;
				return (modifiers.ModifierFlags == this.ModifierFlags);
			}

			return false;
		}

		public override int GetHashCode()
		{
			return _modifierFlags.GetHashCode();
		}
	}
}
