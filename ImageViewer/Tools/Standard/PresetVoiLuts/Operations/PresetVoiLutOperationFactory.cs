#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
