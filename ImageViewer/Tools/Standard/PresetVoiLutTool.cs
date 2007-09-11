using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts;
using ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Applicators;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class PresetVoiLutTool : ImageViewerTool
	{
		private class PresetVoiLutActionContainer
		{
			private readonly PresetVoiLutTool _ownerTool;
			private readonly PresetVoiLut _preset;
			private readonly MenuAction _action;

			public PresetVoiLutActionContainer(PresetVoiLutTool ownerTool, PresetVoiLut preset, int index)
			{
				_ownerTool = ownerTool;
				_preset = preset;

				string actionId = String.Format("apply{0}", _preset.Applicator.Name);
				ActionPath actionPath = new ActionPath(String.Format("imageviewer-contextmenu/PresetVoiLuts/presetLut{0}", index), _ownerTool._resolver);
				_action = new MenuAction(actionId, actionPath, ClickActionFlags.None, _ownerTool._resolver);
				_action.GroupHint = new GroupHint("Tools.Image.Manipulation.Lut.VoiLuts");
				_action.Label = _preset.Applicator.Name;
				_action.KeyStroke = _preset.KeyStroke;
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
						if (_preset.Applicator.AppliesTo(image))
							_preset.Applicator.Apply(image);
					};

				applicator.ApplyToAllImages(del);
				command.EndState = applicator.CreateMemento();
				if (!command.EndState.Equals(command.BeginState))
					_ownerTool.Context.Viewer.CommandHistory.AddCommand(command);
			}
		}

		private readonly ActionResourceResolver _resolver;

		public PresetVoiLutTool()
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
			if (!(this.SelectedPresentationImage is IImageSopProvider))
				yield break;

			List<PresetVoiLut> presets = new List<PresetVoiLut>();

			//Only temporary until we enable the full functionality in the presets.
			PresetVoiLut autoPreset = new PresetVoiLut(new AutoPresetVoiLutApplicatorComponent());
			autoPreset.KeyStroke = XKeys.F2;
			presets.Add(autoPreset);
			
			ImageSop sop = ((IImageSopProvider) this.SelectedPresentationImage).ImageSop;

			PresetVoiLutGroupCollection groups = PresetVoiLutSettings.Default.GetPresetGroups();
			PresetVoiLutGroup group = CollectionUtils.SelectFirst<PresetVoiLutGroup>(groups, delegate(PresetVoiLutGroup testGroup) { return testGroup.AppliesTo(sop); });
			if (group != null)
			{
				foreach (PresetVoiLut preset in group.Clone().Presets)
				{
					if (preset.Applicator.AppliesTo(this.SelectedPresentationImage))
						presets.Add(preset);
				}
			}

			int i = 0;
			presets.Sort(new PresetVoiLutCollectionSortByKeyStrokeSortByName());
			foreach(PresetVoiLut preset in presets)
				yield return new PresetVoiLutActionContainer(this, preset, ++i).Action;
		}
	}
}
