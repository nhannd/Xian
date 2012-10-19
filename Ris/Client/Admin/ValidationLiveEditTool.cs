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
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Admin
{
    /// <summary>
    /// Tool that allows editing of validation rules on a live application component.
    /// </summary>
    [MenuAction("launch", "applicationcomponent-metacontextmenu/Edit Validation Rules", "Launch")]
	[ActionPermission("launch", ClearCanvas.Ris.Client.AuthorityTokens.Desktop.UIValidationRules)]
	[ExtensionOf(typeof(ApplicationComponentMetaToolExtensionPoint), FeatureToken = FeatureTokens.RIS.Core)]
    public class ValidationLiveEditTool : Tool<IApplicationComponentMetaToolContext>
    {
        private Shelf _shelf;

        public void Launch()
        {
            if (_shelf == null)
            {
                try
                {
                    ValidationEditorComponent editor = new ValidationEditorComponent(this.Context.Component);

                    _shelf = ApplicationComponent.LaunchAsShelf(
                        this.Context.DesktopWindow,
                        editor,
                        string.Format("{0} rules editor", this.Context.Component.GetType().Name),
                        ShelfDisplayHint.DockFloat);
                    _shelf.Closed += delegate { _shelf = null; };

                }
                catch (Exception e)
                {
                    // could not launch component
                    ExceptionHandler.Report(e, this.Context.DesktopWindow);
                }
            }
            else
            {
                _shelf.Activate();
            }
        }
    }
}
