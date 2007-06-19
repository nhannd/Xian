using System;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Tools.Standard.LutPresets
{
	internal sealed class VoiLutLinearPresetApplicator : IVoiLutPresetApplicator
	{
		private int _windowWidth;
		private int _windowCenter;
		private string _name;

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

		public string Name
		{
			get	{ return _name; }
			set	{ _name = value; }
		}

		public bool AppliesTo(IPresentationImage image)
		{
			return (image is IVoiLutManagerProvider && image is IVOILUTLinearProvider);
		}

		public void Apply(IPresentationImage image)
		{
			Platform.CheckForNullReference(image, "image");

			IVoiLutManagerProvider provider = image as IVoiLutManagerProvider;
			Platform.CheckForInvalidCast(provider, "image", "provider");
			Platform.CheckForNullReference(provider.VoiLutManager, "provider.VoiLutManager");

			IVOILUTLinearProvider lutProvider = image as IVOILUTLinearProvider;
			Platform.CheckForInvalidCast(lutProvider, "image", "IVOILUTLinearProvider");
			
			VoiLutOperationApplicator applicator = new VoiLutOperationApplicator(image);
			UndoableCommand command = new UndoableCommand(applicator);
			command.Name = SR.CommandWindowLevelPreset;
			command.BeginState = applicator.CreateMemento();

			PresetVoiLutLinearState state = new PresetVoiLutLinearState(_name, this.WindowWidth, this.WindowCenter);
			IStatefulVoiLutLinear statefulLut = lutProvider.VoiLutLinear as IStatefulVoiLutLinear;
			if (statefulLut == null)
			{
			    statefulLut = provider.VoiLutManager.CreateStatefulLut(state);
			    provider.VoiLutManager.InstallVoiLut(statefulLut);
			}
			else
			{
			    statefulLut.State = state;
			}

			applicator.ApplyToLinkedImages();
			command.EndState = applicator.CreateMemento();

			if (!command.EndState.Equals(command.BeginState))
				image.ImageViewer.CommandHistory.AddCommand(command);
		}

		#endregion
	}
}
