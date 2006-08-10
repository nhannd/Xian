using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace ClearCanvas.Controls.WinForms
{
	public partial class SplashScreen : Form
	{
		// Threading
		static SplashScreen _SplashForm = null;
		static Thread _Thread = null;

		// Fade in and out.
		private double _dblOpacityIncrement = .05;
		private double _dblOpacityDecrement = .08;

		// Status and progress bar
		static string ms_sStatus;

		static double _TotalTime = 0;

		public SplashScreen()
		{
			InitializeComponent();
			this._StatusLabel.ForeColor = Color.FromArgb(60, 150, 208);
			this.Opacity = .00;
			_Timer.Start();
			this.ClientSize = this.BackgroundImage.Size;
			Control.CheckForIllegalCrossThreadCalls = false;
		}

		// ************* Static Methods *************** //

		// A static method to create the thread and 
		// launch the SplashScreen.
		static public void ShowSplashScreen()
		{
			// Make sure it's only launched once.
			if (_SplashForm != null)
				return;

			_Thread = new Thread(new ThreadStart(SplashScreen.ShowForm));
            _Thread.IsBackground = true;
            _Thread.SetApartmentState(ApartmentState.STA);
			_Thread.Start();
		}

		// A property returning the splash screen instance
		static public SplashScreen SplashForm
		{
			get
			{
				return _SplashForm;
			}
		}

		// A private entry point for the thread.
		static private void ShowForm()
		{
			_SplashForm = new SplashScreen();
			Application.Run(_SplashForm);
		}

		// A static method to close the SplashScreen
		static public void CloseForm()
		{
			if (_SplashForm != null && _SplashForm.IsDisposed == false)
			{
				// Make it start going away.
				_SplashForm._dblOpacityIncrement = -_SplashForm._dblOpacityDecrement;
			}
			_Thread = null;	// we don't need these any more.
			//_SplashForm = null;
		}

		// A static method to set the status and update the reference.
		static public void SetStatus(string newStatus)
		{
			ms_sStatus = newStatus;
		}


		// ************ Private methods ************


		//********* Event Handlers ************


		// Close the form if they double click on it.
		private void SplashScreen_DoubleClick(object sender, System.EventArgs e)
		{
			CloseForm();
		}

		// Tick Event handler for the Timer control.  Handle fade in and fade out.  Also
		// handle the smoothed progress bar.
		private void _Timer_Tick(object sender, EventArgs e)
		{
			_StatusLabel.Text = ms_sStatus;

			if (_dblOpacityIncrement > 0)
			{
				if (this.Opacity < 1)
					this.Opacity += _dblOpacityIncrement;

				//fade the form after 5 seconds...loading with native assemblies so hardly take any time at all...
				_TotalTime += _Timer.Interval;
				if (_TotalTime > 5000)
					CloseForm();
			}
			else
			{
				if (this.Opacity > 0)
					this.Opacity += _dblOpacityIncrement;
				else
				{
					this.Close();
					Debug.WriteLine("Called this.Close()");
				}
			}
		}
	}
}