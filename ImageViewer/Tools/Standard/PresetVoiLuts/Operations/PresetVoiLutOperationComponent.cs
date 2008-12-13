#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Operations
{
	public class PresetVoiLutConfigurationAttribute : Attribute
	{
		public PresetVoiLutConfigurationAttribute()
		{
		}
	}

	[Serializable]
	public sealed class PresetVoiLutOperationValidationException : Exception
	{
		internal PresetVoiLutOperationValidationException(string message)
			: base(message)
		{
		}
	}

	public abstract class PresetVoiLutOperationComponent : ApplicationComponent, IPresetVoiLutOperation, IPresetVoiLutOperationComponent
	{
		private IPresetVoiLutOperationFactory _sourceFactory;
		private EditContext _editContext;
		private bool _valid;

		protected PresetVoiLutOperationComponent()
		{
			_valid = false;
		}

		#region Sealed Off Application Component functionality

		public sealed override IActionSet ExportedActions
		{
			get
			{
				return base.ExportedActions;
			}
		}

		#endregion

		#region IPresetVoiLutOperation Members

		public abstract string Name { get; }
		public abstract string Description { get; }

		public IPresetVoiLutOperationFactory SourceFactory
		{
			get { return _sourceFactory; }
			internal set
			{
				Platform.CheckForNullReference(value, "SourceFactory");
				_sourceFactory = value;
			}
		}
		#region IImageOperation Members

		public IMemorable GetOriginator(IPresentationImage image)
		{
			if (image is IVoiLutProvider)
				return ((IVoiLutProvider) image).VoiLutManager;

			return null;
		}

		public virtual bool AppliesTo(IPresentationImage presentationImage)
		{
			return GetOriginator(presentationImage) != null;
		}

		public abstract void Apply(IPresentationImage image);

		#endregion

		public PresetVoiLutConfiguration GetConfiguration()
		{
			Validate();

			PresetVoiLutConfiguration configuration = PresetVoiLutConfiguration.FromFactory(_sourceFactory);
			foreach (KeyValuePair<string, string> pair in SimpleSerializer.Deserialize<PresetVoiLutConfigurationAttribute>(this))
				configuration[pair.Key] = pair.Value;

			return configuration;
		}

		#endregion

		#region IEditPresetVoiLutApplicationComponent Members

		public IPresetVoiLutOperation GetOperation()
		{
			Validate();
			return this;
		}

		public EditContext EditContext
		{
			get { return _editContext; }
			set { _editContext = value; }
		}

		public bool Valid
		{
			get { return _valid; }
			protected set
			{
				if (_valid == value)
					return;

				_valid = value;
				NotifyPropertyChanged("Valid");
			}
		}

		#endregion

		public abstract void Validate();

		protected virtual void UpdateValid()
		{
		}

		protected void OnPropertyChanged(string propertyName)
		{
			UpdateValid();
			base.Modified = true;
			NotifyPropertyChanged(propertyName);
		}

		#region Helper Methods

		protected static PresetVoiLutOperationValidationException CreateValidationException(string message)
		{
			return new PresetVoiLutOperationValidationException(message);
		}

		protected static bool IsModalityLutProvider(IPresentationImage presentationImage)
		{
			return presentationImage is IModalityLutProvider;
		}

		protected static bool IsVoiLutProvider(IPresentationImage presentationImage)
		{
			return presentationImage is IVoiLutProvider;
		}

		protected static bool IsImageSopProvider(IPresentationImage presentationImage)
		{
			return presentationImage is IImageSopProvider;
		}

		protected static bool IsGrayScaleImage(IPresentationImage presentationImage)
		{
			IImageGraphicProvider graphicProvider = presentationImage as IImageGraphicProvider;
			return graphicProvider != null && graphicProvider.ImageGraphic.PixelData is GrayscalePixelData;
		}

		#endregion
	}
}