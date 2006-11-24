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

		private bool _visible;
		private string _tooltip;
		private string _label;

		private event EventHandler _visibleChanged;
		private event EventHandler _tooltipChanged;
		private event EventHandler _labelChanged;

		private IImageViewer _imageViewer;
		private LutPresetGroup _lutPresetGroup;

		public LutPresetToolGeneratedAction(IImageViewer imageViewer, LutPresetGroup lutPresetGroup, string pathRoot, string actionRoot)
		{
			_imageViewer = imageViewer;
			_lutPresetGroup = lutPresetGroup;

			Platform.CheckForNullReference(imageViewer, "imageViewer");
			Platform.CheckForNullReference(lutPresetGroup, "lutPresetGroup");
			
			ActionResourceResolver resolver = new ActionResourceResolver(this);

			string actionId = String.Format("{0}:{1}", actionRoot, lutPresetGroup.ActionId);
			_action = new ClickAction(actionId, new ActionPath(pathRoot + lutPresetGroup.ActionId, resolver), ClickActionFlags.None, resolver);

			_action.SetClickHandler(this.ClickHandler);

			_action.SetVisibleObservable(new DynamicObservablePropertyBinding<bool>(this, "Visible", "VisibleChanged"));
			_action.SetLabelObservable(new DynamicObservablePropertyBinding<string>(this, "Label", "LabelChanged"));
			_action.SetTooltipObservable(new DynamicObservablePropertyBinding<string>(this, "Tooltip", "TooltipChanged"));

			_imageViewer.EventBroker.PresentationImageSelected += new EventHandler<PresentationImageSelectedEventArgs>(OnPresentationImageSelected);

			DetermineState(_imageViewer.SelectedPresentationImage);
		}

		public event EventHandler VisibleChanged
		{
			add { _visibleChanged += value; }
			remove { _visibleChanged -= value; }
		}

		public event EventHandler LabelChanged
		{
			add { _labelChanged += value; }
			remove { _labelChanged -= value; }
		}

		public event EventHandler TooltipChanged
		{
			add { _tooltipChanged += value; }
			remove { _tooltipChanged -= value; }
		}

		public bool Visible
		{
			get { return _visible; }
			private set
			{
				if (_visible == value)
					return;

				_visible = value;
				EventsHelper.Fire(_visibleChanged, this, new EventArgs());
			}
		}

		public string Label
		{
			get { return _label; }
			private set
			{
				if (_label == value)
					return;

				_label = value;
				EventsHelper.Fire(_labelChanged, this, new EventArgs());
			}
		}

		public string Tooltip
		{
			get { return _tooltip; }
			private set
			{
				if (_tooltip == value)
					return;

				_tooltip = value;
				EventsHelper.Fire(_tooltipChanged, this, new EventArgs());
			}
		}

		public IClickAction Action
		{
			get { return _action; }
		}

		private void Disable()
		{
			this.Visible = false;
			this.Label = "N/A";
			this.Tooltip = "N/A";
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
				this.Visible = true;
				this.Label = preset.Preset.Label;
				this.Tooltip = this.Label;
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

			if (!this.Visible)
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
