#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;
using System.Drawing;
using ClearCanvas.Enterprise.Desktop.Login;
using ClearCanvas.Utilities.Manifest;

namespace ClearCanvas.Enterprise.Desktop.View.WinForms.Login
{
    public partial class LoginForm : Form
    {
        //private string[] _facilityChoices;
        private Point _refPoint;

        public LoginForm()
        {
            // Need to explicitely dismiss the splash screen here, as the login dialog is shown before the desktop window, which is normally
            // responsible for dismissing it.
#if !MONO
            SplashScreenManager.DismissSplashScreen(this);
#endif

            InitializeComponent();

            LoadProductSplashScreen();

            _manifest.Visible = !ManifestVerification.Valid;           
        }

        public void SetMode(LoginDialogMode mode)
        {
            _userName.Enabled = mode == LoginDialogMode.InitialLogin;
        }

        //public string[] FacilityChoices
        //{
        //    get { return _facilityChoices; }
        //    set
        //    {
        //        _facilityChoices = value;
        //        _domain.Items.Clear();
        //        _domain.Items.AddRange(_facilityChoices);
        //    }
        //}

        //public string SelectedFacility
        //{
        //    get { return (string)_domain.SelectedItem; }
        //    set
        //    {
        //        _domain.SelectedItem = value;
        //    }
        //}

        public string UserName
        {
            get { return _userName.Text; }
            set { _userName.Text = value; }
        }

        public string Password
        {
            get { return _password.Text; }
        }

        private void LoadProductSplashScreen()
        {
            this.SuspendLayout();

            if (SplashScreenSettings.Default.UseSplashScreenSettings)
            {
                Assembly assembly = Assembly.Load(SplashScreenSettings.Default.BackgroundImageAssemblyName);
                if (assembly != null)
                {
                    string streamName = SplashScreenSettings.Default.BackgroundImageAssemblyName + "." +
                                        SplashScreenSettings.Default.BackgroundImageResourceName;
                    Stream stream = assembly.GetManifestResourceStream(streamName);
                    if (stream != null)
                    {
                        this.BackgroundImage = new Bitmap(stream);
                        this.ClientSize = this.BackgroundImage.Size;
                    }
                }
            }

            this.ResumeLayout();
        }

        

        private void _loginButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            // depending on use-case, the username may already be filled in
            if (string.IsNullOrEmpty(_userName.Text))
                _userName.Select();
            else
                _password.Select();
        }

        private void _userName_TextChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
        }

        private void _password_TextChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
        }

        private void _facility_SelectedValueChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            bool ok = !string.IsNullOrEmpty(_userName.Text) && !string.IsNullOrEmpty(_password.Text);
            _loginButton.Enabled = ok;
        }

        private void LoginForm_MouseDown(object sender, MouseEventArgs e)
        {
            _refPoint = new Point(e.X, e.Y);
        }

        private void LoginForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += (e.X - _refPoint.X);
                this.Top += (e.Y - _refPoint.Y);
            }
        }
    }
}