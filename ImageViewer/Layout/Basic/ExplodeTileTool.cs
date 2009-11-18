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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.InputManagement;
using System;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	[DefaultMouseToolButton(XMouseButtons.Left)]
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ExplodeTileTool : MouseImageViewerTool
	{
		private delegate void NotifyRemoveUnexplodedTileCommand(IImageBox imageBox);

		private readonly Dictionary<IImageBox, UnexplodeTileCommand> _unexplodeCommands;
		private ListObserver<IImageBox> _imageBoxesObserver;

		#region UnexplodeTileCommand

		private class UnexplodeTileCommand : Command, IDisposable
		{
			private readonly IImageBox _imageBox;
			private readonly ListObserver<ITile> _listObserver;
			private readonly NotifyRemoveUnexplodedTileCommand _remove;
			private object _unexplodeMemento;

			public UnexplodeTileCommand(IImageBox imageBox, object unexplodeMemento, NotifyRemoveUnexplodedTileCommand remove)
			{
				_imageBox = imageBox;
				_unexplodeMemento = unexplodeMemento;
				_remove = remove;
				_listObserver = new ListObserver<ITile>(imageBox.Tiles, OnTilesChanged);
			}

			private void OnTilesChanged()
			{
				Dispose();
			}

			public override void Execute()
			{
 				if (_unexplodeMemento == null)
					return;

				IImageBox imageBox = _imageBox;
				object unexplodeMemento = _unexplodeMemento;
				Dispose();

				Execute(imageBox, unexplodeMemento);
			}

			private static void Execute(IImageBox imageBox, object unexplodeMemento)
			{
				MemorableUndoableCommand memorableCommand = new MemorableUndoableCommand(imageBox);
				memorableCommand.BeginState = imageBox.CreateMemento();

				IDisplaySet displaySet = imageBox.DisplaySet;
				IPresentationImage selectedImage;
				if (imageBox.SelectedTile != null)
					selectedImage = imageBox.SelectedTile.PresentationImage;
				else
					selectedImage = imageBox.TopLeftPresentationImage;

				imageBox.SetMemento(unexplodeMemento);

				//TODO (architecture): this wouldn't be necessary if we had a SetImageBoxGrid(imageBox[,]).
				//This stuff with mementos is actually a hacky workaround.

				bool locked = imageBox.DisplaySetLocked;
				imageBox.DisplaySetLocked = false;
				imageBox.DisplaySet = displaySet;
				imageBox.DisplaySetLocked = locked;

				if (selectedImage != null)
					imageBox.TopLeftPresentationImage = selectedImage;

				imageBox.SelectDefaultTile();
				imageBox.Draw();

				memorableCommand.EndState = imageBox.CreateMemento();

				DrawableUndoableCommand historyCommand = new DrawableUndoableCommand(imageBox);
				historyCommand.Name = SR.CommandExplodeTile;
				historyCommand.Enqueue(memorableCommand);
				imageBox.ParentPhysicalWorkspace.ImageViewer.CommandHistory.AddCommand(historyCommand);
			}
		
			#region IDisposable Members

			public void  Dispose()
			{
				if (_unexplodeMemento != null)
				{
					_unexplodeMemento = null;
					_listObserver.Dispose();
					_remove(_imageBox);
				}
			}

			#endregion
		}

		#endregion
		public ExplodeTileTool()
		{
			//this tool is activated on a double-click
			base.Behaviour &= ~MouseButtonHandlerBehaviour.CancelStartOnDoubleClick;
			_unexplodeCommands = new Dictionary<IImageBox, UnexplodeTileCommand>();
		}

		public override void Initialize()
		{
			base.Initialize();
			_imageBoxesObserver = new ListObserver<IImageBox>(ImageViewer.PhysicalWorkspace.ImageBoxes, OnImageBoxesChanged);
			_imageBoxesObserver.SuppressChangedEvent = true;
		}

		protected override void Dispose(bool disposing)
		{
			_imageBoxesObserver.Dispose();
			DisposeUnexplodeCommands();

			base.Dispose(disposing);
		}

		private static bool CanExplodeTiles(IImageBox imageBox)
		{
			return imageBox.Tiles.Count > 1 && !imageBox.ParentPhysicalWorkspace.Locked;
		}

		private void DisposeUnexplodeCommands()
		{
			foreach (UnexplodeTileCommand command in _unexplodeCommands.Values)
				command.Dispose();

			_unexplodeCommands.Clear();
		}

		private void RemoveUnexplodeCommand(IImageBox imageBox)
		{
			if (_unexplodeCommands.ContainsKey(imageBox))
			{
				_unexplodeCommands[imageBox].Dispose();
				_unexplodeCommands.Remove(imageBox);
			}
		}

		private void OnImageBoxesChanged()
		{
			DisposeUnexplodeCommands();
		}

		private void ExplodeSelectedTile(IImageBox imageBox)
		{
			MemorableUndoableCommand memorableCommand = new MemorableUndoableCommand(imageBox);
			memorableCommand.BeginState = imageBox.CreateMemento();

			//set this here so checked will be correct.
			object unexplodeMemento = memorableCommand.BeginState;
			_unexplodeCommands[imageBox] = new UnexplodeTileCommand(imageBox, unexplodeMemento, RemoveUnexplodeCommand);
			
			IDisplaySet displaySet = imageBox.DisplaySet;
			IPresentationImage selectedImage = imageBox.SelectedTile.PresentationImage;
			imageBox.SetTileGrid(1, 1);

			//TODO (architecture): this wouldn't be necessary if we had a SetImageBoxGrid(imageBox[,]).
			//This stuff with mementos is actually a hacky workaround.

			bool locked = imageBox.DisplaySetLocked;
			imageBox.DisplaySetLocked = false;
			imageBox.DisplaySet = displaySet;
			imageBox.DisplaySetLocked = locked;

			imageBox.TopLeftPresentationImage = selectedImage;
			imageBox.SelectDefaultTile();

			memorableCommand.EndState = imageBox.CreateMemento();

			DrawableUndoableCommand historyCommand = new DrawableUndoableCommand(imageBox);
			historyCommand.Name = SR.CommandExplodeTile;
			historyCommand.Enqueue(memorableCommand);
			imageBox.ParentPhysicalWorkspace.ImageViewer.CommandHistory.AddCommand(historyCommand);
		
			imageBox.Draw();
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			if (mouseInformation.ClickCount < 2)
				return false;

			IPhysicalWorkspace workspace = base.ImageViewer.PhysicalWorkspace;
			if (workspace == null)
				return false;

			IImageBox imageBox = workspace.SelectedImageBox;
			if (imageBox == null)
				return false;

			if (imageBox.SelectedTile == null || imageBox.SelectedTile.PresentationImage == null)
				return false;

			// This tool could just be lumped into the explode image box tool, but I like the separation.
			if (ExplodeImageBoxTool.IsExploded(base.ImageViewer))
				return false;

			if (_unexplodeCommands.ContainsKey(imageBox))
			{
				_unexplodeCommands[imageBox].Execute();
				return true;
			}
			else if (CanExplodeTiles(imageBox))
			{
				ExplodeSelectedTile(imageBox);
				return true;
			}

			return false;
		}
	}
}
