using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.ImageViewer.Tools.Standard.LutPresets
{
	public sealed class LutPresetToolGeneratedAction
	{
		private ClickAction _action;
		private IImageViewer _imageViewer;
		private LutPresetGroup _lutPresetGroup;

		public LutPresetToolGeneratedAction(IImageViewer imageViewer, LutPresetGroup lutPresetGroup, string pathRoot, string actionRoot, string groupHint)
		{
			_imageViewer = imageViewer;
			_lutPresetGroup = lutPresetGroup;

			Platform.CheckForNullReference(imageViewer, "imageViewer");
			Platform.CheckForNullReference(lutPresetGroup, "lutPresetGroup");
			
			ActionResourceResolver resolver = new ActionResourceResolver(this);

			string actionId = String.Format("{0}:{1}", actionRoot, lutPresetGroup.ActionId);
			ActionPath actionPath = new ActionPath(pathRoot + lutPresetGroup.ActionId, resolver);
			_action = new ClickAction(actionId, actionPath, ClickActionFlags.None, resolver);
			_action.GroupHint = new GroupHint(groupHint);

			_action.SetClickHandler(this.ClickHandler);

			_imageViewer.EventBroker.PresentationImageSelected += new EventHandler<PresentationImageSelectedEventArgs>(OnPresentationImageSelected);

			DetermineState(_imageViewer.SelectedPresentationImage);
		}

		public IClickAction Action
		{
			get { return _action; }
		}

		private void Disable()
		{
			_action.Visible = false;
			_action.Label = SR.LabelNotApplicable;
			_action.Tooltip = SR.LabelNotApplicable;
		}

		private void DetermineState(IPresentationImage image)
		{
			if (!IsImageValid(image))
			{
				Disable();
				return;
			}

			FilteredLutPreset preset = _lutPresetGroup.GetFirstMatch(image as DicomPresentationImage);
			if (preset == null)
			{
				Disable();
			}
			else
			{
				_action.Visible = true;
				_action.Label = preset.Preset.Label;
				_action.Tooltip = _action.Label;
			}
		}

		private void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			DetermineState(e.SelectedPresentationImage);
		}

		private bool IsImageValid(IPresentationImage image)
		{
			if (image == null)
				return false;

			if (!(image is DicomPresentationImage))
				return false;

			return true;
		}

		private void ClickHandler()
		{
			if (!IsImageValid(_imageViewer.SelectedPresentationImage))
				return;

			if (!_action.Visible)
				return;

			try
			{
				_lutPresetGroup.Apply(_imageViewer.SelectedPresentationImage as DicomPresentationImage);
			}
			catch (Exception e)
			{
				Platform.Log(e);
				Platform.ShowMessageBox(e.Message);
			}
		}
	}
}
