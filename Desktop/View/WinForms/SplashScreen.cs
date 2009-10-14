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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.Desktop.View.WinForms
{
	/// <summary>
	/// This class represents the splash screen form.  All access to its properties is 
	/// mitigated through Invoke to avoid cross-threading form exceptions.
	/// </summary>
    public partial class SplashScreen : Form
    {
		// Delegate instances used to call user interface functions from other threads
		public delegate void UpdateStatusTextDelegate(string statusText);
		public delegate void UpdateLicenseTextDelegate(string licenseText);
		public delegate void UpdateOpacityDelegate(double opacity);
		public delegate void AddAssemblyIconDelegate(Assembly pluginAssembly);
		
		private const int DropShadowOffset = 15;
		private BitmapOverlayForm _dropShadow = null;

		private const int IconWidth = 48;
		private const int IconHeight = 48;
		private const int IconPaddingX = 12;
		private const int IconPaddingY = 6;
		private const int IconTextHeight = 36;

		private Rectangle _pluginIconsRectangle = Rectangle.Empty;
		private int _nextIconPositionX = 0;
		private int _nextIconPositionY = 0;

        public SplashScreen()
        {
            InitializeComponent();

			Initialize();
		}

		#region Public Methods

		/// <summary>
		/// Updates the splash screen's status text.
		/// </summary>
		/// <param name="status">The splash screen new status text.</param>
		public void UpdateStatusText(string statusText)
		{
			if (this.Handle != IntPtr.Zero)
				Invoke(new UpdateStatusTextDelegate(SetStatusText), new Object[] { statusText });
			else
				SetStatusText(statusText);
		}

		/// <summary>
		/// Updates the splash screen's license text.
		/// </summary>
		/// <param name="status">The splash screen new license text.</param>
		public void UpdateLicenseText(string licenseText)
		{
			if (this.Handle != IntPtr.Zero)
				Invoke(new UpdateLicenseTextDelegate(SetLicenseText), new Object[] { licenseText });
			else
				SetLicenseText(licenseText);
		}

		/// <summary>
		/// Updates the splash screen's opacity.
		/// </summary>
		/// <param name="opacity">The splash screen's new opacity.</param>
		public void UpdateOpacity(double opacity)
		{
			if (this.Handle != IntPtr.Zero)
				Invoke(new UpdateOpacityDelegate(SetOpacity), new Object[] { opacity });
			else
				SetOpacity(opacity);
		}

		public void AddAssemblyIcon(Assembly pluginAssembly)
		{
			if (this.Handle != IntPtr.Zero)
				Invoke(new AddAssemblyIconDelegate(AddAssemblyIconToImage), new Object[] { pluginAssembly });
			else
				AddAssemblyIconToImage(pluginAssembly);
		}

		#endregion

		#region Private Methods

		private void Initialize()
		{
			// No status at first
			SetStatusText(string.Empty);

			// Initialize the version text to the executing assembly's
			_version.Text = String.Format(SplashScreenSettings.Default.VersionTextFormat, Product.GetVersion(true, true));
			_copyright.Text = Product.Copyright;
			_license.Text = Product.License;

			// Make the window completely transparent
			Opacity = 0;

			// Apply any splash screen settings, if requested
			if (SplashScreenSettings.Default.UseSplashScreenSettings)
			{
				this.SuspendLayout();

				Assembly assembly = Assembly.Load(SplashScreenSettings.Default.BackgroundImageAssemblyName);
				if (assembly != null)
				{
					string streamName = SplashScreenSettings.Default.BackgroundImageAssemblyName + "." + SplashScreenSettings.Default.BackgroundImageResourceName;
					Stream stream = assembly.GetManifestResourceStream(streamName);
					if (stream != null)
					{
						this.BackgroundImage = new Bitmap(stream);
						this.ClientSize = this.BackgroundImage.Size;
					}
				}

				this._status.Visible = SplashScreenSettings.Default.StatusVisible;
				this._status.Location = SplashScreenSettings.Default.StatusLocation;
				this._status.Size = SplashScreenSettings.Default.StatusSize;
				this._status.AutoSize = SplashScreenSettings.Default.StatusAutoSize;
				this._status.ForeColor = SplashScreenSettings.Default.StatusForeColor;
				this._status.Font = SplashScreenSettings.Default.StatusFontBold ? new Font(this._status.Font, FontStyle.Bold) : this._status.Font;
				this._status.TextAlign = SplashScreenSettings.Default.StatusTextAlign;

				this._copyright.Visible = SplashScreenSettings.Default.CopyrightVisible;
				this._copyright.Location = SplashScreenSettings.Default.CopyrightLocation;
				this._copyright.Size = SplashScreenSettings.Default.CopyrightSize;
				this._copyright.AutoSize = SplashScreenSettings.Default.CopyrightAutoSize;
				this._copyright.ForeColor = SplashScreenSettings.Default.CopyrightForeColor;
				this._copyright.Font = SplashScreenSettings.Default.CopyrightFontBold ? new Font(this._copyright.Font, FontStyle.Bold) : this._copyright.Font;
				this._copyright.TextAlign = SplashScreenSettings.Default.CopyrightTextAlign;

				this._version.Visible = SplashScreenSettings.Default.VersionVisible;
				this._version.Location = SplashScreenSettings.Default.VersionLocation;
				this._version.Size = SplashScreenSettings.Default.VersionSize;
				this._version.AutoSize = SplashScreenSettings.Default.VersionAutoSize;
				this._version.ForeColor = SplashScreenSettings.Default.VersionForeColor;
				this._version.Font = SplashScreenSettings.Default.VersionFontBold ? new Font(this._version.Font, FontStyle.Bold) : this._version.Font;
				this._version.TextAlign = SplashScreenSettings.Default.VersionTextAlign;

				this._license.Visible = SplashScreenSettings.Default.LicenseVisible;
				this._license.Location = SplashScreenSettings.Default.LicenseLocation;
				this._license.Size = SplashScreenSettings.Default.LicenseSize;
				this._license.AutoSize = SplashScreenSettings.Default.LicenseAutoSize;
				this._license.ForeColor = SplashScreenSettings.Default.LicenseForeColor;
				this._license.Font = SplashScreenSettings.Default.LicenseFontBold ? new Font(this._license.Font, FontStyle.Bold) : this._license.Font;
				this._license.TextAlign = SplashScreenSettings.Default.LicenseTextAlign;

				this._pluginIconsRectangle = SplashScreenSettings.Default.PluginIconsRectangle;
				this._nextIconPositionX = _pluginIconsRectangle.Left + IconPaddingX / 2;
				this._nextIconPositionY = _pluginIconsRectangle.Top;

				this.ResumeLayout();
			}
		}

		private void SetStatusText(string statusText)
		{
			_status.Text = statusText;
		}

		private void SetLicenseText(string licenseText)
		{
			_license.Text = licenseText;
		}

		private void SetOpacity(double opacity)
		{
			Opacity = opacity;

			// Pass the opacity to the drop shadow form if it exists
			if (_dropShadow != null && !_dropShadow.IsDisposed)
				_dropShadow.BitmapOpacity = opacity;
		}

		private void AddAssemblyIconToImage(Assembly pluginAssembly)
		{
			object[] pluginAttributes = pluginAssembly.GetCustomAttributes(typeof(PluginAttribute), false);

			foreach (PluginAttribute pluginAttribute in pluginAttributes)
			{
				if (!string.IsNullOrEmpty(pluginAttribute.Icon))
				{
					try
					{
						IResourceResolver resolver = new ResourceResolver(pluginAssembly);
						Bitmap icon = IconFactory.CreateIcon(pluginAttribute.Icon, resolver);

						// Burn the icon into the background image
						Graphics g = Graphics.FromImage(this.BackgroundImage);

						int positionX = _nextIconPositionX;
						int positionY = _nextIconPositionY;

						g.DrawImage(icon, positionX, positionY, IconWidth, IconHeight);

						// Burn the icon's name and version into the background image
						string pluginName = pluginAttribute.Name;
						string pluginVersion = string.Empty;
						try
						{
							FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(pluginAssembly.Location);
							pluginVersion = versionInfo.ProductVersion;
						}
						catch
						{
						}

						Font font = new Font("Tahoma", 10F, GraphicsUnit.Pixel);
						Brush brush = new SolidBrush(Color.FromArgb(50, 50, 50));

						StringFormat format = new StringFormat();
						format.Alignment = StringAlignment.Center;

						Rectangle layoutRect = new Rectangle(positionX - IconPaddingX / 2, positionY + IconHeight, IconWidth + IconPaddingX, IconTextHeight);

						g.DrawString(pluginName + "\n" + pluginVersion, font, brush, layoutRect, format);

						font.Dispose();
						brush.Dispose();

						g.Dispose();

						// Advance to the next icon position within the plugin rectangle
						_nextIconPositionX += (IconWidth + IconPaddingX);
						if (_nextIconPositionX + IconWidth + IconPaddingX / 2 > _pluginIconsRectangle.Right)
						{
							_nextIconPositionX = _pluginIconsRectangle.Left + IconPaddingX / 2;
							_nextIconPositionY += IconPaddingY + IconHeight + IconTextHeight;
						}

						this.Invalidate();
					}
					catch
					{
					}
				}
			}
		}

		private void DrawDropShadow(Graphics graphics, Rectangle rect, Color shadowColor, int shadowDepth, byte maxAlpha)
		{
			// Determine the shadow colors
			Color darkShadow = Color.FromArgb(maxAlpha, shadowColor);
			Color lightShadow = Color.FromArgb(0, shadowColor);

			// Create a brush that will create a softshadow circle
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddEllipse(0, 0, 2 * shadowDepth, 2 * shadowDepth);

			PathGradientBrush brush = new PathGradientBrush(graphicsPath);
			brush.CenterColor = darkShadow;
			brush.SurroundColors = new Color[] { lightShadow };

			// Generate a softshadow pattern that can be used to paint the shadow
			Bitmap pattern = new Bitmap(2 * shadowDepth, 2 * shadowDepth);

			Graphics patternGraphics = Graphics.FromImage(pattern);
			patternGraphics.FillEllipse(brush, 0, 0, 2 * shadowDepth, 2 * shadowDepth);

			patternGraphics.Dispose();
			brush.Dispose();

			// Top right corner
			graphics.DrawImage(pattern, new Rectangle(rect.Right - shadowDepth, rect.Top + shadowDepth, shadowDepth, shadowDepth), shadowDepth, 0, shadowDepth, shadowDepth, GraphicsUnit.Pixel);

			// Right side
			graphics.DrawImage(pattern, new Rectangle(rect.Right - shadowDepth, rect.Top + 2 * shadowDepth, shadowDepth, rect.Height - 3 * shadowDepth), shadowDepth, shadowDepth, shadowDepth, 1, GraphicsUnit.Pixel);

			// Bottom right corner
			graphics.DrawImage(pattern, new Rectangle(rect.Right - shadowDepth, rect.Bottom - shadowDepth, shadowDepth, shadowDepth), shadowDepth, shadowDepth, shadowDepth, shadowDepth, GraphicsUnit.Pixel);

			// Bottom side
			graphics.DrawImage(pattern, new Rectangle(rect.Left + 2 * shadowDepth, rect.Bottom - shadowDepth, rect.Width - 3 * shadowDepth, shadowDepth), shadowDepth, shadowDepth, 1, shadowDepth, GraphicsUnit.Pixel);

			// Bottom left corner
			graphics.DrawImage(pattern, new Rectangle(rect.Left + shadowDepth, rect.Bottom - shadowDepth, shadowDepth, shadowDepth), 0, shadowDepth, shadowDepth, shadowDepth, GraphicsUnit.Pixel);

			pattern.Dispose();
		}

		#endregion

		#region Private Event Handlers

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

			// Create the drop shadow bitmap given the size of the background image
			Bitmap dropShadowBmp = new Bitmap(this.BackgroundImage.Width + DropShadowOffset * 2, this.BackgroundImage.Height + DropShadowOffset * 2, PixelFormat.Format32bppArgb);

			Graphics graphics = Graphics.FromImage(dropShadowBmp);
			DrawDropShadow(graphics, new Rectangle(0, 0, dropShadowBmp.Width, dropShadowBmp.Height), Color.FromArgb(255, 0, 0, 0), DropShadowOffset, 96);
			graphics.Dispose();

			_dropShadow.Bitmap = dropShadowBmp;
			_dropShadow.BitmapOpacity = Opacity;

			// Make sure the splash screen and drop shadow are the frontmost windows when they appear
			BringToFront();
			_dropShadow.BringToFront();
		}

		#endregion
	}
}