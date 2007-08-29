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
	public class VoiLutTool : ImageViewerTool
	{
		private class VoiLutActionContainer
		{
			private VoiLutTool _ownerTool;
			private MenuAction _action;
			private IVoiLutFactory _factory;

			public VoiLutActionContainer(VoiLutTool ownerTool, IVoiLutFactory factory)
			{
				_ownerTool = ownerTool;
				_factory = factory;

				string actionId = String.Format("apply{0}", _factory.Name);
				ActionPath actionPath = new ActionPath(String.Format("imageviewer-contextmenu/Voi Luts/{0}", _factory.Name), _ownerTool._resolver);
				_action = new MenuAction(actionId, actionPath, ClickActionFlags.None, _ownerTool._resolver);
				_action.Label = _factory.Description;
				_action.SetClickHandler(this.Apply);
			}

			public ClickAction Action
			{
				get { return _action; }
			}

			private void Apply()
			{
				VoiLutOperationApplicator applicator = new VoiLutOperationApplicator(_ownerTool.SelectedPresentationImage);
				UndoableCommand command = new UndoableCommand(applicator);
				command.BeginState = applicator.CreateMemento();

				ImageOperationApplicator.Apply del =
					delegate(IPresentationImage image)
					{
						//TODO: Install the Lut
					};

				applicator.ApplyToAllImages(del);
				command.EndState = applicator.CreateMemento();
				if (!command.EndState.Equals(command.BeginState))
					_ownerTool.Context.Viewer.CommandHistory.AddCommand(command);
			}
		}
		
		private ActionResourceResolver _resolver;
		private List<IVoiLutFactory> _factories;

		public VoiLutTool()
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

		private List<IVoiLutFactory> Factories
		{
			get
			{
				if (_factories == null)
				{
					_factories = new List<IVoiLutFactory>();

					foreach (object factory in new VoiLutFactoryExtensionPoint().CreateExtensions())
					{
						if (factory is IVoiLutFactory)
							_factories.Add((IVoiLutFactory)factory);
						else
							Platform.Log(
								LogLevel.Warn,
								String.Format("The Voi Lut Factory extension {0} does not implement {1}", factory.GetType().FullName,
							    typeof (IVoiLutFactory).Name));
					}
				}

				return _factories;
			}
		}

		private IEnumerable<IAction> GetActions()
		{
			yield break;
		}
	}
}
