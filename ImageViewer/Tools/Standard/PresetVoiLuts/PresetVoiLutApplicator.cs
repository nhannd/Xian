using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts
{
	public abstract class PresetVoiLutApplicator : IPresetVoiLutApplicator
	{
		private IPresetVoiLutApplicatorFactory _ownerFactory;

		internal void SetOwnerFactory(IPresetVoiLutApplicatorFactory ownerFactory)
		{
			Platform.CheckForNullReference(ownerFactory, "ownerFactory");
			_ownerFactory = ownerFactory;
		}

		#region IPresetVoiLutApplicator Members

		public abstract string Name { get; }
		public abstract string Description { get; }
		
		public abstract bool AppliesTo(IPresentationImage presentationImage);
		public abstract void Apply(IPresentationImage image);

		public PresetVoiLutConfiguration GetConfiguration()
		{
			PresetVoiLutConfiguration configuration = PresetVoiLutConfiguration.FromFactory(_ownerFactory);
			foreach (KeyValuePair<string, string> pair in SimpleSerializer.Deserialize(this))
				configuration[pair.Key] = pair.Value;

			return configuration;
		}

		#endregion

		public abstract void Validate();
	}
}
