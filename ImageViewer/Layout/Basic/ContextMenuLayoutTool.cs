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
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop;
using System.Collections.Generic;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Layout.Basic
{

    /// <summary>
    /// This tool runs an instance of <see cref="LayoutComponent"/> in a shelf, and coordinates
    /// it so that it reflects the state of the active workspace.
	/// </summary>
	[ClearCanvas.Common.ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ContextMenuLayoutTool : ImageViewerTool
	{
		/// <summary>
        /// Constructor
        /// </summary>
		public ContextMenuLayoutTool()
		{
        }

		public override IActionSet Actions
		{
			get
			{
				return GetDisplaySetActions();
			}
		}
		
		/// <summary>
        /// Overridden to subscribe to workspace activation events
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

		/// <summary>
		/// Gets an array of <see cref="IAction"/> objects that allow selection of specific display
		/// sets for display in the currently selected image box.
		/// </summary>
		/// <returns></returns>
		private IActionSet GetDisplaySetActions()
		{
			List<IAction> actions = new List<IAction>();
			int i = 0;

			foreach (IImageSet imageSet in this.ImageViewer.LogicalWorkspace.ImageSets)
			{
				foreach (IDisplaySet displaySet in imageSet.DisplaySets)
				{
					actions.Add(
						CreateDisplaySetAction(
							this.ImageViewer.LogicalWorkspace,
							imageSet, 
							displaySet, 
							++i));
				}
			}

			return new ActionSet(actions);
		}

		/// <summary>
		/// Creates an <see cref="IClickAction"/> that displays the specified display set when clicked.  The index
		/// parameter is used to generate a label for the action.
		/// </summary>
		/// <param name="displaySet"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		private IClickAction CreateDisplaySetAction(
			ILogicalWorkspace logicalWorkspace,
			IImageSet imageSet, 
			IDisplaySet displaySet, 
			int index)
		{
			string pathString;

			if (logicalWorkspace.ImageSets.Count == 1)
			{
				pathString = string.Format("imageviewer-contextmenu/display{0}", index);
			}
			else
			{
				//string imageSetName = imageSet.Name.Replace("/", "\\");
				string imageSetName = imageSet.Name.Replace("/", "-");

				if (IsMoreThanOnePatient(logicalWorkspace.ImageSets))
					pathString = string.Format("imageviewer-contextmenu/{0}/{1}/display{2}", imageSet.PatientInfo, imageSetName, index);
				else
					pathString = string.Format("imageviewer-contextmenu/{0}/display{1}", imageSetName, index);
			}

			ActionPath path = new ActionPath(pathString, null);
			MenuAction action = new MenuAction(string.Format("{0}:display{1}", this.GetType().FullName, index), path, ClickActionFlags.CheckParents, null);
			action.GroupHint = new GroupHint("DisplaySets");
			action.Label = displaySet.Name;
			action.SetClickHandler(
				delegate()
				{
					AssignDisplaySetToImageBox(displaySet);
				}
			);

			action.Checked = this.ImageViewer.SelectedImageBox != null &&
				this.ImageViewer.SelectedImageBox.DisplaySet != null && 
				this.ImageViewer.SelectedImageBox.DisplaySet.Uid == displaySet.Uid;

			return action;
		}

		private bool IsMoreThanOnePatient(ImageSetCollection imageSetCollection)
		{
			string patientInfo = String.Empty;
			int numPatients = 0;

			foreach (IImageSet imageSet in imageSetCollection)
			{
				if (imageSet.PatientInfo != patientInfo)
				{
					patientInfo = imageSet.PatientInfo;
					numPatients++;
				}
			}

			return numPatients > 1;
		}

		private void AssignDisplaySetToImageBox(IDisplaySet displaySet)
		{
			MemorableUndoableCommand command = new MemorableUndoableCommand(this.ImageViewer.SelectedImageBox);
			command.BeginState = this.ImageViewer.SelectedImageBox.CreateMemento();

			// always create a 'fresh copy' to show in the image box.  We never want to show
			// the 'originals' (e.g. the ones in IImageSet.DisplaySets) because we want them 
			// to remain clean and unaltered - consider them to be templates for what actually
			// gets shown.
			this.ImageViewer.SelectedImageBox.DisplaySet = displaySet.CreateFreshCopy();

			this.ImageViewer.SelectedImageBox.Draw();
			//this.ImageViewer.SelectedImageBox[0, 0].Select();

			command.EndState = this.ImageViewer.SelectedImageBox.CreateMemento();

			this.ImageViewer.CommandHistory.AddCommand(command);
		}
	}
}
