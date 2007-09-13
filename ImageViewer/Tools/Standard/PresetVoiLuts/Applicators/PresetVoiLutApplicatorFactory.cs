using System;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Applicators
{
	public abstract class PresetVoiLutApplicatorFactory<PresetVoiLutApplicatorComponentType> : IPresetVoiLutApplicatorFactory
		where PresetVoiLutApplicatorComponentType : PresetVoiLutApplicatorComponent, new()
	{
		#region IPresetVoiLutApplicatorFactory Members

		public abstract string Name { get; }
		public abstract string Description { get; }

		public bool CanCreateMultiple
		{
			get { return typeof (PresetVoiLutApplicatorComponentType).IsDefined(typeof(AllowMultiplePresetVoiLutApplicatorsAttribute), false); }
		}

		public IPresetVoiLutApplicator Create(PresetVoiLutConfiguration configuration)
		{
			Platform.CheckForNullReference(configuration, "configuration");

			ValidateFactoryName(configuration);

			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			configuration.CopyTo(dictionary);

			PresetVoiLutApplicatorComponentType component = new PresetVoiLutApplicatorComponentType();
			component.SourceFactory = this;
			SimpleSerializer.Serialize(component, dictionary);
			component.Validate();
			return component;
		}

		public IPresetVoiLutApplicatorComponent GetEditComponent(PresetVoiLutConfiguration configuration)
		{
			PresetVoiLutApplicatorComponentType component = new PresetVoiLutApplicatorComponentType();
			component.SourceFactory = this;

			if (configuration != null)
			{
				ValidateFactoryName(configuration);
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				configuration.CopyTo(dictionary);
				SimpleSerializer.Serialize(component, dictionary);
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
