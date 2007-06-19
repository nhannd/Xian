using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ClearCanvas.ImageViewer.Tools.Standard.LutPresets
{
	public interface IVoiLutPresetApplicator
	{
		/// <summary>
		/// Set by the 'owner preset' object (e.g. <see cref="VoiLutPreset"/>).  Should not be set internally.
		/// </summary>
		string Name { get; set; }

		bool AppliesTo(IPresentationImage image);
		void Apply(IPresentationImage image);
	}
}
