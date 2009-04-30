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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Represents a single page in a <see cref="StackTabComponentContainer"/>.
	/// </summary>
	public class StackTabPage : TabPage
	{
		private string _title;
		private IconSet _iconSet;
		private IResourceResolver _resourceResolver;

		private event EventHandler _titleChanged;
		private event EventHandler _iconSetChanged;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">The name of the page.</param>
		/// <param name="component">The <see cref="IApplicationComponent"/> to be hosted in this page.</param>
		/// <param name="title">The text to display on the title bar.</param>
		/// <param name="iconSet">The icon to display on the title bar.</param>
		/// <param name="fallbackResolver">Resource resolver to fall back on in case the default failed to find resources.</param>
		public StackTabPage(string name, 
			IApplicationComponent component, 
			string title, 
			IconSet iconSet,
			IResourceResolver fallbackResolver)
			: base(name, component)
		{
			_title = title;
			_iconSet = iconSet;
			_resourceResolver = new ResourceResolver(typeof(StackTabPage).Assembly, fallbackResolver);
		}

		/// <summary>
		/// The text to display on the title bar.
		/// </summary>
		public string Title
		{
			get { return _title; }
			set
			{
				if (_title == value)
					return;

				_title = value;
				EventsHelper.Fire(_titleChanged, this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="ClearCanvas.Desktop.IconSet"/> that should be displayed for the folder.
		/// </summary>
		public IconSet IconSet
		{
			get { return _iconSet; }
			set 
			{
				_iconSet = value;
				EventsHelper.Fire(_iconSetChanged, this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the resource resolver that is used to resolve the Icon.
		/// </summary>
		public IResourceResolver ResourceResolver
		{
			get { return _resourceResolver; }
			set { _resourceResolver = value; }
		}

		/// <summary>
		/// Occurs when <see cref="Title"/> has changed.
		/// </summary>
		public event EventHandler TitleChanged
		{
			add { _titleChanged += value; }
			remove { _titleChanged -= value; }
		}

		/// <summary>
		/// Occurs when <see cref="IconSet"/> has changed.
		/// </summary>
		public event EventHandler IconSetChanged
		{
			add { _iconSetChanged += value; }
			remove { _iconSetChanged -= value; }
		}
	}
}
