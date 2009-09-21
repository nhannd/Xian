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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("undo", "global-menus/MenuEdit/MenuUndo", "Undo", KeyStroke = XKeys.Control | XKeys.Z)]
	[ButtonAction("undo", "global-toolbars/ToolbarStandard/ToolbarUndo", "Undo", KeyStroke = XKeys.Control | XKeys.Z)]
	[EnabledStateObserver("undo", "UndoEnabled", "UndoEnabledChanged")]
	[IconSet("undo", IconScheme.Colour, "Icons.UndoToolSmall.png", "Icons.UndoToolMedium.png", "Icons.UndoToolLarge.png")]
	[Tooltip("undo", "TooltipUndo")]
	[GroupHint("undo", "Application.Edit.Undo")]

	[MenuAction("redo", "global-menus/MenuEdit/MenuRedo", "Redo", KeyStroke = XKeys.Control | XKeys.Y)]
	[ButtonAction("redo", "global-toolbars/ToolbarStandard/ToolbarRedo", "Redo", KeyStroke = XKeys.Control | XKeys.Y)]
	[EnabledStateObserver("redo", "RedoEnabled", "RedoEnabledChanged")]
	[IconSet("redo", IconScheme.Colour, "Icons.RedoToolSmall.png", "Icons.RedoToolMedium.png", "Icons.RedoToolLarge.png")]
	[Tooltip("redo", "TooltipRedo")]
	[GroupHint("redo", "Application.Edit.Redo")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class EditHistoryTool : ImageViewerTool
	{
		private bool _undoEnabled;
		private event EventHandler _undoEnabledChangedEvent;

		private bool _redoEnabled;
		private event EventHandler _redoEnabledChangedEvent;

		/// <summary>
		/// Constructor
		/// </summary>
		public EditHistoryTool()
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			this.ImageViewer.CommandHistory.CurrentCommandChanged += new EventHandler(OnCurrentCommandChanged);
		}

		public bool UndoEnabled
		{
			get { return _undoEnabled; }
			private set
			{
				if (value != _undoEnabled)
				{
					_undoEnabled = value;
					EventsHelper.Fire(_undoEnabledChangedEvent, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler UndoEnabledChanged
		{
			add { _undoEnabledChangedEvent += value; }
			remove { _undoEnabledChangedEvent -= value; }
		}

		public bool RedoEnabled
		{
			get { return _redoEnabled; }
			private set
			{
				if (value != _redoEnabled)
				{
					_redoEnabled = value;
					EventsHelper.Fire(_redoEnabledChangedEvent, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler RedoEnabledChanged
		{
			add { _redoEnabledChangedEvent += value; }
			remove { _redoEnabledChangedEvent -= value; }
		}

		public void Undo()
		{
			this.ImageViewer.CommandHistory.Undo();
		}

		public void Redo()
		{
			this.ImageViewer.CommandHistory.Redo();
		}

		void OnCurrentCommandChanged(object sender, EventArgs e)
		{
			this.UndoEnabled = this.ImageViewer.CommandHistory.CurrentCommandIndex > -1;
			this.RedoEnabled = this.ImageViewer.CommandHistory.CurrentCommandIndex < this.ImageViewer.CommandHistory.LastCommandIndex;
		}
	}
}