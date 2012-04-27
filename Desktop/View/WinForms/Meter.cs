#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Drawing;
using System.Windows.Forms.VisualStyles;
using System.Windows.Forms;
using System.ComponentModel;

namespace ClearCanvas.Desktop.View.WinForms
{
    public enum MeterFillState
    {
        Normal = VsStyles.ProgressBar.FillStates.PBFS_PARTIAL,
        Warning = VsStyles.ProgressBar.FillStates.PBFS_PAUSED,
        Error = VsStyles.ProgressBar.FillStates.PBFS_ERROR
    }

    /// <summary>
	/// 
	/// </summary>
	/// <remarks>
	/// See this page for more about "meters":
	/// http://msdn.microsoft.com/en-us/library/aa511486.aspx
	/// </remarks>
	public class Meter : Label
	{
		private int _value;
        private MeterFillState _fillState = MeterFillState.Normal;

		public Meter()
		{
			this.Size = new Size(160, 15);	// recommended defaults
			this.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		}

		protected override void OnCreateControl()
		{
			base.OnCreateControl();

			// the Label base class really wants to AutoSize itself, but that doesn't make sense for a meter
			base.AutoSize = false;
		}

		[DefaultValue(System.Drawing.ContentAlignment.MiddleCenter)]
		public new System.Drawing.ContentAlignment TextAlign
		{
			get { return base.TextAlign; }
			set { base.TextAlign = value; }
		}

		[Description("A number between 0 and 100 that specifies the percentage of the meter that is filled.")]
		[DefaultValue(0)]
		public int Value
		{
			get { return _value; }
			set
			{
				_value = value;
				Invalidate();
			}
		}

	    [Description("The fill state of the meter.")]
	    [DefaultValue(MeterFillState.Normal)]
        public MeterFillState FillState
	    {
            get { return _fillState; }
            set
            {
                _fillState = value;
                Invalidate();
            }
	    }

	    protected override void OnPaint(PaintEventArgs e)
		{
			var clientRect = this.ClientRectangle;
			var fillRect = clientRect;
			fillRect.Width = (int)(_value / 100.0 * clientRect.Width);
			try
			{
				// draw background
				var renderer = new VisualStyleRenderer(VisualStyleElement.ProgressBar.Bar.Normal);
				renderer.SetParameters(VsStyles.ProgressBar.Progress,
					VsStyles.ProgressBar.ProgressParts.PP_BAR,
					VsStyles.ProgressBar.FillStates.PBFS_NORMAL);
				renderer.DrawBackground(e.Graphics, this.ClientRectangle);

				// draw filled portion
				renderer.SetParameters(VsStyles.ProgressBar.Progress,
				   VsStyles.ProgressBar.ProgressParts.PP_FILL, (int)_fillState);
				renderer.DrawBackground(e.Graphics, fillRect);
			}
			catch(Exception)
			{
				// the VisualStyles stuff can fail when the OS is < Win7, in which case
				// draw a very basic progress bar manually
				e.Graphics.DrawRectangle(Pens.DarkGray, 0, 0, clientRect.Width - 1, clientRect.Height - 1);
				e.Graphics.FillRectangle(GetStateBrush(), 1, 1,
					Math.Min(fillRect.Width, clientRect.Width - 2), clientRect.Height - 2);
			}

			base.OnPaint(e);
		}

        private Brush GetStateBrush()
        {
            switch (_fillState)
            {
                case MeterFillState.Warning:
                    return Brushes.Gold;
                case MeterFillState.Error:
                    return Brushes.Red;
                default:
                    return Brushes.DodgerBlue;
            }
        }
	}
}
