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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters
{
	[ExtensionPoint]
	public sealed class StudyFilterToolExtensionPoint : ExtensionPoint<ITool> {}

	public interface IStudyFilterToolContext : IToolContext
	{
		StudyFilterComponent Component { get; }
		IDesktopWindow DesktopWindow { get; }
	}

	public abstract class StudyFilterTool : Tool<IStudyFilterToolContext>
	{
		public const string DefaultToolbarActionSite = "studyfilter-toolbar";
		public const string DefaultContextMenuActionSite = "studyfilter-context";

		protected StudyFilterComponent Component
		{
			get { return base.Context.Component; }
		}

		protected StudyFilterColumnCollection Columns
		{
			get { return base.Context.Component.Columns; }
		}

		protected StudyItemSelection Selection
		{
			get { return base.Context.Component.Selection; }
		}

		protected IDesktopWindow DesktopWindow
		{
			get { return base.Context.DesktopWindow; }
		}

		public override void Initialize()
		{
			base.Initialize();
			this.Selection.SelectionChanged += SelectionChangedEventHandler;
		}

		protected override void Dispose(bool disposing)
		{
			this.Selection.SelectionChanged -= SelectionChangedEventHandler;
			base.Dispose(disposing);
		}

		private void SelectionChangedEventHandler(object sender, EventArgs e)
		{
			this.OnSelectionChanged();
		}

		protected virtual void OnSelectionChanged() {}
	}
}