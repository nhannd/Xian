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
using System.Reflection;

namespace ClearCanvas.Controls.WinForms
{
	public partial class SplashScreen : Form
	{
		// Threading
		static SplashScreen _splashForm = null;
		static Thread _thread = null;

		// Fade in and out.
		private double _opacityIncrement = .05;
		private double _opacityDecrement = .08;

		// Status and progress bar
		static string _status;

		static double _totalTime = 0;

		public SplashScreen()
		{
			InitializeComponent();
			SetVersion();
			this._statusLabel.ForeColor = Color.FromArgb(60, 150, 208);
			this.Opacity = .00;
			_timer.Start();
			this.ClientSize = this.BackgroundImage.Size;
			Control.CheckForIllegalCrossThreadCalls = false;
		}

		private void SetVersion()
		{
			string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			this._versionLabel.Text = String.Format("Version {0}", version);
		}

		// ************* Static Methods *************** //

		// A static method to create the thread and 
		// launch the SplashScreen.
		static public void ShowSplashScreen()
		{
			// Make sure it's only launched once.
			if (_splashForm != null)
				return;

			_thread = new Thread(new ThreadStart(SplashScreen.ShowForm));
            _thread.IsBackground = true;
            _thread.SetApartmentState(ApartmentState.STA);
			_thread.Start();
		}

		// A property returning the splash screen instance
		static public SplashScreen SplashForm
		{
			get
			{
				return _splashForm;
			}
		}

		// A private entry point for the thread.
		static private void ShowForm()
		{
			_splashForm = new SplashScreen();
			Application.Run(_splashForm);
		}

		// A static method to close the SplashScreen
		static public void CloseForm()
		{
			if (_splashForm != null && _splashForm.IsDisposed == false)
			{
				// Make it start going away.
				_splashForm._opacityIncrement = -_splashForm._opacityDecrement;
			}
			_thread = null;	// we don't need these any more.
			//_SplashForm = null;
		}

		// A static method to set the status and update the reference.
		static public void SetStatus(string newStatus)
		{
			_status = newStatus;
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
			_statusLabel.Text = _status;

			if (_opacityIncrement > 0)
			{
				if (this.Opacity < 1)
					this.Opacity += _opacityIncrement;

				//fade the form after 5 seconds...loading with native assemblies so hardly take any time at all...
				_totalTime += _timer.Interval;
				if (_totalTime > 5000)
					CloseForm();
			}
			else
			{
				if (this.Opacity > 0)
					this.Opacity += _opacityIncrement;
				else
				{
					this.Close();
					Debug.WriteLine("Called this.Close()");
				}
			}
		}
	}
}