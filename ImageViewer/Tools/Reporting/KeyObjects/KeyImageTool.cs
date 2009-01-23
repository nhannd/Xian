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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.BaseTools;
using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyObjects
{
	[MenuAction("create", "imageviewer-contextmenu/MenuCreateKeyImage", "Create")]
	[ButtonAction("create", "global-toolbars/ToolbarKeyImages/ToolbarCreateKeyImage", "Create")]
	[KeyboardAction("create", "imageviewer-keyboard/CreateKeyImage", "Create", KeyStroke = XKeys.Space)]
	[Tooltip("create", "TooltipCreateKeyImage")]
	[IconSet("create", IconScheme.Colour, "Icons.CreateKeyImageToolSmall.png", "Icons.CreateKeyImageToolMedium.png", "Icons.CreateKeyImageToolLarge.png")]
	[EnabledStateObserver("create", "CreateEnabled", "CreateEnabledChanged")]
	[VisibleStateObserver("create", "CreateVisible", "CreateVisibleChanged")]

	[MenuAction("delete", "imageviewer-contextmenu/MenuDeleteKeyImage", "Delete")]
	[ButtonAction("delete", "global-toolbars/ToolbarKeyImages/ToolbarDeleteKeyImage", "Delete")]
	[Tooltip("delete", "TooltipDeleteKeyImage")]
	[IconSet("delete", IconScheme.Colour, "Icons.DeleteKeyImageToolSmall.png", "Icons.DeleteKeyImageToolMedium.png", "Icons.DeleteKeyImageToolLarge.png")]
	[EnabledStateObserver("delete", "DeleteEnabled", "DeleteEnabledChanged")]
	[VisibleStateObserver("delete", "DeleteVisible", "DeleteVisibleChanged")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class KeyImageTool : ImageViewerTool
	{
		#region Private Fields

		private bool _createEnabled;
		private bool _deleteEnabled;

		private bool _createVisible;
		private bool _deleteVisible;

		private event EventHandler _createEnabledChanged;
		private event EventHandler _deleteEnabledChanged;

		private event EventHandler _createVisibleChanged;
		private event EventHandler _deleteVisibleChanged;

		private Dictionary<string, DisplaySet> _keyImageDisplaySets;

		#endregion

		public KeyImageTool()
		{
			_createEnabled = false;
			_deleteEnabled = false;
			_createVisible = false;
			_deleteVisible = false;

			_keyImageDisplaySets = new Dictionary<string, DisplaySet>();
		}

		#region Properties

		public bool CreateEnabled
		{
			get { return _createEnabled; }
			set
			{
				if (value != _createEnabled)
				{
					_createEnabled = value;
					EventsHelper.Fire(_createEnabledChanged, this, EventArgs.Empty);
				}
			}
		}

		public bool DeleteEnabled
		{
			get { return _deleteEnabled; }
			set
			{
				if (value != _deleteEnabled)
				{
					_deleteEnabled = value;
					EventsHelper.Fire(_deleteEnabledChanged, this, EventArgs.Empty);
				}
			}
		}

		public bool CreateVisible
		{
			get { return _createVisible; }
			set
			{
				if (value != _createVisible)
				{
					_createVisible = value;
					EventsHelper.Fire(_createVisibleChanged, this, EventArgs.Empty);
				}
			}
		}

		public bool DeleteVisible
		{
			get { return _deleteVisible; }
			set
			{
				if (value != _deleteVisible)
				{
					_deleteVisible = value;
					EventsHelper.Fire(_deleteVisibleChanged, this, EventArgs.Empty);
				}
			}
		}

		#endregion

		#region Events

		public event EventHandler CreateEnabledChanged
		{
			add { _createEnabledChanged += value; }
			remove { _createEnabledChanged -= value; }
		}

		public event EventHandler DeleteEnabledChanged
		{
			add { _deleteEnabledChanged += value; }
			remove { _deleteEnabledChanged -= value; }
		}

		public event EventHandler CreateVisibleChanged
		{
			add { _createEnabledChanged += value; }
			remove { _createEnabledChanged -= value; }
		}

		public event EventHandler DeleteVisibleChanged
		{
			add { _deleteEnabledChanged += value; }
			remove { _deleteEnabledChanged -= value; }
		}

		#endregion

		#region Methods

		public override void Initialize()
		{
			base.Initialize();
		}

		protected override void Dispose(bool disposing)
		{
			//Before disposing, publish the key images to the default servers.
			base.Dispose(disposing);
		}

		protected override void OnTileSelected(object sender, TileSelectedEventArgs e)
		{
			base.OnTileSelected(sender, e);

			if (e.SelectedTile.PresentationImage == null)
			{
				DeleteVisible = false;
				DeleteEnabled = false;
				CreateEnabled = false;
				CreateVisible = false;
			}
			else
			{
				IDisplaySet displaySet = e.SelectedTile.PresentationImage.ParentDisplaySet;
				if (displaySet is DisplaySet)
				{
					DeleteEnabled = true;
					DeleteVisible = true;
					CreateEnabled = false;
					CreateVisible = false;
				}
				else
				{
					//TODO: determine if ImageSop is 'real' or generated.
					if (e.SelectedTile.PresentationImage is IImageSopProvider)
					{
						DeleteEnabled = false;
						DeleteVisible = false;
						CreateEnabled = true;
						CreateVisible = true;
					}
					else
					{
						DeleteEnabled = false;
						DeleteVisible = false;
						CreateEnabled = false;
						CreateVisible = true;
					}
				}
			}
		}

		protected override void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			base.OnPresentationImageSelected(sender, e);
		}

		public void Create()
		{
			if (base.SelectedPresentationImage == null)
				return;

			IImageSopProvider sopProvider = base.SelectedPresentationImage as IImageSopProvider;
			if (sopProvider == null)
				return;

			DisplaySet displaySet;
			if (_keyImageDisplaySets.ContainsKey(sopProvider.ImageSop.StudyInstanceUID))
			{
				displaySet = _keyImageDisplaySets[sopProvider.ImageSop.StudyInstanceUID];
			}
			else
			{
				//display set placement?
				string name = SR.LabelKeyImageCurrentDisplaySet;
				DicomUid uid = DicomUid.GenerateUid();
				displaySet = new DisplaySet(name, uid.ToString());
				_keyImageDisplaySets[sopProvider.ImageSop.StudyInstanceUID] = displaySet;
				base.SelectedPresentationImage.ParentDisplaySet.ParentImageSet.DisplaySets.Add(displaySet);
			}

			//Need a KeyPresentationImage?
			displaySet.PresentationImages.Add(base.SelectedPresentationImage.Clone());
		}

		public void Delete()
		{
			if (base.SelectedPresentationImage == null)
				return;

			if (base.SelectedPresentationImage.ParentDisplaySet is DisplaySet)
			{
				IDisplaySet displaySet = base.SelectedPresentationImage.ParentDisplaySet;
				displaySet.PresentationImages.Remove(base.SelectedPresentationImage);

				IImageBox imageBox = displaySet.ImageBox;

				if (displaySet.PresentationImages.Count == 0)
				{
					imageBox.DisplaySet = null;
					displaySet.ParentImageSet.DisplaySets.Remove(displaySet);
				}

				imageBox.Draw();
			}
		}

		#endregion
	}
}
