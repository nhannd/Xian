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

using System.Collections.Generic;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Explorer.Local;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Tools
{
	public abstract class LocalExplorerStudyFilterToolProxy<T> : StudyFilterTool
		where T : ToolBase, new()
	{
		private T _baseTool;

		protected LocalExplorerStudyFilterToolProxy()
		{
			_baseTool = new T();
		}

		protected T BaseTool
		{
			get { return _baseTool; }
		}

		protected IActionSet BaseActions
		{
			get { return _baseTool.Actions; }
		}

		public override void Initialize()
		{
			base.Initialize();
			_baseTool.SetContext(new ToolContextProxy(this));
			_baseTool.Initialize();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && _baseTool != null)
			{
				_baseTool.Dispose();
				_baseTool = null;
			}
			base.Dispose(disposing);
		}

		private class ToolContextProxy : ILocalImageExplorerToolContext
		{
			private readonly LocalExplorerStudyFilterToolProxy<T> _owner;
			private ClickHandlerDelegate _defaultActionHandler;

			public ToolContextProxy(LocalExplorerStudyFilterToolProxy<T> owner)
			{
				_owner = owner;
			}

			public IEnumerable<string> SelectedPaths
			{
				get
				{
					foreach (StudyItem item in _owner.SelectedItems)
						yield return item.File.FullName;
				}
			}

			public IDesktopWindow DesktopWindow
			{
				get { return _owner.Context.DesktopWindow; }
			}

			public ClickHandlerDelegate DefaultActionHandler
			{
				get { return _defaultActionHandler; }
				set { _defaultActionHandler = value; }
			}
		}
	}
}