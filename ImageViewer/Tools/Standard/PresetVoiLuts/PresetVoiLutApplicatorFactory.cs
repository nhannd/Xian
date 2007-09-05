using System;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts
{
	public abstract class PresetVoiLutApplicatorFactory<T, U> : IPresetVoiLutApplicatorFactory
		where T : PresetVoiLutApplicator, new()
		where U : EditPresetVoiLutComponentBase<T>, new()
	{
		#region IPresetVoiLutApplicatorFactory Members

		public abstract string Name { get; }

		public IPresetVoiLutApplicator Create(PresetVoiLutConfiguration configuration)
		{
			ValidateFactoryName(configuration);

			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			configuration.CopyTo(dictionary);

			T applicator = new T();
			applicator.SetOwnerFactory(this);
			SimpleSerializer.Serialize(applicator, dictionary);
			applicator.Validate();
			return applicator;
		}

		public IEditPresetVoiLutApplicationComponent GetEditComponent(PresetVoiLutConfiguration configuration)
		{
			if (configuration != null) 
				ValidateFactoryName(configuration);

			U returnComponent = new U();
			T applicator = new T();
			applicator.SetOwnerFactory(this);
			returnComponent.SetPresetApplicator(applicator);

			if (configuration != null)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				configuration.CopyTo(dictionary);
				SimpleSerializer.Serialize(applicator, dictionary);
			}

			return returnComponent;
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