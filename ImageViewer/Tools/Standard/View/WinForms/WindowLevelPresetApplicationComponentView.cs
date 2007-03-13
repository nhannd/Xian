using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="WindowLevelPresetApplicationComponent"/>
    /// </summary>
    [ExtensionOf(typeof(WindowLevelPresetApplicationComponentViewExtensionPoint))]
    public class WindowLevelPresetApplicationComponentView : WinFormsView, IApplicationComponentView
    {
        private WindowLevelPresetApplicationComponent _component;
        private WindowLevelPresetApplicationComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (WindowLevelPresetApplicationComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new WindowLevelPresetApplicationComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
