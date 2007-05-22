using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="BiographyDemographicComponent"/>
    /// </summary>
    [ExtensionOf(typeof(BiographyDemographicComponentViewExtensionPoint))]
    public class BiographyDemographicComponentView : WinFormsView, IApplicationComponentView
    {
        private BiographyDemographicComponent _component;
        private BiographyDemographicComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (BiographyDemographicComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new BiographyDemographicComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
