#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Configuration
{
	/// <summary>
	/// Extension point for views onto <see cref="MonitorConfigurationApplicationComponent"/>
	/// </summary>
	[ExtensionPoint]
	public sealed class MonitorConfigurationApplicationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// MonitorConfigurationApplicationComponent class
	/// </summary>
	[AssociateView(typeof(MonitorConfigurationApplicationComponentViewExtensionPoint))]
	public class MonitorConfigurationApplicationComponent : ConfigurationApplicationComponent
	{
		private WindowBehaviour _windowBehaviour;

		/// <summary>
		/// Constructor
		/// </summary>
		public MonitorConfigurationApplicationComponent()
		{
		}

		public bool SingleWindow
		{
			get
			{
				return _windowBehaviour == WindowBehaviour.Single;
			}
			set
			{
				if (value == true)
				{
					_windowBehaviour = WindowBehaviour.Single;
					this.Modified = true;
				}
			}
		}

		public bool SeparateWindow
		{
			get
			{
				return _windowBehaviour == WindowBehaviour.Separate;
			}
			set
			{
				if (value == true)
				{
					_windowBehaviour = WindowBehaviour.Separate;
					this.Modified = true;
				}
			}
		}

		public override void Start()
		{
			_windowBehaviour = (WindowBehaviour)MonitorConfigurationSettings.Default.WindowBehaviour;
			base.Start();
		}

		public override void Stop()
		{
			base.Stop();
		}

		public override void Save()
		{
			MonitorConfigurationSettings.Default.WindowBehaviour = (int)_windowBehaviour;
			MonitorConfigurationSettings.Default.Save();
		}
	}
}