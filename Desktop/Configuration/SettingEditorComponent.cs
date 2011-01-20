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
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.Desktop.Configuration
{
    /// <summary>
    /// Extension point for views onto <see cref="SettingEditorComponent"/>.
    /// </summary>
    [ExtensionPoint]
	public sealed class SettingEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// Used by the <see cref="SettingsManagementComponent"/> to show
    /// the default and current values of a setting, allowing the current value to be edited.
    /// </summary>
    [AssociateView(typeof(SettingEditorComponentViewExtensionPoint))]
    public class SettingEditorComponent : ApplicationComponent
    {
        private string _defaultValue;
        private string _currentValue;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SettingEditorComponent(string defaultValue, string currentValue)
        {
            _defaultValue = defaultValue;
            _currentValue = currentValue;
        }

        #region Presentation Model

		/// <summary>
		/// Gets the default setting value.
		/// </summary>
        public string DefaultValue
        {
            get { return _defaultValue; }
        }

		/// <summary>
		/// Gets or sets the current setting value.
		/// </summary>
        public string CurrentValue
        {
            get { return _currentValue; }
            set
            {
                _currentValue = value;
                this.Modified = true;
            }
        }

		/// <summary>
		/// The user has accepted the changes (if any).
		/// </summary>
        public void Accept()
        {
            this.ExitCode = ApplicationComponentExitCode.Accepted;
            this.Host.Exit();
        }

		/// <summary>
		/// The user has cancelled the changes (if any).
		/// </summary>
		public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.None;
            this.Host.Exit();
        }

        #endregion
    }
}
