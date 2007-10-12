#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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