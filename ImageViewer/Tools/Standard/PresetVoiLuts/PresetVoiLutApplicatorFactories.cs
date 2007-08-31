using System;
using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts
{
	public static class PresetVoiLutApplicatorFactories
	{
		private static 	List<IPresetVoiLutApplicatorFactory> _applicatorFactories;

		public static IEnumerable<IPresetVoiLutApplicatorFactory> Factories
		{
			get
			{
				if (_applicatorFactories == null)
				{
					_applicatorFactories = new List<IPresetVoiLutApplicatorFactory>();

					PresetVoiLutApplicatorFactoryExtensionPoint xp = new PresetVoiLutApplicatorFactoryExtensionPoint();
					foreach (object factory in xp.CreateExtensions())
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
