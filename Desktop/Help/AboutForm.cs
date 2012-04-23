#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.Utilities.Manifest;

namespace ClearCanvas.Desktop.Help
{
	public partial class AboutForm : Form
	{
		public AboutForm()
		{
			this.SuspendLayout();

			InitializeComponent();

			_version.Text = String.Format(AboutSettings.Default.VersionTextFormat, ProductInformation.GetVersion(true, true, true));
			_copyright.Text = ProductInformation.Copyright;
			_license.Text = ProductInformation.License;
			_closeButton.Text = SR.LabelClose;

            _manifest.Visible = !ManifestVerification.Valid;
           
			if (AboutSettings.Default.UseSettings)
			{
				try
				{
					var stream = OpenResourceStream();
					if (stream != null)
					{
						// GDI+ resource management quirk: don't dispose the source stream (or create an independent copy of the bitmap)
						BackgroundImage = new Bitmap(stream);
						ClientSize = BackgroundImage.Size;
					}
				}
				catch (Exception ex)
				{
					Platform.Log(LogLevel.Warn, ex, "Failed to resolve about dialog resources.");
				}

				this._copyright.Location = AboutSettings.Default.CopyrightLocation;
				this._copyright.Size = AboutSettings.Default.CopyrightSize;
				this._copyright.AutoSize = AboutSettings.Default.CopyrightAutoSize;
				this._copyright.ForeColor = AboutSettings.Default.CopyrightForeColor;
				this._copyright.Font = AboutSettings.Default.CopyrightFontBold ? new Font(this._copyright.Font, FontStyle.Bold) : this._copyright.Font;
				this._copyright.TextAlign = AboutSettings.Default.CopyrightTextAlign;

				this._version.Location = AboutSettings.Default.VersionLocation;
				this._version.Size = AboutSettings.Default.VersionSize;
				this._version.AutoSize = AboutSettings.Default.VersionAutoSize;
				this._version.ForeColor = AboutSettings.Default.VersionForeColor;
				this._version.Font = AboutSettings.Default.VersionFontBold ? new Font(this._version.Font, FontStyle.Bold) : this._version.Font;
				this._version.TextAlign = AboutSettings.Default.VersionTextAlign;

				this._license.Visible = AboutSettings.Default.LicenseVisible;
				this._license.Location = AboutSettings.Default.LicenseLocation;
				this._license.Size = AboutSettings.Default.LicenseSize;
				this._license.AutoSize = AboutSettings.Default.LicenseAutoSize;
				this._license.ForeColor = AboutSettings.Default.LicenseForeColor;
				this._license.Font = AboutSettings.Default.LicenseFontBold ? new Font(this._license.Font, FontStyle.Bold) : this._license.Font;
				this._license.TextAlign = AboutSettings.Default.LicenseTextAlign;

                this._manifest.Location = AboutSettings.Default.ManifestLocation;
                this._manifest.Size = AboutSettings.Default.ManifestSize;
                this._manifest.AutoSize = AboutSettings.Default.ManifestAutoSize;
                this._manifest.ForeColor = AboutSettings.Default.ManifestForeColor;
				this._manifest.Font = AboutSettings.Default.ManifestFontBold ? new Font(this._manifest.Font, FontStyle.Bold) : this._manifest.Font;
                this._manifest.TextAlign = AboutSettings.Default.ManifestTextAlign;

			    AddLicenseLabels();

				this._closeButton.Anchor = AnchorStyles.Right | AnchorStyles.Top;
				this._closeButton.Location = AboutSettings.Default.CloseButtonLocation;
				this._closeButton.LinkColor = AboutSettings.Default.CloseButtonLinkColor;
			}

			this.ResumeLayout();

			this._closeButton.Click += new EventHandler(OnCloseClicked);
		}

        private void AddLicenseLabels()
        {
            // Add some labels that are controlled by license
            bool isEvaluation = false;
            bool expired = false;
            if (!LicenseInformation.IsLicenseInstalled())
            {
                isEvaluation = true;
                expired = true;
            }
            else if (LicenseInformation.IsEvaluation)
            {
                isEvaluation = true; 
                expired = LicenseInformation.Expired;
            }

            if (isEvaluation)
            {
                var evaluationLabel = new Label()
                 {
                    Text = expired ? SR.LabelEvaluationExpired : SR.LabelEvaluation,
                    Visible = AboutSettings.Default.EvaluationVisible,
                    BackColor = System.Drawing.Color.Transparent,
                    Location = AboutSettings.Default.EvaluationLocation,
                    Size = AboutSettings.Default.EvaluationSize,
                    AutoSize = AboutSettings.Default.EvaluationAutoSize,
                    ForeColor = AboutSettings.Default.EvaluationForeColor,
                    TextAlign = AboutSettings.Default.EvaluationTextAlign
                };
                    
                if (AboutSettings.Default.EvaluationFontBold)
                    evaluationLabel.Font = new Font(evaluationLabel.Font, FontStyle.Bold);

                this.Controls.Add(evaluationLabel);
            }


            if (LicenseInformation.DiagnosticUse != LicenseDiagnosticUse.Allowed)
            {
                var text = LicenseInformation.DiagnosticUse == LicenseDiagnosticUse.None
                               ? SR.LabelNotForClinicalUse
                               : SR.LabelNotForHumanDiagnosticUse;

                var notForDiagnosticUseLabel = new Label()
                {
                    Text = text,
                    Visible = AboutSettings.Default.NotForDiagnosticUseVisible,
                    BackColor = System.Drawing.Color.Transparent,
                    Location = AboutSettings.Default.NotForDiagnosticUseLocation,
                    Size = AboutSettings.Default.NotForDiagnosticUseSize,
                    AutoSize = AboutSettings.Default.NotForDiagnosticUseAutoSize,
                    ForeColor = AboutSettings.Default.NotForDiagnosticUseForeColor,
                    TextAlign = AboutSettings.Default.NotForDiagnosticUseTextAlign
                };
                if (AboutSettings.Default.NotForDiagnosticUseFontBold)
                    notForDiagnosticUseLabel.Font = new Font(notForDiagnosticUseLabel.Font, FontStyle.Bold);

                this.Controls.Add(notForDiagnosticUseLabel);

            }
        }

		private static Stream OpenResourceStream()
		{
			var oemPath = System.IO.Path.Combine(Platform.InstallDirectory, @"oem\about.png");
			if (File.Exists(oemPath))
				return File.OpenRead(oemPath);

			var assemblyName = AboutSettings.Default.BackgroundImageAssemblyName;
			var resourceName = AboutSettings.Default.BackgroundImageResourceName;
			if (!string.IsNullOrEmpty(assemblyName) && !string.IsNullOrEmpty(resourceName))
			{
				var assembly = Assembly.Load(assemblyName);
				if (assembly != null)
				{
					var streamName = assemblyName + @"." + resourceName;
					return assembly.GetManifestResourceStream(streamName);
				}
			}

			return null;
		}

		private void OnCloseClicked(object sender, EventArgs e)
		{
			Close();
		}
	}
}