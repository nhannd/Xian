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

using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.Layout.Basic;

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
	/// <summary>
	/// A WinForms view of the <see cref="LayoutChangerAction"/>.
	/// </summary>
	[ExtensionOf(typeof (LayoutChangerActionViewExtensionPoint))]
	public class LayoutChangerActionView : WinFormsView, IActionView
	{
		private LayoutChangerAction _action;
		private LayoutChangerToolStripItem _control;

		/// <summary>
		/// Called by the framework to set the action that the view looks at.
		/// </summary>
		public void SetAction(IAction action)
		{
			_action = (LayoutChangerAction) action;
		}

		/// <summary>
		/// Gets the <see cref="System.Windows.Forms.Control"/> that implements this view, allowing
		/// a parent view to insert the control as one of its children.
		/// </summary>
		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new LayoutChangerToolStripItem(_action);
				}
				return _control;
			}
		}
	}
}