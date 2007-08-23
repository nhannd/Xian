using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;


namespace ClearCanvas.ImageViewer.Imaging
{
	public class BasicVoiLutLinearCreationParameters : VoiLutCreationParameters
	{
		public BasicVoiLutLinearCreationParameters()
			: base(BasicVoiLutLinearFactory.FactoryName)
		{
			this.WindowCenter = 0;
			this.WindowWidth = 1;
		}

		public double WindowWidth
		{
			get { return (double)this["WindowWidth"]; }
			set { this["WindowWidth"] = value; }
		}

		public double WindowCenter
		{
			get { return (double)this["WindowCenter"]; }
			set { this["WindowCenter"] = value; }
		}

		public override string GetKey()
		{
			return BasicVoiLutLinearFactory.GetKey(this.MinInputValue, this.MaxInputValue, this.WindowWidth, this.WindowCenter);
		}
	}
	
	[ExtensionOf(typeof(VoiLutFactoryExtensionPoint))]
	public class BasicVoiLutLinearFactory : IVoiLutFactory
	{
		internal static readonly string FactoryName = "Linear";

		public BasicVoiLutLinearFactory()
		{
		}

		#region IVoiLutFactory Members

		public string Name 
		{
			get { return FactoryName; } 
		}

		public IVoiLut Create(VoiLutCreationParameters creationParameters)
		{
			BasicVoiLutLinearCreationParameters parameters = creationParameters as BasicVoiLutLinearCreationParameters;
			Platform.CheckForInvalidCast(parameters, "creationParameters", typeof(BasicVoiLutLinearCreationParameters).Name);

			return new BasicVoiLutLinear(parameters);
		}

		#endregion

		internal static string GetKey(int minInputValue, int maxInputValue, double windowWidth, double windowCenter)
		{
			return String.Format("{0}_{1}_{2:F2}_{3:F2}",
				minInputValue,
				maxInputValue,
				windowWidth,
				windowCenter);
		}
	}

	internal sealed class BasicVoiLutLinear : VoiLutLinearBase, IBasicVoiLutLinear
	{
		private double _windowWidth;
		private double _windowCenter;

		internal BasicVoiLutLinear(BasicVoiLutLinearCreationParameters creationParameters)
			: base(creationParameters.MinInputValue, creationParameters.MaxInputValue)
		{
			_windowWidth = creationParameters.WindowWidth;
			_windowCenter = creationParameters.WindowCenter;
		}

		protected override double GetWindowWidth()
		{
			return this.WindowWidth;
		}

		protected override double GetWindowCenter()
		{
			return this.WindowCenter;
		}

		/// <summary>
		/// Gets or sets the window width.
		/// </summary>
		public double WindowWidth
		{
			get { return _windowWidth; }
			set
			{
				if (value == _windowWidth)
					return;

				if (value < 1)
					value = 1;

				_windowWidth = value;
				base.Recalculate();
			}
		}

		/// <summary>
		/// Gets or sets the window center.
		/// </summary>
		public double WindowCenter
		{
			get { return _windowCenter; }
			set
			{
				if (value == _windowCenter)
					return;

				_windowCenter = value;
				base.Recalculate();
			}
		}

		public override LutCreationParameters GetCreationParametersMemento()
		{
			BasicVoiLutLinearCreationParameters creationParameters = new BasicVoiLutLinearCreationParameters();
			creationParameters.WindowWidth = this.WindowWidth;
			creationParameters.WindowCenter = this.WindowCenter;
			return creationParameters;
		}

		public override bool TrySetCreationParametersMemento(LutCreationParameters creationParameters)
		{
			BasicVoiLutLinearCreationParameters parameters = creationParameters as BasicVoiLutLinearCreationParameters;
			if (parameters == null)
				return false;

			this.WindowWidth = parameters.WindowWidth;
			this.WindowCenter = parameters.WindowCenter;

			return true;
		}

		public override string GetKey()
		{
			return BasicVoiLutLinearFactory.GetKey(this.MinInputValue, this.MaxInputValue, this.WindowWidth, this.WindowCenter);
		}
	}
}
