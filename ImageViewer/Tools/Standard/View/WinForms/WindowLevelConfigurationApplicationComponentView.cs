using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="WindowLevelConfigurationApplicationComponent"/>
    /// </summary>
    [ExtensionOf(typeof(WindowLevelConfigurationApplicationComponentViewExtensionPoint))]
    public class WindowLevelConfigurationApplicationComponentView : WinFormsView, IApplicationComponentView
    {
        private WindowLevelConfigurationApplicationComponent _component;
        private WindowLevelConfigurationApplicationComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (WindowLevelConfigurationApplicationComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new WindowLevelConfigurationApplicationComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
