#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Web.Common;

namespace ClearCanvas.Web.Services.View
{
    public interface IWebView : IEntityHandler, IView
    {
    }

    [GuiToolkit(ClearCanvas.Common.GuiToolkitID.Web)]
    public abstract class WebView : EntityHandler, IWebView
    {
        #region IView Members

        public string GuiToolkitID
        {
            get { return ClearCanvas.Common.GuiToolkitID.Web; }
        }

        public object GuiElement
        {
            get { throw new System.NotImplementedException(); }
        }

        #endregion
    }

    [GuiToolkit(ClearCanvas.Common.GuiToolkitID.Web)]
    public abstract class WebView<TEntity> : EntityHandler<TEntity>, IWebView where TEntity : Entity, new()
    {
        #region IView Members

        public string GuiToolkitID
        {
            get { return ClearCanvas.Common.GuiToolkitID.Web; }
        }

        public object GuiElement
        {
            get { throw new System.NotImplementedException(); }
        }

        #endregion
    }
}
