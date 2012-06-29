#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.ImageViewer.Tools.Synchronization
{
	[ExtensionOf(typeof (ConfigurationPageProviderExtensionPoint))]
	internal sealed class ConfigurationPageProvider : IConfigurationPageProvider
	{
		public IEnumerable<IConfigurationPage> GetPages()
		{
			if (PermissionsHelper.IsInRole(AuthorityTokens.ViewerVisible))
				yield return new ConfigurationPage<SynchronizationToolConfigurationComponent>("TitleTools/TitleSynchronization");
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