using ClearCanvas.Desktop;
using ClearCanvas.Web.Common;

namespace ClearCanvas.Web.Services.View
{
    public interface IWebApplicationComponentView : IWebView, IApplicationComponentView
    {
    }

    public abstract class WebApplicationComponentView<TEntity> : WebView<TEntity>, IWebApplicationComponentView where TEntity : Entity, new()
    {
        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            SetModelObject(component);
        }

        #endregion
    }
}
