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
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Operations
{
	internal static class PresetVoiLutOperationFactories
	{
		private static List<IPresetVoiLutOperationFactory> _factories;

		private static List<IPresetVoiLutOperationFactory> InternalFactories
		{
			get
			{
				if (_factories == null)
				{
					_factories = new List<IPresetVoiLutOperationFactory>();

					_factories.Add(new LinearPresetVoiLutOperationFactory());
					//TODO: Uncomment this to re-enable multi-type preset lut functionality.
					//_factories.Add(new MinMaxAlgorithmPresetVoiLutOperationFactory());
					//_factories.Add(new AutoPresetVoiLutOperationFactory());

					PresetVoiLutOperationFactoryExtensionPoint xp = new PresetVoiLutOperationFactoryExtensionPoint();
					
					object[] factories = xp.CreateExtensions();
					foreach (object factory in factories)
					{
						if (factory is IPresetVoiLutOperationFactory)
						{
							IPresetVoiLutOperationFactory newFactory = (IPresetVoiLutOperationFactory)factory;
							if (!String.IsNullOrEmpty(newFactory.Name))
								_factories.Add(newFactory);
							else
								Platform.Log(LogLevel.Warn, SR.MessageFormatFactoryHasNoName, factory.GetType().FullName);
						}
						else
							Platform.Log(LogLevel.Warn, SR.MessageFormatFactoryDoesNotImplementRequiredInterface, factory.GetType().FullName, typeof(IPresetVoiLutOperationFactory).Name);
					}
				}

				return _factories;
			}
		}

		public static IEnumerable<IPresetVoiLutOperationFactory> Factories
		{
			get { return InternalFactories; }
		}

		public static IPresetVoiLutOperationFactory GetFactory(string factoryName)
		{
			return InternalFactories.Find(delegate(IPresetVoiLutOperationFactory factory) { return factory.Name == factoryName; });
		}
	}
}