#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.DynamicTe
{
	/// <summary>
	/// Extension point for views onto <see cref="DynamicTeComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class DynamicTeComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// DynamicTeComponent class
	/// </summary>
	[AssociateView(typeof(DynamicTeComponentViewExtensionPoint))]
	public class DynamicTeComponent : ImageViewerToolComponent, IImageOperation
	{
		private bool _probabilityMapVisible = true;
		private decimal _thresholdMinimum = 0;
		private decimal _thresholdMaximum = 100;
		private decimal _threshold = 50;

		private decimal _opacityMinimum = 0;
		private decimal _opacityMaximum = 100;
		private decimal _opacity = 50;

		/// <summary>
		/// Constructor
		/// </summary>
		public DynamicTeComponent(IDesktopWindow desktopWindow)
			: base(desktopWindow)
		{
		}

		public override void Start()
		{
			base.Start();
		}

		public override void Stop()
		{
			// TODO prepare the component to exit the live phase
			// This is a good place to do any clean up
			base.Stop();
		}

		public bool CreateDynamicTeSeriesEnabled
		{
			get
			{
				return this.ImageViewer != null &&
				       !(this.ImageViewer.SelectedPresentationImage is IDynamicTeProvider);
			}
		}

		#region Probability map visibility properties

		public bool ProbabilityMapVisible
		{
			get { return _probabilityMapVisible; }
			set
			{
				if (_probabilityMapVisible != value)
				{
					_probabilityMapVisible = value;
					NotifyPropertyChanged("ProbabilityMapVisible");
					Update();
				}
			}
		}

		public bool ProbabilityMapEnabled
		{
			get
			{
				return this.ImageViewer != null && 
					this.ImageViewer.SelectedPresentationImage is IDynamicTeProvider;
			}
		}

		#endregion

		#region Threshold properties

		public bool ThresholdEnabled
		{
			get { return this.ProbabilityMapEnabled && this.ProbabilityMapVisible; }
		}

		public decimal ThresholdMinimum
		{
			get { return _thresholdMinimum; }
			set
			{
				if (_thresholdMinimum != value)
				{
					_thresholdMinimum = value;
					NotifyPropertyChanged("ThresholdMinimum");
				}
			}
		}

		public decimal ThresholdMaximum
		{
			get { return _thresholdMaximum; }
			set
			{
				if (_thresholdMaximum != value)
				{
					_thresholdMaximum = value;
					NotifyPropertyChanged("ThresholdMaximum");
				}
			}
		}

		public decimal Threshold
		{
			get { return _threshold; }
			set
			{
				if (_threshold != value)
				{
					_threshold = value;
					NotifyPropertyChanged("Threshold");
					Update();
				}
			}
		}

		#endregion

		#region Opacity properties

		public bool OpacityEnabled
		{
			get { return this.ProbabilityMapEnabled && this.ProbabilityMapVisible; }
		}

		public decimal OpacityMinimum
		{
			get { return _opacityMinimum; }
			set
			{
				if (_opacityMinimum != value)
				{
					_opacityMinimum = value;
					NotifyPropertyChanged("OpacityMinimum");
				}
			}
		}

		public decimal OpacityMaximum
		{
			get { return _opacityMaximum; }
			set
			{
				if (_opacityMaximum != value)
				{
					_opacityMaximum = value;
					NotifyPropertyChanged("OpacityMaximum");
				}
			}
		}

		public decimal Opacity
		{
			get { return _opacity; }
			set
			{
				if (_opacity != value)
				{
					_opacity = value;
					NotifyPropertyChanged("Opacity");
					Update();
				}
			}
		}

		#endregion

		public void CreateDynamicTeSeries()
		{
			DynamicTeSeriesCreator.Create(DesktopWindow, ImageViewer);
		}

		private void Update()
		{
			if (!this.ProbabilityMapEnabled)
				return;

			IDynamicTeProvider provider = this.ImageViewer.SelectedPresentationImage as IDynamicTeProvider;

			if (provider == null)
				return;

			ImageOperationApplicator applicator = new ImageOperationApplicator(this.ImageViewer.SelectedPresentationImage, this);
			MemorableUndoableCommand command = new MemorableUndoableCommand(applicator);
			command.Name = "Dynamic Te";
			command.BeginState = applicator.CreateMemento();

			DynamicTe dynamicTe = provider.DynamicTe;

			dynamicTe.ProbabilityMapVisible = this.ProbabilityMapVisible;
			dynamicTe.ApplyProbabilityThreshold((int)this.Threshold, (int)this.Opacity);

			provider.Draw();

			command.EndState = applicator.CreateMemento();

			this.ImageViewer.CommandHistory.AddCommand(command);

			base.NotifyAllPropertiesChanged();
		}

		protected override void OnActiveImageViewerChanged(ActiveImageViewerChangedEventArgs e)
		{
			// stop listening to the old image viewer, if one was set
			if (e.DeactivatedImageViewer != null)
				e.DeactivatedImageViewer.EventBroker.PresentationImageSelected -= EventBroker_PresentationImageSelected;

			// start listening to the new image viewer, if one has been set
			if (e.ActivatedImageViewer != null)
				e.ActivatedImageViewer.EventBroker.PresentationImageSelected += EventBroker_PresentationImageSelected;

			Update();
		}

		private void EventBroker_PresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			Update();
		}

		#region IImageOperation Members

		IMemorable IUndoableOperation<IPresentationImage>.GetOriginator(IPresentationImage image)
		{
			IDynamicTeProvider provider = image as IDynamicTeProvider;
			if (provider == null)
				return null;

			return provider as IMemorable;
		}

		bool IUndoableOperation<IPresentationImage>.AppliesTo(IPresentationImage image)
		{
			return image is IDynamicTeProvider;
		}

		void IUndoableOperation<IPresentationImage>.Apply(IPresentationImage image)
		{
		}

		#endregion
	}
}
