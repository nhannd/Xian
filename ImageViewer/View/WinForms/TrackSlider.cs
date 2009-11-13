#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.View.WinForms
{
	public partial class TrackSlider : Control, INotifyPropertyChanged
	{
		public enum UserAction
		{
			None,
			ClickArrow,
			ClickTrack,
			DragThumb
		}

		public class ValueChangedEventArgs : EventArgs
		{
			internal ValueChangedEventArgs()
			{}

			/// <summary>
			/// Gets the <see cref="UserAction"/> that caused the value to change.
			/// </summary>
			public UserAction UserAction { get; internal set; }
		}

		private event PropertyChangedEventHandler _propertyChanged;
		private event EventHandler<ValueChangedEventArgs> _valueChanged;

		private ITrackSliderVisualStyle _visualStyle;
		private Orientation _orientation = Orientation.Vertical;
		private bool _autoHide = false;
		private int _minimumAlpha = 0;
		private int _value = 0;
		private int _maximumValue = 100;
		private int _minimumValue = 0;
		private int _increment = 10;

		private ITrackSliderVisualStyleReference _style;
		private TrackBar _trackBar;
		private Timer _timer;
		private Size _paddingOffset;

		public TrackSlider() : base()
		{
			base.SetStyle(ControlStyles.UserPaint, true);
			base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			base.SetStyle(ControlStyles.OptimizedDoubleBuffer, false);
			base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

			base.BackColor = Color.Transparent;
			base.DoubleBuffered = true;

			_visualStyle = new StandardTrackSliderVisualStyle();
			_visualStyle.PropertyChanged += VisualStyle_PropertyChanged;
			_style = _visualStyle.CreateReference();
			_trackBar = new TrackBar(this);

			if (!base.DesignMode)
			{
				_timer = new Timer();
				_timer.Interval = 10;
				_timer.Tick += OnTimerTick;
				_timer.Start();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_timer != null)
				{
					_timer.Stop();
					_timer.Tick -= OnTimerTick;
					_timer.Dispose();
					_timer = null;
				}

				if (_trackBar != null)
				{
					_trackBar.Dispose();
					_trackBar = null;
				}

				if (_style != null)
				{
					_style.Dispose();
					_style = null;
				}

				if (_visualStyle != null)
				{
					_visualStyle.PropertyChanged -= VisualStyle_PropertyChanged;
					_visualStyle = null;
				}
			}
			base.Dispose(disposing);
		}

		#region AutoHide Property

		[Bindable(true)]
		[Category("Behavior")]
		[DefaultValue(false)]
		[Description("Indicates whether or not the control should automatically fade out when it is not active.")]
		public bool AutoHide
		{
			get { return _autoHide; }
			set
			{
				if (_autoHide != value)
				{
					if (!value)
						_autoHideAlpha = 255;
					_autoHide = value;
					this.Invalidate();
					this.OnPropertyChanged(new PropertyChangedEventArgs("AutoHide"));
				}
			}
		}

		protected void ResetAutoHide()
		{
			this.AutoHide = false;
		}

		#endregion

		#region Orientation Property

		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue(typeof (Orientation), "Vertical")]
		[Description("Specifies the orientation of the control.")]
		public Orientation Orientation
		{
			get { return _orientation; }
			set
			{
				if (_orientation != value)
				{
					_orientation = value;
					_trackBar.Invalidate();
					this.Invalidate();
					this.OnPropertyChanged(new PropertyChangedEventArgs("Orientation"));
				}
			}
		}

		public void ResetOrientation()
		{
			this.Orientation = Orientation.Vertical;
		}

		#endregion

		#region Value Property

		[Bindable(true)]
		[Category("Behavior")]
		[DefaultValue(0)]
		[Description("The current value of the thumb slider.")]
		public int Value
		{
			get { return _value; }
			set { SetValue(value, UserAction.None); }
		}

		public void ResetValue()
		{
			this.Value = Math.Min(_maximumValue, Math.Max(_minimumValue, 0));
		}

		protected virtual void OnValueChanged(ValueChangedEventArgs e)
		{
			this.OnPropertyChanged(new PropertyChangedEventArgs("Value"));

			if (_valueChanged != null)
				_valueChanged(this, e);
		}

		public event EventHandler<ValueChangedEventArgs> ValueChanged
		{
			add { _valueChanged += value; }
			remove { _valueChanged -= value; }
		}

		#endregion

		#region MinimumValue Property

		[Bindable(true)]
		[Category("Behavior")]
		[DefaultValue(0)]
		[Description("The lower bound of the range for Value.")]
		public int MinimumValue
		{
			get { return _minimumValue; }
			set { SetValueRange(value, _maximumValue); }
		}

		#endregion

		#region MaximumValue Property

		[Bindable(true)]
		[Category("Behavior")]
		[DefaultValue(100)]
		[Description("The upper bound of the range for Value.")]
		public int MaximumValue
		{
			get { return _maximumValue; }
			set { SetValueRange(_minimumValue, value); }
		}

		#endregion

		#region Increment Property

		[Bindable(true)]
		[Category("Behavior")]
		[DefaultValue(10)]
		[Description("The Value change increment when the user clicks on the slider's arrows.")]
		public int Increment
		{
			get { return _increment; }
			set
			{
				if (_increment != value)
				{
					_increment = value;
					this.OnPropertyChanged(new PropertyChangedEventArgs("Increment"));
				}
			}
		}

		private void ResetIncrement()
		{
			this.Increment = 10;
		}

		#endregion

		#region Interval Property

		[Bindable(true)]
		[Category("Behavior")]
		[DefaultValue(10)]
		[Description("The interval in milliseconds at which control fading and click events are processed.")]
		public int Interval
		{
			get { return _timer.Interval; }
			set
			{
				if (_timer.Interval != value)
				{
					_timer.Interval = value;
					this.OnPropertyChanged(new PropertyChangedEventArgs("Interval"));
				}
			}
		}

		private void ResetInterval()
		{
			this.Interval = 10;
		}

		#endregion

		#region MinimumAlpha Property

		[Bindable(true)]
		[Category("Behavior")]
		[DefaultValue(0)]
		[Description("Specifies the minimum alpha level to which the control fades.")]
		public int MinimumAlpha
		{
			get { return _minimumAlpha; }
			set
			{
				if (_minimumAlpha != value)
				{
					_minimumAlpha = value;
					if (this.AutoHide)
						this.Invalidate();
					this.OnPropertyChanged(new PropertyChangedEventArgs("MinimumAlpha"));
				}
			}
		}

		#endregion

		#region VisualStyle Property

		[Category("Appearance")]
		[Description("Specifies the appearance style of the track slider.")]
		[TypeConverter(typeof (ExpandableObjectConverter))]
		public ITrackSliderVisualStyle VisualStyle
		{
			get { return _visualStyle; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");

				if (_visualStyle != value)
				{
					if (_visualStyle != null)
						_visualStyle.PropertyChanged -= VisualStyle_PropertyChanged;

					_visualStyle = value;
					this.ReferencedStyle = _visualStyle.CreateReference();
					_visualStyle.PropertyChanged += VisualStyle_PropertyChanged;

					_trackBar.Invalidate();
					this.Invalidate();
					this.OnPropertyChanged(new PropertyChangedEventArgs("VisualStyle"));
				}
			}
		}

		private void ResetVisualStyle()
		{
			this.VisualStyle = new StandardTrackSliderVisualStyle();
		}

		private bool ShouldSerializeVisualStyle()
		{
			return !new StandardTrackSliderVisualStyle().Equals(this.VisualStyle);
		}

		#endregion

		#region Hidden Designer Properties

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override Font Font
		{
			get { return base.Font; }
			set { base.Font = value; }
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override Color ForeColor
		{
			get { return base.ForeColor; }
			set { base.ForeColor = value; }
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string Text
		{
			get { return base.Text; }
			set { base.Text = value; }
		}

		#endregion

		public void SetValueRange(int minimumValue, int maximumValue)
		{
			this.SetValueAndRange(minimumValue, minimumValue, maximumValue, UserAction.None);
		}

		public void SetValueAndRange(int value, int minimumValue, int maximumValue)
		{
			SetValueAndRange(value, minimumValue, maximumValue, UserAction.None);
		}

		protected void SetValue(int value, UserAction userAction)
		{
			SetValueAndRange(value, _minimumValue, _maximumValue, userAction);
		}

		protected void SetValueAndRange(int value, int minimumValue, int maximumValue, UserAction userAction)
		{
			if (value < minimumValue || value > maximumValue)
			{
				if (!this.DesignMode)
					throw new ArgumentOutOfRangeException("value", "value");

				value = Math.Min(maximumValue, Math.Max(minimumValue, value));
			}

			bool minimumChanged = _minimumValue != minimumValue;
			bool maximumChanged = _maximumValue != maximumValue;
			bool valueChanged = _value != value;

			if (minimumChanged || maximumChanged || valueChanged)
			{
				_minimumValue = minimumValue;
				_maximumValue = maximumValue;
				_value = value;

				_trackBar.Invalidate();
				this.Invalidate();
				if (minimumChanged)
					this.OnPropertyChanged(new PropertyChangedEventArgs("MinimumValue"));
				if (maximumChanged)
					this.OnPropertyChanged(new PropertyChangedEventArgs("MaximumValue"));

				if (valueChanged)
					this.OnValueChanged(new ValueChangedEventArgs(){UserAction = userAction});
			}
		}

		protected ITrackSliderVisualStyleReference ReferencedStyle
		{
			get { return _style; }
			private set
			{
				if (_style != value)
				{
					if (_style != null)
						_style.Dispose();
					_style = value;
				}
			}
		}

		protected Size PaddingOffset
		{
			get { return _paddingOffset; }
		}

		public event PropertyChangedEventHandler PropertyChanged
		{
			add { _propertyChanged += value; }
			remove { _propertyChanged -= value; }
		}

		private void VisualStyle_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			this.OnVisualStylePropertyChanged(e);
		}

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (_propertyChanged != null)
				_propertyChanged(this, e);
		}

		protected virtual void OnVisualStylePropertyChanged(PropertyChangedEventArgs e)
		{
			_trackBar.Invalidate();
			this.Invalidate();
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			// the base PaintBackground will paint the back color or parent background for us
			base.OnPaintBackground(e);

			// if this control is fully opaque, or the parent control has no other children, then the built-in WinForms "simulated transparency" will suffice.
			// if not, we need to trick any sibling controls under us in the Z-Order to paint themselves to our background.
			if (this.BackColor.A < 255 && this.Parent != null && this.Parent.Controls.Count > 1)
			{
				// save the coordinate space of the graphics context (this control's own coordinate space)
				GraphicsContainer thisGraphicSpace = e.Graphics.BeginContainer();
				try
				{
					// shift the graphics context into the parent control's coordinate space
					Rectangle clipRectangleParent = this.RectangleToScreen(e.ClipRectangle);
					e.Graphics.TranslateTransform(-this.Left, -this.Top);

					// only need to handle controls that are layered behind us in the Z-order
					int startingZIndex = this.Parent.Controls.IndexOf(this);
					for (int n = startingZIndex + 1; n < this.Parent.Controls.Count; n++)
					{
						Control sibling = this.Parent.Controls[n];

						Rectangle clipRectangleSibling = sibling.ClientRectangle;
						clipRectangleSibling.Intersect(sibling.RectangleToClient(clipRectangleParent));
						if (clipRectangleSibling.IsEmpty)
							continue;

						// save the coordinate space of the graphics context (the parent control's coordinate space)
						GraphicsContainer parentGraphicSpace = e.Graphics.BeginContainer();
						try
						{
							// shift the graphics context into the sibling control's coordinate space
							e.Graphics.TranslateTransform(sibling.Left, sibling.Top);

							// ideally, you wouldn't have to paint the controls to a secondary buffer and then paint it on to our graphics context.
							// we've provided the correct clipping rectangle, which is only a subsection of the entire graphics context.
							// unfortunately, if the kind of sibling controls are the ones that ignore the clipping rectangle and paint onto the entire
							// graphics context, then we have no choice but to let the control render all of itself onto a secondary buffer, and then
							// extract just the part we want. it is slower, but it is currently the only way to make it work.
							using (Bitmap temp = new Bitmap(sibling.Width, sibling.Height))
							{
								PaintEventArgs paintEventArgs = new PaintEventArgs(System.Drawing.Graphics.FromImage(temp), clipRectangleSibling);
								this.InvokePaintBackground(sibling, paintEventArgs);
								this.InvokePaint(sibling, paintEventArgs);
								e.Graphics.DrawImage(temp, clipRectangleSibling, clipRectangleSibling, GraphicsUnit.Pixel);
							}
						}
						finally
						{
							// restore the parent control's coordinate space
							e.Graphics.EndContainer(parentGraphicSpace);
						}
					}
				}
				finally
				{
					// restore our own coordinate space
					e.Graphics.EndContainer(thisGraphicSpace);
				}
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			_trackBar.DrawTrackBar(e.Graphics, _autoHideAlpha);
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			_trackBar.UpdateClientBounds(this.ClientRectangle, this.Padding);
		}

		protected override void OnPaddingChanged(EventArgs e)
		{
			base.OnPaddingChanged(e);
			_paddingOffset = new Size(this.Padding.Left, this.Padding.Top);
			_trackBar.UpdateClientBounds(this.ClientRectangle, this.Padding);
		}
	}
}