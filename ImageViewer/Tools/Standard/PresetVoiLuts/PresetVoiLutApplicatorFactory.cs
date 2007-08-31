using System;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts
{
	public abstract class PresetVoiLutApplicatorFactory<T> : IPresetVoiLutApplicatorFactory
		where T : PresetVoiLutApplicator, new()
	{
		#region IPresetVoiLutApplicatorFactory Members

		public abstract string Name { get; }

		public IPresetVoiLutApplicator Create(PresetVoiLutConfiguration configuration)
		{
			if (configuration.FactoryName != this.Name)
			{
				throw new ArgumentException(
					String.Format("The input configuration's factory name ('{0}') does not match this factory's name ('{1}')",
					              configuration.FactoryName, this.Name));
			}

			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			configuration.CopyTo(dictionary);

			T applicator = new T();
			applicator.SetOwnerFactory(this);
			SimpleSerializer.Serialize(applicator, dictionary);
			applicator.Validate();
			return applicator;
		}

		#endregion
	}
}