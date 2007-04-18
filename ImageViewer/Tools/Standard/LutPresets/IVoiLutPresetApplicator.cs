using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ClearCanvas.ImageViewer.Tools.Standard.LutPresets
{
	public interface IVoiLutPresetApplicator
	{
		bool AppliesTo(IPresentationImage image);
		void Apply(IPresentationImage image);
	}
}
