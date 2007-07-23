using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace ClearCanvas.Controls.WinForms
{
	/// <summary>
	/// This class represents the splash screen form.  All access to its properties is 
	/// mitigated through Invoke to avoid cross-threading form exceptions.
	/// </summary>
    public partial class SplashScreen : Form
    {
		// Delegate instances used to call user interface functions from other threads
		public delegate void UpdateStatusDelegate(string status);
		public delegate void UpdateOpacityDelegate(double opacity);
		//public delegate void UpdateLicenseInfoDelegate(string licenseText);
		
		private const int DropShadowOffset = 20;
		private BitmapOverlayForm _dropShadow = null;

        public SplashScreen()
        {
            InitializeComponent();

            Opacity = 0;

			if (BackgroundImage != null)
				ClientSize = BackgroundImage.Size;
/*
            // Set the notice text
            Assembly asm = Assembly.GetEntryAssembly();
            string version = Application.ProductVersion.Trim();
            string companyName = Application.CompanyName.Trim();

            AssemblyDescriptionAttribute description;
            description = (AssemblyDescriptionAttribute)AssemblyDescriptionAttribute.GetCustomAttribute(
                    asm, typeof(AssemblyDescriptionAttribute));

            AssemblyCopyrightAttribute copyright;
            copyright = (AssemblyCopyrightAttribute)AssemblyCopyrightAttribute.GetCustomAttribute(
                        asm, typeof(AssemblyCopyrightAttribute));

			string notice = description.Description + " v." + version + "\r\n" + 
							copyright.Copyright + "\r\n" + 
							Properties.Resources.ContactInfo + "\r\n\r\n" + 
							Properties.Resources.Disclaimer;

			_notice.Text = notice;
*/
			// No status at first
			SetStatus(string.Empty);

			// A license likely hasn't been acquired at this point
			//SetLicenseInfo(Properties.Resources.AcquiringLicense);
        }

		/// <summary>
		/// Shows the splash screen as an about box, rather than a fade-in splash.
		/// </summary>
		/// <returns>A dialog result, which is always cancel at the moment.</returns>
		public DialogResult ShowAsAboutBox()
		{
			StartPosition = FormStartPosition.CenterScreen;
			Opacity = 1.0;

			//_help.Visible = true;
			//_close.Visible = true;
			_status.Visible = false;

			// Set the license info
			//SetLicenseInfo(LicenseText);
			
			return ShowDialog();
		}

		/// <summary>
		/// Updates the splash screen's status text.
		/// </summary>
		/// <param name="status">The splash screen new status text.</param>
		public void UpdateStatus(string status)
		{
			Invoke(new UpdateStatusDelegate(SetStatus), new Object[] { status });
		}

		/// <summary>
		/// Updates the splash screen's opacity.
		/// </summary>
		/// <param name="opacity">The splash screen's new opacity.</param>
		public void UpdateOpacity(double opacity)
		{
			Invoke(new UpdateOpacityDelegate(SetOpacity), new Object[] { opacity });
		}
/*
		/// <summary>
		/// Updates the splash screen's license information text.
		/// </summary>
		public void UpdateLicenseInfo()
		{
			Invoke(new UpdateLicenseInfoDelegate(SetLicenseInfo), new Object[] { LicenseText });
		}
*/
		private void SetStatus(string status)
		{
			_status.Text = status;
		}

		private void SetOpacity(double opacity)
		{
			Opacity = opacity;

			// Pass the opacity to the drop shadow form if it exists
			if (_dropShadow != null && !_dropShadow.IsDisposed)
				_dropShadow.BitmapOpacity = opacity;
		}
/*
		private void SetLicenseInfo(string licenseText)
		{
			_licenseInfo.Text = licenseText;
		}

		private string LicenseText
		{
			get
			{
				// Build the license info from the session manager
				string licenseText = "Licensed To:\n\n";

				if (!AegisSessionManager.LicenseAcquired)
					licenseText += "UNREGISTERED";
				else
				{
					if (AegisSessionManager.LicenseUser != string.Empty)
						licenseText += AegisSessionManager.LicenseUser + "\n";

					if (AegisSessionManager.LicenseUser != string.Empty)
						licenseText += AegisSessionManager.LicenseCompany + "\n";

					licenseText += "\n";

					if (AegisSessionManager.IsLicenseTimeLimited)
						licenseText += "Expiry: " + AegisSessionManager.LicenseExpiryDate + "\n";

					if (AegisSessionManager.IsLicenseRunLimited)
						licenseText += "Runs Left: " + AegisSessionManager.LicenseNumRunsLeft.ToString() + "\n";

					if (AegisSessionManager.IsLicenseMultiUser)
						licenseText += "Users: " + AegisSessionManager.LicenseNumUsersActive.ToString() + " of " + AegisSessionManager.LicenseNumUsersAllowed.ToString() + "\n";
					else
						licenseText += "Single User License";
				}

				return licenseText;
			}
		}
*/
		private void SplashScreen_Shown(object sender, EventArgs e)
		{
			// Create the drop shadow form when the splash screen is shown
			if (_dropShadow == null)
			{
				_dropShadow = new BitmapOverlayForm();

				_dropShadow.Owner = this;
				_dropShadow.TopMost = false;
				_dropShadow.ShowInTaskbar = false;

				// Show the drop shadow form
				_dropShadow.Show();
			}

			// Position the drop shadow form (has to be done after it's shown)
			_dropShadow.Top = this.Top - DropShadowOffset;
			_dropShadow.Left = this.Left - DropShadowOffset;
			
			// Pass the drop shadow bitmap to the drop shadow form (done last to avoid flickering redraws during the form's setup)
			_dropShadow.Bitmap = Properties.Resources.SplashShadow;
			_dropShadow.BitmapOpacity = Opacity;

			// Make sure the splash screen and drop shadow are the frontmost windows when they appear
			BringToFront();
			_dropShadow.BringToFront();
		}
/*
		private void _help_Click(object sender, EventArgs e)
		{
			Help.ShowHelp(this, "Aegis.chm", HelpNavigator.TableOfContents);
		}
 */ 
    }
}