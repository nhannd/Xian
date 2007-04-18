using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using System.Xml;

namespace ClearCanvas.ImageViewer.Tools.Standard.LutPresets
{

	public interface IVoiLutPresetApplicatorFactory
	{
		string FactoryKey { get; }
		IVoiLutPresetApplicator CreateNewApplicator(IDictionary<string, string> configurationValues);
	}

	[ExtensionPoint]
	public sealed class VoiLutPresetApplicatorFactoryExtensionPoint : ExtensionPoint<IVoiLutPresetApplicatorFactory>
	{ 
	}

	internal static class VoiLutPresetFactory
	{
		private class ExtensionManager : BasicExtensionPointManager<IVoiLutPresetApplicatorFactory>
		{
			public ExtensionManager()
			{
			}

			protected override IExtensionPoint GetExtensionPoint()
			{
				return new VoiLutPresetApplicatorFactoryExtensionPoint();
			}

			public IEnumerable<IVoiLutPresetApplicatorFactory> Factories
			{
				get 
				{
					base.LoadExtensions();
					return base.Extensions.AsReadOnly(); 
				}
			}
		}

		private static Dictionary<string, IVoiLutPresetApplicatorFactory> _factories;

		static VoiLutPresetFactory()
		{
		}

		private static Dictionary<string, IVoiLutPresetApplicatorFactory> Factories
		{
			get
			{
				if (_factories == null)
				{
					_factories = new Dictionary<string, IVoiLutPresetApplicatorFactory>();

					ExtensionManager manager = new ExtensionManager();
					foreach (IVoiLutPresetApplicatorFactory factory in manager.Factories)
						_factories[factory.FactoryKey] = factory;
				}

				return _factories;
			}
		}

		private static IVoiLutPresetApplicator CreatePresetApplicator(VoiLutPresetApplicatorConfiguration configuration)
		{
			IVoiLutPresetApplicator applicator = null;

			if (VoiLutPresetFactory.Factories.ContainsKey(configuration.FactoryKey))
				applicator = VoiLutPresetFactory.Factories[configuration.FactoryKey].CreateNewApplicator(configuration.ConfigurationValues);

			return applicator;
		}

		public static VoiLutPreset CreateVoiLutPreset(VoiLutPresetConfiguration configuration)
		{ 
			IVoiLutPresetApplicator applicator = CreatePresetApplicator(configuration.VoiLutPresetApplicatorConfiguration);
			if (applicator == null)
				return null;

			return new VoiLutPreset(configuration.Name, configuration.ModalityFilter, configuration.KeyStroke, applicator);
		}
	}
}
