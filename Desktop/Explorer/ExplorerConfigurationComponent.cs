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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;

namespace ClearCanvas.Desktop.Explorer
{
	[ExtensionOf(typeof(ConfigurationPageProviderExtensionPoint))]
	internal class ConfigurationPageProvider : IConfigurationPageProvider
	{
		#region IConfigurationPageProvider Members

		public IEnumerable<IConfigurationPage> GetPages()
		{
			if (!ExplorerLocalSettings.Default.ExplorerIsPrimary && ExplorerTool.GetExplorers().Count > 0)
				yield return new ConfigurationPage<ExplorerConfigurationComponent>("PathExplorer");
		}

		#endregion
	}

	[ExtensionPoint]
	public sealed class ExplorerConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(ExplorerConfigurationComponentViewExtensionPoint))]
	public class ExplorerConfigurationComponent : ConfigurationApplicationComponent
	{
		private bool _launchAsShelf;
		private bool _launchAtStartup;

		public ExplorerConfigurationComponent()
		{
			_launchAsShelf = ExplorerSettings.Default.LaunchAsShelf;
			_launchAtStartup = ExplorerSettings.Default.LaunchAtStartup;
		}

		#region Presentation Model

		public new bool LaunchAsWorkspace
		{
			get { return !_launchAsShelf; }
			set
			{
				if (value)
					LaunchAsShelf = false;
				else
					LaunchAsShelf = true;
			}
		}

		public new bool LaunchAsShelf
		{
			get { return _launchAsShelf; }
			set
			{
				if (_launchAsShelf != value)
				{
					_launchAsShelf = value;
					NotifyPropertyChanged("LaunchAsShelf");
					NotifyPropertyChanged("LaunchAsWorkspace");

					Modified = true;
				}
			}
		}

		public bool LaunchAtStartup
		{
			get { return _launchAtStartup; }
			set
			{
				if (_launchAtStartup != value)
				{
					_launchAtStartup = value;
					NotifyPropertyChanged("LaunchAtStartup");

					Modified = true;
				}
			}
		}

		#endregion

		public override void Save()
		{
			ExplorerSettings.Default.LaunchAsShelf = _launchAsShelf;
			ExplorerSettings.Default.LaunchAtStartup = _launchAtStartup;

			ExplorerSettings.Default.Save();
		}
	}
}
