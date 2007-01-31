using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Desktop.Configuration.Standard;

namespace ClearCanvas.Desktop.View.WinForms.Configuration.Standard
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="DicomConfigurationApplicationComponent"/>
    /// </summary>
	[ExtensionOf(typeof(DateFormatApplicationComponentViewExtensionPoint))]
    public class DateFormatApplicationComponentView : WinFormsView, IApplicationComponentView
    {
        private DateFormatApplicationComponent _component;
        private DateFormatApplicationComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (DateFormatApplicationComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new DateFormatApplicationComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
