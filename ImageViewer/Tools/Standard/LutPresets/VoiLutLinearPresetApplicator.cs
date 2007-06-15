using System;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Tools.Standard.LutPresets
{
	internal sealed class VoiLutLinearPresetApplicator : IVoiLutPresetApplicator
	{
		private int _windowWidth;
		private int _windowCenter;

		public VoiLutLinearPresetApplicator(int windowWidth, int windowCenter)
		{
			_windowWidth = windowWidth;
			_windowCenter = windowCenter;
		}

		public int WindowWidth
		{
			get { return _windowWidth; }
		}

		public int WindowCenter
		{
			get { return _windowCenter; }
		}

		#region IVoiLutPresetApplicator Members

		public bool AppliesTo(IPresentationImage image)
		{
			IVOILUTLinearProvider provider = image as IVOILUTLinearProvider;
			if (provider == null)
				return false;

			if (provider.VoiLutLinear == null)
				return false;

			return true;
		}

		public void Apply(IPresentationImage image)
		{
			Platform.CheckForNullReference(image, "image");

			IVOILUTLinearProvider provider = image as IVOILUTLinearProvider;
			Platform.CheckForInvalidCast(provider, "image", "IVOILUTLinearProvider");
			Platform.CheckForNullReference(provider.VoiLutLinear, "provider.VoiLutLinear");

			VoiLutOperationApplicator applicator = new VoiLutOperationApplicator(image);
			UndoableCommand command = new UndoableCommand(applicator);
			command.Name = SR.CommandWindowLevelPreset;
			command.BeginState = applicator.CreateMemento();

			provider.VoiLutLinear.WindowWidth = this.WindowWidth;
			provider.VoiLutLinear.WindowCenter = this.WindowCenter;

			applicator.ApplyToLinkedImages();
			command.EndState = applicator.CreateMemento();

			if (!command.EndState.Equals(command.BeginState))
				image.ImageViewer.CommandHistory.AddCommand(command);
		}

		#endregion
	}
}
