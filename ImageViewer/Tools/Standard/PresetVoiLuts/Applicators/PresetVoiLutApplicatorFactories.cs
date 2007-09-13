using System;
using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Applicators
{
	internal static class PresetVoiLutApplicatorFactories
	{
		private static List<IPresetVoiLutApplicatorFactory> _applicatorFactories;

		private static List<IPresetVoiLutApplicatorFactory> InternalFactories
		{
			get
			{
				if (_applicatorFactories == null)
				{
					_applicatorFactories = new List<IPresetVoiLutApplicatorFactory>();

					PresetVoiLutApplicatorFactoryExtensionPoint xp = new PresetVoiLutApplicatorFactoryExtensionPoint();
					
					object[] factories = xp.CreateExtensions();
					foreach (object factory in factories)
					{
						if (factory is IPresetVoiLutApplicatorFactory)
						{
							IPresetVoiLutApplicatorFactory newFactory = (IPresetVoiLutApplicatorFactory)factory;
							if (!String.IsNullOrEmpty(newFactory.Name))
								_applicatorFactories.Add(newFactory);
							else
								Platform.Log(LogLevel.Warn, SR.MessageFormatFactoryHasNoName, factory.GetType().FullName);
						}
						else
							Platform.Log(LogLevel.Warn, SR.MessageFormatFactoryDoesNotImplementRequiredInterface, factory.GetType().FullName, typeof(IPresetVoiLutApplicatorFactory).Name);
					}
				}

				return _applicatorFactories;
			}
		}

		public static IEnumerable<IPresetVoiLutApplicatorFactory> Factories
		{
			get { return InternalFactories; }
		}

		public static IPresetVoiLutApplicatorFactory GetFactory(string factoryName)
		{
			return InternalFactories.Find(delegate(IPresetVoiLutApplicatorFactory factory) { return factory.Name == factoryName; });
		}
	}
}