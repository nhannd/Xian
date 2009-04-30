#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.Configuration;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// A Singleton class that provides a way for <see cref="ApplicationSettingsBase"/>-derived objects
	/// to be updated when a setting value was modified externally.
	/// </summary>
	/// <remarks>
    /// This class provides a way to update existing instances of settings objects derived from
    /// <see cref="ApplicationSettingsBase"/>.  The individual instances must register themselves
    /// with this class in order to receive updates.
    /// </remarks>
	public class ApplicationSettingsRegistry
	{
		private static readonly ApplicationSettingsRegistry _instance = new ApplicationSettingsRegistry();

		private readonly object _syncLock = new object();
		private readonly List<ApplicationSettingsBase> _registeredSettingsInstances;

		private ApplicationSettingsRegistry()
		{
			_registeredSettingsInstances = new List<ApplicationSettingsBase>();
		}
		
		/// <summary>
		/// Gets the single instance of this class.
		/// </summary>
		public static ApplicationSettingsRegistry Instance
		{
			get { return _instance; }
		}

		/// <summary>
		/// Registers an instance of a settings class.
		/// </summary>
		public void RegisterInstance(ApplicationSettingsBase settingsInstance)
		{ 
			lock(_syncLock)
			{
				if (!_registeredSettingsInstances.Contains(settingsInstance))
					_registeredSettingsInstances.Add(settingsInstance);
			}
		}

		/// <summary>
		/// Unregisters an instance of a settings class.
		/// </summary>
		public void UnregisterInstance(ApplicationSettingsBase settingsInstance)
		{
			lock (_syncLock)
			{
				if (_registeredSettingsInstances.Contains(settingsInstance))
					_registeredSettingsInstances.Remove(settingsInstance);
			}
		}

		/// <summary>
		/// Calls <see cref="ApplicationSettingsBase.Reload"/> on all registered 
		/// settings instances that match the specified group.
		/// </summary>
		public void Reload(SettingsGroupDescriptor group)
		{
			Type settingsClass = Type.GetType(group.AssemblyQualifiedTypeName, false);
			if (settingsClass != null)
			{
				lock (_syncLock)
				{
					_registeredSettingsInstances
						.FindAll(delegate(ApplicationSettingsBase instance) { return instance.GetType().Equals(settingsClass); })
						.ForEach(delegate(ApplicationSettingsBase instance) { instance.Reload(); });
				}
			}
		}
	}
}