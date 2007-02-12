using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.Desktop.Configuration
{
    /// <summary>
    /// Extension point for views onto <see cref="SettingEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class SettingEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// SettingEditorComponent class
    /// </summary>
    [AssociateView(typeof(SettingEditorComponentViewExtensionPoint))]
    public class SettingEditorComponent : ApplicationComponent
    {
        private string _defaultValue;
        private string _currentValue;

        /// <summary>
        /// Constructor
        /// </summary>
        public SettingEditorComponent(string defaultValue, string currentValue)
        {
            _defaultValue = defaultValue;
            _currentValue = currentValue;
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        #region Presentation Model

        public string DefaultValue
        {
            get { return _defaultValue; }
        }

        public string CurrentValue
        {
            get { return _currentValue; }
            set
            {
                _currentValue = value;
                this.Modified = true;
            }
        }

        public void Accept()
        {
            this.ExitCode = ApplicationComponentExitCode.Normal;
            this.Host.Exit();
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            this.Host.Exit();
        }

        #endregion


    }
}
