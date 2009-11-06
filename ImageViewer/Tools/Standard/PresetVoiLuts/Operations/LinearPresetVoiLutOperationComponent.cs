#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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

			IVoiLutManager manager = (IVoiLutManager)GetOriginator(presentationImage);

			PresetVoiLutLinear.PresetVoiLutLinearParameters parameters = new PresetVoiLutLinear.PresetVoiLutLinearParameters(this.Name, this.WindowWidth, this.WindowCenter);

			PresetVoiLutLinear currentLut = manager.VoiLut as PresetVoiLutLinear;
			if (currentLut == null)
				manager.InstallVoiLut(new PresetVoiLutLinear(parameters));
			else
				currentLut.Parameters = parameters;
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