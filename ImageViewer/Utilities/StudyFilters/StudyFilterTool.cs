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
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters
{
	[ExtensionPoint]
	public sealed class StudyFilterToolExtensionPoint : ExtensionPoint<ITool> {}

	public interface IStudyFilterToolContext : IToolContext
	{
		IDesktopWindow DesktopWindow { get; }

		StudyItem ActiveItem { get; }
		StudyFilterColumn ActiveColumn { get; }
		event EventHandler ActiveChanged;

		StudyItemSelection SelectedItems { get; }

		IList<StudyItem> Items { get; }
		IStudyFilterColumnCollection Columns { get; }

		bool BulkOperationsMode { get; set; }

		bool Load(bool allowCancel, IEnumerable<string> paths, bool recursive);
		int Load(IEnumerable<string> paths, bool recursive);
		void Refresh();
		void Refresh(bool force);
	}

	public abstract class StudyFilterTool : Tool<IStudyFilterToolContext>
	{
		public const string DefaultToolbarActionSite = "studyfilter-toolbar";
		public const string DefaultContextMenuActionSite = "studyfilter-context";

		protected IStudyFilterColumnCollection Columns
		{
			get { return base.Context.Columns; }
		}

		protected IList<StudyItem> Items
		{
			get { return base.Context.Items; }
		}

		protected StudyItemSelection SelectedItems
		{
			get { return base.Context.SelectedItems; }
		}

		protected StudyItem ActiveItem
		{
			get { return base.Context.ActiveItem; }
		}

		protected StudyFilterColumn ActiveColumn
		{
			get { return base.Context.ActiveColumn; }
		}

		protected IDesktopWindow DesktopWindow
		{
			get { return base.Context.DesktopWindow; }
		}

		public override void Initialize()
		{
			base.Initialize();
			this.SelectedItems.SelectionChanged += SelectionChangedEventHandler;
			this.Context.ActiveChanged += ActiveChangedEventHandler;
		}

		protected override void Dispose(bool disposing)
		{
			this.Context.ActiveChanged -= ActiveChangedEventHandler;
			this.SelectedItems.SelectionChanged -= SelectionChangedEventHandler;
			base.Dispose(disposing);
		}

		private void SelectionChangedEventHandler(object sender, EventArgs e)
		{
			this.OnSelectionChanged();
		}

		protected virtual void OnSelectionChanged()
		{
			this.AtLeastOneSelected = this.SelectedItems.Count > 0;
		}

		private void ActiveChangedEventHandler(object sender, EventArgs e)
		{
			this.OnActiveChanged();
		}

		protected virtual void OnActiveChanged() {}

		private bool _atLeastOneSelected;

		public event EventHandler AtLeastOneSelectedChanged;

		protected virtual void OnAtLeastOneSelectedChanged()
		{
			EventsHelper.Fire(this.AtLeastOneSelectedChanged, this, EventArgs.Empty);
		}

		public bool AtLeastOneSelected
		{
			get { return _atLeastOneSelected; }
			private set
			{
				if (_atLeastOneSelected != value)
				{
					_atLeastOneSelected = value;
					this.OnAtLeastOneSelectedChanged();
				}
			}
		}

		protected void RefreshTable()
		{
			this.Context.Refresh();
		}

		protected void RefreshTable(bool forceRefresh)
		{
			this.Context.Refresh(forceRefresh);
		}

		protected bool Load(params string[] paths)
		{
			return Load((IEnumerable<string>) paths);
		}

		protected bool Load(IEnumerable<string> paths)
		{
			return Load(true, paths);
		}

		protected bool Load(bool allowCancel, params string[] paths)
		{
			return Load(allowCancel, (IEnumerable<string>) paths);
		}

		protected bool Load(bool allowCancel, IEnumerable<string> paths)
		{
			return this.Context.Load(allowCancel, paths, true);
		}
	}
}