#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Specifies a folder's default path within its folder system.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class FolderPathAttribute : Attribute
	{
		private readonly string _path;
		private readonly bool _startExpanded;

		public FolderPathAttribute(string path)
		{
			_path = path;
		}

		public FolderPathAttribute(string path, bool startExpanded)
		{
			_path = path;
			_startExpanded = startExpanded;
		}

		public string Path
		{
			get { return _path; }
		}

		public bool StartExpanded
		{
			get { return _startExpanded; }
		}
	}

	/// <summary>
	/// Abstract base implementation of <see cref="IFolder"/>.
	/// </summary>
	public abstract class Folder : IFolder
	{
		private event EventHandler _textChanged;
		private event EventHandler _iconChanged;
		private event EventHandler _tooltipChanged;
		private event EventHandler _refreshBegin;
		private event EventHandler _refreshFinish;
		private event EventHandler _totalItemCountChanged;

		private ActionModelNode _menuModel;

		private IResourceResolver _resourceResolver;
		private bool _isOpen;

		private readonly IList<IFolder> _subfolders;
		private Path _folderPath;
		private readonly bool _startExpanded;
		private bool _isStatic = true;

		private static readonly IconSet _closedIconSet = new IconSet(IconScheme.Colour, "FolderClosedSmall.png", "FolderClosedMedium.png", "FolderClosedMedium.png");
		private static readonly IconSet _openIconSet = new IconSet(IconScheme.Colour, "FolderOpenSmall.png", "FolderOpenMedium.png", "FolderOpenMedium.png");
		private IconSet _iconSet;

		/// <summary>
		/// Constructor
		/// </summary>
		public Folder()
		{
			// establish default resource resolver on this assembly (not the assembly of the derived class)
			_resourceResolver = new ResourceResolver(typeof(Folder).Assembly);

			// initialize icon set to "closed"
			_iconSet = _closedIconSet;

			_subfolders = new List<IFolder>();

			// Initialize folder Path
			FolderPathAttribute attrib = AttributeUtils.GetAttribute<FolderPathAttribute>(this.GetType());
			if (attrib != null)
			{
				_folderPath = new Path(attrib.Path, _resourceResolver);
				_startExpanded = attrib.StartExpanded;
			}
		}

		/// <summary>
		/// Constructor.  Values passed to this constructor will override any <see cref="FolderPathAttribute"/> declaration.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="startExpanded"></param>
		public Folder(string path, bool startExpanded)
		{
			// establish default resource resolver on this assembly (not the assembly of the derived class)
			_resourceResolver = new ResourceResolver(typeof(Folder).Assembly);

			// initialize icon set to "closed"
			_iconSet = _closedIconSet;

			_subfolders = new List<IFolder>();
			_folderPath = new Path(path, _resourceResolver);
			_startExpanded = startExpanded;
		}


		#region IFolder Members

		/// <summary>
		/// Gets the ID that identifies the folder
		/// </summary>
		public virtual string Id
		{
			get { return this.GetType().Name; }
		}

		/// <summary>
		/// Gets the text that should be displayed for the folder.
		/// </summary>
		/// <remarks>
		/// The default implementation of this property returns the <see cref="Name"/> of the folder,
		/// followed by the <see cref="TotalItemCount"/>, if <see cref="IsPopulated"/> returns true.
		/// </remarks>
		public virtual string Text
		{
			get
			{
				return this.IsPopulated ?
					string.Format("{0} ({1})", this.Name, this.TotalItemCount) : this.Name;
			}
		}

		/// <summary>
		/// Gets the folder name, which is the last part of the <see cref="FolderPath"/> value.
		/// </summary>
		public string Name
		{
			get { return _folderPath != null ? _folderPath.LastSegment.LocalizedText : string.Empty; }
		}

		/// <summary>
		/// Allows the folder to notify that it's text has changed
		/// </summary>
		public event EventHandler TextChanged
		{
			add { _textChanged += value; }
			remove { _textChanged -= value; }
		}

		/// <summary>
		/// Asks the folder to refresh its contents.  The implementation may be asynchronous.
		/// </summary>
		public abstract void Refresh();

		/// <summary>
		/// Asks the folder to refresh the count of its contents, without actually refreshing the contents.
		/// The implementation may be asynchronous.
		/// </summary>
		public abstract void RefreshCount();

		/// <summary>
		/// Opens the folder (i.e. instructs the folder to show its "open" state icon).
		/// </summary>
		public virtual void OpenFolder()
		{
			this.IconSet = OpenIconSet;

			_isOpen = true;
			Refresh();
		}

		/// <summary>
		/// Closes the folder (i.e. instructs the folder to show its "closed" state icon).
		/// </summary>
		public virtual void CloseFolder()
		{
			this.IconSet = ClosedIconSet;

			_isOpen = false;
		}

		/// <summary>
		/// Indicates if the folder should be initially expanded.
		/// </summary>
		public virtual bool StartExpanded
		{
			get { return _startExpanded; }
		}

		/// <summary>
		/// Gets the iconset that should be displayed for the folder.
		/// </summary>
		public IconSet IconSet
		{
			get { return _iconSet; }
			protected set
			{
				_iconSet = value;
				EventsHelper.Fire(_iconChanged, this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Allows the folder to nofity that its icon has changed.
		/// </summary>
		public event EventHandler IconChanged
		{
			add { _iconChanged += value; }
			remove { _iconChanged -= value; }
		}

		/// <summary>
		/// Gets the resource resolver associated with this folder.
		/// </summary>
		public IResourceResolver ResourceResolver
		{
			get { return _resourceResolver; }
			protected set { _resourceResolver = value; }
		}

		/// <summary>
		/// Gets the tooltip that should be displayed for the folder.
		/// </summary>
		public virtual string Tooltip
		{
			get { return null; }
		}

		/// <summary>
		/// Allows the folder to notify that its tooltip has changed.
		/// </summary>
		public event EventHandler TooltipChanged
		{
			add { _tooltipChanged += value; }
			remove { _tooltipChanged -= value; }
		}

		/// <summary>
		/// Occurs when refresh is about to begin.
		/// </summary>
		public event EventHandler RefreshBegin
		{
			add { _refreshBegin += value; }
			remove { _refreshBegin -= value; }
		}

		/// <summary>
		/// Occurs when refresh is about to finish.
		/// </summary>
		public event EventHandler RefreshFinish
		{
			add { _refreshFinish += value; }
			remove { _refreshFinish -= value; }
		}

		/// <summary>
		/// Gets the menu model for the context menu that should be displayed when the user right-clicks on the folder.
		/// </summary>
		public ActionModelNode MenuModel
		{
			get { return _menuModel; }
			protected set { _menuModel = value; }
		}

		/// <summary>
		/// Gets the open/close state of the current folder
		/// </summary>
		public bool IsOpen
		{
			get { return _isOpen; }
			protected set { _isOpen = value; }
		}

		/// <summary>
		/// Asks the folder if it can accept a drop of the specified items
		/// </summary>
		/// <param name="items"></param>
		/// <param name="kind"></param>
		/// <returns></returns>
		public virtual DragDropKind CanAcceptDrop(object[] items, DragDropKind kind)
		{
			return DragDropKind.None;
		}

		/// <summary>
		/// Instructs the folder to accept the specified items
		/// </summary>
		/// <param name="items"></param>
		/// <param name="kind"></param>
		public virtual DragDropKind AcceptDrop(object[] items, DragDropKind kind)
		{
			return DragDropKind.None;
		}

		/// <summary>
		/// Informs the folder that the specified items were dragged from it.  It is up to the implementation
		/// of the folder to determine the appropriate response (e.g. whether the items should be removed or not).
		/// </summary>
		/// <param name="items"></param>
		/// <param name="result">The result of the drag drop operation</param>
		public virtual void DragComplete(object[] items, DragDropKind result)
		{
		}

		/// <summary>
		/// Gets a table of the items that are contained in this folder
		/// </summary>
		public abstract ITable ItemsTable
		{
			get;
		}

		/// <summary>
		/// Gets the total number of items "contained" in this folder, which may be the same
		/// as the number of items displayed in the <see cref="IFolder.ItemsTable"/>, or may be larger
		/// in the event the table is only showing a subset of the total number of items.
		/// </summary>
		public abstract int TotalItemCount
		{
			get;
		}

		/// <summary>
		/// Gets a value indicating whether this is populated.
		/// </summary>
		protected abstract bool IsPopulated { get; }

		/// <summary>
		/// Occurs when the value of the <see cref="IFolder.TotalItemCount"/> property changes.
		/// </summary>
		public event EventHandler TotalItemCountChanged
		{
			add { _totalItemCountChanged += value; }
			remove { _totalItemCountChanged -= value; }
		}

		/// <summary>
		/// Gets or sets the folder path which sets up the tree structure
		/// </summary>
		public Path FolderPath
		{
			get { return _folderPath; }
			set { _folderPath = value; }
		}

		/// <summary>
		/// Gets a list of sub folders
		/// </summary>
		public IList<IFolder> Subfolders
		{
			get { return _subfolders; }
		}

		/// <summary>
		/// Add a subfolder
		/// </summary>
		/// <param name="subFolder"></param>
		public void AddFolder(IFolder subFolder)
		{
			_subfolders.Add(subFolder);
		}

		/// <summary>
		/// Remove a sub folder
		/// </summary>
		/// <param name="subFolder"></param>
		/// <returns></returns>
		public bool RemoveFolder(IFolder subFolder)
		{
			return _subfolders.Remove(subFolder);
		}

		/// <summary>
		/// Replace a sub folder with another in its place.  The order of the subfolders is retained
		/// </summary>
		/// <param name="oldSubFolder"></param>
		/// <param name="newSubFolder"></param>
		/// <returns></returns>
		public bool ReplaceFolder(IFolder oldSubFolder, IFolder newSubFolder)
		{
			int oldFolderIndex = _subfolders.IndexOf(oldSubFolder);
			_subfolders.Insert(oldFolderIndex, newSubFolder);
			return _subfolders.Remove(oldSubFolder);
		}

		/// <summary>
		/// Gets a value indicating whether or not the folder is 'static'.
		/// </summary>
		/// <remarks>
		/// In the context of workflow, folders created via the normal constructor (new Folder(...)) are considered static and are
		/// otherwise they are considered generated if created by Activator.CreateInstance.
		/// </remarks>
		public bool IsStatic
		{
			get { return _isStatic; }
			set { _isStatic = value; }
		}

		#endregion

		#region Overridable members

		/// <summary>
		/// Gets the closed-state <see cref="IconSet"/>.
		/// </summary>
		/// <remarks>
		/// Override this property to provide a custom closed-state icon.
		/// </remarks>
		protected virtual IconSet ClosedIconSet
		{
			get { return _closedIconSet; }
		}

		/// <summary>
		/// Gets the open-state <see cref="IconSet"/>.
		/// </summary>
		/// <remarks>
		/// Override this property to provide a custom closed-state icon.
		/// </remarks>
		protected virtual IconSet OpenIconSet
		{
			get { return _openIconSet; }
		}

		#endregion

		#region Protected members

		protected void NotifyTextChanged()
		{
			EventsHelper.Fire(_textChanged, this, EventArgs.Empty);
		}

		protected void NotifyRefreshBegin()
		{
			EventsHelper.Fire(_refreshBegin, this, EventArgs.Empty);
		}

		protected void NotifyRefreshFinish()
		{
			EventsHelper.Fire(_refreshFinish, this, EventArgs.Empty);
		}

		protected void NotifyTotalItemCountChanged()
		{
			EventsHelper.Fire(_totalItemCountChanged, this, EventArgs.Empty);
		}

		#endregion
	}
}
