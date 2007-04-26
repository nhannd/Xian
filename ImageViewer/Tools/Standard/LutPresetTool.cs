using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Imaging;
using System.Xml.Serialization;
using System.IO;
using ClearCanvas.ImageViewer.Tools.Standard.LutPresets;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.BaseTools;
using System.ComponentModel;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	//it's ok to assign the same keystroke more than once as long as it's for the same ClickAction.
	[KeyboardAction("auto", LutPresetTool.ViewerKeyboardRoot + "Auto", KeyStroke = XKeys.F2)]
	[MenuAction("auto", LutPresetTool.ViewerContextMenuRoot + "Auto", Flags = ClickActionFlags.CheckAction, KeyStroke = XKeys.F2)]
	[ClickHandler("auto", "AutoApplyLut")]
	[EnabledStateObserver("auto", "Enabled", "EnabledChanged")]
	[GroupHint("auto", LutPresetTool.GroupHint)]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class LutPresetTool : ImageViewerTool
	{
		#region LutPresetKeyStrokeGroupApplicator class

		private sealed class LutPresetKeyStrokeGroupApplicator
		{
			private XKeys _keyStroke;
			private List<VoiLutPreset> _lutPresets;

			public LutPresetKeyStrokeGroupApplicator(XKeys keyStroke, IEnumerable<VoiLutPresetConfiguration> configurations)
			{
				_keyStroke = keyStroke;

				_lutPresets = new List<VoiLutPreset>();
				foreach (VoiLutPresetConfiguration configuration in configurations)
					_lutPresets.Add(VoiLutPresetFactory.CreateVoiLutPreset(configuration));
			}

			public XKeys KeyStroke
			{
				get { return _keyStroke; }
			}

			public VoiLutPreset GetFirstMatch(IPresentationImage presentationImage)
			{
				foreach (VoiLutPreset preset in _lutPresets)
				{
					if (preset.AppliesTo(presentationImage))
						return preset;
				}

				return null;
			}

			public void Apply(IPresentationImage presentationImage)
			{
				foreach (VoiLutPreset preset in _lutPresets)
				{
					if (preset.AppliesTo(presentationImage))
					{
						preset.Apply(presentationImage);
					}
				}
			}
		}

		#endregion

		#region LutPresetKeyStrokeGroupActionContainer class

		private class LutPresetKeyStrokeGroupActionContainer
		{
			private ClickAction _action;
			private LutPresetKeyStrokeGroupApplicator _applicator;
			private LutPresetTool _parent;

			public LutPresetKeyStrokeGroupActionContainer(LutPresetTool parent, LutPresetKeyStrokeGroupApplicator applicator, string pathRoot)
			{
				_parent = parent;
				_applicator = applicator;

				string actionId = String.Format("{0}:{1}", parent.GetType().FullName, applicator.KeyStroke.ToString());
				string actionPath = String.Format("{0}LutPreset-{1}", pathRoot, applicator.KeyStroke.ToString());

				ActionPath path = new ActionPath(actionPath, _parent._resolver);
				_action = new ClickAction(actionId, path, ClickActionFlags.None, _parent._resolver);
				_action.GroupHint = new GroupHint(LutPresetTool.GroupHint);
				_action.KeyStroke = applicator.KeyStroke;
				_action.SetClickHandler(this.ClickHandler);

				parent.Context.Viewer.EventBroker.PresentationImageSelected += new EventHandler<PresentationImageSelectedEventArgs>(OnPresentationImageSelected);

				DetermineState(parent.Context.Viewer.SelectedPresentationImage);
			}

			public IClickAction Action
			{
				get { return _action; }
			}

			private void Disable()
			{
				_action.Visible = false;
				_action.Enabled = false;
				_action.Label = SR.LabelNotApplicable;
				_action.Tooltip = SR.LabelNotApplicable;
			}

			private void DetermineState(IPresentationImage image)
			{
				VoiLutPreset preset = _applicator.GetFirstMatch(image);
				if (preset == null)
				{
					Disable();
				}
				else
				{
					_action.Visible = true;
					_action.Enabled = true;
					_action.Label = preset.Name;
					_action.Tooltip = preset.Name;
				}
			}

			private void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
			{
				DetermineState(e.SelectedPresentationImage);
			}

			private void ClickHandler()
			{
				if (!_action.Enabled)
					return;

				try
				{
					_applicator.Apply(_parent.Context.Viewer.SelectedPresentationImage);
				}
				catch (Exception e)
				{
					Platform.Log(e);
					Platform.ShowMessageBox(e.Message);
				}
			}
		}

		private class LutPresetNoKeyStrokeActionContainer
		{
			private LutPresetTool _parent;
			private VoiLutPreset _preset;
			private ClickAction _action;

			public LutPresetNoKeyStrokeActionContainer(LutPresetTool parent, VoiLutPresetConfiguration configuration, string pathRoot)
			{
				_parent = parent;
				_preset = VoiLutPresetFactory.CreateVoiLutPreset(configuration);

				string actionId = String.Format("{0}:{1}", parent.GetType().FullName, configuration.Name);
				string actionPath = String.Format("{0}LutPreset-{1}", pathRoot, configuration.Name);

				ActionPath path = new ActionPath(actionPath, _parent._resolver);
				_action = new ClickAction(actionId, path, ClickActionFlags.None, _parent._resolver);
				_action.GroupHint = new GroupHint(LutPresetTool.GroupHint);
				_action.KeyStroke = XKeys.None;
				_action.SetClickHandler(this.ClickHandler);

				_action.Label = _preset.Name;
				_action.Tooltip = _preset.Name;

				parent.Context.Viewer.EventBroker.PresentationImageSelected += new EventHandler<PresentationImageSelectedEventArgs>(OnPresentationImageSelected);

				DetermineState(parent.Context.Viewer.SelectedPresentationImage);
			}

			public IClickAction Action
			{
				get { return _action; }
			}

			private void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
			{
				DetermineState(e.SelectedPresentationImage);
			}

			private void DetermineState(IPresentationImage presentationImage)
			{
				_action.Enabled = _preset.AppliesTo(presentationImage);
				_action.Visible = _action.Enabled;
			}

			private void ClickHandler()
			{
				if (!_action.Enabled)
					return;

				try
				{
					_preset.Apply(_parent.Context.Viewer.SelectedPresentationImage);
				}
				catch (Exception e)
				{
					Platform.Log(e);
					Platform.ShowMessageBox(e.Message);
				}
			}
		}

		#endregion

		public const string GroupHint = "Tools.Image.Manipulation.Lut.Presets";

		public const string ViewerKeyboardRoot = "imageviewer-keyboard/ToolsStandardLutPresets/";
		public const string ViewerContextMenuRoot = "imageviewer-contextmenu/MenuLutPresets/";

		private IActionSet _actionSet;
		ActionResourceResolver _resolver;
		
		public LutPresetTool()
		{
			 _resolver = new ActionResourceResolver(this);
		}

		public override IActionSet Actions
		{
			get
			{
				if (_actionSet == null)
				{
					_actionSet = base.Actions;
					_actionSet = _actionSet.Union(new ActionSet(BuildGeneratedActions()));
				}

				return _actionSet;
			}
		}

		public override void Initialize()
		{
			base.Initialize();

			OnPresentationImageSelected(null, null);

			this.Context.Viewer.EventBroker.PresentationImageSelected += new EventHandler<PresentationImageSelectedEventArgs>(OnPresentationImageSelected);
		}

		private void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			this.Enabled = !(this.SelectedPresentationImage == null || 
				this.SelectedImageSopProvider == null || 
				this.SelectedVOILUTLinearProvider == null || 
				this.SelectedVOILUTLinearProvider.VoiLutLinear == null);
		}

		private void AutoApplyLut()
		{
			if (!this.Enabled)
				return;

			Window[] windowCenterAndWidth = this.SelectedImageSopProvider.ImageSop.WindowCenterAndWidth;

			if (windowCenterAndWidth == null || windowCenterAndWidth.Length == 0)
				return;

			if (double.IsNaN(windowCenterAndWidth[0].Width) ||
				double.IsNaN(windowCenterAndWidth[0].Center))
				return;

			WindowLevelApplicator applicator = new WindowLevelApplicator(this.SelectedPresentationImage);
			UndoableCommand command = new UndoableCommand(applicator);
			command.Name = SR.CommandWindowLevel;
			command.BeginState = applicator.CreateMemento();

			this.SelectedVOILUTLinearProvider.VoiLutLinear.WindowWidth = windowCenterAndWidth[0].Width;
			this.SelectedVOILUTLinearProvider.VoiLutLinear.WindowCenter = windowCenterAndWidth[0].Center;
			this.SelectedVOILUTLinearProvider.Draw();

			applicator.ApplyToLinkedImages();

			command.EndState = applicator.CreateMemento();
			if (!command.EndState.Equals(command.BeginState))
			{
				applicator.SetMemento(command.EndState);
				this.Context.Viewer.CommandHistory.AddCommand(command);
			}
		}

		private IEnumerable<IAction> BuildGeneratedActions()
		{
			VoiLutPresetConfigurationCollection voiLutPresetConfigurationCollection = VoiLutPresetSettings.Default.GetVoiLutPresetConfigurations();

			List<IAction> generatedActions = new List<IAction>();
			IDictionary<XKeys, IEnumerable<VoiLutPresetConfiguration>> configurationsByKeyStroke = VoiLutPresetConfigurations.GetConfigurationsByKeyStroke();
			foreach (KeyValuePair<XKeys, IEnumerable<VoiLutPresetConfiguration>> kvp in configurationsByKeyStroke)
			{
				if (kvp.Key != XKeys.None)
				{
					LutPresetKeyStrokeGroupApplicator groupApplicator = new LutPresetKeyStrokeGroupApplicator(kvp.Key, kvp.Value);
					generatedActions.Add(new LutPresetKeyStrokeGroupActionContainer(this, groupApplicator, ViewerContextMenuRoot).Action);
					generatedActions.Add(new LutPresetKeyStrokeGroupActionContainer(this, groupApplicator, ViewerKeyboardRoot).Action);
				}
			}

			if (configurationsByKeyStroke.ContainsKey(XKeys.None))
			{
				IEnumerable<VoiLutPresetConfiguration> configurations = configurationsByKeyStroke[XKeys.None];
				foreach (VoiLutPresetConfiguration configuration in configurations)
					generatedActions.Add(new LutPresetNoKeyStrokeActionContainer(this, configuration, ViewerContextMenuRoot).Action);
			}

			return generatedActions;
		}
	}
}
