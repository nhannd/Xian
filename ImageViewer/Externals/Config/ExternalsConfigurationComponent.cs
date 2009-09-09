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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;

namespace ClearCanvas.ImageViewer.Externals.Config
{
	[ExtensionPoint]
	public sealed class ExternalsConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (ExternalsConfigurationComponentViewExtensionPoint))]
	public class ExternalsConfigurationComponent : ConfigurationApplicationComponent
	{
		public static readonly string PATH = "ExternalApplications";

		private ExternalsConfigurationSettings _settings;
		private ExternalCollection _externals;

		public IDesktopWindow DesktopWindow
		{
			get { return base.Host.DesktopWindow; }
		}

		public ICollection<IExternal> Externals
		{
			get { return _externals; }
		}

		public void FlagModified()
		{
			this.Modified = true;
			this.NotifyPropertyChanged("Externals");
		}

		public override void Start()
		{
			base.Start();

			_settings = ExternalsConfigurationSettings.Default;

			try
			{
				_externals = ExternalCollection.Deserialize(_settings.Externals);
			}
			catch(Exception ex)
			{
				Platform.Log(LogLevel.Error, ex, "Failed to load external application settings. The XML may be corrupt.");
			}

			if (_externals == null)
				_externals = new ExternalCollection();
		}

		public override void Stop()
		{
			_externals = null;
			_settings = null;

			base.Stop();
		}

		public override void Save()
		{
			try
			{
				_settings.Externals = _externals.Serialize();
			}
			catch(Exception ex)
			{
				Platform.Log(LogLevel.Error, ex, "Failed to save external application settings.");
			}

			_settings.Save();

			ExternalCollection.ReloadSavedExternals();
		}
	}
}