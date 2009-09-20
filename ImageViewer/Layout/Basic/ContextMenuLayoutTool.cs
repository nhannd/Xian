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
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Comparers;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	public class ContextMenuActionFactoryArgs
	{
		private int _nextActionNumber;

		internal ContextMenuActionFactoryArgs()
		{
		}

		public IDesktopWindow DesktopWindow { get; internal set; }
		public IImageViewer ImageViewer { get; internal set; }

		public string Namespace { get; internal set; }
		public string BasePath { get; internal set; }
	
		public IImageSet ImageSet { get; internal set; }

		public string GetNextActionId()
		{
			return String.Format("imageSetAction{0}", ++_nextActionNumber);
		}

		public string GetFullyQualifiedActionId(string actionId)
		{
			return String.Format("{0}:{1}", Namespace, actionId);
		}
	}

	[ExtensionPoint]
	public class ContextMenuActionFactoryExtensionPoint : ExtensionPoint<IContextMenuActionFactory>
	{
	}

	public interface IContextMenuActionFactory
	{
		IAction[] CreateActions(ContextMenuActionFactoryArgs args);
	}

	/// <summary>
    /// This tool runs an instance of <see cref="LayoutComponent"/> in a shelf, and coordinates
    /// it so that it reflects the state of the active workspace.
	/// </summary>
	[ClearCanvas.Common.ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ContextMenuLayoutTool : ImageViewerTool
	{
    	private const string _rootPath = "imageviewer-contextmenu";
		private static readonly List<IContextMenuActionFactory> _actionFactories = CreateActionFactories();

		private List<string> _currentPathElements;

		private ImageSetGroups _imageSetGroups;
		private readonly Dictionary<string, IImageSet> _unavailableImageSets;
		private readonly IComparer<IImageSet> _comparer = new StudyDateComparer();

		private readonly DefaultPatientReconciliationStrategy _patientReconciliationStrategy = new DefaultPatientReconciliationStrategy();

		public ContextMenuLayoutTool()
		{
			_unavailableImageSets = new Dictionary<string, IImageSet>();
		}

		private static List<IContextMenuActionFactory> CreateActionFactories()
		{
			List<IContextMenuActionFactory> factories = new List<IContextMenuActionFactory>();

			factories.Add(new DisplaySetContextMenuActionFactory());

			try
			{
				foreach (IContextMenuActionFactory factory in new ContextMenuActionFactoryExtensionPoint().CreateExtensions())
					factories.Add(factory);
			}
			catch (NotSupportedException)
			{
				throw;
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Debug, e, "Exception encountered while trying to create context menu action factories.");
			}

			return factories;
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

			base.ImageViewer.EventBroker.StudyLoaded += OnStudyLoaded;
			base.ImageViewer.EventBroker.StudyLoadFailed += OnLoadPriorStudyFailed;
		}

		protected override void Dispose(bool disposing)
		{
			base.ImageViewer.EventBroker.StudyLoadFailed -= OnLoadPriorStudyFailed;
			base.ImageViewer.EventBroker.StudyLoaded -= OnStudyLoaded;

			_imageSetGroups.Dispose();

			foreach (IImageSet imageSet in _unavailableImageSets.Values)
				imageSet.Dispose();

			base.Dispose(disposing);
		}

		private void OnStudyLoaded(object sender, StudyLoadedEventArgs e)
		{
			IImageSet unavailableImageSet;
			string studyInstanceUid = e.Study.StudyInstanceUid;
			if (_unavailableImageSets.TryGetValue(studyInstanceUid, out unavailableImageSet))
			{
				_imageSetGroups.Root.Remove(unavailableImageSet);
				_unavailableImageSets.Remove(studyInstanceUid);
			}
		}

		private void OnLoadPriorStudyFailed(object sender, StudyLoadFailedEventArgs e)
		{
			bool notFoundError = e.Error is NotFoundLoadStudyException;
			if (!notFoundError && (e.Error is LoadSopsException || e.Error is StudyLoaderNotFoundException))
			{
				if (null == CollectionUtils.SelectFirst(base.ImageViewer.LogicalWorkspace.ImageSets,
					delegate(IImageSet imageSet) { return imageSet.Uid == e.Study.StudyInstanceUid; }))
				{
					PatientInformation info = new PatientInformation();
					info.PatientId = e.Study.PatientId;
					PatientInformation reconciled = _patientReconciliationStrategy.ReconcilePatientInformation(info);

					StudyItem studyItem = new StudyItem(e.Study);
					studyItem.PatientId = reconciled.PatientId;

					if (!_unavailableImageSets.ContainsKey(studyItem.StudyInstanceUid))
					{
						ImageSetDescriptor descriptor = new UnavailableImageSetDescriptor(studyItem, e.Error);
						ImageSet unavailableImageSet = new ImageSet(descriptor);

						_imageSetGroups.Root.Add(unavailableImageSet);
						_unavailableImageSets[studyItem.StudyInstanceUid] = unavailableImageSet;
					}
				}
			}
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
			_currentPathElements = new List<string>();
			List<IAction> actions = new List<IAction>();

			FilteredGroup<IImageSet> rootGroup = GetRootGroup(_imageSetGroups.Root);
			if (rootGroup != null)
			{
				ContextMenuActionFactoryArgs args = new ContextMenuActionFactoryArgs
				{
					DesktopWindow = Context.DesktopWindow,
					ImageViewer = Context.Viewer,
					Namespace = GetType().FullName
				};

				bool showImageSetNames = base.ImageViewer.LogicalWorkspace.ImageSets.Count > 1 || _unavailableImageSets.Count > 0;

				foreach (FilteredGroup<IImageSet> group in TraverseImageSetGroups(rootGroup))
				{
					string basePath = StringUtilities.Combine(_currentPathElements, "/");

					//not incredibly efficient, but there really aren't that many items.
					List<IImageSet> orderedItems = new List<IImageSet>(group.Items);
					orderedItems.Sort(_comparer);

					foreach (IImageSet imageSet in orderedItems)
					{
						string imageSetPath;
						if (showImageSetNames)
							imageSetPath = String.Format("{0}/{1}", basePath, imageSet.Name.Replace("/", "-"));
						else
							imageSetPath = basePath;

						foreach (IContextMenuActionFactory factory in _actionFactories)
						{
							args.BasePath = imageSetPath;
							args.ImageSet = imageSet;
							actions.AddRange(factory.CreateActions(args));
						}
					}

					if (group.Items.Count > 0 && base.ImageViewer.PriorStudyLoader.IsActive)
						actions.Add(CreateLoadingPriorsAction(basePath));
				}
			}

			//do this so they all get grouped together.
    		foreach (IAction action in actions)
				action.GroupHint = new GroupHint("DisplaySets");

			return new ActionSet(actions);
		}

		private IEnumerable<FilteredGroup<IImageSet>> TraverseImageSetGroups(FilteredGroup<IImageSet> group)
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
				foreach (FilteredGroup<IImageSet> nonEmptyChild in TraverseImageSetGroups(child))
					yield return nonEmptyChild;
			}

			if (allItems.Count != 0)
				_currentPathElements.RemoveAt(_currentPathElements.Count - 1);
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
			string pathString = String.Format("{0}/loadingPriors", basePath);
			ActionPath path = new ActionPath(pathString, null);
			MenuAction action = new MenuAction(string.Format("{0}:loadingPriors", this.GetType().FullName), path, ClickActionFlags.None, null);
			action.Label = SR.LabelLoadingPriors;
			action.SetClickHandler(delegate { });
			return action;
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
