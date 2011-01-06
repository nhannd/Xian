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
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Configuration
{
	[ExtensionPoint]
	public sealed class MprConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (MprConfigurationComponentViewExtensionPoint))]
	public class MprConfigurationComponent : ConfigurationApplicationComponent
	{
		public static readonly string Path = "Mpr";

		private MprSettings _settings;
		private float _sliceSpacingFactor = 1;
		private bool _automaticSliceSpacing = true;

		[ValidateGreaterThan(0.5f, Inclusive = true, Message = "MessageValidateSliceSpacing")]
		[ValidateLessThan(10f, Inclusive = true, Message = "MessageValidateSliceSpacing")]
		public float SliceSpacingFactor
		{
			get { return _sliceSpacingFactor; }
			set
			{
				if (_sliceSpacingFactor != value)
				{
					_sliceSpacingFactor = value;
					this.Modified = true;
					this.NotifyPropertyChanged("SliceSpacingFactor");
				}
			}
		}

		public bool ProportionalSliceSpacing
		{
			get { return !_automaticSliceSpacing; }	
			set
			{
				AutomaticSliceSpacing = !value;
			}
		}

		public bool AutomaticSliceSpacing
		{
			get { return _automaticSliceSpacing; }
			set
			{
				if (_automaticSliceSpacing != value)
				{
					_automaticSliceSpacing = value;
					this.Modified = true;
					this.NotifyPropertyChanged("AutoSliceSpacing");
					this.NotifyPropertyChanged("ProportionalSliceSpacing");
				}
			}
		}

		public override void Start()
		{
			base.Start();

			_settings = MprSettings.Default;
			_sliceSpacingFactor = _settings.SliceSpacingFactor;
			_automaticSliceSpacing = _settings.AutoSliceSpacing;
		}

		public override void Stop()
		{
			_settings = null;

			base.Stop();
		}

		public override void Save()
		{
			_settings.AutoSliceSpacing = _automaticSliceSpacing;
			_settings.SliceSpacingFactor = _sliceSpacingFactor;
			_settings.Save();
		}
	}
}