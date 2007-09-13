using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Luts
{
	public interface IAutoVoiLutLinear : IVoiLutLinear
	{
		void ApplyNext();
	}

	internal sealed class AutoVoiLutLinear : CalculatedVoiLutLinear, IAutoVoiLutLinear
	{
		private class AutoVoiLutLinearMemento : IMemento, IEquatable<AutoVoiLutLinearMemento>
		{
			public readonly uint Index;

			public AutoVoiLutLinearMemento(uint index)
			{
				this.Index = index;
			}

			public override bool Equals(object obj)
			{
				if (obj == this)
					return true;

				if (obj is AutoVoiLutLinearMemento)
					return this.Equals((AutoVoiLutLinearMemento) obj);

				return false;	
			}

			#region IEquatable<AutoVoiLutLinearMemento> Members

			public bool Equals(AutoVoiLutLinearMemento other)
			{
				return this.Index == other.Index;
			}

			#endregion
		}

		private readonly ImageSop _imageSop;
		private uint _index;

		public AutoVoiLutLinear(ImageSop imageSop)
		{
			_imageSop = imageSop;
			_index = 0;
		}

		#region Private Properties/Methods

		private Window[] WindowCenterAndWidth
		{
			get { return _imageSop.WindowCenterAndWidth; }
		}

		private void SetIndex(uint newIndex)
		{
			uint lastIndex = _index;
			_index = newIndex;
			if (_index >= this.WindowCenterAndWidth.Length)
				_index = 0;

			if (lastIndex != _index)
				base.OnLutChanged();
		}

		#endregion

		#region IVoiLutLinear Members

		public override double WindowWidth
		{
			get { return this.WindowCenterAndWidth[_index].Width; }
		}

		public override double WindowCenter
		{
			get { return this.WindowCenterAndWidth[_index].Center; }
		}

		#endregion

		#region Public Methods

		#region IAutoVoiLut Members

		public void ApplyNext()
		{
			SetIndex(_index + 1);
		}

		#endregion

		public override string GetDescription()
		{
			return String.Format(SR.FormatDescriptionAutoLinearLut, WindowWidth, WindowCenter);
		}

		public override IMemento CreateMemento()
		{
			return new AutoVoiLutLinearMemento(_index);
		}

		public override void SetMemento(IMemento memento)
		{
			AutoVoiLutLinearMemento autoMemento = memento as AutoVoiLutLinearMemento;
			Platform.CheckForInvalidCast(autoMemento, "memento", typeof(AutoVoiLutLinearMemento).Name);
			this.SetIndex(autoMemento.Index);
		}

		#endregion
	}
}