#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.InputManagement
{
	public sealed class MouseButtonShortcut
	{
		private XMouseButtons _mouseButton;
		private Modifiers _modifiers;
		private string _description;

		public MouseButtonShortcut(XMouseButtons mouseButton, ModifierFlags modifierFlags)
			: this(mouseButton, new Modifiers(modifierFlags))
		{
		}

		public MouseButtonShortcut(XMouseButtons mouseButton, Modifiers modifiers)
		{
			_mouseButton = mouseButton;
			_modifiers = modifiers ?? new Modifiers(ModifierFlags.None);
			_description = String.Format(SR.FormatMouseButtonShortcutDescription, _mouseButton.ToString(), _modifiers.ToString());
		}

		public MouseButtonShortcut(XMouseButtons mouseButton, bool control, bool alt, bool shift)
			: this(mouseButton, new Modifiers(control, alt, shift))
		{
		}

		public MouseButtonShortcut(XMouseButtons mouseButton)
			: this(mouseButton, false, false, false)
		{
		}
		
		public XMouseButtons MouseButton
		{
			get { return _mouseButton; }
		}

		public Modifiers Modifiers
		{
			get { return _modifiers; }
		}

		public bool IsModified
		{
			get { return _modifiers.ModifierFlags != ModifierFlags.None; }
		}

		public override bool Equals(object obj)
		{
			if (obj is MouseButtonShortcut)
			{
				MouseButtonShortcut shortcut = (MouseButtonShortcut)obj;
				return shortcut.MouseButton == this.MouseButton && shortcut.Modifiers.Equals(this.Modifiers);
			}

			return false;
		}

		public override int GetHashCode()
		{
			int returnvalue = 7;
			returnvalue = 11 * returnvalue + _modifiers.GetHashCode();
			returnvalue = 11 * returnvalue + _mouseButton.GetHashCode();
			return returnvalue;
		}

		public override string ToString()
		{
			return _description;
		}
	}
}
