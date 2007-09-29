using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Applicators
{
	[ExtensionOf(typeof(InitialVoiLutProviderExtensionPoint))]
	public sealed class InitialVoiLutProvider : IInitialVoiLutProvider
	{
		public InitialVoiLutProvider()
		{
		}

		#region IInitialVoiLutProvider Members

		public IComposableLut GetLut(IPresentationImage presentationImage)
		{
			// TODO: Eventually, this should use the IPresetVoiLutApplicatorFactory extensions and simply
			// try to apply each one that matches in order until one works.  The 'Auto' lut applicator would
			// be implemented as an applicator (with a corresponding factory) and treated just like the rest of the presets.
			// However, right now we don't want to add new functionality to 1.0, so the 'Initial Lut Provider' and the
			// 'Auto Lut Applicator' do basically the same thing.

			return AutoPresetVoiLutApplicatorHelper.GetInitialLut(presentationImage);
		}

		#endregion
	}
}