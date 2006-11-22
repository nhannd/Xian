using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Tools.Volume
{
	public class TissueSettingsEventArgs : CollectionEventArgs<TissueSettings>
	{
		public TissueSettingsEventArgs()
		{

		}

		public TissueSettingsEventArgs(TissueSettings tissueSettings)
		{
			Platform.CheckForNullReference(tissueSettings, "tissueSettings");

			base.Item = tissueSettings;
		}

		public TissueSettings TissueSettings { get { return base.Item; } }
	}
}
