#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
	public static class ConvertEnum
	{
		public static ModifierFlags GetModifierFlags(DragEventArgs dragEventArgs)
		{
			DragEventArgsKeyState keyState = (DragEventArgsKeyState) dragEventArgs.KeyState;
			ModifierFlags modifiers = ModifierFlags.None;
			if ((keyState & DragEventArgsKeyState.Shift) == DragEventArgsKeyState.Shift)
				modifiers |= ModifierFlags.Shift;
			if ((keyState & DragEventArgsKeyState.Ctrl) == DragEventArgsKeyState.Ctrl)
				modifiers |= ModifierFlags.Control;
			if ((keyState & DragEventArgsKeyState.Alt) == DragEventArgsKeyState.Alt)
				modifiers |= ModifierFlags.Alt;
			return modifiers;
		}

		public static DragDropOption GetDragDropAction(DragDropEffects dragDropEffects)
		{
			DragDropOption action = DragDropOption.None;
			if ((dragDropEffects & DragDropEffects.Move) == DragDropEffects.Move)
				action |= DragDropOption.Move;
			if ((dragDropEffects & DragDropEffects.Copy) == DragDropEffects.Copy)
				action |= DragDropOption.Copy;
			return action;
		}

		public static DragDropEffects GetDragDropEffects(DragDropOption dragDropAction)
		{
			DragDropEffects effects = DragDropEffects.None;
			if ((dragDropAction & DragDropOption.Move) == DragDropOption.Move)
				effects |= DragDropEffects.Move;
			if ((dragDropAction & DragDropOption.Copy) == DragDropOption.Copy)
				effects |= DragDropEffects.Copy;
			return effects;
		}

		[Flags]
		private enum DragEventArgsKeyState
		{
			LeftButton = 1,
			RightButton = 2,
			Shift = 4,
			Ctrl = 8,
			MiddleButton = 16,
			Alt = 32
		}
	}
}