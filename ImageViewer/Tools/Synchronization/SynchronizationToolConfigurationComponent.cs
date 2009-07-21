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
using ClearCanvas.Desktop.Validation;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer.Tools.Synchronization
{
	[ExtensionOf(typeof (ConfigurationPageProviderExtensionPoint))]
	internal sealed class ConfigurationPageProvider : IConfigurationPageProvider
	{
		public IEnumerable<IConfigurationPage> GetPages()
		{
			if (PermissionsHelper.IsInRole(AuthorityTokens.ViewerVisible))
				yield return new ConfigurationPage<SynchronizationToolConfigurationComponent>("TitleSynchronizationTools");
		}
	}

	[ExtensionPoint]
	public sealed class SynchronizationToolConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (SynchronizationToolConfigurationComponentViewExtensionPoint))]
	public class SynchronizationToolConfigurationComponent : ConfigurationApplicationComponent
	{
		private SynchronizationToolSettings _settings;
		private float _parallelPlaneToleranceAngle;

		[ValidateGreaterThan(0f, Inclusive = true)]
		[ValidateLessThan(15f, Inclusive = true)]
		public float ParallelPlanesToleranceAngle
		{
			get { return _parallelPlaneToleranceAngle; }
			set
			{
				if (_parallelPlaneToleranceAngle != value)
				{
					_parallelPlaneToleranceAngle = value;
					base.Modified = true;
					base.NotifyPropertyChanged("ParallelPlanesToleranceAngle");
				}
			}
		}

		public override void Start()
		{
			base.Start();

			_settings = SynchronizationToolSettings.Default;
			_parallelPlaneToleranceAngle = _settings.ParallelPlanesToleranceAngle;
		}

		public override void Save()
		{
			_settings.ParallelPlanesToleranceAngle = _parallelPlaneToleranceAngle;
			_settings.Save();
		}

		public override void Stop()
		{
			_settings = null;

			base.Stop();
		}
	}
}