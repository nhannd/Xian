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

using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.View.WinForms
{
	public abstract class WinFormsActionView : WinFormsView, IActionView
	{
		private IActionViewContext _context;

		protected WinFormsActionView()
		{
		}

		protected IActionViewContext Context
		{
			get { return _context; }	
		}

		protected virtual void OnSetContext()
		{ }

		#region IActionView Members

		IActionViewContext IActionView.Context
		{
			get { return Context; }
			set
			{
				_context = value;
				OnSetContext();
			}
		}

		#endregion
	}

	internal class StandardWinFormsActionView : WinFormsActionView
	{
		private delegate object CreateGuiElementDelegate(IActionViewContext context);

		private object _guiElement;
		private readonly CreateGuiElementDelegate _createGuiElement;

		private StandardWinFormsActionView(CreateGuiElementDelegate createGuiElement)
		{
			_createGuiElement = createGuiElement;
		}

		public override object GuiElement
		{
			get
			{
				if (_guiElement == null)
					_guiElement = _createGuiElement(base.Context);

				return _guiElement;
			}
		}

		public static IActionView CreateDropDownButtonActionView()
		{
			return new StandardWinFormsActionView(
				delegate(IActionViewContext context)
				{
					DropDownButtonToolbarItem item = new DropDownButtonToolbarItem((IClickAction)context.Action, context.IconSize);
					context.IconSizeChanged += delegate { item.IconSize = context.IconSize; };
					return item;
				});
		}

		public static IActionView CreateDropDownActionView()
		{
			return new StandardWinFormsActionView(
				delegate(IActionViewContext context)
				{
					DropDownToolbarItem item = new DropDownToolbarItem((IDropDownAction) context.Action, context.IconSize);
					context.IconSizeChanged += delegate { item.IconSize = context.IconSize; };
					return item;
				});
		}

		public static IActionView CreateButtonActionView()
		{
			return new StandardWinFormsActionView(
				delegate(IActionViewContext context)
				{
					ActiveToolbarButton item = new ActiveToolbarButton((IClickAction) context.Action, context.IconSize);
					context.IconSizeChanged += delegate { item.IconSize = context.IconSize; };
					return item;
				});
		}

		public  static IActionView CreateMenuActionView()
		{
			return new StandardWinFormsActionView(
				delegate(IActionViewContext context)
				{
					ActiveMenuItem item = new ActiveMenuItem((IClickAction)context.Action, context.IconSize);
					context.IconSizeChanged += delegate { item.IconSize = context.IconSize; };
					return item;
				});
		}
	}
}