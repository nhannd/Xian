using ClearCanvas.Desktop;
using ClearCanvas.Common;
using System;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// The most basic of Linear Luts.  The <see cref="WindowWidth"/> and <see cref="WindowCenter"/> can be
	/// directly set/manipulated.
	/// </summary>
	public sealed class BasicVoiLutLinear : VoiLutLinearBase, IBasicVoiLutLinear
	{
		private class WindowLevelMemento : IMemento, IEquatable<WindowLevelMemento>
		{
			public readonly double WindowWidth;
			public readonly double WindowCenter;

			public WindowLevelMemento(double windowWidth, double windowCenter)
			{
				WindowWidth = windowWidth;
				WindowCenter = windowCenter;
			}

			public override bool Equals(object obj)
			{
				if (obj == this)
					return true;

				if (obj is WindowLevelMemento)
					return this.Equals((WindowLevelMemento) obj);

				return false;
			}

			#region IEquatable<WindowLevelMemento> Members

			public bool Equals(WindowLevelMemento other)
			{
				return this.WindowWidth == other.WindowWidth && this.WindowCenter == other.WindowCenter;
			}

			#endregion
		}

		private double _windowWidth;
		private double _windowCenter;

		/// <summary>
		/// Constructor.  
		/// </summary>
		/// <remarks>
		/// Allows the initial Window Width and Window Center to be set.
		/// </remarks>
		/// <param name="windowWidth">the initial Window Width</param>
		/// <param name="windowCenter">the initial Window Center</param>
		public BasicVoiLutLinear(double windowWidth, double windowCenter)
			: base()
		{
			this.WindowWidth = windowWidth;
			this.WindowCenter = windowCenter;
		}

		/// <summary>
		/// Default Constructor.
		/// </summary>
		/// <remarks>
		/// The initial Window Width and Window Center are 1 and 0, respectively.
		/// </remarks>
		public BasicVoiLutLinear()
			: this(1, 0)
		{
		}

		/// <summary>
		/// Gets the Window Width.
		/// </summary>
		protected override double GetWindowWidth()
		{
			return this.WindowWidth;
		}

		/// <summary>
		/// Gets the Window Center.
		/// </summary>
		protected override double GetWindowCenter()
		{
			return this.WindowCenter;
		}

		/// <summary>
		/// Gets or sets the Window Width.
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
				base.OnLutChanged();
			}
		}

		/// <summary>
		/// Gets or sets the Window Center.
		/// </summary>
		public double WindowCenter
		{
			get { return _windowCenter; }
			set
			{
				if (value == _windowCenter)
					return;

				_windowCenter = value;
				base.OnLutChanged();
			}
		}

		/// <summary>
		/// Gets an abbreviated description of the Lut.
		/// </summary>
		/// <returns>a string briefly describing the Lut</returns>
		public override string GetDescription()
		{
			return String.Format(SR.FormatDescriptionBasicLinearLut, WindowWidth, WindowCenter);
		}

		/// <summary>
		/// Creates a memento, through which the Lut's state can be restored.
		/// </summary>
		/// <returns>an <see cref="IMemento"/></returns>
		public override IMemento CreateMemento()
		{
			return new WindowLevelMemento(this.WindowWidth, this.WindowCenter);
		}

		/// <summary>
		/// Sets the Lut's state from the input <see cref="IMemento"/>.
		/// </summary>
		/// <exception cref="InvalidCastException">thrown when the memento is unrecognized.  This should never happen.</exception>
		/// <param name="memento">The memento to use to restore a previous state.</param>
		public override void SetMemento(IMemento memento)
		{
			WindowLevelMemento windowLevelMemento = memento as WindowLevelMemento;
			Platform.CheckForInvalidCast(windowLevelMemento, "memento", typeof(WindowLevelMemento).Name);

			this.WindowWidth = windowLevelMemento.WindowWidth;
			this.WindowCenter = windowLevelMemento.WindowCenter;
		}
	}
}
