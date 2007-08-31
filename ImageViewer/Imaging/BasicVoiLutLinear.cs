
using ClearCanvas.Desktop;
using ClearCanvas.Common;
namespace ClearCanvas.ImageViewer.Imaging
{
	public sealed class BasicVoiLutLinear : VoiLutLinearBase, IBasicVoiLutLinear
	{
		private class WindowLevelMemento : IMemento
		{
			public readonly double WindowWidth;
			public readonly double WindowCenter;

			public WindowLevelMemento(double windowWidth, double windowCenter)
			{
				WindowWidth = windowWidth;
				WindowCenter = windowCenter;
			}
		}

		private double _windowWidth;
		private double _windowCenter;

		public BasicVoiLutLinear(double windowWidth, double windowCenter)
			: base()
		{
			this.WindowWidth = windowWidth;
			this.WindowCenter = windowCenter;
		}

		public BasicVoiLutLinear()
			: this(1, 0)
		{
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
				base.OnLutChanged();
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
				base.OnLutChanged();
			}
		}

		public override IMemento CreateMemento()
		{
			return new WindowLevelMemento(this.WindowWidth, this.WindowCenter);
		}

		public override void SetMemento(IMemento memento)
		{
			WindowLevelMemento windowLevelMemento = memento as WindowLevelMemento;
			Platform.CheckForInvalidCast(windowLevelMemento, "memento", typeof(WindowLevelMemento).Name);

			this.WindowWidth = windowLevelMemento.WindowWidth;
			this.WindowCenter = windowLevelMemento.WindowCenter;
		}
	}
}
