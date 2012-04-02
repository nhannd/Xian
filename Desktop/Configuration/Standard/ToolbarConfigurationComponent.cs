#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Globalization;
using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Configuration.Standard
{
	[ExtensionPoint]
	public sealed class ToolbarConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (ToolbarConfigurationComponentViewExtensionPoint))]
	public sealed class ToolbarConfigurationComponent : ConfigurationApplicationComponent
	{
		private bool _wrap;
		private IconSize _iconSize;

		public bool Wrap
		{
			get { return _wrap; }
			set
			{
				if (_wrap != value)
				{
					_wrap = value;
					base.NotifyPropertyChanged("Wrap");
					base.Modified = true;
				}
			}
		}

		public IconSize IconSize
		{
			get { return _iconSize; }
			set
			{
				if (_iconSize != value)
				{
					_iconSize = value;
					base.NotifyPropertyChanged("IconSize");
					base.Modified = true;
				}
			}
		}

		public override void Start()
		{
			base.Start();

			_wrap = ToolStripSettings.Default.WrapLongToolstrips;
			_iconSize = ToolStripSettings.Default.IconSize;
		}

		public override void Save()
		{
			var previousCulture = Thread.CurrentThread.CurrentUICulture;
			Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
			try
			{
				ToolStripSettings.Default.WrapLongToolstrips = _wrap;
				ToolStripSettings.Default.IconSize = _iconSize;
				ToolStripSettings.Default.Save();
			}
			finally
			{
				Thread.CurrentThread.CurrentUICulture = previousCulture;
			}
		}
	}
}