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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Operations;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Operations
{
	public abstract class PresetVoiLutOperationFactory<PresetVoiLutOperationComponentType> : IPresetVoiLutOperationFactory
		where PresetVoiLutOperationComponentType : PresetVoiLutOperationComponent, new()
	{
		#region IPresetVoiLutOperationFactory Members

		public abstract string Name { get; }
		public abstract string Description { get; }

		public bool CanCreateMultiple
		{
			get { return typeof (PresetVoiLutOperationComponentType).IsDefined(typeof(AllowMultiplePresetVoiLutOperationsAttribute), false); }
		}

		public IPresetVoiLutOperation Create(PresetVoiLutConfiguration configuration)
		{
			Platform.CheckForNullReference(configuration, "configuration");

			ValidateFactoryName(configuration);

			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			configuration.CopyTo(dictionary);

			PresetVoiLutOperationComponentType component = new PresetVoiLutOperationComponentType();
			component.SourceFactory = this;
			SimpleSerializer.Serialize<PresetVoiLutConfigurationAttribute>(component, dictionary);
			component.Validate();
			return component;
		}

		public IPresetVoiLutOperationComponent GetEditComponent(PresetVoiLutConfiguration configuration)
		{
			PresetVoiLutOperationComponentType component = new PresetVoiLutOperationComponentType();
			component.SourceFactory = this;

			if (configuration != null)
			{
				ValidateFactoryName(configuration);
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				configuration.CopyTo(dictionary);
				SimpleSerializer.Serialize<PresetVoiLutConfigurationAttribute>(component, dictionary);
			}

			return component;
		}

		#endregion

		private void ValidateFactoryName(PresetVoiLutConfiguration configuration)
		{
			if (configuration.FactoryName != this.Name)
			{
				throw new ArgumentException(
					String.Format(SR.ExceptionFormatFactoryNamesDoNotMatch, configuration.FactoryName, this.Name));
			}
		}
	}
}
