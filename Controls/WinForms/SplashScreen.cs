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
		static SplashScreen m_SplashForm = null;
		static Thread m_Thread = null;

		// Fade in and out.
		private double m_dblOpacityIncrement = .05;
		private double m_dblOpacityDecrement = .08;

		// Status and progress bar
		static string ms_sStatus;

		static double m_TotalTime = 0;

		public SplashScreen()
		{
			InitializeComponent();
			this.Opacity = .00;
			m_Timer.Start();
			this.ClientSize = this.BackgroundImage.Size;
			Control.CheckForIllegalCrossThreadCalls = false;
		}

		// ************* Static Methods *************** //

		// A static method to create the thread and 
		// launch the SplashScreen.
		static public void ShowSplashScreen()
		{
			// Make sure it's only launched once.
			if (m_SplashForm != null)
				return;

			m_Thread = new Thread(new ThreadStart(SplashScreen.ShowForm));
			m_Thread.IsBackground = true;
			m_Thread.ApartmentState = ApartmentState.STA;
			m_Thread.Start();
		}

		// A property returning the splash screen instance
		static public SplashScreen SplashForm
		{
			get
			{
				return m_SplashForm;
			}
		}

		// A private entry point for the thread.
		static private void ShowForm()
		{
			m_SplashForm = new SplashScreen();
			Application.Run(m_SplashForm);
		}

		// A static method to close the SplashScreen
		static public void CloseForm()
		{
			if (m_SplashForm != null && m_SplashForm.IsDisposed == false)
			{
				// Make it start going away.
				m_SplashForm.m_dblOpacityIncrement = -m_SplashForm.m_dblOpacityDecrement;
			}
			m_Thread = null;	// we don't need these any more.
			//m_SplashForm = null;
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
		private void m_Timer_Tick(object sender, EventArgs e)
		{
			m_StatusLabel.Text = ms_sStatus;

			if (m_dblOpacityIncrement > 0)
			{
				if (this.Opacity < 1)
					this.Opacity += m_dblOpacityIncrement;

				//fade the form after 5 seconds...loading with native assemblies so hardly take any time at all...
				m_TotalTime += m_Timer.Interval;
				if (m_TotalTime > 5000)
					CloseForm();
			}
			else
			{
				if (this.Opacity > 0)
					this.Opacity += m_dblOpacityIncrement;
				else
				{
					this.Close();
					Debug.WriteLine("Called this.Close()");
				}
			}
		}
	}
}