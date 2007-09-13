using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Luts
{
	internal sealed class PresetVoiLutLinear : CalculatedVoiLutLinear
	{
		public sealed class PresetVoiLutLinearParameters : IMemento, IEquatable<PresetVoiLutLinearParameters>
		{
			public readonly string Name;
			public readonly double WindowWidth;
			public readonly double WindowCenter;

			public PresetVoiLutLinearParameters()
			{
			}

			public PresetVoiLutLinearParameters(string name, double windowWidth, double windowCenter)
			{
				Platform.CheckForEmptyString(name, "name");

				if (double.IsNaN(windowWidth) || windowWidth < 1)
					throw new ArgumentException(String.Format(SR.ExceptionFormatWindowWidthInvalid, windowWidth));
				if (double.IsNaN(windowCenter))
					throw new ArgumentException(SR.ExceptionWindowCenterInvalid);
				if (String.IsNullOrEmpty(name))
					throw new ArgumentException(SR.ExceptionPresetNameCannotBeEmpty);

				this.Name = name;
				this.WindowWidth = windowWidth;
				this.WindowCenter = windowCenter;
			}

			public override bool Equals(object obj)
			{
				if (obj == this)
					return true;

				if (obj is PresetVoiLutLinearParameters)
					return this.Equals((PresetVoiLutLinearParameters) obj);

				return false;
			}

			#region IEquatable<PresetVoiLutLinearParameters> Members

			public bool Equals(PresetVoiLutLinearParameters other)
			{
				return Name == other.Name && WindowWidth == other.WindowWidth && WindowCenter == other.WindowCenter;
			}

			#endregion
		}

		private PresetVoiLutLinearParameters _parameters;

		public PresetVoiLutLinear(PresetVoiLutLinearParameters parameters)
		{
			_parameters = parameters;
		}

		#region Public Properties

		public PresetVoiLutLinearParameters Parameters
		{
			get { return _parameters; }
			set
			{
				Platform.CheckForNullReference(value, "Parameters");

				if (_parameters.Equals(value))
					return;

				_parameters = value;
				base.OnLutChanged();
			}
		}

		public string Name
		{
			get { return _parameters.Name; }
		}

		public override double WindowWidth
		{
			get { return _parameters.WindowWidth; }
		}

		public override double WindowCenter
		{
			get { return _parameters.WindowCenter; }
		}

		#endregion

		#region Public Methods

		public override string GetDescription()
		{
			return String.Format(SR.FormatDescriptionPresetLinearLut, WindowWidth, WindowCenter, this.Name);
		}

		public override IMemento CreateMemento()
		{
			return _parameters;
		}

		public override void SetMemento(IMemento memento)
		{
			PresetVoiLutLinearParameters parameters = memento as PresetVoiLutLinearParameters;
			Platform.CheckForInvalidCast(parameters, "memento", typeof (PresetVoiLutLinearParameters).Name);
			this.Parameters = parameters;
		}

		#endregion
	}
}