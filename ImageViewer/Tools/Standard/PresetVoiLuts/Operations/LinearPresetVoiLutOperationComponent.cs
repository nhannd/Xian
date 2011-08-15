#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Luts;
using ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Operations;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Operations
{
	[ExtensionPoint]
	public sealed class LinearPresetVoiLutOperationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AllowMultiplePresetVoiLutOperations]
	[AssociateView(typeof(LinearPresetVoiLutOperationComponentViewExtensionPoint))]
	public sealed class LinearPresetVoiLutOperationComponent : PresetVoiLutOperationComponent
	{
		private string _name;
		private double _windowWidth;
		private double _windowCenter;

		public LinearPresetVoiLutOperationComponent()
		{
			_name = "";
			_windowWidth = double.NaN;
			_windowCenter = double.NaN;
		}

		public override string Name
		{
			get { return _name; }
		}

		public override string Description
		{
			get { return String.Format(SR.FormatDescriptionLinearPreset, this.WindowWidth, this.WindowCenter); }
		}
		
		[PresetVoiLutConfiguration]
		public string PresetName
		{
			get { return _name; }
			set
			{
				if (_name == value)
					return;
				
				_name = value;
				OnPropertyChanged("Name");
			}
		}

		[PresetVoiLutConfiguration]
		public double WindowWidth
		{
			get { return _windowWidth; }
			set
			{
				if (_windowWidth == value)
					return;

				_windowWidth = value;
				OnPropertyChanged("WindowWidth");
			}
		}

		[PresetVoiLutConfiguration]
		public double WindowCenter
		{
			get { return _windowCenter; }
			set
			{
				if (_windowCenter == value)
					return;

				_windowCenter = value;
				OnPropertyChanged("WindowCenter");
			}
		}

		public override void Apply(IPresentationImage presentationImage)
		{
			if (!AppliesTo(presentationImage))
				throw new InvalidOperationException(String.Format(SR.ExceptionFormatInputPresentationImageMustImplement, typeof(IVoiLutProvider).Name));

			var manager = (IVoiLutManager)GetOriginator(presentationImage);
            var currentLut = manager.VoiLut as NamedVoiLutLinear;
			if (currentLut == null)
			{
			    manager.InstallVoiLut(new NamedVoiLutLinear(Name, WindowWidth, WindowCenter));
			}
			else
			{
			    currentLut.Name = Name;
			    currentLut.WindowWidth = WindowWidth;
			    currentLut.WindowCenter = WindowCenter;
			}
		}

		public override void Start()
		{
			if (this.WindowWidth < 1 || double.IsNaN(this.WindowWidth))
				this.WindowWidth = 1;
			if (double.IsNaN(this.WindowCenter))
				this.WindowCenter = 0;

			UpdateValid();

			base.Modified = false;

			base.Start();
		}

		public override void Validate()
		{
			if (String.IsNullOrEmpty(this.Name))
				throw CreateValidationException(SR.ExceptionPresetNameCannotBeEmpty);
			if (double.IsNaN(this.WindowWidth) || this.WindowWidth < 1)
				throw CreateValidationException(String.Format(SR.ExceptionFormatWindowWidthInvalid, this.WindowWidth));
			if (double.IsNaN(this.WindowCenter))
				throw CreateValidationException(SR.ExceptionWindowCenterInvalid);
		}

		protected override void UpdateValid()
		{
			bool valid = true;
			if (String.IsNullOrEmpty(this.Name))
				valid = false;

			if (this.WindowWidth < 1 || double.IsNaN(this.WindowWidth))
				valid = false;

			if (double.IsNaN(this.WindowCenter))
				valid = false;

			base.Valid = valid;
		}
	}
}