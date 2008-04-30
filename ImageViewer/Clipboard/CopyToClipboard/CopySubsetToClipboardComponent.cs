using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Clipboard.CopyToClipboard
{
	public sealed class CopySubsetToClipboardComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(CopySubsetToClipboardComponentViewExtensionPoint))]
	public partial class CopySubsetToClipboardComponent : ApplicationComponent
	{
		private enum RangeSelectionOption
		{
			InstanceNumber = 0,
			Position 
		}

		private enum CopyOption
		{
			CopyRange = 0,
			CopyCustom
		}

		private enum CopyRangeOption
		{
			CopyAll = 0,
			CopyAtInterval
		}

		private readonly IDesktopWindow _desktopWindow;
		private IImageViewer _activeViewer;
		private IDisplaySet _currentDisplaySet;

		private int _numberOfImages;
		private bool _enabled;

		private RangeSelectionOption _rangeSelectionOption;
		private bool _useInstanceNumberEnabled;
		private int _minInstanceNumber;
		private int _maxInstanceNumber;

		private CopyOption _copyOption;
		private bool _copyCustomEnabled;

		private CopyRangeOption _copyRangeOption;
		private int _copyRangeStart;
		private int _copyRangeEnd;
		private int _rangeMinimum;
		private int _rangeMaximum;

		private int _copyRangeInterval = 2;

		private string _customRange;

		public CopySubsetToClipboardComponent(IDesktopWindow desktopWindow)
		{
			Platform.CheckForNullReference(desktopWindow, "desktopWindow");
			_desktopWindow = desktopWindow;
		}

		#region Private Methods

		private void OnWorkspaceChanged(object sender, ItemEventArgs<Workspace> e)
		{
			IImageViewer viewer = null;

			if (_desktopWindow.ActiveWorkspace != null)
				viewer = ImageViewerComponent.GetAsImageViewer(_desktopWindow.ActiveWorkspace);
			
			SetActiveViewer(viewer);
		}

		private void OnImageBoxSelected(object sender, ImageBoxSelectedEventArgs e)
		{
			CurrentDisplaySet = e.SelectedImageBox.DisplaySet;
		}

		private void SetActiveViewer(IImageViewer viewer)
		{
			if (_activeViewer != null)
				_activeViewer.EventBroker.ImageBoxSelected -= OnImageBoxSelected;

			_activeViewer = viewer;

			IDisplaySet displaySet = null;

			if (_activeViewer != null)
			{
				_activeViewer.EventBroker.ImageBoxSelected += OnImageBoxSelected;

				if (_activeViewer.SelectedImageBox != null)
					displaySet = _activeViewer.SelectedImageBox.DisplaySet;
			}

			CurrentDisplaySet = displaySet;
		}

		private void CopyToClipboardInternal()
		{
			if (this.HasValidationErrors)
			{
				base.ShowValidation(true);
			}
			else
			{
				IImageSelectionStrategy strategy;

				if (CopyRange)
				{
					int interval = 1;
					if (CopyRangeAtInterval)
						interval = CopyRangeInterval;

					strategy = new RangeImageSelectionStrategy(CopyRangeStart, CopyRangeEnd, interval, UseInstanceNumber);
				}
				else
				{
					strategy = new CustomImageSelectionStrategy(CustomRange, RangeMinimum, RangeMaximum, UseInstanceNumber);
				}

				Clipboard.Add(CurrentDisplaySet, strategy);
			}
		}

		#endregion

		public override void Start()
		{
			_desktopWindow.Workspaces.ItemActivationChanged += OnWorkspaceChanged;

			OnWorkspaceChanged(null, null);

			base.Start();
		}

		public override void Stop()
		{
			_desktopWindow.Workspaces.ItemActivationChanged -= OnWorkspaceChanged;

			SetActiveViewer(null);

			base.Stop();
		}

		#region Validation Methods

		[ValidationMethodFor("CustomRange")]
		private ValidationResult ValidateCustomRange()
		{
			List<int> ranges;
			if (CopyCustom && !CustomImageSelectionStrategy.Parse(CustomRange, RangeMinimum, RangeMaximum, out ranges))
				return new ValidationResult(false, SR.MessageCustomRangeInvalid);

			return new ValidationResult(true, "");
		}

		[ValidationMethodFor("CopyRangeStart")]
		private ValidationResult ValidateCopyRangeStart()
		{
			if (CopyRange && (CopyRangeStart < RangeMinimum || CopyRangeStart > RangeMaximum))
				return new ValidationResult(false, SR.MessageStartValueOutOfRange);

			return new ValidationResult(true, "");
		}

		[ValidationMethodFor("CopyRangeEnd")]
		private ValidationResult ValidateCopyRangeEnd()
		{
			if (CopyRange)
			{
				if (CopyRangeEnd < RangeMinimum || CopyRangeEnd > RangeMaximum)
					return new ValidationResult(false, SR.MessageEndValueOutOfRange);
				else if (CopyRangeEnd < CopyRangeStart)
					return new ValidationResult(false, SR.MessageEndValueLargerThanStart);
			}

			return new ValidationResult(true, "");
		}

		[ValidationMethodFor("CopyRangeInterval")]
		private ValidationResult ValidateCopyRangeInterval()
		{
			if (CopyRange && CopyRangeAtInterval)
			{
				if (CopyRangeInterval < RangeMinInterval || CopyRangeInterval > RangeMaxInterval)
					return new ValidationResult(false, SR.MessageRangeIntervalInvalid);
			}

			return new ValidationResult(true, "");
		}

		#endregion

		private IDisplaySet CurrentDisplaySet
		{
			get { return _currentDisplaySet; }
			set
			{
				if (_currentDisplaySet == value)
					return;

				_currentDisplaySet = value;

				Reset();
			}
		}

		private void Reset()
		{
			_minInstanceNumber = int.MaxValue;
			_maxInstanceNumber = int.MinValue;
			_useInstanceNumberEnabled = true;

			_numberOfImages = 0;

			if (CurrentDisplaySet != null)
			{
				_numberOfImages = CurrentDisplaySet.PresentationImages.Count;

				foreach (IPresentationImage image in CurrentDisplaySet.PresentationImages)
				{
					if (image is IImageSopProvider)
					{
						IImageSopProvider provider = (IImageSopProvider) image;
						if (provider.ImageSop.InstanceNumber < _minInstanceNumber)
							_minInstanceNumber = provider.ImageSop.InstanceNumber;
						if (provider.ImageSop.InstanceNumber > _maxInstanceNumber)
							_maxInstanceNumber = provider.ImageSop.InstanceNumber;
					}
				}
			}

			if (_numberOfImages == 0 || _minInstanceNumber == int.MaxValue || _maxInstanceNumber == int.MinValue)
			{
				_minInstanceNumber = _maxInstanceNumber = 0;
				_useInstanceNumberEnabled = false;
			}

			if (!_useInstanceNumberEnabled)
				UseInstanceNumber = false;

			NotifyPropertyChanged("UseInstanceNumberEnabled");

			UpdateRanges();

			_copyCustomEnabled = _numberOfImages > 2;
			if (!_copyCustomEnabled)
				CopyCustom = false;

			NotifyPropertyChanged("CustomRangeEnabled");
			NotifyPropertyChanged("CopyCustomEnabled");

			_enabled = _numberOfImages > 0;
			NotifyPropertyChanged("Enabled");
		}

		private void UpdateRanges()
		{
			if (UseInstanceNumber)
			{
				_rangeMinimum = _minInstanceNumber;
				_rangeMaximum = _maxInstanceNumber;
				
				NotifyPropertyChanged("RangeMinimum");
				NotifyPropertyChanged("RangeMaximum");

				CopyRangeStart = RangeMinimum;
				CopyRangeEnd = RangeMaximum;
			}
			else
			{
				_rangeMinimum = 1;
				_rangeMaximum = _currentDisplaySet == null ? 1 : _currentDisplaySet.PresentationImages.Count;

				NotifyPropertyChanged("RangeMinimum");
				NotifyPropertyChanged("RangeMaximum");
				
				CopyRangeStart = RangeMinimum;
				CopyRangeEnd = RangeMaximum;
			}

			FixRangeInterval();
		}

		private void FixRangeInterval()
		{
			CopyRangeInterval = Math.Min(CopyRangeInterval, RangeMaxInterval);

			if (!CopyRangeAtIntervalEnabled)
				CopyRangeAtInterval = false;

			NotifyPropertyChanged("RangeMaxInterval");
			NotifyPropertyChanged("CopyRangeIntervalEnabled");
			NotifyPropertyChanged("CopyRangeAtIntervalEnabled");
		}

		#region Presentation Model

		public bool UsePositionNumber
		{
			get { return _rangeSelectionOption == RangeSelectionOption.Position; }
			set
			{
				if (!value)
				{
					_rangeSelectionOption = RangeSelectionOption.InstanceNumber;
					NotifyPropertyChanged("UsePositionNumber");
					NotifyPropertyChanged("UseInstanceNumber");

					UpdateRanges();
				}
			}
		}

		public bool UseInstanceNumber
		{
			get { return _rangeSelectionOption == RangeSelectionOption.InstanceNumber; }
			set
			{
				if (!value)
				{
					_rangeSelectionOption = RangeSelectionOption.Position;
					NotifyPropertyChanged("UseInstanceNumber");
					NotifyPropertyChanged("UsePositionNumber");

					UpdateRanges();
				}
			}
		}

		public bool UseInstanceNumberEnabled
		{
			get { return _useInstanceNumberEnabled; }
		}

		public int RangeMinimum
		{
			get { return _rangeMinimum; }
		}

		public int RangeMaximum
		{
			get { return _rangeMaximum; }
		}

		public int RangeMinInterval
		{
			get { return 2; }	
		}

		public int RangeMaxInterval
		{
			get { return Math.Max(RangeMinInterval, CopyRangeEnd - CopyRangeStart); }
		}
		
		public bool CopyRange
		{
			get { return _copyOption == CopyOption.CopyRange; }
			set
			{
				if (!value)
				{
					_copyOption = CopyOption.CopyCustom;
					NotifyPropertyChanged("CopyRange");
					NotifyPropertyChanged("CopyCustom");
				}
			}
		}

		public bool CopyRangeAll
		{
			get { return _copyRangeOption == CopyRangeOption.CopyAll; }
			set
			{
				if (!value)
				{
					_copyRangeOption = CopyRangeOption.CopyAtInterval;
					NotifyPropertyChanged("CopyRangeAll");
					NotifyPropertyChanged("CopyRangeAtInterval");
				}
			}
		}

		public int CopyRangeStart
		{
			get { return _copyRangeStart; }
			set
			{
				if (value == _copyRangeStart)
					return;

				_copyRangeStart = value;
				NotifyPropertyChanged("CopyRangeStart");

				FixRangeInterval();
			}
		}

		public int CopyRangeEnd
		{
			get { return _copyRangeEnd; }
			set
			{
				if (value == _copyRangeEnd)
					return;

				_copyRangeEnd = value;
				NotifyPropertyChanged("CopyRangeEnd");

				FixRangeInterval();
			}
		}

		public bool CopyRangeAtInterval
		{
			get { return _copyRangeOption == CopyRangeOption.CopyAtInterval; }
			set
			{
				if (!value)
				{
					_copyRangeOption = CopyRangeOption.CopyAll;
					NotifyPropertyChanged("CopyRangeAtInterval");
					NotifyPropertyChanged("CopyRangeAll");
				}
			}
		}

		public bool CopyRangeAtIntervalEnabled
		{
			get { return CopyRange && (CopyRangeEnd - CopyRangeStart) >= RangeMinInterval; }
		}

		public int CopyRangeInterval
		{
			get { return _copyRangeInterval; }
			set
			{
				if (value == _copyRangeInterval)
					return;

				_copyRangeInterval = value;
				NotifyPropertyChanged("CopyRangeInterval");
			}
		}

		public bool CopyRangeIntervalEnabled
		{
			get { return CopyRangeAtInterval && CopyRangeAtIntervalEnabled; }
		}

		public bool CopyCustom
		{
			get { return _copyOption == CopyOption.CopyCustom; }
			set
			{
				if (!value)
				{
					_copyOption = CopyOption.CopyRange;
					NotifyPropertyChanged("CopyCustom");
					NotifyPropertyChanged("CopyRange");
				}
			}
		}

		public bool CopyCustomEnabled
		{
			get { return _copyCustomEnabled; }	
		}

		public string CustomRange
		{
			get { return _customRange; }
			set
			{
				if (value == _customRange)
					return;

				_customRange = value;
				NotifyPropertyChanged("CustomRange");
			}
		}

		public bool CustomRangeEnabled
		{
			get { return CopyCustom && CopyCustomEnabled; }
		}

		public bool Enabled
		{
			get { return _enabled; }
		}

		public void CopyToClipboard()
		{
			if (!Enabled)
				return;

			try
			{
				BlockingOperation.Run(CopyToClipboardInternal);
			}
			catch(Exception e)
			{
				ExceptionHandler.Report(e, Host.DesktopWindow);
			}
		}

		#endregion
	}
}
