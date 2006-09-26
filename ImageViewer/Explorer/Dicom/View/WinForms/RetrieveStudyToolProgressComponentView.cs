using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="RetrieveStudyToolProgressComponent"/>
    /// </summary>
    [ExtensionOf(typeof(RetrieveStudyToolProgressComponentViewExtensionPoint))]
    public class RetrieveStudyToolProgressComponentView : WinFormsView, IApplicationComponentView
    {
        private RetrieveStudyToolProgressComponent _component;
        private RetrieveStudyToolProgressComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (RetrieveStudyToolProgressComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new RetrieveStudyToolProgressComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
