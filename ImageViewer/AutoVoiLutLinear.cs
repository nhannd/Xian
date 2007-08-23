using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.Dicom;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	public class AutoVoiLutLinearCreationParameters : VoiLutCreationParameters
	{
		private ImageSop _imageSop;

		public AutoVoiLutLinearCreationParameters(ImageSop imageSop, uint index)
			: base(AutoVoiLutLinearFactory.FactoryName)
		{
			Platform.CheckForNullReference(imageSop, "imageSop");
			if (imageSop.WindowCenterAndWidth.Length == 0 || index > imageSop.WindowCenterAndWidth.Length - 1)
				throw new ArgumentException("The index exceeds the available number of window/level values.");

			_imageSop = imageSop;
			this.Index = index;
		}

		public AutoVoiLutLinearCreationParameters(ImageSop imageSop)
			: this(imageSop, 0)
		{
		}

		internal ImageSop ImageSop
		{
			get { return _imageSop; }
		}

		public uint Index
		{
			get { return (uint)this["Index"]; }
			set { this["Index"] = value; }
		}

		public override string GetKey()
		{
			return AutoVoiLutLinearFactory.GetKey(_imageSop, this.Index, this.MinInputValue, this.MaxInputValue);
		}
	}

	[ExtensionOf(typeof(VoiLutFactoryExtensionPoint))]
	public class AutoVoiLutLinearFactory : IVoiLutFactory
	{
		internal static readonly string FactoryName = "AutoLinear";

		public AutoVoiLutLinearFactory()
		{ 
		}

		#region ILutFactory<IVoiLut,VoiLutCreationParameters> Members

		public string Name
		{
			get { return FactoryName; }
		}

		public IVoiLut Create(VoiLutCreationParameters creationParameters)
		{
			AutoVoiLutLinearCreationParameters parameters = creationParameters as AutoVoiLutLinearCreationParameters;
			Platform.CheckForInvalidCast(parameters, "creationParameters", typeof(AutoVoiLutLinearCreationParameters).Name);

			return new AutoVoiLutLinear(parameters);
		}

		#endregion

		internal static string GetKey(ImageSop imageSop, uint index, int minimumInputValue, int maximumInputValue)
		{
			Window window = imageSop.WindowCenterAndWidth[index];
			return String.Format("Auto Linear: MinIn={0}, MaxIn={1}, WW={2}, WC={3}", minimumInputValue, maximumInputValue, window.Width, window.Center);
		}
	}

	public interface IAutoVoiLut : IVoiLut
	{
		uint Index { get; }
		uint MaximumIndex { get; }
	}

	internal class AutoVoiLutLinear : CalculatedVoiLutLinear, IAutoVoiLut
	{
		private ImageSop _imageSop;
		private uint _index;

		public AutoVoiLutLinear(AutoVoiLutLinearCreationParameters creationParameters)
			: base(creationParameters.MinInputValue, creationParameters.MaxInputValue)
		{
			_imageSop = creationParameters.ImageSop;
			this.Index = creationParameters.Index;
		}
		
		private Window[] WindowCenterAndWidth
		{
			get { return _imageSop.WindowCenterAndWidth; }
		}

		#region IAutoVoiLut Members

		public uint Index
		{
			get { return _index; }
			set
			{
				uint index = Math.Min((uint)this.WindowCenterAndWidth.Length, value);
				if (index == _index)
					return;

				_index = index;
				base.OnLutChanged();
			}
		}
		
		public uint MaximumIndex
		{
			get { return (uint)this.WindowCenterAndWidth.Length - 1; }
		}

		#endregion

		public override double WindowWidth
		{
			get 
			{
				return this.WindowCenterAndWidth[_index].Width;
			}
		}

		public override double WindowCenter
		{
			get
			{
				return this.WindowCenterAndWidth[_index].Center;
			}
		}

		public override LutCreationParameters GetCreationParametersMemento()
		{
			return new AutoVoiLutLinearCreationParameters(_imageSop, _index);
		}

		public override bool TrySetCreationParametersMemento(LutCreationParameters creationParameters)
		{
			AutoVoiLutLinearCreationParameters parameters = creationParameters as AutoVoiLutLinearCreationParameters;
			if (parameters == null)
				return false;

			this.Index = parameters.Index;
			return true;
		}

		public override string GetKey()
		{
			return AutoVoiLutLinearFactory.GetKey(_imageSop, _index, this.MinInputValue, this.MaxInputValue);
		}
	}
}
