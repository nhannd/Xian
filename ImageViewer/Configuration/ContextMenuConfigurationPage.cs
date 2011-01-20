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

namespace ClearCanvas.ImageViewer.Configuration
{
	/// <summary>
	/// Extension point for views onto <see cref="ContextMenuConfigurationPage"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class ContextMenuConfigurationPageViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// ContextMenuConfigurationPage class.
	/// </summary>
	[AssociateView(typeof(ContextMenuConfigurationPageViewExtensionPoint))]
	public class ContextMenuConfigurationPage : ConfigurationApplicationComponent
	{
		private bool _suppressContextMenuTools;

		internal ContextMenuConfigurationPage()
		{
		}

		public bool SuppressContextMenuTools
		{
			get { return _suppressContextMenuTools; }
			set
			{
				if (value == _suppressContextMenuTools)
					return;

				_suppressContextMenuTools = value;
				base.Modified = true;
				NotifyPropertyChanged("SuppressContextMenuTools");
			}
		}

		public override void Start()
		{
			SuppressContextMenuTools = ToolSettings.Default.ApplyActionFilterRules;
			base.Start();
		}

		public override void Save()
		{
			ToolSettings.Default.ApplyActionFilterRules = _suppressContextMenuTools;
			ToolSettings.Default.Save();
		}
	}
}
