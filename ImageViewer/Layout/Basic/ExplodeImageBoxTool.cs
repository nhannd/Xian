using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	[ButtonAction("explodeImageBox", "global-toolbars/ToolbarStandard/ToolbarExplodeImageBox", "ToggleExplode", Flags = ClickActionFlags.CheckAction)]
	[MenuAction("explodeImageBox", "global-menus/MenuTools/MenuStandard/MenuExplodeImageBox", "ToggleExplode", Flags = ClickActionFlags.CheckAction)]
	[KeyboardAction("explodeImageBox", "imageviewer-keyboard/ExplodeImageBox", "ToggleExplode", KeyStroke = XKeys.X)]
	[CheckedStateObserver("explodeImageBox", "Checked", "CheckedChanged")]
	[EnabledStateObserver("explodeImageBox", "Enabled", "EnabledChanged")]
	[Tooltip("explodeImageBox", "TooltipExplodeImageBox")]
	[IconSet("explodeImageBox", IconScheme.Colour, "Icons.ExplodeImageBoxToolSmall.png", "Icons.ExplodeImageBoxMedium.png", "Icons.ExplodeImageBoxLarge.png")]
	[GroupHint("explodeImageBox", "Tools.Layout.ImageBox.Explode")]

	[DefaultMouseToolButton(XMouseButtons.Left)]
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ExplodeImageBoxTool : MouseImageViewerTool
	{
		private static readonly Dictionary<IImageViewer, ExplodeImageBoxTool> _tools;
		private ListObserver<IImageBox> _imageBoxesObserver;
		private object _unexplodeMemento;
		private IImageBox _oldImageBox;

		static ExplodeImageBoxTool()
		{
			_tools = new Dictionary<IImageViewer, ExplodeImageBoxTool>();
		}

		public ExplodeImageBoxTool()
		{
			//this tool is activated on a double-click
			base.Behaviour &= ~MouseButtonHandlerBehaviour.CancelStartOnDoubleClick;
		}

		public bool Checked
		{
			get { return _unexplodeMemento != null; }
			set
			{
				if (value == Checked)
					return;

				ToggleExplode();
			}
		}

		public event EventHandler CheckedChanged;

		public override void Initialize()
		{
			base.Initialize();
			_tools[ImageViewer] = this;

			_imageBoxesObserver = new ListObserver<IImageBox>(ImageViewer.PhysicalWorkspace.ImageBoxes, OnImageBoxesChanged);

			UpdateEnabled();
		}

		protected override void Dispose(bool disposing)
		{
			_tools.Remove(ImageViewer);

			_imageBoxesObserver.Dispose();
			
			base.Dispose(disposing);
		}

		protected override void OnTileSelected(object sender, TileSelectedEventArgs e)
		{
			UpdateEnabled();
		}

		protected override void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			UpdateEnabled();
		}

		private void OnImageBoxesChanged()
		{
			CancelExplodeMode();
			UpdateEnabled();
		}

		private void UpdateEnabled()
		{
			IPhysicalWorkspace workspace = base.ImageViewer.PhysicalWorkspace;
			if (Checked)
			{
				base.Enabled = true;
			}
			else
			{
				base.Enabled = workspace.ImageBoxes.Count > 1 && workspace.SelectedImageBox != null &&
				               workspace.SelectedImageBox.SelectedTile != null &&
				               workspace.SelectedImageBox.SelectedTile.PresentationImage != null;
			}
		}

		private void CancelExplodeMode()
		{
			_unexplodeMemento = null;
			_oldImageBox = null;
			OnCheckedChanged();
		}

		private void OnCheckedChanged()
		{
			EventsHelper.Fire(CheckedChanged, this, EventArgs.Empty);
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			//this is a double-click tool.
			if (mouseInformation.ClickCount < 2)
				return false;

			if (!Enabled)
				return false;

			if (Checked)
			{
				UnexplodeImageBox();
				return true;
			}
			else
			{
				return false;
			}	
		}

		private static bool CanExplodeImageBox(IImageBox imageBox)
		{
			if (imageBox == null)
				return false;

			if (imageBox.ParentPhysicalWorkspace == null)
				return false;

			if (imageBox.DisplaySet == null || imageBox.SelectedTile == null || imageBox.SelectedTile.PresentationImage == null)
				return false;

			return true;
		}

		private bool CanUnexplodeImageBox(IImageBox imageBox)
		{
			if (imageBox == null)
				return false;

			IPhysicalWorkspace workspace = imageBox.ParentPhysicalWorkspace;
			if (workspace == null)
				return false;

			if (imageBox.DisplaySet == null || imageBox.SelectedTile == null || imageBox.SelectedTile.PresentationImage == null)
			{
				CancelExplodeMode();
				return false;
			}

			return true;
		}

		private void ExplodeImageBox()
		{
			IImageBox imageBox = ImageViewer.SelectedImageBox;
			if (!CanExplodeImageBox(imageBox))
				return;

			IPhysicalWorkspace workspace = imageBox.ParentPhysicalWorkspace;
			MemorableUndoableCommand memorableCommand = new MemorableUndoableCommand(workspace);
			memorableCommand.BeginState = workspace.CreateMemento();

			_imageBoxesObserver.SuppressChangedEvent = true;

			//set this here so checked will be correct.
			_unexplodeMemento = memorableCommand.BeginState;
			_oldImageBox = imageBox;
			IDisplaySet displaySet = imageBox.DisplaySet;
			IPresentationImage selectedImage = imageBox.SelectedTile.PresentationImage;

			object imageBoxMemento = imageBox.CreateMemento();
			workspace.SetImageBoxGrid(1, 1);
			workspace.ImageBoxes[0].SetMemento(imageBoxMemento);
			workspace.ImageBoxes[0].DisplaySet = displaySet;
			workspace.ImageBoxes[0].TopLeftPresentationImage = selectedImage;

			workspace.SelectDefaultImageBox();
			
			_imageBoxesObserver.SuppressChangedEvent = false;

			workspace.Draw();

			memorableCommand.EndState = workspace.CreateMemento();
			DrawableUndoableCommand historyCommand = new DrawableUndoableCommand(workspace);
			historyCommand.Name = SR.CommandSurveyExplode;
			historyCommand.Enqueue(memorableCommand);
			base.ImageViewer.CommandHistory.AddCommand(historyCommand);

			OnCheckedChanged();
			UpdateEnabled();
		}

		private void UnexplodeImageBox()
		{
			IImageBox imageBox = ImageViewer.SelectedImageBox;
			if (!CanUnexplodeImageBox(imageBox))
				return;

			object imageBoxMemento = imageBox.CreateMemento();

			IPhysicalWorkspace workspace = imageBox.ParentPhysicalWorkspace;
			MemorableUndoableCommand memorableCommand = new MemorableUndoableCommand(workspace);
			memorableCommand.BeginState = workspace.CreateMemento();

			IImageBox oldImageBox = _oldImageBox;

			workspace.SetMemento(_unexplodeMemento);

			foreach (IImageBox box in workspace.ImageBoxes)
			{
				//Keep the state of the image box the same.
				if (box == oldImageBox)
				{
					box.SetMemento(imageBoxMemento);
					break;
				}
				
			}

			workspace.Draw();

			memorableCommand.EndState = workspace.CreateMemento();

			DrawableUndoableCommand historyCommand = new DrawableUndoableCommand(workspace);
			historyCommand.Name = SR.CommandSurveyExplode;
			historyCommand.Enqueue(memorableCommand);
			base.ImageViewer.CommandHistory.AddCommand(historyCommand);

			CancelExplodeMode();
			OnCheckedChanged();
			UpdateEnabled();
		}

		public void ToggleExplode()
		{
			if (!Enabled)
				return;

			if (Checked)
				UnexplodeImageBox();
			else
				ExplodeImageBox();
		}

		internal static bool IsExploded(IImageViewer viewer)
		{
			if (_tools.ContainsKey(viewer))
				return _tools[viewer].Checked;

			return false;
		}
	}
}
