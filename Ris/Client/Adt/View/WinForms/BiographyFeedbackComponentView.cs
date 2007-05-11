using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="BiographyFeedbackComponent"/>
    /// </summary>
    [ExtensionOf(typeof(BiographyFeedbackComponentViewExtensionPoint))]
    public class BiographyFeedbackComponentView : WinFormsView, IApplicationComponentView
    {
        private BiographyFeedbackComponent _component;
        private BiographyFeedbackComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (BiographyFeedbackComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new BiographyFeedbackComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
