#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Externals.Config;

namespace ClearCanvas.ImageViewer.Externals.CoreTools
{
	public abstract class ExternalToolBase : ImageViewerTool
	{
		private ExternalsConfigurationSettings _settings;

		public override void Initialize()
		{
			base.Initialize();

			_settings = ExternalsConfigurationSettings.Default;
			_settings.ExternalsChanged += Settings_ExternalsChanged;
		}

		protected override void Dispose(bool disposing)
		{
			_settings.ExternalsChanged -= Settings_ExternalsChanged;
			_settings = null;

			base.Dispose(disposing);
		}

		protected virtual void OnExternalsChanged(EventArgs e) {}

		private void Settings_ExternalsChanged(object sender, EventArgs e)
		{
			this.OnExternalsChanged(e);
		}
	}
}