using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using System.Xml;

namespace ClearCanvas.ImageViewer.Tools.Standard.LutPresets
{
	[ExtensionOf(typeof(VoiLutPresetApplicatorFactoryExtensionPoint))]
	public sealed class VoiLutLinearPresetApplicatorFactory : IVoiLutPresetApplicatorFactory
	{
		internal static readonly string InternalFactoryKey = "VoiLutLinear";

		public VoiLutLinearPresetApplicatorFactory()
		{ 
		}

		public static VoiLutPresetApplicatorConfiguration CreateVoiLutLinearApplicatorConfiguration(int windowWidth, int windowCenter)
		{
			Dictionary<string, string> _configurationValues = new Dictionary<string, string>();
			_configurationValues.Add("WindowWidth", windowWidth.ToString());
			_configurationValues.Add("WindowCenter", windowCenter.ToString());
			return new VoiLutPresetApplicatorConfiguration(InternalFactoryKey, _configurationValues);
		}

		#region IVoiLutPresetApplicatorFactory Members

		public string FactoryKey
		{
			get { return InternalFactoryKey; }
		}

		public IVoiLutPresetApplicator CreateNewApplicator(IDictionary<string, string> configurationValues)
		{
			if (!configurationValues.ContainsKey("WindowWidth"))
				throw new ArgumentException(SR.ExceptionNoWindowWidthHasBeenSpecified);

			if (!configurationValues.ContainsKey("WindowCenter"))
				throw new ArgumentException(SR.ExceptionNoWindowCenterHasBeenSpecified);

			int windowWidth = 1, windowCenter = 0;
			int.TryParse(configurationValues["WindowWidth"], out windowWidth);
			int.TryParse(configurationValues["WindowCenter"], out windowCenter);

			return new VoiLutLinearPresetApplicator(windowWidth, windowCenter);
		}

		#endregion
	}
}
