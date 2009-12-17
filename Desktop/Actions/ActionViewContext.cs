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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Actions
{
	/// <summary>
	/// Interface for an <see cref="IActionView"/>'s context.
	/// </summary>
	public interface IActionViewContext
	{
		/// <summary>
		/// Gets the associated <see cref="IAction"/>.
		/// </summary>
		IAction Action { get; }

		/// <summary>
		/// Gets or sets the <see cref="IconSize"/> to be shown by the <see cref="IActionView"/>.
		/// </summary>
		IconSize IconSize { get; set; }

		/// <summary>
		/// Fires when the <see cref="IconSize"/> has changed.
		/// </summary>
		event EventHandler IconSizeChanged;
	}

	/// <summary>
	/// Simple implementation of an <see cref="IActionViewContext"/>.
	/// </summary>
	public class ActionViewContext : IActionViewContext
	{
		private readonly IAction _action;
		private IconSize _iconSize;
		private event EventHandler _iconSizeChanged;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="action">The associated <see cref="IAction"/>.</param>
		public ActionViewContext(IAction action)
			: this(action, default(IconSize))
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="action">The associated <see cref="IAction"/>.</param>
		/// <param name="iconSize">The initial icon size.</param>
		public ActionViewContext(IAction action, IconSize iconSize)
		{
			Platform.CheckForNullReference(action, "action");
			_action = action;
			_iconSize = iconSize;
		}

		#region IActionViewContext Members

		/// <summary>
		/// Gets the associated <see cref="IAction"/>.
		/// </summary>
		public IAction Action
		{
			get { return _action; }
		}

		/// <summary>
		/// Gets or sets the <see cref="IActionViewContext.IconSize"/> to be shown by the <see cref="IActionView"/>.
		/// </summary>
		public IconSize IconSize
		{
			get
			{
				return _iconSize;
			}
			set
			{
				if (_iconSize != value)
				{
					_iconSize = value;
					EventsHelper.Fire(_iconSizeChanged, this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Fires when the <see cref="IActionViewContext.IconSize"/> has changed.
		/// </summary>
		public event EventHandler IconSizeChanged
		{
			add { _iconSizeChanged += value; }
			remove { _iconSizeChanged -= value; }
		}

		#endregion
	}
}