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
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.Common.Utilities;
using System.Diagnostics;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Comparers;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
    /// <summary>
    /// This tool runs an instance of <see cref="LayoutComponent"/> in a shelf, and coordinates
    /// it so that it reflects the state of the active workspace.
	/// </summary>
	[ClearCanvas.Common.ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ContextMenuLayoutTool : ImageViewerTool
	{
    	private ImageSetGroups _imageSetGroups;
    	private static readonly string _rootPath = "imageviewer-contextmenu";
    	private List<string> _currentPathElements;
		private int _actionNumber = 0;
    	private bool _showImageSetNames = false;
		private IComparer<IImageSet> _comparer = new StudyDateComparer();

		/// <summary>
        /// Constructor
        /// </summary>
		public ContextMenuLayoutTool()
		{
        }

		public override IActionSet Actions
		{
			get { return GetDisplaySetActions(); }
		}
		
		/// <summary>
        /// Overridden to subscribe to workspace activation events
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
			_imageSetGroups = new ImageSetGroups(base.Context.Viewer.LogicalWorkspace.ImageSets);
		}

		protected override void Dispose(bool disposing)
		{
			_imageSetGroups.Dispose();
			base.Dispose(disposing);
		}

		/// <summary>
		/// Gets an array of <see cref="IAction"/> objects that allow selection of specific display
		/// sets for display in the currently selected image box.
		/// </summary>
		/// <returns></returns>
		private IActionSet GetDisplaySetActions()
		{
#if TRACEGROUPS
			TraceGroups();
#endif
			_actionNumber = 1;
			_currentPathElements = new List<string>();
			List<IAction> actions = new List<IAction>();

			FilteredGroup<IImageSet> rootGroup = GetRootGroup();
			if (rootGroup != null)
			{
				_showImageSetNames = base.ImageViewer.LogicalWorkspace.ImageSets.Count > 1;
				
				foreach (FilteredGroup<IImageSet> group in TraverseNonEmptyGroups(rootGroup))
				{
					string basePath = StringUtilities.Combine(_currentPathElements, "/");

					//not incredibly efficient, but there really aren't that many items.
					List<IImageSet> orderedItems = new List<IImageSet>(group.Items);
					orderedItems.Sort(_comparer);

					foreach (IImageSet imageSet in orderedItems)
					{
						string imageSetPath;
						if (_showImageSetNames)
							imageSetPath = String.Format("{0}/{1}", basePath, imageSet.Name.Replace("/", "-"));
						else
							imageSetPath = basePath;

						foreach (IDisplaySet displaySet in imageSet.DisplaySets)
						{
							actions.Add(CreateDisplaySetAction(imageSetPath, displaySet));
							++_actionNumber;
						}
					}

					if (group.Items.Count > 0 && base.ImageViewer.IsLoadingPriors)
					{
						actions.Add(CreateLoadingPriorsAction(basePath));
						++_actionNumber;
					}
				}
			}
			
			return new ActionSet(actions);
		}

		private IEnumerable<FilteredGroup<IImageSet>> TraverseNonEmptyGroups(FilteredGroup<IImageSet> group)
		{
			List<IImageSet> allItems = group.GetAllItems();
			if (allItems.Count != 0)
			{
				if (_currentPathElements.Count == 0)
					_currentPathElements.Add(_rootPath);
				else
					_currentPathElements.Add(group.Label.Replace("/", "-"));

				yield return group;
			}

			foreach (FilteredGroup<IImageSet> child in group.ChildGroups)
			{
				foreach (FilteredGroup<IImageSet> nonEmptyChild in TraverseNonEmptyGroups(child))
					yield return nonEmptyChild;
			}

			if (allItems.Count != 0)
				_currentPathElements.RemoveAt(_currentPathElements.Count - 1);
		}

		private FilteredGroup<IImageSet> GetRootGroup()
		{
			return GetRootGroup(_imageSetGroups.Root);
		}

		private FilteredGroup<IImageSet> GetRootGroup(FilteredGroup<IImageSet> group)
		{
			if (group.HasItems)
				return group;

			int validChildGroups = 0;
    		foreach (FilteredGroup<IImageSet> child in group.ChildGroups)
    		{
    			if (child.GetAllItems().Count > 0)
    				++validChildGroups;
    		}

			//if this group has more than one child group with items anywhere in it's tree, then it's first.
			if (validChildGroups > 1)
				return group;

			foreach (FilteredGroup<IImageSet> child in group.ChildGroups)
			{
				FilteredGroup<IImageSet> rootGroup = GetRootGroup(child);
				if (rootGroup != null)
					return rootGroup;
			}

    		return null;
		}

		private IClickAction CreateLoadingPriorsAction(string basePath)
		{
			string pathString = String.Format("{0}/display{1}", basePath, _actionNumber);
			ActionPath path = new ActionPath(pathString, null);
			MenuAction action = new MenuAction(string.Format("{0}:display{1}", this.GetType().FullName, _actionNumber), path, ClickActionFlags.None, null);
			action.GroupHint = new GroupHint("DisplaySets");
			action.Label = SR.LabelLoadingPriors;
			action.SetClickHandler(delegate { });
			return action;
		}

    	/// <summary>
		/// Creates an <see cref="IClickAction"/> that displays the specified display set when clicked.  The index
		/// parameter is used to generate a label for the action.
		/// </summary>
		/// <param name="displaySet"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		private IClickAction CreateDisplaySetAction(string basePath, IDisplaySet displaySet)
		{
    		string pathString = String.Format("{0}/display{1}", basePath, _actionNumber);
			Trace.WriteLine(String.Format("Path: {0}", pathString));

			ActionPath path = new ActionPath(pathString, null);
			MenuAction action = new MenuAction(string.Format("{0}:display{1}", this.GetType().FullName, _actionNumber), path, ClickActionFlags.CheckParents, null);
			action.GroupHint = new GroupHint("DisplaySets");
			action.Label = displaySet.Name;
			action.SetClickHandler(delegate { AssignDisplaySetToImageBox(displaySet); });

			action.Checked = this.ImageViewer.SelectedImageBox != null &&
				this.ImageViewer.SelectedImageBox.DisplaySet != null && 
				this.ImageViewer.SelectedImageBox.DisplaySet.Uid == displaySet.Uid;

			return action;
		}

		private void AssignDisplaySetToImageBox(IDisplaySet displaySet)
		{
			IImageBox imageBox = this.ImageViewer.SelectedImageBox;
			MemorableUndoableCommand memorableCommand = new MemorableUndoableCommand(imageBox);
			memorableCommand.BeginState = imageBox.CreateMemento();

			// always create a 'fresh copy' to show in the image box.  We never want to show
			// the 'originals' (e.g. the ones in IImageSet.DisplaySets) because we want them 
			// to remain clean and unaltered - consider them to be templates for what actually
			// gets shown.
			imageBox.DisplaySet = displaySet.CreateFreshCopy();

			imageBox.Draw();
			//this.ImageViewer.SelectedImageBox[0, 0].Select();

			memorableCommand.EndState = imageBox.CreateMemento();

			DrawableUndoableCommand historyCommand = new DrawableUndoableCommand(imageBox);
			historyCommand.Enqueue(memorableCommand);
			this.ImageViewer.CommandHistory.AddCommand(historyCommand);
		}

#if TRACEGROUPS

		private void TraceGroups()
		{
			TraceGroup(_imageSetGroups.Root, _imageSetGroups.Root.Name);
		}

		private void TraceGroup(FilteredGroup<IImageSet> group, string currentGroupPath)
		{
			foreach (IImageSet imageSet in group.Items)
			{
				string imageSetPath = String.Format("{0}/{1}", currentGroupPath, imageSet.Name);
				Trace.WriteLine(imageSetPath);
			}

			foreach (FilteredGroup<IImageSet> childGroup in group.ChildGroups)
			{
				string name = childGroup.Label;
				string groupPath = String.Format("{0}/{1}", currentGroupPath, name);
				TraceGroup(childGroup, groupPath);
			}
		}
#endif
	}
}
