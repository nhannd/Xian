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