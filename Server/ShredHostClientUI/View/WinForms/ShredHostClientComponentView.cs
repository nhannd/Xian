using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Server.ShredHostClientUI.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="ShredHostClientComponent"/>
    /// </summary>
    [ExtensionOf(typeof(ShredHostClientComponentViewExtensionPoint))]
    public class ShredHostClientComponentView : WinFormsView, IApplicationComponentView
    {
        private ShredHostClientComponent _component;
        private ShredHostClientComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (ShredHostClientComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new ShredHostClientComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
