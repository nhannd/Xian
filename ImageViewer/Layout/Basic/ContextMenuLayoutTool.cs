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
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.Common.Utilities;
using System.Diagnostics;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Comparers;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	internal class UnavailableImageSet : ImageSet
	{
		public UnavailableImageSet(StudyItem studyItem, Exception error)
		{
			StudyItem = studyItem;
			Error = error;
		}

		public readonly Exception Error;
		public readonly StudyItem StudyItem;

		internal string GetActionMessage()
		{
			if (Error is OfflineLoadStudyException)
			{
				return SR.MessageActionStudyOffline;
			}
			else if (Error is NearlineLoadStudyException)
			{
				return SR.MessageActionStudyNearline;
			}
			else if (Error is InUseLoadStudyException)
			{
				return SR.MessageActionStudyInUse;
			}
			else if (Error is StudyLoaderNotFoundException)
			{
				return SR.MessageActionNoStudyLoader;
			}
			else
			{
				return SR.MessageActionStudyCouldNotBeLoaded;
			}
		}

		internal string GetNamePrefix()
		{
			string serverName = "Unknown";
			if (StudyItem.Server != null)
				serverName = StudyItem.Server.ToString();

			return serverName;
		}

		internal string GetActionLabel()
		{
			if (Error is OfflineLoadStudyException)
			{
				return String.Format(SR.LabelFormatStudyUnavailable, SR.Offline);
			}
			else if (Error is NearlineLoadStudyException)
			{
				return String.Format(SR.LabelFormatStudyUnavailable, SR.Nearline);
			}
			else if (Error is InUseLoadStudyException)
			{
				return String.Format(SR.LabelFormatStudyUnavailable, SR.InUse);
			}
			else if (Error is StudyLoaderNotFoundException)
			{
				return String.Format(SR.LabelFormatStudyUnavailable, SR.Unavailable);
			}
			else
			{
				return SR.LabelStudyCouldNotBeLoaded;
			}
		}
	}

	internal class StudyDateComparer : ImageSetComparer
	{
		public StudyDateComparer()
			: base(true)
		{
		}

		private static DateTime? GetStudyDate(IImageSet imageSet)
		{
			if (imageSet is UnavailableImageSet)
			{
				return DateParser.Parse(((UnavailableImageSet) imageSet).StudyItem.StudyDate);
			}
			else
			{
				if (imageSet.DisplaySets.Count > 0)
				{
					IDisplaySet displaySet = imageSet.DisplaySets[0];
					if (displaySet.PresentationImages.Count > 0)
					{
						IPresentationImage image = displaySet.PresentationImages[0];
						if (image is IImageSopProvider)
							return DateParser.Parse(((IImageSopProvider) image).ImageSop.StudyDate);
					}
				}
			}

			return null;
		}

		private static DateTime? GetStudyTime(IImageSet imageSet)
		{
			if (imageSet is UnavailableImageSet)
			{
				return TimeParser.Parse(((UnavailableImageSet)imageSet).StudyItem.StudyTime);
			}
			else
			{
				if (imageSet.DisplaySets.Count > 0)
				{
					IDisplaySet displaySet = imageSet.DisplaySets[0];
					if (displaySet.PresentationImages.Count > 0)
					{
						IPresentationImage image = displaySet.PresentationImages[0];
						if (image is IImageSopProvider)
							return TimeParser.Parse(((IImageSopProvider)image).ImageSop.StudyTime);
					}
				}
			}

			return null;
		}

		private static IEnumerable<IComparable> GetCompareValues(IImageSet imageSet)
		{
			yield return GetStudyDate(imageSet);
			yield return GetStudyTime(imageSet);
		}

		public override int Compare(IImageSet x, IImageSet y)
		{
			return base.Compare(GetCompareValues(x), GetCompareValues(y));
		}
	}

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
    	private bool _anyUnavailable = false;
		private readonly IComparer<IImageSet> _comparer = new StudyDateComparer();
		private readonly DefaultPatientReconciliationStrategy _patientReconciliationStrategy = new DefaultPatientReconciliationStrategy();

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
			base.ImageViewer.EventBroker.StudyLoadFailed += OnLoadPriorStudyFailed;
			_imageSetGroups = new ImageSetGroups(base.Context.Viewer.LogicalWorkspace.ImageSets);
		}

		protected override void Dispose(bool disposing)
		{
			base.ImageViewer.EventBroker.StudyLoadFailed -= OnLoadPriorStudyFailed;
			_imageSetGroups.Dispose();
			base.Dispose(disposing);
		}

		private void OnLoadPriorStudyFailed(object sender, StudyLoadFailedEventArgs e)
		{
			_anyUnavailable = true;

			bool notFoundError = e.Error is NotFoundLoadStudyException;
			if (!notFoundError && (e.Error is LoadSopsException || e.Error is StudyLoaderNotFoundException))
			{
				if (null == CollectionUtils.SelectFirst(base.ImageViewer.LogicalWorkspace.ImageSets,
					delegate(IImageSet imageSet) { return imageSet.Uid == e.Study.StudyInstanceUid; }))
				{
					PatientInformation info = new PatientInformation();
					info.PatientId = e.Study.PatientId;
					PatientInformation reconciled = _patientReconciliationStrategy.ReconcilePatientInformation(info);

					UnavailableImageSet imageSet = CreateUnavailableImageSet(e.Study, e.Error);
					imageSet.PatientInfo = String.Format("{0} · {1}",
						e.Study.PatientsName.FormattedName,
						reconciled.PatientId);

					_imageSetGroups.Root.Add(imageSet);
				}
			}
		}

		private static UnavailableImageSet CreateUnavailableImageSet(StudyItem study, Exception error)
		{
			//TODO: none of this is ideal because this code is duplicated from the layout manager,
			//but unfortunately it's necessary right now.  I think the problem is 3-fold:
			// 1. We are showing the values from IImageSet.Name; the context menu tool is the 
			//    presenter and should be formatting the info itself.
			// 2. There is no unified concept of a 'study' in the viewer.  In fact, there are
			//    several study objects, all of which serve a similar purpose.
			// 3. ImageSets are not directly tied to studies; this is less of an issue because
			//    we know that the layout manager sets the Uid to the study uid and we can
			//    safely make that assumption.
			// The ideal solution would be to create a ContextMenuGroups class, similar to
			// the ImageSetGroups, which keeps track of Studies (both available and unavailable)
			// using some unified Study class/interface as it's data source.  The context
			// menu actions would then be constructed from that information, not relying on
			// image set names.

			UnavailableImageSet imageSet = new UnavailableImageSet(study, error);

			DateTime studyDate;
			DateParser.Parse(study.StudyDate, out studyDate);
			DateTime studyTime;
			TimeParser.Parse(study.StudyTime, out studyTime);

			string modalitiesInStudy = StringUtilities.Combine(study.ModalitiesInStudy, ",");

			imageSet.Name = String.Format("{0} {1} [{2}] {3}",
				studyDate.ToString(Format.DateFormat),
				studyTime.ToString(Format.TimeFormat),
				modalitiesInStudy ?? "",
				study.StudyDescription);

			imageSet.PatientInfo = String.Format("{0} · {1}",
				study.PatientsName.FormattedName,
				study.PatientId);

			imageSet.Uid = study.StudyInstanceUid;
			return imageSet;
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
				_showImageSetNames = base.ImageViewer.LogicalWorkspace.ImageSets.Count > 1 || _anyUnavailable;
				
				foreach (FilteredGroup<IImageSet> group in TraverseImageSetGroups(rootGroup))
				{
					string basePath = StringUtilities.Combine(_currentPathElements, "/");

					//not incredibly efficient, but there really aren't that many items.
					List<IImageSet> orderedItems = new List<IImageSet>(group.Items);
					orderedItems.Sort(_comparer);

					foreach (IImageSet imageSet in orderedItems)
					{
						string imageSetPath;
						if (_showImageSetNames)
						{
							if (imageSet is UnavailableImageSet)
							{
								UnavailableImageSet unavailable = (UnavailableImageSet) imageSet;
								imageSetPath = String.Format("{0}/({1}) {2}", basePath,
									unavailable.GetNamePrefix(), imageSet.Name.Replace("/", "-"));
							}
							else
							{
								imageSetPath = String.Format("{0}/{1}", basePath, imageSet.Name.Replace("/", "-"));
							}
						}
						else
							imageSetPath = basePath;

						if (imageSet is UnavailableImageSet)
						{
							actions.Add(CreateUnavailableStudyAction(imageSetPath, (UnavailableImageSet)imageSet));
							++_actionNumber;
						}
						else
						{
							foreach (IDisplaySet displaySet in imageSet.DisplaySets)
							{
								actions.Add(CreateDisplaySetAction(imageSetPath, displaySet));
								++_actionNumber;
							}
						}
					}

					if (group.Items.Count > 0 && base.ImageViewer.PriorStudyLoader.IsActive)
					{
						actions.Add(CreateLoadingPriorsAction(basePath));
						++_actionNumber;
					}
				}
			}
			
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

		private IClickAction CreateUnavailableStudyAction(string basePath, UnavailableImageSet imageSet)
		{
			string pathString = String.Format("{0}/display{1}", basePath, _actionNumber);
			//Trace.WriteLine(String.Format("Path: {0}", pathString));

			ActionPath path = new ActionPath(pathString, null);
			MenuAction action = new MenuAction(string.Format("{0}:display{1}", this.GetType().FullName, _actionNumber), path, ClickActionFlags.CheckParents, null);
			action.GroupHint = new GroupHint("DisplaySets");
			action.Label = imageSet.GetActionLabel();
			action.SetClickHandler(delegate
			                       	{
			                       		this.Context.DesktopWindow.ShowMessageBox(imageSet.GetActionMessage(),
			                       		                                          MessageBoxActions.Ok);
			                       	});

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
			//Trace.WriteLine(String.Format("Path: {0}", pathString));

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
