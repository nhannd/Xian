using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class PresentationLutTool : ImageViewerTool
	{
		//TODO: load/assign keystrokes.

		private class PresentationLutActionContainer
		{
			private PresentationLutTool _ownerTool;
			private MenuAction _action;
			private PresentationLutDescriptor _descriptor;

			public PresentationLutActionContainer(PresentationLutTool ownerTool, PresentationLutDescriptor descriptor)
			{
				_ownerTool = ownerTool;
				_descriptor = descriptor;

				string actionId = String.Format("apply{0}", _descriptor.Name);
				ActionPath actionPath = new ActionPath(String.Format("imageviewer-contextmenu/Presentation Luts/{0}", _descriptor.Name), _ownerTool._resolver);
				_action = new MenuAction(actionId, actionPath, ClickActionFlags.None, _ownerTool._resolver);
				_action.Label = _descriptor.Description;
				_action.SetClickHandler(this.Apply);
			}
			
			public ClickAction Action
			{
				get { return _action; }
			}

			private void Apply()
			{
				PresentationLutOperationApplicator applicator = new PresentationLutOperationApplicator(_ownerTool.SelectedPresentationImage);
				UndoableCommand command = new UndoableCommand(applicator);
				command.BeginState = applicator.CreateMemento();

				ImageOperationApplicator.Apply del =
					delegate(IPresentationImage image)
						{
							if (image is IPresentationLutProvider)
							{
								(image as IPresentationLutProvider).PresentationLutManager.InstallLut(_descriptor);
							}
						};

				applicator.ApplyToAllImages(del);
				command.EndState = applicator.CreateMemento();
				if (!command.EndState.Equals(command.BeginState))
					_ownerTool.Context.Viewer.CommandHistory.AddCommand(command);
			}
		}

		private ActionResourceResolver _resolver;

		public PresentationLutTool()
		{
			_resolver = new ActionResourceResolver(this);
		}

		public override IActionSet Actions
		{
			get
			{
				return new ActionSet(GetActions());
			}
		}

		private IEnumerable<IAction> GetActions()
		{
			if (this.SelectedPresentationImage is IPresentationLutProvider)
			{
				foreach (PresentationLutDescriptor descriptor in (this.SelectedPresentationImage as IPresentationLutProvider).PresentationLutManager.AvailablePresentationLuts)
				{
					PresentationLutActionContainer container = new PresentationLutActionContainer(this, descriptor);
					yield return container.Action;
				}
			}
			else
			{
				yield break;
			}
		}
	}
}
