using System;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Applicators
{
	public abstract class PresetVoiLutApplicatorFactory<EditPresetVoiLutComponentBaseType> : IPresetVoiLutApplicatorFactory
		where EditPresetVoiLutComponentBaseType : PresetVoiLutApplicatorComponent, new()
	{
		#region IPresetVoiLutApplicatorFactory Members

		public abstract string Name { get; }
		public abstract string Description { get; }

		public bool CanCreateMultiple
		{
			get { return typeof (EditPresetVoiLutComponentBaseType).IsDefined(typeof(AllowMultiplePresetVoiLutApplicatorsAttribute), false); }
		}

		public IPresetVoiLutApplicator Create(PresetVoiLutConfiguration configuration)
		{
			Platform.CheckForNullReference(configuration, "configuration");

			ValidateFactoryName(configuration);

			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			configuration.CopyTo(dictionary);

			EditPresetVoiLutComponentBaseType component = new EditPresetVoiLutComponentBaseType();
			component.SourceFactory = this;
			SimpleSerializer.Serialize(component, dictionary);
			component.Validate();
			return component;
		}

		public IPresetVoiLutApplicatorComponent GetEditComponent(PresetVoiLutConfiguration configuration)
		{
			EditPresetVoiLutComponentBaseType component = new EditPresetVoiLutComponentBaseType();
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
					String.Format("The input configuration's factory name ('{0}') does not match this factory's name ('{1}')",
					              configuration.FactoryName, this.Name));
			}
		}
	}
}