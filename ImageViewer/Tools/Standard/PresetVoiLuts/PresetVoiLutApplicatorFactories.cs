using System;
using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts
{
	internal static class PresetVoiLutApplicatorFactories
	{
		private static 	List<IPresetVoiLutApplicatorFactory> _applicatorFactories;

		public static IPresetVoiLutApplicatorFactory GetFactory(string factoryName)
		{
			return InternalFactories.Find(delegate(IPresetVoiLutApplicatorFactory factory) { return factory.Name == factoryName; });
		}
			
		public static IEnumerable<IPresetVoiLutApplicatorFactory> Factories
		{
			get { return InternalFactories; }	
		}

		private static List<IPresetVoiLutApplicatorFactory> InternalFactories
		{
			get
			{
				if (_applicatorFactories == null)
				{
					_applicatorFactories = new List<IPresetVoiLutApplicatorFactory>();

					PresetVoiLutApplicatorFactoryExtensionPoint xp = new PresetVoiLutApplicatorFactoryExtensionPoint();
					
					//TODO: Later, when we actually want to support presets other than the standard linear presets, we just switch to the commented out line.
					object[] factories = new object[] { xp.CreateExtension(delegate(ExtensionInfo info) { return info.ExtensionClass == typeof(PresetVoiLutLinearApplicatorFactory); }) };
					//object[] factories = xp.CreateExtensions();

					foreach (object factory in factories)
					{
						if (factory is IPresetVoiLutApplicatorFactory)
						{
							IPresetVoiLutApplicatorFactory newFactory = (IPresetVoiLutApplicatorFactory)factory;
							if (!String.IsNullOrEmpty(newFactory.Name))
								_applicatorFactories.Add(newFactory);
							else
								Platform.Log(LogLevel.Warn, "The factory object {0} does not specify a name", factory.GetType().FullName);
						}
						else
							Platform.Log(LogLevel.Warn, "The factory object {0} does not implement interface '{1}'", factory.GetType().FullName, typeof(IPresetVoiLutApplicatorFactory).Name);
					}
				}

				return _applicatorFactories;
			}
		}
	}
}
